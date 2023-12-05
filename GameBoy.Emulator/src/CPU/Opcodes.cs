namespace GameBoy.Emulator.CPU;

public partial class CPU {
    public int ExecuteUnprefixed(byte opcode) {
        switch (opcode) {
            case 0x00:
                return 4;   // NOP 4T
            case 0x01:
                reg.BC = Read16();
                return 12;  // LD BC,u16 12T
            case 0x02:
                Write8(reg.BC, reg.A);
                return 8;   // LD (BC),A 8T
            case 0x03:
                reg.BC++;
                return 8;   // INC BC 8T
            case 0x04:
                reg.B = OP_inc(reg.B);
                return 4;   // INC B 4T
            case 0x05:
                reg.B = OP_dec(reg.B);
                return 4;   // DEC B 4T
            case 0x06:
                reg.B = Read8();
                return 8;   // LD B,u8 8T
            case 0x07:
                return 4;   // RLCA 4T
            case 0x08:
                return 20;  // LD (u16),SP 20T
            case 0x09:
                return 8;   // ADD HL,BC 8T
            case 0x0A:
                return 8;   // LD A,(BC) 8T
            case 0x0B:
                reg.BC--;
                return 8;   // DEC BC 8T
            case 0x0C:
                reg.C = OP_inc(reg.C);
                return 4;   // INC C 4T
            case 0x0D:
                reg.C = OP_dec(reg.C);
                return 4;   // DEC C 4T
            case 0x0E:
                return 8;   // LD C,u8 8T
            case 0x0F:
                return 4;   // RRCA 4T
            case 0x10:
                reg.PC++;   // NOTE: Skip next byte
                halted = true;
                return 4;   // STOP 4T
            case 0x76:
                halted = true;
                return 4;   // HALT 4T
            case 0xF3:
                ime = false;
                imeScheduled = false;
                return 4;   // DI 4T
            case 0xFB:
                imeScheduled = true;
                return 4;   // EI 4T

            case 0xCB: throw new InvalidOperationException("Prefixed opcode called");

            default: throw new NotImplementedException($"Opcode {opcode:X4} not implemented");
        }
    }

    public int ExecutePrefixed(byte opcode) {
        switch (opcode) {
            default: throw new NotImplementedException($"Prefixed opcode {opcode:X4} not implemented");
        }
    }

    private byte OP_inc(byte x) {
        reg.SubtractionFlag = false;
        reg.ZeroFlag = false;

        int y = x + 1;
        if (y > 0xFF) {
            reg.ZeroFlag = true;
            y = 0;
        }

        return (byte)y;
    }

    private byte OP_dec(byte x) {
        return x;
    }

    private void OP_load8(ref byte dest, byte src) {
        dest = src;
    }

    private void OP_load16(ref ushort dest, ushort src) {
        dest = src;
    }

    private void OP_load16(ref ushort dest, byte lo, byte hi) {
        dest = (ushort)((hi << 8) | lo);
    }
}
