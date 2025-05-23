using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000545 RID: 1349
public class FishingRod : TransferrableObject
{
	// Token: 0x060020A0 RID: 8352 RVA: 0x000A3874 File Offset: 0x000A1A74
	public override void OnActivate()
	{
		base.OnActivate();
		Transform transform = base.transform;
		Vector3 vector = transform.up + transform.forward * 640f;
		this.bobRigidbody.AddForce(vector, ForceMode.Impulse);
		this.line.tensionScale = 0.86f;
		this.ReelOut();
	}

	// Token: 0x060020A1 RID: 8353 RVA: 0x000A38CD File Offset: 0x000A1ACD
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		this.line.tensionScale = 1f;
		this.ReelStop();
	}

	// Token: 0x060020A2 RID: 8354 RVA: 0x000A38EB File Offset: 0x000A1AEB
	protected override void Start()
	{
		base.Start();
		this.rig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x060020A3 RID: 8355 RVA: 0x000A38FF File Offset: 0x000A1AFF
	public void SetBobFloat(bool enable)
	{
		if (!this.bobRigidbody)
		{
			return;
		}
		this._bobFloatPlaneY = this.bobRigidbody.position.y;
		this._bobFloating = enable;
	}

	// Token: 0x060020A4 RID: 8356 RVA: 0x000A392C File Offset: 0x000A1B2C
	private void QuickReel()
	{
		if (this._lineResizing)
		{
			return;
		}
		this.bobCollider.enabled = false;
		this.ReelIn();
	}

	// Token: 0x060020A5 RID: 8357 RVA: 0x000A394C File Offset: 0x000A1B4C
	public bool IsFreeHandGripping()
	{
		bool flag = base.InLeftHand();
		Transform transform = (flag ? this.rig.rightHandTransform : this.rig.leftHandTransform);
		float magnitude = (this.reelToSync.position - transform.position).magnitude;
		bool flag2 = this._grippingHand || magnitude <= 0.16f;
		this.disableStealing = flag2;
		if (!flag2)
		{
			return false;
		}
		VRMapThumb vrmapThumb = (flag ? this.rig.rightThumb : this.rig.leftThumb);
		VRMapIndex vrmapIndex = (flag ? this.rig.rightIndex : this.rig.leftIndex);
		VRMap vrmap = (flag ? this.rig.rightMiddle : this.rig.leftMiddle);
		float calcT = vrmapThumb.calcT;
		float calcT2 = vrmapIndex.calcT;
		float calcT3 = vrmap.calcT;
		bool flag3 = calcT >= 0.1f && calcT2 >= 0.2f && calcT3 >= 0.2f;
		this._grippingHand = (flag3 ? transform : null);
		return flag3;
	}

	// Token: 0x060020A6 RID: 8358 RVA: 0x000A3A65 File Offset: 0x000A1C65
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (this._grippingHand)
		{
			this._grippingHand = null;
		}
		this.ResetLineLength(this.lineLengthMin * 1.32f);
		return true;
	}

	// Token: 0x060020A7 RID: 8359 RVA: 0x000A3A9C File Offset: 0x000A1C9C
	public void ReelIn()
	{
		this._manualReeling = false;
		FishingRod.SetHandleMotorUse(true, this.reelSpinRate, this.handleJoint, true);
		this._lineResizing = true;
		this._lineExpanding = false;
		float num = (float)this.line.segmentNumber + 0.0001f;
		this.line.segmentMinLength = (this._targetSegmentMin = this.lineLengthMin / num);
		this.line.segmentMaxLength = (this._targetSegmentMax = this.lineLengthMax / num);
	}

	// Token: 0x060020A8 RID: 8360 RVA: 0x000A3B1C File Offset: 0x000A1D1C
	public void ReelOut()
	{
		this._manualReeling = false;
		FishingRod.SetHandleMotorUse(true, this.reelSpinRate, this.handleJoint, false);
		this._lineResizing = true;
		this._lineExpanding = true;
		float num = (float)this.line.segmentNumber + 0.0001f;
		this.line.segmentMinLength = (this._targetSegmentMin = this.lineLengthMin / num);
		this.line.segmentMaxLength = (this._targetSegmentMax = this.lineLengthMax / num);
	}

	// Token: 0x060020A9 RID: 8361 RVA: 0x000A3B9C File Offset: 0x000A1D9C
	public void ReelStop()
	{
		if (this._manualReeling)
		{
			this._localRotDelta = 0f;
		}
		else
		{
			FishingRod.SetHandleMotorUse(false, 0f, this.handleJoint, false);
		}
		this.bobCollider.enabled = true;
		if (this.line)
		{
			this.line.resizeScale = 1f;
		}
		this._lineResizing = false;
		this._lineExpanding = false;
	}

	// Token: 0x060020AA RID: 8362 RVA: 0x000A3C08 File Offset: 0x000A1E08
	private static void SetHandleMotorUse(bool useMotor, float spinRate, HingeJoint handleJoint, bool reverse)
	{
		JointMotor motor = handleJoint.motor;
		motor.force = (useMotor ? 1f : 0f) * spinRate;
		motor.targetVelocity = 16384f * (reverse ? (-1f) : 1f);
		handleJoint.motor = motor;
	}

	// Token: 0x060020AB RID: 8363 RVA: 0x000A3C58 File Offset: 0x000A1E58
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		this._manualReeling = (this._isGrippingHandle = this.IsFreeHandGripping());
		if (ControllerInputPoller.instance && ControllerInputPoller.PrimaryButtonPress(base.InLeftHand() ? XRNode.LeftHand : XRNode.RightHand))
		{
			this.QuickReel();
		}
		if (this._lineResetting && this._sinceReset.HasElapsed(this.line.resizeSpeed))
		{
			this.bobCollider.enabled = true;
			this._lineResetting = false;
		}
		this.handleTransform.localPosition = this.reelFreezeLocalPosition;
	}

	// Token: 0x060020AC RID: 8364 RVA: 0x000A3CEB File Offset: 0x000A1EEB
	private void ResetLineLength(float length)
	{
		if (!this.line)
		{
			return;
		}
		this._lineResetting = true;
		this.bobCollider.enabled = false;
		this.line.ForceTotalLength(length);
		this._sinceReset = TimeSince.Now();
	}

	// Token: 0x060020AD RID: 8365 RVA: 0x000A3D28 File Offset: 0x000A1F28
	private void FixedUpdate()
	{
		Transform transform = base.transform;
		this.handleRigidbody.useGravity = !this._manualReeling;
		if (this._bobFloating && this.bobRigidbody)
		{
			float y = this.bobRigidbody.position.y;
			float num = this.bobFloatForce * this.bobRigidbody.mass;
			float num2 = num * Mathf.Clamp01(this._bobFloatPlaneY - y);
			num += num2;
			if (y <= this._bobFloatPlaneY)
			{
				this.bobRigidbody.AddForce(0f, num, 0f);
			}
		}
		if (this._manualReeling)
		{
			if (this._isGrippingHandle && this._grippingHand)
			{
				this.reelTo.position = this._grippingHand.position;
			}
			Vector3 vector = this.reelFrom.InverseTransformPoint(this.reelTo.position);
			vector.x = 0f;
			vector.Normalize();
			vector *= 2f;
			Quaternion quaternion = Quaternion.FromToRotation(Vector3.forward, vector);
			quaternion = (base.InRightHand() ? quaternion : Quaternion.Inverse(quaternion));
			this._localRotDelta = FishingRod.GetSignedDeltaYZ(ref this._lastLocalRot, ref quaternion);
			this._lastLocalRot = quaternion;
			Quaternion quaternion2 = transform.rotation * quaternion;
			this.handleRigidbody.MoveRotation(quaternion2);
		}
		else
		{
			this.reelTo.localPosition = transform.InverseTransformPoint(this.reelToSync.position);
		}
		if (!this.line)
		{
			return;
		}
		if (this._manualReeling)
		{
			this._lineResizing = Mathf.Abs(this._localRotDelta) >= 0.001f;
			this._lineExpanding = Mathf.Sign(this._localRotDelta) >= 0f;
		}
		if (!this._lineResizing)
		{
			return;
		}
		float num3 = (this._manualReeling ? (Mathf.Abs(this._localRotDelta) * 0.66f * Time.fixedDeltaTime) : (this.lineResizeRate * this.lineCastFactor));
		this.line.resizeScale = this.lineCastFactor;
		float num4 = num3 * Time.fixedDeltaTime;
		float num5 = this.line.segmentTargetLength;
		if (this._manualReeling)
		{
			float num6 = 1f / ((float)this.line.segmentNumber + 0.0001f);
			float num7 = this.lineLengthMin * num6;
			float num8 = this.lineLengthMax * num6;
			num4 *= (this._lineExpanding ? 1f : (-1f));
			num4 *= (base.InRightHand() ? (-1f) : 1f);
			float num9 = num5 + num4;
			if (num9 > num7 && num9 < num8)
			{
				num5 += num4;
			}
		}
		else if (this._lineExpanding)
		{
			if (num5 < this._targetSegmentMax)
			{
				num5 += num4;
			}
			else
			{
				this._lineResizing = false;
			}
		}
		else if (num5 > this._targetSegmentMin)
		{
			num5 -= num4;
		}
		else
		{
			this._lineResizing = false;
		}
		if (this._lineResizing)
		{
			this.line.segmentTargetLength = num5;
			return;
		}
		this.ReelStop();
	}

	// Token: 0x060020AE RID: 8366 RVA: 0x000A4020 File Offset: 0x000A2220
	private static float GetSignedDeltaYZ(ref Quaternion a, ref Quaternion b)
	{
		Vector3 forward = Vector3.forward;
		Vector3 vector = a * forward;
		Vector3 vector2 = b * forward;
		float num = Mathf.Atan2(vector.y, vector.z) * 57.29578f;
		float num2 = Mathf.Atan2(vector2.y, vector2.z) * 57.29578f;
		return Mathf.DeltaAngle(num, num2);
	}

	// Token: 0x040024A5 RID: 9381
	public Transform handleTransform;

	// Token: 0x040024A6 RID: 9382
	public HingeJoint handleJoint;

	// Token: 0x040024A7 RID: 9383
	public Rigidbody handleRigidbody;

	// Token: 0x040024A8 RID: 9384
	public BoxCollider handleCollider;

	// Token: 0x040024A9 RID: 9385
	public Rigidbody bobRigidbody;

	// Token: 0x040024AA RID: 9386
	public Collider bobCollider;

	// Token: 0x040024AB RID: 9387
	public VerletLine line;

	// Token: 0x040024AC RID: 9388
	public GorillaVelocityEstimator tipTracker;

	// Token: 0x040024AD RID: 9389
	public Rigidbody tipBody;

	// Token: 0x040024AE RID: 9390
	[NonSerialized]
	public VRRig rig;

	// Token: 0x040024AF RID: 9391
	[Space]
	public Vector3 reelFreezeLocalPosition;

	// Token: 0x040024B0 RID: 9392
	public Transform reelFrom;

	// Token: 0x040024B1 RID: 9393
	public Transform reelTo;

	// Token: 0x040024B2 RID: 9394
	public Transform reelToSync;

	// Token: 0x040024B3 RID: 9395
	[Space]
	public float reelSpinRate = 1f;

	// Token: 0x040024B4 RID: 9396
	public float lineResizeRate = 1f;

	// Token: 0x040024B5 RID: 9397
	public float lineCastFactor = 3f;

	// Token: 0x040024B6 RID: 9398
	public float lineLengthMin = 0.1f;

	// Token: 0x040024B7 RID: 9399
	public float lineLengthMax = 8f;

	// Token: 0x040024B8 RID: 9400
	[Space]
	[NonSerialized]
	private bool _bobFloating;

	// Token: 0x040024B9 RID: 9401
	public float bobFloatForce = 8f;

	// Token: 0x040024BA RID: 9402
	public float bobStaticDrag = 3.2f;

	// Token: 0x040024BB RID: 9403
	public float bobDynamicDrag = 1.1f;

	// Token: 0x040024BC RID: 9404
	[NonSerialized]
	private float _bobFloatPlaneY;

	// Token: 0x040024BD RID: 9405
	[Space]
	[NonSerialized]
	private float _targetSegmentMin;

	// Token: 0x040024BE RID: 9406
	[NonSerialized]
	private float _targetSegmentMax;

	// Token: 0x040024BF RID: 9407
	[Space]
	[NonSerialized]
	private bool _manualReeling;

	// Token: 0x040024C0 RID: 9408
	[NonSerialized]
	private bool _lineResizing;

	// Token: 0x040024C1 RID: 9409
	[NonSerialized]
	private bool _lineExpanding;

	// Token: 0x040024C2 RID: 9410
	[NonSerialized]
	private bool _lineResetting;

	// Token: 0x040024C3 RID: 9411
	[NonSerialized]
	private TimeSince _sinceReset;

	// Token: 0x040024C4 RID: 9412
	[Space]
	[NonSerialized]
	private Quaternion _lastLocalRot = Quaternion.identity;

	// Token: 0x040024C5 RID: 9413
	[NonSerialized]
	private float _localRotDelta;

	// Token: 0x040024C6 RID: 9414
	[NonSerialized]
	private bool _isGrippingHandle;

	// Token: 0x040024C7 RID: 9415
	[NonSerialized]
	private Transform _grippingHand;

	// Token: 0x040024C8 RID: 9416
	private TimeSince _sinceGripLoss;
}
