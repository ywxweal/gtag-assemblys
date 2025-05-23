using System;
using UnityEngine;

// Token: 0x020001D6 RID: 470
public class GTDisableStaticOnAwake : MonoBehaviour
{
	// Token: 0x06000B05 RID: 2821 RVA: 0x0003AF28 File Offset: 0x00039128
	private void Awake()
	{
		base.gameObject.isStatic = false;
		Object.Destroy(this);
	}
}
