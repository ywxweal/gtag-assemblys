using System;
using System.Collections.Generic;
using AA;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000CCB RID: 3275
	[RequireComponent(typeof(Rigidbody))]
	public class RigidbodyWaterInteraction : MonoBehaviour
	{
		// Token: 0x06005136 RID: 20790 RVA: 0x0018852E File Offset: 0x0018672E
		protected void Awake()
		{
			this.rb = base.GetComponent<Rigidbody>();
			this.baseAngularDrag = this.rb.angularDrag;
			RigidbodyWaterInteractionManager.RegisterRBWI(this);
		}

		// Token: 0x06005137 RID: 20791 RVA: 0x00188553 File Offset: 0x00186753
		protected void OnEnable()
		{
			this.overlappingWaterVolumes.Clear();
			RigidbodyWaterInteractionManager.RegisterRBWI(this);
		}

		// Token: 0x06005138 RID: 20792 RVA: 0x00188566 File Offset: 0x00186766
		protected void OnDisable()
		{
			this.overlappingWaterVolumes.Clear();
			RigidbodyWaterInteractionManager.UnregisterRBWI(this);
		}

		// Token: 0x06005139 RID: 20793 RVA: 0x00188579 File Offset: 0x00186779
		private void OnDestroy()
		{
			RigidbodyWaterInteractionManager.UnregisterRBWI(this);
		}

		// Token: 0x0600513A RID: 20794 RVA: 0x00188584 File Offset: 0x00186784
		public void InvokeFixedUpdate()
		{
			if (this.rb.isKinematic)
			{
				return;
			}
			bool flag = this.overlappingWaterVolumes.Count > 0;
			WaterVolume.SurfaceQuery surfaceQuery = default(WaterVolume.SurfaceQuery);
			float num = float.MinValue;
			if (flag && this.enablePreciseWaterCollision)
			{
				Vector3 vector = base.transform.position + Vector3.down * 2f * this.objectRadiusForWaterCollision * this.buoyancyEquilibrium;
				bool flag2 = false;
				this.activeWaterCurrents.Clear();
				for (int i = 0; i < this.overlappingWaterVolumes.Count; i++)
				{
					WaterVolume.SurfaceQuery surfaceQuery2;
					if (this.overlappingWaterVolumes[i].GetSurfaceQueryForPoint(vector, out surfaceQuery2, false))
					{
						float num2 = Vector3.Dot(surfaceQuery2.surfacePoint - vector, surfaceQuery2.surfaceNormal);
						if (num2 > num)
						{
							num = num2;
							surfaceQuery = surfaceQuery2;
							flag2 = true;
						}
						WaterCurrent waterCurrent = this.overlappingWaterVolumes[i].Current;
						if (this.applyWaterCurrents && waterCurrent != null && num2 > 0f && !this.activeWaterCurrents.Contains(waterCurrent))
						{
							this.activeWaterCurrents.Add(waterCurrent);
						}
					}
				}
				if (flag2)
				{
					bool flag3 = num > -(1f - this.buoyancyEquilibrium) * 2f * this.objectRadiusForWaterCollision;
					float num3 = (this.enablePreciseWaterCollision ? this.objectRadiusForWaterCollision : 0f);
					bool flag4 = base.transform.position.y + num3 - (surfaceQuery.surfacePoint.y - surfaceQuery.maxDepth) > 0f;
					flag = flag3 && flag4;
				}
				else
				{
					flag = false;
				}
			}
			if (flag)
			{
				float fixedDeltaTime = Time.fixedDeltaTime;
				Vector3 vector2 = this.rb.velocity;
				Vector3 vector3 = Vector3.zero;
				if (this.applyWaterCurrents)
				{
					Vector3 vector4 = Vector3.zero;
					for (int j = 0; j < this.activeWaterCurrents.Count; j++)
					{
						WaterCurrent waterCurrent2 = this.activeWaterCurrents[j];
						Vector3 vector5 = vector2 + vector3;
						Vector3 vector6;
						Vector3 vector7;
						if (waterCurrent2.GetCurrentAtPoint(base.transform.position, vector5, fixedDeltaTime, out vector6, out vector7))
						{
							vector4 += vector6;
							vector3 += vector7;
						}
					}
					if (this.enablePreciseWaterCollision)
					{
						Vector3 vector8 = (surfaceQuery.surfacePoint + (base.transform.position + Vector3.down * this.objectRadiusForWaterCollision)) * 0.5f;
						this.rb.AddForceAtPosition(vector3, vector8, ForceMode.VelocityChange);
					}
					else
					{
						vector2 += vector3;
					}
				}
				if (this.applyBuoyancyForce)
				{
					Vector3 vector9 = Vector3.zero;
					if (this.enablePreciseWaterCollision)
					{
						float num4 = 2f * this.objectRadiusForWaterCollision * this.buoyancyEquilibrium;
						float num5 = Mathf.InverseLerp(0f, num4, num);
						vector9 = -Physics.gravity * this.underWaterBuoyancyFactor * num5 * fixedDeltaTime;
					}
					else
					{
						vector9 = -Physics.gravity * this.underWaterBuoyancyFactor * fixedDeltaTime;
					}
					if (vector3.sqrMagnitude > 0.001f)
					{
						float magnitude = vector3.magnitude;
						Vector3 vector10 = vector3 / magnitude;
						float num6 = Vector3.Dot(vector9, vector10);
						if (num6 < 0f)
						{
							vector9 -= num6 * vector10;
						}
					}
					vector2 += vector9;
				}
				float magnitude2 = vector2.magnitude;
				if (magnitude2 > 0.001f && this.applyDamping)
				{
					Vector3 vector11 = vector2 / magnitude2;
					float num7 = Spring.DamperDecayExact(magnitude2, this.underWaterDampingHalfLife, fixedDeltaTime, 1E-05f);
					if (this.enablePreciseWaterCollision)
					{
						float num8 = Spring.DamperDecayExact(magnitude2, this.waterSurfaceDampingHalfLife, fixedDeltaTime, 1E-05f);
						float num9 = Mathf.Clamp(-(base.transform.position.y - surfaceQuery.surfacePoint.y) / this.objectRadiusForWaterCollision, -1f, 1f) * 0.5f + 0.5f;
						vector2 = Mathf.Lerp(num8, num7, num9) * vector11;
					}
					else
					{
						vector2 = num7 * vector11;
					}
				}
				if (this.applySurfaceTorque && this.enablePreciseWaterCollision)
				{
					float num10 = base.transform.position.y - surfaceQuery.surfacePoint.y;
					if (num10 < this.objectRadiusForWaterCollision && num10 > 0f)
					{
						Vector3 vector12 = vector2 - Vector3.Dot(vector2, surfaceQuery.surfaceNormal) * surfaceQuery.surfaceNormal;
						Vector3 normalized = Vector3.Cross(surfaceQuery.surfaceNormal, vector12).normalized;
						float num11 = Vector3.Dot(this.rb.angularVelocity, normalized);
						float num12 = vector12.magnitude / this.objectRadiusForWaterCollision - num11;
						if (num12 > 0f)
						{
							this.rb.AddTorque(this.surfaceTorqueAmount * num12 * normalized, ForceMode.Acceleration);
						}
					}
				}
				this.rb.velocity = vector2;
				this.rb.angularDrag = this.angularDrag;
				return;
			}
			this.rb.angularDrag = this.baseAngularDrag;
		}

		// Token: 0x0600513B RID: 20795 RVA: 0x00188AB4 File Offset: 0x00186CB4
		protected void OnTriggerEnter(Collider other)
		{
			WaterVolume component = other.GetComponent<WaterVolume>();
			if (component != null && !this.overlappingWaterVolumes.Contains(component))
			{
				this.overlappingWaterVolumes.Add(component);
			}
		}

		// Token: 0x0600513C RID: 20796 RVA: 0x00188AEC File Offset: 0x00186CEC
		protected void OnTriggerExit(Collider other)
		{
			WaterVolume component = other.GetComponent<WaterVolume>();
			if (component != null && this.overlappingWaterVolumes.Contains(component))
			{
				this.overlappingWaterVolumes.Remove(component);
			}
		}

		// Token: 0x04005522 RID: 21794
		public bool applyDamping = true;

		// Token: 0x04005523 RID: 21795
		public bool applyBuoyancyForce = true;

		// Token: 0x04005524 RID: 21796
		public bool applyAngularDrag = true;

		// Token: 0x04005525 RID: 21797
		public bool applyWaterCurrents = true;

		// Token: 0x04005526 RID: 21798
		public bool applySurfaceTorque = true;

		// Token: 0x04005527 RID: 21799
		public float underWaterDampingHalfLife = 0.25f;

		// Token: 0x04005528 RID: 21800
		public float waterSurfaceDampingHalfLife = 1f;

		// Token: 0x04005529 RID: 21801
		public float underWaterBuoyancyFactor = 0.5f;

		// Token: 0x0400552A RID: 21802
		public float angularDrag = 0.5f;

		// Token: 0x0400552B RID: 21803
		public float surfaceTorqueAmount = 0.5f;

		// Token: 0x0400552C RID: 21804
		public bool enablePreciseWaterCollision;

		// Token: 0x0400552D RID: 21805
		public float objectRadiusForWaterCollision = 0.25f;

		// Token: 0x0400552E RID: 21806
		[Range(0f, 1f)]
		public float buoyancyEquilibrium = 0.8f;

		// Token: 0x0400552F RID: 21807
		private Rigidbody rb;

		// Token: 0x04005530 RID: 21808
		private List<WaterVolume> overlappingWaterVolumes = new List<WaterVolume>();

		// Token: 0x04005531 RID: 21809
		private List<WaterCurrent> activeWaterCurrents = new List<WaterCurrent>(16);

		// Token: 0x04005532 RID: 21810
		private float baseAngularDrag = 0.05f;
	}
}
