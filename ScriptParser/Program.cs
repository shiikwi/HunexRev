using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ScriptParser;

namespace hpbUpack
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine($"Usage: ScriptParser.exe <-h/-d> <heslnk/hdlg_file_Path>");
                return;
            }

            var mode = args[0];
            var path = args[1];

            if (mode == "-h")
            {
                string dir = Path.Combine(Path.GetDirectoryName(path)!, Path.GetFileNameWithoutExtension(path) + "Unpack");
                var linkdata = File.ReadAllBytes(path);
                var un = new unas_LINK();
                un.Unpack(linkdata, dir);
            }
            else if (mode == "-d")
            {
                var diagdata = File.ReadAllBytes(path);
                var outpath = Path.Combine(Path.GetDirectoryName(path)!, Path.GetFileNameWithoutExtension(path) + ".txt");
                var un = new unas_DIALOG();
                un.Export(diagdata, outpath);
            }
        }
    }

}
