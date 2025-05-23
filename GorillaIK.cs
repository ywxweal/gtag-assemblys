using System;
using UnityEngine;

// Token: 0x02000616 RID: 1558
public class GorillaIK : MonoBehaviour
{
	// Token: 0x060026C5 RID: 9925 RVA: 0x000C0358 File Offset: 0x000BE558
	private void Awake()
	{
		if (Application.isPlaying && !this.testInEditor)
		{
			this.dU = (this.leftUpperArm.position - this.leftLowerArm.position).magnitude;
			this.dL = (this.leftLowerArm.position - this.leftHand.position).magnitude;
			this.dMax = this.dU + this.dL - this.eps;
			this.initialUpperLeft = this.leftUpperArm.localRotation;
			this.initialLowerLeft = this.leftLowerArm.localRotation;
			this.initialUpperRight = this.rightUpperArm.localRotation;
			this.initialLowerRight = this.rightLowerArm.localRotation;
		}
	}

	// Token: 0x060026C6 RID: 9926 RVA: 0x000C042A File Offset: 0x000BE62A
	private void OnEnable()
	{
		GorillaIKMgr.Instance.RegisterIK(this);
	}

	// Token: 0x060026C7 RID: 9927 RVA: 0x000C0437 File Offset: 0x000BE637
	private void OnDisable()
	{
		GorillaIKMgr.Instance.DeregisterIK(this);
	}

	// Token: 0x060026C8 RID: 9928 RVA: 0x000C0444 File Offset: 0x000BE644
	private void ArmIK(ref Transform upperArm, ref Transform lowerArm, ref Transform hand, Quaternion initRotUpper, Quaternion initRotLower, Transform target)
	{
		upperArm.localRotation = initRotUpper;
		lowerArm.localRotation = initRotLower;
		float num = Mathf.Clamp((target.position - upperArm.position).magnitude, this.eps, this.dMax);
		float num2 = Mathf.Acos(Mathf.Clamp(Vector3.Dot((hand.position - upperArm.position).normalized, (lowerArm.position - upperArm.position).normalized), -1f, 1f));
		float num3 = Mathf.Acos(Mathf.Clamp(Vector3.Dot((upperArm.position - lowerArm.position).normalized, (hand.position - lowerArm.position).normalized), -1f, 1f));
		float num4 = Mathf.Acos(Mathf.Clamp(Vector3.Dot((hand.position - upperArm.position).normalized, (target.position - upperArm.position).normalized), -1f, 1f));
		float num5 = Mathf.Acos(Mathf.Clamp((this.dL * this.dL - this.dU * this.dU - num * num) / (-2f * this.dU * num), -1f, 1f));
		float num6 = Mathf.Acos(Mathf.Clamp((num * num - this.dU * this.dU - this.dL * this.dL) / (-2f * this.dU * this.dL), -1f, 1f));
		Vector3 normalized = Vector3.Cross(hand.position - upperArm.position, lowerArm.position - upperArm.position).normalized;
		Vector3 normalized2 = Vector3.Cross(hand.position - upperArm.position, target.position - upperArm.position).normalized;
		Quaternion quaternion = Quaternion.AngleAxis((num5 - num2) * 57.29578f, Quaternion.Inverse(upperArm.rotation) * normalized);
		Quaternion quaternion2 = Quaternion.AngleAxis((num6 - num3) * 57.29578f, Quaternion.Inverse(lowerArm.rotation) * normalized);
		Quaternion quaternion3 = Quaternion.AngleAxis(num4 * 57.29578f, Quaternion.Inverse(upperArm.rotation) * normalized2);
		this.newRotationUpper = upperArm.localRotation * quaternion3 * quaternion;
		this.newRotationLower = lowerArm.localRotation * quaternion2;
		upperArm.localRotation = this.newRotationUpper;
		lowerArm.localRotation = this.newRotationLower;
		hand.rotation = target.rotation;
	}

	// Token: 0x04002B25 RID: 11045
	public Transform headBone;

	// Token: 0x04002B26 RID: 11046
	public Transform leftUpperArm;

	// Token: 0x04002B27 RID: 11047
	public Transform leftLowerArm;

	// Token: 0x04002B28 RID: 11048
	public Transform leftHand;

	// Token: 0x04002B29 RID: 11049
	public Transform rightUpperArm;

	// Token: 0x04002B2A RID: 11050
	public Transform rightLowerArm;

	// Token: 0x04002B2B RID: 11051
	public Transform rightHand;

	// Token: 0x04002B2C RID: 11052
	public Transform targetLeft;

	// Token: 0x04002B2D RID: 11053
	public Transform targetRight;

	// Token: 0x04002B2E RID: 11054
	public Transform targetHead;

	// Token: 0x04002B2F RID: 11055
	public Quaternion initialUpperLeft;

	// Token: 0x04002B30 RID: 11056
	public Quaternion initialLowerLeft;

	// Token: 0x04002B31 RID: 11057
	public Quaternion initialUpperRight;

	// Token: 0x04002B32 RID: 11058
	public Quaternion initialLowerRight;

	// Token: 0x04002B33 RID: 11059
	public Quaternion newRotationUpper;

	// Token: 0x04002B34 RID: 11060
	public Quaternion newRotationLower;

	// Token: 0x04002B35 RID: 11061
	public float dU;

	// Token: 0x04002B36 RID: 11062
	public float dL;

	// Token: 0x04002B37 RID: 11063
	public float dMax;

	// Token: 0x04002B38 RID: 11064
	public bool testInEditor;

	// Token: 0x04002B39 RID: 11065
	public bool reset;

	// Token: 0x04002B3A RID: 11066
	public bool testDefineRot;

	// Token: 0x04002B3B RID: 11067
	public bool moveOnce;

	// Token: 0x04002B3C RID: 11068
	public float eps;

	// Token: 0x04002B3D RID: 11069
	public float upperArmAngle;

	// Token: 0x04002B3E RID: 11070
	public float elbowAngle;
}
