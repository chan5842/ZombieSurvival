using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInput : MonoBehaviourPun
{
    public string moveAxisName = "Vertical";  //�յ� �������� ���� �Է��� �̸�
    public string rotateAxisName = "Horizontal"; //�¿� ȸ���� ���� �Է��� �̸�
    public string fireButtonName = "Fire1"; //�߻縦 ���� �Է� ��ư �̸�
    public string reloadButtonName = "Reload"; //�������� ���� �Է� ��ư �̸�

    //�� �Ҵ��� ���ο����� ����
    public float move { get; private set; } //������ ������ �Է°�
    public float rotate { get; private set; } //������ ȸ�� �Է°�
    public bool fire { get; private set; } //������ �߻� �Է°�
    public bool reload { get; private set; } //������ ������ �Է°�

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
