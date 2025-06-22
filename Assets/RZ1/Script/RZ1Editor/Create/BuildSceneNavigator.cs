using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
             
namespace RZ1.Editor
{
    public class BuildSceneNavigator : EditorWindow
    {
        //[MenuItem("Tools/Scenes/Open Build Scene")]
        [MenuItem("Tools/Scenes/Open Build Scene %m")] // Ctrl + M で起動
        public static void ShowWindow()
        {
            GetWindow<BuildSceneNavigator>("Build Scenes");
        }

        private Vector2 _scrollPos;

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Scenes in Build Settings", EditorStyles.boldLabel);
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            var scenes = EditorBuildSettings.scenes;
            for (int i = 0; i < scenes.Length; i++)
            {
                var scene = scenes[i];
                if (!scene.enabled) continue;

                string scenePath = scene.path;
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

                if (GUILayout.Button($"{i}: {sceneName}"))
                {
                    OpenScene(scenePath);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private static void OpenScene(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scenePath);
            }
        }
    }
}
