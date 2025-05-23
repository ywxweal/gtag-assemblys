using System;
using CjLib;
using GorillaTag;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000CCD RID: 3277
	[ExecuteAlways]
	public class UnderwaterCameraEffect : MonoBehaviour
	{
		// Token: 0x06005145 RID: 20805 RVA: 0x00188BD8 File Offset: 0x00186DD8
		private void SetOffScreenPosition()
		{
			base.transform.localScale = new Vector3(2f * (this.frustumPlaneExtents.x + 0.04f), 0f, 1f);
			base.transform.localPosition = new Vector3(0f, -(this.frustumPlaneExtents.y + 0.04f), this.distanceFromCamera);
		}

		// Token: 0x06005146 RID: 20806 RVA: 0x00188C44 File Offset: 0x00186E44
		private void SetFullScreenPosition()
		{
			base.transform.localScale = new Vector3(2f * (this.frustumPlaneExtents.x + 0.04f), 2f * (this.frustumPlaneExtents.y + 0.04f), 1f);
			base.transform.localPosition = new Vector3(0f, 0f, this.distanceFromCamera);
		}

		// Token: 0x06005147 RID: 20807 RVA: 0x00188CB4 File Offset: 0x00186EB4
		private void OnEnable()
		{
			if (this.targetCamera == null)
			{
				this.targetCamera = Camera.main;
			}
			this.hasTargetCamera = this.targetCamera != null;
			this.InitializeShaderProperties();
		}

		// Token: 0x06005148 RID: 20808 RVA: 0x00188CE8 File Offset: 0x00186EE8
		private void Start()
		{
			this.player = GTPlayer.Instance;
			this.cachedAspectRatio = this.targetCamera.aspect;
			this.cachedFov = this.targetCamera.fieldOfView;
			this.CalculateFrustumPlaneBounds(this.cachedFov, this.cachedAspectRatio);
			this.SetOffScreenPosition();
		}

		// Token: 0x06005149 RID: 20809 RVA: 0x00188D3C File Offset: 0x00186F3C
		private void LateUpdate()
		{
			if (!this.hasTargetCamera)
			{
				return;
			}
			if (!this.player)
			{
				return;
			}
			if (this.player.HeadOverlappingWaterVolumes.Count < 1)
			{
				this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater);
				if (this.planeRenderer.enabled)
				{
					this.planeRenderer.enabled = false;
					this.SetOffScreenPosition();
				}
				if (this.underwaterParticleEffect != null && this.underwaterParticleEffect.gameObject.activeInHierarchy)
				{
					this.underwaterParticleEffect.UpdateParticleEffect(false, ref this.waterSurface);
				}
				return;
			}
			if (this.targetCamera.aspect != this.cachedAspectRatio || this.targetCamera.fieldOfView != this.cachedFov)
			{
				this.cachedAspectRatio = this.targetCamera.aspect;
				this.cachedFov = this.targetCamera.fieldOfView;
				this.CalculateFrustumPlaneBounds(this.cachedFov, this.cachedAspectRatio);
			}
			bool flag = false;
			float num = float.MinValue;
			Vector3 position = this.targetCamera.transform.position;
			for (int i = 0; i < this.player.HeadOverlappingWaterVolumes.Count; i++)
			{
				WaterVolume.SurfaceQuery surfaceQuery;
				if (this.player.HeadOverlappingWaterVolumes[i].GetSurfaceQueryForPoint(position, out surfaceQuery, false))
				{
					float num2 = Vector3.Dot(surfaceQuery.surfacePoint - position, surfaceQuery.surfaceNormal);
					if (num2 > num)
					{
						flag = true;
						num = num2;
						this.waterSurface = surfaceQuery;
					}
				}
			}
			if (flag)
			{
				Vector3 vector = this.targetCamera.transform.InverseTransformPoint(this.waterSurface.surfacePoint);
				Vector3 vector2 = this.targetCamera.transform.InverseTransformDirection(this.waterSurface.surfaceNormal);
				Plane plane = new Plane(vector2, vector);
				Plane plane2 = new Plane(Vector3.forward, -this.distanceFromCamera);
				Vector3 vector3;
				Vector3 vector4;
				if (this.IntersectPlanes(plane2, plane, out vector3, out vector4))
				{
					Vector3 normalized = Vector3.Cross(vector4, Vector3.forward).normalized;
					float num3 = Vector3.Dot(new Vector3(vector3.x, vector3.y, 0f), normalized);
					if (num3 > this.frustumPlaneExtents.y + 0.04f)
					{
						this.SetFullScreenPosition();
						this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.FullySubmerged);
					}
					else if (num3 < -(this.frustumPlaneExtents.y + 0.04f))
					{
						this.SetOffScreenPosition();
						this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater);
					}
					else
					{
						float num4 = num3;
						num4 += this.GetFrustumCoverageDistance(-normalized) + 0.04f;
						float num5 = this.GetFrustumCoverageDistance(vector4) + 0.04f;
						num5 += this.GetFrustumCoverageDistance(-vector4) + 0.04f;
						base.transform.localScale = new Vector3(num5, num4, 1f);
						base.transform.localPosition = normalized * (num3 - num4 * 0.5f) + new Vector3(0f, 0f, this.distanceFromCamera);
						float num6 = Vector3.SignedAngle(Vector3.up, normalized, Vector3.forward);
						base.transform.localRotation = Quaternion.AngleAxis(num6, Vector3.forward);
						this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.PartiallySubmerged);
					}
					if (this.debugDraw)
					{
						Vector3 vector5 = this.targetCamera.transform.TransformPoint(vector3);
						Vector3 vector6 = this.targetCamera.transform.TransformDirection(vector4);
						DebugUtil.DrawLine(vector5 - 2f * this.frustumPlaneExtents.x * vector6, vector5 + 2f * this.frustumPlaneExtents.x * vector6, Color.white, false);
					}
				}
				else if (new Plane(this.waterSurface.surfaceNormal, this.waterSurface.surfacePoint).GetSide(this.targetCamera.transform.position))
				{
					this.SetFullScreenPosition();
					this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.FullySubmerged);
				}
				else
				{
					this.SetOffScreenPosition();
					this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater);
				}
			}
			else
			{
				this.SetOffScreenPosition();
				this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater);
			}
			if (this.underwaterParticleEffect != null && this.underwaterParticleEffect.gameObject.activeInHierarchy)
			{
				this.underwaterParticleEffect.UpdateParticleEffect(flag, ref this.waterSurface);
			}
		}

		// Token: 0x0600514A RID: 20810 RVA: 0x00189174 File Offset: 0x00187374
		[DebugOption]
		private void InitializeShaderProperties()
		{
			Shader.DisableKeyword("_GLOBAL_CAMERA_TOUCHING_WATER");
			Shader.DisableKeyword("_GLOBAL_CAMERA_FULLY_UNDERWATER");
			float num = -Vector3.Dot(this.waterSurface.surfaceNormal, this.waterSurface.surfacePoint);
			Shader.SetGlobalVector(this.shaderParam_GlobalCameraOverlapWaterSurfacePlane, new Vector4(this.waterSurface.surfaceNormal.x, this.waterSurface.surfaceNormal.y, this.waterSurface.surfaceNormal.z, num));
		}

		// Token: 0x0600514B RID: 20811 RVA: 0x001891F4 File Offset: 0x001873F4
		private void SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState state)
		{
			if (state != this.cameraOverlapWaterState || state == UnderwaterCameraEffect.CameraOverlapWaterState.Uninitialized)
			{
				this.cameraOverlapWaterState = state;
				switch (this.cameraOverlapWaterState)
				{
				case UnderwaterCameraEffect.CameraOverlapWaterState.Uninitialized:
				case UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater:
					Shader.DisableKeyword("_GLOBAL_CAMERA_TOUCHING_WATER");
					Shader.DisableKeyword("_GLOBAL_CAMERA_FULLY_UNDERWATER");
					break;
				case UnderwaterCameraEffect.CameraOverlapWaterState.PartiallySubmerged:
					Shader.EnableKeyword("_GLOBAL_CAMERA_TOUCHING_WATER");
					Shader.DisableKeyword("_GLOBAL_CAMERA_FULLY_UNDERWATER");
					break;
				case UnderwaterCameraEffect.CameraOverlapWaterState.FullySubmerged:
					Shader.EnableKeyword("_GLOBAL_CAMERA_TOUCHING_WATER");
					Shader.EnableKeyword("_GLOBAL_CAMERA_FULLY_UNDERWATER");
					break;
				}
			}
			if (this.cameraOverlapWaterState == UnderwaterCameraEffect.CameraOverlapWaterState.PartiallySubmerged)
			{
				Plane plane = new Plane(this.waterSurface.surfaceNormal, this.waterSurface.surfacePoint);
				Shader.SetGlobalVector(this.shaderParam_GlobalCameraOverlapWaterSurfacePlane, new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance));
			}
		}

		// Token: 0x0600514C RID: 20812 RVA: 0x001892D4 File Offset: 0x001874D4
		private void CalculateFrustumPlaneBounds(float fieldOfView, float aspectRatio)
		{
			float num = Mathf.Tan(0.017453292f * fieldOfView * 0.5f) * this.distanceFromCamera;
			float num2 = aspectRatio * num + 0.04f;
			float num3 = 1f / aspectRatio * num + 0.04f;
			this.frustumPlaneExtents = new Vector2(num2, num3);
			this.frustumPlaneCornersLocal[0] = new Vector3(-num2, -num3, this.distanceFromCamera);
			this.frustumPlaneCornersLocal[1] = new Vector3(-num2, num3, this.distanceFromCamera);
			this.frustumPlaneCornersLocal[2] = new Vector3(num2, num3, this.distanceFromCamera);
			this.frustumPlaneCornersLocal[3] = new Vector3(num2, -num3, this.distanceFromCamera);
		}

		// Token: 0x0600514D RID: 20813 RVA: 0x0018938C File Offset: 0x0018758C
		private bool IntersectPlanes(Plane p1, Plane p2, out Vector3 point, out Vector3 direction)
		{
			direction = Vector3.Cross(p1.normal, p2.normal);
			float num = Vector3.Dot(direction, direction);
			if (num < Mathf.Epsilon)
			{
				point = Vector3.zero;
				return false;
			}
			point = Vector3.Cross(direction, p1.distance * p2.normal - p2.distance * p1.normal) / num;
			return true;
		}

		// Token: 0x0600514E RID: 20814 RVA: 0x00189420 File Offset: 0x00187620
		private float GetFrustumCoverageDistance(Vector3 localDirection)
		{
			float num = float.MinValue;
			for (int i = 0; i < this.frustumPlaneCornersLocal.Length; i++)
			{
				float num2 = Vector3.Dot(this.frustumPlaneCornersLocal[i], localDirection);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x04005535 RID: 21813
		private const float edgeBuffer = 0.04f;

		// Token: 0x04005536 RID: 21814
		[SerializeField]
		private Camera targetCamera;

		// Token: 0x04005537 RID: 21815
		[SerializeField]
		private MeshRenderer planeRenderer;

		// Token: 0x04005538 RID: 21816
		[SerializeField]
		private UnderwaterParticleEffects underwaterParticleEffect;

		// Token: 0x04005539 RID: 21817
		[SerializeField]
		private float distanceFromCamera = 0.02f;

		// Token: 0x0400553A RID: 21818
		[SerializeField]
		[DebugOption]
		private bool debugDraw;

		// Token: 0x0400553B RID: 21819
		private float cachedAspectRatio = 1f;

		// Token: 0x0400553C RID: 21820
		private float cachedFov = 90f;

		// Token: 0x0400553D RID: 21821
		private readonly Vector3[] frustumPlaneCornersLocal = new Vector3[4];

		// Token: 0x0400553E RID: 21822
		private Vector2 frustumPlaneExtents;

		// Token: 0x0400553F RID: 21823
		private GTPlayer player;

		// Token: 0x04005540 RID: 21824
		private WaterVolume.SurfaceQuery waterSurface;

		// Token: 0x04005541 RID: 21825
		private const string kShaderKeyword_GlobalCameraTouchingWater = "_GLOBAL_CAMERA_TOUCHING_WATER";

		// Token: 0x04005542 RID: 21826
		private const string kShaderKeyword_GlobalCameraFullyUnderwater = "_GLOBAL_CAMERA_FULLY_UNDERWATER";

		// Token: 0x04005543 RID: 21827
		private int shaderParam_GlobalCameraOverlapWaterSurfacePlane = Shader.PropertyToID("_GlobalCameraOverlapWaterSurfacePlane");

		// Token: 0x04005544 RID: 21828
		private bool hasTargetCamera;

		// Token: 0x04005545 RID: 21829
		[DebugReadout]
		private UnderwaterCameraEffect.CameraOverlapWaterState cameraOverlapWaterState;

		// Token: 0x02000CCE RID: 3278
		private enum CameraOverlapWaterState
		{
			// Token: 0x04005547 RID: 21831
			Uninitialized,
			// Token: 0x04005548 RID: 21832
			OutOfWater,
			// Token: 0x04005549 RID: 21833
			PartiallySubmerged,
			// Token: 0x0400554A RID: 21834
			FullySubmerged
		}
	}
}
