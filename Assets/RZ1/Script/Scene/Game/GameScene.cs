using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

namespace Game
{
    public class GameScene : NetworkBehaviour
    {
        //プレイヤーのプレハブ
        [SerializeField] private NetworkObject m_playerPrefab;
        [SerializeField] private Transform _spawnTransform;

        public override void OnNetworkSpawn()
        {
            //ホスト以外の場合
            if (IsHost == false) { return; }

            //クライアント接続時
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

            //すでに存在するクライアント用に関数呼び出す
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                OnClientConnected(client.ClientId);
            }
        }

        public void OnClientConnected(ulong clientId)
        {
            //プレイヤーオブジェクト生成
            var generatePos = _spawnTransform.position;
            generatePos.x = -5 + 5 * (NetworkManager.Singleton.ConnectedClients.Count % 3);
            NetworkObject playerObject = Instantiate(m_playerPrefab, generatePos, Quaternion.identity);
            //接続クライアントをOwnerにしてPlayerObjectとしてスポーン
            playerObject.SpawnAsPlayerObject(clientId);
        }
    }
}
