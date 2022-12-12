using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // 싱글턴이 할당된 프로퍼티
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

    // 싱글턴 변수
    // 무분별한 객체 생성을 막고 외부에서 쉽게 접근할 수 있도록 하는 것
    static UIManager m_instance;    

    public Text ammoText;           // 탄약 표시용 텍스트
    public Text scoreText;          // 점수 표시용 텍스트
    public Text waveText;           // 적 웨이브 표시용 텍스트
    public GameObject gameoverUI;   // 게임 오버시 활성화 할 UI

    // 탄약 텍스트 갱신
    public void UpdateAmmoText(int magAmmo, int remainAmmo)
    {
        ammoText.text = magAmmo + "/" + remainAmmo;
    }

    // 점수 텍스트 갱신
    public void UpdateScoreText(int newScore)
    {
        scoreText.text = "Score : " + newScore;
    }

    // 적 웨이브 텍스트 갱신
    public void UpdateWaveText(int newWave, int newCount)
    {
        waveText.text = "Wave : " + newWave + "\nEnemy Left : " + newCount;
    }

    // 적 웨이브 텍스트 갱신
    public void SetActiveGameOverUI(bool active)
    {
        gameoverUI.SetActive(active);
    }

    // 게임 재시작
    public void GameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
