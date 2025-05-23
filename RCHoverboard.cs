using System;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaTag.Cosmetics;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200010D RID: 269
public class RCHoverboard : RCVehicle
{
	// Token: 0x1700009D RID: 157
	// (get) Token: 0x060006E1 RID: 1761 RVA: 0x00026D5B File Offset: 0x00024F5B
	// (set) Token: 0x060006E2 RID: 1762 RVA: 0x00026D63 File Offset: 0x00024F63
	private float _MaxForwardSpeed
	{
		get
		{
			return this.m_maxForwardSpeed;
		}
		set
		{
			this.m_maxForwardSpeed = value;
			this._forwardAccel = value / math.max(0.01f, this.m_forwardAccelTime);
		}
	}

	// Token: 0x1700009E RID: 158
	// (get) Token: 0x060006E3 RID: 1763 RVA: 0x00026D84 File Offset: 0x00024F84
	// (set) Token: 0x060006E4 RID: 1764 RVA: 0x00026D8C File Offset: 0x00024F8C
	private float _MaxTurnRate
	{
		get
		{
			return this.m_maxTurnRate;
		}
		set
		{
			this.m_maxTurnRate = value;
			this._turnAccel = value / math.max(1E-06f, this.m_turnAccelTime);
		}
	}

	// Token: 0x1700009F RID: 159
	// (get) Token: 0x060006E5 RID: 1765 RVA: 0x00026DAD File Offset: 0x00024FAD
	// (set) Token: 0x060006E6 RID: 1766 RVA: 0x00026DB5 File Offset: 0x00024FB5
	private float _MaxTiltAngle
	{
		get
		{
			return this.m_maxTiltAngle;
		}
		set
		{
			this.m_maxTiltAngle = value;
			this._tiltAccel = value / math.max(1E-06f, this.m_tiltTime);
		}
	}

	// Token: 0x060006E7 RID: 1767 RVA: 0x00026DD8 File Offset: 0x00024FD8
	protected override void Awake()
	{
		base.Awake();
		this._hasAudioSource = this.m_audioSource != null;
		this._hasHoverSound = this.m_hoverSound != null;
		this._MaxForwardSpeed = this.m_maxForwardSpeed;
		this._MaxTurnRate = this.m_maxTurnRate;
		this._MaxTiltAngle = this.m_maxTiltAngle;
	}

	// Token: 0x060006E8 RID: 1768 RVA: 0x00026E34 File Offset: 0x00025034
	protected override void AuthorityBeginDocked()
	{
		base.AuthorityBeginDocked();
		this._currentTurnRate = 0f;
		this._currentTiltAngle = 0f;
		float3 @float = this._ProjectOnPlane(base.transform.forward, math.up());
		this._currentTurnAngle = this._SignedAngle(new float3(0f, 0f, 1f), @float, new float3(0f, 1f, 0f));
		this._motorLevel = 0f;
		if (this._hasAudioSource)
		{
			this.m_audioSource.Stop();
			this.m_audioSource.volume = 0f;
		}
	}

	// Token: 0x060006E9 RID: 1769 RVA: 0x00026EDC File Offset: 0x000250DC
	protected override void AuthorityUpdate(float dt)
	{
		base.AuthorityUpdate(dt);
		if (this.localState == RCVehicle.State.Mobilized)
		{
			float num = math.length(this.activeInput.joystick);
			this._motorLevel = math.saturate(num);
			if (this.hasNetworkSync)
			{
				this.networkSync.syncedState.dataA = (byte)((uint)(this._motorLevel * 255f));
				return;
			}
		}
		else
		{
			this._motorLevel = 0f;
		}
	}

	// Token: 0x060006EA RID: 1770 RVA: 0x00026F50 File Offset: 0x00025150
	protected override void RemoteUpdate(float dt)
	{
		base.RemoteUpdate(dt);
		if (this.localState == RCVehicle.State.Mobilized && this.hasNetworkSync)
		{
			this._motorLevel = (float)this.networkSync.syncedState.dataA / 255f;
			return;
		}
		this._motorLevel = 0f;
	}

