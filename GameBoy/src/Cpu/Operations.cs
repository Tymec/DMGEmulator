namespace GameBoy.Cpu;


public static class Operations {
    #region 8-bit Load instructions
    public static void Load(ref byte dest, byte src) {
        dest = src;
    }
    #endregion

    #region 16-bit Load instructions
    public static void Load(ref ushort dest, ushort src) {
        dest = src;
    }
    #endregion

    #region 8-bit Arithmetic/Logic instructions
    public static void Increment(ref byte value) {
        value++;
    }

    public static void Decrement(ref byte value) {
        value--;
    }
    #endregion

    #region 16-bit Arithmetic/Logic instructions
    public static void Increment(ref ushort value) {
        value++;
    }

    public static void Decrement(ref ushort value) {
        value--;
    }
    #endregion

    #region Rotate and Shift instructions
    #endregion

    #region Single-bit Operation instructions
    #endregion

    #region CPU Control instructions
    #endregion

    #region Jump instructions
    #endregion
}
