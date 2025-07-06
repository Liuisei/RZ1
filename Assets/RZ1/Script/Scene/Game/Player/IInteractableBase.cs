using Unity.Netcode;
using UnityEngine;

public abstract class InteractableBase : NetworkBehaviour
{
    [SerializeField] private string _promptText = "Press E to interact";

    public string PromptText => _promptText;

    /// <summary>
    /// 実際のインタラクト処理（継承側で実装）
    /// </summary>
    public abstract void Interact(ulong interactorClientId);
}