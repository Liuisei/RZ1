using TMPro;
using UnityEngine;

public class LobbyID : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_lobbyIdText;

    private void Start()
    {
        //ロビー作成or入室時に記憶しておいたLobbyIDを設定
        m_lobbyIdText.text = SteamLobby.Instance.LobbyID.ToString();
    }
}
