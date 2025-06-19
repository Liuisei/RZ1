using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyID : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_lobbyIdText;

    private void Start()
    {
        //ロビー作成or入室時に記憶しておいたLobbyIDを設定
        m_lobbyIdText.text = "ID:" + SteamLobby.Instance.LobbyID.ToString();
    }

    private void Update()
    {
        // Enterキー+CでロビーIDをクリップボードにコピー
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
        {
            GUIUtility.systemCopyBuffer = SteamLobby.Instance.LobbyID.ToString();
            Debug.Log("Lobby ID copied to clipboard: " + SteamLobby.Instance.LobbyID.ToString());
        }
    }
}
