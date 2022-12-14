using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

public class ItemSpawner : MonoBehaviourPun
{
    public GameObject[] items;          // 생성될 아이템

    public float maxDistance = 5f;      // 플레이어 위치에서 아이템이 배치될 최대 반경

    public float timeBetSpawnMax = 7f;  // 최대 시간 간격
    public float timeBetSpawnMin =2f;   // 최소 시간 간격

    float timeBetSpawn;                 // 생성간격
    float lastSpawnTime;                // 마지막 생성 시점
    void Start()
    {
        //items = Resources.LoadAll
    }


    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
            return;
        lastSpawnTime = Time.time;
        timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
        Spawn();
    }

    //void Spawn()
    //{
    //    Vector3 spawnPosition; //= GetRandomPointOnNavMesh(Vector3.zero, maxDistance);
    //    spawnPosition += Vector3.up * 0.5f;

    //    GameObject selectedItem = items[Random.Range(0, items.Length)];
    //    GameObject item = PhotonNetwork.Instantiate(selectedItem.name, spawnPosition, Quaternion.identity);

    //    StartCoroutine(DestroyAfter(item, 5f));
    //}

    IEnumerator DestroyAfter(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);

        if(target)
    }
}
