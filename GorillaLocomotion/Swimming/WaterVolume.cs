using System;
using System.Collections.Generic;
using CjLib;
using GorillaLocomotion.Climbing;
using GorillaTag.GuidedRefs;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000CD4 RID: 3284
	[RequireComponent(typeof(Collider))]
	public class WaterVolume : BaseGuidedRefTargetMono
	{
		// Token: 0x14000096 RID: 150
		// (add) Token: 0x06005164 RID: 20836 RVA: 0x0018A4C8 File Offset: 0x001886C8
		// (remove) Token: 0x06005165 RID: 20837 RVA: 0x0018A500 File Offset: 0x00188700
		public event WaterVolume.WaterVolumeEvent ColliderEnteredVolume;

		// Token: 0x14000097 RID: 151
		// (add) Token: 0x06005166 RID: 20838 RVA: 0x0018A538 File Offset: 0x00188738
		// (remove) Token: 0x06005167 RID: 20839 RVA: 0x0018A570 File Offset: 0x00188770
		public event WaterVolume.WaterVolumeEvent ColliderExitedVolume;

		// Token: 0x14000098 RID: 152
		// (add) Token: 0x06005168 RID: 20840 RVA: 0x0018A5A8 File Offset: 0x001887A8
		// (remove) Token: 0x06005169 RID: 20841 RVA: 0x0018A5E0 File Offset: 0x001887E0
		public event WaterVolume.WaterVolumeEvent ColliderEnteredWater;

		// Token: 0x14000099 RID: 153
		// (add) Token: 0x0600516A RID: 20842 RVA: 0x0018A618 File Offset: 0x00188818
		// (remove) Token: 0x0600516B RID: 20843 RVA: 0x0018A650 File Offset: 0x00188850
		public event WaterVolume.WaterVolumeEvent ColliderExitedWater;

		// Token: 0x1700083D RID: 2109
		// (get) Token: 0x0600516C RID: 20844 RVA: 0x0018A685 File Offset: 0x00188885
		public GTPlayer.LiquidType LiquidType
		{
			get
			{
				return this.liquidType;
			}
		}

		// Token: 0x1700083E RID: 2110
		// (get) Token: 0x0600516D RID: 20845 RVA: 0x0018A68D File Offset: 0x0018888D
		public WaterCurrent Current
		{
			get
			{
				return this.waterCurrent;
			}
		}

		// Token: 0x1700083F RID: 2111
		// (get) Token: 0x0600516E RID: 20846 RVA: 0x0018A695 File Offset: 0x00188895
		public WaterParameters Parameters
		{
			get
			{
				return this.waterParams;
			}
		}

		// Token: 0x17000840 RID: 2112
		// (get) Token: 0x0600516F RID: 20847 RVA: 0x0018A6A0 File Offset: 0x001888A0
		private VRRig PlayerVRRig
		{
			get
			{
				if (this.playerVRRig == null)
				{
					GorillaTagger instance = GorillaTagger.Instance;
					if (instance != null)
					{
						this.playerVRRig = instance.offlineVRRig;
					}
				}
				return this.playerVRRig;
			}
		}

		// Token: 0x06005170 RID: 20848 RVA: 0x0018A6DC File Offset: 0x001888DC
		public bool GetSurfaceQueryForPoint(Vector3 point, out WaterVolume.SurfaceQuery result, bool debugDraw = false)
		{
			result = default(WaterVolume.SurfaceQuery);
			if (!this.isStationary)
			{
				float num = float.MinValue;
				float num2 = float.MaxValue;
				for (int i = 0; i < this.volumeColliders.Count; i++)
				{
					float y = this.volumeColliders[i].bounds.max.y;
					float y2 = this.volumeColliders[i].bounds.min.y;
					if (y > num)
					{
						num = y;
					}
					if (y2 < num2)
					{
						num2 = y2;
					}
				}
				this.volumeMaxHeight = num;
				this.volumeMinHeight = num2;
			}
			Ray ray = new Ray(new Vector3(point.x, this.volumeMaxHeight, point.z), Vector3.down);
			Ray ray2 = new Ray(new Vector3(point.x, this.volumeMinHeight, point.z), Vector3.up);
			float num3 = this.volumeMaxHeight - this.volumeMinHeight;
			float num4 = float.MinValue;
			float num5 = float.MaxValue;
			bool flag = false;
			bool flag2 = false;
			float num6 = 0f;
			for (int j = 0; j < this.surfaceColliders.Count; j++)
			{
				bool enabled = this.surfaceColliders[j].enabled;
				this.surfaceColliders[j].enabled = true;
				RaycastHit raycastHit;
				if (this.surfaceColliders[j].Raycast(ray, out raycastHit, num3) && raycastHit.point.y > num4 && this.HitOutsideSurfaceOfMesh(ray.direction, this.surfaceColliders[j], raycastHit))
				{
					num4 = raycastHit.point.y;
					flag = true;
					result.surfacePoint = raycastHit.point;
					result.surfaceNormal = raycastHit.normal;
				}
				RaycastHit raycastHit2;
				if (this.surfaceColliders[j].Raycast(ray2, out raycastHit2, num3) && raycastHit2.point.y < num5 && this.HitOutsideSurfaceOfMesh(ray2.direction, this.surfaceColliders[j], raycastHit2))
				{
					num5 = raycastHit2.point.y;
					flag2 = true;
					num6 = raycastHit2.point.y;
				}
				this.surfaceColliders[j].enabled = enabled;
			}
			if (!flag && this.surfacePlane != null)
			{
				flag = true;
				result.surfacePoint = point - Vector3.Dot(point - this.surfacePlane.position, this.surfacePlane.up) * this.surfacePlane.up;
				result.surfaceNormal = this.surfacePlane.up;
			}
			if (flag && flag2)
			{
				result.maxDepth = result.surfacePoint.y - num6;
			}
			else if (flag)
			{
				result.maxDepth = result.surfacePoint.y - this.volumeMinHeight;
			}
			else
			{
				result.maxDepth = this.volumeMaxHeight - this.volumeMinHeight;
			}
			if (debugDraw)
			{
				if (flag)
				{
					DebugUtil.DrawLine(ray.origin, ray.origin + ray.direction * num3, Color.green, false);
					DebugUtil.DrawSphere(result.surfacePoint, 0.001f, 12, 12, Color.green, false, DebugUtil.Style.SolidColor);
				}
				else
				{
					DebugUtil.DrawLine(ray.origin, ray.origin + ray.direction * num3, Color.red, false);
				}
				if (flag2)
				{
					DebugUtil.DrawLine(ray2.origin, ray2.origin + ray2.direction * num3, Color.yellow, false);
					DebugUtil.DrawSphere(new Vector3(result.surfacePoint.x, num6, result.surfacePoint.z), 0.001f, 12, 12, Color.yellow, false, DebugUtil.Style.SolidColor);
				}
			}
			return flag;
		}

		// Token: 0x06005171 RID: 20849 RVA: 0x0018AABC File Offset: 0x00188CBC
		private bool HitOutsideSurfaceOfMesh(Vector3 castDir, MeshCollider meshCollider, RaycastHit hit)
		{
			if (!WaterVolume.meshTrianglesDict.TryGetValue(meshCollider.sharedMesh, out this.sharedMeshTris))
			{
				this.sharedMeshTris = (int[])meshCollider.sharedMesh.triangles.Clone();
				WaterVolume.meshTrianglesDict.Add(meshCollider.sharedMesh, this.sharedMeshTris);
			}
			if (!WaterVolume.meshVertsDict.TryGetValue(meshCollider.sharedMesh, out this.sharedMeshVerts))
			{
				this.sharedMeshVerts = (Vector3[])meshCollider.sharedMesh.vertices.Clone();
				WaterVolume.meshVertsDict.Add(meshCollider.sharedMesh, this.sharedMeshVerts);
			}
			Vector3 vector = this.sharedMeshVerts[this.sharedMeshTris[hit.triangleIndex * 3]];
			Vector3 vector2 = this.sharedMeshVerts[this.sharedMeshTris[hit.triangleIndex * 3 + 1]];
			Vector3 vector3 = this.sharedMeshVerts[this.sharedMeshTris[hit.triangleIndex * 3 + 2]];
			Vector3 vector4 = meshCollider.transform.TransformDirection(Vector3.Cross(vector2 - vector, vector3 - vector).normalized);
			bool flag = Vector3.Dot(castDir, vector4) < 0f;
			if (this.debugDrawSurfaceCast)
			{
				Color color = (flag ? Color.blue : Color.red);
				DebugUtil.DrawLine(hit.point, hit.point + vector4 * 0.3f, color, false);
			}
			return flag;
		}

		// Token: 0x06005172 RID: 20850 RVA: 0x0018AC30 File Offset: 0x00188E30
		private void DebugDrawMeshColliderHitTriangle(RaycastHit hit)
		{
			MeshCollider meshCollider = hit.collider as MeshCollider;
			if (meshCollider != null)
			{
				Mesh sharedMesh = meshCollider.sharedMesh;
				int[] triangles = sharedMesh.triangles;
				Vector3[] vertices = sharedMesh.vertices;
				Vector3 vector = meshCollider.gameObject.transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3]]);
				Vector3 vector2 = meshCollider.gameObject.transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 1]]);
				Vector3 vector3 = meshCollider.gameObject.transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 2]]);
				Vector3 normalized = Vector3.Cross(vector2 - vector, vector3 - vector).normalized;
				float num = 0.2f;
				DebugUtil.DrawLine(vector, vector + normalized * num, Color.blue, false);
				DebugUtil.DrawLine(vector2, vector2 + normalized * num, Color.blue, false);
				DebugUtil.DrawLine(vector3, vector3 + normalized * num, Color.blue, false);
				DebugUtil.DrawLine(vector, vector2, Color.blue, false);
				DebugUtil.DrawLine(vector, vector3, Color.blue, false);
				DebugUtil.DrawLine(vector2, vector3, Color.blue, false);
			}
		}

		// Token: 0x06005173 RID: 20851 RVA: 0x0018AD7C File Offset: 0x00188F7C
		public bool RaycastWater(Vector3 origin, Vector3 direction, out RaycastHit hit, float distance, int layerMask)
		{
			if (this.triggerCollider != null)
			{
				return Physics.Raycast(new Ray(origin, direction), out hit, distance, layerMask, QueryTriggerInteraction.Collide);
			}
			hit = default(RaycastHit);
			return false;
		}

		// Token: 0x06005174 RID: 20852 RVA: 0x0018ADA8 File Offset: 0x00188FA8
		public bool CheckColliderInVolume(Collider collider, out bool inWater, out bool surfaceDetected)
		{
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				if (this.persistentColliders[i].collider == collider)
				{
					inWater = this.persistentColliders[i].inWater;
					surfaceDetected = this.persistentColliders[i].surfaceDetected;
					return true;
				}
			}
			inWater = false;
			surfaceDetected = false;
			return false;
		}

		// Token: 0x06005175 RID: 20853 RVA: 0x0018AE13 File Offset: 0x00189013
		protected override void Awake()
		{
			base.Awake();
			this.RefreshColliders();
		}

		// Token: 0x06005176 RID: 20854 RVA: 0x0018AE24 File Offset: 0x00189024
		public void RefreshColliders()
		{
			this.triggerCollider = base.GetComponent<Collider>();
			if (this.volumeColliders == null || this.volumeColliders.Count < 1)
			{
				this.volumeColliders = new List<Collider>();
				this.volumeColliders.Add(base.gameObject.GetComponent<Collider>());
			}
			float num = float.MinValue;
			float num2 = float.MaxValue;
			for (int i = 0; i < this.volumeColliders.Count; i++)
			{
				float y = this.volumeColliders[i].bounds.max.y;
				float y2 = this.volumeColliders[i].bounds.min.y;
				if (y > num)
				{
					num = y;
				}
				if (y2 < num2)
				{
					num2 = y2;
				}
			}
			this.volumeMaxHeight = num;
			this.volumeMinHeight = num2;
		}

		// Token: 0x06005177 RID: 20855 RVA: 0x0018AEF4 File Offset: 0x001890F4
		private void OnDisable()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				WaterOverlappingCollider waterOverlappingCollider = this.persistentColliders[i];
				waterOverlappingCollider.inVolume = false;
				waterOverlappingCollider.playDripEffect = false;
				WaterVolume.WaterVolumeEvent colliderExitedVolume = this.ColliderExitedVolume;
				if (colliderExitedVolume != null)
				{
					colliderExitedVolume(this, waterOverlappingCollider.collider);
				}
				this.persistentColliders[i] = waterOverlappingCollider;
			}
			this.RemoveCollidersOutsideVolume(Time.time);
		}

		// Token: 0x06005178 RID: 20856 RVA: 0x0018AF6C File Offset: 0x0018916C
		private void Update()
		{
			if (this.persistentColliders.Count < 1)
			{
				return;
			}
			float time = Time.time;
			this.RemoveCollidersOutsideVolume(time);
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				WaterOverlappingCollider waterOverlappingCollider = this.persistentColliders[i];
				bool inWater = waterOverlappingCollider.inWater;
				if (waterOverlappingCollider.inVolume)
				{
					this.CheckColliderAgainstWater(ref waterOverlappingCollider, time);
				}
				else
				{
					waterOverlappingCollider.inWater = false;
				}
				this.TryRegisterOwnershipOfCollider(waterOverlappingCollider.collider, waterOverlappingCollider.inWater, waterOverlappingCollider.surfaceDetected);
				if (waterOverlappingCollider.inWater && !inWater)
				{
					this.OnWaterSurfaceEnter(ref waterOverlappingCollider);
				}
				else if (!waterOverlappingCollider.inWater && inWater)
				{
					this.OnWaterSurfaceExit(ref waterOverlappingCollider, time);
				}
				if (this.HasOwnershipOfCollider(waterOverlappingCollider.collider) && waterOverlappingCollider.surfaceDetected)
				{
					if (!waterOverlappingCollider.inWater)
					{
						this.ColliderOutOfWaterUpdate(ref waterOverlappingCollider, time);
					}
					else
					{
						this.ColliderInWaterUpdate(ref waterOverlappingCollider, time);
					}
				}
				this.persistentColliders[i] = waterOverlappingCollider;
			}
		}

		// Token: 0x06005179 RID: 20857 RVA: 0x0018B064 File Offset: 0x00189264
		private void RemoveCollidersOutsideVolume(float currentTime)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			for (int i = this.persistentColliders.Count - 1; i >= 0; i--)
			{
				WaterOverlappingCollider waterOverlappingCollider = this.persistentColliders[i];
				if (waterOverlappingCollider.collider == null || !waterOverlappingCollider.collider.gameObject.activeInHierarchy || (!waterOverlappingCollider.inVolume && (!waterOverlappingCollider.playDripEffect || currentTime - waterOverlappingCollider.lastInWaterTime > this.waterParams.postExitDripDuration)))
				{
					this.UnregisterOwnershipOfCollider(waterOverlappingCollider.collider);
					GTPlayer instance = GTPlayer.Instance;
					if (waterOverlappingCollider.collider == instance.headCollider || waterOverlappingCollider.collider == instance.bodyCollider)
					{
						instance.OnExitWaterVolume(waterOverlappingCollider.collider, this);
					}
					this.persistentColliders.RemoveAt(i);
				}
			}
		}

		// Token: 0x0600517A RID: 20858 RVA: 0x0018B13C File Offset: 0x0018933C
		private void CheckColliderAgainstWater(ref WaterOverlappingCollider persistentCollider, float currentTime)
		{
			Vector3 position = persistentCollider.collider.transform.position;
			bool flag = true;
			if (persistentCollider.surfaceDetected && persistentCollider.scaleMultiplier > 0.99f && this.isStationary)
			{
				flag = (position - Vector3.Dot(position - persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal) * persistentCollider.lastSurfaceQuery.surfaceNormal - persistentCollider.lastSurfaceQuery.surfacePoint).sqrMagnitude > this.waterParams.recomputeSurfaceForColliderDist * this.waterParams.recomputeSurfaceForColliderDist;
			}
			if (flag)
			{
				WaterVolume.SurfaceQuery surfaceQuery;
				if (this.GetSurfaceQueryForPoint(position, out surfaceQuery, this.debugDrawSurfaceCast))
				{
					persistentCollider.surfaceDetected = true;
					persistentCollider.lastSurfaceQuery = surfaceQuery;
				}
				else
				{
					persistentCollider.surfaceDetected = false;
					persistentCollider.lastSurfaceQuery = default(WaterVolume.SurfaceQuery);
				}
			}
			if (persistentCollider.surfaceDetected)
			{
				bool flag2 = ((persistentCollider.collider is MeshCollider) ? persistentCollider.collider.ClosestPointOnBounds(position + Vector3.down * 10f) : persistentCollider.collider.ClosestPoint(position + Vector3.down * 10f)).y < persistentCollider.lastSurfaceQuery.surfacePoint.y;
				bool flag3 = ((persistentCollider.collider is MeshCollider) ? persistentCollider.collider.ClosestPointOnBounds(position + Vector3.up * 10f) : persistentCollider.collider.ClosestPoint(position + Vector3.up * 10f)).y > persistentCollider.lastSurfaceQuery.surfacePoint.y - persistentCollider.lastSurfaceQuery.maxDepth;
				persistentCollider.inWater = flag2 && flag3;
			}
			else
			{
				persistentCollider.inWater = false;
			}
			if (persistentCollider.inWater)
			{
				persistentCollider.lastInWaterTime = currentTime;
			}
		}

		// Token: 0x0600517B RID: 20859 RVA: 0x0018B324 File Offset: 0x00189524
		private Vector3 GetColliderVelocity(ref WaterOverlappingCollider persistentCollider)
		{
			GTPlayer instance = GTPlayer.Instance;
			Vector3 vector = Vector3.one * (this.waterParams.splashSpeedRequirement + 0.1f);
			if (persistentCollider.velocityTracker != null)
			{
				vector = persistentCollider.velocityTracker.GetAverageVelocity(true, 0.1f, false);
			}
			else if (persistentCollider.collider == instance.headCollider || persistentCollider.collider == instance.bodyCollider)
			{
				vector = instance.AveragedVelocity;
			}
			else if (persistentCollider.collider.attachedRigidbody != null && !persistentCollider.collider.attachedRigidbody.isKinematic)
			{
				vector = persistentCollider.collider.attachedRigidbody.velocity;
			}
			return vector;
		}

		// Token: 0x0600517C RID: 20860 RVA: 0x0018B3DC File Offset: 0x001895DC
		private void OnWaterSurfaceEnter(ref WaterOverlappingCollider persistentCollider)
		{
			WaterVolume.WaterVolumeEvent colliderEnteredWater = this.ColliderEnteredWater;
			if (colliderEnteredWater != null)
			{
				colliderEnteredWater(this, persistentCollider.collider);
			}
			GTPlayer instance = GTPlayer.Instance;
			if (persistentCollider.collider == instance.headCollider || persistentCollider.collider == instance.bodyCollider)
			{
				instance.OnEnterWaterVolume(persistentCollider.collider, this);
			}
			if (this.HasOwnershipOfCollider(persistentCollider.collider))
			{
				Vector3 colliderVelocity = this.GetColliderVelocity(ref persistentCollider);
				bool flag = Vector3.Dot(colliderVelocity, -persistentCollider.lastSurfaceQuery.surfaceNormal) > this.waterParams.splashSpeedRequirement * persistentCollider.scaleMultiplier;
				bool flag2 = Vector3.Dot(colliderVelocity, -persistentCollider.lastSurfaceQuery.surfaceNormal) > this.waterParams.bigSplashSpeedRequirement * persistentCollider.scaleMultiplier;
				persistentCollider.PlayRippleEffect(this.waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, this.waterParams.rippleEffectScale, Time.time, this);
				if (this.waterParams.playSplashEffect && flag && (flag2 || !persistentCollider.playBigSplash))
				{
					persistentCollider.PlaySplashEffect(this.waterParams.splashEffect, persistentCollider.lastRipplePosition, this.waterParams.splashEffectScale, persistentCollider.playBigSplash && flag2, true, this);
				}
			}
		}

		// Token: 0x0600517D RID: 20861 RVA: 0x0018B528 File Offset: 0x00189728
		private void OnWaterSurfaceExit(ref WaterOverlappingCollider persistentCollider, float currentTime)
		{
			WaterVolume.WaterVolumeEvent colliderExitedWater = this.ColliderExitedWater;
			if (colliderExitedWater != null)
			{
				colliderExitedWater(this, persistentCollider.collider);
			}
			persistentCollider.nextDripTime = currentTime + this.waterParams.perDripTimeDelay + Random.Range(-this.waterParams.perDripTimeRandRange * 0.5f, this.waterParams.perDripTimeRandRange * 0.5f);
			GTPlayer instance = GTPlayer.Instance;
			if (persistentCollider.collider == instance.headCollider || persistentCollider.collider == instance.bodyCollider)
			{
				instance.OnExitWaterVolume(persistentCollider.collider, this);
			}
			if (this.HasOwnershipOfCollider(persistentCollider.collider))
			{
				float num = Vector3.Dot(this.GetColliderVelocity(ref persistentCollider), persistentCollider.lastSurfaceQuery.surfaceNormal);
				bool flag = num > this.waterParams.splashSpeedRequirement * persistentCollider.scaleMultiplier;
				bool flag2 = num > this.waterParams.bigSplashSpeedRequirement * persistentCollider.scaleMultiplier;
				persistentCollider.PlayRippleEffect(this.waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, this.waterParams.rippleEffectScale, Time.time, this);
				if (this.waterParams.playSplashEffect && flag && (flag2 || !persistentCollider.playBigSplash))
				{
					persistentCollider.PlaySplashEffect(this.waterParams.splashEffect, persistentCollider.lastRipplePosition, this.waterParams.splashEffectScale, persistentCollider.playBigSplash && flag2, false, this);
				}
			}
		}

		// Token: 0x0600517E RID: 20862 RVA: 0x0018B694 File Offset: 0x00189894
		private void ColliderOutOfWaterUpdate(ref WaterOverlappingCollider persistentCollider, float currentTime)
		{
			if (currentTime < persistentCollider.lastInWaterTime + this.waterParams.postExitDripDuration && currentTime > persistentCollider.nextDripTime && persistentCollider.playDripEffect)
			{
				persistentCollider.nextDripTime = currentTime + this.waterParams.perDripTimeDelay + Random.Range(-this.waterParams.perDripTimeRandRange * 0.5f, this.waterParams.perDripTimeRandRange * 0.5f);
				float num = this.waterParams.rippleEffectScale * 2f * (this.waterParams.perDripDefaultRadius + Random.Range(-this.waterParams.perDripRadiusRandRange * 0.5f, this.waterParams.perDripRadiusRandRange * 0.5f));
				persistentCollider.PlayDripEffect(this.waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, num);
			}
		}

		// Token: 0x0600517F RID: 20863 RVA: 0x0018B77C File Offset: 0x0018997C
		private void ColliderInWaterUpdate(ref WaterOverlappingCollider persistentCollider, float currentTime)
		{
			Vector3 vector = Vector3.ProjectOnPlane(persistentCollider.collider.transform.position - persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal) + persistentCollider.lastSurfaceQuery.surfacePoint;
			bool flag;
			if (persistentCollider.overrideBoundingRadius)
			{
				flag = (persistentCollider.collider.transform.position - vector).sqrMagnitude < persistentCollider.boundingRadiusOverride * persistentCollider.boundingRadiusOverride;
			}
			else
			{
				flag = (persistentCollider.collider.ClosestPointOnBounds(vector) - vector).sqrMagnitude < 0.001f;
			}
			if (flag)
			{
				float num = Mathf.Max(this.waterParams.minDistanceBetweenRipples, this.waterParams.defaultDistanceBetweenRipples * (persistentCollider.lastRippleScale / this.waterParams.rippleEffectScale));
				bool flag2 = (persistentCollider.lastRipplePosition - vector).sqrMagnitude > num * num;
				bool flag3 = currentTime - persistentCollider.lastRippleTime > this.waterParams.minTimeBetweenRipples;
				if (flag2 || flag3)
				{
					persistentCollider.PlayRippleEffect(this.waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, this.waterParams.rippleEffectScale, currentTime, this);
					return;
				}
			}
			else
			{
				persistentCollider.lastRippleTime = currentTime;
			}
		}

		// Token: 0x06005180 RID: 20864 RVA: 0x0018B8CC File Offset: 0x00189ACC
		private void TryRegisterOwnershipOfCollider(Collider collider, bool isInWater, bool isSurfaceDetected)
		{
			WaterVolume waterVolume;
			if (WaterVolume.sharedColliderRegistry.TryGetValue(collider, out waterVolume))
			{
				if (waterVolume != this)
				{
					bool flag;
					bool flag2;
					waterVolume.CheckColliderInVolume(collider, out flag, out flag2);
					if ((isSurfaceDetected && !flag2) || (isInWater && !flag))
					{
						WaterVolume.sharedColliderRegistry.Remove(collider);
						WaterVolume.sharedColliderRegistry.Add(collider, this);
						return;
					}
				}
			}
			else
			{
				WaterVolume.sharedColliderRegistry.Add(collider, this);
			}
		}

		// Token: 0x06005181 RID: 20865 RVA: 0x0018B92E File Offset: 0x00189B2E
		private void UnregisterOwnershipOfCollider(Collider collider)
		{
			if (WaterVolume.sharedColliderRegistry.ContainsKey(collider))
			{
				WaterVolume.sharedColliderRegistry.Remove(collider);
			}
		}

		// Token: 0x06005182 RID: 20866 RVA: 0x0018B94C File Offset: 0x00189B4C
		private bool HasOwnershipOfCollider(Collider collider)
		{
			WaterVolume waterVolume;
			return WaterVolume.sharedColliderRegistry.TryGetValue(collider, out waterVolume) && waterVolume == this;
		}

		// Token: 0x06005183 RID: 20867 RVA: 0x0018B974 File Offset: 0x00189B74
		public void OnTriggerEnter(Collider other)
		{
			GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (other.isTrigger && component == null)
			{
				return;
			}
			WaterVolume.WaterVolumeEvent colliderEnteredVolume = this.ColliderEnteredVolume;
			if (colliderEnteredVolume != null)
			{
				colliderEnteredVolume(this, other);
			}
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				if (this.persistentColliders[i].collider == other)
				{
					WaterOverlappingCollider waterOverlappingCollider = this.persistentColliders[i];
					waterOverlappingCollider.inVolume = true;
					this.persistentColliders[i] = waterOverlappingCollider;
					return;
				}
			}
			WaterOverlappingCollider waterOverlappingCollider2 = new WaterOverlappingCollider
			{
				collider = other
			};
			waterOverlappingCollider2.inVolume = true;
			waterOverlappingCollider2.lastInWaterTime = Time.time - this.waterParams.postExitDripDuration - 10f;
			WaterSplashOverride component2 = other.GetComponent<WaterSplashOverride>();
			if (component2 != null)
			{
				if (component2.suppressWaterEffects)
				{
					return;
				}
				waterOverlappingCollider2.playBigSplash = component2.playBigSplash;
				waterOverlappingCollider2.playDripEffect = component2.playDrippingEffect;
				waterOverlappingCollider2.overrideBoundingRadius = component2.overrideBoundingRadius;
				waterOverlappingCollider2.boundingRadiusOverride = component2.boundingRadiusOverride;
				waterOverlappingCollider2.scaleMultiplier = (component2.scaleByPlayersScale ? GTPlayer.Instance.scale : 1f);
			}
			else
			{
				waterOverlappingCollider2.playDripEffect = true;
				waterOverlappingCollider2.overrideBoundingRadius = false;
				waterOverlappingCollider2.scaleMultiplier = 1f;
				waterOverlappingCollider2.playBigSplash = false;
			}
			GTPlayer instance = GTPlayer.Instance;
			if (component != null)
			{
				waterOverlappingCollider2.velocityTracker = (component.isLeftHand ? instance.leftHandCenterVelocityTracker : instance.rightHandCenterVelocityTracker);
				waterOverlappingCollider2.scaleMultiplier = instance.scale;
			}
			else
			{
				waterOverlappingCollider2.velocityTracker = other.GetComponent<GorillaVelocityTracker>();
			}
			if (this.PlayerVRRig != null && this.waterParams.sendSplashEffectRPCs && (component != null || waterOverlappingCollider2.collider == instance.headCollider || waterOverlappingCollider2.collider == instance.bodyCollider))
			{
				waterOverlappingCollider2.photonViewForRPC = this.PlayerVRRig.netView;
			}
			this.persistentColliders.Add(waterOverlappingCollider2);
		}

		// Token: 0x06005184 RID: 20868 RVA: 0x0018BB84 File Offset: 0x00189D84
		private void OnTriggerExit(Collider other)
		{
			GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (other.isTrigger && component == null)
			{
				return;
			}
			WaterVolume.WaterVolumeEvent colliderExitedVolume = this.ColliderExitedVolume;
			if (colliderExitedVolume != null)
			{
				colliderExitedVolume(this, other);
			}
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				if (this.persistentColliders[i].collider == other)
				{
					WaterOverlappingCollider waterOverlappingCollider = this.persistentColliders[i];
					waterOverlappingCollider.inVolume = false;
					this.persistentColliders[i] = waterOverlappingCollider;
				}
			}
		}

		// Token: 0x06005185 RID: 20869 RVA: 0x0018BC0E File Offset: 0x00189E0E
		public void SetPropertiesFromPlaceholder(WaterVolumeProperties properties, List<Collider> waterVolumeColliders, WaterParameters parameters)
		{
			this.surfacePlane = properties.surfacePlane;
			this.surfaceColliders = properties.surfaceColliders;
			this.volumeColliders = waterVolumeColliders;
			this.liquidType = (GTPlayer.LiquidType)Math.Clamp(properties.liquidType - CMSZoneShaderSettings.EZoneLiquidType.Water, 0, 1);
			this.waterParams = parameters;
		}

		// Token: 0x04005590 RID: 21904
		[SerializeField]
		public Transform surfacePlane;

		// Token: 0x04005591 RID: 21905
		[SerializeField]
		private List<MeshCollider> surfaceColliders = new List<MeshCollider>();

		// Token: 0x04005592 RID: 21906
		[SerializeField]
		public List<Collider> volumeColliders = new List<Collider>();

		// Token: 0x04005593 RID: 21907
		[SerializeField]
		private GTPlayer.LiquidType liquidType;

		// Token: 0x04005594 RID: 21908
		[SerializeField]
		private WaterCurrent waterCurrent;

		// Token: 0x04005595 RID: 21909
		[SerializeField]
		private WaterParameters waterParams;

		// Token: 0x04005596 RID: 21910
		[SerializeField]
		[Tooltip("The water volume be placed in the scene (not spawned) and not moved for this to be true")]
		public bool isStationary = true;

		// Token: 0x04005597 RID: 21911
		public const string WaterSplashRPC = "RPC_PlaySplashEffect";

		// Token: 0x04005598 RID: 21912
		public static float[] splashRPCSendTimes = new float[4];

		// Token: 0x04005599 RID: 21913
		private static Dictionary<Collider, WaterVolume> sharedColliderRegistry = new Dictionary<Collider, WaterVolume>(16);

		// Token: 0x0400559A RID: 21914
		private static Dictionary<Mesh, int[]> meshTrianglesDict = new Dictionary<Mesh, int[]>(16);

		// Token: 0x0400559B RID: 21915
		private static Dictionary<Mesh, Vector3[]> meshVertsDict = new Dictionary<Mesh, Vector3[]>(16);

		// Token: 0x0400559C RID: 21916
		private int[] sharedMeshTris;

		// Token: 0x0400559D RID: 21917
		private Vector3[] sharedMeshVerts;

		// Token: 0x040055A2 RID: 21922
		private VRRig playerVRRig;

		// Token: 0x040055A3 RID: 21923
		private float volumeMaxHeight;

		// Token: 0x040055A4 RID: 21924
		private float volumeMinHeight;

		// Token: 0x040055A5 RID: 21925
		private bool debugDrawSurfaceCast;

		// Token: 0x040055A6 RID: 21926
		private Collider triggerCollider;

		// Token: 0x040055A7 RID: 21927
		private List<WaterOverlappingCollider> persistentColliders = new List<WaterOverlappingCollider>(16);

		// Token: 0x040055A8 RID: 21928
		private GuidedRefTargetIdSO _guidedRefTargetId;

		// Token: 0x040055A9 RID: 21929
		private Object _guidedRefTargetObject;

		// Token: 0x02000CD5 RID: 3285
		public struct SurfaceQuery
		{
			// Token: 0x17000841 RID: 2113
			// (get) Token: 0x06005188 RID: 20872 RVA: 0x0018BCAE File Offset: 0x00189EAE
			public Plane surfacePlane
			{
				get
				{
					return new Plane(this.surfaceNormal, this.surfacePoint);
				}
			}

			// Token: 0x040055AA RID: 21930
			public Vector3 surfacePoint;

			// Token: 0x040055AB RID: 21931
			public Vector3 surfaceNormal;

			// Token: 0x040055AC RID: 21932
			public float maxDepth;
		}

		// Token: 0x02000CD6 RID: 3286
		// (Invoke) Token: 0x0600518A RID: 20874
		public delegate void WaterVolumeEvent(WaterVolume volume, Collider collider);
	}
}
