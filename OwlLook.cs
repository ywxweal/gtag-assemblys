using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000416 RID: 1046
public class OwlLook : MonoBehaviour
{
	// Token: 0x0600198F RID: 6543 RVA: 0x0007BEAF File Offset: 0x0007A0AF
	private void Awake()
	{
		this.overlapRigs = new VRRig[(int)PhotonNetworkController.Instance.GetRoomSize(null)];
		if (this.myRig == null)
		{
			this.myRig = base.GetComponentInParent<VRRig>();
		}
	}

	// Token: 0x06001990 RID: 6544 RVA: 0x0007BEE4 File Offset: 0x0007A0E4
	private void LateUpdate()
	{
		if (NetworkSystem.Instance.InRoom)
		{
			if (this.rigs.Length != NetworkSystem.Instance.RoomPlayerCount)
			{
				this.rigs = VRRigCache.Instance.GetAllRigs();
			}
		}
		else if (this.rigs.Length != 1)
		{
			this.rigs = new VRRig[1];
			this.rigs[0] = VRRig.LocalRig;
		}
		float num = -1f;
		float num2 = Mathf.Cos(this.lookAtAngleDegrees / 180f * 3.1415927f);
		int num3 = 0;
		for (int i = 0; i < this.rigs.Length; i++)
		{
			if (!(this.rigs[i] == this.myRig))
			{
				Vector3 vector = this.rigs[i].tagSound.transform.position - base.transform.position;
				if (vector.magnitude <= this.lookRadius)
				{
					float num4 = Vector3.Dot(-base.transform.up, vector.normalized);
					if (num4 > num2)
					{
						this.overlapRigs[num3++] = this.rigs[i];
					}
				}
			}
		}
		this.lookTarget = null;
		for (int j = 0; j < num3; j++)
		{
			Vector3 vector = (this.overlapRigs[j].tagSound.transform.position - base.transform.position).normalized;
			float num4 = Vector3.Dot(base.transform.forward, vector);
			if (num4 > num)
			{
				num = num4;
				this.lookTarget = this.overlapRigs[j].tagSound.transform;
			}
		}
		Vector3 vector2 = this.neck.forward;
		if (this.lookTarget != null)
		{
			vector2 = (this.lookTarget.position - this.head.position).normalized;
		}
		Vector3 vector3 = this.neck.InverseTransformDirection(vector2);
		vector3.y = Mathf.Clamp(vector3.y, this.minNeckY, this.maxNeckY);
		vector2 = this.neck.TransformDirection(vector3.normalized);
		Vector3 vector4 = Vector3.RotateTowards(this.head.forward, vector2, this.rotSpeed * 0.017453292f * Time.deltaTime, 0f);
		this.head.rotation = Quaternion.LookRotation(vector4, this.neck.up);
	}

	// Token: 0x04001C76 RID: 7286
	public Transform head;

	// Token: 0x04001C77 RID: 7287
	public Transform lookTarget;

	// Token: 0x04001C78 RID: 7288
	public Transform neck;

	// Token: 0x04001C79 RID: 7289
	public float lookRadius = 0.5f;

	// Token: 0x04001C7A RID: 7290
	public Collider[] overlapColliders;

	// Token: 0x04001C7B RID: 7291
	public VRRig[] rigs = new VRRig[10];

	// Token: 0x04001C7C RID: 7292
	public VRRig[] overlapRigs;

	// Token: 0x04001C7D RID: 7293
	public float rotSpeed = 1f;

	// Token: 0x04001C7E RID: 7294
	public float lookAtAngleDegrees = 60f;

	// Token: 0x04001C7F RID: 7295
	public float maxNeckY;

	// Token: 0x04001C80 RID: 7296
	public float minNeckY;

	// Token: 0x04001C81 RID: 7297
	public VRRig myRig;
}
