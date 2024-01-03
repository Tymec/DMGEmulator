namespace GameBoy.Cpu;

public class OpcodeBuilder(CPU cpu) {
    public static readonly Opcode INVALID_OPCODE = new("INVALID", 0, 0, () => {
        throw new InvalidOperationException($"Invalid opcode");
    });

    private byte GetReg(int index) {
        return index switch {
            0 => cpu.Reg.B,
            1 => cpu.Reg.C,
            2 => cpu.Reg.D,
            3 => cpu.Reg.E,
            4 => cpu.Reg.H,
            5 => cpu.Reg.L,
            6 => cpu.Read(cpu.Reg.HL),
            7 => cpu.Reg.A,
            _ => throw new InvalidOperationException($"Invalid register index {index}")
        };
    }

    private void SetReg(int index, byte value) {
        switch (index) {
            case 0:
                cpu.Reg.B = value;
                break;
            case 1:
                cpu.Reg.C = value;
                break;
            case 2:
                cpu.Reg.D = value;
                break;
            case 3:
                cpu.Reg.E = value;
                break;
            case 4:
                cpu.Reg.H = value;
                break;
            case 5:
                cpu.Reg.L = value;
                break;
            case 6:
                cpu.Write(cpu.Reg.HL, value);
                break;
            case 7:
                cpu.Reg.A = value;
                break;
            default:
                throw new InvalidOperationException($"Invalid register index {index}");
        }
    }

    private static string GetRegName(int index) {
        return index switch {
            0 => "B",
            1 => "C",
            2 => "D",
            3 => "E",
            4 => "H",
            5 => "L",
            6 => "(HL)",
            7 => "A",
            _ => throw new InvalidOperationException($"Invalid register index {index}")
        };
    }

    private ushort GetRegPair(int index, bool alt = false) {
        return index switch {
            0 => cpu.Reg.BC,
            1 => cpu.Reg.DE,
            2 => cpu.Reg.HL,
            3 => alt ? cpu.Reg.AF : cpu.Reg.SP,
            _ => throw new InvalidOperationException($"Invalid register pair index {index}")
        };
    }

    private void SetRegPair(int index, ushort value, bool alt = false) {
        switch (index) {
            case 0:
                cpu.Reg.BC = value;
                break;
            case 1:
                cpu.Reg.DE = value;
                break;
            case 2:
                cpu.Reg.HL = value;
                break;
            case 3:
                if (alt) {
                    cpu.Reg.AF = value;
                } else {
                    cpu.Reg.SP = value;
                }
                break;
            default:
                throw new InvalidOperationException($"Invalid register pair index {index}");
        }
    }

    private static string GetRegPairName(int index, bool alt = false) {
        return index switch {
            0 => "BC",
            1 => "DE",
            2 => "HL",
            3 => alt ? "AF" : "SP",
            _ => throw new InvalidOperationException($"Invalid register pair index {index}")
        };
    }

    private bool GetCC(int index) {
        return index switch {
            0 => !cpu.Reg.ZeroFlag,     // NZ
            1 => cpu.Reg.ZeroFlag,      // Z
            2 => !cpu.Reg.CarryFlag,    // NC
            3 => cpu.Reg.CarryFlag,     // C
            _ => throw new InvalidOperationException($"Invalid condition code index {index}")
        };
    }

    private static string GetCCName(int index) {
        return index switch {
            0 => "NZ",  // NZ
            1 => "Z",   // Z
            2 => "NC",  // NC
            3 => "C",   // C
            _ => throw new InvalidOperationException($"Invalid condition code index {index}")
        };
    }

    private Action GetAlu(int index) {
        return index switch {
            0 => null, // ADD
            1 => null, // ADC
            2 => null, // SUB
            3 => null, // SBC
            4 => null, // AND
            5 => null, // XOR
            6 => null, // OR
            7 => null, // CP
            _ => throw new InvalidOperationException($"Invalid ALU index {index}")
        };
    }

    private static string GetAluName(int index) {
        return index switch {
            0 => "ADD",
            1 => "ADC",
            2 => "SUB",
            3 => "SBC",
            4 => "AND",
            5 => "XOR",
            6 => "OR",
            7 => "CP",
            _ => throw new InvalidOperationException($"Invalid ALU index {index}")
        };
    }

