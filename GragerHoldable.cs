using System;
using Cinemachine.Utility;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000189 RID: 393
public class GragerHoldable : MonoBehaviour
{
	// Token: 0x060009B8 RID: 2488 RVA: 0x00033B2C File Offset: 0x00031D2C
	private void Start()
	{
		this.LocalRotationAxis = this.LocalRotationAxis.normalized;
		this.lastWorldPosition = base.transform.TransformPoint(this.LocalCenterOfMass);
		this.lastClackParentLocalPosition = base.transform.parent.InverseTransformPoint(this.lastWorldPosition);
		this.centerOfMassRadius = this.LocalCenterOfMass.magnitude;
		this.RotationCorrection = Quaternion.Euler(this.RotationCorrectionEuler);
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x00033BA0 File Offset: 0x00031DA0
	private void Update()
	{
		Vector3 vector = base.transform.TransformPoint(this.LocalCenterOfMass);
		Vector3 vector2 = this.lastWorldPosition + this.velocity * Time.deltaTime * this.drag;
		Vector3 vector3 = base.transform.parent.TransformDirection(this.LocalRotationAxis);
		Vector3 vector4 = base.transform.position + (vector2 - base.transform.position).ProjectOntoPlane(vector3).normalized * this.centerOfMassRadius;
		vector4 = Vector3.MoveTowards(vector4, vector, this.localFriction * Time.deltaTime);
		this.velocity = (vector4 - this.lastWorldPosition) / Time.deltaTime;
		this.velocity += Vector3.down * this.gravity * Time.deltaTime;
		this.lastWorldPosition = vector4;
		base.transform.rotation = Quaternion.LookRotation(vector4 - base.transform.position, vector3) * this.RotationCorrection;
		Vector3 vector5 = base.transform.parent.InverseTransformPoint(base.transform.TransformPoint(this.LocalCenterOfMass));
		if ((vector5 - this.lastClackParentLocalPosition).IsLongerThan(this.distancePerClack))
		{
			this.clackAudio.GTPlayOneShot(this.allClacks[Random.Range(0, this.allClacks.Length)], 1f);
			this.lastClackParentLocalPosition = vector5;
		}
	}

	// Token: 0x04000BC3 RID: 3011
	[SerializeField]
	private Vector3 LocalCenterOfMass;

	// Token: 0x04000BC4 RID: 3012
	[SerializeField]
	private Vector3 LocalRotationAxis;

	// Token: 0x04000BC5 RID: 3013
	[SerializeField]
	private Vector3 RotationCorrectionEuler;

	// Token: 0x04000BC6 RID: 3014
	[SerializeField]
	private float drag;

	// Token: 0x04000BC7 RID: 3015
	[SerializeField]
	private float gravity;

	// Token: 0x04000BC8 RID: 3016
	[SerializeField]
	private float localFriction;

	// Token: 0x04000BC9 RID: 3017
	[SerializeField]
	private float distancePerClack;

	// Token: 0x04000BCA RID: 3018
	[SerializeField]
	private AudioSource clackAudio;

	// Token: 0x04000BCB RID: 3019
	[SerializeField]
	private AudioClip[] allClacks;

	// Token: 0x04000BCC RID: 3020
	private float centerOfMassRadius;

	// Token: 0x04000BCD RID: 3021
	private Vector3 velocity;

	// Token: 0x04000BCE RID: 3022
	private Vector3 lastWorldPosition;

	// Token: 0x04000BCF RID: 3023
	private Vector3 lastClackParentLocalPosition;

	// Token: 0x04000BD0 RID: 3024
	private Quaternion RotationCorrection;
}
