using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class Enemy : LivingEntity
{
    public LayerMask whatIsTarget;      // 타겟의 레이어
    LivingEntity targetEntity;          // 타겟(목표 대상
    NavMeshAgent agent;

    public ParticleSystem hitEffect;    // 피격 이펙트
    public AudioClip deathSound;
    public AudioClip hitSound;

    Animator animator;
    AudioSource source;
    [SerializeField]
    SkinnedMeshRenderer meshRenderer;

    public float damage = 20f;          // 공격 력
    public float timeBetAttack = 0.5f;  // 공격 간격
    float lastAttcackTime;              // 마지막으로 공격한 시점

    bool hasTarget
    {
        get 
        { 
            if(targetEntity != null && !targetEntity.dead)
            {
                return true;
            }
            return false;
        }
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        meshRenderer = transform.GetChild(2).GetComponent<SkinnedMeshRenderer>();
    }

    [PunRPC]
    public void Setup(float newHealth, float newDamage, float newSpeed, Color skinColor)
    {
        startingHealth = newHealth;
        health = newHealth;
        damage = newDamage;
        agent.speed = newSpeed;
        meshRenderer.material.color = skinColor;
    }
    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        StartCoroutine(UpdatePath());
    }

    IEnumerator UpdatePath()
    {
        // 죽지 않았다면 무한 루프
        while(!dead)
        {
            if(hasTarget)
            {
                agent.isStopped = false;
                agent.SetDestination(targetEntity.transform.position);
            }
            else
            {
                agent.isStopped = true;

                Collider[] colliders =
                    Physics.OverlapSphere(transform.position, 20f, whatIsTarget);

                foreach(var collider in colliders)
                {
                    LivingEntity livingEntity = collider.GetComponent<LivingEntity>();
                    if(livingEntity != null && !livingEntity.dead)
                    {
                        targetEntity = livingEntity;
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        animator.SetBool("HasTarget", hasTarget);
    }

    [PunRPC]
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        // 죽지 않았다면
        if(!dead)
        {
            // 공격받은 지점과 방향으로 파티클 효과 재생
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();

            // 피격음 재생
            source.PlayOneShot(hitSound);
        }
        // 데미지 적용
        base.OnDamage(damage, hitPoint, hitNormal);
    }

    public override void Die()
    {
        base.Die();

        // 다른 AI를 방해하지 않도록 모든 콜라이더 비활성화
        Collider[] zombieColiders = GetComponents<Collider>();
        foreach(var collider in zombieColiders)
        {
            collider.enabled = false;
        }

        agent.isStopped = true;
        agent.enabled = false;

        animator.SetTrigger("Die");
        source.PlayOneShot(deathSound);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if(!dead && Time.time >= lastAttcackTime + timeBetAttack)
        {
            LivingEntity attackTarget = other.GetComponent<LivingEntity>();

            // 상대방이 자신의 추적 대상이라면 공격
            if(attackTarget != null && attackTarget == targetEntity)
            {
                lastAttcackTime = Time.time;
                // 상대방의 피격 위치와 피격 방향을 근삿값으로 계산
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - other.transform.position;

                // 공격
                attackTarget.OnDamage(damage, hitPoint, hitNormal);
            }
        }
    }
}
