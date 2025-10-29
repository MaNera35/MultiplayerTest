using Photon.Pun;
using TMPro;
using UnityEngine;

public class FeedBackMessage : MonoBehaviour
{
    public static FeedBackMessage Instance;

    [SerializeField] private TextMeshProUGUI feedbackMessagePrefab;
    [SerializeField] private Transform content;

    private PhotonView pv;

    private void Awake()
    {
        Instance = this;
        pv = GetComponent<PhotonView>();
    }

    // Local �a�r�: di�er oyunculara mesaj g�ndermek i�in
    public void ShowMessage(string message)
    {
        pv.RPC("RPC_ShowMessage", RpcTarget.All, message);
    }

    [PunRPC]
    private void RPC_ShowMessage(string message)
    {
        TextMeshProUGUI textMessage = Instantiate(feedbackMessagePrefab, content);
        textMessage.text = message;
        Destroy(textMessage.gameObject, 5f);
    }
}
