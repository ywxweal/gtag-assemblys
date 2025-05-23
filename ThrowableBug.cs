using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000A09 RID: 2569
public class ThrowableBug : TransferrableObject
{
	// Token: 0x06003D56 RID: 15702 RVA: 0x001232E4 File Offset: 0x001214E4
	protected override void Start()
	{
		base.Start();
		float num = Random.Range(0f, 6.2831855f);
		this.targetVelocity = new Vector3(Mathf.Sin(num) * this.maxNaturalSpeed, 0f, Mathf.Cos(num) * this.maxNaturalSpeed);
		this.currentState = TransferrableObject.PositionState.Dropped;
		this.rayCastNonAllocColliders = new RaycastHit[5];
		this.rayCastNonAllocColliders2 = new RaycastHit[5];
		this.velocityEstimator = base.GetComponent<GorillaVelocityEstimator>();
	}

	// Token: 0x06003D57 RID: 15703 RVA: 0x00123360 File Offset: 0x00121560
	internal override void OnEnable()
	{
		base.OnEnable();
		ThrowableBugBeacon.OnCall += this.ThrowableBugBeacon_OnCall;
		ThrowableBugBeacon.OnDismiss += this.ThrowableBugBeacon_OnDismiss;
		ThrowableBugBeacon.OnLock += this.ThrowableBugBeacon_OnLock;
		ThrowableBugBeacon.OnUnlock += this.ThrowableBugBeacon_OnUnlock;
		ThrowableBugBeacon.OnChangeSpeedMultiplier += this.ThrowableBugBeacon_OnChangeSpeedMultiplier;
	}

	// Token: 0x06003D58 RID: 15704 RVA: 0x001233C8 File Offset: 0x001215C8
	internal override void OnDisable()
	{
		base.OnDisable();
		ThrowableBugBeacon.OnCall -= this.ThrowableBugBeacon_OnCall;
		ThrowableBugBeacon.OnDismiss -= this.ThrowableBugBeacon_OnDismiss;
		ThrowableBugBeacon.OnLock -= this.ThrowableBugBeacon_OnLock;
		ThrowableBugBeacon.OnUnlock -= this.ThrowableBugBeacon_OnUnlock;
		ThrowableBugBeacon.OnChangeSpeedMultiplier -= this.ThrowableBugBeacon_OnChangeSpeedMultiplier;
	}

	// Token: 0x06003D59 RID: 15705 RVA: 0x00123430 File Offset: 0x00121630
	private bool isValid(ThrowableBugBeacon tbb)
	{
		return tbb.BugName == this.bugName && (tbb.Range <= 0f || Vector3.Distance(tbb.transform.position, base.transform.position) <= tbb.Range);
	}

