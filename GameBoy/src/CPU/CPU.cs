namespace GameBoy.CPU;

using GameBoy.CPU.Opcode;

public partial class CPU {
    const bool HALT_BUG = true;

    private Registers reg;
    private readonly Memory.MMU mmu;
    private readonly Interrupts.Handler interrupts;

    private bool ime = false;
    private bool imeScheduled = false;
    private bool halted = false;

    private Instruction[] unprefixed = new Instruction[256];

    public CPU(Memory.MMU mmu, Interrupts.Handler interrupts) {
        reg = new Registers();
        this.mmu = mmu;
        this.interrupts = interrupts;

        unprefixed = new Instruction[] {
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
            
            new("HALT", 1, 4, () => {
                halted = true;
            }),   // 0x76

            new("PREFIX CB", 1, 4, () => {
                throw new InvalidOperationException("Prefix CB should not be executed directly");
            }),   // 0xCB

            new("DI", 1, 4, () => {
                ime = false;
                imeScheduled = false;
            }),   // 0xF3

            new("EI", 1, 4, () => {
                imeScheduled = true;
            }),   // 0xFB
        };
    }

    public void Cycle() {
        if (ime) {
            // TODO: Handle interrupts
        }

        int cycles = 4; // TODO: Do something with this
        if (halted) {
            return;
        }

        byte opcode = Read8();
        string debugLine = $"PC: 0x{reg.PC - 1:X4} | "; // DEBUG
        bool prefix = false;
        if (opcode == 0xCB) {
            prefix = true;
            debugLine += "Prefixed | "; // DEBUG
            opcode = Read8();
            debugLine += $"Opcode: 0x{opcode:X2} | "; // DEBUG
            // cycles = 4 + ExecutePrefixed(opcode);
        } else {
            debugLine += $"Opcode: 0x{opcode:X2} | "; // DEBUG
            // cycles = ExecuteUnprefixed(opcode);
        }

        debugLine += $"Cycles: {cycles}T"; // DEBUG
        Console.WriteLine(debugLine); // DEBUG

        if (imeScheduled) {
            imeScheduled = false;
            ime = true;
        }
    }

    public int Execute(byte opcode) {
        var instr = unprefixed[opcode];
        instr.Execute();
        return instr.Cycles;
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
