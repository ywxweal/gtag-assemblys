using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000203 RID: 515
[NetworkBehaviourWeaved(2)]
public class RandomTimedSeedManager : NetworkComponent, ITickSystemTick
{
	// Token: 0x1700012E RID: 302
	// (get) Token: 0x06000BF7 RID: 3063 RVA: 0x0003FA26 File Offset: 0x0003DC26
	// (set) Token: 0x06000BF8 RID: 3064 RVA: 0x0003FA2D File Offset: 0x0003DC2D
	public static RandomTimedSeedManager instance { get; private set; }

	// Token: 0x1700012F RID: 303
	// (get) Token: 0x06000BF9 RID: 3065 RVA: 0x0003FA35 File Offset: 0x0003DC35
	// (set) Token: 0x06000BFA RID: 3066 RVA: 0x0003FA3D File Offset: 0x0003DC3D
	public int seed { get; private set; }

	// Token: 0x17000130 RID: 304
	// (get) Token: 0x06000BFB RID: 3067 RVA: 0x0003FA46 File Offset: 0x0003DC46
	// (set) Token: 0x06000BFC RID: 3068 RVA: 0x0003FA4E File Offset: 0x0003DC4E
	public float currentSyncTime { get; private set; }

	// Token: 0x06000BFD RID: 3069 RVA: 0x0003FA57 File Offset: 0x0003DC57
	protected override void Awake()
	{
		base.Awake();
		RandomTimedSeedManager.instance = this;
		this.seed = Random.Range(-1000000, -1000000);
		this.idealSyncTime = 0f;
		this.currentSyncTime = 0f;
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06000BFE RID: 3070 RVA: 0x0003FA96 File Offset: 0x0003DC96
	public void AddCallbackOnSeedChanged(Action callback)
	{
		this.callbacksOnSeedChanged.Add(callback);
	}

	// Token: 0x06000BFF RID: 3071 RVA: 0x0003FAA4 File Offset: 0x0003DCA4
	public void RemoveCallbackOnSeedChanged(Action callback)
	{
		this.callbacksOnSeedChanged.Remove(callback);
	}

	// Token: 0x17000131 RID: 305
	// (get) Token: 0x06000C00 RID: 3072 RVA: 0x0003FAB3 File Offset: 0x0003DCB3
	// (set) Token: 0x06000C01 RID: 3073 RVA: 0x0003FABB File Offset: 0x0003DCBB
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x06000C02 RID: 3074 RVA: 0x0003FAC4 File Offset: 0x0003DCC4
	void ITickSystemTick.Tick()
	{
		this.currentSyncTime += Time.deltaTime;
		this.idealSyncTime += Time.deltaTime;
		if (this.idealSyncTime > 1E+09f)
		{
			this.idealSyncTime -= 1E+09f;
			this.currentSyncTime -= 1E+09f;
		}
		if (!base.GetView.AmOwner)
		{
			this.currentSyncTime = Mathf.Lerp(this.currentSyncTime, this.idealSyncTime, 0.1f);
		}
	}

	// Token: 0x17000132 RID: 306
	// (get) Token: 0x06000C03 RID: 3075 RVA: 0x0003FB4F File Offset: 0x0003DD4F
	// (set) Token: 0x06000C04 RID: 3076 RVA: 0x0003FB79 File Offset: 0x0003DD79
	[Networked]
	[NetworkedWeaved(0, 2)]
	private unsafe RandomTimedSeedManager.RandomTimedSeedManagerData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing RandomTimedSeedManager.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(RandomTimedSeedManager.RandomTimedSeedManagerData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing RandomTimedSeedManager.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(RandomTimedSeedManager.RandomTimedSeedManagerData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x0003FBA4 File Offset: 0x0003DDA4
	public override void WriteDataFusion()
	{
		this.Data = new RandomTimedSeedManager.RandomTimedSeedManagerData(this.seed, this.currentSyncTime);
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x0003FBC0 File Offset: 0x0003DDC0
	public override void ReadDataFusion()
	{
		this.ReadDataShared(this.Data.seed, this.Data.currentSyncTime);
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x0003FBEF File Offset: 0x0003DDEF
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		stream.SendNext(this.seed);
		stream.SendNext(this.currentSyncTime);
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x0003FC24 File Offset: 0x0003DE24
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		int num = (int)stream.ReceiveNext();
		float num2 = (float)stream.ReceiveNext();
		this.ReadDataShared(num, num2);
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x0003FC60 File Offset: 0x0003DE60
	private void ReadDataShared(int seedVal, float testTime)
	{
		if (!float.IsFinite(testTime))
		{
			return;
		}
		this.seed = seedVal;
		if (testTime >= 0f && testTime <= 1E+09f)
		{
			if (this.idealSyncTime - testTime > 500000000f)
			{
				this.currentSyncTime = testTime;
			}
			this.idealSyncTime = testTime;
		}
		if (this.seed != this.cachedSeed && this.seed >= -1000000 && this.seed <= -1000000)
		{
			this.currentSyncTime = this.idealSyncTime;
			this.cachedSeed = this.seed;
			foreach (Action action in this.callbacksOnSeedChanged)
			{
				action();
			}
		}
	}

	// Token: 0x06000C0B RID: 3083 RVA: 0x0003FD43 File Offset: 0x0003DF43
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06000C0C RID: 3084 RVA: 0x0003FD5B File Offset: 0x0003DF5B
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04000E89 RID: 3721
	private List<Action> callbacksOnSeedChanged = new List<Action>();

	// Token: 0x04000E8B RID: 3723
	private float idealSyncTime;

	// Token: 0x04000E8D RID: 3725
	private int cachedSeed;

	// Token: 0x04000E8E RID: 3726
	private const int SeedMin = -1000000;

	// Token: 0x04000E8F RID: 3727
	private const int SeedMax = -1000000;

	// Token: 0x04000E90 RID: 3728
	private const float MaxSyncTime = 1E+09f;

	// Token: 0x04000E92 RID: 3730
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 2)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private RandomTimedSeedManager.RandomTimedSeedManagerData _Data;

	// Token: 0x02000204 RID: 516
	[NetworkStructWeaved(2)]
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	private struct RandomTimedSeedManagerData : INetworkStruct
	{
		// Token: 0x17000133 RID: 307
		// (get) Token: 0x06000C0D RID: 3085 RVA: 0x0003FD6F File Offset: 0x0003DF6F
		// (set) Token: 0x06000C0E RID: 3086 RVA: 0x0003FD7D File Offset: 0x0003DF7D
		[Networked]
		public unsafe int seed
		{
			readonly get
			{
				return *(int*)Native.ReferenceToPointer<FixedStorage@1>(ref this._seed);
			}
			set
			{
				*(int*)Native.ReferenceToPointer<FixedStorage@1>(ref this._seed) = value;
			}
		}

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x06000C0F RID: 3087 RVA: 0x0003FD8C File Offset: 0x0003DF8C
		// (set) Token: 0x06000C10 RID: 3088 RVA: 0x0003FD9A File Offset: 0x0003DF9A
		[Networked]
		public unsafe float currentSyncTime
		{
			readonly get
			{
				return *(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._currentSyncTime);
			}
			set
			{
				*(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._currentSyncTime) = value;
			}
		}

		// Token: 0x06000C11 RID: 3089 RVA: 0x0003FDA9 File Offset: 0x0003DFA9
		public RandomTimedSeedManagerData(int seed, float currentSyncTime)
		{
			this.seed = seed;
			this.currentSyncTime = currentSyncTime;
		}

		// Token: 0x04000E93 RID: 3731
		[FixedBufferProperty(typeof(int), typeof(UnityValueSurrogate@ReaderWriter@System_Int32), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(0)]
		private FixedStorage@1 _seed;

		// Token: 0x04000E94 RID: 3732
		[FixedBufferProperty(typeof(float), typeof(UnityValueSurrogate@ReaderWriter@System_Single), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(4)]
		private FixedStorage@1 _currentSyncTime;
	}
}
