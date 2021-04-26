using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

//Server only
public class PlayerFiringController : NetworkBehaviour
{
    [SerializeField, Min(1)] int MaxPrefabsInScene;
    [SerializeField] float FireRate;
    [SerializeField] Transform FirePoint;
    [SerializeField, Tooltip("Must have IShootable Component")] IShootable PrefabToShoot;
    [SerializeField] GameEvent PlayerFireEvent;

    public bool IsFiring { get; private set; }

    private int CurrentPrefabCount;
    private float m_LastShot;

    public override void NetworkStart()
    {
        if(!IsServer)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (IsFiring && (CurrentPrefabCount < MaxPrefabsInScene) && ((Time.time - m_LastShot) > FireRate))
        {
            m_LastShot = Time.time;

            SpawnPrefab();
        }
    }

    private void SpawnPrefab()
    {
        //Debug.Log($"Owner {OwnerClientId} vs Network Owner {transform.parent.GetComponent<NetworkObject>().OwnerClientId}"); //Same
        ShootEventClientRpc(OwnerClientId);
        var _prefab = Instantiate(PrefabToShoot, FirePoint.position, FirePoint.rotation);
        _prefab.Initialize(this);
        _prefab.GetComponent<NetworkObject>().Spawn();
    }

    [ClientRpc]
    private void ShootEventClientRpc(ulong _ownerClientId)
    {
        PlayerFireEvent?.Invoke(_ownerClientId);
    }

    public void SetIsFiring(bool _isFiring) => IsFiring = _isFiring;
    public void AddPrefab() => CurrentPrefabCount++;
    public void RemovePrefab() => CurrentPrefabCount--;
}
