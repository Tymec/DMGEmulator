using GameBoy.Emulator.Cartridge;

namespace GameBoy.ConsoleApp;


public class Program {
    public static void Main(string[] args) {
        Cartridge cartridge = Cartridge.FromFile("Roms/*");

        Console.WriteLine($"Entry point: 0x{cartridge.EntryPoint:X4}");
        Console.WriteLine($"Title: {cartridge.Title}");
        Console.WriteLine($"Manufacturer code: {cartridge.ManufacturerCode}");
        Console.WriteLine($"CGB flag: {cartridge.IsCGB}");
        Console.WriteLine($"New Licensee code: {cartridge.LicenseeCode}");
        Console.WriteLine($"SGB flag: {cartridge.IsSGB}");
        Console.WriteLine($"Cartridge type: {cartridge.GetType().Name}");
        Console.WriteLine($"Japanese: {cartridge.IsJapanese}");
        Console.WriteLine($"Version number: {cartridge.VersionNumber}");
        Console.WriteLine($"Header checksum: {cartridge.IsHeaderChecksumValid}");
        Console.WriteLine($"Global checksum: {cartridge.GlobalChecksum}");
    }
}
