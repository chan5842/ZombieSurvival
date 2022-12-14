using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInput : MonoBehaviourPun
{
    public string moveAxisName = "Vertical";  //앞뒤 움직임을 위한 입력출 이름
    public string rotateAxisName = "Horizontal"; //좌우 회전을 위한 입력축 이름
    public string fireButtonName = "Fire1"; //발사를 위한 입력 버튼 이름
    public string reloadButtonName = "Reload"; //재장전을 위한 입력 버튼 이름

    //값 할당은 내부에서만 가능
    public float move { get; private set; } //감지된 움직임 입력값
    public float rotate { get; private set; } //감지된 회전 입력값
    public bool fire { get; private set; } //감지된 발사 입력값
    public bool reload { get; private set; } //감지된 재장전 입력값

    void Update()
    {
        if (!photonView.IsMine)
            return;
        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            move = 0;
            rotate = 0;
            fire = false;
            reload = false;
            return;
        }

        move = Input.GetAxis(moveAxisName);
        rotate = Input.GetAxis(rotateAxisName);
        fire = Input.GetButton(fireButtonName);
        reload = Input.GetButtonDown(reloadButtonName);
    }
}
