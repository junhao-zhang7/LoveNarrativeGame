using UnityEngine;

namespace LoveNarrativeGame.Prototype
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class MenuAudioChannel : MonoBehaviour
    {
        [SerializeField] private bool music = false;
        [SerializeField, Range(0, 1)] private float baseVolume = 1;
        private AudioSource source;
        private void OnEnable() { source = GetComponent<AudioSource>(); MenuPreferences.AudioChanged += Apply; Apply(); }
        private void OnDisable() { MenuPreferences.AudioChanged -= Apply; }
        private void Apply() { if (source != null) source.volume = baseVolume * (music ? MenuPreferences.Music : MenuPreferences.Sound); }
    }
}
