using UnityEngine;

public class WeaponSwitchSystem : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private PlayerHUD playerHUD;

    [SerializeField]
    private WeaponBase[] weapons; // 소지중인 무기 4종류

    private WeaponBase currentWeapon; // 현재 사용중인 무기
    private WeaponBase previousWeapon; // 직전에 사용했던 무기

    private void Awake(){
        // 무기 정보 출력을 위해 소지중인 모든 무기 이벤트 등록
        playerHUD.SetupAllWeapons(weapons);

        // 현재 소지중인 모든 무기를 보이지 않게 설정
        for (int i = 0; i < weapons.Length; ++i){
            if(weapons[i].gameObject != null){
                weapons[i].gameObject.SetActive(false);
            }
        }

        // Main 무기를 현재 사용 무기로 설정
        SwitchingWeapon(WeaponType.Main);
    }

    private void Update(){
        UpdateSwitch();
    }

    private void UpdateSwitch(){
        if (!Input.anyKeyDown) return;

        // 1~4 숫자키를 누르면 무기 교체
        int inputIndex = 0;
        if(int.TryParse(Input.inputString, out inputIndex) && (inputIndex > 0 && inputIndex < 5)){
            SwitchingWeapon((WeaponType)(inputIndex - 1));
        }
    }

    private void SwitchingWeapon(WeaponType weaponType){
        // 교체 가능한 무기가 없으면 종료
        if(weapons[(int)weaponType] == null){
            return;
        }

        // 현재 사용중인 무기가 있으면 이전 무기 정보에 저장
        if(currentWeapon != null){
            previousWeapon = currentWeapon;
        }

        // 무기 교체
        currentWeapon = weapons[(int)weaponType];

        // 현재 사용중인 무기로 교체하려고 할 때 종료
        if(currentWeapon == previousWeapon){
            return;
        }

        // 무기를 사용하는 PlayerController, PlyaerHUD에 현재 무기 정보 전달
        playerController.SwitchingWeapon(currentWeapon);
        playerHUD.SwitchingWeapon(currentWeapon);

        // 이전에 사용하던 무기 비활성화
        if(previousWeapon != null){
            previousWeapon.gameObject.SetActive(false);
        }
        // 현재 사용하는 무기 활성화
        currentWeapon.gameObject.SetActive(true);
    }
}
