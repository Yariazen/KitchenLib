using HarmonyLib;
using Kitchen;
using UnityEngine;
using KitchenLib.Utils;

namespace KitchenLib.Customs
{
    [HarmonyPatch(typeof(PlayerView), "GetSound")]
    public class PlayerViewPatch
    {
        static void Postfix(ref AudioClip __result, int process)
        {
            if (AudioUtils.DoesProcessAudioClipExist(process))
            {
                __result = AudioUtils.GetProcessAudioClip(process);
            }
        }
    }
}