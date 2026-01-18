#if UNITY_EDITOR
using System;
using Game.Shared.Constants.Scene;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Game.Shared.SceneManagement.Editor {

public class SceneSetupWindow : EditorWindow {
    readonly string[] _mainScenes = {
        SceneConst.MainScenePath,
    };

    readonly string[] _playgroundScenes = {
        SceneConst.SampleSceneScenePath,
        SceneConst.PlayerScenePath,
        SceneConst.TurnPlayScenePath
    };

    LoadSceneParameters param = new(LoadSceneMode.Additive);

    [MenuItem("Window/Scene Loader")]
    public static void ShowWindow() {
        GetWindow<SceneSetupWindow>("Scene Setup");
    }

    void OnGUI() {
        GUILayout.Label("Scene Management", EditorStyles.boldLabel);
        GUILayout.Label(
            "\tLoad multiple scenes in the editor for easy iteration on actual gameplay testing.",
            EditorStyles.wordWrappedLabel
        );
        GUILayout.Space(10);

        if (GUILayout.Button($"Open {SceneConst.Main}")) {
            loadTestScenes(_mainScenes);
        }

        GUILayout.Label("Test System", EditorStyles.boldLabel);

        if (GUILayout.Button($"Open {SceneConst.SampleScene}")) {
            loadTestScenes(_playgroundScenes);
        }

        GUILayout.Label("Interaction Systems", EditorStyles.boldLabel);


        // if (GUILayout.Button("Open Story Test")) {
        //     loadTestScenes(_storyTestScenes);
        // }

        GUILayout.Label("Base Systems", EditorStyles.boldLabel);

        // if (GUILayout.Button("Open Entity ID Test")) {
        //     loadTestScenes(_entityIdTestScenes);
        // }

        GUILayout.Space(12);
        GUILayout.Label("Story", EditorStyles.boldLabel);
        GUILayout.Space(12);

        // if (GUILayout.Button("Chapter 1")) {
        //     loadTestScenes(_chapter1Scenes);
        // }
    }

    void loadTestScenes(string[] scenes) {
        for (int i = 0; i < scenes.Length; ++i) {
            var scenePath = scenes[i];

            if (i == 0)
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            else {
                try {
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                }
                catch (Exception ex) {
                    EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, param);
                }
            }
        }
    }
}

}
#endif
