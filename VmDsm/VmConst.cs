namespace VmDsm
{
    public enum OpAction
    {
        None,
        ReadInt,
        ReadFloat,
        ReadString,
        ReadIntInt,
        ReadIntString,

        //for specific
        ReadOn,
        ReadCOM,
        ReadRET
    }

    public struct CodeLine
    {
        public byte Magic;
        public byte Opcode;
        public byte Padding;
        public byte Type;
    }

    enum ByteType : byte
    {
        Language = 0x01,
        String = 0x02,
        Void = 0x04,
        Float = 0x08,
        Int = 0x10,

        Stack = 0x20,
        VarRef = 0x40,
        Valid = 0x80,

        //for 2E: DBVAR
        IntString = 0xD0,  //D0, C8
        ZeroInt = 0x80,
    }
}