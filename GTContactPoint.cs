using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020001CE RID: 462
[Serializable]
[StructLayout(LayoutKind.Explicit)]
public class GTContactPoint
{
	// Token: 0x04000D45 RID: 3397
	[NonSerialized]
	[FieldOffset(0)]
	public Matrix4x4 data;

	// Token: 0x04000D46 RID: 3398
	[NonSerialized]
	[FieldOffset(0)]
	public Vector4 data0;

	// Token: 0x04000D47 RID: 3399
	[NonSerialized]
	[FieldOffset(16)]
	public Vector4 data1;

	// Token: 0x04000D48 RID: 3400
	[NonSerialized]
	[FieldOffset(32)]
	public Vector4 data2;

	// Token: 0x04000D49 RID: 3401
	[NonSerialized]
	[FieldOffset(48)]
	public Vector4 data3;

	// Token: 0x04000D4A RID: 3402
	[FieldOffset(0)]
	public Vector3 contactPoint;

	// Token: 0x04000D4B RID: 3403
	[FieldOffset(12)]
	public float radius;

	// Token: 0x04000D4C RID: 3404
	[FieldOffset(16)]
	public Vector3 counterVelocity;

	// Token: 0x04000D4D RID: 3405
	[FieldOffset(28)]
	public float timestamp;

	// Token: 0x04000D4E RID: 3406
	[FieldOffset(32)]
	public Color color;

	// Token: 0x04000D4F RID: 3407
	[FieldOffset(48)]
	public GTContactType contactType;

	// Token: 0x04000D50 RID: 3408
	[FieldOffset(52)]
	public float lifetime = 1f;

	// Token: 0x04000D51 RID: 3409
	[FieldOffset(56)]
	public uint free = 1U;
}
