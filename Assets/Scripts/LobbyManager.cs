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
        ConnectionInfoText.text = "마스터 서버에 접속 중...";
    }
    public override void OnConnectedToMaster()
    {
        JoinButton.interactable = true;
        ConnectionInfoText.text = "온라인 : 마스터 서버와 연결됨";
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        JoinButton.interactable = false;
        ConnectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음\n 접속 재시도 중....";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Connect()
    {
        JoinButton.interactable = false;

        if(PhotonNetwork.IsConnected)
        {
            ConnectionInfoText.text = "룸에 접속....";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            ConnectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음\n 접속 재시도 중....";
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ConnectionInfoText.text = "빈 방 없음, 새로운 방 생성...";
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 4;
        ro.IsOpen = true;
        ro.IsVisible = true;
        PhotonNetwork.CreateRoom(null, ro);
    }

    public override void OnJoinedRoom()
    {
        ConnectionInfoText.text = "방 참가 성공";
        PhotonNetwork.LoadLevel(sceneName);
        // LoadScene을 사용하지 않는 이유
        // 위 방법을 사용하는 경우 이전 Scene의 모든 오브젝트를 삭제하고 다음 Scene을 ㄹ로드
        // 따라서 로비 Scene의 네트워크 정보가 유지 되지 않는다.
        // 또한, 플레이어들이 서로 동기화 없이 각자 Scene을 로드하므로 타 캐릭터가 보이지 않음
    }

}
