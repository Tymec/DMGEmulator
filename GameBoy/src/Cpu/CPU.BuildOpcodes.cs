namespace GameBoy.Cpu;

using GameBoy.Cpu.Op;


public partial class CPU {
    private readonly Opcode[] unprefixedOps = new Opcode[256];
    private readonly Opcode[] prefixedOps = new Opcode[256];

    public void BuildOpcodes() {
        BuildUnprefixedOpcodes();
        BuildPrefixedOpcodes();
    }

    private byte GetReg(int index) {
        return index switch {
            0 => Reg.B,
            1 => Reg.C,
            2 => Reg.D,
            3 => Reg.E,
            4 => Reg.H,
            5 => Reg.L,
            6 => Read(Reg.HL),
            7 => Reg.A,
            _ => throw new InvalidOperationException($"Invalid register index {index}")
        };
    }

    private void SetReg(int index, byte value) {
        switch (index) {
            case 0:
                Reg.B = value;
                break;
            case 1:
                Reg.C = value;
                break;
            case 2:
                Reg.D = value;
                break;
            case 3:
                Reg.E = value;
                break;
            case 4:
                Reg.H = value;
                break;
            case 5:
                Reg.L = value;
                break;
            case 6:
                Write(Reg.HL, value);
                break;
            case 7:
                Reg.A = value;
                break;
            default:
                throw new InvalidOperationException($"Invalid register index {index}");
        }
    }

