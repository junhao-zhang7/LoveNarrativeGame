using System;
using UnityEngine;

namespace LoveNarrativeGame.Prototype
{
    public enum MenuLanguage { SimplifiedChinese, TraditionalChinese, English, Japanese }
    public static class MenuPreferences
    {
        private const string Prefix = "SchoolPrototype.Settings.";
        public static event Action AudioChanged;
        public static float Music => NormalizeVolume(PlayerPrefs.GetFloat(Prefix + "Music", .8f));
        public static float Sound => NormalizeVolume(PlayerPrefs.GetFloat(Prefix + "Sound", .8f));
        public static bool Fullscreen => PlayerPrefs.GetInt(Prefix + "Fullscreen", 1) == 1;
        public static MenuLanguage Language => NormalizeLanguage(PlayerPrefs.GetInt(Prefix + "Language", 0));
        public static int Width => Mathf.Clamp(PlayerPrefs.GetInt(Prefix + "Width", 1920), 640, 7680);
        public static int Height => Mathf.Clamp(PlayerPrefs.GetInt(Prefix + "Height", 1080), 360, 4320);
        public static float NormalizeVolume(float value) => float.IsNaN(value) || float.IsInfinity(value) ? .8f : Mathf.Clamp01(value);
        public static MenuLanguage NormalizeLanguage(int value) => value >= 0 && value < 4 ? (MenuLanguage)value : MenuLanguage.SimplifiedChinese;
        public static void Apply()
        {
            // One-time migration requested by the author. Later player choices remain persistent.
            if (PlayerPrefs.GetInt(Prefix + "DisplayDefaultV4", 0) == 0)
            {
                PlayerPrefs.SetInt(Prefix + "Fullscreen", 1);
                PlayerPrefs.SetInt(Prefix + "Width", 1920);
                PlayerPrefs.SetInt(Prefix + "Height", 1080);
                PlayerPrefs.SetInt(Prefix + "DisplayDefaultV4", 1); PlayerPrefs.Save();
            }
            AudioListener.volume = 1;
            AudioChanged?.Invoke();
            ApplyDisplay();
        }
        private static void ApplyDisplay()
        {
#if !UNITY_EDITOR
            Screen.SetResolution(Width, Height, Fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
#endif
        }
        public static void SetMusic(float value)
        { PlayerPrefs.SetFloat(Prefix + "Music", NormalizeVolume(value)); PlayerPrefs.Save(); AudioChanged?.Invoke(); }
        public static void SetSound(float value)
        { PlayerPrefs.SetFloat(Prefix + "Sound", NormalizeVolume(value)); PlayerPrefs.Save(); AudioChanged?.Invoke(); }
        public static void SetFullscreen(bool value)
        {
            PlayerPrefs.SetInt(Prefix + "Fullscreen", value ? 1 : 0); PlayerPrefs.Save(); ApplyDisplay();
        }
        public static void SetResolution(int width, int height)
        {
            if (width < 640 || height < 360 || width > 7680 || height > 4320) return;
            PlayerPrefs.SetInt(Prefix + "Width", width); PlayerPrefs.SetInt(Prefix + "Height", height); PlayerPrefs.Save(); ApplyDisplay();
        }
        public static void SetLanguage(MenuLanguage language)
        { PlayerPrefs.SetInt(Prefix + "Language", (int)NormalizeLanguage((int)language)); PlayerPrefs.Save(); }
        public static void RestoreDefaults()
        {
            // Keep the selected language so the player can still read the page after resetting.
            PlayerPrefs.SetFloat(Prefix + "Music", .8f); PlayerPrefs.SetFloat(Prefix + "Sound", .8f);
            PlayerPrefs.SetInt(Prefix + "Fullscreen", 1); PlayerPrefs.SetInt(Prefix + "Width", 1920); PlayerPrefs.SetInt(Prefix + "Height", 1080);
            PlayerPrefs.DeleteKey(Prefix + "LargeText"); PlayerPrefs.DeleteKey(Prefix + "Volume"); PlayerPrefs.Save(); Apply();
        }
    }
}
