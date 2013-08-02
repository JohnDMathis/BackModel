using System;
using System.IO;

namespace BackModel
{
    class Program
    {
        public static bool Debug = false;
        static void Main(string[] args)
        {
#if (DEBUG)
            Debug=true;
#endif

            string source;
            string dest;
            if (Debug)
            {
                //var testsource = @"C:\Users\John\Documents\GitHub\integration\Kanban.Integration.ClientService\IntegrationService\API\Models\";
                //var testdest = @"C:\Users\John\Documents\GitHub\integration\Kanban.Integration.ClientService\IntegrationService\Site\generated\";
                var testsource = @"source\";
                var testdest = @"dest\";
                args = new[] { testsource, testdest};
            }

            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());

            var currentDir = Directory.GetCurrentDirectory();
            var thisDir = currentDir;
            while (Directory.GetFiles(thisDir, "*.csproj").Length==0)
            {
                thisDir = Directory.GetParent(thisDir).FullName;
            }
            var projectDir = thisDir;
            source = Path.Combine(projectDir, args[0]);//.Replace(@"\", @"\\"));
            dest = Path.Combine(projectDir, args[1]);//.Replace(@"\", @"\\"));
            
            Console.WriteLine("Source      : "+ source);
            Console.WriteLine("Destination : " + dest);
            if (File.Exists(source))
                ObjectToJs.Convert(source, dest);
            else
            {

                    // handle multiple files
                    var len = args.GetLength(0);
                    string file = "";
                    if(len>2)
                    {
                        for (int i = 2; i < len; i++)
                        {
                            file = source + args[i];
                            ObjectToJs.Convert(file, dest, args[i].Replace(".cs",".js"));
                        }
                    }
                    else
                    {
                        var files = Directory.GetFiles(source);
                        foreach (var item in files)
                        {
                            var pos = item.LastIndexOf("\\");
                            var fileName = item.Substring(pos + 1).Replace(".cs", ".js");
                            ObjectToJs.Convert(item, dest, fileName);
                        }
                    }
                }

            if (Debug)
            {
                Console.WriteLine("press a key...");
                Console.ReadKey();
            }
        }
    }
}
