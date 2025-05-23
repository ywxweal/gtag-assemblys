using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020002E5 RID: 741
public class LocomotionTeleport : MonoBehaviour
{
	// Token: 0x060011BA RID: 4538 RVA: 0x0005530B File Offset: 0x0005350B
	public void EnableMovement(bool ready, bool aim, bool pre, bool post)
	{
		this.EnableMovementDuringReady = ready;
		this.EnableMovementDuringAim = aim;
		this.EnableMovementDuringPreTeleport = pre;
		this.EnableMovementDuringPostTeleport = post;
	}

	// Token: 0x060011BB RID: 4539 RVA: 0x0005532A File Offset: 0x0005352A
	public void EnableRotation(bool ready, bool aim, bool pre, bool post)
	{
		this.EnableRotationDuringReady = ready;
		this.EnableRotationDuringAim = aim;
		this.EnableRotationDuringPreTeleport = pre;
		this.EnableRotationDuringPostTeleport = post;
	}

	// Token: 0x170001F8 RID: 504
	// (get) Token: 0x060011BC RID: 4540 RVA: 0x00055349 File Offset: 0x00053549
	// (set) Token: 0x060011BD RID: 4541 RVA: 0x00055351 File Offset: 0x00053551
	public LocomotionTeleport.States CurrentState { get; private set; }

	// Token: 0x14000031 RID: 49
	// (add) Token: 0x060011BE RID: 4542 RVA: 0x0005535C File Offset: 0x0005355C
	// (remove) Token: 0x060011BF RID: 4543 RVA: 0x00055394 File Offset: 0x00053594
	public event Action<bool, Vector3?, Quaternion?, Quaternion?> UpdateTeleportDestination;

	// Token: 0x060011C0 RID: 4544 RVA: 0x000553C9 File Offset: 0x000535C9
	public void OnUpdateTeleportDestination(bool isValidDestination, Vector3? position, Quaternion? rotation, Quaternion? landingRotation)
	{
		if (this.UpdateTeleportDestination != null)
		{
			this.UpdateTeleportDestination(isValidDestination, position, rotation, landingRotation);
		}
	}

	// Token: 0x170001F9 RID: 505
	// (get) Token: 0x060011C1 RID: 4545 RVA: 0x000553E3 File Offset: 0x000535E3
	public Quaternion DestinationRotation
	{
		get
		{
			return this._teleportDestination.OrientationIndicator.rotation;
		}
	}

	// Token: 0x170001FA RID: 506
	// (get) Token: 0x060011C2 RID: 4546 RVA: 0x000553F5 File Offset: 0x000535F5
	// (set) Token: 0x060011C3 RID: 4547 RVA: 0x000553FD File Offset: 0x000535FD
	public LocomotionController LocomotionController { get; private set; }

	// Token: 0x060011C4 RID: 4548 RVA: 0x00055408 File Offset: 0x00053608
	public bool AimCollisionTest(Vector3 start, Vector3 end, LayerMask aimCollisionLayerMask, out RaycastHit hitInfo)
	{
		Vector3 vector = end - start;
		float magnitude = vector.magnitude;
		Vector3 vector2 = vector / magnitude;
		switch (this.AimCollisionType)
		{
		case LocomotionTeleport.AimCollisionTypes.Point:
			return Physics.Raycast(start, vector2, out hitInfo, magnitude, aimCollisionLayerMask, QueryTriggerInteraction.Ignore);
		case LocomotionTeleport.AimCollisionTypes.Sphere:
		{
			float num;
			if (this.UseCharacterCollisionData)
			{
				num = this.LocomotionController.CharacterController.radius;
			}
			else
			{
				num = this.AimCollisionRadius;
			}
			return Physics.SphereCast(start, num, vector2, out hitInfo, magnitude, aimCollisionLayerMask, QueryTriggerInteraction.Ignore);
		}
		case LocomotionTeleport.AimCollisionTypes.Capsule:
		{
			float num2;
			float num3;
			if (this.UseCharacterCollisionData)
			{
				CapsuleCollider characterController = this.LocomotionController.CharacterController;
				num2 = characterController.height;
				num3 = characterController.radius;
			}
			else
			{
				num2 = this.AimCollisionHeight;
				num3 = this.AimCollisionRadius;
			}
			return Physics.CapsuleCast(start + new Vector3(0f, num3, 0f), start + new Vector3(0f, num2 + num3, 0f), num3, vector2, out hitInfo, magnitude, aimCollisionLayerMask, QueryTriggerInteraction.Ignore);
		}
		default:
			throw new Exception();
		}
	}