	// Token: 0x060006EB RID: 1771 RVA: 0x00026FA0 File Offset: 0x000251A0
	protected override void SharedUpdate(float dt)
	{
		base.SharedUpdate(dt);
		switch (this.localState)
		{
		case RCVehicle.State.Disabled:
		case RCVehicle.State.DockedLeft:
		case RCVehicle.State.DockedRight:
		case RCVehicle.State.Crashed:
			break;
		case RCVehicle.State.Mobilized:
			if (this._hasAudioSource && this._hasHoverSound)
			{
				if (this.localStatePrev != RCVehicle.State.Mobilized)
				{
					this.m_audioSource.volume = 0f;
					this.m_audioSource.clip = this.m_hoverSound;
					this.m_audioSource.loop = true;
					this.m_audioSource.Play();
					return;
				}
				float num = math.lerp(this.m_hoverSoundVolumeMinMax.x, this.m_hoverSoundVolumeMinMax.y, this._motorLevel);
				float num2 = this.m_hoverSoundVolumeMinMax.y / this.m_hoverSoundVolumeRampTime * dt;
				this.m_audioSource.volume = this._MoveTowards(this.m_audioSource.volume, num, num2);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060006EC RID: 1772 RVA: 0x00027084 File Offset: 0x00025284
	protected void FixedUpdate()
	{
		if (!base.HasLocalAuthority || this.localState != RCVehicle.State.Mobilized)
		{
			return;
		}
		float fixedDeltaTime = Time.fixedDeltaTime;
		float num = this.m_inputThrustForward.Get(this.activeInput) - this.m_inputThrustBack.Get(this.activeInput);
		float num2 = this.m_inputTurn.Get(this.activeInput);
		float num3 = this.m_inputJump.Get(this.activeInput);
		RaycastHit raycastHit;
		bool flag = Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 10f, 134218241, QueryTriggerInteraction.Ignore);
		bool flag2 = flag && raycastHit.distance <= this.m_hoverHeight + 0.1f;
		if (num3 > 0.001f && flag2 && !this._hasJumped)
		{
			this.rb.AddForce(Vector3.up * this.m_jumpForce, ForceMode.Impulse);
			this._hasJumped = true;
		}
		else if (num3 <= 0.001f)
		{
			this._hasJumped = false;
		}
		float num4 = num2 * this._MaxTurnRate;
		this._currentTurnRate = this._MoveTowards(this._currentTurnRate, num4, this._turnAccel * fixedDeltaTime);
		this._currentTurnAngle += this._currentTurnRate * fixedDeltaTime;
		float num5 = math.lerp(-this.m_maxTiltAngle, this.m_maxTiltAngle, math.unlerp(-1f, 1f, num));
		this._currentTiltAngle = this._MoveTowards(this._currentTiltAngle, num5, this._tiltAccel * fixedDeltaTime);
		base.transform.rotation = quaternion.EulerXYZ(math.radians(new float3(this._currentTiltAngle, this._currentTurnAngle, 0f)));
		float3 @float = base.transform.forward;
		float num6 = math.dot(@float, this.rb.velocity);
		float num7 = num * this.m_maxForwardSpeed;
		float num8 = ((math.abs(num7) > 0.001f && ((num7 > 0f && num6 < num7) || (num7 < 0f && num6 > num7))) ? math.sign(num7) : 0f);
		this.rb.AddForce(@float * this._forwardAccel * num8, ForceMode.Acceleration);
		if (flag)
		{
			float num9 = math.saturate(this.m_hoverHeight - raycastHit.distance);
			float num10 = math.dot(this.rb.velocity, Vector3.up);
			float num11 = num9 * this.m_hoverForce - num10 * this.m_hoverDamp;
			this.rb.AddForce(math.up() * num11, ForceMode.Force);
		}
	}

	// Token: 0x060006ED RID: 1773 RVA: 0x00027330 File Offset: 0x00025530
	protected void OnCollisionEnter(Collision collision)
	{
		GameObject gameObject = collision.collider.gameObject;
		bool flag = gameObject.IsOnLayer(UnityLayer.GorillaThrowable);
		bool flag2 = gameObject.IsOnLayer(UnityLayer.GorillaHand);
		if ((flag || flag2) && this.localState == RCVehicle.State.Mobilized)
		{
			Vector3 vector = Vector3.zero;
			if (flag2)
			{
				GorillaHandClimber component = gameObject.GetComponent<GorillaHandClimber>();
				if (component != null)
				{
					vector = ((component.xrNode == XRNode.LeftHand) ? GTPlayer.Instance.leftHandCenterVelocityTracker : GTPlayer.Instance.rightHandCenterVelocityTracker).GetAverageVelocity(true, 0.15f, false);
				}
			}
			else if (collision.rigidbody != null)
			{
				vector = collision.rigidbody.velocity;
			}
			if (flag || vector.sqrMagnitude > 0.01f)
			{
				if (base.HasLocalAuthority)
				{
					this.AuthorityApplyImpact(vector, flag);
					return;
				}
				if (this.networkSync != null)
				{
					this.networkSync.photonView.RPC("HitRCVehicleRPC", RpcTarget.Others, new object[] { vector, flag });
				}
			}
		}
	}

	// Token: 0x060006EE RID: 1774 RVA: 0x00027431 File Offset: 0x00025631
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float _MoveTowards(float current, float target, float maxDelta)
	{
		if (math.abs(target - current) > maxDelta)
		{
			return current + math.sign(target - current) * maxDelta;
		}
		return target;
	}

	// Token: 0x060006EF RID: 1775 RVA: 0x0002744C File Offset: 0x0002564C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float _SignedAngle(float3 from, float3 to, float3 axis)
	{
		float3 @float = math.normalize(from);
		float3 float2 = math.normalize(to);
		float num = math.acos(math.dot(@float, float2));
		float num2 = math.sign(math.dot(math.cross(@float, float2), axis));
		return math.degrees(num) * num2;
	}

	// Token: 0x060006F0 RID: 1776 RVA: 0x0002748D File Offset: 0x0002568D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float3 _ProjectOnPlane(float3 vector, float3 planeNormal)
	{
		return vector - math.dot(vector, planeNormal) * planeNormal;
	}

	// Token: 0x04000829 RID: 2089
	[SerializeField]
	private RCHoverboard._SingleInputOption m_inputTurn = new RCHoverboard._SingleInputOption(RCHoverboard._EInputSource.StickX, new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0f, 0f, 0f),
		new Keyframe(0.1f, 0f, 0f, 1.25f, 0f, 0f),
		new Keyframe(0.9f, 1f, 1.25f, 0f, 0f, 0f),
		new Keyframe(1f, 1f, 0f, 0f, 0f, 0f)
	}));

	// Token: 0x0400082A RID: 2090
	[SerializeField]
	private RCHoverboard._SingleInputOption m_inputThrustForward = new RCHoverboard._SingleInputOption(RCHoverboard._EInputSource.Trigger, AnimationCurves.EaseInCirc);

	// Token: 0x0400082B RID: 2091
	[SerializeField]
	private RCHoverboard._SingleInputOption m_inputThrustBack = new RCHoverboard._SingleInputOption(RCHoverboard._EInputSource.StickBack, new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0f, 0f, 0f),
		new Keyframe(0.9f, 0f, 0f, 9.9999f, 0.5825f, 0.3767f),
		new Keyframe(1f, 1f, 9.9999f, 1f, 0f, 0f)
	}));

	// Token: 0x0400082C RID: 2092
	[SerializeField]
	private RCHoverboard._SingleInputOption m_inputJump = new RCHoverboard._SingleInputOption(RCHoverboard._EInputSource.PrimaryFaceButton, AnimationCurves.Linear);

	// Token: 0x0400082D RID: 2093
	[Tooltip("Desired hover height above ground from this transform's position.")]
	[SerializeField]
	private float m_hoverHeight = 0.2f;

	// Token: 0x0400082E RID: 2094
	[Tooltip("Upward force to maintain hover when below hoverHeight.")]
	[SerializeField]
	private float m_hoverForce = 200f;

	// Token: 0x0400082F RID: 2095
	[Tooltip("Damping factor to smooth out vertical movement.")]
	[SerializeField]
	private float m_hoverDamp = 5f;

	// Token: 0x04000830 RID: 2096
	[Tooltip("Upward impulse force for jump.")]
	[SerializeField]
	private float m_jumpForce = 3.5f;

	// Token: 0x04000831 RID: 2097
	private bool _hasJumped;

	// Token: 0x04000832 RID: 2098
	[SerializeField]
	[HideInInspector]
	private float m_maxForwardSpeed = 6f;

	// Token: 0x04000833 RID: 2099
	[SerializeField]
	[Tooltip("Time (seconds) to reach max forward speed from zero.")]
	private float m_forwardAccelTime = 2f;

	// Token: 0x04000834 RID: 2100
	[SerializeField]
	[HideInInspector]
	private float m_maxTurnRate = 720f;

	// Token: 0x04000835 RID: 2101
	[Tooltip("Time (seconds) to reach max turning rate.")]
	[SerializeField]
	private float m_turnAccelTime = 0.75f;

	// Token: 0x04000836 RID: 2102
	[SerializeField]
	[HideInInspector]
	private float m_maxTiltAngle = 30f;

	// Token: 0x04000837 RID: 2103
	[Tooltip("Time (seconds) to reach max tilt angle.")]
	[SerializeField]
	private float m_tiltTime = 0.1f;

	// Token: 0x04000838 RID: 2104
	[Tooltip("Audio source for any motor or hover sound.")]
	[SerializeField]
	private AudioSource m_audioSource;

	// Token: 0x04000839 RID: 2105
	[Tooltip("Looping motor/hover sound clip.")]
	[SerializeField]
	private AudioClip m_hoverSound;

	// Token: 0x0400083A RID: 2106
	[Tooltip("Volume range for the hover sound (x = min, y = max).")]
	[SerializeField]
	private float2 m_hoverSoundVolumeMinMax = new float2(0.1f, 0.5f);

	// Token: 0x0400083B RID: 2107
	[Tooltip("Time it takes for the volume to reach max value.")]
	[SerializeField]
	private float m_hoverSoundVolumeRampTime = 1f;

	// Token: 0x0400083C RID: 2108
	private bool _hasAudioSource;

	// Token: 0x0400083D RID: 2109
	private bool _hasHoverSound;

	// Token: 0x0400083E RID: 2110
	private float _forwardAccel;

	// Token: 0x0400083F RID: 2111
	private float _turnAccel;

	// Token: 0x04000840 RID: 2112
	private float _tiltAccel;

	// Token: 0x04000841 RID: 2113
	private float _currentTurnRate;

	// Token: 0x04000842 RID: 2114
	private float _currentTurnAngle;

	// Token: 0x04000843 RID: 2115
	private float _currentTiltAngle;

	// Token: 0x04000844 RID: 2116
	private float _motorLevel;

	// Token: 0x0200010E RID: 270
	private enum _EInputSource
	{
		// Token: 0x04000846 RID: 2118
		None,
		// Token: 0x04000847 RID: 2119
		StickX,
		// Token: 0x04000848 RID: 2120
		StickForward,
		// Token: 0x04000849 RID: 2121
		StickBack,
		// Token: 0x0400084A RID: 2122
		Trigger,
		// Token: 0x0400084B RID: 2123
		PrimaryFaceButton
	}

	// Token: 0x0200010F RID: 271
	[Serializable]
	private struct _SingleInputOption
	{
		// Token: 0x060006F2 RID: 1778 RVA: 0x000276BB File Offset: 0x000258BB
		public _SingleInputOption(RCHoverboard._EInputSource source, AnimationCurve remapCurve)
		{
			this.source = new GTOption<StringEnum<RCHoverboard._EInputSource>>(source);
			this.remapCurve = new GTOption<AnimationCurve>(remapCurve);
		}

		// Token: 0x060006F3 RID: 1779 RVA: 0x000276DC File Offset: 0x000258DC
		public float Get(RCRemoteHoldable.RCInput input)
		{
			float num;
			switch (this.source.ResolvedValue.Value)
			{
			case RCHoverboard._EInputSource.None:
				num = 0f;
				break;
			case RCHoverboard._EInputSource.StickX:
				num = input.joystick.x;
				break;
			case RCHoverboard._EInputSource.StickForward:
				num = math.saturate(input.joystick.y);
				break;
			case RCHoverboard._EInputSource.StickBack:
				num = math.saturate(-input.joystick.y);
				break;
			case RCHoverboard._EInputSource.Trigger:
				num = input.trigger;
				break;
			case RCHoverboard._EInputSource.PrimaryFaceButton:
				num = (float)input.buttons;
				break;
			default:
				num = 0f;
				break;
			}
			float num2 = num;
			return this.remapCurve.ResolvedValue.Evaluate(math.abs(num2)) * math.sign(num2);
		}

		// Token: 0x0400084C RID: 2124
		public GTOption<StringEnum<RCHoverboard._EInputSource>> source;

		// Token: 0x0400084D RID: 2125
		public GTOption<AnimationCurve> remapCurve;
	}
}
