using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    private double startTime;

    PhotonView pv;

    float restartDelay = 1.5f;
    float crashTime = -1f; // -1 = daha �nce �arp��ma olmad�
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    void Start()
    {
        startTime = PhotonNetwork.Time; // t�m oyuncular i�in ayn� zaman
    }

    void Update()
    {
        double elapsed = PhotonNetwork.Time - startTime;
        timerText.text = FormatTime(elapsed);

        AutoRestartLevel();
    }


    private string FormatTime(double seconds)
    {
        int minutes = Mathf.FloorToInt((float)seconds / 60f);
        int secs = Mathf.FloorToInt((float)seconds % 60f);
        int millis = Mathf.FloorToInt((float)((seconds - Mathf.FloorToInt((float)seconds)) * 1000));

        return $"{minutes:00}.{secs:00}<size=50%>{millis:000}</size>";
    }


    void AutoRestartLevel()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        PlayerCollide[] players = FindObjectsOfType<PlayerCollide>();
        bool allDead = true;

        foreach (var player in players)
        {
            if (!player.IsCrashed()) // Oyuncu canl�ysa
            {
                allDead = false;
                break;
            }
        }
        if (allDead)
        {
            if (crashTime < 0) crashTime = Time.time; // ilk frame�de zaman� kaydet
            if (Time.time - crashTime > restartDelay)
            {
                pv.RPC("RPC_RestartLevel", RpcTarget.All);
            }
        }
        else
        {
            crashTime = -1f; // baz� oyuncular hayattaysa s�f�rla
        }
    }

    [PunRPC]
    void RPC_RestartLevel()
    {
        // Leveli yeniden y�kle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ----------------------- Placeholder for NextLevel -------------------
    public void NextLevel()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("RPC_NextLevel", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_NextLevel()
    {
        // Burada sahne de�i�imi veya level progression mant��� eklenecek
        Debug.Log("NextLevel triggered!");
    }
}
