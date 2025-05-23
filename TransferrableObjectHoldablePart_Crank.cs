using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003BD RID: 957
public class TransferrableObjectHoldablePart_Crank : TransferrableObjectHoldablePart
{
	// Token: 0x06001644 RID: 5700 RVA: 0x0006BF57 File Offset: 0x0006A157
	public void SetOnCrankedCallback(Action<float> onCrankedCallback)
	{
		this.onCrankedCallback = onCrankedCallback;
	}

	// Token: 0x06001645 RID: 5701 RVA: 0x0006BF60 File Offset: 0x0006A160
	private void Awake()
	{
		if (this.rotatingPart == null)
		{
			this.rotatingPart = base.transform;
		}
		Vector3 vector = this.rotatingPart.parent.InverseTransformPoint(this.rotatingPart.TransformPoint(Vector3.right));
		this.lastAngle = Mathf.Atan2(vector.y, vector.x);
		this.baseLocalAngle = this.rotatingPart.localRotation;
		this.baseLocalAngleInverse = Quaternion.Inverse(this.baseLocalAngle);
		this.crankRadius = new Vector2(this.crankHandleX, this.crankHandleY).magnitude;
		this.crankAngleOffset = Mathf.Atan2(this.crankHandleY, this.crankHandleX) * 57.29578f;
		if (this.crankHandleMaxZ < this.crankHandleMinZ)
		{
			float num = this.crankHandleMaxZ;
			float num2 = this.crankHandleMinZ;
			this.crankHandleMinZ = num;
			this.crankHandleMaxZ = num2;
		}
	}

	// Token: 0x06001646 RID: 5702 RVA: 0x0006C048 File Offset: 0x0006A248
	protected override void UpdateHeld(VRRig rig, bool isHeldLeftHand)
	{
		Vector3 vector4;
		if (rig.isOfflineVRRig)
		{
			Transform transform = (isHeldLeftHand ? GTPlayer.Instance.leftControllerTransform : GTPlayer.Instance.rightControllerTransform);
			Vector3 vector = this.rotatingPart.InverseTransformPoint(transform.position);
			Vector3 vector2 = (vector.xy().normalized * this.crankRadius).WithZ(Mathf.Clamp(vector.z, this.crankHandleMinZ, this.crankHandleMaxZ));
			Vector3 vector3 = this.rotatingPart.TransformPoint(vector2);
			if (this.maxHandSnapDistance > 0f && (transform.position - vector3).IsLongerThan(this.maxHandSnapDistance))
			{
				this.OnRelease(null, isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
				return;
			}
			transform.position = vector3;
			vector4 = transform.position;
		}
		else
		{
			VRMap vrmap = (isHeldLeftHand ? rig.leftHand : rig.rightHand);
			vector4 = vrmap.GetExtrapolatedControllerPosition();
			vector4 -= vrmap.rigTarget.rotation * (isHeldLeftHand ? GTPlayer.Instance.leftHandOffset : GTPlayer.Instance.rightHandOffset) * rig.scaleFactor;
		}
		Vector3 vector5 = this.baseLocalAngleInverse * Quaternion.Inverse(this.rotatingPart.parent.rotation) * (vector4 - this.rotatingPart.position);
		float num = Mathf.Atan2(vector5.y, vector5.x) * 57.29578f;
		float num2 = Mathf.DeltaAngle(this.lastAngle, num);
		this.lastAngle = num;
		if (num2 != 0f)
		{
			if (this.onCrankedCallback != null)
			{
				this.onCrankedCallback(num2);
			}
			for (int i = 0; i < this.thresholds.Length; i++)
			{
				this.thresholds[i].OnCranked(num2);
			}
		}
		this.rotatingPart.localRotation = this.baseLocalAngle * Quaternion.AngleAxis(num - this.crankAngleOffset, Vector3.forward);
	}

	// Token: 0x06001647 RID: 5703 RVA: 0x0006C264 File Offset: 0x0006A464
	private void OnDrawGizmosSelected()
	{
		Transform transform = ((this.rotatingPart != null) ? this.rotatingPart : base.transform);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.TransformPoint(new Vector3(this.crankHandleX, this.crankHandleY, this.crankHandleMinZ)), transform.TransformPoint(new Vector3(this.crankHandleX, this.crankHandleY, this.crankHandleMaxZ)));
	}

	// Token: 0x040018C0 RID: 6336
	[SerializeField]
	private float crankHandleX;

	// Token: 0x040018C1 RID: 6337
	[SerializeField]
	private float crankHandleY;

	// Token: 0x040018C2 RID: 6338
	[SerializeField]
	private float crankHandleMinZ;

	// Token: 0x040018C3 RID: 6339
	[SerializeField]
	private float crankHandleMaxZ;

	// Token: 0x040018C4 RID: 6340
	[SerializeField]
	private float maxHandSnapDistance;

	// Token: 0x040018C5 RID: 6341
	private float crankAngleOffset;

	// Token: 0x040018C6 RID: 6342
	private float crankRadius;

	// Token: 0x040018C7 RID: 6343
	[SerializeField]
	private Transform rotatingPart;

	// Token: 0x040018C8 RID: 6344
	private float lastAngle;

	// Token: 0x040018C9 RID: 6345
	private Quaternion baseLocalAngle;

	// Token: 0x040018CA RID: 6346
	private Quaternion baseLocalAngleInverse;

	// Token: 0x040018CB RID: 6347
	private Action<float> onCrankedCallback;

	// Token: 0x040018CC RID: 6348
	[SerializeField]
	private TransferrableObjectHoldablePart_Crank.CrankThreshold[] thresholds;

	// Token: 0x020003BE RID: 958
	[Serializable]
	private struct CrankThreshold
	{
		// Token: 0x06001649 RID: 5705 RVA: 0x0006C2DF File Offset: 0x0006A4DF
		public void OnCranked(float deltaAngle)
		{
			this.currentAngle += deltaAngle;
			if (Mathf.Abs(this.currentAngle) > this.angleThreshold)
			{
				this.currentAngle = 0f;
				this.onReached.Invoke();
			}
		}

		// Token: 0x040018CD RID: 6349
		public float angleThreshold;

		// Token: 0x040018CE RID: 6350
		public UnityEvent onReached;

		// Token: 0x040018CF RID: 6351
		[HideInInspector]
		public float currentAngle;
	}
}
