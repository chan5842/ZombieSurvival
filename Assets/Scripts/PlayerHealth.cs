using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHealth : LivingEntity
{
    public Slider healthSlider;     // ü�� UI

    public AudioClip deathClip;     // ��� �Ҹ�
    public AudioClip hitClip;       // �ǰ� �Ҹ�
    public AudioClip itemPickupClip;// ������ ���� �Ҹ�

    AudioSource source;
    Animator animator;

    PlayerMovement playerMovement;
    PlayerShooter playerShooter;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        healthSlider.gameObject.SetActive(true);    // ü�� �����̴� Ȱ��ȭ
        healthSlider.maxValue = startingHealth;     // ü�� �����̴� �ִ� ���� ���� ü�� ������ ����
        healthSlider.value = health;

        playerMovement.enabled = true;
        playerShooter.enabled = true;
    }

    [PunRPC]
    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth);
        healthSlider.value = health;    // ���ŵ� ü������ �����̴� ����
    }

    [PunRPC]
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        // ��� ���°� �ƴ϶�� �ǰ� �Ҹ� ���
        if(!dead)
        {
            source.PlayOneShot(hitClip);
        }
        base.OnDamage(damage, hitPoint, hitNormal);
        healthSlider.value = health;    // ���ŵ� ü������ �����̴� ����
    }

    public override void Die()
    {
        base.Die();

        healthSlider.gameObject.SetActive(false);   // UI ��Ȱ��ȭ

        source.PlayOneShot(deathClip);              // ��� �Ҹ� ���
        animator.SetTrigger("Die");                 // ��� �ִϸ��̼� Ʈ���� ���

        // ��ũ��Ʈ ��Ȱ��ȭ
        playerMovement.enabled = false;             
        playerShooter.enabled = false;

        Invoke("Respawn", 5f);
    }

    // �����۰� �浹�� ���
    private void OnTriggerEnter(Collider other)
    {
        if(!dead)
        {
            IItem item = other.GetComponent<IItem>();

            if(item != null)
            {
                if(PhotonNetwork.IsMasterClient)
                    item.Use(gameObject);
            }
            source.PlayOneShot(itemPickupClip);
        }
    }
    void Respawn()
    {
        if(photonView.IsMine)
        {
            Vector3 randomSpawnPos = Random.insideUnitSphere * 5f;
            randomSpawnPos.y = 0f;

            transform.position = randomSpawnPos;
        }

        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