    private Action GetRot(int index) {
        return index switch {
            0 => null, // RLC
            1 => null, // RRC
            2 => null, // RL
            3 => null, // RR
            4 => null, // SLA
            5 => null, // SRA
            6 => null, // SWAP
            7 => null, // SRL
            _ => throw new InvalidOperationException($"Invalid rot index {index}")
        };
    }

    private static string GetRotName(int index) {
        return index switch {
            0 => "RLC",
            1 => "RRC",
            2 => "RL",
            3 => "RR",
            4 => "SLA",
            5 => "SRA",
            6 => "SWAP",
            7 => "SRL",
            _ => throw new InvalidOperationException($"Invalid rot index {index}")
        };
    }

    public Opcode[] BuildUnprefixed() {
        Opcode[] unprefixedOps = new Opcode[256];

        // Fill unprefixed opcodes with invalid instructions
        for (int i = 0; i < unprefixedOps.Length; i++) {
            unprefixedOps[i] = INVALID_OPCODE;
        }

        unprefixedOps[0x00] = new("NOP", 1, 4, () => {
            // Do nothing
        }); // NOP
        unprefixedOps[0x10] = new("STOP", 1, 4, () => {
            // TODO: Implement STOP behavior
            // NOTE: STOP is actually 1 byte, but the next byte is skipped
            cpu.Reg.PC++;
        }); // STOP
        unprefixedOps[0x08] = new("LD (u16), SP", 3, 20, () => {
            var addr = cpu.ReadWord();
            cpu.WriteWord(addr, cpu.Reg.SP);
        }); // LD (u16), SP

        unprefixedOps[0x18] = new("JR i8", 2, 12, () => {
            var offset = (sbyte)cpu.Read(); // TODO: Check if this behaves correctly
            cpu.Reg.PC += (ushort)offset;
        }); // JR i8

        for (int i = 0; i < 4; i++) {
            int index = i;
            ConditionalOpcode opcode = new($"JR {GetCCName(i)}, i8", 2, 8, 12, () => {
                var offset = (sbyte)cpu.Read(); // TODO: Check if this behaves correctly
                if (GetCC(index)) {
                    cpu.Reg.PC += (ushort)offset;
                }
            });
            unprefixedOps[0x20 + i * 8] = opcode;
        } // JR cc[y - 4], i8

        for (int i = 0; i < 4; i++) {
            int index = i;
            Opcode opcode = new($"LD {GetRegPairName(i)}, u16", 3, 12, () => {
                var value = cpu.ReadWord();
                SetRegPair(index, value);
            });
            unprefixedOps[0x01 + i * 16] = opcode;
        } // LD rp[p], u16

        for (int i = 0; i < 4; i++) {
            int index = i;
            Opcode opcode = new($"ADD HL, {GetRegPairName(i)}", 1, 8, () => {
                var value = GetRegPair(index);
                var (sum, halfCarry, carry) = Utils.Add(cpu.Reg.HL, value);

                cpu.Reg.SubtractionFlag = false;
                cpu.Reg.HalfCarryFlag = halfCarry;
                cpu.Reg.CarryFlag = carry;

                cpu.Reg.HL = sum;
            });
            unprefixedOps[0x09 + i * 16] = opcode;
        } // ADD HL, rp[p]

        unprefixedOps[0x02] = new("LD (BC), A", 1, 8, () => {
            cpu.Write(cpu.Reg.BC, cpu.Reg.A);
        }); // LD (BC), A
        unprefixedOps[0x12] = new("LD (DE), A", 1, 8, () => {
            cpu.Write(cpu.Reg.DE, cpu.Reg.A);
        }); // LD (DE), A
        unprefixedOps[0x22] = new("LD (HL+), A", 1, 8, () => {
            cpu.Write(cpu.Reg.HL, cpu.Reg.A);
            cpu.Reg.HL++;
        }); // LD (HL+), A
        unprefixedOps[0x32] = new("LD (HL-), A", 1, 8, () => {
            cpu.Write(cpu.Reg.HL, cpu.Reg.A);
            cpu.Reg.HL--;
        }); // LD (HL-), A

        unprefixedOps[0x0A] = new("LD A, (BC)", 1, 8, () => {
            cpu.Reg.A = cpu.Read(cpu.Reg.BC);
        }); // LD A, (BC)
        unprefixedOps[0x1A] = new("LD A, (DE)", 1, 8, () => {
            cpu.Reg.A = cpu.Read(cpu.Reg.DE);
        }); // LD A, (DE)
        unprefixedOps[0x2A] = new("LD A, (HL+)", 1, 8, () => {
            cpu.Reg.A = cpu.Read(cpu.Reg.HL);
            cpu.Reg.HL++;
        }); // LD A, (HL+)
        unprefixedOps[0x3A] = new("LD A, (HL-)", 1, 8, () => {
            cpu.Reg.A = cpu.Read(cpu.Reg.HL);
            cpu.Reg.HL--;
        }); // LD A, (HL-)

        for (int i = 0; i < 4; i++) {
            int index = i;
            Opcode opcode = new($"INC {GetRegPairName(i)}", 1, 8, () => {
                var value = GetRegPair(index);
                value++;
                SetRegPair(index, value);
            });
            unprefixedOps[0x03 + i * 16] = opcode; // TODO: Check if opcode is correct
        } // INC rp[p]

        for (int i = 0; i < 4; i++) {
            int index = i;
            Opcode opcode = new($"DEC {GetRegPairName(i)}", 1, 8, () => {
                var value = GetRegPair(index);
                value--;
                SetRegPair(index, value);
            });
            unprefixedOps[0x0B + i * 16] = opcode; // TODO: Check if opcode is correct
        } // DEC rp[p]

        for (int i = 0; i < 8; i++) {
            int index = i;
            var cycles = i == 6 ? 12 : 4;
            Opcode opcode = new($"INC {GetRegName(i)}", 1, cycles, () => {
                var value = GetReg(index);
                var (sum, halfCarry, _) = Utils.Add(value, 1);

                cpu.Reg.SubtractionFlag = false;
                cpu.Reg.ZeroFlag = sum == 0;
                cpu.Reg.HalfCarryFlag = halfCarry;

                SetReg(index, value);
            });
            unprefixedOps[0x04 + i * 8] = opcode; // TODO: Check if opcode is correct
        } // INC r[y]

        for (int i = 0; i < 8; i++) {
            int index = i;
            var cycles = i == 6 ? 12 : 4;
            Opcode opcode = new($"DEC {GetRegName(i)}", 1, cycles, () => {
                var value = GetReg(index);
                var (diff, halfCarry, _) = Utils.Sub(value, 1);

                cpu.Reg.SubtractionFlag = true;
                cpu.Reg.ZeroFlag = diff == 0;
                cpu.Reg.HalfCarryFlag = halfCarry;

                SetReg(index, value);
            });
            unprefixedOps[0x05 + i * 8] = opcode; // TODO: Check if opcode is correct
        } // DEC r[y]

        for (int i = 0; i < 8; i++) {
            int index = i;
            var cycles = i == 6 ? 12 : 8;
            Opcode opcode = new($"LD {GetRegName(i)}, u8", 2, cycles, () => {
                var value = cpu.Read();
                SetReg(index, value);
            });
            unprefixedOps[0x06 + i * 8] = opcode; // TODO: Check if opcode is correct
        } // LD r[y], u8

        unprefixedOps[0x07] = new("RLCA", 1, 4, () => {
            // TODO: Implement RLCA behavior
        }); // RLCA
        unprefixedOps[0x0F] = new("RRCA", 1, 4, () => {
            // TODO: Implement RRCA behavior
        }); // RRCA
        unprefixedOps[0x17] = new("RLA", 1, 4, () => {
            // TODO: Implement RLA behavior
        }); // RLA
        unprefixedOps[0x1F] = new("RRA", 1, 4, () => {
            // TODO: Implement RRA behavior
        }); // RRA
        unprefixedOps[0x27] = new("DAA", 1, 4, () => {
            // TODO: Implement DAA behavior
        }); // DAA
        unprefixedOps[0x2F] = new("CPL", 1, 4, () => {
            // TODO: Implement CPL behavior
        }); // CPL
        unprefixedOps[0x37] = new("SCF", 1, 4, () => {
            // TODO: Implement SCF behavior
        }); // SCF
        unprefixedOps[0x3F] = new("CCF", 1, 4, () => {
            // TODO: Implement CCF behavior
        }); // CCF

        for (int i = 0; i < 8; i++) {
            var firstIndex = i;
            for (int j = 0; j < 8; j++) {
                var secondIndex = j;
                if (i == 6 && j == 6) {
                    unprefixedOps[0x76] = new("HALT", 1, 4, () => {
                        // TODO: Implement HALT behavior
                    }); // HALT
                    continue;
                }

                var cycles = i == 6 || j == 6 ? 8 : 4;
                Opcode opcode = new($"LD {GetRegName(i)}, {GetRegName(j)}", 1, cycles, () => {
                    var value = GetReg(secondIndex);
                    SetReg(firstIndex, value);
                });
                unprefixedOps[0x40 + i * 8 + j] = opcode; // TODO: Check if opcode is correct
            }
        } // LD r[y], r[z]

        for (int i = 0; i < 8; i++) {
            var firstIndex = i;
            for (int j = 0; j < 8; j++) {
                var secondIndex = j;
                var cycles = j == 6 ? 8 : 4;
                Opcode opcode = new($"{GetAluName(i)} A, {GetRegName(j)}", 1, cycles, () => {
                    // TODO: Implement alu[y] r[z] behavior
                });
                unprefixedOps[0x80 + i * 8 + j] = opcode; // TODO: Check if opcode is correct
            }
        } // alu[y] r[z]

        for (int i = 0; i < 4; i++) {
            int index = i;
            ConditionalOpcode opcode = new($"RET {GetCCName(i)}", 1, 8, 20, () => {
                if (GetCC(index)) {
                    cpu.Reg.PC = cpu.Pop();
                }
            });
            unprefixedOps[0xC0 + i * 8] = opcode;
        } // RET cc[y]
        unprefixedOps[0xE0] = new("LD (0xFF00+u8), A", 2, 12, () => {
            // TODO: Implement LD (FF00+u8), A behavior
        }); // LD (FF00+u8), A
        unprefixedOps[0xE8] = new("ADD SP, i8", 2, 16, () => {
            // TODO: Implement ADD SP, i8 behavior
        }); // ADD SP, i8
        unprefixedOps[0xF0] = new("LD A, (0xFF00+u8)", 2, 12, () => {
            // TODO: Implement LD A, (FF00+u8) behavior
        }); // LD A, (FF00+u8)
        unprefixedOps[0xF8] = new("LD HL, SP+i8", 2, 12, () => {
            // TODO: Implement LD HL, SP+i8 behavior
        }); // LD HL, SP+i8

        for (int i = 0; i < 4; i++) {
            int index = i;
            Opcode opcode = new($"POP {GetRegPairName(i, true)}", 1, 12, () => {
                var value = cpu.Pop();
                SetRegPair(index, value, true); // TODO: address or value?
            });
            unprefixedOps[0xC1 + i * 16] = opcode;
        } // POP rp2[p]
        unprefixedOps[0xC9] = new("RET", 1, 16, () => {
            cpu.Reg.PC = cpu.Pop();
        }); // RET
        unprefixedOps[0xD9] = new("RETI", 1, 16, () => {
            cpu.Reg.PC = cpu.Pop();
            cpu.InterruptsScheduled = true; // TODO: More accurate EI behavior
        }); // RETI
        unprefixedOps[0xE9] = new("JP HL", 1, 4, () => {
            cpu.Reg.PC = cpu.Reg.HL;
        }); // JP HL
        unprefixedOps[0xF9] = new("LD SP, HL", 1, 8, () => {
            cpu.Reg.SP = cpu.Reg.HL;
        }); // LD SP, HL

        for (int i = 0; i < 4; i++) {
            ConditionalOpcode opcode = new($"JP {GetCCName(i)}, u16", 3, 12, 16, () => {
                var addr = cpu.ReadWord();
                if (GetCC(i)) {
                    cpu.Reg.PC = addr;
                }
            });
            unprefixedOps[0xC2 + i * 8] = opcode;
        } // JP cc[y], u16
        unprefixedOps[0xE2] = new("LD (0xFF00+C), A", 1, 8, () => {
            // TODO: Implement LD (FF00+C), A behavior
        }); // LD (FF00+C), A
        unprefixedOps[0xEA] = new("LD (u16), A", 3, 16, () => {
            var addr = cpu.ReadWord();
            cpu.Write(addr, cpu.Reg.A);
        }); // LD (u16), A
        unprefixedOps[0xF2] = new("LD A, (0xFF00+C)", 1, 8, () => {
            // TODO: Implement LD A, (FF00+C) behavior
        }); // LD A, (FF00+C)
        unprefixedOps[0xFA] = new("LD A, (u16)", 3, 16, () => {
            var addr = cpu.ReadWord();
            cpu.Reg.A = cpu.Read(addr);
        }); // LD A, (u16)

        unprefixedOps[0xC3] = new("JP u16", 3, 16, () => {
            var addr = cpu.ReadWord();
            cpu.Reg.PC = addr;
        }); // JP u16
        unprefixedOps[0xCB] = new("PREFIX CB", 1, 4, () => {
            throw new InvalidOperationException($"CB prefix should not be executed directly");
        }); // CB prefix
        unprefixedOps[0xF3] = new("DI", 1, 4, () => {
            cpu.InterruptsScheduled = false; // TODO: More accurate DI behavior
            cpu.InterruptsEnabled = false;
        }); // DI
        unprefixedOps[0xFB] = new("EI", 1, 4, () => {
            cpu.InterruptsScheduled = true; // TODO: More accurate EI behavior
        }); // EI

        for (int i = 0; i < 4; i++) {
            int index = i;
            ConditionalOpcode opcode = new($"CALL {GetCCName(i)}, u16", 3, 12, 24, () => {
                var addr = cpu.ReadWord();
                if (GetCC(index)) {
                    cpu.Push(cpu.Reg.PC);
                    cpu.Reg.PC = addr;
                }
            });
            unprefixedOps[0xC4 + i * 8] = opcode;
        } // CALL cc[y], u16

        for (int i = 0; i < 4; i++) {
            int index = i;
            Opcode opcode = new($"PUSH {GetRegPairName(i, true)}", 1, 16, () => {
                var value = GetRegPair(index, true); // TODO: is this value or address?
                cpu.Push(value);
            });
            unprefixedOps[0xC5 + i * 16] = opcode;
        } // PUSH rp2[p]

        unprefixedOps[0xCD] = new("CALL u16", 3, 24, () => {
            var addr = cpu.ReadWord();
            cpu.Push(cpu.Reg.PC);
            cpu.Reg.PC = addr;
        }); // CALL u16

        for (int i = 0; i < 8; i++) {
            int index = i;
            Opcode opcode = new($"{GetAluName(i)} A, u8", 2, 8, () => {
                // TODO: Implement alu[y] u8 behavior
            });
            unprefixedOps[0xC6 + i * 8] = opcode; // TODO: Check if opcode is correct
        } // alu[y] u8

        for (int i = 0; i < 8; i++) {
            int index = i;
            Opcode opcode = new($"RST {i * 8:X2}h", 1, 16, () => {
                cpu.Push(cpu.Reg.PC);
                cpu.Reg.PC = (ushort)(index * 8);
            });
            unprefixedOps[0xC7 + i * 8] = opcode;
        } // RST XXh

        return unprefixedOps;
    }

