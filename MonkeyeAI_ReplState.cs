using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000BB RID: 187
[NetworkBehaviourWeaved(42)]
public class MonkeyeAI_ReplState : NetworkComponent
{
	// Token: 0x17000053 RID: 83
	// (get) Token: 0x060004AC RID: 1196 RVA: 0x0001B8C8 File Offset: 0x00019AC8
	// (set) Token: 0x060004AD RID: 1197 RVA: 0x0001B8F2 File Offset: 0x00019AF2
	[Networked]
	[NetworkedWeaved(0, 42)]
	private unsafe MonkeyeAI_ReplState.MonkeyeAI_RepStateData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing MonkeyeAI_ReplState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(MonkeyeAI_ReplState.MonkeyeAI_RepStateData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing MonkeyeAI_ReplState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(MonkeyeAI_ReplState.MonkeyeAI_RepStateData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x060004AE RID: 1198 RVA: 0x0001B920 File Offset: 0x00019B20
	public override void WriteDataFusion()
	{
		MonkeyeAI_ReplState.MonkeyeAI_RepStateData monkeyeAI_RepStateData = new MonkeyeAI_ReplState.MonkeyeAI_RepStateData(this.userId, this.attackPos, this.timer, this.floorEnabled, this.portalEnabled, this.freezePlayer, this.alpha, this.state);
		this.Data = monkeyeAI_RepStateData;
	}

	// Token: 0x060004AF RID: 1199 RVA: 0x0001B96C File Offset: 0x00019B6C
	public override void ReadDataFusion()
	{
		this.userId = this.Data.UserId.Value;
		this.attackPos = this.Data.AttackPos;
		this.timer = this.Data.Timer;
		this.floorEnabled = this.Data.FloorEnabled;
		this.portalEnabled = this.Data.PortalEnabled;
		this.freezePlayer = this.Data.FreezePlayer;
		this.alpha = this.Data.Alpha;
		this.state = this.Data.State;
	}

	// Token: 0x060004B0 RID: 1200 RVA: 0x0001BA30 File Offset: 0x00019C30
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.userId);
		stream.SendNext(this.attackPos);
		stream.SendNext(this.timer);
		stream.SendNext(this.floorEnabled);
		stream.SendNext(this.portalEnabled);
		stream.SendNext(this.freezePlayer);
		stream.SendNext(this.alpha);
		stream.SendNext(this.state);
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x0001BAC0 File Offset: 0x00019CC0
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.photonView.Owner == null)
		{
			return;
		}
		if (info.Sender.ActorNumber != info.photonView.Owner.ActorNumber)
		{
			return;
		}
		this.userId = (string)stream.ReceiveNext();
		Vector3 vector = (Vector3)stream.ReceiveNext();
		(ref this.attackPos).SetValueSafe(in vector);
		this.timer = (float)stream.ReceiveNext();
		this.floorEnabled = (bool)stream.ReceiveNext();
		this.portalEnabled = (bool)stream.ReceiveNext();
		this.freezePlayer = (bool)stream.ReceiveNext();
		this.alpha = ((float)stream.ReceiveNext()).ClampSafe(0f, 1f);
		this.state = (MonkeyeAI_ReplState.EStates)stream.ReceiveNext();
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x0001BB98 File Offset: 0x00019D98
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x0001BBB0 File Offset: 0x00019DB0
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x0400057A RID: 1402
	public MonkeyeAI_ReplState.EStates state;

	// Token: 0x0400057B RID: 1403
	public string userId;

	// Token: 0x0400057C RID: 1404
	public Vector3 attackPos;

	// Token: 0x0400057D RID: 1405
	public float timer;

	// Token: 0x0400057E RID: 1406
	public bool floorEnabled;

	// Token: 0x0400057F RID: 1407
	public bool portalEnabled;

	// Token: 0x04000580 RID: 1408
	public bool freezePlayer;

	// Token: 0x04000581 RID: 1409
	public float alpha;

