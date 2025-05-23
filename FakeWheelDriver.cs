using System;
using Cinemachine.Utility;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000176 RID: 374
public class FakeWheelDriver : MonoBehaviour
{
	// Token: 0x170000DE RID: 222
	// (get) Token: 0x06000960 RID: 2400 RVA: 0x00032811 File Offset: 0x00030A11
	// (set) Token: 0x06000961 RID: 2401 RVA: 0x00032819 File Offset: 0x00030A19
	public bool hasCollision { get; private set; }

	// Token: 0x06000962 RID: 2402 RVA: 0x00032822 File Offset: 0x00030A22
	public void SetThrust(Vector3 thrust)
	{
		this.thrust = thrust;
	}

	// Token: 0x06000963 RID: 2403 RVA: 0x0003282C File Offset: 0x00030A2C
	private void OnCollisionStay(Collision collision)
	{
		int num = 0;
		Vector3 vector = Vector3.zero;
		foreach (ContactPoint contactPoint in collision.contacts)
		{
			if (contactPoint.thisCollider == this.wheelCollider)
			{
				vector += contactPoint.point;
				num++;
			}
		}
		if (num > 0)
		{
			this.collisionNormal = collision.contacts[0].normal;
			this.collisionPoint = vector / (float)num;
			this.hasCollision = true;
		}
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x000328B8 File Offset: 0x00030AB8
	private void FixedUpdate()
	{
		if (this.hasCollision)
		{
			Vector3 vector = base.transform.rotation * this.thrust;
			if (this.myRigidBody.velocity.IsShorterThan(this.maxSpeed))
			{
				vector = vector.ProjectOntoPlane(this.collisionNormal).normalized * this.thrust.magnitude;
				this.myRigidBody.AddForceAtPosition(vector, this.collisionPoint);
			}
			Vector3 vector2 = this.myRigidBody.velocity.ProjectOntoPlane(this.collisionNormal).ProjectOntoPlane(vector.normalized);
			if (vector2.IsLongerThan(this.lateralFrictionForce))
			{
				this.myRigidBody.AddForceAtPosition(-vector2.normalized * this.lateralFrictionForce, this.collisionPoint);
			}
			else
			{
				this.myRigidBody.AddForceAtPosition(-vector2, this.collisionPoint);
			}
		}
		this.hasCollision = false;
	}

	// Token: 0x04000B46 RID: 2886
	[SerializeField]
	private Rigidbody myRigidBody;

	// Token: 0x04000B47 RID: 2887
	[SerializeField]
	private Vector3 thrust;

	// Token: 0x04000B48 RID: 2888
	[SerializeField]
	private Collider wheelCollider;

	// Token: 0x04000B49 RID: 2889
	[SerializeField]
	private float maxSpeed;

	// Token: 0x04000B4A RID: 2890
	[SerializeField]
	private float lateralFrictionForce;

	// Token: 0x04000B4C RID: 2892
	private Vector3 collisionPoint;

	// Token: 0x04000B4D RID: 2893
	private Vector3 collisionNormal;
}
