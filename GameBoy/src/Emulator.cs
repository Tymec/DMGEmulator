namespace GameBoy;

using GameBoy.Gamepak;
using GameBoy.Memory;
using GameBoy.Cpu;


public class Emulator {
    private bool _running = false;

    private readonly CPU _cpu;
    private readonly MMU _mmu;

    public Emulator(Cartridge cartridge) {
        if (!cartridge.Valid) throw new Exception("Invalid cartridge");

        _mmu = new MMU(cartridge);
        var interrupts = new Interrupts.Handler();
        _cpu = new CPU(_mmu, interrupts);
    }

    public Emulator(string romPath, string? bootromPath = null) {
        var cartridge = Cartridge.FromFile(romPath);

        _mmu = new MMU(cartridge);
        var interrupts = new Interrupts.Handler();
        _cpu = new CPU(_mmu, interrupts);

        if (bootromPath != null) {
            byte[] bootrom = File.ReadAllBytes(bootromPath);
            _mmu.LoadBootrom(bootrom);
        } else {
            throw new NotImplementedException();
        }
    }

    public void Run() {
        _running = true;
        while (_running) {
            _cpu.Cycle();

            // For now sleep for 500ms to see what's going on
            Thread.Sleep(500);
        }
    }
}
