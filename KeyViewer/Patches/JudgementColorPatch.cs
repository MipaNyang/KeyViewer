using HarmonyLib;
using KeyViewer.API;
using SkyHook;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KeyViewer.Patches
{
    [HarmonyPatch]
    public static class JudgementColorPatch
    {
        static bool initialized = false;
        static HashSet<Key> keys = new HashSet<Key>();
        public static void Init()
        {
            if (initialized) return;
            //keys = new ShiftStack<KeyCode>(() => Math.Max(Main.KeyManager?.keys.Count ?? 0, 2), KeyCode.None);
            SkyHookManager.KeyUpdated.AddListener(she =>
            {
                try
                {
                    if (!AsyncInputManager.isActive) return;
                    if (Main.KeyManager == null) return;
                    if (she.Type == SkyHook.EventType.KeyReleased) return;
                    if (Main.IsEnabled && (scrController.instance?.gameworld ?? false))
                    {
                        var code = AsyncInputCompat.Convert(she.Label);
                        if (Main.KeyManager.Codes.Contains(code))
                            keys.Add(Main.KeyManager[code]);
                    }
                }
                catch { }
            });
            initialized = true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(scrController), "ValidInputWasTriggered")]
        public static void SyncInput_StackPushPatch(scrController __instance)
        {
            if (!initialized) return;
            if (AsyncInputManager.isActive) return;
            foreach (var code in Main.KeyManager.Codes)
                if (Input.GetKeyDown(code))
                    keys.Add(Main.KeyManager[code]);
        }
        static bool stackFlushed = false;
        static bool rainRestored = false;
        static States prevState = States.None;
        [HarmonyPrefix]
        [HarmonyPatch(typeof(scrController), "Update")]
        public static void StackFlushPatch(scrController __instance)
        {
            if (!initialized) return;
            var state = __instance.state;
            if (!stackFlushed && state == States.PlayerControl)
            {
                keys.Clear();
                stackFlushed = true;
                rainRestored = false;
            }
            if (!rainRestored && state != prevState)
            {
                foreach (var activeKey in Main.KeyManager.Keys)
                    if (activeKey.config.RainEnabled)
                        activeKey.rains.ForEach(kr => kr.image.color = activeKey.config.RainConfig.RainColor);
                rainRestored = true;
            }
            prevState = state;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(scrController), "Awake_Rewind")]
        public static void StackFlushPatch2(scrController __instance)
        {
            if (!initialized) return;
            stackFlushed = false;
        }
        [Comment("Change Key's Background Color")]
        [HarmonyPrefix]
        [HarmonyPatch(typeof(scrMistakesManager), "AddHit")]
        public static void ChangeColorPatch(HitMargin hit)
        {
            if (!initialized) return;
            foreach (var key in keys)
                key.ChangeHitMarginColor(hit);
            keys.Clear();
        }
    }
}
