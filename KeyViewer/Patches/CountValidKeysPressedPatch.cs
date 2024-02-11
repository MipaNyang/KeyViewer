using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace KeyViewer.Patches
{
    [HarmonyPatch(typeof(scrController), "CountValidKeysPressed")]
    public static class CountValidKeysPressedPatch
    {
        public static readonly List<KeyCode> AlwaysBoundKeys = new List<KeyCode>()
        {
            KeyCode.Mouse0,
            KeyCode.Mouse1,
            KeyCode.Mouse2,
            KeyCode.Mouse3,
            KeyCode.Mouse4,
        };
        public static bool Prefix(ref int __result)
        {
            if (Main.IsListening)
            {
                __result = 0;
                return false;
            }
            return true;
        }
    }
}
