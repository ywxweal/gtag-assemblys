using System;
using UnityEngine;

// Token: 0x0200068C RID: 1676
[Serializable]
public class SizeLayerMask
{
	// Token: 0x1700040D RID: 1037
	// (get) Token: 0x060029EB RID: 10731 RVA: 0x000CF4FC File Offset: 0x000CD6FC
	public int Mask
	{
		get
		{
			int num = 0;
			if (this.affectLayerA)
			{
				num |= 1;
			}
			if (this.affectLayerB)
			{
				num |= 2;
			}
			if (this.affectLayerC)
			{
				num |= 4;
			}
			if (this.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x04002F06 RID: 12038
	[SerializeField]
	private bool affectLayerA = true;

	// Token: 0x04002F07 RID: 12039
	[SerializeField]
	private bool affectLayerB = true;

	// Token: 0x04002F08 RID: 12040
	[SerializeField]
	private bool affectLayerC = true;

	// Token: 0x04002F09 RID: 12041
	[SerializeField]
	private bool affectLayerD = true;
}