    private string GetRegName(int index) {
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
            0 => Reg.BC,
            1 => Reg.DE,
            2 => Reg.HL,
            3 => alt ? Reg.AF : Reg.SP,
            _ => throw new InvalidOperationException($"Invalid register pair index {index}")
        };
    }

    private void SetRegPair(int index, ushort value, bool alt = false) {
        switch (index) {
            case 0:
                Reg.BC = value;
                break;
            case 1:
                Reg.DE = value;
                break;
            case 2:
                Reg.HL = value;
                break;
            case 3:
                if (alt) {
                    Reg.AF = value;
                } else {
                    Reg.SP = value;
                }
                break;
            default:
                throw new InvalidOperationException($"Invalid register pair index {index}");
        }
    }

    private string GetRegPairName(int index, bool alt = false) {
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
            0 => !Reg.ZeroFlag,     // NZ
            1 => Reg.ZeroFlag,      // Z
            2 => !Reg.CarryFlag,    // NC
            3 => Reg.CarryFlag,     // C
            _ => throw new InvalidOperationException($"Invalid condition code index {index}")
        };
    }

    private string GetCCName(int index) {
        return index switch {
            0 => "NZ",  // NZ
            1 => "Z",   // Z
            2 => "NC",  // NC
            3 => "C",   // C
            _ => throw new InvalidOperationException($"Invalid condition code index {index}")
        };
    }

    private Action<CPU> GetAlu(int index) {
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

    private string GetAluName(int index) {
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

    private void BuildUnprefixedOpcodes() {
        var invalidOpcode = new Opcode("INVALID", 1, 4, (_) =>
            throw new InvalidOperationException($"Invalid opcode")
        );

        // Fill unprefixed opcodes with invalid instructions
        for (int i = 0; i < unprefixedOps.Length; i++) {
            unprefixedOps[i] = invalidOpcode;
        }

        // NOTE: conditional jumps can have different cycle counts depending on whether the condition is true or false

        unprefixedOps[0x00] = new Opcode("NOP", 1, 4, (_) => {
        }); // NOP
        unprefixedOps[0x10] = new Opcode("STOP", 2, 4, (cpu) => {
            // TODO: Implement actual STOP behavior
        }); // STOP
        unprefixedOps[0x08] = new Opcode("LD (nn), SP", 3, 20, (cpu) => {
            var addr = cpu.ReadWord();
            cpu.WriteWord(addr, cpu.Reg.SP);
        }); // LD (nn), SP

        unprefixedOps[0x18] = new Opcode("JR PC+dd", 2, 12, (cpu) => {
            var offset = (sbyte)cpu.Read(); // TODO: Check if this behaves correctly
            cpu.Reg.PC += (ushort)offset;
        }); // JR PC + dd

        for (int i = 0; i < 4; i++) {
            var opcode = new Opcode($"JR {GetCCName(i)}, PC+dd", 2, 12, (cpu) => { // NOTE: Is 8 cycles if condition is false
                var offset = (sbyte)cpu.Read(); // TODO: Check if this behaves correctly
                if (GetCC(i)) {
                    cpu.Reg.PC += (ushort)offset;
                }
            });
            unprefixedOps[0x20 + i * 8] = opcode; // TODO: Check if opcode is correct
        } // JR cc[y - 4], dd

        for (int i = 0; i < 4; i++) {
            var opcode = new Opcode($"LD {GetRegPairName(i)}, nn", 3, 12, (cpu) => {
                var value = cpu.ReadWord();
                SetRegPair(i, value);
            });
            unprefixedOps[0x01 + i * 16] = opcode; // TODO: Check if opcode is correct
        } // LD rp[p], nn

        for (int i = 0; i < 4; i++) {
            var opcode = new Opcode($"ADD HL, {GetRegPairName(i)}", 1, 8, (cpu) => {
                var value = GetRegPair(i);
                var (sum, halfCarry, carry) = Utils.Add(cpu.Reg.HL, value);

                cpu.Reg.SubtractionFlag = false;
                cpu.Reg.HalfCarryFlag = halfCarry;
                cpu.Reg.CarryFlag = carry;

                cpu.Reg.HL = sum;
            });
            unprefixedOps[0x09 + i * 16] = opcode; // TODO: Check if opcode is correct
        } // ADD HL, rp[p]

        unprefixedOps[0x02] = new Opcode("LD (BC), A", 1, 8, (cpu) => {
            cpu.Write(cpu.Reg.BC, cpu.Reg.A);
        }); // LD (BC), A
        unprefixedOps[0x12] = new Opcode("LD (DE), A", 1, 8, (cpu) => {
            cpu.Write(cpu.Reg.DE, cpu.Reg.A);
        }); // LD (DE), A
        unprefixedOps[0x22] = new Opcode("LD (HL+), A", 1, 8, (cpu) => {
            cpu.Write(cpu.Reg.HL, cpu.Reg.A);
            cpu.Reg.HL++;
        }); // LD (HL+), A
        unprefixedOps[0x32] = new Opcode("LD (HL-), A", 1, 8, (cpu) => {
            cpu.Write(cpu.Reg.HL, cpu.Reg.A);
            cpu.Reg.HL--;
        }); // LD (HL-), A

        unprefixedOps[0x0A] = new Opcode("LD A, (BC)", 1, 8, (cpu) => {
            cpu.Reg.A = cpu.Read(cpu.Reg.BC);
        }); // LD A, (BC)
        unprefixedOps[0x1A] = new Opcode("LD A, (DE)", 1, 8, (cpu) => {
            cpu.Reg.A = cpu.Read(cpu.Reg.DE);
        }); // LD A, (DE)
        unprefixedOps[0x2A] = new Opcode("LD A, (HL+)", 1, 8, (cpu) => {
            cpu.Reg.A = cpu.Read(cpu.Reg.HL);
            cpu.Reg.HL++;
        }); // LD A, (HL+)
        unprefixedOps[0x3A] = new Opcode("LD A, (HL-)", 1, 8, (cpu) => {
            cpu.Reg.A = cpu.Read(cpu.Reg.HL);
            cpu.Reg.HL--;
        }); // LD A, (HL-)

        for (int i = 0; i < 4; i++) {
            var opcode = new Opcode($"INC {GetRegPairName(i)}", 1, 8, (cpu) => {
                var value = GetRegPair(i);
                value++;
                SetRegPair(i, value);
            });
            unprefixedOps[0x03 + i * 16] = opcode; // TODO: Check if opcode is correct
        } // INC rp[p]

        for (int i = 0; i < 4; i++) {
            var opcode = new Opcode($"DEC {GetRegPairName(i)}", 1, 8, (cpu) => {
                var value = GetRegPair(i);
                value--;
                SetRegPair(i, value);
            });
            unprefixedOps[0x0B + i * 16] = opcode; // TODO: Check if opcode is correct
        } // DEC rp[p]

        for (int i = 0; i < 8; i++) {
            var opcode = new Opcode($"INC {GetRegName(i)}", 1, 4, (cpu) => {
                var value = GetReg(i);
                var (sum, halfCarry, _) = Utils.Add(value, 1);

                cpu.Reg.SubtractionFlag = false;
                cpu.Reg.ZeroFlag = sum == 0;
                cpu.Reg.HalfCarryFlag = halfCarry;

                SetReg(i, value);
            });
            unprefixedOps[0x04 + i * 8] = opcode; // TODO: Check if opcode is correct
        } // INC r[y]

        for (int i = 0; i < 8; i++) {
            var opcode = new Opcode($"DEC {GetRegName(i)}", 1, 4, (cpu) => {
                var value = GetReg(i);
                var (diff, halfCarry, _) = Utils.Sub(value, 1);

                cpu.Reg.SubtractionFlag = true;
                cpu.Reg.ZeroFlag = diff == 0;
                cpu.Reg.HalfCarryFlag = halfCarry;

                SetReg(i, value);
            });
            unprefixedOps[0x05 + i * 8] = opcode; // TODO: Check if opcode is correct
        } // DEC r[y]

        for (int i = 0; i < 8; i++) {
            var opcode = new Opcode($"LD {GetRegName(i)}, n", 2, 8, (cpu) => {
                var value = cpu.Read();
                SetReg(i, value);
            });
            unprefixedOps[0x06 + i * 8] = opcode; // TODO: Check if opcode is correct
        } // LD r[y], n

        // RLCA (TODO: Implement)
        // RRCA (TODO: Implement)
        // RLA (TODO: Implement)
        // RRA (TODO: Implement)
        // DAA (TODO: Implement)
        // CPL (TODO: Implement)
        // SCF (TODO: Implement)
        // CCF (TODO: Implement)

        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 7; j++) {
                if (i == 6 && j == 6) {
                    unprefixedOps[0x76] = new Opcode("HALT", 1, 4, (_) => {
                        // TODO: Implement HALT behavior
                    }); // HALT
                    continue;
                }

                var opcode = new Opcode($"LD {GetRegName(i)}, {GetRegName(j)}", 1, 4, (cpu) => {
                    var value = GetReg(j);
                    SetReg(i, value);
                });
                unprefixedOps[0x40 + i * 8 + j] = opcode; // TODO: Check if opcode is correct
            }
        } // LD r[y], r[z]

        // alu[y] r[z] (TODO: Implement)

        for (int i = 0; i < 4; i++) {
            var opcode = new Opcode($"RET {GetCCName(i)}", 1, 8, (cpu) => {
                if (GetCC(i)) {
                    cpu.Reg.PC = cpu.Pop();
                }
            });
            unprefixedOps[0xC0 + i * 8] = opcode; // TODO: Check if opcode is correct
        } // RET cc[y]
        // LD (FF00 + n), A (TODO: Implement)
        // ADD SP, d (TODO: Implement)
        // LD A, (FF00 + n) (TODO: Implement)
        // LD HL, SP + d (TODO: Implement)

        for (int i = 0; i < 4; i++) {
            var opcode = new Opcode($"POP {GetRegPairName(i, true)}", 1, 12, (cpu) => {
                var value = cpu.Pop();
                SetRegPair(i, value, true); // TODO: address or value?
            });
            unprefixedOps[0xC1 + i * 16] = opcode; // TODO: Check if opcode is correct
        } // POP rp2[p]
        unprefixedOps[0xC9] = new Opcode("RET", 1, 16, (cpu) => {
            cpu.Reg.PC = cpu.Pop();
        }); // RET
        unprefixedOps[0xD9] = new Opcode("RETI", 1, 16, (cpu) => {
            cpu.Reg.PC = cpu.Pop();
            cpu.InterruptsScheduled = true; // TODO: More accurate EI behavior
        }); // RETI
        unprefixedOps[0xE9] = new Opcode("JP HL", 1, 4, (cpu) => {
            cpu.Reg.PC = cpu.Reg.HL;
        }); // JP HL
        unprefixedOps[0xF9] = new Opcode("LD SP, HL", 1, 8, (cpu) => {
            cpu.Reg.SP = cpu.Reg.HL;
        }); // LD SP, HL

        for (int i = 0; i < 4; i++) {
            var opcode = new Opcode($"JP {GetCCName(i)}, nn", 3, 12, (cpu) => {
                var addr = cpu.ReadWord();
                if (GetCC(i)) {
                    cpu.Reg.PC = addr;
                }
            });
            unprefixedOps[0xC2 + i * 8] = opcode; // TODO: Check if opcode is correct
        } // JP cc[y], nn
        // LD (FF00 + C), A (TODO: Implement)
        unprefixedOps[0xEA] = new Opcode("LD (nn), A", 3, 16, (cpu) => {
            var addr = cpu.ReadWord();
            cpu.Write(addr, cpu.Reg.A);
        }); // LD (nn), A
        // LD A, (FF00 + C) (TODO: Implement)
        unprefixedOps[0xFA] = new Opcode("LD A, (nn)", 3, 16, (cpu) => {
            var addr = cpu.ReadWord();
            cpu.Reg.A = cpu.Read(addr);
        }); // LD A, (nn)

        unprefixedOps[0xC3] = new Opcode("JP nn", 3, 16, (cpu) => {
            var addr = cpu.ReadWord();
            cpu.Reg.PC = addr;
        }); // JP nn
        unprefixedOps[0xCB] = new Opcode("CB prefix", 1, 4, (cpu) => {
            var opcode = cpu.Read();
            var instr = prefixedOps[opcode];
            instr.Execute(cpu); // TODO: Or throw exception?
        }); // CB prefix
        unprefixedOps[0xF3] = new Opcode("DI", 1, 4, (cpu) => {
            cpu.InterruptsScheduled = false; // TODO: More accurate DI behavior
            cpu.InterruptsEnabled = false;
        }); // DI
        unprefixedOps[0xFB] = new Opcode("EI", 1, 4, (cpu) => {
            cpu.InterruptsScheduled = true; // TODO: More accurate EI behavior
        }); // EI

        for (int i = 0; i < 4; i++) {
            var opcode = new Opcode($"CALL {GetCCName(i)}, nn", 3, 12, (cpu) => {
                var addr = cpu.ReadWord();
                if (GetCC(i)) {
                    cpu.Push(cpu.Reg.PC);
                    cpu.Reg.PC = addr;
                }
            });
            unprefixedOps[0xC4 + i * 8] = opcode; // TODO: Check if opcode is correct
        } // CALL cc[y], nn

        for (int i = 0; i < 4; i++) {
            var opcode = new Opcode($"PUSH {GetRegPairName(i, true)}", 1, 16, (cpu) => {
                var value = GetRegPair(i, true); // TODO: is this value or address?
                cpu.Push(value);
            });
            unprefixedOps[0xC5 + i * 16] = opcode; // TODO: Check if opcode is correct
        } // PUSH rp2[p]

        unprefixedOps[0xCD] = new Opcode("CALL nn", 3, 24, (cpu) => {
            var addr = cpu.ReadWord();
            cpu.Push(cpu.Reg.PC);
            cpu.Reg.PC = addr;
        }); // CALL nn

        // alu[y] n (TODO: Implement)

        for (int i = 0; i < 8; i++) {
            var opcode = new Opcode($"RST {i * 8}", 1, 16, (cpu) => {
                cpu.Push(cpu.Reg.PC);
                cpu.Reg.PC = (ushort)(i * 8);
            });
            unprefixedOps[0xC7 + i * 8] = opcode; // TODO: Check if opcode is correct
        } // RST y * 8
    }

    private void BuildPrefixedOpcodes() {
        var invalidOpcode = new Opcode("INVALID", 1, 4, (_) =>
            throw new InvalidOperationException($"Invalid opcode")
        );

        // Fill prefixed opcodes with invalid instructions
        for (int i = 0; i < prefixedOps.Length; i++) {
            prefixedOps[i] = invalidOpcode;
        }

        var rTable = new string[] { "B", "C", "D", "E", "H", "L", "(HL)", "A" };
        var rotTable = new string[] { "RLC", "RRC", "RL", "RR", "SLA", "SRA", "SWAP", "SRL" };

        // rot[y] r[z] (TODO: Implement)
        // BIT y, r[z] (TODO: Implement)
        // RES y, r[z] (TODO: Implement)
        // SET y, r[z] (TODO: Implement)
    }
}
