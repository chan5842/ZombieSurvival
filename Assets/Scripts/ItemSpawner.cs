using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

public class ItemSpawner : MonoBehaviourPun
{
    public GameObject[] items;          // ������ ������
    public Transform playerTr;

    public float maxDistance = 5f;      // �÷��̾� ��ġ���� �������� ��ġ�� �ִ� �ݰ�

    public float timeBetSpawnMax = 7f;  // �ִ� �ð� ����
    public float timeBetSpawnMin =2f;   // �ּ� �ð� ����

    float timeBetSpawn;                 // ��������
    float lastSpawnTime;                // ������ ���� ����

  
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

        // ���� ������ ������ ���� �������� ���� �ֱ� �̻� ����
        if (Time.time >= lastSpawnTime + timeBetSpawn)
        {
            // ������ ���� �ð� ����
            lastSpawnTime = Time.time;
            // �����ֱ⸦ �������� ����
            timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
            // ������ ����
            Spawn();
        }
    }

    void Spawn()
    {
        // �÷��̾� ��ó NavMesh ���� ���� ��ġ ��������
        Vector3 spawnPosition = GetRandomPointOnNavMesh(Vector3.zero, maxDistance);
        spawnPosition += Vector3.up * 0.5f;

        // �������� �������� ��� ������ ��ġ�� ����
        GameObject selectedItem = items[Random.Range(0, items.Length)];
        GameObject item = PhotonNetwork.Instantiate(selectedItem.name, spawnPosition, Quaternion.identity);

        //Destroy(item, 5f);
        // ������ �������� 5�� �ڿ� �ı�
        StartCoroutine(DestroyAfter(item, 5f));
    }

    IEnumerator DestroyAfter(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (target != null)
            PhotonNetwork.Destroy(target);
    }

    // NavMesh���� ������ ��ġ�� ��ȯ�ϴ� �Լ�
    Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance)
    {
        // center�� �߽����� distance �ݰ� �ȿ��� ������ ��ġ ã��
        Vector3 randomPos = Random.insideUnitSphere * distance + center;
        // NavMesh ���ø��� ��� ������ �����ϴ� ����
        NavMeshHit hit;
        // maxDistance �ݰ� �ȿ��� randomPos�� ���� ����� NavMesh ���� �� ���� ã��
        NavMesh.SamplePosition(randomPos, out hit, distance, NavMesh.AllAreas);
        // ã�� ���� ��ȯ
        return hit.position;
    }
}
