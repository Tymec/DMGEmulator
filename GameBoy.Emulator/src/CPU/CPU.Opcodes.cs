namespace GameBoy.Emulator.CPU;


public partial class CPU {
    private byte OP_inc(byte x) {
        byte y = (byte)((x + 1) & 0xFF);
        reg.SubtractionFlag = false;
        reg.ZeroFlag = y == 0;
        reg.HalfCarryFlag = Utils.HalfCarryAdd(x, 1);
        return y;
    }

    private ushort OP_inc(ushort x) {
        return (ushort)((x + 1) & 0xFFFF);
    }

    private byte OP_dec(byte x) {
        byte y = (byte)((x - 1) & 0xFF);
        reg.SubtractionFlag = true;
        reg.ZeroFlag = y == 0;
        reg.HalfCarryFlag = Utils.HalfCarrySub(x, 1);
        return x;
    }

    private ushort OP_dec(ushort x) {
        return (ushort)((x - 1) & 0xFFFF);
    }
}
