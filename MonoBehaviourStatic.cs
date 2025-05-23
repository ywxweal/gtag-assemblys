using System;
using UnityEngine;

// Token: 0x02000772 RID: 1906
public class MonoBehaviourStatic<T> : MonoBehaviour where T : MonoBehaviour
{
	// Token: 0x170004B9 RID: 1209
	// (get) Token: 0x06002F83 RID: 12163 RVA: 0x000ECC09 File Offset: 0x000EAE09
	public static T Instance
	{
		get
		{
			return MonoBehaviourStatic<T>.gInstance;
		}
	}

	// Token: 0x06002F84 RID: 12164 RVA: 0x000ECC10 File Offset: 0x000EAE10
	protected void Awake()
	{
		if (MonoBehaviourStatic<T>.gInstance && MonoBehaviourStatic<T>.gInstance != this)
		{
			Object.Destroy(this);
		}
		MonoBehaviourStatic<T>.gInstance = this as T;
		this.OnAwake();
	}

	// Token: 0x06002F85 RID: 12165 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnAwake()
	{
	}

	// Token: 0x04003611 RID: 13841
	protected static T gInstance;
}
