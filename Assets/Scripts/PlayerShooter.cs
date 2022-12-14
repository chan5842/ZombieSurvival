using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerShooter : MonoBehaviourPun
{
    public Gun gun;
    public Transform gunPivot;          // 총 배치 기준점
    public Transform leftHandMount;     // 왼쪽 손잡이
    public Transform rightHandMount;   // 오른쪽 손잡이

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
        // 발사 입력 감지되면
        if(playerInput.fire)
        {
            // 총알발사
            gun.Fire();
        }
        // 재장전 감지되면
        else if(playerInput.reload)
        {
            // 재장전이 가능한 상태라면
            if(gun.Reload())
            {
                // 재장전 애니메이션 실행
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
        // 총의 기준점을 모델의 오른쪽 팔꿈치 위치로 이동
        gunPivot.position = animator.GetIKHintPosition(AvatarIKHint.RightElbow);

        // IK를 사용하여 왼손의 위치와 회전을 총의 왼쪽 손잡이에 맞춤
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);

        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMount.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMount.rotation);

        // IK를 사용하여 오른손의 위치와 회전을 총의 오른쪽 손잡이에 맞춤
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);

        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMount.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandMount.rotation);

    }
}
