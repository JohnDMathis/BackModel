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
                var testsource = @"C:\Users\John\Documents\GitHub\integration\Kanban.Integration.ClientService\IntegrationService\API\";
                var testdest = @"C:\Users\John\Documents\GitHub\integration\Kanban.Integration.ClientService\IntegrationService\Site\generated\";
                args = new[] { testsource, testdest, "Models.cs"};
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
                            ObjectToJs.Convert(file, dest, args[i].Replace(".cs",".js"));
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