	// Token: 0x060011C5 RID: 4549 RVA: 0x00055514 File Offset: 0x00053714
	[Conditional("DEBUG_TELEPORT_STATES")]
	protected void LogState(string msg)
	{
		Debug.Log(Time.frameCount.ToString() + ": " + msg);
	}

	// Token: 0x060011C6 RID: 4550 RVA: 0x00055540 File Offset: 0x00053740
	protected void CreateNewTeleportDestination()
	{
		this.TeleportDestinationPrefab.gameObject.SetActive(false);
		TeleportDestination teleportDestination = Object.Instantiate<TeleportDestination>(this.TeleportDestinationPrefab);
		teleportDestination.LocomotionTeleport = this;
		teleportDestination.gameObject.layer = this.TeleportDestinationLayer;
		this._teleportDestination = teleportDestination;
		this._teleportDestination.LocomotionTeleport = this;
	}

	// Token: 0x060011C7 RID: 4551 RVA: 0x00055595 File Offset: 0x00053795
	private void DeactivateDestination()
	{
		this._teleportDestination.OnDeactivated();
	}

	// Token: 0x060011C8 RID: 4552 RVA: 0x000555A2 File Offset: 0x000537A2
	public void RecycleTeleportDestination(TeleportDestination oldDestination)
	{
		if (oldDestination == this._teleportDestination)
		{
			this.CreateNewTeleportDestination();
		}
		Object.Destroy(oldDestination.gameObject);
	}

	// Token: 0x060011C9 RID: 4553 RVA: 0x000555C3 File Offset: 0x000537C3
	private void EnableMotion(bool enableLinear, bool enableRotation)
	{
		this.LocomotionController.PlayerController.EnableLinearMovement = enableLinear;
		this.LocomotionController.PlayerController.EnableRotation = enableRotation;
	}

	// Token: 0x060011CA RID: 4554 RVA: 0x000555E7 File Offset: 0x000537E7
	private void Awake()
	{
		this.LocomotionController = base.GetComponent<LocomotionController>();
		this.CreateNewTeleportDestination();
	}

	// Token: 0x060011CB RID: 4555 RVA: 0x000555FB File Offset: 0x000537FB
	public virtual void OnEnable()
	{
		this.CurrentState = LocomotionTeleport.States.Ready;
		base.StartCoroutine(this.ReadyStateCoroutine());
	}

	// Token: 0x060011CC RID: 4556 RVA: 0x00004F01 File Offset: 0x00003101
	public virtual void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x14000032 RID: 50
	// (add) Token: 0x060011CD RID: 4557 RVA: 0x00055614 File Offset: 0x00053814
	// (remove) Token: 0x060011CE RID: 4558 RVA: 0x0005564C File Offset: 0x0005384C
	public event Action EnterStateReady;

	// Token: 0x060011CF RID: 4559 RVA: 0x00055681 File Offset: 0x00053881
	protected IEnumerator ReadyStateCoroutine()
	{
		yield return null;
		this.CurrentState = LocomotionTeleport.States.Ready;
		this.EnableMotion(this.EnableMovementDuringReady, this.EnableRotationDuringReady);
		if (this.EnterStateReady != null)
		{
			this.EnterStateReady();
		}
		while (this.CurrentIntention != LocomotionTeleport.TeleportIntentions.Aim)
		{
			yield return null;
		}
		yield return null;
		base.StartCoroutine(this.AimStateCoroutine());
		yield break;
	}

