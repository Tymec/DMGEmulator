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

    public ushort this[ERegisterPair pair] {
        readonly get => pair switch {
            ERegisterPair.AF => AF,
            ERegisterPair.BC => BC,
            ERegisterPair.DE => DE,
            ERegisterPair.HL => HL,
            ERegisterPair.SP => SP,
            ERegisterPair.PC => PC,
            _ => throw new System.NotImplementedException()
        };
        set {
            switch (pair) {
                case ERegisterPair.AF:
                    AF = value;
                    break;
                case ERegisterPair.BC:
                    BC = value;
                    break;
                case ERegisterPair.DE:
                    DE = value;
                    break;
                case ERegisterPair.HL:
                    HL = value;
                    break;
                case ERegisterPair.SP:
                    SP = value;
                    break;
                case ERegisterPair.PC:
                    PC = value;
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }
    }

    public byte this[ERegister r] {
        readonly get => r switch {
            ERegister.A => A,
            ERegister.B => B,
            ERegister.C => C,
            ERegister.D => D,
            ERegister.E => E,
            ERegister.H => H,
            ERegister.L => L,
            ERegister.F => F,
            _ => throw new System.NotImplementedException()
        };
        set {
            switch (r) {
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
                    throw new System.NotImplementedException();
            }
        }
    }
}
