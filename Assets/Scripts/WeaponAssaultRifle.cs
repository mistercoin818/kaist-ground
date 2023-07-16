using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }
public class WeaponAssaultRifle : MonoBehaviour
{
    [HideInInspector]
    public AmmoEvent onAmmoEvent = new AmmoEvent();
    [Header("Fire Effects")]
    [SerializeField]
    private GameObject muzzleFlashEffect; // 총구 이펙트(On/Off)

    [Header("Spawn Points")]
    [SerializeField]
    private Transform casingSpawnPoint; // 탄피 생성 위치

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipTakeWeapon; // 무기 장착 사운드
    [SerializeField]
    private AudioClip audioClipFire; // 공격 사운드
    [SerializeField]
    private AudioClip audioClipReload; // 재장전 사운드

    [Header("Weapon Setting")]
    [SerializeField]
    private WeaponSetting weaponSetting; // 무기 설정
    private float lastAttackTime = 0; // 마지막 발사시간 체크용
    private bool isReload = false; // 재장전중인지 체크

    private AudioSource audioSource; // 사운드 재생 컴포넌트
    private PlayerAnimatorController animator; // 애니메이션 재생 제어
    private CasingMemoryPool casingMemoryPool; // 탄피 생성 후 활성/비활성 관리

    // 외부에서 필요한 정보를 열람하기 위해
    public WeaponName WeaponName => weaponSetting.weaponName;

    private void Awake(){
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInParent<PlayerAnimatorController>();
        casingMemoryPool = GetComponent<CasingMemoryPool>();

        // 처음 탄 수는 최대로 설정
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;

        // 무기가 활성화될 때 해당 무기의 탄 수 정보를 갱신
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
    }
    // Start is called before the first frame update
    private void OnEnable()
    {
        // 무기 장착 사운드 재생
        PlaySound(audioClipTakeWeapon);
        // 총구 이펙트 오브젝트 비활성화
        muzzleFlashEffect.SetActive(false);
    }

    public void StartWeaponAction(int type=0){
        // 재장전 웅일때 무기 액션 불가
        if (isReload) return;

        // 마우스 왼쪽 클릭(공격 시작)
        if(type == 0){
            //연속 공격
            if(weaponSetting.isAutomaticAttack == true){
                StartCoroutine("OnAttackLoop");
            }
            // 단발 공격
            else{
                OnAttack();
            }
        } 
    }

    public void StopWeaponAction(int type=0){
        // 마우스 왼쪽 클릭 (공격 종료)
        if(type == 0){
            StopCoroutine("OnAttackLoop");
        }
    }

    public void StartReload() {
        // 현재 재장전 중이면 재장전 불가능
        if (isReload) return;
        Debug.Log("startReload");

        // 무기 액션 도중 재장전 시도 -> 무기 액션 종료 후 재장전
        StopWeaponAction();

        StartCoroutine("OnReload");
    }

    public IEnumerator OnAttackLoop(){
        while(true){
            OnAttack();

            yield return null;
        }
    }

    public void OnAttack(){
        if (Time.time - lastAttackTime > weaponSetting.attackRate){
            // 뛰고있을 때는 공격할 수 없다
            if(animator.MoveSpeed > 0.5f){
                return;
            }

            // 공격주기가 되어야 공격할 수 있도록 하기 위해 현재 시간 저장
            lastAttackTime = Time.time;

            // 탄 수가 없으면 공격 불가능
            if (weaponSetting.currentAmmo <= 0) {
                return;
            }
            // 공격시 currentAmmo 1 감소, 탄 수 UI 업데이트
            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            //무기 애니메이션 재생
            animator.Play("Fire", -1, 0);
            //총구 이펙트 재생
            StartCoroutine("OnMuzzleFlashEffect");
            // 공격 사운드 재생
            PlaySound(audioClipFire);
            // 탄피 생성
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);
        }
    }

    private IEnumerator OnMuzzleFlashEffect(){
        muzzleFlashEffect.SetActive(true);

        yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f);

        muzzleFlashEffect.SetActive(false);
    }

    private IEnumerator OnReload() {
        isReload = true;
        Debug.Log("OnReload");

        // 재장전 애니메이션, 사운드 재생
        animator.OnReload();
        PlaySound(audioClipReload);

        while(true) {
            // 사운드 재생중 아니고, 현재 애니메이션이 Movement이면
            // 재장전 애니메이션, 사운드 종료되었다는 뜻
            if (audioSource.isPlaying == false && animator.CurrentAnimationIs("Movement")) {
                isReload = false;

                // 현재 탄 수를 최대로 설정하고, 바뀐 탄 수 정보를 Text UI에 업데이트
                weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                yield break;
            }
            yield return null;
        }
    }
    private void PlaySound(AudioClip clip){
        audioSource.Stop(); // 기존에 재생중인 사운드를 정지하고,
        audioSource.clip = clip; // 새로운 사운드를 clip으로 교체 후
        audioSource.Play(); // 사운드 재생
    }
    // Update is called once per frame
}
