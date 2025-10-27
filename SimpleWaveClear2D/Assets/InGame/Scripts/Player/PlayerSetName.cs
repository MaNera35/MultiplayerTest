using TMPro;
using UnityEngine;
using Photon.Pun;

public class PlayerSetName : MonoBehaviour
{
    [SerializeField] private TextMeshPro playerText;

    [PunRPC]
    public void SetPlayerName(string _playerName)
    {
        playerText.text = _playerName;
    }


    private void Start()
    {
        PhotonView photonView = GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            photonView.RPC("SetPlayerName", RpcTarget.AllBuffered, PhotonNetwork.NickName);
        }
        
    }
}
