namespace GameBoy.Interrupts;


public enum Vector {
    VBlank = 0x40,
    LCDStat = 0x48,
    Timer = 0x50,
    Serial = 0x58,
    Joypad = 0x60,
}
