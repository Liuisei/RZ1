using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;

public static class SceneMenuGenerator
{
    private const string SceneFolder = "Assets/RZ1/Scene/";
    private const string OutputPath = "Assets/Editor/Generated/SceneMenuGenerated.cs";

    [MenuItem("Tools/Scenes/🛠 シーンメニューを再生成")]
    public static void GenerateSceneMenu()
    {
        var sceneFiles = Directory.GetFiles(SceneFolder, "*.unity", SearchOption.AllDirectories);
        var sb = new StringBuilder();

        sb.AppendLine("using UnityEditor;");
        sb.AppendLine("using UnityEditor.SceneManagement;");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine();
        sb.AppendLine("// 自動生成されたシーン切り替えメニュー");
        sb.AppendLine("public static class SceneMenuGenerated");
        sb.AppendLine("{");

        foreach (var fullPath in sceneFiles)
        {
            var relativePath = fullPath.Replace("\\", "/");
            var fileName = Path.GetFileNameWithoutExtension(relativePath);
            var safeMethodName = fileName.Replace(" ", "_");

            sb.AppendLine($@"    [MenuItem(""Tools/Scenes/{fileName}"")]");
            sb.AppendLine($"    public static void Open_{safeMethodName}()");
            sb.AppendLine("    {");
            sb.AppendLine("        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())");
            sb.AppendLine($"            EditorSceneManager.OpenScene(\"{relativePath}\");");
            sb.AppendLine("    }");
            sb.AppendLine();
        }

        sb.AppendLine("}");

        Directory.CreateDirectory(Path.GetDirectoryName(OutputPath));
        File.WriteAllText(OutputPath, sb.ToString());
        AssetDatabase.Refresh();

        Debug.Log("✅ シーンメニューを再生成しました。");
    }
}