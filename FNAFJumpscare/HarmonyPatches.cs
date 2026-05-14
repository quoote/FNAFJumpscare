using System.Reflection;

namespace FNAFJumpscare;

internal static class HarmonyPatches
{
    private static HarmonyLib.Harmony? _harmony;

    internal static void ApplyHarmonyPatches()
    {
        _harmony = new HarmonyLib.Harmony(Constants.Guid);
        _harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    internal static void RemoveHarmonyPatches()
    {
        _harmony?.UnpatchSelf();
    }
}