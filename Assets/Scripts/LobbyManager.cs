using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    string gameVersion = "1";
    public Text ConnectionInfoText;
    public Button JoinButton;

    readonly string sceneName = "BattleField";
    private void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();

        JoinButton.interactable = false;
        ConnectionInfoText.text = "������ ������ ���� ��...";
    }
    public override void OnConnectedToMaster()
    {
        JoinButton.interactable = true;
        ConnectionInfoText.text = "�¶��� : ������ ������ �����";
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        JoinButton.interactable = false;
        ConnectionInfoText.text = "�������� : ������ ������ ������� ����\n ���� ��õ� ��....";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Connect()
    {
        JoinButton.interactable = false;

        if(PhotonNetwork.IsConnected)
        {
            ConnectionInfoText.text = "�뿡 ����....";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            ConnectionInfoText.text = "�������� : ������ ������ ������� ����\n ���� ��õ� ��....";
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ConnectionInfoText.text = "�� �� ����, ���ο� �� ����...";
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 4;
        ro.IsOpen = true;
        ro.IsVisible = true;
        PhotonNetwork.CreateRoom(null, ro);
    }

    public override void OnJoinedRoom()
    {
        ConnectionInfoText.text = "�� ���� ����";
        PhotonNetwork.LoadLevel(sceneName);
        // LoadScene�� ������� �ʴ� ����
        // �� ����� ����ϴ� ��� ���� Scene�� ��� ������Ʈ�� �����ϰ� ���� Scene�� ���ε�
        // ���� �κ� Scene�� ��Ʈ��ũ ������ ���� ���� �ʴ´�.
        // ����, �÷��̾���� ���� ����ȭ ���� ���� Scene�� �ε��ϹǷ� Ÿ ĳ���Ͱ� ������ ����
    }

}