	// Token: 0x14000033 RID: 51
	// (add) Token: 0x060011D0 RID: 4560 RVA: 0x00055690 File Offset: 0x00053890
	// (remove) Token: 0x060011D1 RID: 4561 RVA: 0x000556C8 File Offset: 0x000538C8
	public event Action EnterStateAim;

	// Token: 0x14000034 RID: 52
	// (add) Token: 0x060011D2 RID: 4562 RVA: 0x00055700 File Offset: 0x00053900
	// (remove) Token: 0x060011D3 RID: 4563 RVA: 0x00055738 File Offset: 0x00053938
	public event Action<LocomotionTeleport.AimData> UpdateAimData;

	// Token: 0x060011D4 RID: 4564 RVA: 0x0005576D File Offset: 0x0005396D
	public void OnUpdateAimData(LocomotionTeleport.AimData aimData)
	{
		if (this.UpdateAimData != null)
		{
			this.UpdateAimData(aimData);
		}
	}

	// Token: 0x14000035 RID: 53
	// (add) Token: 0x060011D5 RID: 4565 RVA: 0x00055784 File Offset: 0x00053984
	// (remove) Token: 0x060011D6 RID: 4566 RVA: 0x000557BC File Offset: 0x000539BC
	public event Action ExitStateAim;

	// Token: 0x060011D7 RID: 4567 RVA: 0x000557F1 File Offset: 0x000539F1
	protected IEnumerator AimStateCoroutine()
	{
		this.CurrentState = LocomotionTeleport.States.Aim;
		this.EnableMotion(this.EnableMovementDuringAim, this.EnableRotationDuringAim);
		if (this.EnterStateAim != null)
		{
			this.EnterStateAim();
		}
		this._teleportDestination.gameObject.SetActive(true);
		while (this.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim)
		{
			yield return null;
		}
		if (this.ExitStateAim != null)
		{
			this.ExitStateAim();
		}
		yield return null;
		if ((this.CurrentIntention == LocomotionTeleport.TeleportIntentions.PreTeleport || this.CurrentIntention == LocomotionTeleport.TeleportIntentions.Teleport) && this._teleportDestination.IsValidDestination)
		{
			base.StartCoroutine(this.PreTeleportStateCoroutine());
		}
		else
		{
			base.StartCoroutine(this.CancelAimStateCoroutine());
		}
		yield break;
	}

	// Token: 0x14000036 RID: 54
	// (add) Token: 0x060011D8 RID: 4568 RVA: 0x00055800 File Offset: 0x00053A00
	// (remove) Token: 0x060011D9 RID: 4569 RVA: 0x00055838 File Offset: 0x00053A38
	public event Action EnterStateCancelAim;

	// Token: 0x060011DA RID: 4570 RVA: 0x0005586D File Offset: 0x00053A6D
	protected IEnumerator CancelAimStateCoroutine()
	{
		this.CurrentState = LocomotionTeleport.States.CancelAim;
		if (this.EnterStateCancelAim != null)
		{
			this.EnterStateCancelAim();
		}
		this.DeactivateDestination();
		yield return null;
		base.StartCoroutine(this.ReadyStateCoroutine());
		yield break;
	}

	// Token: 0x14000037 RID: 55
	// (add) Token: 0x060011DB RID: 4571 RVA: 0x0005587C File Offset: 0x00053A7C
	// (remove) Token: 0x060011DC RID: 4572 RVA: 0x000558B4 File Offset: 0x00053AB4
	public event Action EnterStatePreTeleport;

