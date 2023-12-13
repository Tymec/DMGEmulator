namespace GameBoy;

using GameBoy.Gamepak;
using GameBoy.Memory;


// TODO: Rename
public class Emulator {
    private bool isRunning = false;

    private readonly CPU.CPU cpu;
    private readonly MMU mmu;

    public Emulator(string romPath, string bootRomPath) {
        // TODO: In the future, boot rom should be optional
        var cartridge = Cartridge.FromFile(romPath);
        if (!cartridge.Valid) throw new ArgumentException("Cartridge is not valid");
        var bootRom = Bootrom.FromFile(bootRomPath);
        if (!bootRom.Valid) throw new ArgumentException("Bootrom is not valid");
        mmu = new MMU(cartridge, bootRom);

        var interrupts = new Interrupts.Handler();
        cpu = new CPU.CPU(mmu, interrupts);
    }

    public static Emulator LoadRom(string romPath) { }

    public static Emulator LoadBootrom(string bootromPath) { }

    public void Run() {
        isRunning = true;
        while (isRunning) {
            cpu.Cycle();

            // For now sleep for 500ms to see what's going on
            Thread.Sleep(500);
        }
    }
}
