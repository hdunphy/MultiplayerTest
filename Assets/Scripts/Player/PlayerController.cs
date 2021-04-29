using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class PlayerController : NetworkBehaviour, IMoveableObject
{
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float RotationSpeed;
    [SerializeField] private float TankRotationSpeed;
    [SerializeField] private float Health;
    [SerializeField] private float RespawnTime;
    [SerializeField] private int Lives;
    [SerializeField] private Transform RotationPoint;
    [SerializeField] private Transform TankBaseTransform;
    [SerializeField] private List<PlayerFiringController> FiringControllers;
    [SerializeField] private GameEvent OnDiedGameEvent;


    private float turnSmoothVelocity;
    private float m_LastSentMove;
    private float currentHealth;
    private int livesLeft;

    private const float k_MoveSendRateSeconds = 0.05f;

    public event Action<Vector2> UpdateVelocityDirection;
    public event Action OnDiedEvent;

    public NetworkVariableFloat TargetAngle = new NetworkVariableFloat();

    public NetworkVariableVector2 NetworkPosition { get; } = new NetworkVariableVector2();
    public NetworkVariableVector2 FacingDirection { get; } = new NetworkVariableVector2();

    public float _RespawnTime => RespawnTime;
    public bool HasLivesLeft => livesLeft > 0;
    public float GetMoveSpeed() => MoveSpeed;
    public Vector2 GetNetworkPosition() => NetworkPosition.Value;

    #region Unity Functions
    private void Start()
    {
        livesLeft = Lives;
        Initialize();

        if (IsClient)
        {
        }
    }
    private void FixedUpdate()
    {
        //Set gun rotation
        float angle = Mathf.SmoothDampAngle(RotationPoint.eulerAngles.z, TargetAngle.Value, ref turnSmoothVelocity, RotationSpeed);
        RotationPoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Update()
    {
        TankBaseTransform.up = Vector2.MoveTowards(TankBaseTransform.up, FacingDirection.Value, TankRotationSpeed);
    }
    #endregion

    #region Network RPCs
    /* Server RPCs */
    [ServerRpc]
    private void SetIsFiringControllerServerRpc(bool _isFiring, int index)
    {
        FiringControllers[index].SetIsFiring(_isFiring);
    }

    [ServerRpc]
    private void SendCharacterInputServerRpc(Vector2 movementTarget)
    {
        UpdateVelocityDirection?.Invoke(movementTarget);
    }

    [ServerRpc]
    void SetTargetAngleServerRpc(Vector2 direction, ServerRpcParams rpcParams = default)
    {
        direction.Normalize();
        TargetAngle.Value = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    /* Client RPCs */
    [ClientRpc]
    public void OnBulletShotClientRpc(ulong FromClientId)
    {
        if(FromClientId == OwnerClientId)
        {
            Debug.Log($"I Shot ({FromClientId}, {OwnerClientId}");
        }
    }

    [ClientRpc]
    public void OnDiedClientRpc(ulong FromClientId)
    {
        OnDiedGameEvent.Invoke(FromClientId);
    }
    #endregion

    #region Helper Functions
    public void Initialize()
    {
        currentHealth = Health;
        FacingDirection.Value = TankBaseTransform.up;

        if (IsOwner)
        {
            InputManager.Instance.SetLocalPlayerController(this);
        }
    }

    public void SetTransformPosition(Vector2 _position)
    {
        Debug.Log($"is Server: {IsServer}");
        NetworkPosition.Value = _position;
        transform.position = _position;
    }

    public void SetNetworkPosition(Vector2 _position)
    {
        NetworkPosition.Value = _position;
    }

    public void SetFacingDirection(Vector2 _facingDirection)
    {
        FacingDirection.Value = _facingDirection;
    }

    public void SetFiring(bool _isFiring, int index)
    {
        SetIsFiringControllerServerRpc(_isFiring, index);
    }

    public void TakeDamage(float _damage)
    {
        currentHealth -= _damage;

        if (!IsServer) Debug.Log("Not called by SERVER!"); //Should enforce this somewhere

        if (currentHealth <= 0)
        {
            livesLeft--;
            OnDiedEvent?.Invoke(); //For Server

            OnDiedClientRpc(OwnerClientId);
        }
    }
    #endregion

    #region Inputs
    public void Move(Vector2 inputVector)
    {
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
    #endregion
}
