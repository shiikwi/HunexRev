using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptParser
{
    public class unas_LINK : ScriptLoader
    {
        private static int UNAS_COUNTER_NUM = 4;
        private static uint LINK_ID = 0x4C534548;  //HESL
        private static int LINK_HEAD_SIZE = 0x30;
        private struct Head
        {
            public uint ID;
            public uint Version;
            public uint Number;
            public uint MaxSize;
        }

        private struct LinkInfo
        {
            public string Name;
            public uint CRC;
            public uint Offset;
            public uint Size;
        }

        public void Unpack(byte[] data, string outDir)
        {
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                var header = new Head();
                header.ID = br.ReadUInt32();
                header.Version = br.ReadUInt32();
                header.Number = br.ReadUInt32();
                header.MaxSize = br.ReadUInt32();

                if (header.ID != LINK_ID)
                    throw new Exception($"Invalid Magic {header.ID}");

                //skip some redundant data
                br.BaseStream.Position = 0x14 + (UNAS_COUNTER_NUM - 1) * 4;
                uint NamePtrOffset = br.ReadUInt32();

                Console.WriteLine($"[*] Number: {header.Number}");
                Console.WriteLine($"[*] NameOffset: {NamePtrOffset}");

                long infoCr = LINK_HEAD_SIZE;
                long nameCr = NamePtrOffset;
                var infos = new List<LinkInfo>();

                for (int i = 0; i < header.Number; i++)
                {
                    br.BaseStream.Position = nameCr;
                    string name = ReadCString(br);
                    nameCr = br.BaseStream.Position;

                    br.BaseStream.Position = infoCr;
                    LinkInfo info = new LinkInfo();
                    info.Name = name;
                    info.CRC = br.ReadUInt32();
                    info.Offset = br.ReadUInt32();
                    info.Size = br.ReadUInt32();
                    infoCr += 16;

                    infos.Add(info);
                }

                foreach(var info in infos)
                {
                    var outdata = new byte[info.Size];
                    br.BaseStream.Position = info.Offset;
                    outdata = br.ReadBytes((int)info.Size);
                    var decoutdata = DecodeAsset(outdata);

                    string outpath = Path.Combine(outDir, info.Name);
                    if (!Directory.Exists(Path.GetDirectoryName(outpath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(outpath)!);
                    File.WriteAllBytes(outpath, decoutdata);
                    Console.WriteLine($"Export: {info.Name}");
                }
            }

        }

    }
}
