﻿using System;
using CjLib;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000CCF RID: 3279
	public class UnderwaterParticleEffects : MonoBehaviour
	{
		// Token: 0x06005151 RID: 20817 RVA: 0x00189588 File Offset: 0x00187788
		public void UpdateParticleEffect(bool waterSurfaceDetected, ref WaterVolume.SurfaceQuery waterSurface)
		{
			GTPlayer instance = GTPlayer.Instance;
			Plane plane = new Plane(waterSurface.surfaceNormal, waterSurface.surfacePoint);
			if (waterSurfaceDetected && plane.GetDistanceToPoint(instance.headCollider.transform.position) < instance.headCollider.radius)
			{
				this.underwaterFloaterParticles.gameObject.SetActive(true);
				Vector3 averagedVelocity = instance.AveragedVelocity;
				float magnitude = averagedVelocity.magnitude;
				Vector3 vector = ((magnitude > 0.001f) ? (averagedVelocity / magnitude) : this.playerCamera.transform.forward);
				float num = this.floaterSpeedVsOffsetDist.Evaluate(Mathf.Clamp(magnitude, this.floaterSpeedVsOffsetDistMinMax.x, this.floaterSpeedVsOffsetDistMinMax.y));
				Quaternion rotation = this.playerCamera.transform.rotation;
				Vector3 vector2 = this.playerCamera.transform.position + this.playerCamera.transform.rotation * this.floaterParticleBaseOffset + vector * num;
				Vector3 vector3 = vector2 + rotation * new Vector3(0f, this.floaterParticleBoxExtents.y, -this.floaterParticleBoxExtents.z);
				Vector3 vector4 = vector2 + rotation * new Vector3(0f, this.floaterParticleBoxExtents.y, this.floaterParticleBoxExtents.z);
				float num2 = this.floaterParticleBoxExtents.z * 2f;
				float num3 = plane.GetDistanceToPoint(vector3);
				float num4 = plane.GetDistanceToPoint(vector4);
				Quaternion quaternion = rotation;
				Vector3 vector5 = vector2;
				if (num3 > 0f || num4 > 0f)
				{
					if (vector3.y < vector4.y)
					{
						if (num3 > 0f)
						{
							vector3 -= plane.normal * num3;
							num3 = 0f;
						}
						Vector3 vector6 = (new Vector3(vector4.x, vector3.y, vector4.z) - vector3).normalized * num2;
						Vector3 vector7 = Vector3.Cross(vector4 - vector3, vector6);
						quaternion = Quaternion.AngleAxis((Mathf.Asin((vector4.y - vector3.y) / num2) - Mathf.Asin(-num3 / num2)) * 57.29578f, vector7) * this.playerCamera.transform.rotation;
						vector5 = vector3 + quaternion * new Vector3(0f, -this.floaterParticleBoxExtents.y, this.floaterParticleBoxExtents.z);
					}
					else
					{
						if (num4 > 0f)
						{
							vector4 -= plane.normal * num4;
							num4 = 0f;
						}
						Vector3 vector8 = (new Vector3(vector3.x, vector4.y, vector3.z) - vector4).normalized * num2;
						Vector3 vector9 = Vector3.Cross(vector3 - vector4, vector8);
						quaternion = Quaternion.AngleAxis((Mathf.Asin((vector3.y - vector4.y) / num2) - Mathf.Asin(-num4 / num2)) * 57.29578f, vector9) * this.playerCamera.transform.rotation;
						vector5 = vector4 + quaternion * new Vector3(0f, -this.floaterParticleBoxExtents.y, -this.floaterParticleBoxExtents.z);
					}
				}
				if (this.IsValid(vector5))
				{
					this.underwaterFloaterParticles.transform.rotation = quaternion;
					this.underwaterFloaterParticles.transform.position = vector5;
				}
				else
				{
					this.underwaterFloaterParticles.gameObject.SetActive(false);
				}
				if (this.debugDraw)
				{
					vector3 = vector2 + rotation * new Vector3(0f, this.floaterParticleBoxExtents.y, -this.floaterParticleBoxExtents.z);
					vector4 = vector2 + rotation * new Vector3(0f, this.floaterParticleBoxExtents.y, this.floaterParticleBoxExtents.z);
					DebugUtil.DrawSphere(vector3, 0.1f, 12, 12, Color.red, false, DebugUtil.Style.SolidColor);
					DebugUtil.DrawSphere(vector4, 0.1f, 12, 12, Color.red, false, DebugUtil.Style.SolidColor);
					DebugUtil.DrawLine(vector3, vector4, Color.red, false);
					vector3 = vector5 + quaternion * new Vector3(0f, this.floaterParticleBoxExtents.y, -this.floaterParticleBoxExtents.z);
					vector4 = vector5 + quaternion * new Vector3(0f, this.floaterParticleBoxExtents.y, this.floaterParticleBoxExtents.z);
					DebugUtil.DrawSphere(vector3, 0.1f, 12, 12, Color.green, false, DebugUtil.Style.SolidColor);
					DebugUtil.DrawSphere(vector4, 0.1f, 12, 12, Color.green, false, DebugUtil.Style.SolidColor);
					DebugUtil.DrawLine(vector3, vector4, Color.green, false);
					return;
				}
			}
			else
			{
				this.underwaterFloaterParticles.gameObject.SetActive(false);
			}
		}

		// Token: 0x06005152 RID: 20818 RVA: 0x00189ABD File Offset: 0x00187CBD
		private bool IsValid(Vector3 vector)
		{
			return !float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z);
		}

		// Token: 0x0400554C RID: 21836
		public ParticleSystem underwaterFloaterParticles;

		// Token: 0x0400554D RID: 21837
		public ParticleSystem underwaterBubbleParticles;

		// Token: 0x0400554E RID: 21838
		public Camera playerCamera;

		// Token: 0x0400554F RID: 21839
		public Vector3 floaterParticleBoxExtents = Vector3.one;

		// Token: 0x04005550 RID: 21840
		public Vector3 floaterParticleBaseOffset = Vector3.forward;

		// Token: 0x04005551 RID: 21841
		public AnimationCurve floaterSpeedVsOffsetDist = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04005552 RID: 21842
		public Vector2 floaterSpeedVsOffsetDistMinMax = new Vector2(0f, 1f);

		// Token: 0x04005553 RID: 21843
		private bool debugDraw;
	}
}
