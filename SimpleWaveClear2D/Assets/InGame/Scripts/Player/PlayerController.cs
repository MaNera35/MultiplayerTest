using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

[RequireComponent(typeof(Rigidbody2D), typeof(PhotonView))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveForce = 500f;
    [SerializeField] private float rotationTorque = 200f;

    [Header("Fuel Settings")]
    [SerializeField] private float maxFuel = 10f;
    private float currentFuel;

    private Rigidbody2D rb;
    public  PhotonView pv;
    private PlayerEffectVisual pev;

    private float statsSendInterval = 0.1f;
    private float nextSendTime;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pv = GetComponent<PhotonView>();
        pev = GetComponent<PlayerEffectVisual>();
    }

    private void Start()
    {
        currentFuel = maxFuel;

        if (pv.IsMine)
        {
            CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
            if (cam != null)
            {
                cam.Follow = transform;
                cam.LookAt = transform;
                cam.gameObject.SetActive(true);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!pv.IsMine) return;

        HandleMovement();
        SendStatsPeriodically();
    }

    private void HandleMovement()
    {
        if (currentFuel <= 0)
        {
            pev?.PlayEffect(false, false, false);
            return;
        }

        bool up = Keyboard.current.upArrowKey.isPressed;
        bool left = Keyboard.current.leftArrowKey.isPressed;
        bool right = Keyboard.current.rightArrowKey.isPressed;

        if (up || left || right)
            currentFuel -= Time.fixedDeltaTime;

        if (up)
            rb.AddForce(transform.up * moveForce * Time.fixedDeltaTime);

        if (left)
            rb.AddTorque(rotationTorque * Time.fixedDeltaTime);

        if (right)
            rb.AddTorque(-rotationTorque * Time.fixedDeltaTime);

        pev?.PlayEffect(up, left, right);
    }

    private void SendStatsPeriodically()
    {
        if (Time.time >= nextSendTime)
        {
            pv.RPC("UpdateStatsRPC", RpcTarget.Others, currentFuel, rb.linearVelocity.x, rb.linearVelocity.y);
            nextSendTime = Time.time + statsSendInterval;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!pv.IsMine) return;

        if (collision.TryGetComponent<Fuel>(out Fuel fuel))
        {
            currentFuel = Mathf.Min(currentFuel + maxFuel, maxFuel);
            PhotonView fuelView = fuel.GetComponent<PhotonView>();

            if (PhotonNetwork.IsMasterClient)
            {
                fuel.DestroySelf();
            }
            else
            {
                pv.RPC("DestroyFuelRPC", RpcTarget.MasterClient, fuelView.ViewID);
            }
        }
    }

    [PunRPC]
    private void DestroyFuelRPC(int viewID)
    {
        PhotonView target = PhotonView.Find(viewID);
        if (target != null)
            target.GetComponent<Fuel>().DestroySelf();
    }

    [PunRPC]
    private void UpdateStatsRPC(float fuel, float velX, float velY)
    {
        currentFuel = fuel;
        rb.linearVelocity = new Vector2(velX, velY); // Görsel olarak diğer oyuncu hareketi
    }

    // Helper methods
    public float GetFuelNormalized() => Mathf.Clamp01(currentFuel / maxFuel);
    public float GetVelocityX() => rb.linearVelocityX;
    public float GetVelocityY() => rb.linearVelocityY;

}
