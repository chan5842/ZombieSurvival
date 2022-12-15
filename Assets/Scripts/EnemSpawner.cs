using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class EnemSpawner : MonoBehaviourPun, IPunObservable
{
    // 싱글 게임의 경우 Resources가 아닌 프리펩으로 관리해도 됨
    // 네트워크 게임은 반드시 Resources로 관리

    public Enemy enemyPrefabs;      // 생성할 적AI
    public Transform[] spawnPoints; // 스폰 위치

    public float damageMax = 40f;   // 최대 공격력
    public float damageMin = 20f;   // 최소 공격력

    public float healthMax = 200f;  // 최대 체력
    public float healthMin = 100f;  // 최소 체력

    public float speedMax = 3f;     // 최대 이동속도
    public float speedMin = 1f;     // 최소 이동속도

    public Color strongEnemyColor = Color.red; // 네임드 표시 컬러
    List<Enemy> enemies = new List<Enemy>();   // 에너미 리스트 
    int remainEnemy = 0;
    int wave;                                  // 웨이브 수

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(enemies.Count);
            stream.SendNext(wave);
        }
        else
        {
            remainEnemy = (int)stream.ReceiveNext();
            wave = (int)stream.ReceiveNext();
        }
    }
    
    void Awake()
    {
        photonView.ObservedComponents[0] = this;
        PhotonPeer.RegisterType(typeof(Color), 128, ColorSerialization.SerializeColor, ColorSerialization.DeserializeColor);
    }

    void Update()
    {
        // 호스트만 좀비 생성
        if(PhotonNetwork.IsMasterClient)
        {
            if (GameManager.instance != null &&
              GameManager.instance.isGameover)
            {
                return;
            }
            if (enemies.Count <= 0)
                SpawnWave();
        }
        UpdateUI();
    }

    // UI갱신
    void UpdateUI()
    {
        // 현재 웨이브아 남은 적 수 표시
        if (PhotonNetwork.IsMasterClient)
            UIManager.instance.UpdateWaveText(wave, enemies.Count);
    }

    void SpawnWave()
    {
        wave++;

        // 에너미의 수는 웨이브의 1.5배만 소환
        int spawnCount = Mathf.RoundToInt(wave * 1.5f);
        // 강함에 따른 에너미 생성
        for(int i=0; i<spawnCount; i++)
        {
            float enemyIntensity = Random.Range(0f, 1f);
            CreateEnemy(enemyIntensity);
        }
    }

    void CreateEnemy(float intensity)
    {
        float health = Mathf.Lerp(healthMin, healthMax, intensity);
        float damage = Mathf.Lerp(damageMin, damageMin, intensity);
        float speed = Mathf.Lerp(speedMin, speedMax, intensity);
        Color skinColor = Color.Lerp(Color.white, strongEnemyColor, intensity);

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject createEnemy = PhotonNetwork.Instantiate(enemyPrefabs.gameObject.name, spawnPoint.position, spawnPoint.rotation);

        Enemy enemy = createEnemy.GetComponent<Enemy>();

        enemy.photonView.RPC("Setup", RpcTarget.All, health, damage, speed, skinColor);
        enemies.Add(enemy);

        // 좀비의 onDeath 이벤트에 익명 메소드 등록
        enemy.onDeath += () => enemies.Remove(enemy);
        enemy.onDeath += () => StartCoroutine(DestroyAfter(enemy.gameObject, 10f));
        enemy.onDeath += () => GameManager.instance.AddScore(100);
    }

    IEnumerator DestroyAfter(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);

        if(target != null)
        {
            PhotonNetwork.Destroy(target);
        }
    }


}
