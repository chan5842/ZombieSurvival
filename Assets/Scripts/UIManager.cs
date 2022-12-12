using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // �̱����� �Ҵ�� ������Ƽ
    public static UIManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<UIManager>();
            }
            return m_instance;
        }
    }

    // �̱��� ����
    // ���к��� ��ü ������ ���� �ܺο��� ���� ������ �� �ֵ��� �ϴ� ��
    static UIManager m_instance;    

    public Text ammoText;           // ź�� ǥ�ÿ� �ؽ�Ʈ
    public Text scoreText;          // ���� ǥ�ÿ� �ؽ�Ʈ
    public Text waveText;           // �� ���̺� ǥ�ÿ� �ؽ�Ʈ
    public GameObject gameoverUI;   // ���� ������ Ȱ��ȭ �� UI

    // ź�� �ؽ�Ʈ ����
    public void UpdateAmmoText(int magAmmo, int remainAmmo)
    {
        ammoText.text = magAmmo + "/" + remainAmmo;
    }

    // ���� �ؽ�Ʈ ����
    public void UpdateScoreText(int newScore)
    {
        scoreText.text = "Score : " + newScore;
    }

    // �� ���̺� �ؽ�Ʈ ����
    public void UpdateWaveText(int newWave, int newCount)
    {
        waveText.text = "Wave : " + newWave + "\nEnemy Left : " + newCount;
    }

    // �� ���̺� �ؽ�Ʈ ����
    public void SetActiveGameOverUI(bool active)
    {
        gameoverUI.SetActive(active);
    }

    // ���� �����
    public void GameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
