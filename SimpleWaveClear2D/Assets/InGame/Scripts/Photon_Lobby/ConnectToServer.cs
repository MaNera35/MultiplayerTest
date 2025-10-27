using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TextMeshProUGUI buttonText;

    public void OnClickConnect()
    {
        if (usernameInputField.text.Length >= 1)
        {
            PhotonNetwork.NickName = usernameInputField.text;
            buttonText.text = "Connect...";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
        Debug.Log("Connected Master");
    }

}
