using System.Text;

namespace GameBoy.Emulator.Cartridge;


public abstract class Cartridge {
    protected readonly byte[] data;
    protected readonly int romSize;
    protected readonly int ramSize;
    protected readonly int romBanks;
    protected readonly int ramBanks;

    private ushort FirstEntryInstruction => (ushort)((data[0x0101] << 8) | data[0x0100]);
    private ushort SecondEntryInstruction => (ushort)((data[0x0103] << 8) | data[0x0102]);
    public ushort EntryPoint => FirstEntryInstruction == 0x00 ? SecondEntryInstruction : FirstEntryInstruction;

    public byte[] Logo => data[0x0104..0x0133];
    // TODO: Depending on the cartridge type, the title can be 11 or 15 characters long
    public string Title => Encoding.ASCII.GetString(
        IsCGB ? data[0x0134..0x0142] : data[0x0134..0x0143]
    ).TrimEnd('\0');
    public string ManufacturerCode => Encoding.ASCII.GetString(data[0x013F..0x0142]).TrimEnd('\0');
    public bool IsCGB => data[0x0143] == 0x80 || data[0x0143] == 0xC0;
    public bool IsSGB => data[0x0146] == 0x03;
    public bool IsJapanese => data[0x014A] == 0x00;
    public string LicenseeCode => Encoding.ASCII.GetString(
        OldLicenseeCode == 0x33 ? data[0x0144..0x0146] : [OldLicenseeCode]
    ).TrimEnd('\0');
    public byte VersionNumber => data[0x014C];
    public bool IsHeaderChecksumValid => data[0x014D] == CalculateHeaderChecksum();
    public ushort GlobalChecksum => (ushort)((data[0x014E] << 8) | data[0x014F]);

    private byte CartridgeType => data[0x0147];
    private byte ROMSize => data[0x0148];
    private byte RAMSize => data[0x0149];
    private byte OldLicenseeCode => data[0x014B];

    public Cartridge(byte[] data) {
        this.data = data;

        // NOTE: Missing implementation of $52, $53 and $54 (https://gbdev.io/pandocs/The_Cartridge_Header.html#0148--rom-size)
        romSize = 0x8000 * (1 << ROMSize);
        // NOTE: Missing implementation of $01 (https://gbdev.io/pandocs/The_Cartridge_Header.html#0149--ram-size)
        ramSize = RAMSize switch {
            0x00 => 0x0,
            0x02 => 0x2000,
            0x03 => 0x8000,
            0x04 => 0x20000,
            0x05 => 0x10000,
            _ => throw new Exception("Unsupported RAM size")
        };

        romBanks = romSize / 0x4000;
        ramBanks = ramSize / 0x2000;
    }

    public static Cartridge FromFile(string path) {
        var data = File.ReadAllBytes(path);

        byte cartridgeType = data[0x0147];
        return cartridgeType switch {
            0x00 => new NoMBC(data),
            _ => throw new Exception("Unsupported cartridge type")
        };
    }

    public virtual byte ReadRom(ushort address) => data[address];
    public virtual byte ReadRam(ushort address) => 0xFF;

    public virtual void WriteRom(ushort address, byte value) { }
    public virtual void WriteRam(ushort address, byte value) { }

    private byte CalculateHeaderChecksum() {
        byte checksum = 0;
        for (int i = 0x0134; i <= 0x014C; i++) {
            checksum = (byte)(checksum - data[i] - 1);
        }
        return checksum;
    }
}
