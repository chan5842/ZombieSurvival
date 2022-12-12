using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth = 100f; // ���� ü��
    public float health { get; protected set; } // ���� ü��
    public bool dead { get; protected set; }    // ��� ����
    public event Action onDeath;                // ��� �� �ߵ��� �̺�Ʈ

    protected virtual void OnEnable()
    {
        dead = false;
        health = startingHealth;    // ���� ü���� ���� ü������ �ʱ�ȭ
    }

    // �������� �Դ� ���
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        // ü���� ������ ��ŭ ����
        health -= damage;

        // ü���� 0���� �۰ų� ������ Die�Լ� ����
        if(health <=0 && !dead)
        {
            Die();
        }
    }
    // ü�� ȸ���ϴ� ���
    public virtual void RestoreHealth(float newHealth)
    {
        // �׾����� �Լ� ����
        if(dead)
        {
            return;
        }
        // ���� ü�¿� newHealth��ŭ ȸ��
        health += newHealth;
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
