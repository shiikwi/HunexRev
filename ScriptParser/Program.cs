using System;
using System.Collections.Generic;
using System.Text;
using ScriptParser;

namespace hpbUpack
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine($"Usage: ScriptParser.exe <heslnk_file_Path>");
                return;
            }

            string dir = Path.Combine(Path.GetDirectoryName(args[0])!, Path.GetFileNameWithoutExtension(args[0]) + "Unpack");
            var linkdata = File.ReadAllBytes(args[0]);
            var un = new unas_LINK();
            un.Unpack(linkdata, dir);
        }
    }

}
