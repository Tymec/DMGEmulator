namespace GameBoy.Emulator;


public class MMU {
    private readonly byte[] boot;
    private readonly Cartridge.Cartridge cart;

    // Memory
    private readonly byte[] vram;
    private readonly byte[] wram;
    private readonly byte[] oam;
    private readonly byte[] hram;

    // TODO: IO
    // TODO: Interrupts

    public bool HasBootRom => boot.Length > 0;

    public byte this[ushort address] {
        get => Read(address);
        set => throw new NotImplementedException();
    }

    public MMU(Cartridge.Cartridge cart, byte[]? bootRom) {
        this.cart = cart;
        boot = bootRom ?? new byte[0x0100];
        // TODO: If no boot ROM is provided, set initial state

        vram = new byte[0x2000];
        wram = new byte[0x2000];
        oam = new byte[0x00A0];
        hram = new byte[0x007F];
    }

    public byte Read(ushort address) => address switch {
        >= 0x0000 and < 0x0100 => boot[address],                    // Boot ROM
        >= 0x0000 and < 0x4000 => cart.ReadRom(address),            // ROM bank 00
        >= 0x4000 and < 0x8000 => cart.ReadRom(address),            // ROM bank 01-NN
        >= 0x8000 and < 0xA000 => vram[address - 0x8000],           // VRAM
        >= 0xA000 and < 0xC000 => cart.ReadRam((ushort)(address - 0xA000)), // External RAM
        >= 0xC000 and < 0xD000 => wram[address - 0xC000],           // WRAM
        >= 0xD000 and < 0xE000 => wram[address - 0xD000],           // WRAM
        >= 0xE000 and < 0xFE00 => wram[address - 0xE000],           // Echo RAM
        >= 0xFE00 and < 0xFEA0 => oam[address - 0xFE00],            // OAM
        >= 0xFEA0 and < 0xFF00 => 0,                                // Unusable
        >= 0xFF00 and < 0xFF80 => 0,                                // IO
        >= 0xFF80 and < 0xFFFF => hram[address - 0xFF80],           // HRAM
        0xFFFF => 0,                                                // Interrupts
    };
};
