using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour {
    void Start() {
        // Start a coroutine to load scenes
        StartCoroutine(LoadGameScenes());
    }

    IEnumerator LoadGameScenes() {
        // 1️⃣ Load SampleScene as the main scene
        AsyncOperation mainLoad = SceneManager.LoadSceneAsync("PLAYER", LoadSceneMode.Additive);

        // Wait until it's fully loaded
        yield return mainLoad;

        // 2️⃣ Load PLAYER scene additively
        AsyncOperation playerLoad = SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Additive);

        // Wait until it's fully loaded
        yield return playerLoad;

        Debug.Log("All scenes loaded successfully!");
    }
}
