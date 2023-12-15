namespace GameBoy.Cpu;

using static GameBoy.Cpu.Operations;


public readonly struct Instruction(string mnemonic, int length, int cycles, Action<CPU> execute) {
    public string Mnemonic { get; } = mnemonic;
    public int Length { get; } = length;
    public int Cycles { get; } = cycles;
    // public Action Execute { get; } = execute;
    public Action<CPU> Execute { get; } = execute;

    // public static string ToString(byte opcode, string[] operands) {
    //     var instruction = Parse(opcode);
    //     return $"0x{opcode:X2} {string.Format(instruction.Mnemonic, operands)}";
    // }

    public static readonly Action<CPU> Unimplemented = (_) => throw new NotImplementedException();

    public static readonly Instruction[] UNPREFIXED = [
        new("NOP", 1, 4, (_) => { }), // 0x00
        new("LD BC, 0x{0:X4}", 3, 12, (cpu) => Load(ref cpu.Reg.BC, cpu.Read16())), // 0x01
        new("LD (BC), A", 1, 8, (cpu) => cpu.Write16(cpu.Reg.BC, cpu.Reg.A)), // 0x02
        new("INC BC", 1, 8, (cpu) => Increment(ref cpu.Reg.BC)), // 0x03
        new("INC B", 1, 4, (cpu) => Increment(ref cpu.Reg.B)), // 0x04
        new("DEC B", 1, 4, (cpu) => Decrement(ref cpu.Reg.B)), // 0x05
        new("LD B, 0x{0:X2}", 2, 8, (cpu) => Load(ref cpu.Reg.B, cpu.Read8())), // 0x06
        new("RLCA", 1, 4, Unimplemented), // 0x07
        new("LD (0x{0:X4}), SP", 3, 20, Unimplemented), // 0x08
        new("ADD HL, BC", 1, 8, Unimplemented), // 0x09
        new("LD A, (BC)", 1, 8, Unimplemented), // 0x0A
        new("DEC BC", 1, 8, Unimplemented), // 0x0B
        new("INC C", 1, 4, Unimplemented), // 0x0C
        new("DEC C", 1, 4, Unimplemented), // 0x0D
        new("LD C, 0x{0:X2}", 2, 8, Unimplemented), // 0x0E
        new("RRCA", 1, 4, Unimplemented), // 0x0F
        new("STOP", 2, 4, Unimplemented), // 0x10   (NOTE: skips next byte)
        new("LD DE, 0x{0:X4}", 3, 12, Unimplemented), // 0x11
        new("LD (DE), A", 1, 8, Unimplemented), // 0x12
        new("INC DE", 1, 8, Unimplemented), // 0x13
        new("INC D", 1, 4, Unimplemented), // 0x14
        new("DEC D", 1, 4, Unimplemented), // 0x15
        new("LD D, 0x{0:X2}", 2, 8, Unimplemented), // 0x16
        new("RLA", 1, 4, Unimplemented), // 0x17
        new("JR 0x{0:X2}", 2, 12, Unimplemented), // 0x18   (NOTE: negative argument might not format as expected)
        new("ADD HL, DE", 1, 8, Unimplemented), // 0x19
        new("LD A, (DE)", 1, 8, Unimplemented), // 0x1A
        new("DEC DE", 1, 8, Unimplemented), // 0x1B
        new("INC E", 1, 4, Unimplemented), // 0x1C
        new("DEC E", 1, 4, Unimplemented), // 0x1D
        new("LD E, 0x{0:X2}", 2, 8, Unimplemented), // 0x1E
        new("RRA", 1, 4, Unimplemented), // 0x1F
        new("JR NZ, 0x{0:X2}", 2, 12, Unimplemented), // 0x20   (NOTE: negative argument might not format as expected)
        new("LD HL, 0x{0:X4}", 3, 12, Unimplemented), // 0x21
        new("LD (HL+), A", 1, 8, Unimplemented), // 0x22
        new("INC HL", 1, 8, Unimplemented), // 0x23
        new("INC H", 1, 4, Unimplemented), // 0x24
        new("DEC H", 1, 4, Unimplemented), // 0x25
        new("LD H, 0x{0:X2}", 2, 8, Unimplemented), // 0x26
        new("DAA", 1, 4, Unimplemented), // 0x27
        new("JR Z, 0x{0:X2}", 2, 12, Unimplemented), // 0x28   (NOTE: negative argument might not format as expected)
        new("ADD HL, HL", 1, 8, Unimplemented), // 0x29
        new("LD A, (HL+)", 1, 8, Unimplemented), // 0x2A
        new("DEC HL", 1, 8, Unimplemented), // 0x2B
        new("INC L", 1, 4, Unimplemented), // 0x2C
        new("DEC L", 1, 4, Unimplemented), // 0x2D
        new("LD L, 0x{0:X2}", 2, 8, Unimplemented), // 0x2E
        new("CPL", 1, 4, Unimplemented), // 0x2F
        new("JR NC, 0x{0:X2}", 2, 12, Unimplemented), // 0x30   (NOTE: negative argument might not format as expected)
        new("LD SP, 0x{0:X4}", 3, 12, Unimplemented), // 0x31
        new("LD (HL-), A", 1, 8, Unimplemented), // 0x32
        new("INC SP", 1, 8, Unimplemented), // 0x33
        new("INC (HL)", 1, 12, Unimplemented), // 0x34
        new("DEC (HL)", 1, 12, Unimplemented), // 0x35
        new("LD (HL), 0x{0:X2}", 2, 12, Unimplemented), // 0x36
        new("SCF", 1, 4, Unimplemented), // 0x37
        new("JR C, 0x{0:X2}", 2, 12, Unimplemented), // 0x38   (NOTE: negative argument might not format as expected)
        new("ADD HL, SP", 1, 8, Unimplemented), // 0x39
        new("LD A, (HL-)", 1, 8, Unimplemented), // 0x3A
        new("DEC SP", 1, 8, Unimplemented), // 0x3B
        new("INC A", 1, 4, Unimplemented), // 0x3C
        new("DEC A", 1, 4, Unimplemented), // 0x3D
        new("LD A, 0x{0:X2}", 2, 8, Unimplemented), // 0x3E
        new("CCF", 1, 4, Unimplemented), // 0x3F
        new("LD B, B", 1, 4, Unimplemented), // 0x40
        new("LD B, C", 1, 4, Unimplemented), // 0x41
        new("LD B, D", 1, 4, Unimplemented), // 0x42
        new("LD B, E", 1, 4, Unimplemented), // 0x43
        new("LD B, H", 1, 4, Unimplemented), // 0x44
        new("LD B, L", 1, 4, Unimplemented), // 0x45
        new("LD B, (HL)", 1, 8, Unimplemented), // 0x46
        new("LD B, A", 1, 4, Unimplemented), // 0x47
        new("LD C, B", 1, 4, Unimplemented), // 0x48
        new("LD C, C", 1, 4, Unimplemented), // 0x49
        new("LD C, D", 1, 4, Unimplemented), // 0x4A
        new("LD C, E", 1, 4, Unimplemented), // 0x4B
        new("LD C, H", 1, 4, Unimplemented), // 0x4C
        new("LD C, L", 1, 4, Unimplemented), // 0x4D
        new("LD C, (HL)", 1, 8, Unimplemented), // 0x4E
        new("LD C, A", 1, 4, Unimplemented), // 0x4F
        new("LD D, B", 1, 4, Unimplemented), // 0x50
        new("LD D, C", 1, 4, Unimplemented), // 0x51
        new("LD D, D", 1, 4, Unimplemented), // 0x52
        new("LD D, E", 1, 4, Unimplemented), // 0x53
        new("LD D, H", 1, 4, Unimplemented), // 0x54
        new("LD D, L", 1, 4, Unimplemented), // 0x55
        new("LD D, (HL)", 1, 8, Unimplemented), // 0x56
        new("LD D, A", 1, 4, Unimplemented), // 0x57
        new("LD E, B", 1, 4, Unimplemented), // 0x58
        new("LD E, C", 1, 4, Unimplemented), // 0x59
        new("LD E, D", 1, 4, Unimplemented), // 0x5A
        new("LD E, E", 1, 4, Unimplemented), // 0x5B
        new("LD E, H", 1, 4, Unimplemented), // 0x5C
        new("LD E, L", 1, 4, Unimplemented), // 0x5D
        new("LD E, (HL)", 1, 8, Unimplemented), // 0x5E
        new("LD E, A", 1, 4, Unimplemented), // 0x5F
        new("LD H, B", 1, 4, Unimplemented), // 0x60
        new("LD H, C", 1, 4, Unimplemented), // 0x61
        new("LD H, D", 1, 4, Unimplemented), // 0x62
        new("LD H, E", 1, 4, Unimplemented), // 0x63
        new("LD H, H", 1, 4, Unimplemented), // 0x64
        new("LD H, L", 1, 4, Unimplemented), // 0x65
        new("LD H, (HL)", 1, 8, Unimplemented), // 0x66
        new("LD H, A", 1, 4, Unimplemented), // 0x67
        new("LD L, B", 1, 4, Unimplemented), // 0x68
        new("LD L, C", 1, 4, Unimplemented), // 0x69
        new("LD L, D", 1, 4, Unimplemented), // 0x6A
        new("LD L, E", 1, 4, Unimplemented), // 0x6B
        new("LD L, H", 1, 4, Unimplemented), // 0x6C
        new("LD L, L", 1, 4, Unimplemented), // 0x6D
        new("LD L, (HL)", 1, 8, Unimplemented), // 0x6E
        new("LD L, A", 1, 4, Unimplemented), // 0x6F
        new("LD (HL), B", 1, 8, Unimplemented), // 0x70
        new("LD (HL), C", 1, 8, Unimplemented), // 0x71
        new("LD (HL), D", 1, 8, Unimplemented), // 0x72
        new("LD (HL), E", 1, 8, Unimplemented), // 0x73
        new("LD (HL), H", 1, 8, Unimplemented), // 0x74
        new("LD (HL), L", 1, 8, Unimplemented), // 0x75
        new("HALT", 1, 4, Unimplemented), // 0x76
        new("LD (HL), A", 1, 8, Unimplemented), // 0x77
        new("LD A, B", 1, 4, Unimplemented), // 0x78
        new("LD A, C", 1, 4, Unimplemented), // 0x79
        new("LD A, D", 1, 4, Unimplemented), // 0x7A
        new("LD A, E", 1, 4, Unimplemented), // 0x7B
        new("LD A, H", 1, 4, Unimplemented), // 0x7C
        new("LD A, L", 1, 4, Unimplemented), // 0x7D
        new("LD A, (HL)", 1, 8, Unimplemented), // 0x7E
        new("LD A, A", 1, 4, Unimplemented), // 0x7F
        new("ADD A, B", 1, 4, Unimplemented), // 0x80
        new("ADD A, C", 1, 4, Unimplemented), // 0x81
        new("ADD A, D", 1, 4, Unimplemented), // 0x82
        new("ADD A, E", 1, 4, Unimplemented), // 0x83
        new("ADD A, H", 1, 4, Unimplemented), // 0x84
        new("ADD A, L", 1, 4, Unimplemented), // 0x85
        new("ADD A, (HL)", 1, 8, Unimplemented), // 0x86
        new("ADD A, A", 1, 4, Unimplemented), // 0x87
        new("ADC A, B", 1, 4, Unimplemented), // 0x88
        new("ADC A, C", 1, 4, Unimplemented), // 0x89
        new("ADC A, D", 1, 4, Unimplemented), // 0x8A
        new("ADC A, E", 1, 4, Unimplemented), // 0x8B
        new("ADC A, H", 1, 4, Unimplemented), // 0x8C
        new("ADC A, L", 1, 4, Unimplemented), // 0x8D
        new("ADC A, (HL)", 1, 8, Unimplemented), // 0x8E
        new("ADC A, A", 1, 4, Unimplemented), // 0x8F
        new("SUB B", 1, 4, Unimplemented), // 0x90
        new("SUB C", 1, 4, Unimplemented), // 0x91
        new("SUB D", 1, 4, Unimplemented), // 0x92
        new("SUB E", 1, 4, Unimplemented), // 0x93
        new("SUB H", 1, 4, Unimplemented), // 0x94
        new("SUB L", 1, 4, Unimplemented), // 0x95
        new("SUB (HL)", 1, 8, Unimplemented), // 0x96
        new("SUB A", 1, 4, Unimplemented), // 0x97
        new("SBC A, B", 1, 4, Unimplemented), // 0x98
        new("SBC A, C", 1, 4, Unimplemented), // 0x99
        new("SBC A, D", 1, 4, Unimplemented), // 0x9A
        new("SBC A, E", 1, 4, Unimplemented), // 0x9B
        new("SBC A, H", 1, 4, Unimplemented), // 0x9C
        new("SBC A, L", 1, 4, Unimplemented), // 0x9D
        new("SBC A, (HL)", 1, 8, Unimplemented), // 0x9E
        new("SBC A, A", 1, 4, Unimplemented), // 0x9F
        new("AND B", 1, 4, Unimplemented), // 0xA0
        new("AND C", 1, 4, Unimplemented), // 0xA1
        new("AND D", 1, 4, Unimplemented), // 0xA2
        new("AND E", 1, 4, Unimplemented), // 0xA3
        new("AND H", 1, 4, Unimplemented), // 0xA4
        new("AND L", 1, 4, Unimplemented), // 0xA5
        new("AND (HL)", 1, 8, Unimplemented), // 0xA6
        new("AND A", 1, 4, Unimplemented), // 0xA7
        new("XOR B", 1, 4, Unimplemented), // 0xA8
        new("XOR C", 1, 4, Unimplemented), // 0xA9
        new("XOR D", 1, 4, Unimplemented), // 0xAA
        new("XOR E", 1, 4, Unimplemented), // 0xAB
        new("XOR H", 1, 4, Unimplemented), // 0xAC
        new("XOR L", 1, 4, Unimplemented), // 0xAD
        new("XOR (HL)", 1, 8, Unimplemented), // 0xAE
        new("XOR A", 1, 4, Unimplemented), // 0xAF
        new("OR B", 1, 4, Unimplemented), // 0xB0
        new("OR C", 1, 4, Unimplemented), // 0xB1
        new("OR D", 1, 4, Unimplemented), // 0xB2
        new("OR E", 1, 4, Unimplemented), // 0xB3
        new("OR H", 1, 4, Unimplemented), // 0xB4
        new("OR L", 1, 4, Unimplemented), // 0xB5
        new("OR (HL)", 1, 8, Unimplemented), // 0xB6
        new("OR A", 1, 4, Unimplemented), // 0xB7
        new("CP B", 1, 4, Unimplemented), // 0xB8
        new("CP C", 1, 4, Unimplemented), // 0xB9
        new("CP D", 1, 4, Unimplemented), // 0xBA
        new("CP E", 1, 4, Unimplemented), // 0xBB
        new("CP H", 1, 4, Unimplemented), // 0xBC
        new("CP L", 1, 4, Unimplemented), // 0xBD
        new("CP (HL)", 1, 8, Unimplemented), // 0xBE
        new("CP A", 1, 4, Unimplemented), // 0xBF
        new("RET NZ", 1, 20, Unimplemented), // 0xC0    (TODO: branch)
        new("POP BC", 1, 12, Unimplemented), // 0xC1
        new("JP NZ, 0x{0:X4}", 3, 16, Unimplemented), // 0xC2   (TODO: branch)
        new("JP 0x{0:X4}", 3, 16, Unimplemented), // 0xC3
        new("CALL NZ, 0x{0:X4}", 3, 24, Unimplemented), // 0xC4     (TODO: branch)
        new("PUSH BC", 1, 16, Unimplemented), // 0xC5
        new("ADD A, 0x{0:X2}", 2, 8, Unimplemented), // 0xC6
        new("RST 0x00", 1, 16, Unimplemented), // 0xC7
        new("RET Z", 1, 20, Unimplemented), // 0xC8     (TODO: branch)
        new("RET", 1, 16, Unimplemented), // 0xC9
        new("JP Z, 0x{0:X4}", 3, 16, Unimplemented), // 0xCA     (TODO: branch)
        new("PREFIX CB", 1, 4, (_) => throw new InvalidOperationException("Invalid opcode: 0xCB")), // 0xCB
        new("CALL Z, 0x{0:X4}", 3, 24, Unimplemented), // 0xCC      (TODO: branch)
        new("CALL 0x{0:X4}", 3, 24, Unimplemented), // 0xCD
        new("ADC A, 0x{0:X2}", 2, 8, Unimplemented), // 0xCE
        new("RST 0x08", 1, 16, Unimplemented), // 0xCF
        new("RET NC", 1, 20, Unimplemented), // 0xD0    (TODO: branch)
        new("POP DE", 1, 12, Unimplemented), // 0xD1
        new("JP NC, 0x{0:X4}", 3, 16, Unimplemented), // 0xD2    (TODO: branch)
        new("INVALID", 1, 4, (_) => throw new InvalidOperationException("Invalid opcode: 0xD3")), // 0xD3
        new("CALL NC, 0x{0:X4}", 3, 24, Unimplemented), // 0xD4    (TODO: branch)
        new("PUSH DE", 1, 16, Unimplemented), // 0xD5
        new("SUB 0x{0:X2}", 2, 8, Unimplemented), // 0xD6
        new("RST 0x10", 1, 16, Unimplemented), // 0xD7
        new("RET C", 1, 20, Unimplemented), // 0xD8     (TODO: branch)
        new("RETI", 1, 16, Unimplemented), // 0xD9
        new("JP C, 0x{0:X4}", 3, 16, Unimplemented), // 0xDA     (TODO: branch)
        new("INVALID", 1, 4, (_) => throw new InvalidOperationException("Invalid opcode: 0xDB")), // 0xDB
        new("CALL C, 0x{0:X4}", 3, 24, Unimplemented), // 0xDC     (TODO: branch)
        new("INVALID", 1, 4, (_) => throw new InvalidOperationException("Invalid opcode: 0xDD")), // 0xDD
        new("SBC A, 0x{0:X2}", 2, 8, Unimplemented), // 0xDE
        new("RST 0x18", 1, 16, Unimplemented), // 0xDF
        new("LD (0xFF00 + 0x{0:X2}), A", 2, 12, Unimplemented), // 0xE0
        new("POP HL", 1, 12, Unimplemented), // 0xE1
        new("LD (0xFF00 + C), A", 1, 8, Unimplemented), // 0xE2
        new("INVALID", 1, 4, (_) => throw new InvalidOperationException("Invalid opcode: 0xE3")), // 0xE3
        new("INVALID", 1, 4, (_) => throw new InvalidOperationException("Invalid opcode: 0xE4")), // 0xE4
        new("PUSH HL", 1, 16, Unimplemented), // 0xE5
        new("AND A, 0x{0:X2}", 2, 8, Unimplemented), // 0xE6
        new("RST 0x20", 1, 16, Unimplemented), // 0xE7
        new("ADD SP, 0x{0:X2}", 2, 16, Unimplemented), // 0xE8  (NOTE: negative argument might not format as expected)
        new("JP HL", 1, 4, Unimplemented), // 0xE9
        new("LD (0x{0:X4}), A", 3, 16, Unimplemented), // 0xEA
        new("INVALID", 1, 4, (_) => throw new InvalidOperationException("Invalid opcode: 0xEB")), // 0xEB
        new("INVALID", 1, 4, (_) => throw new InvalidOperationException("Invalid opcode: 0xEC")), // 0xEC
        new("INVALID", 1, 4, (_) => throw new InvalidOperationException("Invalid opcode: 0xED")), // 0xED
        new("XOR A, 0x{0:X2}", 2, 8, Unimplemented), // 0xEE
        new("RST 0x28", 1, 16, Unimplemented), // 0xEF
        new("LD A, (0xFF00 + 0x{0:X2})", 2, 12, Unimplemented), // 0xF0
        new("POP AF", 1, 12, Unimplemented), // 0xF1
        new("LD A, (0xFF00 + C)", 1, 8, Unimplemented), // 0xF2
        new("DI", 1, 4, Unimplemented), // 0xF3
        new("INVALID", 1, 4, (_) => throw new InvalidOperationException("Invalid opcode: 0xF4")), // 0xF4
        new("PUSH AF", 1, 16, Unimplemented), // 0xF5
        new("OR A, 0x{0:X2}", 2, 8, Unimplemented), // 0xF6
        new("RST 0x30", 1, 16, Unimplemented), // 0xF7
        new("LD HL, SP + 0x{0:X2}", 2, 12, Unimplemented), // 0xF8  (NOTE: negative argument might not format as expected)
        new("LD A, (0x{0:X4})", 3, 16, Unimplemented), // 0xFA
        new("EI", 1, 4, Unimplemented), // 0xFB
        new("INVALID", 1, 4, (_) => throw new InvalidOperationException("Invalid opcode: 0xFC")), // 0xFC
        new("INVALID", 1, 4, (_) => throw new InvalidOperationException("Invalid opcode: 0xFD")), // 0xFD
        new("CP A, 0x{0:X2}", 2, 8, Unimplemented), // 0xFE
        new("RST 0x38", 1, 16, Unimplemented), // 0xFF
    ];

    public static readonly Instruction[] PREFIXED = [
        new("RLC B", 2, 8, Unimplemented), // 0x00
        new("RLC C", 2, 8, Unimplemented), // 0x01
        new("RLC D", 2, 8, Unimplemented), // 0x02
        new("RLC E", 2, 8, Unimplemented), // 0x03
        new("RLC H", 2, 8, Unimplemented), // 0x04
        new("RLC L", 2, 8, Unimplemented), // 0x05
        new("RLC (HL)", 2, 16, Unimplemented), // 0x06
        new("RLC A", 2, 8, Unimplemented), // 0x07
        new("RRC B", 2, 8, Unimplemented), // 0x08
        new("RRC C", 2, 8, Unimplemented), // 0x09
        new("RRC D", 2, 8, Unimplemented), // 0x0A
        new("RRC E", 2, 8, Unimplemented), // 0x0B
        new("RRC H", 2, 8, Unimplemented), // 0x0C
        new("RRC L", 2, 8, Unimplemented), // 0x0D
        new("RRC (HL)", 2, 16, Unimplemented), // 0x0E
        new("RRC A", 2, 8, Unimplemented), // 0x0F
        new("RL B", 2, 8, Unimplemented), // 0x10
        new("RL C", 2, 8, Unimplemented), // 0x11
        new("RL D", 2, 8, Unimplemented), // 0x12
        new("RL E", 2, 8, Unimplemented), // 0x13
        new("RL H", 2, 8, Unimplemented), // 0x14
        new("RL L", 2, 8, Unimplemented), // 0x15
        new("RL (HL)", 2, 16, Unimplemented), // 0x16
        new("RL A", 2, 8, Unimplemented), // 0x17
        new("RR B", 2, 8, Unimplemented), // 0x18
        new("RR C", 2, 8, Unimplemented), // 0x19
        new("RR D", 2, 8, Unimplemented), // 0x1A
        new("RR E", 2, 8, Unimplemented), // 0x1B
        new("RR H", 2, 8, Unimplemented), // 0x1C
        new("RR L", 2, 8, Unimplemented), // 0x1D
        new("RR (HL)", 2, 16, Unimplemented), // 0x1E
        new("RR A", 2, 8, Unimplemented), // 0x1F
        new("SLA B", 2, 8, Unimplemented), // 0x20
        new("SLA C", 2, 8, Unimplemented), // 0x21
        new("SLA D", 2, 8, Unimplemented), // 0x22
        new("SLA E", 2, 8, Unimplemented), // 0x23
        new("SLA H", 2, 8, Unimplemented), // 0x24
        new("SLA L", 2, 8, Unimplemented), // 0x25
        new("SLA (HL)", 2, 16, Unimplemented), // 0x26
        new("SLA A", 2, 8, Unimplemented), // 0x27
        new("SRA B", 2, 8, Unimplemented), // 0x28
        new("SRA C", 2, 8, Unimplemented), // 0x29
        new("SRA D", 2, 8, Unimplemented), // 0x2A
        new("SRA E", 2, 8, Unimplemented), // 0x2B
        new("SRA H", 2, 8, Unimplemented), // 0x2C
        new("SRA L", 2, 8, Unimplemented), // 0x2D
        new("SRA (HL)", 2, 16, Unimplemented), // 0x2E
        new("SRA A", 2, 8, Unimplemented), // 0x2F
        new("SWAP B", 2, 8, Unimplemented), // 0x30
        new("SWAP C", 2, 8, Unimplemented), // 0x31
        new("SWAP D", 2, 8, Unimplemented), // 0x32
        new("SWAP E", 2, 8, Unimplemented), // 0x33
        new("SWAP H", 2, 8, Unimplemented), // 0x34
        new("SWAP L", 2, 8, Unimplemented), // 0x35
        new("SWAP (HL)", 2, 16, Unimplemented), // 0x36
        new("SWAP A", 2, 8, Unimplemented), // 0x37
        new("SRL B", 2, 8, Unimplemented), // 0x38
        new("SRL C", 2, 8, Unimplemented), // 0x39
        new("SRL D", 2, 8, Unimplemented), // 0x3A
        new("SRL E", 2, 8, Unimplemented), // 0x3B
        new("SRL H", 2, 8, Unimplemented), // 0x3C
        new("SRL L", 2, 8, Unimplemented), // 0x3D
        new("SRL (HL)", 2, 16, Unimplemented), // 0x3E
        new("SRL A", 2, 8, Unimplemented), // 0x3F
        // BIT (0x40 - 0x7F)
        // RES (0x80 - 0xBF)
        // SET (0xC0 - 0xFF)
    ];
}
