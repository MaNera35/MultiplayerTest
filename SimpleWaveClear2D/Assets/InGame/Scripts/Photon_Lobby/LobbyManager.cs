using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Collections.Generic;
using Photon.Realtime;
using System;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Input Fields")]
    [SerializeField] private TMP_InputField roomNameField;

    [Header("Panels")]
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private GameObject lobbyPanel;

    [Header("Room UI")]
    [SerializeField] private TextMeshProUGUI roomNameText;

    [Header("Room List")]
    [SerializeField] private List<RoomListItem> roomItemList = new List<RoomListItem>();
    [SerializeField] private RoomListItem roomListItemPrefab;
    [SerializeField] private Transform content;

    [Header("Player List")]
    [SerializeField] private List<PlayerListItem> playerListItems = new List<PlayerListItem>();
    [SerializeField] private PlayerListItem playerListItemPrefab;
    [SerializeField] private Transform playerContent;

    [Header("Update Settings")]
    [SerializeField] private float updateListTime = 1.5f;
    private float nextUpdateTime;

    [Header("MasterClient-SetActive")]
    [SerializeField] private GameObject startButton;


    private void Start()
    {
        PhotonNetwork.JoinLobby();

    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
    }
    // ===============================
    // CONNECTION CALLBACKS
    // ===============================
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined the Lobby");
    }

    // ===============================
    // ROOM CREATION & JOIN
    // ===============================
    public void OnClickCreateRoom()
    {
        if (roomNameField.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(roomNameField.text);
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void StartGame()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount >= 1)
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }

    // ===============================
    // ROOM & LOBBY CALLBACKS
    // ===============================
    public override void OnJoinedRoom()
    {
        roomPanel.SetActive(true);
        lobbyPanel.SetActive(false);

        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (Time.time >= nextUpdateTime)
        {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + updateListTime;
        }
    }

    // ===============================
    //  LIST UPDATES (UI)
    // ===============================
    private void UpdateRoomList(List<RoomInfo> _roomList)
    {
        foreach (RoomListItem roomInfo in roomItemList)
        {
            Destroy(roomInfo.gameObject);
        }
        roomItemList.Clear();

        foreach (RoomInfo room in _roomList)
        {
            if (room.IsOpen && room.PlayerCount > 0)
            {
                RoomListItem newRoom = Instantiate(roomListItemPrefab, content);
                newRoom.SetRoomName(room.Name);
                roomItemList.Add(newRoom);
            }
            else
            {
                room.RemovedFromList = true;
                roomItemList.Clear();
            }
        }
    }

    private void UpdatePlayerList()
    {
        foreach (PlayerListItem playerInfo in playerListItems)
        {
            Destroy(playerInfo.gameObject);
        }
        playerListItems.Clear();

        foreach (var player in PhotonNetwork.PlayerList)
        {           
                var newPlayer = Instantiate(playerListItemPrefab, playerContent);
                newPlayer.SetPlayerName(player.NickName);
                playerListItems.Add(newPlayer);        
        }

    }
}
