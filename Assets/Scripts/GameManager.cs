using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<GameManager>();
            }
            return m_instance;
        }
    }
    static GameManager m_instance;

    GameObject playerPrefab;

    int score = 0;
    public bool isGameover { get; private set; }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(score);
        }
        else
        {
            score = (int)stream.ReceiveNext();
            UIManager.instance.UpdateScoreText(score);
        }
    }

    private void Awake()
    {
        if (instance != this)
            Destroy(gameObject);

        playerPrefab = Resources.Load("Player Character") as GameObject;
    }

    private void Start()
    {
        FindObjectOfType<PlayerHealth>().onDeath += EndGame;

        Vector3 randomSpawnPos = Random.insideUnitSphere * 5f;
        randomSpawnPos.y = 0f;
        PhotonNetwork.Instantiate(playerPrefab.name, randomSpawnPos, Quaternion.identity);
        
    }
    public void AddScore(int newScore)
    {
        if (!isGameover)
        {
            score += newScore;
            UIManager.instance.UpdateScoreText(score);
        }
    }

    public void EndGame()
    {
        isGameover = true;
        UIManager.instance.SetActiveGameOverUI(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            PhotonNetwork.LeaveRoom();
    }

    // 방에서 나갈 때 자동으로 실행
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }


}
