namespace GameBoy.Cpu;


public partial class CPU {
    const bool HALT_BUG = true;

    private Registers reg;
    private readonly Memory.MMU mmu;
    private readonly Interrupts.Handler interrupts;

    private bool ime = false;
    private bool imeScheduled = false;
    private bool halted = false;

    private readonly Instruction[] unprefixed = new Instruction[256];

    public CPU(Memory.MMU mmu, Interrupts.Handler interrupts) {
        reg = new Registers();
        this.mmu = mmu;
        this.interrupts = interrupts;

        unprefixed = [
            new("NOP", 1, 4, () => { }),   // 0x00
            new("LD BC, 0x{0:X4}", 3, 12, () => reg.BC = Read16()),   // 0x01
            new("LD (BC), A", 1, 8, () => Write8(reg.BC, reg.A)),   // 0x02
            new("INC BC", 1, 8, () => reg.BC = OP_inc(reg.BC)),   // 0x03
            new("INC B", 1, 4, () => reg.B = OP_inc(reg.B)),   // 0x04
            new("DEC B", 1, 4, () => reg.B = OP_dec(reg.B)),   // 0x05
            new("LD B, 0x{0:X2}", 2, 8, () => reg.B = Read8()),   // 0x06
            new("RLCA", 1, 4, () => { /* TODO */ }),   // 0x07
            new("LD (0x{0:X4}), SP", 3, 20, () => Write16(Read16(), reg.SP)),   // 0x08
            new("ADD HL, BC", 1, 8, () => OP_add(reg.BC)),   // 0x09
            new("LD A, (BC)", 1, 8, () => reg.A = Read8(reg.BC)),   // 0x0A
            new("DEC BC", 1, 8, () => reg.BC = OP_dec(reg.BC)),   // 0x0B
            new("INC C", 1, 4, () => reg.C = OP_inc(reg.C)),   // 0x0C
            new("DEC C", 1, 4, () => reg.C = OP_dec(reg.C)),   // 0x0D
            new("LD C, 0x{0:X2}", 2, 8, () => reg.C = Read8()),   // 0x0E
            new("RRCA", 1, 4, () => { /* TODO */ }),   // 0x0F
            new("STOP", 2, 4, () => {
                /* TODO */
                reg.PC++; // NOTE: Skip next byte
            }),   // 0x10
            Instruction.INVALID, // 0x11
            Instruction.INVALID, // 0x12
            Instruction.INVALID, // 0x13
            Instruction.INVALID, // 0x14
            Instruction.INVALID, // 0x15
            Instruction.INVALID, // 0x16
            Instruction.INVALID, // 0x17
            Instruction.INVALID, // 0x18
            Instruction.INVALID, // 0x19
            Instruction.INVALID, // 0x1A
            Instruction.INVALID, // 0x1B
            Instruction.INVALID, // 0x1C
            Instruction.INVALID, // 0x1D
            Instruction.INVALID, // 0x1E
            Instruction.INVALID, // 0x1F
            Instruction.INVALID, // 0x20
            Instruction.INVALID, // 0x21
            Instruction.INVALID, // 0x22
            Instruction.INVALID, // 0x23
            Instruction.INVALID, // 0x24
            Instruction.INVALID, // 0x25
            Instruction.INVALID, // 0x26
            Instruction.INVALID, // 0x27
            Instruction.INVALID, // 0x28
            Instruction.INVALID, // 0x29
            Instruction.INVALID, // 0x2A
            Instruction.INVALID, // 0x2B
            Instruction.INVALID, // 0x2C
            Instruction.INVALID, // 0x2D
            Instruction.INVALID, // 0x2E
            Instruction.INVALID, // 0x2F
            Instruction.INVALID, // 0x30
            Instruction.INVALID, // 0x31
            Instruction.INVALID, // 0x32
            Instruction.INVALID, // 0x33
            Instruction.INVALID, // 0x34
            Instruction.INVALID, // 0x35
            Instruction.INVALID, // 0x36
            Instruction.INVALID, // 0x37
            Instruction.INVALID, // 0x38
            Instruction.INVALID, // 0x39
            Instruction.INVALID, // 0x3A
            Instruction.INVALID, // 0x3B
            Instruction.INVALID, // 0x3C
            Instruction.INVALID, // 0x3D
            Instruction.INVALID, // 0x3E
            Instruction.INVALID, // 0x3F
            Instruction.INVALID, // 0x40
            Instruction.INVALID, // 0x41
            Instruction.INVALID, // 0x42
            Instruction.INVALID, // 0x43
            Instruction.INVALID, // 0x44
            Instruction.INVALID, // 0x45
            Instruction.INVALID, // 0x46
            Instruction.INVALID, // 0x47
            Instruction.INVALID, // 0x48
            Instruction.INVALID, // 0x49
            Instruction.INVALID, // 0x4A
            Instruction.INVALID, // 0x4B
            Instruction.INVALID, // 0x4C
            Instruction.INVALID, // 0x4D
            Instruction.INVALID, // 0x4E
            Instruction.INVALID, // 0x4F
            Instruction.INVALID, // 0x50
            Instruction.INVALID, // 0x51
            Instruction.INVALID, // 0x52
            Instruction.INVALID, // 0x53
            Instruction.INVALID, // 0x54
            Instruction.INVALID, // 0x55
            Instruction.INVALID, // 0x56
            Instruction.INVALID, // 0x57
            Instruction.INVALID, // 0x58
            Instruction.INVALID, // 0x59
            Instruction.INVALID, // 0x5A
            Instruction.INVALID, // 0x5B
            Instruction.INVALID, // 0x5C
            Instruction.INVALID, // 0x5D
            Instruction.INVALID, // 0x5E
            Instruction.INVALID, // 0x5F
            Instruction.INVALID, // 0x60
            Instruction.INVALID, // 0x61
            Instruction.INVALID, // 0x62
            Instruction.INVALID, // 0x63
            Instruction.INVALID, // 0x64
            Instruction.INVALID, // 0x65
            Instruction.INVALID, // 0x66
            Instruction.INVALID, // 0x67
            Instruction.INVALID, // 0x68
            Instruction.INVALID, // 0x69
            Instruction.INVALID, // 0x6A
            Instruction.INVALID, // 0x6B
            Instruction.INVALID, // 0x6C
            Instruction.INVALID, // 0x6D
            Instruction.INVALID, // 0x6E
            Instruction.INVALID, // 0x6F
            Instruction.INVALID, // 0x70
            Instruction.INVALID, // 0x71
            Instruction.INVALID, // 0x72
            Instruction.INVALID, // 0x73
            Instruction.INVALID, // 0x74
            Instruction.INVALID, // 0x75
            new("HALT", 1, 4, () => {
                halted = true;
            }),   // 0x76
            Instruction.INVALID, // 0x77
            Instruction.INVALID, // 0x78
            Instruction.INVALID, // 0x79
            Instruction.INVALID, // 0x7A
            Instruction.INVALID, // 0x7B
            Instruction.INVALID, // 0x7C
            Instruction.INVALID, // 0x7D
            Instruction.INVALID, // 0x7E
            Instruction.INVALID, // 0x7F
            Instruction.INVALID, // 0x80
            Instruction.INVALID, // 0x81
            Instruction.INVALID, // 0x82
            Instruction.INVALID, // 0x83
            Instruction.INVALID, // 0x84
            Instruction.INVALID, // 0x85
            Instruction.INVALID, // 0x86
            Instruction.INVALID, // 0x87
            Instruction.INVALID, // 0x88
            Instruction.INVALID, // 0x89
            Instruction.INVALID, // 0x8A
            Instruction.INVALID, // 0x8B
            Instruction.INVALID, // 0x8C
            Instruction.INVALID, // 0x8D
            Instruction.INVALID, // 0x8E
            Instruction.INVALID, // 0x8F
            Instruction.INVALID, // 0x90
            Instruction.INVALID, // 0x91
            Instruction.INVALID, // 0x92
            Instruction.INVALID, // 0x93
            Instruction.INVALID, // 0x94
            Instruction.INVALID, // 0x95
            Instruction.INVALID, // 0x96
            Instruction.INVALID, // 0x97
            Instruction.INVALID, // 0x98
            Instruction.INVALID, // 0x99
            Instruction.INVALID, // 0x9A
            Instruction.INVALID, // 0x9B
            Instruction.INVALID, // 0x9C
            Instruction.INVALID, // 0x9D
            Instruction.INVALID, // 0x9E
            Instruction.INVALID, // 0x9F
            Instruction.INVALID, // 0xA0
            Instruction.INVALID, // 0xA1
            Instruction.INVALID, // 0xA2
            Instruction.INVALID, // 0xA3
            Instruction.INVALID, // 0xA4
            Instruction.INVALID, // 0xA5
            Instruction.INVALID, // 0xA6
            Instruction.INVALID, // 0xA7
            Instruction.INVALID, // 0xA8
            Instruction.INVALID, // 0xA9
            Instruction.INVALID, // 0xAA
            Instruction.INVALID, // 0xAB
            Instruction.INVALID, // 0xAC
            Instruction.INVALID, // 0xAD
            Instruction.INVALID, // 0xAE
            Instruction.INVALID, // 0xAF
            Instruction.INVALID, // 0xB0
            Instruction.INVALID, // 0xB1
            Instruction.INVALID, // 0xB2
            Instruction.INVALID, // 0xB3
            Instruction.INVALID, // 0xB4
            Instruction.INVALID, // 0xB5
            Instruction.INVALID, // 0xB6
            Instruction.INVALID, // 0xB7
            Instruction.INVALID, // 0xB8
            Instruction.INVALID, // 0xB9
            Instruction.INVALID, // 0xBA
            Instruction.INVALID, // 0xBB
            Instruction.INVALID, // 0xBC
            Instruction.INVALID, // 0xBD
            Instruction.INVALID, // 0xBE
            Instruction.INVALID, // 0xBF
            Instruction.INVALID, // 0xC0
            Instruction.INVALID, // 0xC1
            Instruction.INVALID, // 0xC2
            Instruction.INVALID, // 0xC3
            Instruction.INVALID, // 0xC4
            Instruction.INVALID, // 0xC5
            Instruction.INVALID, // 0xC6
            Instruction.INVALID, // 0xC7
            Instruction.INVALID, // 0xC8
            Instruction.INVALID, // 0xC9
            Instruction.INVALID, // 0xCA
            new("PREFIX CB", 1, 4, () => {
                throw new InvalidOperationException("Prefix CB should not be executed directly");
            }),   // 0xCB
            Instruction.INVALID, // 0xCC
            Instruction.INVALID, // 0xCD
            Instruction.INVALID, // 0xCE
            Instruction.INVALID, // 0xCF
            Instruction.INVALID, // 0xD0
            Instruction.INVALID, // 0xD1
            Instruction.INVALID, // 0xD2
            Instruction.INVALID, // 0xD3
            Instruction.INVALID, // 0xD4
            Instruction.INVALID, // 0xD5
            Instruction.INVALID, // 0xD6
            Instruction.INVALID, // 0xD7
            Instruction.INVALID, // 0xD8
            Instruction.INVALID, // 0xD9
            Instruction.INVALID, // 0xDA
            Instruction.INVALID, // 0xDB
            Instruction.INVALID, // 0xDC
            Instruction.INVALID, // 0xDD
            Instruction.INVALID, // 0xDE
            Instruction.INVALID, // 0xDF
            Instruction.INVALID, // 0xE0
            Instruction.INVALID, // 0xE1
            Instruction.INVALID, // 0xE2
            Instruction.INVALID, // 0xE3
            Instruction.INVALID, // 0xE4
            Instruction.INVALID, // 0xE5
            Instruction.INVALID, // 0xE6
            Instruction.INVALID, // 0xE7
            Instruction.INVALID, // 0xE8
            Instruction.INVALID, // 0xE9
            Instruction.INVALID, // 0xEA
            Instruction.INVALID, // 0xEB
            Instruction.INVALID, // 0xEC
            Instruction.INVALID, // 0xED
            Instruction.INVALID, // 0xEE
            Instruction.INVALID, // 0xEF
            Instruction.INVALID, // 0xF0
            Instruction.INVALID, // 0xF1
            Instruction.INVALID, // 0xF2
            new("DI", 1, 4, () => {
                ime = false;
                imeScheduled = false;
            }),   // 0xF3
            Instruction.INVALID, // 0xF4
            Instruction.INVALID, // 0xF5
            Instruction.INVALID, // 0xF6
            Instruction.INVALID, // 0xF7
            Instruction.INVALID, // 0xF8
            Instruction.INVALID, // 0xF9
            Instruction.INVALID, // 0xFA
            new("EI", 1, 4, () => {
                imeScheduled = true;
            }),   // 0xFB
            Instruction.INVALID, // 0xFC
            Instruction.INVALID, // 0xFD
            Instruction.INVALID, // 0xFE
            Instruction.INVALID, // 0xFF
        ];
    }

