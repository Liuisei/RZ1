using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainPage : MonoBehaviour
{
    [SerializeField] Button _hostButton;
    [SerializeField] Button _joinButton;

    private void Awake()
    {
        _hostButton.onClick.AddListener(OnHostButtonClicked);
        _joinButton.onClick.AddListener(OnJoinButtonClicked);
    }
    #region Button Event Handlers
    private void OnHostButtonClicked()
    {
        //ホスト開始
        NetworkManager.Singleton.StartHost();
        //シーンを切り替え
        NetworkManager.Singleton.SceneManager.LoadScene("Home", LoadSceneMode.Single);
    }
    private void OnJoinButtonClicked()
    {
        NetworkManager.Singleton.StartClient();
    }
    #endregion
}
