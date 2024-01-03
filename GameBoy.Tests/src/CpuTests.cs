using System.Reflection;
using Newtonsoft.Json;
using Xunit.Abstractions;
using GameBoy.Cpu;
using GameBoy.Memory;
using GameBoy.Interrupts;
using GameBoy.Gamepak;

namespace GameBoy.Tests;

public class CpuTests : IDisposable {
    private readonly ITestOutputHelper _output;
    private readonly CPU _cpu;

    public CpuTests(ITestOutputHelper testOutputHelper) {
        _output = testOutputHelper;

        Cartridge cart = new NoMBC(new byte[0x8000]);
        MMU mmu = new MMU(cart);
        Handler interrupts = new Handler();
        _cpu = new CPU(mmu, interrupts);
    }

    public void Dispose() {
        GC.SuppressFinalize(this);
    }

    public static JsonInstructionSet GetInstructionSet() {
        Assembly assembly = Assembly.GetExecutingAssembly();

        using Stream stream = assembly.GetManifestResourceStream("GameBoy.Tests.Resources.opcodes.json")!;
        using StreamReader reader = new StreamReader(stream);
        string json = reader.ReadToEnd();
        return JsonConvert.DeserializeObject<JsonInstructionSet>(json)!;
    }

    [Fact]
    public void TestOpcodeBuilder() {
        OpcodeBuilder builder = new OpcodeBuilder(_cpu);
        Opcode[] unprefixed = builder.BuildUnprefixed();
        Opcode[] prefixed = builder.BuildPrefixed();

        Assert.Equal(256, unprefixed.Length);
        Assert.Equal(256, prefixed.Length);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TestOpcodes(bool prefixed) {
        JsonInstructionSet instructionSet = GetInstructionSet();
        OpcodeBuilder builder = new OpcodeBuilder(_cpu);

        Opcode[] opcodes = prefixed ? builder.BuildPrefixed() : builder.BuildUnprefixed();
        Dictionary<string, JsonOpcode> jsonOpcodes = prefixed ? instructionSet.Prefixed : instructionSet.Unprefixed;

        int i = 0;
        foreach (KeyValuePair<string, JsonOpcode> entry in jsonOpcodes) {
            JsonOpcode expected = entry.Value;
            Opcode actual = opcodes[Convert.ToByte(entry.Key, 16)];

            i++;

            if (expected.Invalid) {
                Assert.True(actual.Mnemonic == "INVALID");
                continue;
            }

            string ExpectedMnemonic = expected.Mnemonic;

            if (expected.Operand1 != null) {
                ExpectedMnemonic += " " + expected.Operand1;
                if (expected.Operand2 != null) {
                    ExpectedMnemonic += ", " + expected.Operand2;
                }
            }

            // if (
            //     expected.Cycles.Length > 1 && (
            //         expected.Cycles[0] != ((ConditionalOpcode)actual).CyclesBranch ||
            //         expected.Cycles[1] != actual.Cycles
            //     )
            // ) {
            //     var opcode = (ConditionalOpcode)actual;
            //     _output.WriteLine($"{entry.Key} {expected.Mnemonic}: taken ({expected.Cycles[0]}) != {opcode.CyclesBranch}) or not taken ({expected.Cycles[1]}) != {opcode.Cycles}) (exp vs act)");
            // } else if (expected.Cycles.Length == 1 && expected.Cycles[0] != actual.Cycles) {
            //     _output.WriteLine($"{entry.Key} {expected.Mnemonic}: ({expected.Cycles[0]} != {actual.Cycles}) (exp vs act)");
            // }

            Assert.Equal(ExpectedMnemonic, actual.Mnemonic);
            Assert.Equal(expected.Length, actual.Length);
            if (expected.Cycles.Length > 1) {
                Assert.True(actual is ConditionalOpcode);
                Assert.Equal(expected.Cycles[0], ((ConditionalOpcode)actual).CyclesBranch);
                Assert.Equal(expected.Cycles[1], actual.Cycles);
            } else {
                Assert.Equal(expected.Cycles[0], actual.Cycles);
            }
        }

        Assert.Equal(256, i);
    }

    // Definition of JSON instruction set
    public class JsonInstructionSet {
        [JsonProperty("unprefixed")]
        public Dictionary<string, JsonOpcode> Unprefixed { get; set; } = [];

        [JsonProperty("cbprefixed")]
        public Dictionary<string, JsonOpcode> Prefixed { get; set; } = [];
    }

    public class JsonOpcode {
        public string Mnemonic { get; set; } = "";
        public int Length { get; set; }
        public int[] Cycles { get; set; } = [];
        public string[] Flags { get; set; } = [];
        public string Addr { get; set; } = "";
        public string Group { get; set; } = "";
        public bool Invalid { get; set; } = false;

        [JsonProperty("operand1")]
        public string? Operand1 { get; set; }

        [JsonProperty("operand2")]
        public string? Operand2 { get; set; }
    }
}
