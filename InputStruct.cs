using System;
using System.Runtime.InteropServices;
using Fusion;
using UnityEngine;

// Token: 0x0200029E RID: 670
[NetworkStructWeaved(27)]
[Serializable]
[StructLayout(LayoutKind.Explicit, Size = 108)]
public struct InputStruct : INetworkStruct
{
	// Token: 0x04001283 RID: 4739
	[FieldOffset(0)]
	public int headRotation;

	// Token: 0x04001284 RID: 4740
	[FieldOffset(4)]
	public long rightHandLong;

	// Token: 0x04001285 RID: 4741
	[FieldOffset(12)]
	public long leftHandLong;

	// Token: 0x04001286 RID: 4742
	[FieldOffset(20)]
	public long position;

	// Token: 0x04001287 RID: 4743
	[FieldOffset(28)]
	public int handPosition;

	// Token: 0x04001288 RID: 4744
	[FieldOffset(32)]
	public int packedFields;

	// Token: 0x04001289 RID: 4745
	[FieldOffset(36)]
	public short packedCompetitiveData;

	// Token: 0x0400128A RID: 4746
	[FieldOffset(40)]
	public Vector3 velocity;

	// Token: 0x0400128B RID: 4747
	[FieldOffset(52)]
	public int grabbedRopeIndex;

	// Token: 0x0400128C RID: 4748
	[FieldOffset(56)]
	public int ropeBoneIndex;

	// Token: 0x0400128D RID: 4749
	[FieldOffset(60)]
	public bool ropeGrabIsLeft;

	// Token: 0x0400128E RID: 4750
	[FieldOffset(64)]
	public bool ropeGrabIsBody;

	// Token: 0x0400128F RID: 4751
	[FieldOffset(68)]
	public Vector3 ropeGrabOffset;

	// Token: 0x04001290 RID: 4752
	[FieldOffset(80)]
	public bool movingSurfaceIsMonkeBlock;

	// Token: 0x04001291 RID: 4753
	[FieldOffset(84)]
	public long hoverboardPosRot;

	// Token: 0x04001292 RID: 4754
	[FieldOffset(92)]
	public short hoverboardColor;

	// Token: 0x04001293 RID: 4755
	[FieldOffset(96)]
	public double serverTimeStamp;

	// Token: 0x04001294 RID: 4756
	[FieldOffset(104)]
	public short taggedById;
}
