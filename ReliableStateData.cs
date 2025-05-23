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
	// (get) Token: 0x060017D4 RID: 6100 RVA: 0x00074243 File Offset: 0x00072443
	// (set) Token: 0x060017D5 RID: 6101 RVA: 0x0007424B File Offset: 0x0007244B
	public long Header { readonly get; set; }

	// Token: 0x170002A4 RID: 676
	// (get) Token: 0x060017D6 RID: 6102 RVA: 0x00074254 File Offset: 0x00072454
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
	// (get) Token: 0x060017D7 RID: 6103 RVA: 0x00074277 File Offset: 0x00072477
	// (set) Token: 0x060017D8 RID: 6104 RVA: 0x0007427F File Offset: 0x0007247F
	public int WearablesPackedState { readonly get; set; }

	// Token: 0x170002A6 RID: 678
	// (get) Token: 0x060017D9 RID: 6105 RVA: 0x00074288 File Offset: 0x00072488
	// (set) Token: 0x060017DA RID: 6106 RVA: 0x00074290 File Offset: 0x00072490
	public int LThrowableProjectileIndex { readonly get; set; }

	// Token: 0x170002A7 RID: 679
	// (get) Token: 0x060017DB RID: 6107 RVA: 0x00074299 File Offset: 0x00072499
	// (set) Token: 0x060017DC RID: 6108 RVA: 0x000742A1 File Offset: 0x000724A1
	public int RThrowableProjectileIndex { readonly get; set; }

	// Token: 0x170002A8 RID: 680
	// (get) Token: 0x060017DD RID: 6109 RVA: 0x000742AA File Offset: 0x000724AA
	// (set) Token: 0x060017DE RID: 6110 RVA: 0x000742B2 File Offset: 0x000724B2
	public int SizeLayerMask { readonly get; set; }

	// Token: 0x170002A9 RID: 681
	// (get) Token: 0x060017DF RID: 6111 RVA: 0x000742BB File Offset: 0x000724BB
	// (set) Token: 0x060017E0 RID: 6112 RVA: 0x000742C3 File Offset: 0x000724C3
	public int RandomThrowableIndex { readonly get; set; }

	// Token: 0x170002AA RID: 682
	// (get) Token: 0x060017E1 RID: 6113 RVA: 0x000742CC File Offset: 0x000724CC
	// (set) Token: 0x060017E2 RID: 6114 RVA: 0x000742D4 File Offset: 0x000724D4
	public long PackedBeads { readonly get; set; }

	// Token: 0x170002AB RID: 683
	// (get) Token: 0x060017E3 RID: 6115 RVA: 0x000742DD File Offset: 0x000724DD
	// (set) Token: 0x060017E4 RID: 6116 RVA: 0x000742E5 File Offset: 0x000724E5
	public long PackedBeadsMoreThan6 { readonly get; set; }

	// Token: 0x04001AB5 RID: 6837
	[FixedBufferProperty(typeof(NetworkArray<long>), typeof(UnityArraySurrogate@ReaderWriter@System_Int64), 5, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(44)]
	private FixedStorage@10 _TransferrableStates;
}
