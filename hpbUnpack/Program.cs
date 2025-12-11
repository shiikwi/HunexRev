using System;
using System.Collections.Generic;
using System.Text;

namespace hpbUpack
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine($"Usage: hpbUnpack.exe <hpb_File_Path>");
                return;
            }

            string dir = Path.Combine(Path.GetDirectoryName(args[0])!, Path.GetFileNameWithoutExtension(args[0]) + "Unpack");
            string hphPath = Path.Combine(Path.GetDirectoryName(args[0])!, Path.GetFileNameWithoutExtension(args[0]) + ".hph");
            var un = new unas_FilePack();
            un.Unpack(args[0], hphPath, dir);
        }
    }

}
