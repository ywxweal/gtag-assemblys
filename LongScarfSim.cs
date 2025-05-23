using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200018F RID: 399
public class LongScarfSim : MonoBehaviour
{
	// Token: 0x060009D9 RID: 2521 RVA: 0x00034614 File Offset: 0x00032814
	private void Start()
	{
		this.clampToPlane.Normalize();
		this.velocityEstimator = base.GetComponent<GorillaVelocityEstimator>();
		this.baseLocalRotations = new Quaternion[this.gameObjects.Length];
		for (int i = 0; i < this.gameObjects.Length; i++)
		{
			this.baseLocalRotations[i] = this.gameObjects[i].transform.localRotation;
		}
	}

	// Token: 0x060009DA RID: 2522 RVA: 0x0003467C File Offset: 0x0003287C
	private void LateUpdate()
	{
		this.velocity *= this.drag;
		this.velocity.y = this.velocity.y - this.gravityStrength * Time.deltaTime;
		Vector3 position = base.transform.position;
		Vector3 vector = this.lastCenterPos + this.velocity * Time.deltaTime;
		Vector3 vector2 = position + (vector - position).normalized * this.centerOfMassLength;
		Vector3 vector3 = base.transform.InverseTransformPoint(vector2);
		float num = Vector3.Dot(vector3, this.clampToPlane);
		if (num < 0f)
		{
			vector3 -= this.clampToPlane * num;
			vector2 = base.transform.TransformPoint(vector3);
		}
		Vector3 vector4 = vector2;
		this.velocity = (vector4 - this.lastCenterPos) / Time.deltaTime;
		this.lastCenterPos = vector4;
		float num2 = (float)(this.velocityEstimator.linearVelocity.IsLongerThan(this.speedThreshold) ? 1 : 0);
		this.currentBlend = Mathf.MoveTowards(this.currentBlend, num2, this.blendAmountPerSecond * Time.deltaTime);
		Quaternion quaternion = Quaternion.LookRotation(vector4 - position);
		for (int i = 0; i < this.gameObjects.Length; i++)
		{
			Quaternion quaternion2 = this.gameObjects[i].transform.parent.rotation * this.baseLocalRotations[i];
			this.gameObjects[i].transform.rotation = Quaternion.Lerp(quaternion2, quaternion, this.currentBlend);
		}
	}

	// Token: 0x04000BFD RID: 3069
	[SerializeField]
	private GameObject[] gameObjects;

	// Token: 0x04000BFE RID: 3070
	[SerializeField]
	private float speedThreshold = 1f;

	// Token: 0x04000BFF RID: 3071
	[SerializeField]
	private float blendAmountPerSecond = 1f;

	// Token: 0x04000C00 RID: 3072
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04000C01 RID: 3073
	private Quaternion[] baseLocalRotations;

	// Token: 0x04000C02 RID: 3074
	private float currentBlend;

	// Token: 0x04000C03 RID: 3075
	[SerializeField]
	private float centerOfMassLength;

	// Token: 0x04000C04 RID: 3076
	[SerializeField]
	private float gravityStrength;

	// Token: 0x04000C05 RID: 3077
	[SerializeField]
	private float drag;

	// Token: 0x04000C06 RID: 3078
	[SerializeField]
	private Vector3 clampToPlane;

	// Token: 0x04000C07 RID: 3079
	private Vector3 lastCenterPos;

	// Token: 0x04000C08 RID: 3080
	private Vector3 velocity;
}
