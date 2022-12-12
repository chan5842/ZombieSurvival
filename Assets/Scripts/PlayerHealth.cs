using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth);
        healthSlider.value = health;    // ���ŵ� ü������ �����̴� ����
    }

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
    }

    // �����۰� �浹�� ���
    private void OnTriggerEnter(Collider other)
    {
        if(!dead)
        {
            IItem item = other.GetComponent<IItem>();

            if(item != null)
            {
                item.Use(gameObject);
                source.PlayOneShot(itemPickupClip);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
