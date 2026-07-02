using System.Runtime.InteropServices;
using DisplayConfigExplorer.Native;

Console.WriteLine("====================================");
Console.WriteLine(" DisplayConfig Explorer");
Console.WriteLine("====================================");
Console.WriteLine();

Console.WriteLine("Le Sandbox est prêt.");

Console.WriteLine();
Console.WriteLine($"OS : {RuntimeInformation.OSDescription}");
Console.WriteLine($".NET : {Environment.Version}");
Console.WriteLine($"Architecture : {RuntimeInformation.ProcessArchitecture}");

const uint QDC_ONLY_ACTIVE_PATHS = 0x00000002;

int result = NativeMethods.GetDisplayConfigBufferSizes(
    QDC_ONLY_ACTIVE_PATHS,
    out uint pathCount,
    out uint modeCount);

Console.WriteLine();
Console.WriteLine($"Résultat : {result}");
Console.WriteLine($"Chemins  : {pathCount}");
Console.WriteLine($"Modes    : {modeCount}");

Console.ReadKey();