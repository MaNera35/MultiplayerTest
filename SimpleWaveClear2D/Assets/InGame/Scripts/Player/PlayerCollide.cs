using Photon.Pun;
using UnityEngine;
using DG.Tweening;
using Unity.Cinemachine;

public class PlayerCollide : MonoBehaviour
{
    [SerializeField] private LayerMask safeGround;
    [SerializeField] private LayerMask wrongGround;
    [SerializeField] private ParticleSystem crashEffect;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerController controller;

    private PhotonView pv;
    private bool isCrashed = false;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!pv.IsMine || isCrashed) return;

        int layer = collision.gameObject.layer;

        if (((1 << layer) & wrongGround) != 0)
        {
            Crash();
        }
    }

    private void Crash()
    {
        isCrashed = true;

        // Input devre d���
        if (controller != null) controller.enabled = false;

        // Rigidbody durdur
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
        }

        // RPC ile patlama ve yok olma animasyonunu t�m oyunculara uygula
        pv.RPC("RPC_PlayCrashEffect", RpcTarget.All);
    }

    [PunRPC]
    void RPC_PlayCrashEffect()
    {
        if (crashEffect != null) crashEffect.Play();

        // DOTween ile k���lme ve yok olma
        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);

            // Kameray� ba�ka canl� oyuncuya ge�irme
            SwitchCameraToAlivePlayer();
        });
    }

    private void SwitchCameraToAlivePlayer()
    {
        // Sadece kendi client'�m�z i�in yap�yoruz
        if (!pv.IsMine) return;

        // T�m oyuncular� bul
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach (var player in players)
        {
            PlayerCollide collide = player.GetComponent<PlayerCollide>();
            if (!collide.IsCrashed() && player.pv.IsMine)
            {
                // Cinemachine kamera bul
                CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
                if (cam != null)
                {
                    cam.Follow = player.transform;
                    cam.LookAt = player.transform;
                }
                break;
            }
        }
    }

    public bool IsCrashed() => isCrashed;
}
