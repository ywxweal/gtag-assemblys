using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x02000482 RID: 1154
[NetworkStructWeaved(31)]
[StructLayout(LayoutKind.Explicit, Size = 124)]
public struct PaintbrawlData : INetworkStruct
{
	// Token: 0x1700030C RID: 780
	// (get) Token: 0x06001C41 RID: 7233 RVA: 0x0008ABBC File Offset: 0x00088DBC
	[Networked]
	[Capacity(10)]
	public NetworkArray<int> playerLivesArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@10>(ref this._playerLivesArray), 10, ReaderWriter@System_Int32.GetInstance());
		}
	}

	// Token: 0x1700030D RID: 781
	// (get) Token: 0x06001C42 RID: 7234 RVA: 0x0008ABE4 File Offset: 0x00088DE4
	[Networked]
	[Capacity(10)]
	public NetworkArray<int> playerActorNumberArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@10>(ref this._playerActorNumberArray), 10, ReaderWriter@System_Int32.GetInstance());
		}
	}

	// Token: 0x1700030E RID: 782
	// (get) Token: 0x06001C43 RID: 7235 RVA: 0x0008AC0C File Offset: 0x00088E0C
	[Networked]
	[Capacity(10)]
	public NetworkArray<GorillaPaintbrawlManager.PaintbrawlStatus> playerStatusArray
	{
		get
		{
			return new NetworkArray<GorillaPaintbrawlManager.PaintbrawlStatus>(Native.ReferenceToPointer<FixedStorage@10>(ref this._playerStatusArray), 10, ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus.GetInstance());
		}
	}

	// Token: 0x04001F78 RID: 8056
	[FieldOffset(0)]
	public GorillaPaintbrawlManager.PaintbrawlState currentPaintbrawlState;

	// Token: 0x04001F79 RID: 8057
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ReaderWriter@System_Int32), 10, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@10 _playerLivesArray;

	// Token: 0x04001F7A RID: 8058
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ReaderWriter@System_Int32), 10, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(44)]
	private FixedStorage@10 _playerActorNumberArray;

	// Token: 0x04001F7B RID: 8059
	[FixedBufferProperty(typeof(NetworkArray<GorillaPaintbrawlManager.PaintbrawlStatus>), typeof(UnityArraySurrogate@ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus), 10, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(84)]
	private FixedStorage@10 _playerStatusArray;
}
