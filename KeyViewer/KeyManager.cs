﻿using KeyViewer.API;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace KeyViewer
{
    public class KeyManager : MonoBehaviour, IDisposable
    {
        public Key this[KeyCode code]
        {
            get => keys[code];
            set => keys[code] = value;
        }
        public Key this[SpecialKeyType type]
        {
            get => specialKeys[type];
            set => specialKeys[type] = value;
        }
        public Profile Profile
        {
            get => profile;
            set
            {
                profile = value;
                KPSCalculator.Start(value);
                UpdateKeys();
            }
        }
        public IEnumerable<KeyCode> Codes => keys.Keys;
        public IEnumerable<Key> Keys => keys.Values;
        Profile profile;
        internal Canvas keysCanvas;
        internal Dictionary<KeyCode, Key> keys;
        internal Dictionary<SpecialKeyType, Key> specialKeys;
        internal RectTransform keysCanvasRt;
        public void Init(Profile profile)
        {
            DontDestroyOnLoad(gameObject);
            Canvas mainCanvas = gameObject.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.referenceResolution = new Vector2(1280, 720);
            Profile = profile;
        }
        public void UpdateKeys()
        {
            if (keysCanvas)
                Destroy(keysCanvas.gameObject);
            GameObject keysObject = new GameObject("Canvas");
            keysObject.transform.SetParent(transform);
            keysCanvas = keysObject.AddComponent<Canvas>();
            keysCanvasRt = keysCanvas.GetComponent<RectTransform>();
            keys = new Dictionary<KeyCode, Key>();
            specialKeys = new Dictionary<SpecialKeyType, Key>();
            foreach (Key.Config config in Profile.ActiveKeys.Where(c => c.Code != KeyCode.None))
                if (!keys.ContainsKey(config.Code))
                    keys.Add(config.Code, new GameObject($"{config.Code} Key").AddComponent<Key>().Init(this, config));
                else
                {
                    var resolvedCode = Main.KeyCodes.First(k => Profile.ActiveKeys.FirstOrDefault(kc => kc.Code == k) == null);
                    keys.Add(resolvedCode, new GameObject($"{config.Code} Key").AddComponent<Key>().Init(this, config));
                    Main.Logger.Log($"{config.Code} Is Duplicate! One Of {config.Code} Key Was Changed To {resolvedCode}!");
                }
            foreach (Key.Config config in Profile.ActiveKeys.Where(c => c.SpecialType != SpecialKeyType.None))
                specialKeys.Add(config.SpecialType, new GameObject($"{config.SpecialType} Key").AddComponent<Key>().Init(this, config));
            if (!Main.Settings.CurrentProfile.EditEachKeys)
                Main.ApplyEachKeys(Profile.GlobalConfig);
            UpdateLayout();
        }
        public void UpdateLayout()
        {
            EventAPI.UpdateLayout(this);
            int count = keys.Count;
            float keyHeight = Profile.ShowKeyPressTotal ? 150 : 100;
            float spacing = 10;
            float width = count * 100 + (count - 1) * spacing;

            Vector2 pos = new Vector2(Profile.KeyViewerXPos, Profile.KeyViewerYPos);
            keysCanvasRt.anchorMin = pos;
            keysCanvasRt.anchorMax = pos;
            keysCanvasRt.pivot = pos;
            keysCanvasRt.sizeDelta = new Vector2(width, keyHeight);
            keysCanvasRt.anchoredPosition = Vector2.zero;
            keysCanvasRt.localScale = new Vector3(1, 1, 1) * Profile.KeyViewerSize / 100f;

            float x = 0;
            float tempX = 0;
            int updateCount = 0;
            foreach (Key key in keys.Values)
                key.UpdateLayout(ref x, ref tempX, updateCount++);
            tempX = 0;
            updateCount = 0;
            foreach (Key key in specialKeys.Values)
                key.UpdateLayout(ref x, ref tempX, updateCount++);
        }
        public bool isPlaying;
        public void ClearCounts()
        {
            EventAPI.ClearCounts();
            foreach (Key key in keys.Values)
            {
                key.Count = 0;
                key.CountText.text = "0";
            }
            if (specialKeys.TryGetValue(SpecialKeyType.Total, out Key total))
            {
                total.Count = 0;
                total.CountText.text = "0";
            }
        }
        public void Dispose()
        {
            Destroy(gameObject);
            keys.Clear();
            keys = null;
            GC.SuppressFinalize(this);
        }
    }
}
