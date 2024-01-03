namespace GameBoy.Cpu;

public class Opcode(string mnemonic, int length, int cycles, Action execute) {
    public string Mnemonic { get; } = mnemonic;
    public int Length { get; } = length;
    public int Cycles { get; } = cycles;
    public Action Execute { get; } = execute;
};

public class ConditionalOpcode(string mnemonic, int length, int cycles, int cyclesBranch, Action execute) : Opcode(mnemonic, length, cycles, execute) {
    public int CyclesBranch { get; } = cyclesBranch;
};
