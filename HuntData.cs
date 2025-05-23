using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x02000489 RID: 1161
[NetworkStructWeaved(23)]
[StructLayout(LayoutKind.Explicit, Size = 92)]
public struct HuntData : INetworkStruct
{
	// Token: 0x17000314 RID: 788
	// (get) Token: 0x06001C67 RID: 7271 RVA: 0x0008B02C File Offset: 0x0008922C
	[Networked]
	[Capacity(10)]
	public NetworkArray<int> currentHuntedArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@10>(ref this._currentHuntedArray), 10, ReaderWriter@System_Int32.GetInstance());
		}
	}

	// Token: 0x17000315 RID: 789
	// (get) Token: 0x06001C68 RID: 7272 RVA: 0x0008B054 File Offset: 0x00089254
	[Networked]
	[Capacity(10)]
	public NetworkArray<int> currentTargetArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@10>(ref this._currentTargetArray), 10, ReaderWriter@System_Int32.GetInstance());
		}
	}

	// Token: 0x04001F82 RID: 8066
	[FieldOffset(0)]
	public NetworkBool huntStarted;

	// Token: 0x04001F83 RID: 8067
	[FieldOffset(4)]
	public NetworkBool waitingToStartNextHuntGame;

	// Token: 0x04001F84 RID: 8068
	[FieldOffset(8)]
	public int countDownTime;

	// Token: 0x04001F85 RID: 8069
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ReaderWriter@System_Int32), 10, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(12)]
	private FixedStorage@10 _currentHuntedArray;

	// Token: 0x04001F86 RID: 8070
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ReaderWriter@System_Int32), 10, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(52)]
	private FixedStorage@10 _currentTargetArray;
}
