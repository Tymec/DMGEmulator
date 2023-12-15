namespace GameBoy.Cpu;


public partial class CPU {
    const bool HALT_BUG = true;

    public Registers Reg;
    private readonly Memory.MMU mmu;
    private readonly Interrupts.Handler interrupts;

    private bool _ime = false;
    private bool _imeScheduled = false;
    private bool _halted = false;

    private readonly Instruction[] unprefixed = new Instruction[256];
    private readonly Instruction[] prefixed = new Instruction[256];

    public CPU(Memory.MMU mmu, Interrupts.Handler interrupts) {
        Reg = new Registers();
        this.mmu = mmu;
        this.interrupts = interrupts;

        // Build unprefixed instruction table
        for (int i = 0; i < unprefixed.Length; i++) {
            unprefixed[i] = new("INVALID", 1, 4, (_) =>
                throw new InvalidOperationException($"Invalid opcode: 0x{i:X2}")
            );
        }

        unprefixed[0x00] = new("NOP", 1, 4, (_) => { });

    }

    public void Cycle() {
        if (_ime) {
            // TODO: Handle interrupts
        }

        if (_halted) {
            return;
        }

        string debugLine = $"PC: 0x{Reg.PC - 1:X4} | "; // DEBUG
        byte opcode = Read8();
        if (opcode == 0xCB) {
            debugLine += $"0xCB Prefix  | "; // DEBUG
        } else {
            debugLine += $"Unprefixed   | "; // DEBUG
        }

        Instruction instr = Decode(opcode);
        instr.Execute(this);

        debugLine += $"0x{opcode:X2} | {instr.Mnemonic} | ({instr.Cycles}T, {instr.Length}B)"; // DEBUG
        Console.WriteLine(debugLine); // DEBUG

        if (_imeScheduled) {
            _imeScheduled = false;
            _ime = true;
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

    public byte Read8(ushort addr) => mmu.Read(addr);

    public byte Read8() => Read8(Reg.PC++);

    public ushort Read16() {
        var lo = Read8();
        var hi = Read8();
        return (ushort)((hi << 8) | lo);
    }

    public void Write8(ushort addr, byte value) => mmu.Write(addr, value);

    public void Write16(ushort addr, ushort value) {
        Write8(addr, (byte)(value & 0xFF));
        Write8((ushort)(addr + 1), (byte)(value >> 8));
    }
}
