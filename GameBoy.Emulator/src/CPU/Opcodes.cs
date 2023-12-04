namespace GameBoy.Emulator.CPU;

public partial class CPU {
    public void execute(ushort opcode) {
        switch (opcode) {
            case 0x00: break;   // NOP 4T

            case 0xCB:
                // opcode = read8();
                switch (opcode) {
                    default: throw new NotImplementedException($"Opcode {opcode:X4} not implemented");
                }; break;

            default: throw new NotImplementedException($"Opcode {opcode:X4} not implemented");
        }
    }
}
