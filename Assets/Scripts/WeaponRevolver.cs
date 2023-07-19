using System.Collections;
using UnityEngine;

public class WeaponRevolver : WeaponBase
{
	[Header("Fire Effects")]
	[SerializeField]
	private	GameObject			muzzleFlashEffect;	// 총구 이펙트 (On/Off)

	[Header("Spawn Points")]
	[SerializeField]
	private	Transform			bulletSpawnPoint;	// 총알 생성 위치
	
	[Header("Audio Clips")]
	[SerializeField]
	private	AudioClip			audioClipFire;		// 공격 사운드
	[SerializeField]
	private	AudioClip			audioClipReload;    // 장전 사운드

	private	ImpactMemoryPool	impactMemoryPool;	// 공격 효과 생성 후 활성/비활성 관리
	private	Camera				mainCamera;			// 광선 발사

	private SceneLoad sceneLoad;

	private void OnEnable()
	{
		// 총구 이펙트 오브젝트 비활성화
		muzzleFlashEffect.SetActive(false);

		// 무기가 활성화될 때 해당 무기의 탄창 정보를 갱신한다
		onMagazineEvent.Invoke(weaponSetting.currentMagazine);
		// 무기가 활성화될 때 해당 무기의 탄 수 정보를 갱신한다
		onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

		ResetVariables();
	}

	private void Awake()
	{
		base.Setup();

		impactMemoryPool				= GetComponent<ImpactMemoryPool>();
		mainCamera						= Camera.main;

		// 처음 탄창 수는 최대로 설정
		weaponSetting.currentMagazine	= weaponSetting.maxMagazine;
		// 처음 탄 수는 최대로 설정
		weaponSetting.currentAmmo		= weaponSetting.maxAmmo;
		GameObject socketManagerObject = GameObject.Find("SocketManager");
		sceneLoad = socketManagerObject.GetComponent<SceneLoad>();
	}

	public override void StartWeaponAction(int type = 0)
	{
		if ( type == 0 && isAttack == false && isReload == false )
		{
			OnAttack();
		}
	}

	public override void StopWeaponAction(int type = 0)
	{
		isAttack = false;
	}

	public override void StartReload()
	{
		// 현재 재장전 중이거나 탄창 수가 0이면 재장전 불가능
		if ( isReload == true || weaponSetting.currentMagazine <= 0 ) return;

		// 무기 액션 도중에 'R'키를 눌러 재장전을 시도하면 무기 액션 종료 후 재장전
		StopWeaponAction();

		StartCoroutine("OnReload");
	}

	public void OnAttack()
	{
		if ( Time.time - lastAttackTime > weaponSetting.attackRate )
		{
			// 뛰고있을 때는 공격할 수 없다
			if ( animator.MoveSpeed > 0.5f )
			{
				return;
			}

			// 공격주기가 되어야 공격할 수 있도록 하기 위해 현재 시간 저장
			lastAttackTime = Time.time;

			// 탄 수가 없으면 공격 불가능
			if ( weaponSetting.currentAmmo <= 0 )
			{
				return;
			}

			sceneLoad.SendWebSocketMessage("shoot", "0");

			// 공격시 currentAmmo 1 감소, 탄 수 UI 업데이트
			weaponSetting.currentAmmo --;
			onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
			
			// 무기 애니메이션 재생
			animator.Play("Fire", -1, 0);
			// 총구 이펙트 재생
			StartCoroutine("OnMuzzleFlashEffect");
			// 공격 사운드 재생
			PlaySound(audioClipFire);

			// 광선을 발사해 원하는 위치 공격
			TwoStepRaycast();
		}
	}

	private IEnumerator OnMuzzleFlashEffect()
	{
		muzzleFlashEffect.SetActive(true);

		yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f);

		muzzleFlashEffect.SetActive(false);
	}

	private IEnumerator OnReload()
	{
		isReload = true;

		// 재장전 애니메이션, 사운드 재생
		animator.OnReload();
		PlaySound(audioClipReload);

		while ( true )
		{
			// 사운드가 재생중이 아니고, 현재 애니메이션이 Movement이면
			// 재장전 애니메이션(, 사운드) 재생이 종료되었다는 뜻
			if ( audioSource.isPlaying == false && animator.CurrentAnimationIs("Movement") )
			{
				isReload = false;

				// 현재 탄창 수를 1 감소시키고, 바뀐 탄창 정보를 Text UI에 업데이트
				weaponSetting.currentMagazine --;
				onMagazineEvent.Invoke(weaponSetting.currentMagazine);

				// 현재 탄 수를 최대로 설정하고, 바뀐 탄 수 정보를 Text UI에 업데이트
				weaponSetting.currentAmmo = weaponSetting.maxAmmo;
				onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

				yield break;
			}

			yield return null;
		}
	}

	private void TwoStepRaycast()
	{
		Ray			ray;
		RaycastHit	hit;
		Vector3		targetPoint = Vector3.zero;
		
		// 화면의 중앙 좌표 (Aim 기준으로 Raycast 연산)
		ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
		// 공격 사거리(attackDistance) 안에 부딪히는 오브젝트가 있으면 targetPoint는 광선에 부딪힌 위치
		if ( Physics.Raycast(ray, out hit, weaponSetting.attackDistance) )
		{
			targetPoint = hit.point;
		}
		// 공격 사거리 안에 부딪히는 오브젝트가 없으면 targetPoint는 최대 사거리 위치
		else
		{
			targetPoint = ray.origin + ray.direction*weaponSetting.attackDistance;
		}

		// 첫번째 Raycast연산으로 얻어진 targetPoint를 목표지점으로 설정하고,
		// 총구를 시작지점으로 하여 Raycast 연산
		Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
		if ( Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance) )
		{
			impactMemoryPool.SpawnImpact(hit);

			if ( hit.transform.CompareTag("ImpactEnemy") )
			{
				hit.transform.GetComponent<EnemyFSM>().TakeDamage(weaponSetting.damage);
			}
			else if ( hit.transform.CompareTag("InteractionObject") )
			{
				hit.transform.GetComponent<InteractionObject>().TakeDamage(weaponSetting.damage);
			}
		}
	}

	private void ResetVariables()
	{
		isReload = false;
		isAttack = false;
	}
}

