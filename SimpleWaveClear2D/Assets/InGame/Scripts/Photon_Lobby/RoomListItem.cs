using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomNameText;

    private LobbyManager LobbyManager;

    private void Start()
    {
        LobbyManager = FindFirstObjectByType<LobbyManager>();
    }
    public void SetRoomName(string _roomName)
    {
        roomNameText.text = _roomName;
    }


    public void OnClickJoin()
    {
        LobbyManager.JoinRoom(roomNameText.text);
    }
}
