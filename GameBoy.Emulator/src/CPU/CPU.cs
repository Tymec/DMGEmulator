namespace GameBoy.Emulator.CPU;


public partial class CPU {
    private readonly Registers reg;

    public CPU() {
        reg = new Registers();
    }

    private byte read(ushort address) {
        return 0;
    }

    private void write(ushort address, byte value) {
    }
}
