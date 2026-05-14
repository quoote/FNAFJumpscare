using System;
using BepInEx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FNAFJumpscare;

[BepInPlugin(Constants.Guid, Constants.ModName, Constants.Version)]
public class Plugin : BaseUnityPlugin
{
    private bool Initialized;
    private int Chance = 400;
    private float Timer;

    private void Start()
    {
        GorillaTagger.OnPlayerSpawned(() =>
        {
            Initialized = true;
        });
    }

    private void Update()
    {
        if (!Initialized || !GorillaTagger.Instance?.offlineVRRig)
            return;

        Timer += Time.deltaTime;

        if (Timer >= 1f)
        {
            Timer = 0f;
            
            if (Random.Range(0, Chance) == 0)
                JumpscarePlayer.DoIt();
        }
    }
}