using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020000CE RID: 206
[NetworkStructWeaved(11)]
[StructLayout(LayoutKind.Explicit, Size = 44)]
public struct GhostLabData : INetworkStruct
{
	// Token: 0x1700005F RID: 95
	// (get) Token: 0x06000508 RID: 1288 RVA: 0x0001CF1D File Offset: 0x0001B11D
	// (set) Token: 0x06000509 RID: 1289 RVA: 0x0001CF25 File Offset: 0x0001B125
	public int DoorState { readonly get; set; }

	// Token: 0x17000060 RID: 96
	// (get) Token: 0x0600050A RID: 1290 RVA: 0x0001CF30 File Offset: 0x0001B130
	[Networked]
	[Capacity(10)]
	public NetworkArray<NetworkBool> OpenDoors
	{
		get
		{
			return new NetworkArray<NetworkBool>(Native.ReferenceToPointer<FixedStorage@10>(ref this._OpenDoors), 10, ReaderWriter@Fusion_NetworkBool.GetInstance());
		}
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x0001CF58 File Offset: 0x0001B158
	public GhostLabData(int state, bool[] openDoors)
	{
		this.DoorState = state;
		for (int i = 0; i < openDoors.Length; i++)
		{
			bool flag = openDoors[i];
			this.OpenDoors.Set(i, flag);
		}
	}

	// Token: 0x040005ED RID: 1517
	[FixedBufferProperty(typeof(NetworkArray<NetworkBool>), typeof(UnityArraySurrogate@ReaderWriter@Fusion_NetworkBool), 10, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@10 _OpenDoors;
}
