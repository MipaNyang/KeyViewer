using Newgrounds;
using System.Xml.Serialization;
using UnityEngine;

namespace KeyViewer
{
    public partial class KeyRain
    {
        public class RainConfig
        {
            public float OffsetX = 0f;
            public float OffsetY = 0f;
            public float RainSpeed = 400f;
            public float RainWidth = -1f;
            public float RainHeight = -1f;
            public float RainLength = 400f;
            public int Softness = 100;
            public Color RainColor = Color.white;
            public string[] RainImages = new string[0];
            public int RainPoolSize = 25;
            public int[] RainImageCounts = new int[0];
            public bool SequentialImages = true;
            public bool ShuffleImages = false;
            public Direction Direction = Direction.U;

            [XmlIgnore]
            public bool ColorExpanded = false;
            [XmlIgnore]
            public int CurrentImageIndex = 0;

            public RainConfig Copy()
            {
                RainConfig newConfig = new RainConfig();
                newConfig.OffsetX = OffsetX;
                newConfig.OffsetY = OffsetY;
                newConfig.RainSpeed = RainSpeed;
                newConfig.RainWidth = RainWidth;
                newConfig.RainHeight = RainHeight;
                newConfig.RainLength = RainLength;
                newConfig.RainColor = RainColor;
                newConfig.RainImages = (string[])RainImages.Clone();
                newConfig.RainImageCounts = (int[])RainImageCounts.Clone();
                newConfig.ShuffleImages = ShuffleImages;
                newConfig.SequentialImages = SequentialImages;
                newConfig.Softness = Softness;
                newConfig.Direction = Direction;
                return newConfig;
            }

            internal Sprite GetRainImage() => RainImages.Length < 1 ? null : Main.GetSprite(RainImages[GetRainImageIndex()]);
            internal Sprite GetRainImageRandomly() => RainImages.Length < 1 ? null : Main.GetSprite(RainImages[(int)((RainImages.Length - 1) * Random.value)]);
            private int GetRainImageIndex()
            {
                if (CurrentImageIndex < RainImages.Length)
                    return CurrentImageIndex++;
                 var ret = CurrentImageIndex = 0;
                CurrentImageIndex++;
                return ret;
            }
        }
    }
}
