// 무기의 종류가 여러 종류일 때 공용으로 사용하는 변수들은 구조체로 묶어서 정의하면
// 변수가 추가/삭제될 때 구조체에 선언하기 때문에 추가/삭제에 대한 관리가 용이함

public enum WeaponName {AssaultRifle = 0}

[System.Serializable]
public struct WeaponSetting
{
    public WeaponName weaponName; // 무기 이름
    public int damage; // 무기 공격력
    public int currentMagazine; // 현재 탄창 수
    public int maxMagazine; // 최대 탄창 수
    public int currentAmmo; // 현재 탄약 수
    public int maxAmmo; // 최대 탄약 수
    public float attackRate; // 공격 속도
    public float attackDistance; // 공격 사거리
    public bool isAutomaticAttack; // 연속 공격 여부
}
