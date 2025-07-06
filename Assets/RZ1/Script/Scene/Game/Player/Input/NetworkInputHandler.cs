using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// クライアントからの入力を収集し、サーバーに送信・保存する。
/// </summary>
public class NetworkInputHandler : NetworkBehaviour
{
    [System.Flags]
    public enum InputButton : uint
    {
        None     = 0,
        Jump     = 1 << 0,
        Fire     = 1 << 1,
        Interact = 1 << 2,
        Dash     = 1 << 3,
        // 必要に応じて追加可能
    }

    public struct PlayerInputData : INetworkSerializable
    {
        public Vector2 Move;
        public uint Buttons;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Move);
            serializer.SerializeValue(ref Buttons);
        }

        public bool IsButtonPressed(InputButton button)
        {
            return (Buttons & (uint)button) != 0;
        }
    }

    // サーバーで各クライアントの入力を保持
    private static readonly Dictionary<ulong, PlayerInputData> _inputMap = new();

    public static bool TryGetInput(ulong clientId, out PlayerInputData input)
    {
        return _inputMap.TryGetValue(clientId, out input);
    }

    // InputSystem のアクションマップ
    private RZ1Input _inputActions;
    private RZ1Input.DefaultActions _player;

    private void Awake()
    {
        _inputActions = new RZ1Input();
        _player = _inputActions.Default;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            _inputActions.Enable();
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
            _inputActions.Disable();
    }

    private void Update()
    {
        if (!IsOwner || !IsClient) return;

        var input = CollectInput();

        // ローカルにも保存（ローカルプレビューなどに使用）
        if (NetworkManager.Singleton && NetworkManager.Singleton.IsListening) _inputMap[NetworkManager.Singleton.LocalClientId] = input;

        // サーバーに送信
        SubmitInputServerRpc(input);
    }

    private PlayerInputData CollectInput()
    {
        Vector2 move = _player.Move.ReadValue<Vector2>();
        uint buttons = 0;

        if (_player.Jump.IsPressed())     buttons |= (uint)InputButton.Jump;
        if (_player.Fire.IsPressed())     buttons |= (uint)InputButton.Fire;
        if (_player.Interact.IsPressed()) buttons |= (uint)InputButton.Interact;
        if (_player.Dash.IsPressed())     buttons |= (uint)InputButton.Dash;

        return new PlayerInputData
        {
            Move = move.normalized,
            Buttons = buttons
        };
    }

    [ServerRpc]
    private void SubmitInputServerRpc(PlayerInputData input, ServerRpcParams rpcParams = default)
    {
        ulong senderId = rpcParams.Receive.SenderClientId;
        _inputMap[senderId] = input;
    }
}
