using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCodes")]
    [SerializeField]
    private	KeyCode		keyCodeRun = KeyCode.LeftShift;	// 달리기 키
    [SerializeField]
    private	KeyCode		keyCodeJump = KeyCode.Space;	// 점프 키
    [SerializeField]
    private	KeyCode		keyCodeReload = KeyCode.R;		// 탄 재장전 키

    [Header("Audio Clips")]
    [SerializeField]
    private	AudioClip	audioClipWalk;					// 걷기 사운드
    [SerializeField]
    private	AudioClip	audioClipRun;					// 달리기 사운드

    private	RotateToMouse				rotateToMouse;	// 마우스 이동으로 카메라 회전
    private	MovementCharacterController	movement;		// 키보드 입력으로 플레이어 이동, 점프
    private	Status						status;			// 이동속도 등의 플레이어 정보
    private	AudioSource					audioSource;	// 사운드 재생 제어
    private	WeaponBase					weapon;			// 모든 무기가 상속받는 기반 클래스

    private SceneLoad sceneLoad;

    private float lastSendTime;
    private float sendRate = 0.5f; // 500ms마다 위치 전송

    private void Awake()
    {
        // 마우스 커서를 보이지 않게 설정하고, 현재 위치에 고정시킨다
        Cursor.visible		= false;
        Cursor.lockState	= CursorLockMode.Locked;

        rotateToMouse		= GetComponent<RotateToMouse>();
        movement			= GetComponent<MovementCharacterController>();
        status				= GetComponent<Status>();
        audioSource			= GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(Time.time - lastSendTime > sendRate){
            lastSendTime = Time.time;
            sceneLoad.SendWebSocketMessage("position", JsonUtility.ToJson(new Position(transform.position, transform.rotation)));
        }
        UpdateRotate();
        UpdateMove();
        UpdateJump();
        UpdateWeaponAction();
    }

    private void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotateToMouse.UpdateRotate(mouseX, mouseY);
    }

    private void UpdateMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // 이동중 일 때 (걷기 or 뛰기)
        if ( x != 0 || z != 0 )
        {
            bool isRun = false;

            // 옆이나 뒤로 이동할 때는 달릴 수 없다
            if ( z > 0 ) isRun = Input.GetKey(keyCodeRun);

            movement.MoveSpeed			= isRun == true ? status.RunSpeed : status.WalkSpeed;
            weapon.Animator.MoveSpeed	= isRun == true ? 1 : 0.5f;
            audioSource.clip			= isRun == true ? audioClipRun : audioClipWalk;

            // 방향키 입력 여부는 매 프레임 확인하기 때문에
            // 재생중일 때는 다시 재생하지 않도록 isPlaying으로 체크해서 재생
            if ( audioSource.isPlaying == false )
            {
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        // 제자리에 멈춰있을 때
        else
        {
            movement.MoveSpeed = 0;
            weapon.Animator.MoveSpeed = 0;

            // 멈췄을 때 사운드가 재생중이면 정지
            if ( audioSource.isPlaying == true )
            {
                audioSource.Stop();
            }
        }

        movement.MoveTo(new Vector3(x, 0, z));
    }

    private void UpdateJump()
    {
        if ( Input.GetKeyDown(keyCodeJump) )
        {
            movement.Jump();
        }
    }

    private void UpdateWeaponAction()
    {
        if ( Input.GetMouseButtonDown(0) )
        {
            weapon.StartWeaponAction();
        }
        else if ( Input.GetMouseButtonUp(0) )
        {
            weapon.StopWeaponAction();
        }

        if ( Input.GetMouseButtonDown(1) )
        {
            weapon.StartWeaponAction(1);
        }
        else if ( Input.GetMouseButtonUp(1) )
        {
            weapon.StopWeaponAction(1);
        }

        if ( Input.GetKeyDown(keyCodeReload) )
        {
            weapon.StartReload();
        }
    }

    private void Start() {
        // transform.position += new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f));
        GameObject socketManagerObject = GameObject.Find("SocketManager");
        if (socketManagerObject != null)
        {
            sceneLoad = socketManagerObject.GetComponent<SceneLoad>();

            if (sceneLoad != null)
            {
                sceneLoad.SendWebSocketMessage("position", JsonUtility.ToJson(new Position(transform.position, transform.rotation)));
            }
            else {
                Debug.Log("sceneLoad is null");

            }
        }
    }

    public void TakeDamage(int damage)
    {
        bool isDie = status.DecreaseHP(damage);

        if ( isDie == true )
        {
            SceneManager.LoadScene("GameOver"); // GameOver 씬으로 전환
            Debug.Log("GameOver");
        }
    }

    public void SwitchingWeapon(WeaponBase newWeapon)
    {
        weapon = newWeapon;
    }
}

