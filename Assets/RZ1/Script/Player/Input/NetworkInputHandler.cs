using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Netcode専用の入力ハンドラー。クライアントからの入力を受け取り、サーバー上にクライアントIDごとに保存する。
/// </summary>
public class NetworkInputHandler : NetworkBehaviour
{
    public struct PlayerInputData : INetworkSerializable
    {
        public Vector2 Move;
        public uint Buttons;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Move);
            serializer.SerializeValue(ref Buttons);
        }
    }

    // サーバーで各クライアントの入力を保持
    private static readonly Dictionary<ulong, PlayerInputData> _inputMap = new();

    /// <summary>
    /// 指定されたクライアントIDの入力データを取得する
    /// </summary>
    public static bool TryGetInput(ulong clientId, out PlayerInputData input)
    {
        return _inputMap.TryGetValue(clientId, out input);
    }

    // 入力アクション
    private RZ1Input _inputActions;
    private RZ1Input.DefaultActions _player;

    private void Awake()
    {
        _inputActions = new RZ1Input();
        _player = _inputActions.Default;
        _inputActions.Enable();
    }

    private void OnDestroy()
    {
        _inputActions?.Disable();
    }

    private void Update()
    {
        // オーナーのみが入力を処理
        if (!IsOwner) return;

        var input = CollectInput();

        // ローカルに保存（即座に反映用）
        _inputMap[NetworkManager.Singleton.LocalClientId] = input;
        // サーバーに送信
        SubmitInputServerRpc(input);
    }

    /// <summary>
    /// 現在の入力を収集する
    /// </summary>
    private PlayerInputData CollectInput()
    {
        Vector2 move = _player.Move.ReadValue<Vector2>();
        uint buttons = 0;

        // ボタン入力をビットフラグで管理
        if (_player.Jump.IsPressed()) buttons |= 1u << 0;      // Jump
        if (_player.Fire.IsPressed()) buttons |= 1u << 1;      // Fire
        if (_player.Interact.IsPressed()) buttons |= 1u << 2;  // Interact
        if (_player.Dash.IsPressed()) buttons |= 1u << 3;      // Dash

        return new PlayerInputData
        {
            Move = move.normalized,
            Buttons = buttons
        };
    }

    /// <summary>
    /// サーバーに入力データを送信する
    /// </summary>
    [ServerRpc]
    private void SubmitInputServerRpc(PlayerInputData input, ServerRpcParams rpcParams = default)
    {
        // サーバー側で送信元クライアントの入力を保存
        _inputMap[rpcParams.Receive.SenderClientId] = input;
    }
}