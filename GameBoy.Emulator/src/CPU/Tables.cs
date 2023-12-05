namespace GameBoy.Emulator.CPU;


// maps indexes to register types
public static class Tables {
    public static readonly ERegister[] Registers = [
        ERegister.B,
        ERegister.C,
        ERegister.D,
        ERegister.E,
        ERegister.H,
        ERegister.L,
        ERegister.HL,
        ERegister.A
    ];

    public static readonly ERegister[] Registers16 = [
        ERegister.BC,
        ERegister.DE,
        ERegister.HL,
        ERegister.SP
    ];
}
