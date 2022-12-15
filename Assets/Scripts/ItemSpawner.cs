using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

public class ItemSpawner : MonoBehaviourPun
{
    public GameObject[] items;          // 생성될 아이템
    public Transform playerTr;

    public float maxDistance = 5f;      // 플레이어 위치에서 아이템이 배치될 최대 반경

    public float timeBetSpawnMax = 7f;  // 최대 시간 간격
    public float timeBetSpawnMin =2f;   // 최소 시간 간격

    float timeBetSpawn;                 // 생성간격
    float lastSpawnTime;                // 마지막 생성 시점

  
    void Start()
    {
        //items = Resources.LoadAll
        timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
        lastSpawnTime = 0f;
    }


    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
            return;

        // 현재 시점이 마지막 생성 시점에서 생성 주기 이상 지남
        if (Time.time >= lastSpawnTime + timeBetSpawn)
        {
            // 마지막 생성 시간 갱신
            lastSpawnTime = Time.time;
            // 생성주기를 랜덤으로 변경
            timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
            // 아이템 스폰
            Spawn();
        }
    }

    void Spawn()
    {
        // 플레이어 근처 NavMesh 위의 랜덤 위치 가져오기
        Vector3 spawnPosition = GetRandomPointOnNavMesh(Vector3.zero, maxDistance);
        spawnPosition += Vector3.up * 0.5f;

        // 아이템을 무작위로 골라 랜덤한 위치에 생성
        GameObject selectedItem = items[Random.Range(0, items.Length)];
        GameObject item = PhotonNetwork.Instantiate(selectedItem.name, spawnPosition, Quaternion.identity);

        //Destroy(item, 5f);
        // 생성한 아이템을 5초 뒤에 파괴
        StartCoroutine(DestroyAfter(item, 5f));
    }

    IEnumerator DestroyAfter(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (target != null)
            PhotonNetwork.Destroy(target);
    }

    // NavMesh위의 랜덤한 위치를 반환하는 함수
    Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance)
    {
        // center를 중심으로 distance 반경 안에서 랜덤한 위치 찾음
        Vector3 randomPos = Random.insideUnitSphere * distance + center;
        // NavMesh 샘플링의 결과 정보를 저장하는 변수
        NavMeshHit hit;
        // maxDistance 반경 안에서 randomPos에 가장 가까운 NavMesh 위의 한 점을 찾음
        NavMesh.SamplePosition(randomPos, out hit, distance, NavMesh.AllAreas);
        // 찾은 지점 반환
        return hit.position;
    }
}
