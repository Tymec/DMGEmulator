namespace GameBoy;


public static class Utils {
    public static (byte sum, bool halfCarry, bool carry) Add(params byte[] values) {
        byte sum = 0;
        bool halfCarry = false;
        bool carry = false;
        foreach (var value in values) {
            sum += value;
            halfCarry |= (sum & 0xF0) != 0;
            carry |= (sum & 0xFF00) != 0;
        }
        return (sum, halfCarry, carry);
    }

    public static (ushort sum, bool halfCarry, bool carry) Add(params ushort[] values) {
        ushort sum = 0;
        bool halfCarry = false;
        bool carry = false;
        foreach (var value in values) {
            sum += value;
            halfCarry |= (sum & 0xF000) != 0;
            carry |= (sum & 0xFFFF0000) != 0;
        }
        return (sum, halfCarry, carry);
    }

    public static (byte difference, bool halfCarry, bool carry) Sub(params byte[] values) {
        byte difference = 0;
        bool halfCarry = false;
        bool carry = false;
        foreach (var value in values) {
            difference -= value;
            halfCarry |= (difference & 0xF0) != 0;
            carry |= (difference & 0xFF00) != 0;
        }
        return (difference, halfCarry, carry);
    }

    public static (ushort difference, bool halfCarry, bool carry) Sub(params ushort[] values) {
        ushort difference = 0;
        bool halfCarry = false;
        bool carry = false;
        foreach (var value in values) {
            difference -= value;
            halfCarry |= (difference & 0xF000) != 0;
            carry |= (difference & 0xFFFF0000) != 0;
        }
        return (difference, halfCarry, carry);
    }
}
