using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class LivingEntity : MonoBehaviourPun, IDamageable
{
    public float startingHealth = 100f; // 시작 체력
    public float health { get; protected set; } // 현재 체력
    public bool dead { get; protected set; }    // 사망 상태
    public event Action onDeath;                // 사망 시 발동할 이벤트

    [PunRPC]
    public void ApplyUpdateHealth(float newHealth, bool newDead)
    {
        health = newHealth;
        dead = newDead;
    }

    protected virtual void OnEnable()
    {
        dead = false;
        health = startingHealth;    // 현재 체력을 시작 체력으로 초기화
    }

    [PunRPC]
    // 데미지를 입는 기능
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            // 체력이 데미지 만큼 감소
            health -= damage;

            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health, dead);

            photonView.RPC("OnDamage", RpcTarget.Others, hitPoint, hitNormal);
        }

        // 체력이 0보다 작거나 같으면 Die함수 실행
        if(health <=0 && !dead)
        {
            Die();
        }
    }

    [PunRPC]
    // 체력 회복하는 기능
    public virtual void RestoreHealth(float newHealth)
    {
        // 죽었으면 함수 종료
        if(dead)
        {
            return;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            // 현재 체력에 newHealth만큼 회복
            health += newHealth;
            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health, dead);
            photonView.RPC("RestoreHelath", RpcTarget.Others, newHealth);
        }
            
    }

    // 사망 처리
    public virtual void Die()
    {
        // onDeath 이벤트에 등록된 메소드가 있다면 실행
        if(onDeath != null)
        {
            onDeath();
        }

        // 사망한 상태로 변경
        dead = true;
    }
}
