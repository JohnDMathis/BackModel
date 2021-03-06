﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BackModel
{
    public enum FileType
    {
        Enum,
        Model
    }
    internal class ObjectToJs
    {
        private static string output = "";
        private static string _jsfilename = "";
        private static string _jsNamespace = "codegen.models";
        private static string _jsModelBase = "Backbone.Model";
        private const string NAMESPACE = "// Namespace:";
        private const string MODELBASE = "// ModelBase:";
        private const string END = "// End";

        private static FileType fileType;
        private static bool _skippingMethod = false;

        public static void Convert(string csFile, string jsFolder, string fileName=null)
        {
            // convert any enums found in 'csFile' to the same-named js file in 'jsFolder'
            // overwrite the js file if it exists

            // open csFile or throw exception
            output = "";

            if (fileName != null)
                _jsfilename = fileName;

            IEnumerable<string> lines=new List<string>();
            try
            {
                 lines = File.ReadLines(csFile);
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message);
            }

            bool inNamespace = false;
            bool inBody = false;
            int i;
            bool ignoreTheRest = false;
            foreach (var line in lines)
            {
                if (inNamespace)
                {
                    if (ignoreTheRest) continue;
                    
                    if(line.Contains(END))
                    {
                        ignoreTheRest = true;
                        continue;
                    }

                    // check for name space definition
                    if(line.Contains(NAMESPACE))
                    {
                        ParseNamespace(line.Trim());
                    }

                    // check for model base definition
                    if(line.Contains(MODELBASE))
                    {
                        ParseModelBase(line.Trim());
                    }

                    if(line.Contains("public enum"))
                    {
                        ParseDeclaration(line.Trim(), FileType.Enum);
                    } 
                    if(line.Contains("public class"))
                    {
                        ParseDeclaration(line.Trim(), FileType.Model);
                    }
                    if (inBody)
                    {
                        if (!_skippingMethod && IsEndOfBody(line))
                        {
                            inBody = false;
                            var lastComma = output.LastIndexOf(",");
                            if (lastComma > output.Length - 4)
                                output = output.Remove(lastComma);
                            if (fileType == FileType.Enum)
                                output += "\n};";
                            else
                                output += "\n});";
                            continue;
                        }
                        ParseLine(line.Trim());
                    }
                    if (!inBody && line.Contains("{"))
                        inBody = true;
                }
                if (!inNamespace && line.Contains("{"))
                    inNamespace = true;
            }
            if (Program.Debug)
                Console.WriteLine(output);
            if (!string.IsNullOrEmpty(output))
            {

                string fName = jsFolder +  _jsfilename;
                Console.WriteLine("Save to: " + fName);
                File.WriteAllText(fName, output);
            }
        }

        private static void ParseNamespace(string line)
        {
            var pos = line.IndexOf(NAMESPACE) + NAMESPACE.Length;
            _jsNamespace = line.Substring(pos);
        }

        private static void ParseModelBase(string line)
        {
            var pos = line.IndexOf(MODELBASE) + MODELBASE.Length;
            _jsModelBase = line.Substring(pos);
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static bool IsEndOfBody(string line)
        {
            if (fileType == FileType.Enum && line.Contains("}"))
                return true;
            if (fileType == FileType.Model && line.Trim() == "}")
                return true;
            return false;
        }


        private static void ParseDeclaration(string line, FileType type)
        {
            string name;
            fileType = type;
            var decl = type == FileType.Enum ? "enum" : "class";
            line = line
                .Replace("public", "")
                .Replace(decl, "")
                .TrimStart();
            var p = line.IndexOf(' ');
            if (p == -1)
                name = line;
            else
                name = line.Substring(0, p);

            if (_jsfilename == "") _jsfilename = name + ".js";


            output += "\n\n";


            if (fileType == FileType.Enum)
                output += string.Format("{0}.{1} = ", _jsNamespace, name);
            else
                output += string.Format("{0}.{1} = {2}.extend(", _jsNamespace, name, _jsModelBase);

            output += "{\n";
        }

        private static void ParseLine(string line)
        {
            if (fileType == FileType.Enum) ParseEnumLine(line);
            else ParseModelLine(line);
        }

        private static void ParseEnumLine(string line)
        {
            if (line.Length != 0 && line[0] == '[')
            {
                var rightBracketPos = line.IndexOf(']')+1;
                line = line.Substring(rightBracketPos).TrimStart();
                if (line.Length == 0) return;
            }
            line = line
                .Replace(" ","")
                .Replace("=",": ");

            if (line.Length > 0)
            {
                output += "   " + line + "\n";
            }
        }

        private static void ParseModelLine(string line)
        {
            if(line.Length==0) return;
            var start = line.IndexOf("public ", StringComparison.Ordinal);
            if(start==-1) return;

            // skip method definition
            if (_skippingMethod)
                _skippingMethod = false;
            else if (line.IndexOf("(", StringComparison.Ordinal) > 0)
            {
                _skippingMethod = true;
                return;
            }

            int startOfName;
            if (line.Contains("<") && line.Contains(">"))
                // must be a generic
                startOfName = line.LastIndexOf(">", StringComparison.Ordinal) + 2;
            else
                startOfName = line.IndexOf(' ', 7) + 1;

            var endOfName = line.IndexOf(' ', startOfName + 1);
            if (endOfName == -1) return;
            var propName = line.Substring(startOfName, (endOfName - startOfName));
            if (propName.IndexOfAny(new[] { '(', ')', '{', '}', ';' }) > -1) return;
            var template = "###:function (val, silent) { silent = silent ? { silent: true } : null; if (!_.isUndefined(val)) this.set('###', val, silent); else return this.get('###');},";
            var func = template.Replace("###", propName);
            output += "   " + func + "\n";
        }

    }
}


