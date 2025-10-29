using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    

    private LobbyManager LobbyManager;

    private void Start()
    {
        LobbyManager = FindFirstObjectByType<LobbyManager>();
    }
    public void SetRoomName(string _roomName, string _playerCount)
    {
        roomNameText.text = _roomName;
        playerCountText.text = _playerCount;
    }


    public void OnClickJoin()
    {
        LobbyManager.JoinRoom(roomNameText.text);
    }
}
