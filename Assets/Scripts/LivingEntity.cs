using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class LivingEntity : MonoBehaviourPun, IDamageable
{
    public float startingHealth = 100f; // ���� ü��
    public float health { get; protected set; } // ���� ü��
    public bool dead { get; protected set; }    // ��� ����
    public event Action onDeath;                // ��� �� �ߵ��� �̺�Ʈ

    [PunRPC]
    public void ApplyUpdateHealth(float newHealth, bool newDead)
    {
        health = newHealth;
        dead = newDead;
    }

    protected virtual void OnEnable()
    {
        dead = false;
        health = startingHealth;    // ���� ü���� ���� ü������ �ʱ�ȭ
    }

    [PunRPC]
    // �������� �Դ� ���
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            // ü���� ������ ��ŭ ����
            health -= damage;

            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health, dead);

            photonView.RPC("OnDamage", RpcTarget.Others, hitPoint, hitNormal);
        }

        // ü���� 0���� �۰ų� ������ Die�Լ� ����
        if(health <=0 && !dead)
        {
            Die();
        }
    }

    [PunRPC]
    // ü�� ȸ���ϴ� ���
    public virtual void RestoreHealth(float newHealth)
    {
        // �׾����� �Լ� ����
        if(dead)
        {
            return;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            // ���� ü�¿� newHealth��ŭ ȸ��
            health += newHealth;
            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health, dead);
            photonView.RPC("RestoreHelath", RpcTarget.Others, newHealth);
        }
            
    }

    // ��� ó��
    public virtual void Die()
    {
        // onDeath �̺�Ʈ�� ��ϵ� �޼ҵ尡 �ִٸ� ����
        if(onDeath != null)
        {
            onDeath();
        }

        // ����� ���·� ����
        dead = true;
    }
}
