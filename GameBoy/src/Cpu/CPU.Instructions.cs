namespace GameBoy.Cpu;


public partial class CPU {
    #region 16-bit Arithmetic/Logic instructions
    private ushort OP_inc(ushort x) {
        return (ushort)((x + 1) & 0xFFFF);
    }

    private ushort OP_dec(ushort x) {
        return (ushort)((x - 1) & 0xFFFF);
    }

    private void OP_add(ushort x) {
        var (sum, halfCarry, carry) = Utils.Add(Reg.HL, x);

        Reg.SubtractionFlag = false;
        Reg.HalfCarryFlag = halfCarry;
        Reg.CarryFlag = carry;

        Reg.HL = sum;
    }

    private void OP_add(sbyte x) {
        var (res, halfCarry, carry) = x > 0 ?
            Utils.Add(Reg.SP, (ushort)x) :
            Utils.Sub(Reg.SP, (ushort)((-1) * x));

        Reg.ZeroFlag = false;
        Reg.SubtractionFlag = false;
        Reg.HalfCarryFlag = halfCarry;
        Reg.CarryFlag = carry;

        Reg.SP = res;
    }
    #endregion

    #region 8-bit Arithmetic/Logic instructions
    private byte OP_inc(byte x) {
        var (sum, halfCarry, _) = Utils.Add(x, 1);

        Reg.SubtractionFlag = false;
        Reg.ZeroFlag = sum == 0;
        Reg.HalfCarryFlag = halfCarry;

        return sum;
    }

    private byte OP_dec(byte x) {
        var (diff, halfCarry, _) = Utils.Sub(x, 1);

        Reg.SubtractionFlag = true;
        Reg.ZeroFlag = diff == 0;
        Reg.HalfCarryFlag = halfCarry;

        return diff;
    }

    private void OP_add(byte x, bool withCarry = false) {
        var c = withCarry ? (Reg.CarryFlag ? 1 : 0) : 0;
        var (sum, halfCarry, carry) = Utils.Add(Reg.A, x, (byte)c);

        Reg.SubtractionFlag = false;
        Reg.ZeroFlag = sum == 0;
        Reg.HalfCarryFlag = halfCarry;
        Reg.CarryFlag = carry;

        Reg.A = sum;
    }

    private void OP_sub(byte x, bool withCarry = false) {
        var c = withCarry ? (Reg.CarryFlag ? 1 : 0) : 0;
        var (diff, halfCarry, carry) = Utils.Sub(Reg.A, x, (byte)c);

        Reg.SubtractionFlag = true;
        Reg.ZeroFlag = diff == 0;
        Reg.HalfCarryFlag = halfCarry;
        Reg.CarryFlag = carry;

        Reg.A = diff;
    }

    private void OP_and(byte x) {
        Reg.A &= x;
        Reg.SubtractionFlag = false;
        Reg.ZeroFlag = Reg.A == 0;
        Reg.HalfCarryFlag = true;
        Reg.CarryFlag = false;
    }

    private void OP_xor(byte x) {
        Reg.A ^= x;
        Reg.SubtractionFlag = false;
        Reg.ZeroFlag = Reg.A == 0;
        Reg.HalfCarryFlag = false;
        Reg.CarryFlag = false;
    }

    private void OP_or(byte x) {
        Reg.A |= x;
        Reg.SubtractionFlag = false;
        Reg.ZeroFlag = Reg.A == 0;
        Reg.HalfCarryFlag = false;
        Reg.CarryFlag = false;
    }

    private void OP_cp(byte x) {
        byte a = Reg.A;
        OP_sub(x);
        Reg.A = a;
    }
    #endregion

    #region Rotate and Shift instructions
    private void OP_rlca(bool reverse = false, bool throughCarry = false) {
        var (r, l) = reverse ?
            (7, 1) :    // rotate right
            (1, 7);     // rotate left

        var prevCarry = Reg.CarryFlag ? 1 : 0;
        var carry = reverse ?
            Reg.A << 7 :    // rotate right
            Reg.A >> 7;     // rotate left
        Reg.CarryFlag = carry != 0;

        byte shifted = (byte)(reverse ?
            Reg.A >> 1 :    // rotate right
            Reg.A << 1      // rotate left
        );

        // TODO: Fixme
        // shifted |= throughCarry ?


        Reg.ZeroFlag = false;
        Reg.SubtractionFlag = false;
        Reg.HalfCarryFlag = false;

        Reg.A = shifted;
    }
    #endregion
}
