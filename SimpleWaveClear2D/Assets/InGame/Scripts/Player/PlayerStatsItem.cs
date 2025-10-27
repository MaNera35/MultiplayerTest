using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI velocityXText;
    [SerializeField] private TextMeshProUGUI velocityYText;
    [SerializeField] private Image fuelFillAmount;



    public void SetStats(string _playerName, string _velocityXText, string _velocityYText, float fillAmountValue)
    {
        playerName.text = _playerName;
        velocityXText.text = _velocityXText;
        velocityYText.text = _velocityYText;
        fuelFillAmount.fillAmount = fillAmountValue;
    }

}
