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

public class OldRegisters {
    public byte A { get; set; }
    public byte B { get; set; }
    public byte C { get; set; }
    public byte D { get; set; }
    public byte E { get; set; }
    public byte H { get; set; }
    public byte L { get; set; }

    public ushort SP { get; set; }
    public ushort PC { get; set; }

    public bool ZeroFlag { get; set; }
    public bool SubtractionFlag { get; set; }
    public bool HalfCarryFlag { get; set; }
    public bool CarryFlag { get; set; }

    public byte F {
        get => (byte)((ZeroFlag ? 0x80 : 0) | (SubtractionFlag ? 0x40 : 0) | (HalfCarryFlag ? 0x20 : 0) | (CarryFlag ? 0x10 : 0));
        set {
            ZeroFlag = (value & 0x80) != 0;
            SubtractionFlag = (value & 0x40) != 0;
            HalfCarryFlag = (value & 0x20) != 0;
            CarryFlag = (value & 0x10) != 0;
        }
    }

    public ushort AF {
        get => (ushort)((A << 8) | F);
        set {
            A = (byte)(value >> 8);
            F = (byte)(value & 0xFF);
        }
    }

    public ushort BC {
        get => (ushort)((B << 8) | C);
        set {
            B = (byte)(value >> 8);
            C = (byte)(value & 0xFF);
        }
    }

    public ushort DE {
        get => (ushort)((D << 8) | E);
        set {
            D = (byte)(value >> 8);
            E = (byte)(value & 0xFF);
        }
    }

    public ushort HL {
        get => (ushort)((H << 8) | L);
        set {
            H = (byte)(value >> 8);
            L = (byte)(value & 0xFF);
        }
    }
}
