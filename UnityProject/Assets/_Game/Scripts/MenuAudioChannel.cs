using UnityEngine;

namespace LoveNarrativeGame.Prototype
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class MenuAudioChannel : MonoBehaviour
    {
        [SerializeField] private bool music = false;
        [SerializeField, Range(0, 1)] private float baseVolume = 1;
        private AudioSource source;
        public void Configure(bool isMusic, float gain = 1)
        { music = isMusic; baseVolume = Mathf.Clamp01(gain); Apply(); }
        private void OnEnable() { source = GetComponent<AudioSource>(); MenuPreferences.AudioChanged += Apply; Apply(); }
        private void OnDisable() { MenuPreferences.AudioChanged -= Apply; }
        private void Apply() { if (source != null) source.volume = baseVolume * (music ? MenuPreferences.Music : MenuPreferences.Sound); }
    }
}
