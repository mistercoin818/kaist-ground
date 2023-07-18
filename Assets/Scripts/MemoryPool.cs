using UnityEngine;
using System.Collections.Generic;

public class MemoryPool
{
	// 메모리 풀로 관리되는 오브젝트 정보
	private class PoolItem
	{
		public	bool		isActive;		// "gameObject"의 활성화/비활성화 정보
		public	GameObject	gameObject;		// 화면에 보이는 실제 게임오브젝트
	}
	
	private	int	increaseCount = 5;			// 오브젝트가 부족할 때 Instantiate()로 추가 생성되는 오브젝트 개수
	private	int	maxCount;					// 현재 리스트에 등록되어 있는 오브젝트 개수
	private	int	activeCount;				// 현재 게임에 사용되고 있는(활성화) 오브젝트 개수

	private	GameObject		poolObject;		// 오브젝트 풀링에서 관리하는 게임 오브젝트 프리팹
	private	List<PoolItem>	poolItemList;	// 관리되는 모든 오브젝트를 저장하는 리스트

	public	int	MaxCount	=> maxCount;	// 외부에서 현재 리스트에 등록되어 있는 오브젝트 개수 확인을 위한 프로퍼티
	public	int	ActiveCount	=> activeCount;	// 외부에서 현재 활성화 되어 있는 오브젝트 개수 확인을 위한 프로퍼티
	
	// 오브젝트가 임시로 보관되는 위치
	private	Vector3	tempPosition = new Vector3(48, 1, 48);

	public MemoryPool(GameObject poolObject)
	{
		maxCount		= 0;
		activeCount		= 0;
		this.poolObject	= poolObject;

		poolItemList	= new List<PoolItem>();
		
		InstantiateObjects();
	}

	/// <summary>
	/// increaseCount 단위로 오브젝트를 생성
	/// </summary>
	public void InstantiateObjects()
	{
		maxCount += increaseCount;

		for ( int i = 0; i < increaseCount; ++ i )
		{
			PoolItem poolItem = new PoolItem();

			poolItem.isActive = false;
			poolItem.gameObject	= GameObject.Instantiate(poolObject);
			poolItem.gameObject.transform.position = tempPosition;
			poolItem.gameObject.SetActive(false);

			poolItemList.Add(poolItem);
		}
	}

	/// <summary>
	/// 현재 관리중인(활성/비활성) 모든 오브젝트를 삭제
	/// </summary>
	public void DestroyObjects()
	{
		if ( poolItemList == null ) return;

		int count = poolItemList.Count;
		for ( int i = 0; i < count; ++ i )
		{
			GameObject.Destroy(poolItemList[i].gameObject);
		}

		poolItemList.Clear();
	}

	/// <summary>
	/// poolItemList에 저장되어 있는 오브젝트를 활성화해서 사용
	/// 현재 모든 오브젝트가 사용중이면 InstantiateObjects()로 추가 생성
	/// </summary>
	public GameObject ActivatePoolItem()
	{
		if ( poolItemList == null ) return null;

		// 현재 생성해서 관리하는 모든 오브젝트 개수와 현재 활성화 상태인 오브젝트 개수 비교
		// 모든 오브젝트가 활성화 상태이면 새로운 오브젝트 필요
		if ( maxCount == activeCount )
		{
			InstantiateObjects();
		}

		int count = poolItemList.Count;
		for ( int i = 0; i < count; ++ i )
		{
			PoolItem poolItem = poolItemList[i];

			if ( poolItem.isActive == false )
			{
				activeCount ++;
				
				poolItem.isActive = true;
				poolItem.gameObject.SetActive(true);
				
				return poolItem.gameObject;
			}
		}

		return null;
	}

	/// <summary>
	/// 현재 사용이 완료된 오브젝트를 비활성화 상태로 설정
	/// </summary>
	public void DeactivatePoolItem(GameObject removeObject)
	{
		if ( poolItemList == null || removeObject == null ) return;

		int count = poolItemList.Count;
		for ( int i = 0; i < count; ++ i )
		{
			PoolItem poolItem = poolItemList[i];

			if ( poolItem.gameObject == removeObject )
			{
				activeCount --;

				poolItem.gameObject.transform.position = tempPosition;
				poolItem.isActive = false;
				poolItem.gameObject.SetActive(false);

				return;
			}
		}
	}

	/// <summary>
	/// 게임에 사용중인 모든 오브젝트를 비활성화 상태로 설정
	/// </summary>
	public void DeactivateAllPoolItems()
	{
		if ( poolItemList == null ) return;

		int count = poolItemList.Count;
		for ( int i = 0; i < count; ++ i )
		{
			PoolItem poolItem = poolItemList[i];

			if ( poolItem.gameObject != null && poolItem.isActive == true )
			{
				poolItem.gameObject.transform.position = tempPosition;
				poolItem.isActive = false;
				poolItem.gameObject.SetActive(false);
			}
		}

		activeCount = 0;
	}
}

