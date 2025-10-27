using TMPro;
using UnityEngine;

public class PlayerListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;


    public void SetPlayerName(string _playerName)
    {
        playerName.text = _playerName;
    }
}
