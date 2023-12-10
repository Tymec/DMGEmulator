namespace GameBoy.CPU;

public partial class CPU {
    public int ExecuteUnprefixedMod(byte opcode) {
        // XXYYYZZZ -> xyz (in octal)
        var x = (opcode & 0b11000000) >> 6;
        var y = (opcode & 0b00111000) >> 3;
        var z = opcode & 0b00000111;
        var p = (y & 0b000110) >> 1;
        var q = y & 0b000001;

        byte GetR(int i) => i switch {
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

        void SetR(int i, byte v) {
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
        }

        ushort GetRP(int i, bool prim = true) => i switch {
            0 => reg.BC,
            1 => reg.DE,
            2 => reg.HL,
            3 when prim => reg.SP,
            3 when !prim => reg.AF,
            _ => throw new InvalidOperationException()
        };

        void SetRP(int i, ushort v, bool prim = true) {
            switch (i) {
                case 0: reg.BC = v; break;
                case 1: reg.DE = v; break;
                case 2: reg.HL = v; break;
                case 3 when prim: reg.SP = v; break;
                case 3 when !prim: reg.AF = v; break;
                default: throw new InvalidOperationException();
            }
        }

        Action<byte> GetALU(int i) => i switch {
            0 => (x) => OP_add(x, false),   // ADD A, x
            1 => (x) => OP_add(x, true),    // ADC A, x
            2 => (x) => OP_sub(x, false),   // SUB A, x
            3 => (x) => OP_sub(x, true),    // SBC A, x
            4 => OP_and,                    // AND A, x
            5 => OP_xor,                    // XOR A, x
            6 => OP_or,                     // OR A, x
            7 => OP_cp,                     // CP A, x
            _ => throw new InvalidOperationException()
        };

        switch (x) {
            case 0:
                switch (z) {
                    case 1 when q == 0:
                        // LD rp[p], nn
                        SetRP(p, Read16());
                        return 12;
                    case 1 when q == 1:
                        // ADD HL, rp[p]
                        OP_add(GetRP(p));
                        return 8;
                    case 2 when q == 0 && p == 0:
                        break; // LD (BC), A
                    case 2 when q == 0 && p == 1:
                        break; // LD (DE), A
                    case 2 when q == 0 && p == 2:
                        break; // LD (HL+), A
                    case 2 when q == 0 && p == 3:
                        break; // LD (HL-), A
                    case 2 when q == 1 && p == 0:
                        break; // LD A, (BC)
                    case 2 when q == 1 && p == 1:
                        break; // LD A, (DE)
                    case 2 when q == 1 && p == 2:
                        break; // LD A, (HL+)
                    case 2 when q == 1 && p == 3:
                        break; // LD A, (HL-)
                    case 3 when q == 0:
                        // INC rp[p]
                        SetRP(p, OP_inc(GetRP(p)));
                        return 8;
                    case 3 when q == 1:
                        // DEC rp[p]
                        SetRP(p, OP_dec(GetRP(p)));
                        return 8;
                    case 4:
                        // INC r[y]
                        SetR(y, OP_inc(GetR(y)));
                        return 4;
                    case 5:
                        // DEC r[y]
                        SetR(y, OP_dec(GetR(y)));
                        return 4;
                    case 6:
                        // LD r[y], n
                        SetR(y, Read8());
                        return 8;
                    case 7 when y == 0:
                        // RLCA
                        OP_rlca(reverse: false, throughCarry: false);
                        return 4;
                    case 7 when y == 1:
                        // RRCA
                        OP_rlca(reverse: true, throughCarry: false);
                        return 4;
                    case 7 when y == 2:
                        // RLA
                        OP_rlca(reverse: false, throughCarry: true);
                        return 4;
                    case 7 when y == 3:
                        // RRA
                        OP_rlca(reverse: true, throughCarry: true);
                        return 4;
                    case 7 when y == 4:
                        // DAA
                        return 4;
                    case 7 when y == 5:
                        // CPL
                        return 4;
                    case 7 when y == 6:
                        // SCF
                        return 4;
                    case 7 when y == 7:
                        // CCF
                        return 4;
                    default: throw new NotImplementedException($"Opcode {opcode:X4} not implemented");
                }
                break;
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
                GetALU(y)(GetR(z));
                return 4;
            case 3:
                throw new NotImplementedException($"Opcode {opcode:X4} not implemented");
            default: throw new NotImplementedException($"Opcode {opcode:X4} not implemented");
        }

        throw new NotImplementedException($"Opcode {opcode:X4} not implemented");
    }
}
