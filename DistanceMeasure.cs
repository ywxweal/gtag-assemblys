using System;
using UnityEngine;

// Token: 0x02000752 RID: 1874
public class DistanceMeasure : MonoBehaviour
{
	// Token: 0x06002ED5 RID: 11989 RVA: 0x000EAC03 File Offset: 0x000E8E03
	private void Awake()
	{
		if (this.from == null)
		{
			this.from = base.transform;
		}
		if (this.to == null)
		{
			this.to = base.transform;
		}
	}

	// Token: 0x04003554 RID: 13652
	public Transform from;

	// Token: 0x04003555 RID: 13653
	public Transform to;
}
