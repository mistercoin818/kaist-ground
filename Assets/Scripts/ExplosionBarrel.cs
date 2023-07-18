using System.Collections;
using UnityEngine;

public class ExplosionBarrel : InteractionObject
{
    [Header("Explosion Barrel")]
    [SerializeField]
    private GameObject explosionPrefab;
    [SerializeField]
    private float explosionDelayTime = 0.3f;
    [SerializeField]
    private float explosionRadius = 10.0f;
    [SerializeField]
    private float explosionForce = 1000.0f;

    private bool isExplode = false;

    public override void TakeDamage(int damage){
        currentHP -= damage;

        if(currentHP <= 0 && isExplode == false){
            StartCoroutine("ExplodeBarrel");
        }
    }

    private IEnumerator ExplodeBarrel(){
        yield return new WaitForSeconds(explosionDelayTime);

        // 근처의 배럴이 터져서 다시 현재 배럴을 터트리려고 할 때(StackOverflow 방지)
        isExplode = true;

        // 폭발 이펙트 생성
        Bounds bounds = GetComponent<Collider>().bounds;
        Instantiate(explosionPrefab, new Vector3(bounds.center.x, bounds.min.y, bounds.center.z), transform.rotation);

        // 폭발 범위에 있는 모든 오브젝트의 Collider 정보를 받아와 폭발 효과 처리
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider hit in colliders){
            // 폭발 범위에 부딪힌 오브젝트가 플레이어일 때 처리
            PlayerController player = hit.GetComponent<PlayerController>();
            if(player != null){
                player.TakeDamage(50);
                continue;
            }

            // 폭발 범위에 부딪힌 오브젝트가 적 캐릭터일 때 처리
            EnemyFSM enemy = hit.GetComponentInParent<EnemyFSM>();
            if(enemy != null){
                enemy.TakeDamage(300);
                continue;
            }

            // 폭발 범위에 부딪힌 오브젝트가 상호작용 오브젝트이면 TakeDamage()로 피해를 줌
            InteractionObject interaction = hit.GetComponent<InteractionObject>();
            if(interaction != null){
                interaction.TakeDamage(300);
            }

            // 중력을 가지고 있는 오브젝트이면 힘을 받아 밀려나도록
            Rigidbody rigidbody = hit.GetComponent<Rigidbody>();
            if(rigidbody != null){
                rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // 배럴 오브젝트 삭제
        Destroy(gameObject);
    }
}
