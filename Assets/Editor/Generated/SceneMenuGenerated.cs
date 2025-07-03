using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// 自動生成されたシーン切り替えメニュー
public static class SceneMenuGenerated
{
    [MenuItem("Tools/Scenes/Game")]
    public static void Open_Game()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene("Assets/RZ1/Scene/Game.unity");
    }

    [MenuItem("Tools/Scenes/Main")]
    public static void Open_Main()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene("Assets/RZ1/Scene/Main.unity");
    }

    [MenuItem("Tools/Scenes/SimpleTest")]
    public static void Open_SimpleTest()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene("Assets/RZ1/Scene/SimpleTest.unity");
    }

}
