using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020000E5 RID: 229
public class GrowingSnowballThrowable : SnowballThrowable
{
	// Token: 0x17000071 RID: 113
	// (get) Token: 0x060005B7 RID: 1463 RVA: 0x00020DB6 File Offset: 0x0001EFB6
	public int SizeLevel
	{
		get
		{
			return this.sizeLevel;
		}
	}

	// Token: 0x17000072 RID: 114
	// (get) Token: 0x060005B8 RID: 1464 RVA: 0x00020DBE File Offset: 0x0001EFBE
	public int MaxSizeLevel
	{
		get
		{
			return Mathf.Max(this.snowballSizeLevels.Count - 1, 0);
		}
	}

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x060005B9 RID: 1465 RVA: 0x00020DD4 File Offset: 0x0001EFD4
	public float CurrentSnowballRadius
	{
		get
		{
			if (this.snowballSizeLevels.Count > 0 && this.sizeLevel > -1 && this.sizeLevel < this.snowballSizeLevels.Count)
			{
				return this.snowballSizeLevels[this.sizeLevel].snowballScale * this.modelRadius * base.transform.lossyScale.x;
			}
			return this.modelRadius * base.transform.lossyScale.x;
		}
	}

	// Token: 0x060005BA RID: 1466 RVA: 0x00020E54 File Offset: 0x0001F054
	protected override void Awake()
	{
		base.Awake();
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnMultiplayerStarted += this.StartedMultiplayerSession;
		}
		else
		{
			Debug.LogError("NetworkSystem.Instance was null in SnowballThrowable Awake");
		}
		VRRigCache.OnRigActivated += this.VRRigActivated;
		VRRigCache.OnRigDeactivated += this.VRRigDeactivated;
	}

	// Token: 0x060005BB RID: 1467 RVA: 0x00020EB8 File Offset: 0x0001F0B8
	public override void OnEnable()
	{
		base.OnEnable();
		this.snowballModelParentTransform.localPosition = this.modelParentOffset;
		this.snowballModelTransform.localPosition = this.modelOffset;
		this.otherHandSnowball = (this.isLeftHanded ? (EquipmentInteractor.instance.rightHandHeldEquipment as GrowingSnowballThrowable) : (EquipmentInteractor.instance.leftHandHeldEquipment as GrowingSnowballThrowable));
		if (this.resetSizeOnNextEnable)
		{
			this.SetSizeLevelLocal(0);
		}
		this.CreatePhotonEventsIfNull();
	}

	// Token: 0x060005BC RID: 1468 RVA: 0x00020F34 File Offset: 0x0001F134
	public override void OnDisable()
	{
		base.OnDisable();
		this.resetSizeOnNextEnable = true;
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x00020F43 File Offset: 0x0001F143
	protected override void OnDestroy()
	{
		this.DestroyPhotonEvents();
	}

	// Token: 0x060005BE RID: 1470 RVA: 0x00020F4C File Offset: 0x0001F14C
	private void VRRigActivated(RigContainer rigContainer)
	{
		this.targetRig = base.GetComponentInParent<VRRig>(true);
		this.isOfflineRig = this.targetRig != null && this.targetRig.isOfflineVRRig;
		if (rigContainer.Rig == this.targetRig)
		{
			this.CreatePhotonEventsIfNull();
		}
	}

	// Token: 0x060005BF RID: 1471 RVA: 0x00020FA1 File Offset: 0x0001F1A1
	private void VRRigDeactivated(RigContainer rigContainer)
	{
		if (rigContainer.Rig == this.targetRig)
		{
			this.DestroyPhotonEvents();
		}
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x00020FBC File Offset: 0x0001F1BC
	private void StartedMultiplayerSession()
	{
		this.targetRig = base.GetComponentInParent<VRRig>(true);
		this.isOfflineRig = this.targetRig != null && this.targetRig.isOfflineVRRig;
		if (this.isOfflineRig)
		{
			this.DestroyPhotonEvents();
			this.CreatePhotonEventsIfNull();
		}
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x0002100C File Offset: 0x0001F20C
	private void CreatePhotonEventsIfNull()
	{
		if (this.targetRig == null)
		{
			this.targetRig = base.GetComponentInParent<VRRig>(true);
			this.isOfflineRig = this.targetRig != null && this.targetRig.isOfflineVRRig;
		}
		if (this.targetRig == null || this.targetRig.netView == null)
		{
			return;
		}
		if (this.changeSizeEvent == null)
		{
			"SnowballThrowable" + (this.isLeftHanded ? "ChangeSizeEventLeft" : "ChangeSizeEventRight") + this.targetRig.netView.ViewID.ToString();
			int num = StaticHash.Compute("SnowballThrowable", this.isLeftHanded ? "ChangeSizeEventLeft" : "ChangeSizeEventRight", this.targetRig.netView.ViewID.ToString());
			this.changeSizeEvent = new PhotonEvent(num);
			this.changeSizeEvent.reliable = true;
			this.changeSizeEvent += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.ChangeSizeEventReceiver);
		}
		if (this.snowballThrowEvent == null)
		{
			"SnowballThrowable" + (this.isLeftHanded ? "SnowballThrowEventLeft" : "SnowballThrowEventRight") + this.targetRig.netView.ViewID.ToString();
			int num2 = StaticHash.Compute("SnowballThrowable", this.isLeftHanded ? "SnowballThrowEventLeft" : "SnowballThrowEventRight", this.targetRig.netView.ViewID.ToString());
			this.snowballThrowEvent = new PhotonEvent(num2);
			this.snowballThrowEvent.reliable = true;
			this.snowballThrowEvent += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.SnowballThrowEventReceiver);
		}
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x000211E0 File Offset: 0x0001F3E0
	private void DestroyPhotonEvents()
	{
		if (this.changeSizeEvent != null)
		{
			this.changeSizeEvent -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.ChangeSizeEventReceiver);
			this.changeSizeEvent.Dispose();
			this.changeSizeEvent = null;
		}
		if (this.snowballThrowEvent != null)
		{
			this.snowballThrowEvent -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.SnowballThrowEventReceiver);
			this.snowballThrowEvent.Dispose();
			this.snowballThrowEvent = null;
		}
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x00021267 File Offset: 0x0001F467
	public void IncreaseSize(int increase)
	{
		this.SetSizeLevelAuthority(this.sizeLevel + increase);
	}

	// Token: 0x060005C4 RID: 1476 RVA: 0x00021278 File Offset: 0x0001F478
	private void SetSizeLevelAuthority(int sizeLevel)
	{
		if (this.targetRig != null && this.targetRig.creator != null && this.targetRig.creator.IsLocal)
		{
			int validSizeLevel = this.GetValidSizeLevel(sizeLevel);
			if (validSizeLevel > this.sizeLevel)
			{
				this.sizeIncreaseSoundBankPlayer.Play();
			}
			this.SetSizeLevelLocal(validSizeLevel);
			PhotonEvent photonEvent = this.changeSizeEvent;
			if (photonEvent == null)
			{
				return;
			}
			photonEvent.RaiseOthers(new object[] { validSizeLevel });
		}
	}

	// Token: 0x060005C5 RID: 1477 RVA: 0x000212F4 File Offset: 0x0001F4F4
	private int GetValidSizeLevel(int inputSizeLevel)
	{
		int num = Mathf.Max(this.snowballSizeLevels.Count - 1, 0);
		return Mathf.Clamp(inputSizeLevel, 0, num);
	}

	// Token: 0x060005C6 RID: 1478 RVA: 0x00021320 File Offset: 0x0001F520
	private void SetSizeLevelLocal(int sizeLevel)
	{
		int validSizeLevel = this.GetValidSizeLevel(sizeLevel);
		if (validSizeLevel >= 0 && validSizeLevel != this.sizeLevel)
		{
			this.sizeLevel = validSizeLevel;
			this.snowballModelParentTransform.localScale = Vector3.one * this.snowballSizeLevels[this.sizeLevel].snowballScale;
		}
	}

	// Token: 0x060005C7 RID: 1479 RVA: 0x00021374 File Offset: 0x0001F574
	private void ChangeSizeEventReceiver(int sender, int receiver, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != receiver)
		{
			return;
		}
		if (args == null || args.Length < 1)
		{
			return;
		}
		int num = ((this.targetRig != null && this.targetRig.gameObject.activeInHierarchy && this.targetRig.netView != null && this.targetRig.netView.Owner != null) ? this.targetRig.netView.Owner.ActorNumber : (-1));
		if (info.senderID != num)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "ChangeSizeEventReceiver");
		int num2 = (int)args[0];
		if (this.GetValidSizeLevel(num2) > this.sizeLevel)
		{
			this.sizeIncreaseSoundBankPlayer.Play();
		}
		this.SetSizeLevelLocal(num2);
		if (!base.gameObject.activeSelf)
		{
			this.resetSizeOnNextEnable = false;
		}
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x00021444 File Offset: 0x0001F644
	private void SnowballThrowEventReceiver(int sender, int receiver, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != receiver)
		{
			return;
		}
		if (args == null || args.Length < 3)
		{
			return;
		}
		if (this.targetRig.IsNull() || !this.targetRig.gameObject.activeSelf)
		{
			return;
		}
		NetPlayer creator = this.targetRig.creator;
		if (info.senderID != this.targetRig.creator.ActorNumber)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "SnowballThrowEventReceiver");
		if (!this.snowballThrowCallLimit.CheckCallTime(Time.time))
		{
			return;
		}
		object obj = args[0];
		if (obj is Vector3)
		{
			Vector3 vector = (Vector3)obj;
			obj = args[1];
			if (obj is Vector3)
			{
				Vector3 vector2 = (Vector3)obj;
				obj = args[2];
				if (obj is int)
				{
					int num = (int)obj;
					Vector3 vector3 = this.targetRig.ClampVelocityRelativeToPlayerSafe(vector2, 50f);
					float x = this.snowballModelTransform.lossyScale.x;
					float num2 = 10000f;
					if (!(in vector).IsValid(in num2) || !this.targetRig.IsPositionInRange(vector, 4f))
					{
						return;
					}
					this.LaunchSnowballRemote(vector, vector3, x, num, info);
					return;
				}
			}
		}
	}

	// Token: 0x060005C9 RID: 1481 RVA: 0x00021564 File Offset: 0x0001F764
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (GrowingSnowballThrowable.twoHandedSnowballGrowing && this.otherHandSnowball != null)
		{
			IHoldableObject holdableObject = (this.isLeftHanded ? EquipmentInteractor.instance.rightHandHeldEquipment : EquipmentInteractor.instance.leftHandHeldEquipment);
			if (holdableObject != null && this.otherHandSnowball != (GrowingSnowballThrowable)holdableObject)
			{
				this.otherHandSnowball = null;
				return;
			}
			float num = this.otherHandSnowball.CurrentSnowballRadius + this.CurrentSnowballRadius;
			if (this.SizeLevel < this.MaxSizeLevel && this.otherHandSnowball.SizeLevel < this.otherHandSnowball.MaxSizeLevel && (this.otherHandSnowball.snowballModelTransform.position - this.snowballModelTransform.position).sqrMagnitude < num * num)
			{
				int num2 = this.SizeLevel - this.otherHandSnowball.SizeLevel;
				float magnitude = this.velocityEstimator.linearVelocity.magnitude;
				float magnitude2 = this.otherHandSnowball.velocityEstimator.linearVelocity.magnitude;
				bool flag;
				if (Mathf.Abs(magnitude - magnitude2) > this.combineBasedOnSpeedThreshold || num2 == 0)
				{
					flag = magnitude > magnitude2;
				}
				else
				{
					flag = num2 < 0;
				}
				if (flag)
				{
					this.otherHandSnowball.IncreaseSize(this.sizeLevel + 1);
					GorillaTagger.Instance.StartVibration(!this.isLeftHanded, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
					base.SetSnowballActiveLocal(false);
					return;
				}
				this.IncreaseSize(this.otherHandSnowball.SizeLevel + 1);
				GorillaTagger.Instance.StartVibration(this.isLeftHanded, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				this.otherHandSnowball.SetSnowballActiveLocal(false);
			}
		}
	}

	// Token: 0x060005CA RID: 1482 RVA: 0x00021752 File Offset: 0x0001F952
	protected override void OnSnowballRelease()
	{
		if (base.isActiveAndEnabled)
		{
			this.PerformSnowballThrowAuthority();
		}
	}

	// Token: 0x060005CB RID: 1483 RVA: 0x00021764 File Offset: 0x0001F964
	protected override void PerformSnowballThrowAuthority()
	{
		if (!(this.targetRig != null) || this.targetRig.creator == null || !this.targetRig.creator.IsLocal)
		{
			return;
		}
		Vector3 vector = Vector3.zero;
		Rigidbody component = GorillaTagger.Instance.GetComponent<Rigidbody>();
		if (component != null)
		{
			vector = component.velocity;
		}
		Vector3 vector2 = this.velocityEstimator.linearVelocity - vector;
		float magnitude = vector2.magnitude;
		if (magnitude > 0.001f)
		{
			float num = Mathf.Clamp(magnitude * this.linSpeedMultiplier, 0f, this.maxLinSpeed);
			vector2 *= num / magnitude;
		}
		Vector3 vector3 = vector2 + vector;
		this.targetRig.GetThrowableProjectileColor(this.isLeftHanded);
		Transform transform = this.snowballModelTransform;
		Vector3 position = transform.position;
		float x = transform.lossyScale.x;
		SlingshotProjectile slingshotProjectile = this.LaunchSnowballLocal(position, vector3, x);
		base.SetSnowballActiveLocal(false);
		if (this.randModelIndex > -1 && this.randModelIndex < this.localModels.Count && this.localModels[this.randModelIndex].destroyAfterRelease)
		{
			slingshotProjectile.DestroyAfterRelease();
		}
		PhotonEvent photonEvent = this.snowballThrowEvent;
		if (photonEvent == null)
		{
			return;
		}
		photonEvent.RaiseOthers(new object[] { position, vector3, slingshotProjectile.myProjectileCount });
	}

	// Token: 0x060005CC RID: 1484 RVA: 0x000218C8 File Offset: 0x0001FAC8
	protected virtual SlingshotProjectile LaunchSnowballLocal(Vector3 location, Vector3 velocity, float scale)
	{
		return this.LaunchSnowballLocal(location, velocity, scale, false, Color.white);
	}

	// Token: 0x060005CD RID: 1485 RVA: 0x000218DC File Offset: 0x0001FADC
	protected override SlingshotProjectile LaunchSnowballLocal(Vector3 location, Vector3 velocity, float scale, bool randomizeColour, Color colour)
	{
		SlingshotProjectile slingshotProjectile = this.SpawnGrowingSnowball(ref velocity, scale);
		int num = ProjectileTracker.AddAndIncrementLocalProjectile(slingshotProjectile, velocity, location, scale);
		slingshotProjectile.Launch(location, velocity, NetworkSystem.Instance.LocalPlayer, false, false, num, scale, randomizeColour, colour);
		if (string.IsNullOrEmpty(this.throwEventName))
		{
			PlayerGameEvents.LaunchedProjectile(this.projectilePrefab.name);
		}
		else
		{
			PlayerGameEvents.LaunchedProjectile(this.throwEventName);
		}
		slingshotProjectile.OnImpact += this.OnProjectileImpact;
		return slingshotProjectile;
	}

	// Token: 0x060005CE RID: 1486 RVA: 0x00021953 File Offset: 0x0001FB53
	protected virtual SlingshotProjectile LaunchSnowballRemote(Vector3 location, Vector3 velocity, float scale, int index, PhotonMessageInfoWrapped info)
	{
		return this.LaunchSnowballRemote(location, velocity, scale, index, false, Color.white, info);
	}

	// Token: 0x060005CF RID: 1487 RVA: 0x00021968 File Offset: 0x0001FB68
	protected virtual SlingshotProjectile LaunchSnowballRemote(Vector3 location, Vector3 velocity, float scale, int index, bool randomizeColour, Color colour, PhotonMessageInfoWrapped info)
	{
		SlingshotProjectile slingshotProjectile = this.SpawnGrowingSnowball(ref velocity, scale);
		ProjectileTracker.AddRemotePlayerProjectile(info.Sender, slingshotProjectile, index, info.SentServerTime, velocity, location, scale);
		slingshotProjectile.Launch(location, velocity, info.Sender, false, false, index, scale, randomizeColour, Color.white);
		if (string.IsNullOrEmpty(this.throwEventName))
		{
			PlayerGameEvents.LaunchedProjectile(this.projectilePrefab.name);
		}
		else
		{
			PlayerGameEvents.LaunchedProjectile(this.throwEventName);
		}
		slingshotProjectile.OnImpact += this.OnProjectileImpact;
		return slingshotProjectile;
	}

	// Token: 0x060005D0 RID: 1488 RVA: 0x000219F4 File Offset: 0x0001FBF4
	private SlingshotProjectile SpawnGrowingSnowball(ref Vector3 velocity, float scale)
	{
		SlingshotProjectile component = ObjectPools.instance.Instantiate(this.randomModelSelection ? this.localModels[this.randModelIndex].projectilePrefab : this.projectilePrefab, true).GetComponent<SlingshotProjectile>();
		if (this.snowballSizeLevels.Count > 0 && this.sizeLevel >= 0 && this.sizeLevel < this.snowballSizeLevels.Count)
		{
			float num = scale / this.snowballSizeLevels[this.sizeLevel].snowballScale;
			SlingshotProjectile.AOEKnockbackConfig aoeKnockbackConfig = this.snowballSizeLevels[this.sizeLevel].aoeKnockbackConfig;
			aoeKnockbackConfig.aeoInnerRadius *= num;
			aoeKnockbackConfig.aeoOuterRadius *= num;
			aoeKnockbackConfig.knockbackVelocity *= num;
			aoeKnockbackConfig.impactVelocityThreshold *= num;
			velocity *= this.snowballSizeLevels[this.sizeLevel].throwSpeedMultiplier;
			component.gravityMultiplier = this.snowballSizeLevels[this.sizeLevel].gravityMultiplier;
			component.impactEffectScaleMultiplier = this.snowballSizeLevels[this.sizeLevel].impactEffectScale;
			component.aoeKnockbackConfig = new SlingshotProjectile.AOEKnockbackConfig?(aoeKnockbackConfig);
			component.impactSoundVolumeOverride = new float?(this.snowballSizeLevels[this.sizeLevel].impactSoundVolume);
			component.impactSoundPitchOverride = new float?(this.snowballSizeLevels[this.sizeLevel].impactSoundPitch);
		}
		return component;
	}

	// Token: 0x060005D1 RID: 1489 RVA: 0x00021B7C File Offset: 0x0001FD7C
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!(this.targetRig != null) || this.targetRig.creator == null || !this.targetRig.creator.IsLocal)
		{
			return;
		}
		SnowballThrowable snowballThrowable;
		if (((this.isLeftHanded && grabbingHand == EquipmentInteractor.instance.rightHand && EquipmentInteractor.instance.rightHandHeldEquipment == null) || (!this.isLeftHanded && grabbingHand == EquipmentInteractor.instance.leftHand && EquipmentInteractor.instance.leftHandHeldEquipment == null)) && (this.isLeftHanded ? SnowballMaker.rightHandInstance : SnowballMaker.leftHandInstance).TryCreateSnowball(this.matDataIndexes[0], out snowballThrowable))
		{
			GrowingSnowballThrowable growingSnowballThrowable = snowballThrowable as GrowingSnowballThrowable;
			if (growingSnowballThrowable != null)
			{
				growingSnowballThrowable.IncreaseSize(this.sizeLevel);
				GorillaTagger.Instance.StartVibration(!this.isLeftHanded, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				base.SetSnowballActiveLocal(false);
			}
		}
	}

	// Token: 0x040006BC RID: 1724
	public Transform snowballModelParentTransform;

	// Token: 0x040006BD RID: 1725
	public Transform snowballModelTransform;

	// Token: 0x040006BE RID: 1726
	public Vector3 modelParentOffset = Vector3.zero;

	// Token: 0x040006BF RID: 1727
	public Vector3 modelOffset = Vector3.zero;

	// Token: 0x040006C0 RID: 1728
	public float modelRadius = 0.055f;

	// Token: 0x040006C1 RID: 1729
	[Tooltip("Snowballs will combine into the larger snowball unless they are moving faster than this threshold.Then the faster moving snowball will go in to the more stationary hand")]
	public float combineBasedOnSpeedThreshold = 0.5f;

	// Token: 0x040006C2 RID: 1730
	public SoundBankPlayer sizeIncreaseSoundBankPlayer;

	// Token: 0x040006C3 RID: 1731
	public List<GrowingSnowballThrowable.SizeParameters> snowballSizeLevels = new List<GrowingSnowballThrowable.SizeParameters>();

	// Token: 0x040006C4 RID: 1732
	private int sizeLevel;

	// Token: 0x040006C5 RID: 1733
	private bool resetSizeOnNextEnable;

	// Token: 0x040006C6 RID: 1734
	private PhotonEvent changeSizeEvent;

	// Token: 0x040006C7 RID: 1735
	private PhotonEvent snowballThrowEvent;

	// Token: 0x040006C8 RID: 1736
	private CallLimiterWithCooldown snowballThrowCallLimit = new CallLimiterWithCooldown(10f, 10, 2f);

	// Token: 0x040006C9 RID: 1737
	[HideInInspector]
	public static bool debugDrawAOERange = false;

	// Token: 0x040006CA RID: 1738
	[HideInInspector]
	public static bool twoHandedSnowballGrowing = true;

	// Token: 0x040006CB RID: 1739
	private Queue<GrowingSnowballThrowable.AOERangeDebugDraw> aoeRangeDebugDrawQueue = new Queue<GrowingSnowballThrowable.AOERangeDebugDraw>();

	// Token: 0x040006CC RID: 1740
	private GrowingSnowballThrowable otherHandSnowball;

	// Token: 0x040006CD RID: 1741
	private float debugDrawAOERangeTime = 1.5f;

	// Token: 0x020000E6 RID: 230
	[Serializable]
	public struct SizeParameters
	{
		// Token: 0x040006CE RID: 1742
		public float snowballScale;

		// Token: 0x040006CF RID: 1743
		public float impactEffectScale;

		// Token: 0x040006D0 RID: 1744
		public float impactSoundVolume;

		// Token: 0x040006D1 RID: 1745
		public float impactSoundPitch;

		// Token: 0x040006D2 RID: 1746
		public float throwSpeedMultiplier;

		// Token: 0x040006D3 RID: 1747
		public float gravityMultiplier;

		// Token: 0x040006D4 RID: 1748
		public SlingshotProjectile.AOEKnockbackConfig aoeKnockbackConfig;
	}

	// Token: 0x020000E7 RID: 231
	private struct AOERangeDebugDraw
	{
		// Token: 0x040006D5 RID: 1749
		public float impactTime;

		// Token: 0x040006D6 RID: 1750
		public Vector3 position;

		// Token: 0x040006D7 RID: 1751
		public float innerRadius;

		// Token: 0x040006D8 RID: 1752
		public float outerRadius;
	}
}