    public void Cycle() {
        if (ime) {
            // TODO: Handle interrupts
        }

        if (halted) {
            return;
        }

        string debugLine = $"PC: 0x{reg.PC - 1:X4} | "; // DEBUG
        byte opcode = Read8();
        if (opcode == 0xCB) {
            debugLine += $"0xCB Prefix  | "; // DEBUG
        } else {
            debugLine += $"Unprefixed   | "; // DEBUG
        }

        Instruction instr = Decode(opcode);
        instr.Execute();

        debugLine += $"0x{opcode:X2} | {instr.Mnemonic} | ({instr.Cycles}T, {instr.Length}B)"; // DEBUG
        Console.WriteLine(debugLine); // DEBUG

        if (imeScheduled) {
            imeScheduled = false;
            ime = true;
        }
    }

    private Instruction Decode(byte opcode) {
        Instruction? instr;
        if (opcode == 0xCB) {
            opcode = Read8();
            // instr = prefixed.ElementAtOrDefault(opcode);
            instr = unprefixed.ElementAtOrDefault(0x00); // DEBUG
        } else {
            instr = unprefixed.ElementAtOrDefault(opcode);
        }

        if (instr == null) {
            throw new InvalidOperationException($"Invalid opcode: 0x{opcode:X2}");
        }

        return (Instruction)instr;
    }

    private byte Read8(ushort addr) => mmu.Read(addr);

    private byte Read8() => Read8(reg.PC++);

    private ushort Read16() {
        var lo = Read8();
        var hi = Read8();
        return (ushort)((hi << 8) | lo);
    }

    private void Write8(ushort addr, byte value) => mmu.Write(addr, value);

    private void Write16(ushort addr, ushort value) {
        Write8(addr, (byte)(value & 0xFF));
        Write8((ushort)(addr + 1), (byte)(value >> 8));
    }
}
