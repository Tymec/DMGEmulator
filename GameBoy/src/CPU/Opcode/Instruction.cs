namespace GameBoy.CPU.Opcode;


public readonly struct Instruction(string mnemonic, int length, int cycles, Action execute) {
    public string Mnemonic { get; } = mnemonic;
    public int Length { get; } = length;
    public int Cycles { get; } = cycles;
    public Action Execute { get; } = execute;

    // public static string ToString(byte opcode, string[] operands) {
    //     var instruction = Parse(opcode);
    //     return $"0x{opcode:X2} {string.Format(instruction.Mnemonic, operands)}";
    // }
}
