using UnityEngine;

namespace LoveNarrativeGame.Prototype
{
    // One player survives scene changes, so returning to the menu never stacks music.
    public sealed class BackgroundMusic : MonoBehaviour
    {
        private static BackgroundMusic instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetInstance() { instance = null; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void StartMusic()
        {
            if (FindAnyObjectByType<BackgroundMusic>() == null)
                new GameObject("Persistent Background Music").AddComponent<BackgroundMusic>();
        }

        private void Awake()
        {
            if (instance != null && instance != this) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
            var clip = Resources.Load<AudioClip>("Audio/BackgroundMusic");
            if (clip == null) { Debug.LogWarning("Background music is missing: Audio/BackgroundMusic"); return; }
            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0;
            source.loop = true;
            source.clip = clip;
            gameObject.AddComponent<MenuAudioChannel>().Configure(true);
            source.Play();
        }
        private void OnDestroy() { if (instance == this) instance = null; }
    }
}
