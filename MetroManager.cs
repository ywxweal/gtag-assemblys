using System;
using UnityEngine;

// Token: 0x020000C1 RID: 193
public class MetroManager : MonoBehaviour
{
	// Token: 0x060004DB RID: 1243 RVA: 0x0001C41C File Offset: 0x0001A61C
	private void Update()
	{
		for (int i = 0; i < this._blimps.Length; i++)
		{
			this._blimps[i].Tick();
		}
		for (int j = 0; j < this._spotlights.Length; j++)
		{
			this._spotlights[j].Tick();
		}
	}

	// Token: 0x040005B2 RID: 1458
	[SerializeField]
	private MetroBlimp[] _blimps = new MetroBlimp[0];

	// Token: 0x040005B3 RID: 1459
	[SerializeField]
	private MetroSpotlight[] _spotlights = new MetroSpotlight[0];

	// Token: 0x040005B4 RID: 1460
	[Space]
	[SerializeField]
	private Transform _blimpsRotationAnchor;
}
