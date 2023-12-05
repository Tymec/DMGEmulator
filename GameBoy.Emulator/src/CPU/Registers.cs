namespace GameBoy.Emulator.CPU;


public class Registers {
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

    public byte Get8(ERegister reg) {
        return reg switch {
            ERegister.A => A,
            ERegister.B => B,
            ERegister.C => C,
            ERegister.D => D,
            ERegister.E => E,
            ERegister.H => H,
            ERegister.L => L,
            ERegister.F => F,
            _ => throw new NotImplementedException($"Register {reg} not implemented")
        };
    }

    public ushort Get16(ERegister reg) {
        return reg switch {
            ERegister.AF => AF,
            ERegister.BC => BC,
            ERegister.DE => DE,
            ERegister.HL => HL,
            ERegister.SP => SP,
            ERegister.PC => PC,
            _ => throw new NotImplementedException($"Register {reg} not implemented")
        };
    }

    public bool GetFlag(EFlag flag) {
        return flag switch {
            EFlag.Zero => ZeroFlag,
            EFlag.Subtraction => SubtractionFlag,
            EFlag.HalfCarry => HalfCarryFlag,
            EFlag.Carry => CarryFlag,
            _ => throw new NotImplementedException($"Flag {flag} not implemented")
        };
    }

    public void Set8(ERegister reg, byte value) {
        switch (reg) {
            case ERegister.A:
                A = value;
                break;
            case ERegister.B:
                B = value;
                break;
            case ERegister.C:
                C = value;
                break;
            case ERegister.D:
                D = value;
                break;
            case ERegister.E:
                E = value;
                break;
            case ERegister.H:
                H = value;
                break;
            case ERegister.L:
                L = value;
                break;
            case ERegister.F:
                F = value;
                break;
            default:
                throw new NotImplementedException($"Register {reg} not implemented");
        }
    }

    public void Set16(ERegister reg, ushort value) {
        switch (reg) {
            case ERegister.AF:
                AF = value;
                break;
            case ERegister.BC:
                BC = value;
                break;
            case ERegister.DE:
                DE = value;
                break;
            case ERegister.HL:
                HL = value;
                break;
            case ERegister.SP:
                SP = value;
                break;
            case ERegister.PC:
                PC = value;
                break;
            default:
                throw new NotImplementedException($"Register {reg} not implemented");
        }
    }

    public void SetFlag(EFlag flag, bool value) {
        switch (flag) {
            case EFlag.Zero:
                ZeroFlag = value;
                break;
            case EFlag.Subtraction:
                SubtractionFlag = value;
                break;
            case EFlag.HalfCarry:
                HalfCarryFlag = value;
                break;
            case EFlag.Carry:
                CarryFlag = value;
                break;
            default:
                throw new NotImplementedException($"Flag {flag} not implemented");
        }
    }
}
