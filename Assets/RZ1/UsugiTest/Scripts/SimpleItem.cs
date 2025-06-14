using UnityEngine;

public class SimpleItem : ItemBase
{
    public override void Use()
    {
        Debug.Log($"{name} を使用しました！");
        // ここに実際の効果を書く
    }
}