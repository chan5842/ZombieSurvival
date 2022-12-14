using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class Enemy : LivingEntity
{
    public LayerMask whatIsTarget;      // Ÿ���� ���̾�
    LivingEntity targetEntity;          // Ÿ��(��ǥ ���
    NavMeshAgent agent;

    public ParticleSystem hitEffect;    // �ǰ� ����Ʈ
    public AudioClip deathSound;
    public AudioClip hitSound;

    Animator animator;
    AudioSource source;
    [SerializeField]
    SkinnedMeshRenderer meshRenderer;

    public float damage = 20f;          // ���� ��
    public float timeBetAttack = 0.5f;  // ���� ����
    float lastAttcackTime;              // ���������� ������ ����

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
        // ���� �ʾҴٸ� ���� ����
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
        // ���� �ʾҴٸ�
        if(!dead)
        {
            // ���ݹ��� ������ �������� ��ƼŬ ȿ�� ���
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();

            // �ǰ��� ���
            source.PlayOneShot(hitSound);
        }
        // ������ ����
        base.OnDamage(damage, hitPoint, hitNormal);
    }

    public override void Die()
    {
        base.Die();

        // �ٸ� AI�� �������� �ʵ��� ��� �ݶ��̴� ��Ȱ��ȭ
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

            // ������ �ڽ��� ���� ����̶�� ����
            if(attackTarget != null && attackTarget == targetEntity)
            {
                lastAttcackTime = Time.time;
                // ������ �ǰ� ��ġ�� �ǰ� ������ �ٻ����� ���
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - other.transform.position;

                // ����
                attackTarget.OnDamage(damage, hitPoint, hitNormal);
            }
        }
    }
}
