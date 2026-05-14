using BepInEx;
using UnityEngine;

namespace FNAFJumpscare;

[BepInPlugin(Constants.Guid, Constants.ModName, Constants.Version)]
public class Plugin : BaseUnityPlugin
{
    private bool initialized;
    private bool wasTagged;
    private bool triggeredThisTag;

    private void Start()
    {
        GorillaTagger.OnPlayerSpawned(() =>
        {
            initialized = true;
        });
    }

    // wow
    private void Update()
    {
        var rig = GorillaTagger.Instance?.offlineVRRig;
        if (!initialized || !rig)
            return;

        var isTagged = IsTagged();

        if (isTagged && !wasTagged)
        {
            triggeredThisTag = false;
        }

        if (isTagged && !triggeredThisTag)
        {
            triggeredThisTag = true;
            JumpscarePlayer.DoIt();
        }

        wasTagged = isTagged;
    }

    public static bool IsTagged()
    {
        var materialName = GorillaTagger.Instance?.offlineVRRig?.mainSkin?.material?.name;
        if (string.IsNullOrEmpty(materialName))
            return false;

        var name = materialName.ToLowerInvariant();

        return name.Contains("fected")
               || name.Contains("it")
               || name.Contains("stealth");
    }
}