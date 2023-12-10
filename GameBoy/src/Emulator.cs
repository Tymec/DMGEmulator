namespace GameBoy;


// TODO: Rename
public class Emulator {
    private bool isRunning = false;

    private readonly Cartridge.Cartridge cartridge;
    private readonly CPU.CPU cpu;
    private readonly Memory.MMU mmu;
    private readonly Interrupts.Handler interrupts;

    public Emulator(string romPath, string? bootRomPath = null) {
        cartridge = Cartridge.Cartridge.FromFile(romPath);
        mmu = new Memory.MMU(cartridge, LoadBootRom(bootRomPath));
        interrupts = new Interrupts.Handler();
        cpu = new CPU.CPU(mmu, interrupts);
    }

    public static byte[]? LoadBootRom(string? path) {
        return path != null ? File.ReadAllBytes(path) : null;
    }

    public void Run() {
        isRunning = true;
        while (isRunning) {
            cpu.Cycle();

            // For now sleep for 500ms to see what's going on
            Thread.Sleep(500);
        }
    }
}
