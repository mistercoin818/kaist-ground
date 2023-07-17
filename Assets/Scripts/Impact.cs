using UnityEngine;

public class Impact : MonoBehaviour
{
    private ParticleSystem particle;
    private MemoryPool memoryPool;

    private void Awake() {
        particle = GetComponent<ParticleSystem>();
    }

    public void Setup(MemoryPool pool) {
        memoryPool = pool;
    }

    // Update is called once per frame
    void Update()
    {
        // 파티클이 재생중이 아니면 삭제
        if (particle.isPlaying == false) {
            memoryPool.DeactivatePoolItem(gameObject);
        }
    }
}
