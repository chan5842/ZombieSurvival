using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum State
    {
        Ready,      // 발사 준비
        Empty,      // 탄창 빔
        Reloading   // 재장전 중
    }

    // 현재 총의 상태
    public State state { get; private set; }

    // 총알 발사 위치
    public Transform fireTransform;

    public ParticleSystem muzzleFlashEffect;    // 총구 화염 효과
    public ParticleSystem shellEjectEffect;     // 탄피 배출 효과

    LineRenderer bulletLineRenderer;    // 총알 궤적을 위한 라인 렌더러
    AudioSource gunAudioPlayer;         // 총소리 재생기
    public GunData gunData;             // 총의 현재 데이터

    float fireDistance = 50f;           // 사정 거리
    public int ammoRemain = 100;        // 남은 전체 총알
    public int magAmmo;                 // 현재 탄창에 남아있는 ㅌ총알

    float lastFireTime;                 // 총알 마지막 발사 시간

    void Awake()
    {
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();

        bulletLineRenderer.positionCount = 2;
        bulletLineRenderer.enabled = false;
    }

    private void OnEnable()
    {
        // 전체 탄창 초기화
        ammoRemain = gunData.startAmmoRemain;
        // 현재 탄창 초기화
        magAmmo = gunData.magCapacity;
        // 발사 준비 상태로 변환
        state = State.Ready;
        // 마지막 발사시간 초기화
        lastFireTime = 0f;
    }

    public void Fire()
    {
        // 준비상태이고 마지막 발사 시점에서 발사간격만큼의 시간이 흘렀다면
        if((state == State.Ready) && (Time.time >= lastFireTime + gunData.timeBetFire))
        {
            // 마지막 총 발사 시점 갱신
            lastFireTime = Time.time;
            // 발사
            Shot();
        }
    }

    void Shot()
    {
        RaycastHit hit;
        // 총알이 맞은 위치
        Vector3 hitPosition = Vector3.zero;

        if(Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance))
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>();

            if (target != null)
            {
                target.OnDamage(gunData.damage, hit.point, hit.normal);
            }
            hitPosition = hit.point;
        }
        else
        {
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
        }

        StartCoroutine(ShotEffect(hitPosition));

        magAmmo--;  // 현재 탄창 감소
        // 탄창이 비었따면 상태 변경
        if(magAmmo <=0 )
        {
            state = State.Empty;
        }
    }

    IEnumerator ShotEffect(Vector3 hitPosition)
    {
        muzzleFlashEffect.Play();
        shellEjectEffect.Play();

        gunAudioPlayer.PlayOneShot(gunData.shotClip);

        bulletLineRenderer.SetPosition(0, fireTransform.position);
        bulletLineRenderer.SetPosition(1, hitPosition);

        bulletLineRenderer.enabled = true;
        yield return new WaitForSeconds(0.03f);
        bulletLineRenderer.enabled = false;
    }

    public bool Reload()
    {
        // 재장전 중 이거나 남은 탄알이 없거나 탄창에 이미 탄알이 가득찬 경우 재장전 불가
        if(state == State.Reloading || ammoRemain <= 0 || magAmmo >=gunData.magCapacity)
        {
            return false;
        }
        // 재장전 코루틴 실행
        StartCoroutine(ReloadRoutine());
        return true;       
    }

    IEnumerator ReloadRoutine()
    {
        state = State.Reloading;
        gunAudioPlayer.PlayOneShot(gunData.reloadClip);
        yield return new WaitForSeconds(gunData.reloadTime);

        // 현재 탄창에 채울 탄알 계산(20발 남아있다면 5발만 채우면 되기 때문)
        int ammoToFill = gunData.magCapacity - magAmmo;
        // 탄창에 남은 탄알이 채워야할 탄알보다 적다면
        // 채울 탄알은 남은 탄알만큼으로 변경
        if(ammoRemain < ammoToFill)
        {
            ammoToFill = ammoRemain;
        }

        magAmmo += ammoToFill;
        ammoRemain -= ammoToFill;
        state = State.Ready;
    }
}
