using System;
using System.Runtime.InteropServices;
using Fusion;
using UnityEngine;

// Token: 0x02000287 RID: 647
[NetworkInputWeaved(35)]
[StructLayout(LayoutKind.Explicit, Size = 140)]
public struct NetworkedInput : INetworkInput
{
	// Token: 0x040011E8 RID: 4584
	[FieldOffset(0)]
	public Quaternion headRot_LS;

	// Token: 0x040011E9 RID: 4585
	[FieldOffset(16)]
	public Vector3 rightHandPos_LS;

	// Token: 0x040011EA RID: 4586
	[FieldOffset(28)]
	public Quaternion rightHandRot_LS;

	// Token: 0x040011EB RID: 4587
	[FieldOffset(44)]
	public Vector3 leftHandPos_LS;

	// Token: 0x040011EC RID: 4588
	[FieldOffset(56)]
	public Quaternion leftHandRot_LS;

	// Token: 0x040011ED RID: 4589
	[FieldOffset(72)]
	public Vector3 rootPosition;

	// Token: 0x040011EE RID: 4590
	[FieldOffset(84)]
	public Quaternion rootRotation;

	// Token: 0x040011EF RID: 4591
	[FieldOffset(100)]
	public bool leftThumbTouch;

	// Token: 0x040011F0 RID: 4592
	[FieldOffset(104)]
	public bool leftThumbPress;

	// Token: 0x040011F1 RID: 4593
	[FieldOffset(108)]
	public float leftIndexValue;

	// Token: 0x040011F2 RID: 4594
	[FieldOffset(112)]
	public float leftMiddleValue;

	// Token: 0x040011F3 RID: 4595
	[FieldOffset(116)]
	public bool rightThumbTouch;

	// Token: 0x040011F4 RID: 4596
	[FieldOffset(120)]
	public bool rightThumbPress;

	// Token: 0x040011F5 RID: 4597
	[FieldOffset(124)]
	public float rightIndexValue;

	// Token: 0x040011F6 RID: 4598
	[FieldOffset(128)]
	public float rightMiddleValue;

	// Token: 0x040011F7 RID: 4599
	[FieldOffset(132)]
	public float scale;

	// Token: 0x040011F8 RID: 4600
	[FieldOffset(136)]
	public int handPoseData;
}
