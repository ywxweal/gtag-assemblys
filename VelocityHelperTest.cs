using System;
using UnityEngine;

// Token: 0x02000782 RID: 1922
public class VelocityHelperTest : MonoBehaviour
{
	// Token: 0x06003036 RID: 12342 RVA: 0x000EE36D File Offset: 0x000EC56D
	private void Setup()
	{
		this.lastPosition = base.transform.position;
		this.lastVelocity = Vector3.zero;
		this.velocity = Vector3.zero;
		this.speed = 0f;
	}

	// Token: 0x06003037 RID: 12343 RVA: 0x000EE3A1 File Offset: 0x000EC5A1
	private void Start()
	{
		this.Setup();
	}

	// Token: 0x06003038 RID: 12344 RVA: 0x000EE3AC File Offset: 0x000EC5AC
	private void FixedUpdate()
	{
		float deltaTime = Time.deltaTime;
		Vector3 position = base.transform.position;
		Vector3 vector = (position - this.lastPosition) / deltaTime;
		this.velocity = Vector3.Lerp(this.lastVelocity, vector, deltaTime);
		this.speed = this.velocity.magnitude;
		this.lastPosition = position;
		this.lastVelocity = vector;
	}

	// Token: 0x06003039 RID: 12345 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Update()
	{
	}

	// Token: 0x0400364E RID: 13902
	public Vector3 velocity;

	// Token: 0x0400364F RID: 13903
	public float speed;

	// Token: 0x04003650 RID: 13904
	[Space]
	public Vector3 lastVelocity;

	// Token: 0x04003651 RID: 13905
	public Vector3 lastPosition;

	// Token: 0x04003652 RID: 13906
	[Space]
	[SerializeField]
	private float[] _deltaTimes = new float[5];
}
