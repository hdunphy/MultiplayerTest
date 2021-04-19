using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System;
using UnityEngine;

public class PlayerController : NetworkBehaviour, IDamageable
{
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float RotationSpeed;
    [SerializeField] private float FireRate;
    [SerializeField] private float Health;
    [SerializeField] private Rigidbody2D Rb;
    [SerializeField] private Transform RotationPoint;
    [SerializeField] private Transform FirePoint;
    [SerializeField] private Projectile ProjectilePrefab;

    private float turnSmoothVelocity;
    private float m_LastSentMove;
    private float m_LastShot;
    private bool isFiring;
    private float currentHealth;

    private const float k_MoveSendRateSeconds = 0.05f;


    public NetworkVariableFloat TargetAngle = new NetworkVariableFloat();
    public NetworkVariableVector2 Velocity = new NetworkVariableVector2();

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
        SetVelocityServerRpc(inputVector);
    }

    public void Look(Vector2 mousePos)
    {
        if((Time.time - m_LastSentMove) > k_MoveSendRateSeconds) //Can send a move update
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

    public void SetFiring(bool _isFiring)
    {
        isFiring = _isFiring;
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
        _projectile.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
    }

    private void FixedUpdate()
    {
        //Set Velocity
        Rb.velocity = Velocity.Value * Time.deltaTime;

        //Set gun rotation
        float angle = Mathf.SmoothDampAngle(RotationPoint.eulerAngles.z, TargetAngle.Value, ref turnSmoothVelocity, RotationSpeed);
        RotationPoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Update()
    {
        if(isFiring && (Time.time - m_LastShot) > FireRate)
        {
            m_LastShot = Time.time;

            SpawnProjectileServerRpc();
        }
    }

    public void TakeDamage(float _damage)
    {
        currentHealth -= _damage;
        Debug.Log($"Take Damage {_damage}, Health {currentHealth}");
        if(currentHealth <= 0)
        {
            InputManager.Instance.RemoveController(this);
            SetPlayerObjectActiveServerRpc(false);
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
