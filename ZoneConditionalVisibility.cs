using System;
using UnityEngine;

// Token: 0x02000242 RID: 578
public class ZoneConditionalVisibility : MonoBehaviour
{
	// Token: 0x06000D4C RID: 3404 RVA: 0x00045AC7 File Offset: 0x00043CC7
	private void Start()
	{
		this.OnZoneChanged();
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06000D4D RID: 3405 RVA: 0x00045AF5 File Offset: 0x00043CF5
	private void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06000D4E RID: 3406 RVA: 0x00045B1D File Offset: 0x00043D1D
	private void OnZoneChanged()
	{
		if (this.invisibleWhileLoaded)
		{
			base.gameObject.SetActive(!ZoneManagement.IsInZone(this.zone));
			return;
		}
		base.gameObject.SetActive(ZoneManagement.IsInZone(this.zone));
	}

	// Token: 0x040010D7 RID: 4311
	[SerializeField]
	private GTZone zone;

	// Token: 0x040010D8 RID: 4312
	[SerializeField]
	private bool invisibleWhileLoaded;
}
