using Photon.Pun;
using UnityEngine;
using DG.Tweening; // DOTween için

public class Fuel : MonoBehaviour
{
    [SerializeField] private float popupDuration = 0.5f; // popup süresi
    [SerializeField] private float scaleMultiplier = 1.5f; // pop-up büyüklüðü
    [SerializeField] private float fadeDuration = 0.3f; // yok olma süresi
    [SerializeField] private SpriteRenderer spriteRenderer; // fuel sprite renderer

    private void Reset()
    {
        // Eðer inspector'da atamadýysan otomatik al
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
            .DOScale(Vector3.one * scaleMultiplier, popupDuration) // büyü
            .SetEase(Ease.OutBack) // hoþ bir easing
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
