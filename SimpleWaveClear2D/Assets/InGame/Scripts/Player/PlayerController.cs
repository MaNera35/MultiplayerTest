using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem; 


[RequireComponent(typeof(Rigidbody2D), typeof(PhotonView))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveForce = 500f;
    [SerializeField] private float rotationTorque = 200f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private PhotonView pv;

    [SerializeField] float FuelAmount = 10f;
    float currentFuel;


    private float statsSendInterval = 0.1f;
    private float nextSendTime;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        currentFuel = FuelAmount;
        if (pv.IsMine)
        {
            var cam = FindFirstObjectByType<CinemachineCamera>();

            cam.Follow = transform;
            cam.LookAt = transform;

            cam.gameObject.SetActive(true);
        }
    }
    private void Update()
    {
        if (!pv.IsMine) return;


    }

    private void FixedUpdate()
    {
        if (!pv.IsMine) return;

        Move();

        if (Time.time >= nextSendTime)
        {
            SendStatsRPC();
            nextSendTime = Time.time + statsSendInterval;
        }
    }

    private void Move()
    {
        if (currentFuel <= 0) return;

        if(Keyboard.current.upArrowKey.isPressed || Keyboard.current.leftArrowKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            currentFuel -= Time.deltaTime;
        }

        if (Keyboard.current.upArrowKey.isPressed)
        {
            rb.AddForce(transform.up * moveForce * Time.deltaTime);
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            rb.AddTorque(rotationTorque * Time.deltaTime);
        }
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            rb.AddTorque(-rotationTorque * Time.deltaTime);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!pv.IsMine) return;

        if (collision.gameObject.TryGetComponent(out Fuel fuel))
        {
            currentFuel += FuelAmount;
            if (currentFuel > FuelAmount)
            {
                currentFuel = FuelAmount;
            }
            
        }

            
    }


    private void SendStatsRPC()
    {
        pv.RPC("UpdateStatsRPC", RpcTarget.Others, currentFuel, rb.linearVelocity.x, rb.linearVelocity.y);
    }

    [PunRPC]
    void UpdateStatsRPC(float fuel, float velX, float velY)
    {
        currentFuel = fuel;
        rb.linearVelocity = new Vector2(velX, velY); // opsiyonel: di�er oyuncular�n hareketini g�rsel olarak g�stermek i�in
    }


    public float GetFuelNormalized()
    {
        return Mathf.Clamp01(currentFuel / FuelAmount);
    }
    public float GetVelocityX() => rb.linearVelocityX;
    public float GetVelocityY() => rb.linearVelocityY;

}
