namespace GameBoy.Emulator;


public static class Utils {
    public static bool HalfCarryAdd(byte a, byte b) {
        return (((a & 0xF) + (b & 0xF)) & 0x10) == 0x10;
    }

    public static bool HalfCarryAdd(ushort a, ushort b) {
        return (((a & 0xFFF) + (b & 0xFFF)) & 0x1000) == 0x1000;
    }

    public static bool HalfCarrySub(byte a, byte b) {
        return (((a & 0xF) - (b & 0xF)) & 0x10) == 0x10;
    }

    public static bool HalfCarrySub(ushort a, ushort b) {
        return (((a & 0xFFF) - (b & 0xFFF)) & 0x1000) == 0x1000;
    }

    // TODO: HalfCarry with carry
}
