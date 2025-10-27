using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerStatsItem playerStatsItemPrefab;
    [SerializeField] private Transform[] holder;
    

    private Dictionary<int, PlayerStatsItem> playerUI = new Dictionary<int, PlayerStatsItem>();

    private void Start()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            CreatePlayerUI(player);
        }
    }

    private void Update()
    {
        UpdateAllPlayerUI();
    }

    private void CreatePlayerUI(Player player)
    {
        PlayerStatsItem statsItem = Instantiate(playerStatsItemPrefab, holder[player.ActorNumber - 1]);
        statsItem.SetStats(player.NickName, "0", "0", 0f);
        playerUI.Add(player.ActorNumber, statsItem);
    }

    private void UpdateAllPlayerUI()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (playerUI.TryGetValue(player.ActorNumber, out PlayerStatsItem statsItem))
            {
                GameObject playerGO = FindPlayerGO(player);
                if (playerGO == null) continue;

                PlayerController pc = playerGO.GetComponent<PlayerController>();
                if (pc == null) continue;

                statsItem.SetStats(
                    player.NickName,
                    Mathf.Round(Mathf.Abs(pc.GetVelocityX() * 3f)).ToString(),
                    Mathf.Round(Mathf.Abs(pc.GetVelocityY() * 3f)).ToString(),
                    pc.GetFuelNormalized()
                );
            }
        }
    }

    private GameObject FindPlayerGO(Player player)
    {
        foreach (var go in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView pv = go.GetComponent<PhotonView>();
            if (pv != null && pv.Owner.ActorNumber == player.ActorNumber)
                return go;
        }
        return null;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
      
        if (playerUI.TryGetValue(otherPlayer.ActorNumber, out PlayerStatsItem statsItem))
        {
            Destroy(statsItem.gameObject);
            playerUI.Remove(otherPlayer.ActorNumber);
        }
    }
}
