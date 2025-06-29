using Cysharp.Threading.Tasks;
using UnityEngine;

public static class AnimatorUtility
{
    public static async UniTask WaitForAnimationToEnd(Animator animator, string stateName, int layer = 0)
    {
        // ステートに遷移するまで待つ
        while (!animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName))
        {
            await UniTask.Yield();
        }

        // アニメーションの終了まで待つ
        while (animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName) &&
               animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1f)
        {
            await UniTask.Yield();
        }
    }
}