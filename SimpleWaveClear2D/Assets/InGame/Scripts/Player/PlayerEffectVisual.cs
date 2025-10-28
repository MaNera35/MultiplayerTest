using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PlayerEffectVisual : MonoBehaviourPun
{
    [SerializeField] private ParticleSystem leftPS;
    [SerializeField] private ParticleSystem topPS;
    [SerializeField] private ParticleSystem rightPS;

    private PhotonView pv;

    private void Start()
    {
        pv = GetComponent<PhotonView>();

        // Baþlangýçta tüm efektleri kapat
        SetValueToEmission(topPS, false);
        SetValueToEmission(leftPS, false);
        SetValueToEmission(rightPS, false);
    }

    public void PlayEffect(bool up, bool left, bool right)
    {
        if (pv.IsMine)
            pv.RPC("RPC_PlayEffect", RpcTarget.All, up, left, right);
    }

    [PunRPC]
    private void RPC_PlayEffect(bool up, bool left, bool right)
    {
        SetValueToEmission(topPS, up);
        SetValueToEmission(leftPS, left);
        SetValueToEmission(rightPS, right);
    }

    private void SetValueToEmission(ParticleSystem effect, bool value)
    {
        var emission = effect.emission;
        emission.enabled = value;
    }
}