	// Token: 0x04000582 RID: 1410
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 42)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private MonkeyeAI_ReplState.MonkeyeAI_RepStateData _Data;

	// Token: 0x020000BC RID: 188
	public enum EStates
	{
		// Token: 0x04000584 RID: 1412
		Sleeping,
		// Token: 0x04000585 RID: 1413
		Patrolling,
		// Token: 0x04000586 RID: 1414
		Chasing,
		// Token: 0x04000587 RID: 1415
		ReturnToSleepPt,
		// Token: 0x04000588 RID: 1416
		GoToSleep,
		// Token: 0x04000589 RID: 1417
		BeginAttack,
		// Token: 0x0400058A RID: 1418
		OpenFloor,
		// Token: 0x0400058B RID: 1419
		DropPlayer,
		// Token: 0x0400058C RID: 1420
		CloseFloor
	}

	// Token: 0x020000BD RID: 189
	[NetworkStructWeaved(42)]
	[StructLayout(LayoutKind.Explicit, Size = 168)]
	public struct MonkeyeAI_RepStateData : INetworkStruct
	{
		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060004B5 RID: 1205 RVA: 0x0001BBC4 File Offset: 0x00019DC4
		// (set) Token: 0x060004B6 RID: 1206 RVA: 0x0001BBD6 File Offset: 0x00019DD6
		[Networked]
		public unsafe NetworkString<_32> UserId
		{
			readonly get
			{
				return *(NetworkString<_32>*)Native.ReferenceToPointer<FixedStorage@33>(ref this._UserId);
			}
			set
			{
				*(NetworkString<_32>*)Native.ReferenceToPointer<FixedStorage@33>(ref this._UserId) = value;
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060004B7 RID: 1207 RVA: 0x0001BBE9 File Offset: 0x00019DE9
		// (set) Token: 0x060004B8 RID: 1208 RVA: 0x0001BBFB File Offset: 0x00019DFB
		[Networked]
		public unsafe Vector3 AttackPos
		{
			readonly get
			{
				return *(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._AttackPos);
			}
			set
			{
				*(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._AttackPos) = value;
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060004B9 RID: 1209 RVA: 0x0001BC0E File Offset: 0x00019E0E
		// (set) Token: 0x060004BA RID: 1210 RVA: 0x0001BC1C File Offset: 0x00019E1C
		[Networked]
		public unsafe float Timer
		{
			readonly get
			{
				return *(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._Timer);
			}
			set
			{
				*(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._Timer) = value;
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060004BB RID: 1211 RVA: 0x0001BC2B File Offset: 0x00019E2B
		// (set) Token: 0x060004BC RID: 1212 RVA: 0x0001BC33 File Offset: 0x00019E33
		public NetworkBool FloorEnabled { readonly get; set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060004BD RID: 1213 RVA: 0x0001BC3C File Offset: 0x00019E3C
		// (set) Token: 0x060004BE RID: 1214 RVA: 0x0001BC44 File Offset: 0x00019E44
		public NetworkBool PortalEnabled { readonly get; set; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060004BF RID: 1215 RVA: 0x0001BC4D File Offset: 0x00019E4D
		// (set) Token: 0x060004C0 RID: 1216 RVA: 0x0001BC55 File Offset: 0x00019E55
		public NetworkBool FreezePlayer { readonly get; set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060004C1 RID: 1217 RVA: 0x0001BC5E File Offset: 0x00019E5E
		// (set) Token: 0x060004C2 RID: 1218 RVA: 0x0001BC6C File Offset: 0x00019E6C
		[Networked]
		public unsafe float Alpha
		{
			readonly get
			{
				return *(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._Alpha);
			}
			set
			{
				*(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._Alpha) = value;
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060004C3 RID: 1219 RVA: 0x0001BC7B File Offset: 0x00019E7B
		// (set) Token: 0x060004C4 RID: 1220 RVA: 0x0001BC83 File Offset: 0x00019E83
		public MonkeyeAI_ReplState.EStates State { readonly get; set; }

		// Token: 0x060004C5 RID: 1221 RVA: 0x0001BC8C File Offset: 0x00019E8C
		public MonkeyeAI_RepStateData(string id, Vector3 atPos, float timer, bool floorOn, bool portalOn, bool freezePlayer, float alpha, MonkeyeAI_ReplState.EStates state)
		{
			this.UserId = id;
			this.AttackPos = atPos;
			this.Timer = timer;
			this.FloorEnabled = floorOn;
			this.PortalEnabled = portalOn;
			this.FreezePlayer = freezePlayer;
			this.Alpha = alpha;
			this.State = state;
		}

		// Token: 0x0400058D RID: 1421
		[FixedBufferProperty(typeof(NetworkString<_32>), typeof(UnityValueSurrogate@ReaderWriter@Fusion_NetworkString), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(0)]
		private FixedStorage@33 _UserId;

		// Token: 0x0400058E RID: 1422
		[FixedBufferProperty(typeof(Vector3), typeof(UnityValueSurrogate@ReaderWriter@UnityEngine_Vector3), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(132)]
		private FixedStorage@3 _AttackPos;

		// Token: 0x0400058F RID: 1423
		[FixedBufferProperty(typeof(float), typeof(UnityValueSurrogate@ReaderWriter@System_Single), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(144)]
		private FixedStorage@1 _Timer;

		// Token: 0x04000593 RID: 1427
		[FixedBufferProperty(typeof(float), typeof(UnityValueSurrogate@ReaderWriter@System_Single), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(160)]
		private FixedStorage@1 _Alpha;
	}
}
