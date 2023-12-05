namespace GameBoy.Emulator.Interrupts;

public enum Type {
    VBlank = 0x01,
    LCDStat = 0x02,
    Timer = 0x04,
    Serial = 0x08,
    Joypad = 0x10,
}
