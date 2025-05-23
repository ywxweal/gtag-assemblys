using System;
using UnityEngine;

// Token: 0x0200097B RID: 2427
public class BitPackDebug : MonoBehaviour
{
	// Token: 0x04003F55 RID: 16213
	public bool debugPos;

	// Token: 0x04003F56 RID: 16214
	public Vector3 pos;

	// Token: 0x04003F57 RID: 16215
	public Vector3 min = Vector3.one * -2f;

	// Token: 0x04003F58 RID: 16216
	public Vector3 max = Vector3.one * 2f;

	// Token: 0x04003F59 RID: 16217
	public float rad = 4f;

	// Token: 0x04003F5A RID: 16218
	[Space]
	public bool debug32;

	// Token: 0x04003F5B RID: 16219
	public uint packed;

	// Token: 0x04003F5C RID: 16220
	public Vector3 unpacked;

	// Token: 0x04003F5D RID: 16221
	[Space]
	public bool debug16;

	// Token: 0x04003F5E RID: 16222
	public ushort packed16;

	// Token: 0x04003F5F RID: 16223
	public Vector3 unpacked16;
}
