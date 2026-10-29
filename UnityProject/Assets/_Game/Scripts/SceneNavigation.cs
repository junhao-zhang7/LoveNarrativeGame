using UnityEngine;
using UnityEngine.SceneManagement;

namespace LoveNarrativeGame.Prototype
{
    // Attach to a scene object, then connect UI Button On Click events.
    public sealed class SceneNavigation : MonoBehaviour
    {
        [SerializeField] private string gameplaySceneName = "SampleScene";
        [SerializeField] private string menuSceneName = "MainMenu";

        private bool isLoading;

        public void StartGame()
        {
            LoadScene(gameplaySceneName);
        }

        public void ReturnToMenu()
        {
            LoadScene(menuSceneName);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            Debug.Log("Quit is available in the Windows build. Stop Play to exit this test.", this);
#else
            Application.Quit();
#endif
        }

        private void LoadScene(string sceneName)
        {
            if (isLoading)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(sceneName) || !Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError($"Cannot load '{sceneName}'. Add and enable it in Build Profiles > Scene List.", this);
                return;
            }

            isLoading = true;
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }
    }
}
