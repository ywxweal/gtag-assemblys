using System;
using UnityEngine;

// Token: 0x0200064D RID: 1613
public class GorillaUIParent : MonoBehaviour
{
	// Token: 0x0600284F RID: 10319 RVA: 0x000C90B5 File Offset: 0x000C72B5
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

	// Token: 0x04002D34 RID: 11572
	[OnEnterPlay_SetNull]
	public static volatile GorillaUIParent instance;
}