	// Token: 0x060011DD RID: 4573 RVA: 0x000558E9 File Offset: 0x00053AE9
	protected IEnumerator PreTeleportStateCoroutine()
	{
		this.CurrentState = LocomotionTeleport.States.PreTeleport;
		this.EnableMotion(this.EnableMovementDuringPreTeleport, this.EnableRotationDuringPreTeleport);
		if (this.EnterStatePreTeleport != null)
		{
			this.EnterStatePreTeleport();
		}
		while (this.CurrentIntention == LocomotionTeleport.TeleportIntentions.PreTeleport || this.IsPreTeleportRequested)
		{
			yield return null;
		}
		if (this._teleportDestination.IsValidDestination)
		{
			base.StartCoroutine(this.TeleportingStateCoroutine());
		}
		else
		{
			base.StartCoroutine(this.CancelTeleportStateCoroutine());
		}
		yield break;
	}

	// Token: 0x14000038 RID: 56
	// (add) Token: 0x060011DE RID: 4574 RVA: 0x000558F8 File Offset: 0x00053AF8
	// (remove) Token: 0x060011DF RID: 4575 RVA: 0x00055930 File Offset: 0x00053B30
	public event Action EnterStateCancelTeleport;

	// Token: 0x060011E0 RID: 4576 RVA: 0x00055965 File Offset: 0x00053B65
	protected IEnumerator CancelTeleportStateCoroutine()
	{
		this.CurrentState = LocomotionTeleport.States.CancelTeleport;
		if (this.EnterStateCancelTeleport != null)
		{
			this.EnterStateCancelTeleport();
		}
		this.DeactivateDestination();
		yield return null;
		base.StartCoroutine(this.ReadyStateCoroutine());
		yield break;
	}

	// Token: 0x14000039 RID: 57
	// (add) Token: 0x060011E1 RID: 4577 RVA: 0x00055974 File Offset: 0x00053B74
	// (remove) Token: 0x060011E2 RID: 4578 RVA: 0x000559AC File Offset: 0x00053BAC
	public event Action EnterStateTeleporting;

	// Token: 0x060011E3 RID: 4579 RVA: 0x000559E1 File Offset: 0x00053BE1
	protected IEnumerator TeleportingStateCoroutine()
	{
		this.CurrentState = LocomotionTeleport.States.Teleporting;
		this.EnableMotion(false, false);
		if (this.EnterStateTeleporting != null)
		{
			this.EnterStateTeleporting();
		}
		while (this.IsTransitioning)
		{
			yield return null;
		}
		yield return null;
		base.StartCoroutine(this.PostTeleportStateCoroutine());
		yield break;
	}

	// Token: 0x1400003A RID: 58
	// (add) Token: 0x060011E4 RID: 4580 RVA: 0x000559F0 File Offset: 0x00053BF0
	// (remove) Token: 0x060011E5 RID: 4581 RVA: 0x00055A28 File Offset: 0x00053C28
	public event Action EnterStatePostTeleport;

	// Token: 0x060011E6 RID: 4582 RVA: 0x00055A5D File Offset: 0x00053C5D
	protected IEnumerator PostTeleportStateCoroutine()
	{
		this.CurrentState = LocomotionTeleport.States.PostTeleport;
		this.EnableMotion(this.EnableMovementDuringPostTeleport, this.EnableRotationDuringPostTeleport);
		if (this.EnterStatePostTeleport != null)
		{
			this.EnterStatePostTeleport();
		}
		while (this.IsPostTeleportRequested)
		{
			yield return null;
		}
		this.DeactivateDestination();
		yield return null;
		base.StartCoroutine(this.ReadyStateCoroutine());
		yield break;
	}

	// Token: 0x1400003B RID: 59
	// (add) Token: 0x060011E7 RID: 4583 RVA: 0x00055A6C File Offset: 0x00053C6C
	// (remove) Token: 0x060011E8 RID: 4584 RVA: 0x00055AA4 File Offset: 0x00053CA4
	public event Action<Transform, Vector3, Quaternion> Teleported;

