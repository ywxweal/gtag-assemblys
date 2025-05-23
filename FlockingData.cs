using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x0200054F RID: 1359
[NetworkStructWeaved(337)]
[StructLayout(LayoutKind.Explicit, Size = 1348)]
public struct FlockingData : INetworkStruct
{
	// Token: 0x1700034E RID: 846
	// (get) Token: 0x060020F3 RID: 8435 RVA: 0x000A59B8 File Offset: 0x000A3BB8
	// (set) Token: 0x060020F4 RID: 8436 RVA: 0x000A59C0 File Offset: 0x000A3BC0
	public int count { readonly get; set; }

	// Token: 0x1700034F RID: 847
	// (get) Token: 0x060020F5 RID: 8437 RVA: 0x000A59CC File Offset: 0x000A3BCC
	[Networked]
	[Capacity(30)]
	public NetworkLinkedList<Vector3> Positions
	{
		get
		{
			return new NetworkLinkedList<Vector3>(Native.ReferenceToPointer<FixedStorage@153>(ref this._Positions), 30, ReaderWriter@UnityEngine_Vector3.GetInstance());
		}
	}

	// Token: 0x17000350 RID: 848
	// (get) Token: 0x060020F6 RID: 8438 RVA: 0x000A59F4 File Offset: 0x000A3BF4
	[Networked]
	[Capacity(30)]
	public NetworkLinkedList<Quaternion> Rotations
	{
		get
		{
			return new NetworkLinkedList<Quaternion>(Native.ReferenceToPointer<FixedStorage@183>(ref this._Rotations), 30, ReaderWriter@UnityEngine_Quaternion.GetInstance());
		}
	}

	// Token: 0x060020F7 RID: 8439 RVA: 0x000A5A1C File Offset: 0x000A3C1C
	public FlockingData(List<Flocking> items)
	{
		this.count = items.Count;
		foreach (Flocking flocking in items)
		{
			this.Positions.Add(flocking.pos);
			this.Rotations.Add(flocking.rot);
		}
	}

	// Token: 0x0400251E RID: 9502
	[FixedBufferProperty(typeof(NetworkLinkedList<Vector3>), typeof(UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Vector3), 30, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@153 _Positions;

	// Token: 0x0400251F RID: 9503
	[FixedBufferProperty(typeof(NetworkLinkedList<Quaternion>), typeof(UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Quaternion), 30, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(616)]
	private FixedStorage@183 _Rotations;
}
