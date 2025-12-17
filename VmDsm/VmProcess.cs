using System.Text;

namespace VmDsm
{
    public class VmProcess
    {
        private BinaryReader br;
        private StreamWriter wr;

        public void VmDsm(int offset, byte[] data, string outpath)
        {
            using (var ms = new MemoryStream(data))
            using (br = new BinaryReader(ms))
            //using (var er = new StreamWriter("error.log", false, new UTF8Encoding(false)))
            using (wr = new StreamWriter(outpath, false, new UTF8Encoding(false)))
            {
                while (br.BaseStream.Position < ms.Length)
                {
                    var currentPos = br.BaseStream.Position + offset;

                    //check op
                    byte magic = br.ReadByte();
                    br.BaseStream.Position--;
                    if (magic != 0xA0)
                    {
                        byte b = br.ReadByte();
                        Console.WriteLine($"[Warn] {currentPos:X} opcode parse error");
                        continue;
                    }

                    //read op
                    var code = new CodeLine()
                    {
                        Magic = br.ReadByte(),
                        Opcode = br.ReadByte(),
                        Padding = br.ReadByte(),
                        Type = br.ReadByte()
                    };

                    string FuncName = "";
                    OpAction action = OpAction.None;

                    ExecuteVm(ref FuncName, code, ref action);
                    FormatWrite(currentPos, code, FuncName, action);
                }
            }
        }

        private void ExecuteVm(ref string funcName, CodeLine code, ref OpAction action)
        {
            switch (code.Opcode)
            {
                case 0x00: funcName = "Off"; action = OpAction.None; break;
                case 0x01: funcName = "INC"; action = OpAction.None; break;
                case 0x02: funcName = "DEC"; action = OpAction.None; break;
                case 0x03: funcName = "NEG"; action = OpAction.None; break;
                case 0x04: funcName = "NOT"; action = OpAction.None; break;
                case 0x05: funcName = "ADD"; action = OpAction.None; break;
                case 0x06: funcName = "SUB"; action = OpAction.None; break;
                case 0x07: funcName = "MUL"; action = OpAction.None; break;
                case 0x08: funcName = "DIV"; action = OpAction.None; break;
                case 0x09: funcName = "MOD"; action = OpAction.None; break;
                case 0x0A: funcName = "AND"; action = OpAction.None; break;
                case 0x0B: funcName = "OR"; action = OpAction.None; break;
                case 0x0C: funcName = "XOR"; action = OpAction.None; break;
                case 0x0D: funcName = "REV"; action = OpAction.None; break;
                case 0x0E: funcName = "SAL"; action = OpAction.None; break;
                case 0x0F: funcName = "SAR"; action = OpAction.None; break;
                case 0x10: funcName = "LSS"; action = OpAction.None; break;
                case 0x11: funcName = "LEQ"; action = OpAction.None; break;
                case 0x12: funcName = "GRT"; action = OpAction.None; break;
                case 0x13: funcName = "GEQ"; action = OpAction.None; break;
                case 0x14: funcName = "EQU"; action = OpAction.None; break;
                case 0x15: funcName = "NEQ"; action = OpAction.None; break;
                case 0x16: funcName = "AND2"; action = OpAction.None; break;
                case 0x17: funcName = "OR2"; action = OpAction.None; break;
                case 0x18: funcName = "CAST"; action = OpAction.ReadInt; break;
                case 0x19: funcName = "CALL"; action = OpAction.ReadIntString; break;
                case 0x1A: funcName = "FUNC"; action = OpAction.ReadInt; break;
                case 0x1B: funcName = "COM"; action = OpAction.ReadCOM; break;
                case 0x1C: funcName = "DEL"; action = OpAction.ReadInt; break;
                case 0x1D: funcName = "JMP"; action = OpAction.ReadInt; break;
                case 0x1E: funcName = "JPT"; action = OpAction.ReadInt; break;
                case 0x1F: funcName = "JPF"; action = OpAction.ReadInt; break;
                case 0x20: funcName = "EQCMP"; action = OpAction.ReadInt; break;
                case 0x21: funcName = "LOD"; action = OpAction.ReadInt; break;
                case 0x22: funcName = "LDA"; action = OpAction.ReadInt; break;
                case 0x23: funcName = "LDI"; action = OpAction.ReadInt; break;
                case 0x24: funcName = "STO"; action = OpAction.ReadInt; break;
                case 0x25: funcName = "ADBR"; action = OpAction.ReadInt; break;
                case 0x26: funcName = "ADSP"; action = OpAction.ReadInt; break;
                case 0x27: funcName = "RET"; action = OpAction.ReadRET; break;
                case 0x28: funcName = "ASS"; action = OpAction.None; break;
                case 0x29: funcName = "ASSV"; action = OpAction.None; break;
                case 0x2A: funcName = "VAL"; action = OpAction.ReadInt; break;
                case 0x2B: funcName = "UNK_2B"; action = OpAction.None; break;
                case 0x2C: funcName = "Check"; action = OpAction.None; break;
                case 0x2D: funcName = "On"; action = OpAction.ReadOn; break;
                case 0x2E: funcName = "DBVAR"; action = code.Type == (byte)ByteType.ZeroInt ? OpAction.ReadInt : OpAction.ReadIntString; break;

                default:
                    Console.WriteLine($"[Warn] Unknown opcode: {code.Opcode}");
                    if ((code.Type & (byte)ByteType.Int) != 0) action = OpAction.ReadInt;
                    else if ((code.Type & (byte)ByteType.Float) != 0) action = OpAction.ReadFloat;
                    else if ((code.Type & (byte)ByteType.String) != 0) action = OpAction.ReadString;
                    break;
            }
        }

