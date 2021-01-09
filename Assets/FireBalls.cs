using System;
using System.Collections.Generic;
using UnityEngine;

public class FireBalls : MonoBehaviour
{


    public GameObject Prefab;
    public Vector3 SpawnPosition;
    public int MinFireballMilliseconds;
    public int MaxFireballMilliseconds;


    private DateTime spawnTime;   
    private List<GameObject> _fireBalls = new List<GameObject>(); 


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
        lock (_fireBalls)
        {
            foreach (GameObject fireBall in _fireBalls)
            {
                Destroy(fireBall);
            }

            _fireBalls.Clear();
        }
    }


    private void SetNextSpawnTime()
    {
        spawnTime = DateTime.Now.AddMilliseconds(UnityEngine.Random.Range(MinFireballMilliseconds, MaxFireballMilliseconds));
    }


    private void DoSpawn()
    {
        GameObject instance = Instantiate(Prefab, SpawnPosition, Quaternion.Euler(0, 0, 0));
        instance.name = "fireball";
        Rigidbody rigidBody = instance.GetComponent<Rigidbody>();
        rigidBody.velocity = new Vector3(0, 0, 0);

        _fireBalls.Add(instance);
    }


}
