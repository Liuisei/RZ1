using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// 自動生成されたシーン切り替えメニュー
public static class SceneMenuGenerated
{
    [MenuItem("Tools/Scenes/InGame")]
    public static void Open_InGame()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene("Assets/RZ1/Scene/InGame.unity");
    }

    [MenuItem("Tools/Scenes/SimpleTest")]
    public static void Open_SimpleTest()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene("Assets/RZ1/Scene/SimpleTest.unity");
    }

    [MenuItem("Tools/Scenes/Title")]
    public static void Open_Title()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene("Assets/RZ1/Scene/Title.unity");
    }

}
