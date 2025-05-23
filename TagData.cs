using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x0200048B RID: 1163
[NetworkStructWeaved(12)]
[StructLayout(LayoutKind.Explicit, Size = 48)]
public struct TagData : INetworkStruct
{
	// Token: 0x17000318 RID: 792
	// (get) Token: 0x06001C70 RID: 7280 RVA: 0x0008B0F8 File Offset: 0x000892F8
	[Networked]
	[Capacity(10)]
	public NetworkArray<int> infectedPlayerList
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@10>(ref this._infectedPlayerList), 10, ReaderWriter@System_Int32.GetInstance());
		}
	}

	// Token: 0x17000319 RID: 793
	// (get) Token: 0x06001C71 RID: 7281 RVA: 0x0008B11F File Offset: 0x0008931F
	// (set) Token: 0x06001C72 RID: 7282 RVA: 0x0008B127 File Offset: 0x00089327
	public int currentItID { readonly get; set; }

	// Token: 0x04001F89 RID: 8073
	[FieldOffset(4)]
	public NetworkBool isCurrentlyTag;

	// Token: 0x04001F8A RID: 8074
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ReaderWriter@System_Int32), 10, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(8)]
	private FixedStorage@10 _infectedPlayerList;
}
