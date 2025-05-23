using System;
using UnityEngine;

// Token: 0x020000C3 RID: 195
public class RotateXform : MonoBehaviour
{
	// Token: 0x060004E0 RID: 1248 RVA: 0x0001C5F0 File Offset: 0x0001A7F0
	private void Update()
	{
		if (!this.xform)
		{
			return;
		}
		Vector3 vector = ((this.mode == RotateXform.Mode.Local) ? this.xform.localEulerAngles : this.xform.eulerAngles);
		float num = Time.deltaTime * this.speedFactor;
		vector.x += this.speed.x * num;
		vector.y += this.speed.y * num;
		vector.z += this.speed.z * num;
		if (this.mode == RotateXform.Mode.Local)
		{
			this.xform.localEulerAngles = vector;
			return;
		}
		this.xform.eulerAngles = vector;
	}

	// Token: 0x040005BD RID: 1469
	public Transform xform;

	// Token: 0x040005BE RID: 1470
	public Vector3 speed = Vector3.zero;

	// Token: 0x040005BF RID: 1471
	public RotateXform.Mode mode;

	// Token: 0x040005C0 RID: 1472
	public float speedFactor = 0.0625f;

	// Token: 0x020000C4 RID: 196
	public enum Mode
	{
		// Token: 0x040005C2 RID: 1474
		Local,
		// Token: 0x040005C3 RID: 1475
		World
	}
}
