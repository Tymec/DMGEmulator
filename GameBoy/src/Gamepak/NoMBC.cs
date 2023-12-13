namespace GameBoy.Gamepak;


public class NoMBC(byte[] data) : Cartridge(data) {
    public override byte ReadRom(ushort address) => data[address];
    public override byte ReadRam(ushort address) => 0xFF;
}
