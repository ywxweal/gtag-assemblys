using System;
using UnityEngine;

// Token: 0x02000241 RID: 577
public class ZoneConditionalGameObjectEnabling : MonoBehaviour
{
	// Token: 0x06000D48 RID: 3400 RVA: 0x000459F2 File Offset: 0x00043BF2
	private void Start()
	{
		this.OnZoneChanged();
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06000D49 RID: 3401 RVA: 0x00045A20 File Offset: 0x00043C20
	private void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06000D4A RID: 3402 RVA: 0x00045A48 File Offset: 0x00043C48
	private void OnZoneChanged()
	{
		if (this.invisibleWhileLoaded)
		{
			if (this.gameObjects != null)
			{
				for (int i = 0; i < this.gameObjects.Length; i++)
				{
					this.gameObjects[i].SetActive(!ZoneManagement.IsInZone(this.zone));
				}
				return;
			}
		}
		else if (this.gameObjects != null)
		{
			for (int j = 0; j < this.gameObjects.Length; j++)
			{
				this.gameObjects[j].SetActive(ZoneManagement.IsInZone(this.zone));
			}
		}
	}

	// Token: 0x040010D4 RID: 4308
	[SerializeField]
	private GTZone zone;

	// Token: 0x040010D5 RID: 4309
	[SerializeField]
	private bool invisibleWhileLoaded;

	// Token: 0x040010D6 RID: 4310
	[SerializeField]
	private GameObject[] gameObjects;
}
