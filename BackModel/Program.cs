using System;
using System.IO;

namespace BackModel
{
    class Program
    {
        public static bool Debug = false;
        static void Main(string[] args)
        {
            string source;
            string dest;
            if (Debug)
            {
                var testsource = @"C:\Users\John\Documents\GitHub\celljournalist\enterprise.reboot\models\";
                var testdest = @"c:\users\john\documents\github\celljournalist\enterprise.reboot\client\generated\";
                args = new[] { testsource, testdest, "GalleryItemModel.cs", "GalleryModel.cs", "GalleryType.cs", "MyGalleryStatus.cs" };
            }
                source = args[0];
                dest = args[1];

            Console.WriteLine("Source   : "+source);
            Console.WriteLine("Folder : "+dest);
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
                            ObjectToJs.Convert(file, dest);
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
