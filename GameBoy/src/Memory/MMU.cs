using GameBoy.Gamepak;

namespace GameBoy.Memory;


public class MMU(Cartridge cart) {
    private byte[] _bootrom = new byte[0x0100];

    private readonly byte[] _vram = new byte[0x2000];
    private readonly byte[] _wram = new byte[0x2000];
    private readonly byte[] _oam = new byte[0x00A0];
    private readonly byte[] _hram = new byte[0x007F];

    // TODO: IO
    // TODO: Interrupts

    public void LoadBootrom(byte[] bootrom) {
        if (bootrom.Length != 0x0100) throw new Exception("Invalid boot ROM size");
        _bootrom = bootrom;
    }

    public byte this[ushort address] {
        get => Read(address);
        set => Write(address, value);
    }

    public Cartridge GetCartridge() => cart;

    public byte Read(ushort address) => address switch {
        >= 0x0000 and < 0x0100 => _bootrom[address],                // Boot ROM
        >= 0x0100 and < 0x4000 => cart.ReadRom(address),            // ROM bank 00
        >= 0x4000 and < 0x8000 => cart.ReadRom(address),            // ROM bank 01-NN
        >= 0x8000 and < 0xA000 => _vram[address - 0x8000],          // VRAM
        >= 0xA000 and < 0xC000 => cart.ReadRam((ushort)(address - 0xA000)), // External RAM
        >= 0xC000 and < 0xD000 => _wram[address - 0xC000],          // WRAM
        >= 0xD000 and < 0xE000 => _wram[address - 0xD000],          // WRAM
        >= 0xE000 and < 0xFE00 => _wram[address - 0xE000],          // Echo RAM
        >= 0xFE00 and < 0xFEA0 => _oam[address - 0xFE00],           // OAM
        >= 0xFEA0 and < 0xFF00 => 0x00,                             // Unusable (TODO: OAM Corruption)
        >= 0xFF00 and < 0xFF80 => 0x00,                             // IO (TODO)
        >= 0xFF80 and < 0xFFFF => _hram[address - 0xFF80],          // HRAM (TODO)
        0xFFFF => 0x00,                                             // Interrupts (TODO)
    };

    public void Write(ushort address, byte value) {
        switch (address) {
            case >= 0x0000 and < 0x0100:
                break;  // Boot ROM
            case >= 0x0100 and < 0x4000:
                cart.WriteRom(address, value);
                break;  // ROM bank 00
            case >= 0x4000 and < 0x8000:
                cart.WriteRom(address, value);
                break;  // ROM bank 01-NN
            case >= 0x8000 and < 0xA000:
                _vram[address - 0x8000] = value;
                break;  // VRAM
            case >= 0xA000 and < 0xC000:
                cart.WriteRam((ushort)(address - 0xA000), value);
                break;  // External RAM
            case >= 0xC000 and < 0xD000:
                _wram[address - 0xC000] = value;
                break;  // WRAM
            case >= 0xD000 and < 0xE000:
                _wram[address - 0xD000] = value;
                break;  // WRAM
            case >= 0xE000 and < 0xFE00:
                _wram[address - 0xE000] = value;
                break;  // Echo RAM
            case >= 0xFE00 and < 0xFEA0:
                _oam[address - 0xFE00] = value;
                break;  // OAM
            case >= 0xFEA0 and < 0xFF00:
                break;  // Unusable
            case >= 0xFF00 and < 0xFF80:
                break;  // IO
            case >= 0xFF80 and < 0xFFFF:
                _hram[address - 0xFF80] = value;
                break;  // HRAM
            case 0xFFFF:
                break;  // Interrupts
        }
    }
};
