﻿using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020009A6 RID: 2470
[Serializable]
public class SinglePool
{
	// Token: 0x06003B35 RID: 15157 RVA: 0x0011B08C File Offset: 0x0011928C
	private void PrivAllocPooledObjects()
	{
		int count = this.inactivePool.Count;
		for (int i = count; i < count + this.initAmountToPool; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.objectToPool, this.gameObject.transform, true);
			gameObject.name = this.objectToPool.name + "(PoolIndex=" + i.ToString() + ")";
			gameObject.SetActive(false);
			this.inactivePool.Push(gameObject);
			int instanceID = gameObject.GetInstanceID();
			this.pooledObjects.Add(instanceID);
		}
	}

	// Token: 0x06003B36 RID: 15158 RVA: 0x0011B11E File Offset: 0x0011931E
	public void Initialize(GameObject gameObject_)
	{
		this.gameObject = gameObject_;
		this.activePool = new Dictionary<int, GameObject>(this.initAmountToPool);
		this.inactivePool = new Stack<GameObject>(this.initAmountToPool);
		this.pooledObjects = new HashSet<int>();
		this.PrivAllocPooledObjects();
	}

	// Token: 0x06003B37 RID: 15159 RVA: 0x0011B15C File Offset: 0x0011935C
	public GameObject Instantiate(bool setActive = true)
	{
		if (this.inactivePool.Count == 0)
		{
			Debug.LogWarning("Pool '" + this.objectToPool.name + "'is expanding consider changing initial pool size");
			this.PrivAllocPooledObjects();
		}
		GameObject gameObject = this.inactivePool.Pop();
		int instanceID = gameObject.GetInstanceID();
		gameObject.SetActive(setActive);
		this.activePool.Add(instanceID, gameObject);
		return gameObject;
	}

	// Token: 0x06003B38 RID: 15160 RVA: 0x0011B1C4 File Offset: 0x001193C4
	public void Destroy(GameObject obj)
	{
		int instanceID = obj.GetInstanceID();
		if (!this.activePool.ContainsKey(instanceID))
		{
			Debug.Log("Failed to destroy Object " + obj.name + " in pool, It is not contained in the activePool");
			return;
		}
		if (!this.pooledObjects.Contains(instanceID))
		{
			Debug.Log("Failed to destroy Object " + obj.name + " in pool, It is not contained in the pooledObjects");
			return;
		}
		obj.SetActive(false);
		this.inactivePool.Push(obj);
		this.activePool.Remove(instanceID);
	}

	// Token: 0x06003B39 RID: 15161 RVA: 0x0011B24A File Offset: 0x0011944A
	public int PoolGUID()
	{
		return PoolUtils.GameObjHashCode(this.objectToPool);
	}

	// Token: 0x06003B3A RID: 15162 RVA: 0x0011B257 File Offset: 0x00119457
	public int GetTotalCount()
	{
		return this.pooledObjects.Count;
	}

	// Token: 0x06003B3B RID: 15163 RVA: 0x0011B264 File Offset: 0x00119464
	public int GetActiveCount()
	{
		return this.activePool.Count;
	}

	// Token: 0x06003B3C RID: 15164 RVA: 0x0011B271 File Offset: 0x00119471
	public int GetInactiveCount()
	{
		return this.inactivePool.Count;
	}

	// Token: 0x04003FCF RID: 16335
	public GameObject objectToPool;

	// Token: 0x04003FD0 RID: 16336
	public int initAmountToPool = 32;

	// Token: 0x04003FD1 RID: 16337
	private HashSet<int> pooledObjects;

	// Token: 0x04003FD2 RID: 16338
	private Stack<GameObject> inactivePool;

	// Token: 0x04003FD3 RID: 16339
	private Dictionary<int, GameObject> activePool;

	// Token: 0x04003FD4 RID: 16340
	private GameObject gameObject;
}
