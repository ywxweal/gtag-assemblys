using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200019C RID: 412
public class SpringyWobbler : MonoBehaviour
{
	// Token: 0x06000A2D RID: 2605 RVA: 0x000355D8 File Offset: 0x000337D8
	private void Start()
	{
		int num = 1;
		Transform transform = base.transform;
		while (transform.childCount > 0)
		{
			transform = transform.GetChild(0);
			num++;
		}
		this.children = new Transform[num];
		transform = base.transform;
		this.children[0] = transform;
		int num2 = 1;
		while (transform.childCount > 0)
		{
			transform = transform.GetChild(0);
			this.children[num2] = transform;
			num2++;
		}
		this.lastEndpointWorldPos = this.children[this.children.Length - 1].transform.position;
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x00035664 File Offset: 0x00033864
	private void Update()
	{
		float x = base.transform.lossyScale.x;
		Vector3 vector = base.transform.TransformPoint(this.idealEndpointLocalPos);
		this.endpointVelocity += (vector - this.lastEndpointWorldPos) * this.stabilizingForce * x * Time.deltaTime;
		Vector3 vector2 = this.lastEndpointWorldPos + this.endpointVelocity * Time.deltaTime;
		float num = this.maxDisplacement * x;
		if ((vector2 - vector).IsLongerThan(num))
		{
			vector2 = vector + (vector2 - vector).normalized * num;
		}
		this.endpointVelocity = (vector2 - this.lastEndpointWorldPos) * (1f - this.drag) / Time.deltaTime;
		Vector3 vector3 = base.transform.TransformPoint(this.rotateToFaceLocalPos);
		Vector3 vector4 = base.transform.TransformDirection(Vector3.up);
		Vector3 position = base.transform.position;
		Vector3 vector5 = position + base.transform.TransformDirection(this.idealEndpointLocalPos) * this.startStiffness * x;
		Vector3 vector6 = vector2;
		Vector3 vector7 = vector6 + (vector3 - vector6).normalized * this.endStiffness * x;
		for (int i = 1; i < this.children.Length; i++)
		{
			float num2 = (float)i / (float)(this.children.Length - 1);
			Vector3 vector8 = BezierUtils.BezierSolve(num2, position, vector5, vector7, vector6);
			Vector3 vector9 = BezierUtils.BezierSolve(num2 + 0.1f, position, vector5, vector7, vector6);
			this.children[i].transform.position = vector8;
			this.children[i].transform.rotation = Quaternion.LookRotation(vector9 - vector8, vector4);
		}
		this.lastIdealEndpointWorldPos = vector;
		this.lastEndpointWorldPos = vector2;
	}

	// Token: 0x04000C47 RID: 3143
	[SerializeField]
	private float stabilizingForce;

	// Token: 0x04000C48 RID: 3144
	[SerializeField]
	private float drag;

	// Token: 0x04000C49 RID: 3145
	[SerializeField]
	private float maxDisplacement;

	// Token: 0x04000C4A RID: 3146
	private Transform[] children;

	// Token: 0x04000C4B RID: 3147
	[SerializeField]
	private Vector3 idealEndpointLocalPos;

	// Token: 0x04000C4C RID: 3148
	[SerializeField]
	private Vector3 rotateToFaceLocalPos;

	// Token: 0x04000C4D RID: 3149
	[SerializeField]
	private float startStiffness;

	// Token: 0x04000C4E RID: 3150
	[SerializeField]
	private float endStiffness;

	// Token: 0x04000C4F RID: 3151
	private Vector3 lastIdealEndpointWorldPos;

	// Token: 0x04000C50 RID: 3152
	private Vector3 lastEndpointWorldPos;

	// Token: 0x04000C51 RID: 3153
	private Vector3 endpointVelocity;
}
