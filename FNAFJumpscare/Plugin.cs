using BepInEx;
using UnityEngine;

namespace FNAFJumpscare;

[BepInPlugin(Constants.Guid, Constants.ModName, Constants.Version)]
public class Plugin : BaseUnityPlugin
{
    private bool initialized;
    private bool wasTagged;
    private float nextAllowedTime;

    private const float Cooldown = 3f;

    private void Start()
    {
        GorillaTagger.OnPlayerSpawned(() => initialized = true);
    }
    
    // yes i know this is horrible submit a pr idc gorilla tag's tag shit is ass
    private void Update()
    {
        if (!initialized || !VRRig.LocalRig)
            return;

        var isTagged = IsTagged();

        if (!isTagged && wasTagged && Time.time >= nextAllowedTime)
        {
            nextAllowedTime = Time.time + Cooldown;
            JumpscarePlayer.DoIt();
        }

        wasTagged = isTagged;
    }

    private static bool IsTagged()
    {
        var material = VRRig.LocalRig?.mainSkin?.material;
        if (!material)
            return false;

        var name = material.name;
        return name.Contains("fected") || name.Contains("it") || name.Contains("stealth");
    }
}