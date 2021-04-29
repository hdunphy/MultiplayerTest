using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocationManager : MonoBehaviour
{
    [SerializeField] private List<Vector2> RespawnLocations;
    [SerializeField] private Color GizmoColor;

    public static SpawnLocationManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Input manager already exists");
            Destroy(this);
        }
    }

    public static Vector2 GetRandomSpawn()
    {
        Vector2 location = Vector2.zero;

        if(Instance.RespawnLocations.Count > 0)
        {
            location = Instance.RespawnLocations[UnityEngine.Random.Range(0, Instance.RespawnLocations.Count)];
        }

        return location;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = GizmoColor;
        foreach(Vector2 _loc in RespawnLocations)
        {
            Gizmos.DrawSphere(_loc, .25f);
        }
    }
}
