using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;

namespace ScriptParser
{
    public class unas_DIALOG : ScriptLoader
    {
        private static uint DIALOG_ID = 0x4C474448;  //HDGL
        private static int HEAD_SIZE = 0x20;

        private struct Head
        {
            public uint ID;
            public uint Version;
            public int Count;
            public int Offset;
        }

        public void Export(byte[] data, string outPath)
        {
            var decdata = DecodeAsset(data);
            //File.WriteAllBytes("decode.bin", decdata);
            using (var br = new BinaryReader(new MemoryStream(decdata)))
            {
                var header = new Head();
                header.ID = br.ReadUInt32();
                header.Version = br.ReadUInt32();
                header.Count = br.ReadInt32();
                header.Offset = br.ReadInt32();

                if (header.ID != DIALOG_ID)
                {
                    throw new Exception($"Invalid Magic {header.ID}");
                }

                br.BaseStream.Position = HEAD_SIZE;
                int[] addrs = new int[header.Count];
                for (int i = 0; i < header.Count; i++)
                {
                    addrs[i] = br.ReadInt32();
                }

                using (var wr = new StreamWriter(outPath, false, Encoding.UTF8))
                {
                    for (int i = 0; i < header.Count; i++)
                    {
                        int off = header.Offset + addrs[i];
                        br.BaseStream.Position = off;
                        string text = ReadCString(br).Replace("\n", "\\n");
                        wr.WriteLine($"*0x{off:X}* {text}");
                    }
                }

            }
            Console.WriteLine($"Export {Path.GetFileName(outPath)} Success.");
        }

    }
}
