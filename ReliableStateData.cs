using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020003DE RID: 990
[NetworkStructWeaved(21)]
[Serializable]
[StructLayout(LayoutKind.Explicit, Size = 84)]
public struct ReliableStateData : INetworkStruct
{
	// Token: 0x170002A3 RID: 675
	// (get) Token: 0x060017D4 RID: 6100 RVA: 0x00074263 File Offset: 0x00072463
	// (set) Token: 0x060017D5 RID: 6101 RVA: 0x0007426B File Offset: 0x0007246B
	public long Header { readonly get; set; }

	// Token: 0x170002A4 RID: 676
	// (get) Token: 0x060017D6 RID: 6102 RVA: 0x00074274 File Offset: 0x00072474
	[Networked]
	[Capacity(5)]
	public NetworkArray<long> TransferrableStates
	{
		get
		{
			return new NetworkArray<long>(Native.ReferenceToPointer<FixedStorage@10>(ref this._TransferrableStates), 5, ReaderWriter@System_Int64.GetInstance());
		}
	}

	// Token: 0x170002A5 RID: 677
	// (get) Token: 0x060017D7 RID: 6103 RVA: 0x00074297 File Offset: 0x00072497
	// (set) Token: 0x060017D8 RID: 6104 RVA: 0x0007429F File Offset: 0x0007249F
	public int WearablesPackedState { readonly get; set; }

	// Token: 0x170002A6 RID: 678
	// (get) Token: 0x060017D9 RID: 6105 RVA: 0x000742A8 File Offset: 0x000724A8
	// (set) Token: 0x060017DA RID: 6106 RVA: 0x000742B0 File Offset: 0x000724B0
	public int LThrowableProjectileIndex { readonly get; set; }

	// Token: 0x170002A7 RID: 679
	// (get) Token: 0x060017DB RID: 6107 RVA: 0x000742B9 File Offset: 0x000724B9
	// (set) Token: 0x060017DC RID: 6108 RVA: 0x000742C1 File Offset: 0x000724C1
	public int RThrowableProjectileIndex { readonly get; set; }

	// Token: 0x170002A8 RID: 680
	// (get) Token: 0x060017DD RID: 6109 RVA: 0x000742CA File Offset: 0x000724CA
	// (set) Token: 0x060017DE RID: 6110 RVA: 0x000742D2 File Offset: 0x000724D2
	public int SizeLayerMask { readonly get; set; }

	// Token: 0x170002A9 RID: 681
	// (get) Token: 0x060017DF RID: 6111 RVA: 0x000742DB File Offset: 0x000724DB
	// (set) Token: 0x060017E0 RID: 6112 RVA: 0x000742E3 File Offset: 0x000724E3
	public int RandomThrowableIndex { readonly get; set; }

	// Token: 0x170002AA RID: 682
	// (get) Token: 0x060017E1 RID: 6113 RVA: 0x000742EC File Offset: 0x000724EC
	// (set) Token: 0x060017E2 RID: 6114 RVA: 0x000742F4 File Offset: 0x000724F4
	public long PackedBeads { readonly get; set; }

	// Token: 0x170002AB RID: 683
	// (get) Token: 0x060017E3 RID: 6115 RVA: 0x000742FD File Offset: 0x000724FD
	// (set) Token: 0x060017E4 RID: 6116 RVA: 0x00074305 File Offset: 0x00072505
	public long PackedBeadsMoreThan6 { readonly get; set; }

	// Token: 0x04001AB5 RID: 6837
	[FixedBufferProperty(typeof(NetworkArray<long>), typeof(UnityArraySurrogate@ReaderWriter@System_Int64), 5, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(44)]
	private FixedStorage@10 _TransferrableStates;
}
