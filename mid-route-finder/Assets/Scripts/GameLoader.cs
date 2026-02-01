using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour {
    void Start() {
        StartCoroutine(loadGameScenes());
    }

    IEnumerator loadGameScenes() {
        AsyncOperation playerLoad = SceneManager.LoadSceneAsync("PLAYER", LoadSceneMode.Additive);

        yield return playerLoad;

        AsyncOperation sampleSceneLoad = SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Additive);

        yield return sampleSceneLoad;

        AsyncOperation turnPlayLoad = SceneManager.LoadSceneAsync("TurnPlay", LoadSceneMode.Additive);

        yield return turnPlayLoad;

        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync("Main");

        yield return unloadOp;

        Debug.Log("All scenes loaded successfully!");
    }
}
