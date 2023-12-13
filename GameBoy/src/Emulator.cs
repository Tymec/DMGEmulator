namespace GameBoy;

using GameBoy.Gamepak;
using GameBoy.Memory;
using GameBoy.Cpu;


public class Emulator {
    private bool _running = false;

    private readonly CPU cpu;
    private readonly MMU mmu;

    public Emulator(Cartridge cartridge) {
        if (!cartridge.Valid) throw new Exception("Invalid cartridge");

        mmu = new MMU(cartridge);
        var interrupts = new Interrupts.Handler();
        cpu = new CPU(mmu, interrupts);
    }

    public Emulator(string romPath, string? bootromPath = null) {
        var cartridge = Cartridge.FromFile(romPath);

        mmu = new MMU(cartridge);
        var interrupts = new Interrupts.Handler();
        cpu = new CPU(mmu, interrupts);

        if (bootromPath != null) {
            byte[] bootrom = File.ReadAllBytes(bootromPath);
            mmu.LoadBootrom(bootrom);
        } else {
            throw new NotImplementedException();
        }
    }

    public void Run() {
        _running = true;
        while (_running) {
            cpu.Cycle();

            // For now sleep for 500ms to see what's going on
            Thread.Sleep(500);
        }
    }
}
