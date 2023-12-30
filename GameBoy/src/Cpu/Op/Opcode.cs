namespace GameBoy.Cpu.Op;

public class Opcode(string mnemonic, int length, int cycles, Action<CPU> execute) {
    public string Mnemonic { get; } = mnemonic;
    public int Length { get; } = length;
    public int Cycles { get; } = cycles;
    public Action<CPU> Execute { get; } = execute;
};
