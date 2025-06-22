using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using System.IO;

namespace RZ1.Editor
{
    public class CreateInterfaceScript
    {
        [MenuItem("Assets/Create/Scripting/Interface", false, 0)]
        public static void CreateInterface()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<DoCreateInterfaceAsset>(),
                "NewInterface.cs",
                null,
                null
            );
        }
    }

    public class DoCreateInterfaceAsset : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string fileName = Path.GetFileNameWithoutExtension(pathName);
            string templateContent = $@"using System;

public interface {fileName}
{{
    
}}";
            
            File.WriteAllText(pathName, templateContent);
            AssetDatabase.Refresh();
            
            // 作成されたファイルを選択状態にする
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(pathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }
}