    public Opcode[] BuildPrefixed() {
        Opcode[] prefixedOps = new Opcode[256];

        // Fill prefixed opcodes with invalid instructions
        for (int i = 0; i < prefixedOps.Length; i++) {
            prefixedOps[i] = INVALID_OPCODE;
        }

        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                var cycles = j == 6 ? 16 : 8;
                Opcode opcode = new($"{GetRotName(i)} {GetRegName(j)}", 2, cycles, () => {
                    // TODO: Implement rot[y] r[z] behavior
                });
                prefixedOps[0x00 + i * 8 + j] = opcode;
            }
        } // rot[y] r[z]

        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                var cycles = j == 6 ? 16 : 8;
                Opcode opcode = new($"BIT {i}, {GetRegName(j)}", 2, cycles, () => {
                    // TODO: Implement BIT y, r[z] behavior
                });
                prefixedOps[0x40 + i * 8 + j] = opcode;
            }
        } // BIT y, r[z] 

        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                var cycles = j == 6 ? 16 : 8;
                Opcode opcode = new($"RES {i}, {GetRegName(j)}", 2, cycles, () => {
                    // TODO: Implement RES y, r[z] behavior
                });
                prefixedOps[0x80 + i * 8 + j] = opcode;
            }
        } // RES y, r[z] 

        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                var cycles = j == 6 ? 16 : 8;
                Opcode opcode = new($"SET {i}, {GetRegName(j)}", 2, cycles, () => {
                    // TODO: Implement SET y, r[z] behavior
                });
                prefixedOps[0xC0 + i * 8 + j] = opcode;
            }
        } // SET y, r[z]

        return prefixedOps;
    }
}
