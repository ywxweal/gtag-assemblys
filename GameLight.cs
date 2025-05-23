using System;
using UnityEngine;

// Token: 0x02000570 RID: 1392
public class GameLight : MonoBehaviour
{
	// Token: 0x060021FE RID: 8702 RVA: 0x000AA740 File Offset: 0x000A8940
	private void OnEnable()
	{
		if (this.initialized)
		{
			this.lightId = GameLightingManager.instance.AddGameLight(this);
		}
	}

	// Token: 0x060021FF RID: 8703 RVA: 0x000AA75D File Offset: 0x000A895D
	private void Start()
	{
		this.lightId = GameLightingManager.instance.AddGameLight(this);
		this.initialized = true;
	}

	// Token: 0x06002200 RID: 8704 RVA: 0x000AA779 File Offset: 0x000A8979
	private void OnDisable()
	{
		GameLightingManager.instance.RemoveGameLight(this);
	}

	// Token: 0x0400260D RID: 9741
	public Light light;

	// Token: 0x0400260E RID: 9742
	public int lightId;

	// Token: 0x0400260F RID: 9743
	private bool initialized;
}
