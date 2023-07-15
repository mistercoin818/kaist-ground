using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementCharacterController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed; // 이동속도
    private Vector3 moveForce; // 이동 힘 (x, z와 y축을 별도로 계산해 실제 이동에 적용)

    [SerializeField]
    private float jumpForce; // 점프 힘
    [SerializeField]
    private float gravity; // 중력 계수

    public float MoveSpeed{
        set => moveSpeed = Mathf.Max(0, value);
        get => moveSpeed;
    }
    private CharacterController characterController; // 플레이어 이동 제어를 위한 컴포넌트

    private void Awake(){
        characterController = GetComponent<CharacterController>();
    }
    // Update is called once per frame
    private void Update()
    {
        // 허공에 떠있으면 중력만큼 y축 이동속도 감소
        if( !characterController.isGrounded){
            moveForce.y += gravity * Time.deltaTime;
        }
        // 1초당 moveForce 속력으로 이동
        characterController.Move(moveForce * Time.deltaTime);
    }

    public void MoveTo(Vector3 direction){
        // 이동 방향 = 캐릭터의 회전 값 * 방향 값
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);

        // 이동 힘 = 이동방향 * 속도
        moveForce = new Vector3(direction.x * moveSpeed, moveForce.y, direction.z * moveSpeed);
    }

    public void Jump(){
        // 플레이어가 바닥에 있을 때만 점프 가능
        if(characterController.isGrounded){
            moveForce.y = jumpForce;
        }
    }
}
