using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System;
using System.IO;
using System.Text;
using UnityEngine;

public class PlayerController : NetworkBehaviour, IDamageable, IMoveableObject
{
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float RotationSpeed;
    [SerializeField] private float FireRate;
    [SerializeField] private float Health;
    //[SerializeField] private Rigidbody2D Rb;
    [SerializeField] private Transform RotationPoint;
    [SerializeField] private Transform FirePoint;
    [SerializeField] private Projectile ProjectilePrefab;


    private float turnSmoothVelocity;
    private float m_LastSentMove;
    private float m_LastShot;
    private bool isFiring;
    private float currentHealth;

    private const float k_MoveSendRateSeconds = 0.05f;

    public event Action<Vector2> UpdateVelocityDirection;

    public NetworkVariableFloat TargetAngle = new NetworkVariableFloat();
    public NetworkVariableVector2 Velocity = new NetworkVariableVector2();
    public NetworkVariableVector2 NetworkPosition { get; } = new NetworkVariableVector2();

    public float GetMoveSpeed() => MoveSpeed;
    public Vector2 GetNetworkPosition() => NetworkPosition.Value;

    private void Start()
    {
        isFiring = false;
        currentHealth = Health;

        if (IsOwner)
        {
            InputManager.Instance.SetLocalPlayerController(this);
        }
    }

    public void Move(Vector2 inputVector)
    {
        //SetVelocityServerRpc(inputVector);
        SendCharacterInputServerRpc(inputVector);
    }

    public void Look(Vector2 mousePos)
    {
        if ((Time.time - m_LastSentMove) > k_MoveSendRateSeconds) //Can send a move update
        {
            var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            Vector2 direction = worldPos - transform.position;

            if (direction.sqrMagnitude > 0.01f)
            {
                m_LastSentMove = Time.time;
                SetTargetAngleServerRpc(direction);
            }
        }
    }

    public void SetNetworkPosition(Vector2 _position)
    {
        NetworkPosition.Value = _position;
    }

    public void SetFiring(bool _isFiring)
    {
        isFiring = _isFiring;
    }

    [ServerRpc]
    private void SendCharacterInputServerRpc(Vector2 movementTarget)
    {
        UpdateVelocityDirection?.Invoke(movementTarget);
    }

    [ServerRpc]
    private void SetVelocityServerRpc(Vector2 inputVector, ServerRpcParams rpcParams = default)
    {
        Velocity.Value = inputVector.normalized * MoveSpeed;
    }

    [ServerRpc]
    void SetTargetAngleServerRpc(Vector2 direction, ServerRpcParams rpcParams = default)
    {
        direction.Normalize();
        TargetAngle.Value = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    [ServerRpc]
    private void SpawnProjectileServerRpc(ServerRpcParams rpcParams = default)
    {
        Projectile _projectile = Instantiate(ProjectilePrefab, FirePoint.position, FirePoint.rotation);
        Stream _positionStream = new MemoryStream();
        var writer = new StreamWriter(_positionStream);
        writer.Write($"{FirePoint.position.x},{FirePoint.position.y}");
        writer.Flush();
        _positionStream.Position = 0;

        _projectile.GetComponent<NetworkObject>().Spawn(_positionStream, destroyWithScene: true);
    }

    private void FixedUpdate()
    {
        //Set Velocity
        //Rb.velocity = Velocity.Value * Time.deltaTime;

        //Set gun rotation
        float angle = Mathf.SmoothDampAngle(RotationPoint.eulerAngles.z, TargetAngle.Value, ref turnSmoothVelocity, RotationSpeed);
        RotationPoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Update()
    {
        if (isFiring && (Time.time - m_LastShot) > FireRate)
        {
            m_LastShot = Time.time;

            SpawnProjectileServerRpc();
        }
    }

    public void TakeDamage(float _damage)
    {
        currentHealth -= _damage;
        
        Debug.Log($"Take Damage {_damage}, Health {currentHealth}");
        if (!IsServer) Debug.Log("Not called by SERVER!"); //Should enforce this somewhere

        if (currentHealth <= 0)
        {
            InputManager.Instance.RemoveController(this);
            SetPlayerObjectActiveClientRpc(false);
        }
    }

    [ServerRpc]
    private void SetPlayerObjectActiveServerRpc(bool _isActive)
    {
        SetPlayerObjectActiveClientRpc(_isActive);
    }

    [ClientRpc]
    private void SetPlayerObjectActiveClientRpc(bool _isActive)
    {
        gameObject.SetActive(_isActive);
    }
}
