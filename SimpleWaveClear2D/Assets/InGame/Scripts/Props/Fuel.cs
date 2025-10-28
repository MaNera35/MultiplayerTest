using Photon.Pun;
using UnityEngine;
using DG.Tweening; // DOTween i�in

public class Fuel : MonoBehaviour
{
    [SerializeField] private float popupDuration = 0.5f; // popup s�resi
    [SerializeField] private float scaleMultiplier = 1.5f; // pop-up b�y�kl���
    [SerializeField] private float fadeDuration = 0.3f; // yok olma s�resi
    [SerializeField] private SpriteRenderer spriteRenderer; // fuel sprite renderer

    private void Reset()
    {
        // E�er inspector'da atamad�ysan otomatik al
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DestroySelf()
    {
        if (spriteRenderer == null)
        {
            PhotonNetwork.Destroy(gameObject);
            return;
        }

        // DOTween ile popup ve fade
        transform
            .DOScale(Vector3.one * scaleMultiplier, popupDuration) // b�y�
            .SetEase(Ease.OutBack) // ho� bir easing
            .OnComplete(() =>
            {
                // Fade out
                spriteRenderer.DOFade(0, fadeDuration)
                    .OnComplete(() =>
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            PhotonNetwork.Destroy(gameObject); // photon ile yok et
                        }
                    });
            });
    }
}
