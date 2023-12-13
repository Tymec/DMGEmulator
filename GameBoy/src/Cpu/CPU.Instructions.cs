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
        var (sum, halfCarry, carry) = Utils.Add(reg.HL, x);

        reg.SubtractionFlag = false;
        reg.HalfCarryFlag = halfCarry;
        reg.CarryFlag = carry;

        reg.HL = sum;
    }

    private void OP_add(sbyte x) {
        var (res, halfCarry, carry) = x > 0 ?
            Utils.Add(reg.SP, (ushort)x) :
            Utils.Sub(reg.SP, (ushort)((-1) * x));

        reg.ZeroFlag = false;
        reg.SubtractionFlag = false;
        reg.HalfCarryFlag = halfCarry;
        reg.CarryFlag = carry;

        reg.SP = res;
    }
    #endregion

    #region 8-bit Arithmetic/Logic instructions
    private byte OP_inc(byte x) {
        var (sum, halfCarry, _) = Utils.Add(x, 1);

        reg.SubtractionFlag = false;
        reg.ZeroFlag = sum == 0;
        reg.HalfCarryFlag = halfCarry;

        return sum;
    }

    private byte OP_dec(byte x) {
        var (diff, halfCarry, _) = Utils.Sub(x, 1);

        reg.SubtractionFlag = true;
        reg.ZeroFlag = diff == 0;
        reg.HalfCarryFlag = halfCarry;

        return diff;
    }

    private void OP_add(byte x, bool withCarry = false) {
        var c = withCarry ? (reg.CarryFlag ? 1 : 0) : 0;
        var (sum, halfCarry, carry) = Utils.Add(reg.A, x, (byte)c);

        reg.SubtractionFlag = false;
        reg.ZeroFlag = sum == 0;
        reg.HalfCarryFlag = halfCarry;
        reg.CarryFlag = carry;

        reg.A = sum;
    }

    private void OP_sub(byte x, bool withCarry = false) {
        var c = withCarry ? (reg.CarryFlag ? 1 : 0) : 0;
        var (diff, halfCarry, carry) = Utils.Sub(reg.A, x, (byte)c);

        reg.SubtractionFlag = true;
        reg.ZeroFlag = diff == 0;
        reg.HalfCarryFlag = halfCarry;
        reg.CarryFlag = carry;

        reg.A = diff;
    }

    private void OP_and(byte x) {
        reg.A &= x;
        reg.SubtractionFlag = false;
        reg.ZeroFlag = reg.A == 0;
        reg.HalfCarryFlag = true;
        reg.CarryFlag = false;
    }

    private void OP_xor(byte x) {
        reg.A ^= x;
        reg.SubtractionFlag = false;
        reg.ZeroFlag = reg.A == 0;
        reg.HalfCarryFlag = false;
        reg.CarryFlag = false;
    }

    private void OP_or(byte x) {
        reg.A |= x;
        reg.SubtractionFlag = false;
        reg.ZeroFlag = reg.A == 0;
        reg.HalfCarryFlag = false;
        reg.CarryFlag = false;
    }

    private void OP_cp(byte x) {
        byte a = reg.A;
        OP_sub(x);
        reg.A = a;
    }
    #endregion

    #region Rotate and Shift instructions
    private void OP_rlca(bool reverse = false, bool throughCarry = false) {
        var (r, l) = reverse ?
            (7, 1) :    // rotate right
            (1, 7);     // rotate left

        var prevCarry = reg.CarryFlag ? 1 : 0;
        var carry = reverse ?
            reg.A << 7 :    // rotate right
            reg.A >> 7;     // rotate left
        reg.CarryFlag = carry != 0;

        byte shifted = (byte)(reverse ?
            reg.A >> 1 :    // rotate right
            reg.A << 1      // rotate left
        );

        // TODO: Fixme
        // shifted |= throughCarry ?


        reg.ZeroFlag = false;
        reg.SubtractionFlag = false;
        reg.HalfCarryFlag = false;

        reg.A = shifted;
    }
    #endregion
}