        private void FormatWrite(long offset, CodeLine code, string funcname, OpAction action)
        {
            var datastr = new List<string>();

            switch (action)
            {
                case OpAction.None:
                    break;

                case OpAction.ReadInt:
                case OpAction.ReadFloat:
                    datastr.Add($"{br.ReadUInt32()}");
                    break;

                case OpAction.ReadString:
                    datastr.Add(ReadCString());
                    break;

                case OpAction.ReadIntInt:
                    datastr.Add($"{br.ReadUInt32()}");
                    datastr.Add($"{br.ReadUInt32()}");
                    break;

                case OpAction.ReadIntString:
                    datastr.Add($"{br.ReadUInt32()}");
                    datastr.Add(ReadCString());
                    break;

                case OpAction.ReadOn:
                    ReadOn(code, ref datastr);
                    break;

                case OpAction.ReadCOM:
                    ReadCOM(ref datastr);
                    break;

                case OpAction.ReadRET:
                    ReadRET(ref datastr, offset);
                    break;
            }

            var str = string.Join(", ", datastr);
            string fmtdatastr = datastr.Count == 0
                                ? $"{funcname}({code.Padding}, {code.Type})"
                                : $"{funcname}({code.Padding}, {code.Type}, {str})";
            string outLine = $"◇0x{offset:X8}◇ " + fmtdatastr;

            wr.WriteLine(outLine);
        }

        private string ReadCString()
        {
            var bytes = new List<byte>();
            while (true)
            {
                byte b = br.ReadByte();
                if (b == 0) break;
                bytes.Add(b);
            }
            var result = Encoding.UTF8.GetString(bytes.ToArray());

            long remainder = br.BaseStream.Position % 4;
            if (remainder != 0)
            {
                int skip = (int)(4 - remainder);
                br.ReadBytes(skip);
            }
            return result;
        }

        private void ReadOn(CodeLine code, ref List<string> str)
        {
            //A0 2D string
            if (code.Padding != 0 && code.Type != 0)
            {
                code.Padding = 0;
                code.Type = 0;
                br.BaseStream.Position -= 2;
                str.Add(ReadCString());
            }
            //else
            //{
            //A0 2D 00 00
            //}
        }

        private void ReadCOM(ref List<string> str)
        {
            var flag = br.ReadUInt32();
            if (flag != 0x01)
            {
                str.Add($"{flag}");
            }
            else
            {
                str.Add($"{flag}");
                str.Add($"{br.ReadUInt32()}");
                str.Add($"{br.ReadUInt32()}");
            }
        }

        private void ReadRET(ref List<string> str, long curroff)
        {
            var remainLength = br.BaseStream.Length - br.BaseStream.Position;
            bool eofflag = false;
            if (remainLength < 4)
            {
                var remaindata = br.ReadBytes((int)remainLength);
                eofflag = Array.TrueForAll(remaindata, b => b == 0);
            }
            else
            {
                eofflag = br.ReadUInt32() == 0 ? true : false;
            }

            if (!eofflag)  //OpAction.None
            {
                br.BaseStream.Position -= 4;
            }
            else  //File end
            {
                var eofpara = br.ReadBytes((int)remainLength);
                if (Array.TrueForAll(eofpara, b => b == 0))
                {
                    str.Add("0");
                }
                else
                {
                    Console.WriteLine($"[Warn] 0x{curroff:X} Parse 0x27:RET Error");
                }
            }
        }

    }
}