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

        SetupUserDirectory();

        _mmu = new MMU(cartridge);
        var interrupts = new Interrupts.Handler();
        _cpu = new CPU(_mmu, interrupts);
    }

    public Emulator(string romPath, string? bootromPath = null) {
        SetupUserDirectory();

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

    public void DumpRom() {
        List<string> instructions = [];

        Cartridge cartridge = _mmu.GetCartridge();
        byte[] rom = cartridge.GetData();
        for (int i = 0; i < rom.Length; i++) {
            byte opcode = _mmu[(ushort)i];

            Opcode op;
            if (opcode == 0xCB) {
                opcode = _mmu[(ushort)(++i)];
                op = _cpu.GetOpcode(opcode, true);
            } else {
                op = _cpu.GetOpcode(opcode);
            }

            string instr = op.Mnemonic;
            if (op.Length == 2) {
                byte operand = _mmu[(ushort)(++i)];
                instr = instr
                    .Replace("FF00+u8", $"{0xFF00 + operand:X4}")
                    .Replace("u8", $"0x{operand:X2}")
                    .Replace("i8", $"0x{(sbyte)operand:X2}");
            } else if (op.Length == 3) {
                byte operand1 = _mmu[(ushort)(++i)];
                byte operand2 = _mmu[(ushort)(++i)];
                instr = instr
                    .Replace("u16", $"0x{operand1:X2}{operand2:X2}");
            }

            instructions.Add(instr);
        }

        File.WriteAllLines($"{Constants.DUMP_DIR}/{cartridge.Title}.asm", instructions);
    }

    private static void SetupUserDirectory() {
        if (!Directory.Exists(Constants.USER_DIR)) {
            Directory.CreateDirectory(Constants.USER_DIR);
        }

        if (!Directory.Exists(Constants.ROM_DIR)) {
            Directory.CreateDirectory(Constants.ROM_DIR);
        }

        if (!Directory.Exists(Constants.DUMP_DIR)) {
            Directory.CreateDirectory(Constants.DUMP_DIR);
        }

        if (!Directory.Exists(Constants.BOOT_DIR)) {
            Directory.CreateDirectory(Constants.BOOT_DIR);
        }
    }
}
