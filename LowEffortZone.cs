using System;
using UnityEngine;

// Token: 0x02000233 RID: 563
public class LowEffortZone : GorillaTriggerBox
{
	// Token: 0x06000D03 RID: 3331 RVA: 0x00044A1C File Offset: 0x00042C1C
	private void Awake()
	{
		if (this.triggerOnAwake)
		{
			this.OnBoxTriggered();
		}
	}

	// Token: 0x06000D04 RID: 3332 RVA: 0x00044A2C File Offset: 0x00042C2C
	public override void OnBoxTriggered()
	{
		for (int i = 0; i < this.objectsToEnable.Length; i++)
		{
			if (this.objectsToEnable[i] != null)
			{
				this.objectsToEnable[i].SetActive(true);
			}
		}
		for (int j = 0; j < this.objectsToDisable.Length; j++)
		{
			if (this.objectsToDisable[j] != null)
			{
				this.objectsToDisable[j].SetActive(false);
			}
		}
	}

	// Token: 0x04001071 RID: 4209
	public GameObject[] objectsToEnable;

	// Token: 0x04001072 RID: 4210
	public GameObject[] objectsToDisable;

	// Token: 0x04001073 RID: 4211
	public bool triggerOnAwake;
}
