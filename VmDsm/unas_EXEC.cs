using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;

namespace VmDsm
{
    public class unas_EXEC
    {
        private static int EXEC_ID = 0x45534548; //HESE
        private static uint HEAD_SZIE;

        private struct Head
        {
            public uint ID;
            public uint Version;
            public uint StartAddr;
            public uint LabelCount;
            //public int[] m_counter;
            //public int[] m_begin;
            //public int[] m_reserved;
        }

        public byte[] bytecode;

        public void Parse(string inpath, string outpath)
        {
            var data = File.ReadAllBytes(inpath);
            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                var head = new Head
                {
                    ID = br.ReadUInt32(),
                    Version = br.ReadUInt32(),
                    StartAddr = br.ReadUInt32(),
                    LabelCount = br.ReadUInt32()
                };

                if (head.ID != EXEC_ID)
                    throw new Exception($"Invalid Magic {head.ID}");

                Console.WriteLine($"OpStart: {head.StartAddr}");

                br.BaseStream.Seek(head.StartAddr, SeekOrigin.Begin);
                bytecode = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
                HEAD_SZIE = head.StartAddr;
            }

            var vm = new VmProcess();
            vm.VmDsm((int)HEAD_SZIE, bytecode, outpath);
        }


    }
}
