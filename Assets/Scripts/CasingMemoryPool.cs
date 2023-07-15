using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasingMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject casingPrefabs;   // 탄피 오브젝트
    private MemoryPool memoryPool;      // 탄피 메모리풀

    private void Awake() {
        memoryPool = new MemoryPool(casingPrefabs);
    }

    public void SpawnCasing(Vector3 position, Vector3 direction) {
        GameObject item = memoryPool.ActivatePoolItem();
        item.transform.position = position;
        item.transform.rotation = Random.rotation;
        item.GetComponent<Casing>().Setup(memoryPool, direction);
    }
}