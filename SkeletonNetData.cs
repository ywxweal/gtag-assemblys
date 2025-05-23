using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020000D5 RID: 213
[NetworkStructWeaved(11)]
[StructLayout(LayoutKind.Explicit, Size = 44)]
public struct SkeletonNetData : INetworkStruct
{
	// Token: 0x17000062 RID: 98
	// (get) Token: 0x06000546 RID: 1350 RVA: 0x0001ED65 File Offset: 0x0001CF65
	// (set) Token: 0x06000547 RID: 1351 RVA: 0x0001ED6D File Offset: 0x0001CF6D
	public int CurrentState { readonly get; set; }

	// Token: 0x17000063 RID: 99
	// (get) Token: 0x06000548 RID: 1352 RVA: 0x0001ED76 File Offset: 0x0001CF76
	// (set) Token: 0x06000549 RID: 1353 RVA: 0x0001ED88 File Offset: 0x0001CF88
	[Networked]
	public unsafe Vector3 Position
	{
		readonly get
		{
			return *(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._Position);
		}
		set
		{
			*(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._Position) = value;
		}
	}

	// Token: 0x17000064 RID: 100
	// (get) Token: 0x0600054A RID: 1354 RVA: 0x0001ED9B File Offset: 0x0001CF9B
	// (set) Token: 0x0600054B RID: 1355 RVA: 0x0001EDAD File Offset: 0x0001CFAD
	[Networked]
	public unsafe Quaternion Rotation
	{
		readonly get
		{
			return *(Quaternion*)Native.ReferenceToPointer<FixedStorage@4>(ref this._Rotation);
		}
		set
		{
			*(Quaternion*)Native.ReferenceToPointer<FixedStorage@4>(ref this._Rotation) = value;
		}
	}

	// Token: 0x17000065 RID: 101
	// (get) Token: 0x0600054C RID: 1356 RVA: 0x0001EDC0 File Offset: 0x0001CFC0
	// (set) Token: 0x0600054D RID: 1357 RVA: 0x0001EDC8 File Offset: 0x0001CFC8
	public int CurrentNode { readonly get; set; }

	// Token: 0x17000066 RID: 102
	// (get) Token: 0x0600054E RID: 1358 RVA: 0x0001EDD1 File Offset: 0x0001CFD1
	// (set) Token: 0x0600054F RID: 1359 RVA: 0x0001EDD9 File Offset: 0x0001CFD9
	public int NextNode { readonly get; set; }

	// Token: 0x17000067 RID: 103
	// (get) Token: 0x06000550 RID: 1360 RVA: 0x0001EDE2 File Offset: 0x0001CFE2
	// (set) Token: 0x06000551 RID: 1361 RVA: 0x0001EDEA File Offset: 0x0001CFEA
	public int AngerPoint { readonly get; set; }

	// Token: 0x06000552 RID: 1362 RVA: 0x0001EDF3 File Offset: 0x0001CFF3
	public SkeletonNetData(int state, Vector3 pos, Quaternion rot, int cNode, int nNode, int angerPoint)
	{
		this.CurrentState = state;
		this.Position = pos;
		this.Rotation = rot;
		this.CurrentNode = cNode;
		this.NextNode = nNode;
		this.AngerPoint = angerPoint;
	}

	// Token: 0x0400063C RID: 1596
	[FixedBufferProperty(typeof(Vector3), typeof(UnityValueSurrogate@ReaderWriter@UnityEngine_Vector3), 0, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@3 _Position;

	// Token: 0x0400063D RID: 1597
	[FixedBufferProperty(typeof(Quaternion), typeof(UnityValueSurrogate@ReaderWriter@UnityEngine_Quaternion), 0, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(16)]
	private FixedStorage@4 _Rotation;
}
