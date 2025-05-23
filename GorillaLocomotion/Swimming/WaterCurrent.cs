using System;
using System.Collections.Generic;
using AA;
using CjLib;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000CD0 RID: 3280
	public class WaterCurrent : MonoBehaviour
	{
		// Token: 0x17000839 RID: 2105
		// (get) Token: 0x06005154 RID: 20820 RVA: 0x00189B49 File Offset: 0x00187D49
		public float Speed
		{
			get
			{
				return this.currentSpeed;
			}
		}

		// Token: 0x1700083A RID: 2106
		// (get) Token: 0x06005155 RID: 20821 RVA: 0x00189B51 File Offset: 0x00187D51
		public float Accel
		{
			get
			{
				return this.currentAccel;
			}
		}

		// Token: 0x1700083B RID: 2107
		// (get) Token: 0x06005156 RID: 20822 RVA: 0x00189B59 File Offset: 0x00187D59
		public float InwardSpeed
		{
			get
			{
				return this.inwardCurrentSpeed;
			}
		}

		// Token: 0x1700083C RID: 2108
		// (get) Token: 0x06005157 RID: 20823 RVA: 0x00189B61 File Offset: 0x00187D61
		public float InwardAccel
		{
			get
			{
				return this.inwardCurrentAccel;
			}
		}

		// Token: 0x06005158 RID: 20824 RVA: 0x00189B6C File Offset: 0x00187D6C
		public bool GetCurrentAtPoint(Vector3 worldPoint, Vector3 startingVelocity, float dt, out Vector3 currentVelocity, out Vector3 velocityChange)
		{
			float num = (this.fullEffectDistance + this.fadeDistance) * (this.fullEffectDistance + this.fadeDistance);
			bool flag = false;
			velocityChange = Vector3.zero;
			currentVelocity = Vector3.zero;
			float num2 = 0.0001f;
			float magnitude = startingVelocity.magnitude;
			if (magnitude > num2)
			{
				Vector3 vector = startingVelocity / magnitude;
				float num3 = Spring.DamperDecayExact(magnitude, this.dampingHalfLife, dt, 1E-05f);
				Vector3 vector2 = vector * num3;
				velocityChange += vector2 - startingVelocity;
			}
			for (int i = 0; i < this.splines.Count; i++)
			{
				CatmullRomSpline catmullRomSpline = this.splines[i];
				Vector3 vector3;
				float closestEvaluationOnSpline = catmullRomSpline.GetClosestEvaluationOnSpline(worldPoint, out vector3);
				Vector3 vector4 = catmullRomSpline.Evaluate(closestEvaluationOnSpline);
				Vector3 vector5 = vector4 - worldPoint;
				if (vector5.sqrMagnitude < num)
				{
					flag = true;
					float magnitude2 = vector5.magnitude;
					float num4 = ((magnitude2 > this.fullEffectDistance) ? (1f - Mathf.Clamp01((magnitude2 - this.fullEffectDistance) / this.fadeDistance)) : 1f);
					float num5 = Mathf.Clamp01(closestEvaluationOnSpline + this.velocityAnticipationAdjustment);
					Vector3 forwardTangent = catmullRomSpline.GetForwardTangent(num5, 0.01f);
					if (this.currentSpeed > num2 && Vector3.Dot(startingVelocity, forwardTangent) < num4 * this.currentSpeed)
					{
						velocityChange += forwardTangent * (this.currentAccel * dt);
					}
					else if (this.currentSpeed < num2 && Vector3.Dot(startingVelocity, forwardTangent) > num4 * this.currentSpeed)
					{
						velocityChange -= forwardTangent * (this.currentAccel * dt);
					}
					currentVelocity += forwardTangent * num4 * this.currentSpeed;
					float num6 = Mathf.InverseLerp(this.inwardCurrentNoEffectRadius, this.inwardCurrentFullEffectRadius, magnitude2);
					if (num6 > num2)
					{
						vector3 = Vector3.ProjectOnPlane(vector5, forwardTangent);
						Vector3 normalized = vector3.normalized;
						if (this.inwardCurrentSpeed > num2 && Vector3.Dot(startingVelocity, normalized) < num6 * this.inwardCurrentSpeed)
						{
							velocityChange += normalized * (this.InwardAccel * dt);
						}
						else if (this.inwardCurrentSpeed < num2 && Vector3.Dot(startingVelocity, normalized) > num6 * this.inwardCurrentSpeed)
						{
							velocityChange -= normalized * (this.InwardAccel * dt);
						}
					}
					this.debugSplinePoint = vector4;
				}
			}
			this.debugCurrentVelocity = velocityChange.normalized;
			return flag;
		}

		// Token: 0x06005159 RID: 20825 RVA: 0x00189E20 File Offset: 0x00188020
		private void Update()
		{
			if (this.debugDrawCurrentQueries)
			{
				DebugUtil.DrawSphere(this.debugSplinePoint, 0.15f, 12, 12, Color.green, false, DebugUtil.Style.Wireframe);
				DebugUtil.DrawArrow(this.debugSplinePoint, this.debugSplinePoint + this.debugCurrentVelocity, 0.1f, 0.1f, 12, 0.1f, Color.yellow, false, DebugUtil.Style.Wireframe);
			}
		}

		// Token: 0x0600515A RID: 20826 RVA: 0x00189E84 File Offset: 0x00188084
		private void OnDrawGizmosSelected()
		{
			int num = 16;
			for (int i = 0; i < this.splines.Count; i++)
			{
				CatmullRomSpline catmullRomSpline = this.splines[i];
				Vector3 vector = catmullRomSpline.Evaluate(0f);
				for (int j = 1; j <= num; j++)
				{
					float num2 = (float)j / (float)num;
					Vector3 vector2 = catmullRomSpline.Evaluate(num2);
					vector2 - vector;
					Quaternion quaternion = Quaternion.LookRotation(catmullRomSpline.GetForwardTangent(num2, 0.01f), Vector3.up);
					Gizmos.color = new Color(0f, 0.5f, 0.75f);
					this.DrawGizmoCircle(vector2, quaternion, this.fullEffectDistance);
					Gizmos.color = new Color(0f, 0.25f, 0.5f);
					this.DrawGizmoCircle(vector2, quaternion, this.fullEffectDistance + this.fadeDistance);
				}
			}
		}

		// Token: 0x0600515B RID: 20827 RVA: 0x00189F6C File Offset: 0x0018816C
		private void DrawGizmoCircle(Vector3 center, Quaternion rotation, float radius)
		{
			Vector3 vector = Vector3.right * radius;
			int num = 16;
			for (int i = 1; i <= num; i++)
			{
				float num2 = (float)i / (float)num * 2f * 3.1415927f;
				Vector3 vector2 = new Vector3(Mathf.Cos(num2), Mathf.Sin(num2), 0f) * radius;
				Gizmos.DrawLine(center + rotation * vector, center + rotation * vector2);
				vector = vector2;
			}
		}

		// Token: 0x04005554 RID: 21844
		[SerializeField]
		private List<CatmullRomSpline> splines = new List<CatmullRomSpline>();

		// Token: 0x04005555 RID: 21845
		[SerializeField]
		private float fullEffectDistance = 1f;

		// Token: 0x04005556 RID: 21846
		[SerializeField]
		private float fadeDistance = 0.5f;

		// Token: 0x04005557 RID: 21847
		[SerializeField]
		private float currentSpeed = 1f;

		// Token: 0x04005558 RID: 21848
		[SerializeField]
		private float currentAccel = 10f;

		// Token: 0x04005559 RID: 21849
		[SerializeField]
		private float velocityAnticipationAdjustment = 0.05f;

		// Token: 0x0400555A RID: 21850
		[SerializeField]
		private float inwardCurrentFullEffectRadius = 1f;

		// Token: 0x0400555B RID: 21851
		[SerializeField]
		private float inwardCurrentNoEffectRadius = 0.25f;

		// Token: 0x0400555C RID: 21852
		[SerializeField]
		private float inwardCurrentSpeed = 1f;

		// Token: 0x0400555D RID: 21853
		[SerializeField]
		private float inwardCurrentAccel = 10f;

		// Token: 0x0400555E RID: 21854
		[SerializeField]
		private float dampingHalfLife = 0.25f;

		// Token: 0x0400555F RID: 21855
		[SerializeField]
		private bool debugDrawCurrentQueries;

		// Token: 0x04005560 RID: 21856
		private Vector3 debugCurrentVelocity = Vector3.zero;

		// Token: 0x04005561 RID: 21857
		private Vector3 debugSplinePoint = Vector3.zero;
	}
}
