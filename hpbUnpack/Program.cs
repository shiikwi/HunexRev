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

            var hpbbytes = File.ReadAllBytes(args[0]);
            var hphbytes = File.ReadAllBytes(hphPath);
            var un = new unas_FilePack();
            un.LoadAsset(hpbbytes, hphbytes);
            un.Unpack(hpbbytes, hphbytes, dir);
        }
    }

}
