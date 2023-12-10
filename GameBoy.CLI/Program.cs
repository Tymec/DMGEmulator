using GameBoy.Gamepak;

namespace GameBoy.ConsoleApp;


public class Program {
    public static void Main(string[] args) {
        string romPath = args[0];
        string? bootRomPath = args.Length > 1 ? args[1] : null;

        if (!TestRom(romPath)) {
            return;
        }

        if (bootRomPath != null && !TestBootRom(bootRomPath)) {
            return;
        }

        Emulator emulator = new(romPath, bootRomPath);
        emulator.Run();
    }

    private static bool TestRom(string romPath) {
        try {
            Cartridge cartridge = Cartridge.FromFile(romPath);
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
            return true;
        } catch (Exception e) {
            Console.WriteLine(e);
            return false;
        }
    }

    private static bool TestBootRom(string bootRomPath) {
        try {
            byte[] bootRom = File.ReadAllBytes(bootRomPath);
            Console.WriteLine($"Boot ROM size: {bootRom.Length}");
            return bootRom.Length == 256;
        } catch (Exception e) {
            Console.WriteLine(e);
            return false;
        }
    }
}
