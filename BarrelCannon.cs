using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020000B3 RID: 179
[NetworkBehaviourWeaved(3)]
public class BarrelCannon : NetworkComponent
{
	// Token: 0x0600045B RID: 1115 RVA: 0x000194C1 File Offset: 0x000176C1
	private void Update()
	{
		if (base.IsMine)
		{
			this.AuthorityUpdate();
		}
		else
		{
			this.ClientUpdate();
		}
		this.SharedUpdate();
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x000194E0 File Offset: 0x000176E0
	private void AuthorityUpdate()
	{
		float time = Time.time;
		this.syncedState.hasAuthorityPassenger = this.localPlayerInside;
		switch (this.syncedState.currentState)
		{
		default:
			if (this.localPlayerInside)
			{
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.Loaded;
				return;
			}
			break;
		case BarrelCannon.BarrelCannonState.Loaded:
			if (time - this.stateStartTime > this.cannonEntryDelayTime)
			{
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.MovingToFirePosition;
				return;
			}
			break;
		case BarrelCannon.BarrelCannonState.MovingToFirePosition:
			if (this.moveToFiringPositionTime > Mathf.Epsilon)
			{
				this.syncedState.firingPositionLerpValue = Mathf.Clamp01((time - this.stateStartTime) / this.moveToFiringPositionTime);
			}
			else
			{
				this.syncedState.firingPositionLerpValue = 1f;
			}
			if (this.syncedState.firingPositionLerpValue >= 1f - Mathf.Epsilon)
			{
				this.syncedState.firingPositionLerpValue = 1f;
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.Firing;
				return;
			}
			break;
		case BarrelCannon.BarrelCannonState.Firing:
			if (this.localPlayerInside && this.localPlayerRigidbody != null)
			{
				Vector3 vector = base.transform.position - GorillaTagger.Instance.headCollider.transform.position;
				this.localPlayerRigidbody.MovePosition(this.localPlayerRigidbody.position + vector);
			}
			if (time - this.stateStartTime > this.preFiringDelayTime)
			{
				base.transform.localPosition = this.firingPositionOffset;
				base.transform.localRotation = Quaternion.Euler(this.firingRotationOffset);
				this.FireBarrelCannonLocal(base.transform.position, base.transform.up);
				if (PhotonNetwork.InRoom && GorillaGameManager.instance != null)
				{
					base.SendRPC("FireBarrelCannonRPC", RpcTarget.Others, new object[]
					{
						base.transform.position,
						base.transform.up
					});
				}
				Collider[] array = this.colliders;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = false;
				}
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.PostFireCooldown;
				return;
			}
			break;
		case BarrelCannon.BarrelCannonState.PostFireCooldown:
			if (time - this.stateStartTime > this.postFiringCooldownTime)
			{
				Collider[] array = this.colliders;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = true;
				}
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.ReturningToIdlePosition;
				return;
			}
			break;
		case BarrelCannon.BarrelCannonState.ReturningToIdlePosition:
			if (this.returnToIdlePositionTime > Mathf.Epsilon)
			{
				this.syncedState.firingPositionLerpValue = 1f - Mathf.Clamp01((time - this.stateStartTime) / this.returnToIdlePositionTime);
			}
			else
			{
				this.syncedState.firingPositionLerpValue = 0f;
			}
			if (this.syncedState.firingPositionLerpValue <= Mathf.Epsilon)
			{
				this.syncedState.firingPositionLerpValue = 0f;
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.Idle;
			}
			break;
		}
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x000197E4 File Offset: 0x000179E4
	private void ClientUpdate()
	{
		if (!this.syncedState.hasAuthorityPassenger && this.syncedState.currentState == BarrelCannon.BarrelCannonState.Idle && this.localPlayerInside)
		{
			base.RequestOwnership();
		}
	}

