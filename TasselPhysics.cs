using System;
using UnityEngine;

// Token: 0x0200019F RID: 415
public class TasselPhysics : MonoBehaviour
{
	// Token: 0x06000A46 RID: 2630 RVA: 0x00035BF4 File Offset: 0x00033DF4
	private void Awake()
	{
		this.centerOfMassLength = this.localCenterOfMass.magnitude;
		if (this.LockXAxis)
		{
			this.rotCorrection = Quaternion.Inverse(Quaternion.LookRotation(Vector3.right, this.localCenterOfMass));
			return;
		}
		this.rotCorrection = Quaternion.Inverse(Quaternion.LookRotation(this.localCenterOfMass));
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x00035C4C File Offset: 0x00033E4C
	private void Update()
	{
		float y = base.transform.lossyScale.y;
		this.velocity *= this.drag;
		this.velocity.y = this.velocity.y - this.gravityStrength * y * Time.deltaTime;
		Vector3 position = base.transform.position;
		Vector3 vector = this.lastCenterPos + this.velocity * Time.deltaTime;
		Vector3 vector2 = position + (vector - position).normalized * this.centerOfMassLength * y;
		this.velocity = (vector2 - this.lastCenterPos) / Time.deltaTime;
		this.lastCenterPos = vector2;
		if (this.LockXAxis)
		{
			foreach (GameObject gameObject in this.tasselInstances)
			{
				gameObject.transform.rotation = Quaternion.LookRotation(gameObject.transform.right, vector2 - position) * this.rotCorrection;
			}
			return;
		}
		foreach (GameObject gameObject2 in this.tasselInstances)
		{
			gameObject2.transform.rotation = Quaternion.LookRotation(vector2 - position, gameObject2.transform.position - position) * this.rotCorrection;
		}
	}

	// Token: 0x04000C66 RID: 3174
	[SerializeField]
	private GameObject[] tasselInstances;

	// Token: 0x04000C67 RID: 3175
	[SerializeField]
	private Vector3 localCenterOfMass;

	// Token: 0x04000C68 RID: 3176
	[SerializeField]
	private float gravityStrength;

	// Token: 0x04000C69 RID: 3177
	[SerializeField]
	private float drag;

	// Token: 0x04000C6A RID: 3178
	[SerializeField]
	private bool LockXAxis;

	// Token: 0x04000C6B RID: 3179
	private Vector3 lastCenterPos;

	// Token: 0x04000C6C RID: 3180
	private Vector3 velocity;

	// Token: 0x04000C6D RID: 3181
	private float centerOfMassLength;

	// Token: 0x04000C6E RID: 3182
	private Quaternion rotCorrection;
}
