#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

namespace HandVR.Editor
{
    [InitializeOnLoad]
    public static class HandVRSceneSetup
    {
        static HandVRSceneSetup()
        {
            EditorApplication.delayCall += EnsureScene;
        }

        [MenuItem("HandVR/Create Startup Scene")]
        public static void EnsureScene()
        {
            const string folder = "Assets/Scenes";
            const string scenePath = folder + "/HandVR.unity";
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            if (!File.Exists(scenePath))
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,
                    NewSceneMode.Single);
                var camera = new GameObject("Main Camera");
                camera.tag = "MainCamera";
                camera.AddComponent<Camera>();
                camera.AddComponent<AudioListener>();
                EditorSceneManager.SaveScene(scene, scenePath);
            }

            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(
                EditorBuildSettings.scenes);
            bool found = false;
            foreach (var item in scenes)
                if (item.path == scenePath) found = true;
            if (!found)
            {
                scenes.Insert(0, new EditorBuildSettingsScene(scenePath, true));
                EditorBuildSettings.scenes = scenes.ToArray();
            }
        }
    }
}
#endif
