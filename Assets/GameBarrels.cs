using System;
using System.Collections.Generic;
using UnityEngine;


public class GameBarrels : MonoBehaviour
{


    public GameObject Prefab;
    public Vector3 SpawnPosition;
    public Vector3 Rotation;
    public Vector3 Scale;
    public float InitialBarrelVelocity;
    public int MinBarrelMilliseconds;
    public int MaxBarrelMilliseconds;

    private DateTime spawnTime;   
    private List<GameObject> _barrels = new List<GameObject>(); 


    void Start()
    {
        SetNextSpawnTime();
    }


    void Update()
    {
        if (DateTime.Now > spawnTime)
        {
            SetNextSpawnTime();
            DoSpawn();
        }
    }


    public void Reset()
    {
        lock (_barrels)
        {
            foreach (GameObject barrel in _barrels)
            {
                Destroy(barrel);
            }

            _barrels.Clear();
        }
    }


    private void SetNextSpawnTime()
    {
        spawnTime = DateTime.Now.AddMilliseconds(UnityEngine.Random.Range(MinBarrelMilliseconds, MaxBarrelMilliseconds));
    }


    private void DoSpawn()
    {
        GameObject instance = Instantiate(Prefab, SpawnPosition, Quaternion.Euler(Rotation.x, Rotation.y, Rotation.z));
        instance.name = "barrel";
        Rigidbody rigidBody = instance.GetComponent<Rigidbody>();
        rigidBody.velocity = new Vector3(0, 0, InitialBarrelVelocity);

        GameBarrel barrel = instance.GetComponent<GameBarrel>();
        barrel.OnShouldDestroy += OnShouldDestroy;

        _barrels.Add(instance);
    }


    private void OnShouldDestroy(GameObject barrel)
    {
        lock (_barrels)
        {
            _barrels.Remove(barrel);
        }
        barrel.GetComponent<GameBarrel>().Explode();
    }


}
