using System;
using UnityEngine;

// Token: 0x0200064D RID: 1613
public class GorillaUIParent : MonoBehaviour
{
	// Token: 0x06002850 RID: 10320 RVA: 0x000C9159 File Offset: 0x000C7359
	private void Awake()
	{
		if (GorillaUIParent.instance == null)
		{
			GorillaUIParent.instance = this;
			return;
		}
		if (GorillaUIParent.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x04002D36 RID: 11574
	[OnEnterPlay_SetNull]
	public static volatile GorillaUIParent instance;
}