	// Token: 0x0600045E RID: 1118 RVA: 0x00019810 File Offset: 0x00017A10
	private void SharedUpdate()
	{
		if (this.syncedState.firingPositionLerpValue != this.localFiringPositionLerpValue)
		{
			this.localFiringPositionLerpValue = this.syncedState.firingPositionLerpValue;
			base.transform.localPosition = Vector3.Lerp(Vector3.zero, this.firingPositionOffset, this.firePositionAnimationCurve.Evaluate(this.localFiringPositionLerpValue));
			base.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, this.firingRotationOffset, this.fireRotationAnimationCurve.Evaluate(this.localFiringPositionLerpValue)));
		}
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x0001989E File Offset: 0x00017A9E
	[PunRPC]
	private void FireBarrelCannonRPC(Vector3 cannonCenter, Vector3 firingDirection)
	{
		this.FireBarrelCannonLocal(cannonCenter, firingDirection);
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x000198A8 File Offset: 0x00017AA8
	private void FireBarrelCannonLocal(Vector3 cannonCenter, Vector3 firingDirection)
	{
		if (this.audioSource != null)
		{
			this.audioSource.GTPlay();
		}
		if (this.localPlayerInside && this.localPlayerRigidbody != null)
		{
			Vector3 vector = cannonCenter - GorillaTagger.Instance.headCollider.transform.position;
			this.localPlayerRigidbody.position = this.localPlayerRigidbody.position + vector;
			this.localPlayerRigidbody.velocity = firingDirection * this.firingSpeed;
		}
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x00019934 File Offset: 0x00017B34
	private void OnTriggerEnter(Collider other)
	{
		Rigidbody rigidbody;
		if (this.LocalPlayerTriggerFilter(other, out rigidbody))
		{
			this.localPlayerInside = true;
			this.localPlayerRigidbody = rigidbody;
		}
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x0001995C File Offset: 0x00017B5C
	private void OnTriggerExit(Collider other)
	{
		Rigidbody rigidbody;
		if (this.LocalPlayerTriggerFilter(other, out rigidbody))
		{
			this.localPlayerInside = false;
			this.localPlayerRigidbody = null;
		}
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x00019982 File Offset: 0x00017B82
	private bool LocalPlayerTriggerFilter(Collider other, out Rigidbody rb)
	{
		rb = null;
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			rb = GorillaTagger.Instance.GetComponent<Rigidbody>();
		}
		return rb != null;
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x000199B8 File Offset: 0x00017BB8
	private bool IsLocalPlayerInCannon()
	{
		Vector3 vector;
		Vector3 vector2;
		this.GetCapsulePoints(this.triggerCollider, out vector, out vector2);
		Physics.OverlapCapsuleNonAlloc(vector, vector2, this.triggerCollider.radius, this.triggerOverlapResults);
		for (int i = 0; i < this.triggerOverlapResults.Length; i++)
		{
			Rigidbody rigidbody;
			if (this.LocalPlayerTriggerFilter(this.triggerOverlapResults[i], out rigidbody))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000465 RID: 1125 RVA: 0x00019A18 File Offset: 0x00017C18
	private void GetCapsulePoints(CapsuleCollider capsule, out Vector3 pointA, out Vector3 pointB)
	{
		float num = capsule.height * 0.5f - capsule.radius;
		pointA = capsule.transform.position + capsule.transform.up * num;
		pointB = capsule.transform.position - capsule.transform.up * num;
	}

	// Token: 0x1700004F RID: 79
	// (get) Token: 0x06000466 RID: 1126 RVA: 0x00019A87 File Offset: 0x00017C87
	// (set) Token: 0x06000467 RID: 1127 RVA: 0x00019AB1 File Offset: 0x00017CB1
	[Networked]
	[NetworkedWeaved(0, 3)]
	private unsafe BarrelCannon.BarrelCannonSyncedStateData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BarrelCannon.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(BarrelCannon.BarrelCannonSyncedStateData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BarrelCannon.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(BarrelCannon.BarrelCannonSyncedStateData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x00019ADC File Offset: 0x00017CDC
	public override void WriteDataFusion()
	{
		this.Data = this.syncedState;
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x00019AF0 File Offset: 0x00017CF0
	public override void ReadDataFusion()
	{
		this.syncedState.currentState = this.Data.CurrentState;
		this.syncedState.hasAuthorityPassenger = this.Data.HasAuthorityPassenger;
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x00019B34 File Offset: 0x00017D34
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.syncedState.currentState);
		stream.SendNext(this.syncedState.hasAuthorityPassenger);
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x00019B62 File Offset: 0x00017D62
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.syncedState.currentState = (BarrelCannon.BarrelCannonState)stream.ReceiveNext();
		this.syncedState.hasAuthorityPassenger = (bool)stream.ReceiveNext();
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x00019B90 File Offset: 0x00017D90
	public override void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
		if (!this.localPlayerInside)
		{
			targetView.TransferOwnership(requestingPlayer);
		}
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x00019C65 File Offset: 0x00017E65
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x00019C7D File Offset: 0x00017E7D
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04000507 RID: 1287
	[SerializeField]
	private float firingSpeed = 10f;

	// Token: 0x04000508 RID: 1288
	[Header("Cannon's Movement Before Firing")]
	[SerializeField]
	private Vector3 firingPositionOffset = Vector3.zero;

	// Token: 0x04000509 RID: 1289
	[SerializeField]
	private Vector3 firingRotationOffset = Vector3.zero;

	// Token: 0x0400050A RID: 1290
	[SerializeField]
	private AnimationCurve firePositionAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400050B RID: 1291
	[SerializeField]
	private AnimationCurve fireRotationAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400050C RID: 1292
	[Header("Cannon State Change Timing Parameters")]
	[SerializeField]
	private float moveToFiringPositionTime = 0.5f;

	// Token: 0x0400050D RID: 1293
	[SerializeField]
	[Tooltip("The minimum time to wait after a gorilla enters the cannon before it starts moving into the firing position.")]
	private float cannonEntryDelayTime = 0.25f;

	// Token: 0x0400050E RID: 1294
	[SerializeField]
	[Tooltip("The minimum time to wait after a gorilla enters the cannon before it starts moving into the firing position.")]
	private float preFiringDelayTime = 0.25f;

	// Token: 0x0400050F RID: 1295
	[SerializeField]
	[Tooltip("The minimum time to wait after the cannon fires before it starts moving back to the idle position.")]
	private float postFiringCooldownTime = 0.25f;

	// Token: 0x04000510 RID: 1296
	[SerializeField]
	private float returnToIdlePositionTime = 1f;

	// Token: 0x04000511 RID: 1297
	[Header("Component References")]
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000512 RID: 1298
	[SerializeField]
	private CapsuleCollider triggerCollider;

	// Token: 0x04000513 RID: 1299
	[SerializeField]
	private Collider[] colliders;

	// Token: 0x04000514 RID: 1300
	private BarrelCannon.BarrelCannonSyncedState syncedState = new BarrelCannon.BarrelCannonSyncedState();

	// Token: 0x04000515 RID: 1301
	private Collider[] triggerOverlapResults = new Collider[16];

	// Token: 0x04000516 RID: 1302
	private bool localPlayerInside;

	// Token: 0x04000517 RID: 1303
	private Rigidbody localPlayerRigidbody;

	// Token: 0x04000518 RID: 1304
	private float stateStartTime;

	// Token: 0x04000519 RID: 1305
	private float localFiringPositionLerpValue;

	// Token: 0x0400051A RID: 1306
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 3)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private BarrelCannon.BarrelCannonSyncedStateData _Data;

	// Token: 0x020000B4 RID: 180
	private enum BarrelCannonState
	{
		// Token: 0x0400051C RID: 1308
		Idle,
		// Token: 0x0400051D RID: 1309
		Loaded,
		// Token: 0x0400051E RID: 1310
		MovingToFirePosition,
		// Token: 0x0400051F RID: 1311
		Firing,
		// Token: 0x04000520 RID: 1312
		PostFireCooldown,
		// Token: 0x04000521 RID: 1313
		ReturningToIdlePosition
	}

	// Token: 0x020000B5 RID: 181
	private class BarrelCannonSyncedState
	{
		// Token: 0x04000522 RID: 1314
		public BarrelCannon.BarrelCannonState currentState;

		// Token: 0x04000523 RID: 1315
		public bool hasAuthorityPassenger;

		// Token: 0x04000524 RID: 1316
		public float firingPositionLerpValue;
	}

	// Token: 0x020000B6 RID: 182
	[NetworkStructWeaved(3)]
	[StructLayout(LayoutKind.Explicit, Size = 12)]
	private struct BarrelCannonSyncedStateData : INetworkStruct
	{
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000471 RID: 1137 RVA: 0x00019C91 File Offset: 0x00017E91
		// (set) Token: 0x06000472 RID: 1138 RVA: 0x00019CA3 File Offset: 0x00017EA3
		[Networked]
		public unsafe BarrelCannon.BarrelCannonState CurrentState
		{
			readonly get
			{
				return *(BarrelCannon.BarrelCannonState*)Native.ReferenceToPointer<FixedStorage@1>(ref this._CurrentState);
			}
			set
			{
				*(BarrelCannon.BarrelCannonState*)Native.ReferenceToPointer<FixedStorage@1>(ref this._CurrentState) = value;
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000473 RID: 1139 RVA: 0x00019CB6 File Offset: 0x00017EB6
		// (set) Token: 0x06000474 RID: 1140 RVA: 0x00019CC8 File Offset: 0x00017EC8
		[Networked]
		public unsafe NetworkBool HasAuthorityPassenger
		{
			readonly get
			{
				return *(NetworkBool*)Native.ReferenceToPointer<FixedStorage@1>(ref this._HasAuthorityPassenger);
			}
			set
			{
				*(NetworkBool*)Native.ReferenceToPointer<FixedStorage@1>(ref this._HasAuthorityPassenger) = value;
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000475 RID: 1141 RVA: 0x00019CDB File Offset: 0x00017EDB
		// (set) Token: 0x06000476 RID: 1142 RVA: 0x00019CE3 File Offset: 0x00017EE3
		public float FiringPositionLerpValue { readonly get; set; }

		// Token: 0x06000477 RID: 1143 RVA: 0x00019CEC File Offset: 0x00017EEC
		public BarrelCannonSyncedStateData(BarrelCannon.BarrelCannonState state, bool hasAuthPassenger, float firingPosLerpVal)
		{
			this.CurrentState = state;
			this.HasAuthorityPassenger = hasAuthPassenger;
			this.FiringPositionLerpValue = firingPosLerpVal;
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x00019D08 File Offset: 0x00017F08
		public static implicit operator BarrelCannon.BarrelCannonSyncedStateData(BarrelCannon.BarrelCannonSyncedState state)
		{
			return new BarrelCannon.BarrelCannonSyncedStateData(state.currentState, state.hasAuthorityPassenger, state.firingPositionLerpValue);
		}

		// Token: 0x04000525 RID: 1317
		[FixedBufferProperty(typeof(BarrelCannon.BarrelCannonState), typeof(UnityValueSurrogate@ReaderWriter@BarrelCannon__BarrelCannonState), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(0)]
		private FixedStorage@1 _CurrentState;

		// Token: 0x04000526 RID: 1318
		[FixedBufferProperty(typeof(NetworkBool), typeof(UnityValueSurrogate@ReaderWriter@Fusion_NetworkBool), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(4)]
		private FixedStorage@1 _HasAuthorityPassenger;
	}
}
