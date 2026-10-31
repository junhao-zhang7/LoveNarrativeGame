using UnityEngine;

namespace LoveNarrativeGame.Prototype
{
    public static class MenuPreferences
    {
        private const string Prefix = "SchoolPrototype.Settings.";
        public static float Volume => NormalizeVolume(PlayerPrefs.GetFloat(Prefix + "Volume", .8f));
        public static bool Fullscreen => PlayerPrefs.GetInt(Prefix + "Fullscreen", 0) == 1;
        public static float TextScale => PlayerPrefs.GetInt(Prefix + "LargeText", 0) == 1 ? 1.15f : 1f;
        public static float NormalizeVolume(float value) => float.IsNaN(value) || float.IsInfinity(value) ? .8f : Mathf.Clamp01(value);
        public static void Apply()
        {
            AudioListener.volume = Volume;
#if !UNITY_EDITOR
            Screen.fullScreenMode = Fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
#endif
        }
        public static void SetVolume(float value)
        {
            PlayerPrefs.SetFloat(Prefix + "Volume", NormalizeVolume(value)); AudioListener.volume = Volume; PlayerPrefs.Save();
        }
        public static void SetFullscreen(bool value)
        {
            PlayerPrefs.SetInt(Prefix + "Fullscreen", value ? 1 : 0); PlayerPrefs.Save(); Apply();
        }
        public static void SetLargeText(bool value) { PlayerPrefs.SetInt(Prefix + "LargeText", value ? 1 : 0); PlayerPrefs.Save(); }
        public static void RestoreDefaults()
        {
            SetVolume(.8f); SetFullscreen(false); SetLargeText(false);
        }
    }
}