	// Token: 0x06003D5A RID: 15706 RVA: 0x00123482 File Offset: 0x00121682
	private void ThrowableBugBeacon_OnCall(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.reliableState.travelingDirection = tbb.transform.position - base.transform.position;
		}
	}

	// Token: 0x06003D5B RID: 15707 RVA: 0x001234B4 File Offset: 0x001216B4
	private void ThrowableBugBeacon_OnLock(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.reliableState.travelingDirection = tbb.transform.position - base.transform.position;
			this.lockedTarget = tbb.transform;
			this.locked = true;
		}
	}

	// Token: 0x06003D5C RID: 15708 RVA: 0x00123503 File Offset: 0x00121703
	private void ThrowableBugBeacon_OnDismiss(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.reliableState.travelingDirection = base.transform.position - tbb.transform.position;
			this.locked = false;
		}
	}

	// Token: 0x06003D5D RID: 15709 RVA: 0x0012353B File Offset: 0x0012173B
	private void ThrowableBugBeacon_OnUnlock(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.locked = false;
		}
	}

	// Token: 0x06003D5E RID: 15710 RVA: 0x0012354D File Offset: 0x0012174D
	private void ThrowableBugBeacon_OnChangeSpeedMultiplier(ThrowableBugBeacon tbb, float f)
	{
		if (this.isValid(tbb))
		{
			this.speedMultiplier = f;
		}
	}

	// Token: 0x06003D5F RID: 15711 RVA: 0x00047642 File Offset: 0x00045842
	public override bool ShouldBeKinematic()
	{
		return true;
	}

	// Token: 0x06003D60 RID: 15712 RVA: 0x00123560 File Offset: 0x00121760
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand || this.currentState == TransferrableObject.PositionState.InRightHand;
		if (this.animator.enabled)
		{
			this.animator.SetBool(ThrowableBug._g_IsHeld, flag);
		}
		if (!this.audioSource)
		{
			return;
		}
		switch (this.currentAudioState)
		{
		case ThrowableBug.AudioState.JustGrabbed:
			if (!flag)
			{
				this.currentAudioState = ThrowableBug.AudioState.JustReleased;
				return;
			}
			if (this.grabBugAudioClip && this.audioSource.clip != this.grabBugAudioClip)
			{
				this.audioSource.clip = this.grabBugAudioClip;
				this.audioSource.time = 0f;
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
					return;
				}
			}
			else if (!this.audioSource.isPlaying)
			{
				this.currentAudioState = ThrowableBug.AudioState.ContinuallyGrabbed;
				return;
			}
			break;
		case ThrowableBug.AudioState.ContinuallyGrabbed:
			if (!flag)
			{
				this.currentAudioState = ThrowableBug.AudioState.JustReleased;
				return;
			}
			break;
		case ThrowableBug.AudioState.JustReleased:
			if (!flag)
			{
				if (this.releaseBugAudioClip && this.audioSource.clip != this.releaseBugAudioClip)
				{
					this.audioSource.clip = this.releaseBugAudioClip;
					this.audioSource.time = 0f;
					if (this.audioSource.isActiveAndEnabled)
					{
						this.audioSource.GTPlay();
						return;
					}
				}
				else if (!this.audioSource.isPlaying)
				{
					this.currentAudioState = ThrowableBug.AudioState.NotHeld;
					return;
				}
			}
			else
			{
				this.currentAudioState = ThrowableBug.AudioState.JustGrabbed;
			}
			break;
		case ThrowableBug.AudioState.NotHeld:
			if (flag)
			{
				this.currentAudioState = ThrowableBug.AudioState.JustGrabbed;
				return;
			}
			if (this.flyingBugAudioClip && !this.audioSource.isPlaying)
			{
				this.audioSource.clip = this.flyingBugAudioClip;
				this.audioSource.time = 0f;
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
					return;
				}
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06003D61 RID: 15713 RVA: 0x0012374C File Offset: 0x0012194C
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!this.reliableState)
		{
			return;
		}
		if (this.currentState != TransferrableObject.PositionState.InLeftHand)
		{
			bool flag = this.currentState == TransferrableObject.PositionState.InRightHand;
		}
		if (this.currentState == TransferrableObject.PositionState.Dropped)
		{
			if (this.locked && Vector3.Distance(this.lockedTarget.position, base.transform.position) > 0.1f)
			{
				this.reliableState.travelingDirection = this.lockedTarget.position - base.transform.position;
			}
			if (this.slowingDownProgress < 1f)
			{
				this.slowingDownProgress += this.slowdownAcceleration * Time.deltaTime;
				float num = Mathf.SmoothStep(0f, 1f, this.slowingDownProgress);
				this.reliableState.travelingDirection = Vector3.Slerp(this.thrownVeloicity, this.targetVelocity, num);
			}
			else
			{
				this.reliableState.travelingDirection = this.reliableState.travelingDirection.normalized * this.maxNaturalSpeed;
			}
			this.bobingFrequency = (this.shouldRandomizeFrequency ? this.RandomizeBobingFrequency() : this.bobbingDefaultFrequency);
			float num2 = this.bobingState + this.bobingSpeed * Time.deltaTime;
			float num3 = Mathf.Sin(num2 / this.bobingFrequency) - Mathf.Sin(this.bobingState / this.bobingFrequency);
			Vector3 vector = Vector3.up * (num3 * this.bobMagnintude);
			this.bobingState = num2;
			if (this.bobingState > 6.2831855f)
			{
				this.bobingState -= 6.2831855f;
			}
			vector += this.reliableState.travelingDirection * Time.deltaTime;
			int num4 = Physics.SphereCastNonAlloc(base.transform.position, this.collisionHitRadius, vector.normalized, this.rayCastNonAllocColliders, vector.magnitude, this.collisionCheckMask);
			float num5 = this.maximumHeightOffOfTheGroundBeforeStartingDescent;
			if (this.isTooHighTravelingDown)
			{
				num5 = this.minimumHeightOffOfTheGroundBeforeStoppingDescent;
			}
			float num6 = this.minimumHeightOffOfTheGroundBeforeStartingAscent;
			if (this.isTooLowTravelingUp)
			{
				num6 = this.maximumHeightOffOfTheGroundBeforeStoppingAscent;
			}
			if (Physics.RaycastNonAlloc(base.transform.position, Vector3.down, this.rayCastNonAllocColliders2, num5, this.collisionCheckMask) > 0)
			{
				this.isTooHighTravelingDown = false;
				if (this.descentSlerp > 0f)
				{
					this.descentSlerp = Mathf.Clamp01(this.descentSlerp - this.descentSlerpRate * Time.deltaTime);
				}
				RaycastHit raycastHit = this.rayCastNonAllocColliders2[0];
				this.isTooLowTravelingUp = raycastHit.distance < num6;
				if (this.isTooLowTravelingUp)
				{
					if (this.ascentSlerp < 1f)
					{
						this.ascentSlerp = Mathf.Clamp01(this.ascentSlerp + this.ascentSlerpRate * Time.deltaTime);
					}
				}
				else if (this.ascentSlerp > 0f)
				{
					this.ascentSlerp = Mathf.Clamp01(this.ascentSlerp - this.ascentSlerpRate * Time.deltaTime);
				}
			}
			else
			{
				this.isTooHighTravelingDown = true;
				if (this.descentSlerp < 1f)
				{
					this.descentSlerp = Mathf.Clamp01(this.descentSlerp + this.descentSlerpRate * Time.deltaTime);
				}
			}
			vector += Time.deltaTime * Mathf.SmoothStep(0f, 1f, this.descentSlerp) * this.descentRate * Vector3.down;
			vector += Time.deltaTime * Mathf.SmoothStep(0f, 1f, this.ascentSlerp) * this.ascentRate * Vector3.up;
			float num7;
			Vector3 vector2;
			Quaternion.FromToRotation(base.transform.rotation * Vector3.up, Quaternion.identity * Vector3.up).ToAngleAxis(out num7, out vector2);
			Quaternion quaternion = Quaternion.AngleAxis(num7 * 0.02f, vector2);
			float num8;
			Vector3 vector3;
			Quaternion.FromToRotation(base.transform.rotation * Vector3.forward, this.reliableState.travelingDirection.normalized).ToAngleAxis(out num8, out vector3);
			Quaternion quaternion2 = Quaternion.AngleAxis(num8 * 0.005f, vector3);
			quaternion = quaternion2 * quaternion;
			vector = quaternion * quaternion * quaternion * quaternion * vector;
			vector *= this.speedMultiplier;
			if (this.speedMultiplier != 1f)
			{
				this.speedMultiplier = Mathf.MoveTowards(this.speedMultiplier, 1f, Time.deltaTime);
			}
			if (num4 > 0)
			{
				Vector3 normal = this.rayCastNonAllocColliders[0].normal;
				this.reliableState.travelingDirection = Vector3.Reflect(this.reliableState.travelingDirection, normal).x0z();
				base.transform.position += Vector3.Reflect(vector, normal);
				this.thrownVeloicity = Vector3.Reflect(this.thrownVeloicity, normal);
				this.targetVelocity = Vector3.Reflect(this.targetVelocity, normal).x0z();
			}
			else
			{
				base.transform.position += vector;
			}
			this.bugRotationalVelocity = quaternion * this.bugRotationalVelocity;
			float num9;
			Vector3 vector4;
			this.bugRotationalVelocity.ToAngleAxis(out num9, out vector4);
			this.bugRotationalVelocity = Quaternion.AngleAxis(num9 * 0.9f, vector4);
			base.transform.rotation = this.bugRotationalVelocity * base.transform.rotation;
		}
	}

	// Token: 0x06003D62 RID: 15714 RVA: 0x00123CC9 File Offset: 0x00121EC9
	private float RandomizeBobingFrequency()
	{
		return Random.Range(this.minRandFrequency, this.maxRandFrequency);
	}

	// Token: 0x06003D63 RID: 15715 RVA: 0x00123CDC File Offset: 0x00121EDC
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		this.slowingDownProgress = 0f;
		Vector3 linearVelocity = this.velocityEstimator.linearVelocity;
		this.thrownVeloicity = linearVelocity;
		this.reliableState.travelingDirection = linearVelocity;
		this.bugRotationalVelocity = Quaternion.Euler(this.velocityEstimator.angularVelocity);
		this.startingSpeed = linearVelocity.magnitude;
		Vector3 normalized = this.reliableState.travelingDirection.x0z().normalized;
		this.targetVelocity = normalized * this.maxNaturalSpeed;
		return true;
	}

	// Token: 0x06003D64 RID: 15716 RVA: 0x00123D6E File Offset: 0x00121F6E
	public void OnCollisionEnter(Collision collision)
	{
		this.reliableState.travelingDirection *= -1f;
	}

	// Token: 0x06003D65 RID: 15717 RVA: 0x00123D8C File Offset: 0x00121F8C
	private void Update()
	{
		if (this.updateMultiplier > 0)
		{
			for (int i = 0; i < this.updateMultiplier; i++)
			{
				this.LateUpdateLocal();
			}
		}
	}

	// Token: 0x0400411C RID: 16668
	public ThrowableBugReliableState reliableState;

	// Token: 0x0400411D RID: 16669
	public float slowingDownProgress;

	// Token: 0x0400411E RID: 16670
	public float startingSpeed;

	// Token: 0x0400411F RID: 16671
	public float bobingSpeed = 1f;

	// Token: 0x04004120 RID: 16672
	public float bobMagnintude = 0.1f;

	// Token: 0x04004121 RID: 16673
	public bool shouldRandomizeFrequency;

	// Token: 0x04004122 RID: 16674
	public float minRandFrequency = 0.008f;

	// Token: 0x04004123 RID: 16675
	public float maxRandFrequency = 1f;

	// Token: 0x04004124 RID: 16676
	public float bobingFrequency = 1f;

	// Token: 0x04004125 RID: 16677
	public float bobingState;

	// Token: 0x04004126 RID: 16678
	public float thrownYVelocity;

	// Token: 0x04004127 RID: 16679
	public float collisionHitRadius;

	// Token: 0x04004128 RID: 16680
	public LayerMask collisionCheckMask;

	// Token: 0x04004129 RID: 16681
	public Vector3 thrownVeloicity;

	// Token: 0x0400412A RID: 16682
	public Vector3 targetVelocity;

	// Token: 0x0400412B RID: 16683
	public Quaternion bugRotationalVelocity;

	// Token: 0x0400412C RID: 16684
	private RaycastHit[] rayCastNonAllocColliders;

	// Token: 0x0400412D RID: 16685
	private RaycastHit[] rayCastNonAllocColliders2;

	// Token: 0x0400412E RID: 16686
	public VRRig followingRig;

	// Token: 0x0400412F RID: 16687
	public bool isTooHighTravelingDown;

	// Token: 0x04004130 RID: 16688
	public float descentSlerp;

	// Token: 0x04004131 RID: 16689
	public float ascentSlerp;

	// Token: 0x04004132 RID: 16690
	public float maxNaturalSpeed;

	// Token: 0x04004133 RID: 16691
	public float slowdownAcceleration;

	// Token: 0x04004134 RID: 16692
	public float maximumHeightOffOfTheGroundBeforeStartingDescent = 5f;

	// Token: 0x04004135 RID: 16693
	public float minimumHeightOffOfTheGroundBeforeStoppingDescent = 3f;

	// Token: 0x04004136 RID: 16694
	public float descentRate = 0.2f;

	// Token: 0x04004137 RID: 16695
	public float descentSlerpRate = 0.2f;

	// Token: 0x04004138 RID: 16696
	public float minimumHeightOffOfTheGroundBeforeStartingAscent = 0.5f;

	// Token: 0x04004139 RID: 16697
	public float maximumHeightOffOfTheGroundBeforeStoppingAscent = 0.75f;

	// Token: 0x0400413A RID: 16698
	public float ascentRate = 0.4f;

	// Token: 0x0400413B RID: 16699
	public float ascentSlerpRate = 1f;

	// Token: 0x0400413C RID: 16700
	private bool isTooLowTravelingUp;

	// Token: 0x0400413D RID: 16701
	public Animator animator;

	// Token: 0x0400413E RID: 16702
	[FormerlySerializedAs("grabBugAudioSource")]
	public AudioClip grabBugAudioClip;

	// Token: 0x0400413F RID: 16703
	[FormerlySerializedAs("releaseBugAudioSource")]
	public AudioClip releaseBugAudioClip;

	// Token: 0x04004140 RID: 16704
	[FormerlySerializedAs("flyingBugAudioSource")]
	public AudioClip flyingBugAudioClip;

	// Token: 0x04004141 RID: 16705
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04004142 RID: 16706
	private float bobbingDefaultFrequency = 1f;

	// Token: 0x04004143 RID: 16707
	public int updateMultiplier;

	// Token: 0x04004144 RID: 16708
	private ThrowableBug.AudioState currentAudioState;

	// Token: 0x04004145 RID: 16709
	private float speedMultiplier = 1f;

	// Token: 0x04004146 RID: 16710
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04004147 RID: 16711
	[SerializeField]
	private ThrowableBug.BugName bugName;

	// Token: 0x04004148 RID: 16712
	private Transform lockedTarget;

	// Token: 0x04004149 RID: 16713
	private bool locked;

	// Token: 0x0400414A RID: 16714
	private static readonly int _g_IsHeld = Animator.StringToHash("isHeld");

	// Token: 0x02000A0A RID: 2570
	public enum BugName
	{
		// Token: 0x0400414C RID: 16716
		NONE,
		// Token: 0x0400414D RID: 16717
		DougTheBug,
		// Token: 0x0400414E RID: 16718
		MattTheBat
	}

	// Token: 0x02000A0B RID: 2571
	private enum AudioState
	{
		// Token: 0x04004150 RID: 16720
		JustGrabbed,
		// Token: 0x04004151 RID: 16721
		ContinuallyGrabbed,
		// Token: 0x04004152 RID: 16722
		JustReleased,
		// Token: 0x04004153 RID: 16723
		NotHeld
	}
}
