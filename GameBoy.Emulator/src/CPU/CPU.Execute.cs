namespace GameBoy.Emulator.CPU;

public partial class CPU {
    public int ExecuteUnprefixed(byte opcode) {
        // XXYYYZZZ -> xyz (in octal)
        var x = (opcode & 0b11000000) >> 6;
        var y = (opcode & 0b00111000) >> 3;
        var z = opcode & 0b00000111;
        var p = (opcode & 0b00110000) >> 4;
        var q = (opcode & 0b00001000) >> 3;

        var GetR = (int i) => i switch {
            0 => reg.B,
            1 => reg.C,
            2 => reg.D,
            3 => reg.E,
            4 => reg.H,
            5 => reg.L,
            6 => Read8(reg.HL),
            7 => reg.A,
            _ => throw new InvalidOperationException()
        };

        var SetR = (int i, byte v) => {
            switch (i) {
                case 0: reg.B = v; break;
                case 1: reg.C = v; break;
                case 2: reg.D = v; break;
                case 3: reg.E = v; break;
                case 4: reg.H = v; break;
                case 5: reg.L = v; break;
                case 6: Write8(reg.HL, v); break;
                case 7: reg.A = v; break;
                default: throw new InvalidOperationException();
            }
        };

        switch (x) {
            case 1:
                // HALT
                if (z == 6 && y == 6) {
                    halted = true;
                    return 4;
                }

                // LD r[y], r[z]
                SetR(y, GetR(z));
                return 4;
            case 2:
                // alu[y] r[z]
                return 0;
            default: throw new NotImplementedException($"Opcode {opcode:X4} not implemented");
        }
    }

    public int ExecuteUnprefixedOld(byte opcode) {
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
                reg.BC = OP_inc(reg.BC);
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
                reg.A = (byte)((reg.A >> 7) | (reg.A << 1));
                reg.ZeroFlag = false;
                reg.SubtractionFlag = false;
                reg.HalfCarryFlag = false;
                reg.CarryFlag = (reg.A & 0x01) == 0x01;
                return 4;   // RLCA 4T
            case 0x08:
                Write16(Read16(), reg.SP);
                return 20;  // LD (u16),SP 20T
            case 0x09:
                return 8;   // ADD HL,BC 8T
            case 0x0A:
                reg.A = Read8(reg.BC);
                return 8;   // LD A,(BC) 8T
            case 0x0B:
                reg.BC = OP_dec(reg.BC);
                return 8;   // DEC BC 8T
            case 0x0C:
                reg.C = OP_inc(reg.C);
                return 4;   // INC C 4T
            case 0x0D:
                reg.C = OP_dec(reg.C);
                return 4;   // DEC C 4T
            case 0x0E:
                reg.C = Read8();
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
}
