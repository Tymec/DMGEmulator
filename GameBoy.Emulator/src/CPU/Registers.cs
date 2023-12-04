namespace GameBoy.Emulator.CPU;


public class Registers {
    public byte A { get; set; }
    public byte B { get; set; }
    public byte C { get; set; }
    public byte D { get; set; }
    public byte E { get; set; }
    public byte H { get; set; }
    public byte L { get; set; }

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
