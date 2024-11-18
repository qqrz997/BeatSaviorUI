using System;
using HarmonyLib;
using JetBrains.Annotations;
using Zenject;

namespace BeatSaviorUI.HarmonyPatches;

[UsedImplicitly]
public class HarmonyPatchController : IInitializable, IDisposable
{
    private Harmony Harmony { get; } = new(Plugin.Metadata.Name);
    
    public void Initialize()
    {
        Harmony.PatchAll(Plugin.ExecutingAssembly);
    }

    public void Dispose()
    {
        Harmony.UnpatchSelf();
    }
}