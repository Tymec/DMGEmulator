using GameBoy.Cpu.Op;

namespace GameBoy.Cpu;


public partial class CPU {
    const bool HALT_BUG = true;

    public Registers Reg;
    public bool InterruptsScheduled { get; set; } = false;
    public bool InterruptsEnabled { get; set; } = false;
    public bool Halted { get; set; } = false;

    private readonly Memory.MMU mmu;
    private readonly Interrupts.Handler interrupts;

    private readonly Opcode[] unprefixed = new Opcode[256];
    private readonly Opcode[] prefixed = new Opcode[256];

    public CPU(Memory.MMU mmu, Interrupts.Handler interrupts) {
        Reg = new Registers();
        this.mmu = mmu;
        this.interrupts = interrupts;

        BuildUnprefixedOpcodes();
        BuildPrefixedOpcodes();
    }

    public void Cycle() {
        if (InterruptsEnabled) {
            // TODO: Handle interrupts
        }

        if (Halted) {
            return;
        }

        string debugLine = $"PC: 0x{Reg.PC - 1:X4} | "; // DEBUG
        byte opcode = Read();
        if (opcode == 0xCB) {
            debugLine += $"0xCB Prefix  | "; // DEBUG
        } else {
            debugLine += $"Unprefixed   | "; // DEBUG
        }

        Opcode instr = Decode(opcode);
        instr.Execute(this);

        debugLine += $"0x{opcode:X2} | {instr.Mnemonic} | ({instr.Cycles}T, {instr.Length}B)"; // DEBUG
        Console.WriteLine(debugLine); // DEBUG

        if (InterruptsScheduled) {
            InterruptsScheduled = false;
            InterruptsEnabled = true;
        }
    }

    private Opcode Decode(byte opcode) {
        Opcode? instr;
        if (opcode == 0xCB) {
            opcode = Read();
            instr = prefixed.ElementAtOrDefault(opcode);
        } else {
            instr = unprefixed.ElementAtOrDefault(opcode);
        }

        if (instr == null) {
            throw new InvalidOperationException($"Invalid opcode: 0x{opcode:X2}");
        }

        return instr;
    }

    public byte Read(ushort addr) => mmu.Read(addr);

    public byte Read() => Read(Reg.PC++);

    public ushort ReadWord() {
        var lo = Read();
        var hi = Read();
        return (ushort)((hi << 8) | lo);
    }

    public void Write(ushort addr, byte value) => mmu.Write(addr, value);

    public void WriteWord(ushort addr, ushort value) {
        Write(addr, (byte)(value & 0xFF));
        Write((ushort)(addr + 1), (byte)(value >> 8));
    }

    public void Push(ushort value) {
        Reg.SP -= 2;
        WriteWord(Reg.SP, value);
    }

    public ushort Pop() {
        var value = ReadWord();
        Reg.SP += 2;
        return value;
    }
}
