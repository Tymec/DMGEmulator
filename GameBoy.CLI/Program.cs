namespace GameBoy.ConsoleApp;

using CommandLine;
using GameBoy.Gamepak;

public class Program {
    public class Options {
        [Option('r', "rom", HelpText = "Path to the ROM file.")]
        public required string RomPath { get; set; }

        // TODO: In the future, the boot ROM should be optional
        [Option('b', "boot-rom", HelpText = "Path to the boot ROM file.")]
        public required string BootromPath { get; set; }
    }

    public static void Main(string[] args) => Parser.Default
        .ParseArguments<Options>(args)
        .WithParsed(RunWithOptions)
        .WithNotParsed(HandleParseError);

    private static void RunWithOptions(Options opts) {
        Console.WriteLine($"ROM path: {opts.RomPath}");
        Console.WriteLine($"Boot ROM path: {opts.BootromPath}");

        if (!TestRom(opts.RomPath) || !TestBootRom(opts.BootromPath)) {
            return;
        }

        Emulator emulator = new(opts.RomPath, opts.BootromPath);
        emulator.Run();
    }

    private static void HandleParseError(IEnumerable<Error> errs) {
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
