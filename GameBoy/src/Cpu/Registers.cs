namespace GameBoy.Cpu;


using System.Runtime.InteropServices;


[StructLayout(LayoutKind.Explicit)]
public struct Registers {
    [FieldOffset(0)] public ushort AF;
    [FieldOffset(16)] public ushort BC;
    [FieldOffset(32)] public ushort DE;
    [FieldOffset(48)] public ushort HL;
    [FieldOffset(64)] public ushort SP;
    [FieldOffset(72)] public ushort PC;

    [FieldOffset(0)] public byte A;
    [FieldOffset(8)] public byte F;
    [FieldOffset(16)] public byte B;
    [FieldOffset(24)] public byte C;
    [FieldOffset(32)] public byte D;
    [FieldOffset(40)] public byte E;
    [FieldOffset(48)] public byte H;
    [FieldOffset(56)] public byte L;

    public bool ZeroFlag {
        readonly get => (F & 0x80) != 0;
        set => F = (byte)((F & 0x7F) | (value ? 0x80 : 0));
    }

    public bool SubtractionFlag {
        readonly get => (F & 0x40) != 0;
        set => F = (byte)((F & 0xBF) | (value ? 0x40 : 0));
    }

    public bool HalfCarryFlag {
        readonly get => (F & 0x20) != 0;
        set => F = (byte)((F & 0xDF) | (value ? 0x20 : 0));
    }

    public bool CarryFlag {
        readonly get => (F & 0x10) != 0;
        set => F = (byte)((F & 0xEF) | (value ? 0x10 : 0));
    }
}
