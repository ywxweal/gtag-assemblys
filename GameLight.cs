using System;
using UnityEngine;

// Token: 0x02000570 RID: 1392
public class GameLight : MonoBehaviour
{
	// Token: 0x060021FE RID: 8702 RVA: 0x000AA720 File Offset: 0x000A8920
	private void OnEnable()
	{
		if (this.initialized)
		{
			this.lightId = GameLightingManager.instance.AddGameLight(this);
		}
	}

	// Token: 0x060021FF RID: 8703 RVA: 0x000AA73D File Offset: 0x000A893D
	private void Start()
	{
		this.lightId = GameLightingManager.instance.AddGameLight(this);
		this.initialized = true;
	}

	// Token: 0x06002200 RID: 8704 RVA: 0x000AA759 File Offset: 0x000A8959
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
