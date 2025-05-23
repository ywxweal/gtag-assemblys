using System;
using UnityEngine;

// Token: 0x0200047A RID: 1146
public class DestroyIfNotBeta : MonoBehaviour
{
	// Token: 0x06001C24 RID: 7204 RVA: 0x0003A34F File Offset: 0x0003854F
	private void Awake()
	{
		Object.Destroy(base.gameObject);
	}
}
