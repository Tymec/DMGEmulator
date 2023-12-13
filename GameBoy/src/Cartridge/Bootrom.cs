namespace GameBoy.Gamepak;

using System.Security.Cryptography;

public class Bootrom(byte[] data, bool allowCustom = false) {
    public enum BootromType {
        DMG0, DMG,
        MGB,
        SGB, SGB2,
        CGB0, CGB,
        AGB0, AGB,

        STADIUM2, UNKNOWN
    }

    public byte this[int index] => data[index];
    public string Hash => BitConverter.ToString(SHA256.HashData(data)).Replace("-", "").ToLower();
    public BootromType Type => Hash switch {
        "26e71cf01e301e5dc40e987cd2ecbf6d0276245890ac829db2a25323da86818e" => BootromType.DMG0,
        "cf053eccb4ccafff9e67339d4e78e98dce7d1ed59be819d2a1ba2232c6fce1c7" => BootromType.DMG,
        "a8cb5f4f1f16f2573ed2ecd8daedb9c5d1dd2c30a481f9b179b5d725d95eafe2" => BootromType.MGB,
        "0e4ddff32fc9d1eeaae812a157dd246459b00c9e14f2f61751f661f32361e360" => BootromType.SGB,
        "fd243c4fb27008986316ce3df29e9cfbcdc0cd52704970555a8bb76edbec3988" => BootromType.SGB2,
        "3a307a41689bee99a9a32ea021bf45136906c86b2e4f06c806738398e4f92e45" => BootromType.CGB0,
        "b4f2e416a35eef52cba161b159c7c8523a92594facb924b3ede0d722867c50c7" => BootromType.CGB,
        "fe2d45405531756d87622abde6127c804bd675cb968081b2c052497a470ffeb2" => BootromType.AGB0,
        "fe3cceb79930c4cb6c6f62f742c2562fd4c96b827584ef8ea89d49b387bd6860" => BootromType.AGB,
        "7b7a881e483c0dffdb20d1e2c4dae93a04cfbb84052a91f652340f15bc315200" => BootromType.STADIUM2,
        _ when Hash.Length != 64 => throw new ArgumentException($"Invalid SHA-256: {Hash}"),
        _ => BootromType.UNKNOWN,
    };
    public int Size => Type switch {
        BootromType.DMG0 => 256,
        BootromType.DMG => 256,
        BootromType.MGB => 256,
        BootromType.SGB => 256,
        BootromType.SGB2 => 256,
        BootromType.CGB0 => 256 + 1792,
        BootromType.CGB => 256 + 1792,
        BootromType.AGB0 => 256 + 1792,
        BootromType.AGB => 256 + 1792,
        BootromType.STADIUM2 => 256 + 752,
        BootromType.UNKNOWN => -1,
        _ => throw new ArgumentException($"Invalid bootrom type: {Type}")
    };

    public static Bootrom FromFile(string path, bool allowCustom = false) {
        return new Bootrom(File.ReadAllBytes(path), allowCustom);
    }

    public bool Valid => Type == BootromType.UNKNOWN ?
        allowCustom :
        Size == data.Length;
}
