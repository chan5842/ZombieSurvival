using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerShooter : MonoBehaviourPun
{
    public Gun gun;
    public Transform gunPivot;          // �� ��ġ ������
    public Transform leftHandMount;     // ���� ������
    public Transform rightHandMount;   // ������ ������

    PlayerInput playerInput;
    Animator animator;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        gun.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        gun.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;
        // �߻� �Է� �����Ǹ�
        if(playerInput.fire)
        {
            // �Ѿ˹߻�
            gun.Fire();
        }
        // ������ �����Ǹ�
        else if(playerInput.reload)
        {
            // �������� ������ ���¶��
            if(gun.Reload())
            {
                // ������ �ִϸ��̼� ����
                animator.SetTrigger("Reload");
            }
        }

        UpdateUI();
    }
    
    void UpdateUI()
    {
        if (gun != null && UIManager.instance != null)
        {
            UIManager.instance.UpdateAmmoText(gun.magAmmo, gun.ammoRemain);
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        // ���� �������� ���� ������ �Ȳ�ġ ��ġ�� �̵�
        gunPivot.position = animator.GetIKHintPosition(AvatarIKHint.RightElbow);

        // IK�� ����Ͽ� �޼��� ��ġ�� ȸ���� ���� ���� �����̿� ����
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);

        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMount.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMount.rotation);

        // IK�� ����Ͽ� �������� ��ġ�� ȸ���� ���� ������ �����̿� ����
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);

        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMount.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandMount.rotation);

    }
}
