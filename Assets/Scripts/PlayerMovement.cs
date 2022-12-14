using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    public float moveSpeed = 5f; //�յ� �������� �ӵ�
    public float rotateSpeed = 180f; //�¿� ȸ�� �ӵ�
    
    private PlayerInput playerInput; //�÷��̾� �Է��� �˷��ִ� ������Ʈ

    private Rigidbody playerRigidbody;
    private Animator playerAnimator;

    private readonly int hashMove = Animator.StringToHash("Move");

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
    }

    
    void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

        Rotate();
        Move();

        playerAnimator.SetFloat(hashMove, playerInput.move);
    }

    void Move()
    {
        Vector3 moveDistance = playerInput.move * transform.forward * moveSpeed * Time.deltaTime;
        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
    }
    void Rotate()
    {
        float turn = playerInput.rotate * rotateSpeed * Time.deltaTime;
        playerRigidbody.rotation = playerRigidbody.rotation * Quaternion.Euler(0, turn, 0f);
    }
}
