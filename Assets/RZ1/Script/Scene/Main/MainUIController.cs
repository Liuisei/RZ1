using Steamworks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainPage : MonoBehaviour
{
    [SerializeField] Button _hostButton;
    [SerializeField] Button _joinButton;
    [SerializeField] TMP_InputField m_joinLobbyID;
    private CallResult<LobbyCreated_t> m_crLobbyCreated;
    private void Awake()
    {
        _hostButton.onClick.AddListener(OnHostButtonClicked);
        _joinButton.onClick.AddListener(OnJoinButtonClicked);
    }
    #region Button Event Handlers
    private void OnHostButtonClicked()
    {
        SteamLobby.Instance.CreateLobby();
    }
    private void OnJoinButtonClicked()
    {
        SteamLobby.Instance.JoinLobby((CSteamID)ulong.Parse(m_joinLobbyID.text));
    }
    #endregion




}
