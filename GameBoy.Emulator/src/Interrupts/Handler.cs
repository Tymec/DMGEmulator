namespace GameBoy.Emulator.Interrupts;


public class Handler {
    private byte InterruptEnable;
    private byte InterruptFlag;

    public Handler() {
        InterruptEnable = 0x00;
        InterruptFlag = 0x00;
    }

    public bool IsEnabled(Type t) => (InterruptEnable & (byte)t) != 0;
    public bool IsRequested(Type t) => (InterruptFlag & (byte)t) != 0;

    public void Enable(Type t) {
        InterruptEnable |= (byte)t;
    }

    public void Disable(Type t) {
        InterruptEnable &= (byte)~t;
    }

    public void Request(Type t) {
        InterruptFlag |= (byte)t;
    }
}
