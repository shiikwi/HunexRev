using System;
using System.Collections.Generic;
using System.Text;
using Compress;

namespace hpbUpack
{
    public class unas_FilePack
    {
        private static uint m_ID = 0x43415048; //HPAC
        private static int HEAD_SIZE = 0x20;
        private static int TABLE_SIZE = 0X20;

        private struct Header
        {
            public uint ID;
            public uint Version;
            public int Count;
            public uint Offset;
            public uint NameOffset;
            //public uint[] Padding;
        }

        private struct Table
        {
            public long Offset;
            public uint Key;
            public uint FileSize;
            public uint MeltSize;
            public uint FileCRC;
            public uint MeltCRC;
            //public uint[] Padding;
        }

        private class Entry
        {
            public string FileName;
            public Table Info;
        }

        public void LoadAsset(byte[] hpb, byte[] hph)
        {
            void Check(byte[] data)
            {
                if(BitConverter.ToUInt32(data, 0) == unas_LZH.ID)
                {
                    var lzh = new unas_LZH();
                    lzh.Decode(hpb);
                }
                else if(BitConverter.ToUInt32(data, 0) == unas_LZS.ID)
                {
                    var lzs = new unas_LZS();
                    lzs.Decode(hpb);
                }
            }

            Check(hpb);
            Check(hph);
        }

        public void Unpack(byte[] hpb, byte[] hph, string outDir)
        {
            if (!Path.Exists(outDir)) Directory.CreateDirectory(outDir);

            var FileEntries = new List<Entry>();
            using (var br = new BinaryReader(new MemoryStream(hph)))
            {
                Header header = new Header();
                header.ID = br.ReadUInt32();
                header.Version = br.ReadUInt32();
                header.Count = br.ReadInt32();
                header.Offset = br.ReadUInt32();
                header.NameOffset = br.ReadUInt32();

                if (header.ID != m_ID)
                {
                    throw new Exception($"Invalid Magic {header.ID}");
                }

                Console.WriteLine($"[*] FileCount: {header.Count}");
                Console.WriteLine($"[*] NameOffset: {header.NameOffset}");

                long table_cr = HEAD_SIZE;
                long name_cr = header.NameOffset;

                for (int i = 0; i < header.Count; i++)
                {
                    var entry = new Entry();
                    br.BaseStream.Position = name_cr;
                    entry.FileName = ReadCString(br);
                    name_cr = br.BaseStream.Position;

                    br.BaseStream.Position = table_cr;
                    entry.Info = new Table
                    {
                        Offset = br.ReadInt64(),
                        Key = br.ReadUInt32(),
                        FileSize = br.ReadUInt32(),
                        MeltSize = br.ReadUInt32(),
                        FileCRC = br.ReadUInt32(),
                        MeltCRC = br.ReadUInt32(),
                    };
                    table_cr += TABLE_SIZE;

                    FileEntries.Add(entry);
                }
            }

            using (var br = new BinaryReader(new MemoryStream(hpb)))
            {
                foreach(var entry in FileEntries)
                {
                    br.BaseStream.Position = entry.Info.Offset;
                    byte[] data = br.ReadBytes((int)entry.Info.FileSize);

                    string outpath = Path.Combine(outDir, entry.FileName);
                    if(!Directory.Exists(Path.GetDirectoryName(outpath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(outpath)!);
                    File.WriteAllBytes(outpath, data);
                    Console.WriteLine($"Export: {entry.FileName}");
                }
            }

        }

        private string ReadCString(BinaryReader br)
        {
            var bytes = new List<byte>();
            while (true)
            {
                byte b = br.ReadByte();
                if (b == 0) break;
                bytes.Add(b);
            }
            return Encoding.UTF8.GetString(bytes.ToArray());
        }

    }
}
