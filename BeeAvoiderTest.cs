using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000104 RID: 260
public class BeeAvoiderTest : MonoBehaviour
{
	// Token: 0x06000680 RID: 1664 RVA: 0x00025E38 File Offset: 0x00024038
	public void Update()
	{
		Vector3 position = this.patrolPoints[this.nextPatrolPoint].transform.position;
		Vector3 position2 = base.transform.position;
		Vector3 vector = (position - position2).normalized * this.speed;
		this.velocity = Vector3.MoveTowards(this.velocity * this.drag, vector, this.acceleration);
		if ((position2 - position).IsLongerThan(this.instabilityOffRadius))
		{
			this.velocity += Random.insideUnitSphere * this.instability * Time.deltaTime;
		}
		Vector3 vector2 = position2 + this.velocity * Time.deltaTime;
		GameObject[] array = this.avoidancePoints;
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 position3 = array[i].transform.position;
			if ((vector2 - position3).IsShorterThan(this.avoidRadius))
			{
				Vector3 normalized = Vector3.Cross(position3 - vector2, position - vector2).normalized;
				Vector3 normalized2 = (position - position3).normalized;
				float num = Vector3.Dot(vector2 - position3, normalized);
				Vector3 vector3 = (this.avoidRadius - num) * normalized;
				vector2 += vector3;
				this.velocity += vector3;
			}
		}
		base.transform.position = vector2;
		base.transform.rotation = Quaternion.LookRotation(position - vector2);
		if ((vector2 - position).IsShorterThan(this.patrolArrivedRadius))
		{
			this.nextPatrolPoint = (this.nextPatrolPoint + 1) % this.patrolPoints.Length;
		}
	}

	// Token: 0x040007E0 RID: 2016
	public GameObject[] patrolPoints;

	// Token: 0x040007E1 RID: 2017
	public GameObject[] avoidancePoints;

	// Token: 0x040007E2 RID: 2018
	public float speed;

	// Token: 0x040007E3 RID: 2019
	public float acceleration;

	// Token: 0x040007E4 RID: 2020
	public float instability;

	// Token: 0x040007E5 RID: 2021
	public float instabilityOffRadius;

	// Token: 0x040007E6 RID: 2022
	public float drag;

	// Token: 0x040007E7 RID: 2023
	public float avoidRadius;

	// Token: 0x040007E8 RID: 2024
	public float patrolArrivedRadius;

	// Token: 0x040007E9 RID: 2025
	private int nextPatrolPoint;

	// Token: 0x040007EA RID: 2026
	private Vector3 velocity;
}
