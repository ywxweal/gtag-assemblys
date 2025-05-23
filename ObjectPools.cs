using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020009A7 RID: 2471
public class ObjectPools : MonoBehaviour, IBuildValidation
{
	// Token: 0x170005D9 RID: 1497
	// (get) Token: 0x06003B3E RID: 15166 RVA: 0x0011B28E File Offset: 0x0011948E
	// (set) Token: 0x06003B3F RID: 15167 RVA: 0x0011B296 File Offset: 0x00119496
	public bool initialized { get; private set; }

	// Token: 0x06003B40 RID: 15168 RVA: 0x0011B29F File Offset: 0x0011949F
	protected void Awake()
	{
		ObjectPools.instance = this;
	}

	// Token: 0x06003B41 RID: 15169 RVA: 0x0011B2A7 File Offset: 0x001194A7
	protected void Start()
	{
		this.InitializePools();
	}

	// Token: 0x06003B42 RID: 15170 RVA: 0x0011B2B0 File Offset: 0x001194B0
	public void InitializePools()
	{
		if (this.initialized)
		{
			return;
		}
		this.lookUp = new Dictionary<int, SinglePool>();
		foreach (SinglePool singlePool in this.pools)
		{
			singlePool.Initialize(base.gameObject);
			int num = singlePool.PoolGUID();
			if (this.lookUp.ContainsKey(num))
			{
				using (List<SinglePool>.Enumerator enumerator2 = this.pools.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						SinglePool singlePool2 = enumerator2.Current;
						if (singlePool2.PoolGUID() == num)
						{
							Debug.LogError("Pools contain more then one instance of the same object\n" + string.Format("First object in question is {0} tag: {1}\n", singlePool2.objectToPool, singlePool2.objectToPool.tag) + string.Format("Second object is {0} tag: {1}", singlePool.objectToPool, singlePool.objectToPool.tag));
							break;
						}
					}
					continue;
				}
			}
			this.lookUp.Add(singlePool.PoolGUID(), singlePool);
		}
		this.initialized = true;
	}

	// Token: 0x06003B43 RID: 15171 RVA: 0x0011B3E4 File Offset: 0x001195E4
	public bool DoesPoolExist(GameObject obj)
	{
		return this.DoesPoolExist(PoolUtils.GameObjHashCode(obj));
	}

	// Token: 0x06003B44 RID: 15172 RVA: 0x0011B3F2 File Offset: 0x001195F2
	public bool DoesPoolExist(int hash)
	{
		return this.lookUp.ContainsKey(hash);
	}

	// Token: 0x06003B45 RID: 15173 RVA: 0x0011B400 File Offset: 0x00119600
	public SinglePool GetPoolByHash(int hash)
	{
		return this.lookUp[hash];
	}

	// Token: 0x06003B46 RID: 15174 RVA: 0x0011B410 File Offset: 0x00119610
	public SinglePool GetPoolByObjectType(GameObject obj)
	{
		int num = PoolUtils.GameObjHashCode(obj);
		return this.GetPoolByHash(num);
	}

	// Token: 0x06003B47 RID: 15175 RVA: 0x0011B42B File Offset: 0x0011962B
	public GameObject Instantiate(GameObject obj, bool setActive = true)
	{
		return this.GetPoolByObjectType(obj).Instantiate(setActive);
	}

	// Token: 0x06003B48 RID: 15176 RVA: 0x0011B43A File Offset: 0x0011963A
	public GameObject Instantiate(int hash, bool setActive = true)
	{
		return this.GetPoolByHash(hash).Instantiate(setActive);
	}

	// Token: 0x06003B49 RID: 15177 RVA: 0x0011B449 File Offset: 0x00119649
	public GameObject Instantiate(int hash, Vector3 position, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(hash, setActive);
		gameObject.transform.position = position;
		return gameObject;
	}

	// Token: 0x06003B4A RID: 15178 RVA: 0x0011B45F File Offset: 0x0011965F
	public GameObject Instantiate(int hash, Vector3 position, Quaternion rotation, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(hash, setActive);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		return gameObject;
	}

	// Token: 0x06003B4B RID: 15179 RVA: 0x0011B477 File Offset: 0x00119677
	public GameObject Instantiate(GameObject obj, Vector3 position, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(obj, setActive);
		gameObject.transform.position = position;
		return gameObject;
	}

	// Token: 0x06003B4C RID: 15180 RVA: 0x0011B48D File Offset: 0x0011968D
	public GameObject Instantiate(GameObject obj, Vector3 position, Quaternion rotation, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(obj, setActive);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		return gameObject;
	}

	// Token: 0x06003B4D RID: 15181 RVA: 0x0011B4A5 File Offset: 0x001196A5
	public GameObject Instantiate(GameObject obj, Vector3 position, Quaternion rotation, float scale, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(obj, setActive);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		gameObject.transform.localScale = Vector3.one * scale;
		return gameObject;
	}

	// Token: 0x06003B4E RID: 15182 RVA: 0x0011B4D4 File Offset: 0x001196D4
	public void Destroy(GameObject obj)
	{
		this.GetPoolByObjectType(obj).Destroy(obj);
	}

	// Token: 0x06003B4F RID: 15183 RVA: 0x0011B4E4 File Offset: 0x001196E4
	public bool BuildValidationCheck()
	{
		using (List<SinglePool>.Enumerator enumerator = this.pools.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.objectToPool == null)
				{
					Debug.Log("GlobalObjectPools contains a nullref. Failing build validation.");
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x04003FD5 RID: 16341
	public static ObjectPools instance;

	// Token: 0x04003FD7 RID: 16343
	[SerializeField]
	private List<SinglePool> pools;

	// Token: 0x04003FD8 RID: 16344
	private Dictionary<int, SinglePool> lookUp;
}
