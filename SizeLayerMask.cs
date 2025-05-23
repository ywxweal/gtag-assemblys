using System;
using UnityEngine;

// Token: 0x0200068C RID: 1676
[Serializable]
public class SizeLayerMask
{
	// Token: 0x1700040D RID: 1037
	// (get) Token: 0x060029EC RID: 10732 RVA: 0x000CF5A0 File Offset: 0x000CD7A0
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

	// Token: 0x04002F08 RID: 12040
	[SerializeField]
	private bool affectLayerA = true;

	// Token: 0x04002F09 RID: 12041
	[SerializeField]
	private bool affectLayerB = true;

	// Token: 0x04002F0A RID: 12042
	[SerializeField]
	private bool affectLayerC = true;

	// Token: 0x04002F0B RID: 12043
	[SerializeField]
	private bool affectLayerD = true;
}
