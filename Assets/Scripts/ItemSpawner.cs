using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

public class ItemSpawner : MonoBehaviourPun
{
    public GameObject[] items;          // ������ ������

    public float maxDistance = 5f;      // �÷��̾� ��ġ���� �������� ��ġ�� �ִ� �ݰ�

    public float timeBetSpawnMax = 7f;  // �ִ� �ð� ����
    public float timeBetSpawnMin =2f;   // �ּ� �ð� ����

    float timeBetSpawn;                 // ��������
    float lastSpawnTime;                // ������ ���� ����
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
