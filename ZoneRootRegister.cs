using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000239 RID: 569
public class ZoneRootRegister : MonoBehaviour
{
	// Token: 0x06000D21 RID: 3361 RVA: 0x0004526E File Offset: 0x0004346E
	private void Awake()
	{
		this.watchableSlot.Value = base.gameObject;
	}

	// Token: 0x06000D22 RID: 3362 RVA: 0x00045281 File Offset: 0x00043481
	private void OnDestroy()
	{
		this.watchableSlot.Value = null;
	}

	// Token: 0x040010AF RID: 4271
	[SerializeField]
	private WatchableGameObjectSO watchableSlot;
}
