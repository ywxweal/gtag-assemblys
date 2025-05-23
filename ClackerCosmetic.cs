using System;
using Cinemachine.Utility;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000162 RID: 354
public class ClackerCosmetic : MonoBehaviour
{
	// Token: 0x060008F6 RID: 2294 RVA: 0x00030588 File Offset: 0x0002E788
	private void Start()
	{
		this.LocalRotationAxis = this.LocalRotationAxis.normalized;
		this.arm1.parent = this;
		this.arm2.parent = this;
		this.arm1.transform = this.clackerArm1;
		this.arm2.transform = this.clackerArm2;
		this.arm1.lastWorldPosition = this.clackerArm1.transform.TransformPoint(this.LocalCenterOfMass);
		this.arm2.lastWorldPosition = this.clackerArm2.transform.TransformPoint(this.LocalCenterOfMass);
		this.centerOfMassRadius = this.LocalCenterOfMass.magnitude;
		this.RotationCorrection = Quaternion.Euler(this.RotationCorrectionEuler);
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x00030644 File Offset: 0x0002E844
	private void Update()
	{
		Vector3 lastWorldPosition = this.arm1.lastWorldPosition;
		this.arm1.UpdateArm();
		this.arm2.UpdateArm();
		ref Vector3 eulerAngles = this.clackerArm1.transform.eulerAngles;
		Vector3 eulerAngles2 = this.clackerArm2.transform.eulerAngles;
		Mathf.DeltaAngle(eulerAngles.y, eulerAngles2.y);
		if ((this.arm1.lastWorldPosition - this.arm2.lastWorldPosition).IsShorterThan(this.collisionDistance))
		{
			float sqrMagnitude = (this.arm1.velocity - this.arm2.velocity).sqrMagnitude;
			if (this.parentHoldable.InHand())
			{
				if (sqrMagnitude > this.heavyClackSpeed * this.heavyClackSpeed)
				{
					this.heavyClackAudio.Play();
				}
				else if (sqrMagnitude > this.mediumClackSpeed * this.mediumClackSpeed)
				{
					this.mediumClackAudio.Play();
				}
				else if (sqrMagnitude > this.minimumClackSpeed * this.minimumClackSpeed)
				{
					this.lightClackAudio.Play();
				}
			}
			Vector3 vector = (this.arm1.lastWorldPosition + this.arm2.lastWorldPosition) / 2f;
			Vector3 vector2 = (this.arm1.lastWorldPosition - this.arm2.lastWorldPosition).normalized * (this.collisionDistance + 0.001f) / 2f;
			Vector3 vector3 = vector + vector2;
			Vector3 vector4 = vector - vector2;
			if ((lastWorldPosition - vector3).IsLongerThan(lastWorldPosition - vector4))
			{
				vector2 = -vector2;
			}
			this.arm1.SetPosition(vector + vector2);
			this.arm2.SetPosition(vector - vector2);
			ref Vector3 ptr = ref this.arm1.velocity;
			Vector3 velocity = this.arm2.velocity;
			Vector3 velocity2 = this.arm1.velocity;
			ptr = velocity;
			this.arm2.velocity = velocity2;
			Vector3 vector5 = (this.arm1.lastWorldPosition - this.arm2.lastWorldPosition).normalized * this.pushApartStrength * Mathf.Sqrt(sqrMagnitude);
			this.arm1.velocity = this.arm1.velocity + vector5;
			this.arm2.velocity = this.arm2.velocity - vector5;
		}
	}

	// Token: 0x04000A8F RID: 2703
	[SerializeField]
	private TransferrableObject parentHoldable;

	// Token: 0x04000A90 RID: 2704
	[SerializeField]
	private Transform clackerArm1;

	// Token: 0x04000A91 RID: 2705
	[SerializeField]
	private Transform clackerArm2;

	// Token: 0x04000A92 RID: 2706
	[SerializeField]
	private Vector3 LocalCenterOfMass;

	// Token: 0x04000A93 RID: 2707
	[SerializeField]
	private Vector3 LocalRotationAxis;

	// Token: 0x04000A94 RID: 2708
	[SerializeField]
	private Vector3 RotationCorrectionEuler;

	// Token: 0x04000A95 RID: 2709
	[SerializeField]
	private float drag;

	// Token: 0x04000A96 RID: 2710
	[SerializeField]
	private float gravity;

	// Token: 0x04000A97 RID: 2711
	[SerializeField]
	private float localFriction;

	// Token: 0x04000A98 RID: 2712
	[SerializeField]
	private float minimumClackSpeed;

	// Token: 0x04000A99 RID: 2713
	[SerializeField]
	private SoundBankPlayer lightClackAudio;

	// Token: 0x04000A9A RID: 2714
	[SerializeField]
	private float mediumClackSpeed;

	// Token: 0x04000A9B RID: 2715
	[SerializeField]
	private SoundBankPlayer mediumClackAudio;

	// Token: 0x04000A9C RID: 2716
	[SerializeField]
	private float heavyClackSpeed;

	// Token: 0x04000A9D RID: 2717
	[SerializeField]
	private SoundBankPlayer heavyClackAudio;

	// Token: 0x04000A9E RID: 2718
	[SerializeField]
	private float collisionDistance;

	// Token: 0x04000A9F RID: 2719
	private float centerOfMassRadius;

	// Token: 0x04000AA0 RID: 2720
	[SerializeField]
	private float pushApartStrength;

	// Token: 0x04000AA1 RID: 2721
	private ClackerCosmetic.PerArmData arm1;

	// Token: 0x04000AA2 RID: 2722
	private ClackerCosmetic.PerArmData arm2;

	// Token: 0x04000AA3 RID: 2723
	private Quaternion RotationCorrection;

	// Token: 0x02000163 RID: 355
	private struct PerArmData
	{
		// Token: 0x060008F9 RID: 2297 RVA: 0x000308D0 File Offset: 0x0002EAD0
		public void UpdateArm()
		{
			Vector3 vector = this.transform.TransformPoint(this.parent.LocalCenterOfMass);
			Vector3 vector2 = this.lastWorldPosition + this.velocity * Time.deltaTime * this.parent.drag;
			Vector3 vector3 = this.transform.parent.TransformDirection(this.parent.LocalRotationAxis);
			Vector3 vector4 = this.transform.position + (vector2 - this.transform.position).ProjectOntoPlane(vector3).normalized * this.parent.centerOfMassRadius;
			vector4 = Vector3.MoveTowards(vector4, vector, this.parent.localFriction * Time.deltaTime);
			this.velocity = (vector4 - this.lastWorldPosition) / Time.deltaTime;
			this.velocity += Vector3.down * this.parent.gravity * Time.deltaTime;
			this.lastWorldPosition = vector4;
			this.transform.rotation = Quaternion.LookRotation(vector3, vector4 - this.transform.position) * this.parent.RotationCorrection;
			this.lastWorldPosition = this.transform.TransformPoint(this.parent.LocalCenterOfMass);
		}

		// Token: 0x060008FA RID: 2298 RVA: 0x00030A38 File Offset: 0x0002EC38
		public void SetPosition(Vector3 newPosition)
		{
			Vector3 vector = this.transform.parent.TransformDirection(this.parent.LocalRotationAxis);
			this.transform.rotation = Quaternion.LookRotation(vector, newPosition - this.transform.position) * this.parent.RotationCorrection;
			this.lastWorldPosition = this.transform.TransformPoint(this.parent.LocalCenterOfMass);
		}

		// Token: 0x04000AA4 RID: 2724
		public ClackerCosmetic parent;

		// Token: 0x04000AA5 RID: 2725
		public Transform transform;

		// Token: 0x04000AA6 RID: 2726
		public Vector3 velocity;

		// Token: 0x04000AA7 RID: 2727
		public Vector3 lastWorldPosition;
	}
}
