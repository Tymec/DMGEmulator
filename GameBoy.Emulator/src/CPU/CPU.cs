namespace GameBoy.Emulator.CPU;

public partial class CPU {
    const bool HALT_BUG = true;

    private readonly Registers reg;
    private readonly Memory.MMU mmu;
    private readonly Interrupts.Handler interrupts;

    private bool ime = false;
    private bool imeScheduled = false;
    private bool halted = false;

    public CPU(Memory.MMU mmu, Interrupts.Handler interrupts) {
        reg = new Registers();
        this.mmu = mmu;
        this.interrupts = interrupts;
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
        bool prefix = false;
        if (opcode == 0xCB) {
            prefix = true;
            opcode = Read8();
            cycles = 4 + ExecutePrefixed(opcode);
        } else {
            cycles = ExecuteUnprefixed(opcode);
        }

        if (imeScheduled) {
            imeScheduled = false;
            ime = true;
        }
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
