using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemSpawner : MonoBehaviour
{
    // �̱� ������ ��� Resources�� �ƴ� ���������� �����ص� ��
    // ��Ʈ��ũ ������ �ݵ�� Resources�� ����

    public Enemy enemyPrefabs;      // ������ ��AI
    public Transform[] spawnPoints; // ���� ��ġ

    public float damageMax = 40f;   // �ִ� ���ݷ�
    public float damageMin = 20f;   // �ּ� ���ݷ�

    public float healthMax = 200f;  // �ִ� ü��
    public float healthMin = 100f;  // �ּ� ü��

    public float speedMax = 3f;     // �ִ� �̵��ӵ�
    public float speedMin = 1f;     // �ּ� �̵��ӵ�

    public Color strongEnemyColor = Color.red; // ���ӵ� ǥ�� �÷�
    List<Enemy> enemies = new List<Enemy>();   // ���ʹ� ����Ʈ 
    int wave;                                  // ���̺� ��

    void Update()
    {
        if(GameManager.instance != null &&
            GameManager.instance.isGameover)
        {
            return;
        }
        if (enemies.Count <= 0)
            SpawnWave();

        UpdateUI();
    }

    // UI����
    void UpdateUI()
    {
        UIManager.instance.UpdateWaveText(wave, enemies.Count);
    }

    void SpawnWave()
    {
        wave++;

        // ���ʹ��� ���� ���̺��� 1.5�踸 ��ȯ
        int spawnCount = Mathf.RoundToInt(wave * 1.5f);
        // ���Կ� ���� ���ʹ� ����
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
        Enemy enemy = Instantiate(enemyPrefabs, spawnPoint.position, spawnPoint.rotation);

        enemy.Setup(health, damage, speed, skinColor);
        enemies.Add(enemy);

        // ������ onDeath �̺�Ʈ�� �͸� �޼ҵ� ���
        enemy.onDeath += () => enemies.Remove(enemy);
        enemy.onDeath += () => Destroy(enemy.gameObject, 10f);
        enemy.onDeath += () => GameManager.instance.AddScore(100);
    }
}
