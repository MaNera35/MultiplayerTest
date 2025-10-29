using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;
    public Image masterClientCrown;

    public Player Player { get; private set; } 
    public PhotonView PhotonView { get; private set; }

    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
    }

    public void SetPlayerInfo(Player player)
    {
        Player = player;
        playerName.text = player.NickName;
        UpdateCrownState();
    }

    public void UpdateCrownState()
    {
        bool isMaster = Player != null && Player == PhotonNetwork.MasterClient;
        masterClientCrown.gameObject.SetActive(isMaster);
    }
}
