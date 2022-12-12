using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : LivingEntity
{
    public Slider healthSlider;     // 체력 UI

    public AudioClip deathClip;     // 사망 소리
    public AudioClip hitClip;       // 피격 소리
    public AudioClip itemPickupClip;// 아이템 줍줍 소리

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

        healthSlider.gameObject.SetActive(true);    // 체력 슬라이더 활성화
        healthSlider.maxValue = startingHealth;     // 체력 슬라이더 최대 값을 현재 체력 값으로 변경
        healthSlider.value = health;

        playerMovement.enabled = true;
        playerShooter.enabled = true;
    }

    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth);
        healthSlider.value = health;    // 갱신된 체력으로 슬라이더 갱신
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        // 사망 상태가 아니라면 피격 소리 출력
        if(!dead)
        {
            source.PlayOneShot(hitClip);
        }
        base.OnDamage(damage, hitPoint, hitNormal);
        healthSlider.value = health;    // 갱신된 체력으로 슬라이더 갱신
    }

    public override void Die()
    {
        base.Die();

        healthSlider.gameObject.SetActive(false);   // UI 비활성화

        source.PlayOneShot(deathClip);              // 사망 소리 출력
        animator.SetTrigger("Die");                 // 사망 애니메이션 트리거 재생

        // 스크립트 비활성화
        playerMovement.enabled = false;             
        playerShooter.enabled = false;
    }

    // 아이템과 충돌시 사용
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
