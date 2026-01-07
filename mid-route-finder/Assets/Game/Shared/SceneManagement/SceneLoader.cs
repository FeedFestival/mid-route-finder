using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Shared.SceneManagement {

public class SceneLoader : MonoBehaviour {
    IEnumerator _loadScene;

    public void LoadSingleScene(string sceneName) {
        if (isSceneLoaded(sceneName)) return;

        _loadScene = loadSingleScene(sceneName);
        StartCoroutine(_loadScene);
    }

    public void LoadSingleScene<T>(string sceneName, Action<T> onSceneLoaded) {
        _loadScene = loadSingleScene(sceneName, onSceneLoaded);
        StartCoroutine(_loadScene);
    }

    public void UnloadSingleScene(string sceneName) {
        SceneManager.UnloadSceneAsync(sceneName);
    }

    IEnumerator loadSingleScene(string sceneName) {
        var asyncLoadLevel = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (asyncLoadLevel != null && !asyncLoadLevel.isDone) {
            yield return null;
        }

        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        if (!loadedScene.IsValid()) {
            Debug.LogError($"Scene {sceneName} is not valid or failed to load.");
        }

        StopCoroutine(_loadScene);
    }

    IEnumerator loadSingleScene<T>(string sceneName, Action<T> action) {
        var asyncLoadLevel = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (asyncLoadLevel != null && !asyncLoadLevel.isDone) {
            yield return null;
        }

        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        if (loadedScene.IsValid()) {
            var rootObjects = loadedScene.GetRootGameObjects();
            T component = default(T);
            foreach (var go in rootObjects) {
                if (go.name == sceneName)
                    component = go.GetComponent<T>();
            }

            if (component != null) {
                action?.Invoke(component);
            }
            else {
                Debug.LogError(
                    $"No component of type {typeof(T).Name} found in the scene {sceneName}. Make sure the gameObject of type T has the same name as the scene.");
            }
        }
        else {
            Debug.LogError($"Scene {sceneName} is not valid or failed to load.");
        }

        StopCoroutine(_loadScene);
    }

    //
    // void loadNext() {
    //     if (_loadedIndex == _scenesToLoad.Length - 1) {
    //         OnDependenciesLoaded?.Invoke();
    //         return;
    //     }
    //
    //     _loadedIndex++;
    //     _loadScene = loadScene(_scenesToLoad[_loadedIndex].name);
    //     StartCoroutine(_loadScene);
    // }
    //
    // void onDependenciesLoaded() {
    //     _onSceneLoaded -= onSceneLoaded;
    //     OnDependenciesLoaded -= onDependenciesLoaded;
    //     StopCoroutine(_loadScene);
    // }
    //
    // IEnumerator loadScene(string sceneName) {
    //     var asyncLoadLevel = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    //     while (!asyncLoadLevel.isDone) {
    //         yield return null;
    //     }
    //
    //     _onSceneLoaded?.Invoke(sceneName);
    // }
    //
    // void onSceneLoaded(string sceneName) {
    //     Debug.Log((_loadedIndex + 1) + ". " + sceneName + " has been loaded.");
    //
    //     _scenesToLoad[_loadedIndex].action?.Invoke();
    //
    //     loadNext();
    // }

    bool isSceneLoaded(string sceneName) {
        bool sceneAlreadyLoaded = false;
        int sceneCount = SceneManager.sceneCount;
        for (int i = 0; i < sceneCount; i++) {
            var scene = SceneManager.GetSceneAt(i);
            sceneAlreadyLoaded = scene.name == sceneName;
            if (sceneAlreadyLoaded) break;
        }

        return sceneAlreadyLoaded;
    }
}

}
