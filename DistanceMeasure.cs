using System;
using UnityEngine;

// Token: 0x02000752 RID: 1874
public class DistanceMeasure : MonoBehaviour
{
	// Token: 0x06002ED6 RID: 11990 RVA: 0x000EACA7 File Offset: 0x000E8EA7
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

	// Token: 0x04003556 RID: 13654
	public Transform from;

	// Token: 0x04003557 RID: 13655
	public Transform to;
}
