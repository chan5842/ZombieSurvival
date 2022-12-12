using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum State
    {
        Ready,      // �߻� �غ�
        Empty,      // źâ ��
        Reloading   // ������ ��
    }

    // ���� ���� ����
    public State state { get; private set; }

    // �Ѿ� �߻� ��ġ
    public Transform fireTransform;

    public ParticleSystem muzzleFlashEffect;    // �ѱ� ȭ�� ȿ��
    public ParticleSystem shellEjectEffect;     // ź�� ���� ȿ��

    LineRenderer bulletLineRenderer;    // �Ѿ� ������ ���� ���� ������
    AudioSource gunAudioPlayer;         // �ѼҸ� �����
    public GunData gunData;             // ���� ���� ������

    float fireDistance = 50f;           // ���� �Ÿ�
    public int ammoRemain = 100;        // ���� ��ü �Ѿ�
    public int magAmmo;                 // ���� źâ�� �����ִ� ���Ѿ�

    float lastFireTime;                 // �Ѿ� ������ �߻� �ð�

    void Awake()
    {
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();

        bulletLineRenderer.positionCount = 2;
        bulletLineRenderer.enabled = false;
    }

    private void OnEnable()
    {
        // ��ü źâ �ʱ�ȭ
        ammoRemain = gunData.startAmmoRemain;
        // ���� źâ �ʱ�ȭ
        magAmmo = gunData.magCapacity;
        // �߻� �غ� ���·� ��ȯ
        state = State.Ready;
        // ������ �߻�ð� �ʱ�ȭ
        lastFireTime = 0f;
    }

    public void Fire()
    {
        // �غ�����̰� ������ �߻� �������� �߻簣�ݸ�ŭ�� �ð��� �귶�ٸ�
        if((state == State.Ready) && (Time.time >= lastFireTime + gunData.timeBetFire))
        {
            // ������ �� �߻� ���� ����
            lastFireTime = Time.time;
            // �߻�
            Shot();
        }
    }

    void Shot()
    {
        RaycastHit hit;
        // �Ѿ��� ���� ��ġ
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

        magAmmo--;  // ���� źâ ����
        // źâ�� ������� ���� ����
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
        // ������ �� �̰ų� ���� ź���� ���ų� źâ�� �̹� ź���� ������ ��� ������ �Ұ�
        if(state == State.Reloading || ammoRemain <= 0 || magAmmo >=gunData.magCapacity)
        {
            return false;
        }
        // ������ �ڷ�ƾ ����
        StartCoroutine(ReloadRoutine());
        return true;       
    }

    IEnumerator ReloadRoutine()
    {
        state = State.Reloading;
        gunAudioPlayer.PlayOneShot(gunData.reloadClip);
        yield return new WaitForSeconds(gunData.reloadTime);

        // ���� źâ�� ä�� ź�� ���(20�� �����ִٸ� 5�߸� ä��� �Ǳ� ����)
        int ammoToFill = gunData.magCapacity - magAmmo;
        // źâ�� ���� ź���� ä������ ź�˺��� ���ٸ�
        // ä�� ź���� ���� ź�˸�ŭ���� ����
        if(ammoRemain < ammoToFill)
        {
            ammoToFill = ammoRemain;
        }

        magAmmo += ammoToFill;
        ammoRemain -= ammoToFill;
        state = State.Ready;
    }
}