	// Token: 0x060011E9 RID: 4585 RVA: 0x00055ADC File Offset: 0x00053CDC
	public void DoTeleport()
	{
		CapsuleCollider characterController = this.LocomotionController.CharacterController;
		Transform transform = characterController.transform;
		Vector3 position = this._teleportDestination.OrientationIndicator.position;
		position.y += characterController.height * 0.5f;
		Quaternion landingRotation = this._teleportDestination.LandingRotation;
		if (this.Teleported != null)
		{
			this.Teleported(transform, position, landingRotation);
		}
		transform.position = position;
		transform.rotation = landingRotation;
	}

	// Token: 0x060011EA RID: 4586 RVA: 0x00055B54 File Offset: 0x00053D54
	public Vector3 GetCharacterPosition()
	{
		return this.LocomotionController.CharacterController.transform.position;
	}

	// Token: 0x060011EB RID: 4587 RVA: 0x00055B6C File Offset: 0x00053D6C
	public Quaternion GetHeadRotationY()
	{
		Quaternion quaternion = Quaternion.identity;
		InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(XRNode.Head);
		if (deviceAtXRNode.isValid)
		{
			deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceRotation, out quaternion);
		}
		Vector3 eulerAngles = quaternion.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		quaternion = Quaternion.Euler(eulerAngles);
		return quaternion;
	}

	// Token: 0x060011EC RID: 4588 RVA: 0x00055BC8 File Offset: 0x00053DC8
	public void DoWarp(Vector3 startPos, float positionPercent)
	{
		Vector3 position = this._teleportDestination.OrientationIndicator.position;
		position.y += this.LocomotionController.CharacterController.height / 2f;
		Transform transform = this.LocomotionController.CharacterController.transform;
		Vector3 vector = Vector3.Lerp(startPos, position, positionPercent);
		transform.position = vector;
	}

	// Token: 0x040013EE RID: 5102
	[Tooltip("Allow linear movement prior to the teleport system being activated.")]
	public bool EnableMovementDuringReady = true;

	// Token: 0x040013EF RID: 5103
	[Tooltip("Allow linear movement while the teleport system is in the process of aiming for a teleport target.")]
	public bool EnableMovementDuringAim = true;

	// Token: 0x040013F0 RID: 5104
	[Tooltip("Allow linear movement while the teleport system is in the process of configuring the landing orientation.")]
	public bool EnableMovementDuringPreTeleport = true;

	// Token: 0x040013F1 RID: 5105
	[Tooltip("Allow linear movement after the teleport has occurred but before the system has returned to the ready state.")]
	public bool EnableMovementDuringPostTeleport = true;

	// Token: 0x040013F2 RID: 5106
	[Tooltip("Allow rotation prior to the teleport system being activated.")]
	public bool EnableRotationDuringReady = true;

	// Token: 0x040013F3 RID: 5107
	[Tooltip("Allow rotation while the teleport system is in the process of aiming for a teleport target.")]
	public bool EnableRotationDuringAim = true;

	// Token: 0x040013F4 RID: 5108
	[Tooltip("Allow rotation while the teleport system is in the process of configuring the landing orientation.")]
	public bool EnableRotationDuringPreTeleport = true;

	// Token: 0x040013F5 RID: 5109
	[Tooltip("Allow rotation after the teleport has occurred but before the system has returned to the ready state.")]
	public bool EnableRotationDuringPostTeleport = true;

	// Token: 0x040013F7 RID: 5111
	[NonSerialized]
	public TeleportAimHandler AimHandler;

	// Token: 0x040013F8 RID: 5112
	[Tooltip("This prefab will be instantiated as needed and updated to match the current aim target.")]
	public TeleportDestination TeleportDestinationPrefab;

	// Token: 0x040013F9 RID: 5113
	[Tooltip("TeleportDestinationPrefab will be instantiated into this layer.")]
	public int TeleportDestinationLayer;

	// Token: 0x040013FB RID: 5115
	[NonSerialized]
	public TeleportInputHandler InputHandler;

	// Token: 0x040013FC RID: 5116
	[NonSerialized]
	public LocomotionTeleport.TeleportIntentions CurrentIntention;

	// Token: 0x040013FD RID: 5117
	[NonSerialized]
	public bool IsPreTeleportRequested;

	// Token: 0x040013FE RID: 5118
	[NonSerialized]
	public bool IsTransitioning;

	// Token: 0x040013FF RID: 5119
	[NonSerialized]
	public bool IsPostTeleportRequested;

	// Token: 0x04001400 RID: 5120
	private TeleportDestination _teleportDestination;

	// Token: 0x04001402 RID: 5122
	[Tooltip("When aiming at possible destinations, the aim collision type determines which shape to use for collision tests.")]
	public LocomotionTeleport.AimCollisionTypes AimCollisionType;

	// Token: 0x04001403 RID: 5123
	[Tooltip("Use the character collision radius/height/skinwidth for sphere/capsule collision tests.")]
	public bool UseCharacterCollisionData;

	// Token: 0x04001404 RID: 5124
	[Tooltip("Radius of the sphere or capsule used for collision testing when aiming to possible teleport destinations. Ignored if UseCharacterCollisionData is true.")]
	public float AimCollisionRadius;

	// Token: 0x04001405 RID: 5125
	[Tooltip("Height of the capsule used for collision testing when aiming to possible teleport destinations. Ignored if UseCharacterCollisionData is true.")]
	public float AimCollisionHeight;

	// Token: 0x020002E6 RID: 742
	public enum States
	{
		// Token: 0x04001411 RID: 5137
		Ready,
		// Token: 0x04001412 RID: 5138
		Aim,
		// Token: 0x04001413 RID: 5139
		CancelAim,
		// Token: 0x04001414 RID: 5140
		PreTeleport,
		// Token: 0x04001415 RID: 5141
		CancelTeleport,
		// Token: 0x04001416 RID: 5142
		Teleporting,
		// Token: 0x04001417 RID: 5143
		PostTeleport
	}

	// Token: 0x020002E7 RID: 743
	public enum TeleportIntentions
	{
		// Token: 0x04001419 RID: 5145
		None,
		// Token: 0x0400141A RID: 5146
		Aim,
		// Token: 0x0400141B RID: 5147
		PreTeleport,
		// Token: 0x0400141C RID: 5148
		Teleport
	}

	// Token: 0x020002E8 RID: 744
	public enum AimCollisionTypes
	{
		// Token: 0x0400141E RID: 5150
		Point,
		// Token: 0x0400141F RID: 5151
		Sphere,
		// Token: 0x04001420 RID: 5152
		Capsule
	}

	// Token: 0x020002E9 RID: 745
	public class AimData
	{
		// Token: 0x060011EE RID: 4590 RVA: 0x00055C66 File Offset: 0x00053E66
		public AimData()
		{
			this.Points = new List<Vector3>();
		}

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x060011EF RID: 4591 RVA: 0x00055C79 File Offset: 0x00053E79
		// (set) Token: 0x060011F0 RID: 4592 RVA: 0x00055C81 File Offset: 0x00053E81
		public List<Vector3> Points { get; private set; }

		// Token: 0x060011F1 RID: 4593 RVA: 0x00055C8A File Offset: 0x00053E8A
		public void Reset()
		{
			this.Points.Clear();
			this.TargetValid = false;
			this.Destination = null;
		}

		// Token: 0x04001421 RID: 5153
		public RaycastHit TargetHitInfo;

		// Token: 0x04001422 RID: 5154
		public bool TargetValid;

		// Token: 0x04001423 RID: 5155
		public Vector3? Destination;

		// Token: 0x04001424 RID: 5156
		public float Radius;
	}
}
