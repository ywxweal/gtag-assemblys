using System;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000CD1 RID: 3281
	public struct WaterOverlappingCollider
	{
		// Token: 0x0600515C RID: 20828 RVA: 0x00189FB4 File Offset: 0x001881B4
		public void PlayRippleEffect(GameObject rippleEffectPrefab, Vector3 surfacePoint, Vector3 surfaceNormal, float defaultRippleScale, float currentTime, WaterVolume volume)
		{
			this.lastRipplePosition = this.GetClosestPositionOnSurface(surfacePoint, surfaceNormal);
			this.lastBoundingRadius = this.GetBoundingRadiusOnSurface(surfaceNormal);
			this.lastRippleScale = defaultRippleScale * this.lastBoundingRadius * 2f * this.scaleMultiplier;
			this.lastRippleTime = currentTime;
			ObjectPools.instance.Instantiate(rippleEffectPrefab, this.lastRipplePosition, Quaternion.FromToRotation(Vector3.up, this.lastSurfaceQuery.surfaceNormal) * Quaternion.AngleAxis(-90f, Vector3.right), this.lastRippleScale, true).GetComponent<WaterRippleEffect>().PlayEffect(volume);
		}

		// Token: 0x0600515D RID: 20829 RVA: 0x0018A050 File Offset: 0x00188250
		public void PlaySplashEffect(GameObject splashEffectPrefab, Vector3 splashPosition, float splashScale, bool bigSplash, bool enteringWater, WaterVolume volume)
		{
			Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, this.lastSurfaceQuery.surfaceNormal) * Quaternion.AngleAxis(-90f, Vector3.right);
			ObjectPools.instance.Instantiate(splashEffectPrefab, splashPosition, quaternion, splashScale * this.scaleMultiplier, true).GetComponent<WaterSplashEffect>().PlayEffect(bigSplash, enteringWater, this.scaleMultiplier, volume);
			if (this.photonViewForRPC != null)
			{
				float time = Time.time;
				int num = -1;
				float num2 = time + 10f;
				for (int i = 0; i < WaterVolume.splashRPCSendTimes.Length; i++)
				{
					if (WaterVolume.splashRPCSendTimes[i] < num2)
					{
						num2 = WaterVolume.splashRPCSendTimes[i];
						num = i;
					}
				}
				if (time - 0.5f > num2)
				{
					WaterVolume.splashRPCSendTimes[num] = time;
					this.photonViewForRPC.SendRPC("RPC_PlaySplashEffect", RpcTarget.Others, new object[]
					{
						splashPosition,
						quaternion,
						splashScale * this.scaleMultiplier,
						this.lastBoundingRadius,
						bigSplash,
						enteringWater
					});
				}
			}
		}

		// Token: 0x0600515E RID: 20830 RVA: 0x0018A170 File Offset: 0x00188370
		public void PlayDripEffect(GameObject rippleEffectPrefab, Vector3 surfacePoint, Vector3 surfaceNormal, float dripScale)
		{
			Vector3 closestPositionOnSurface = this.GetClosestPositionOnSurface(surfacePoint, surfaceNormal);
			float num = (this.overrideBoundingRadius ? this.boundingRadiusOverride : this.lastBoundingRadius);
			Vector3 vector = Vector3.ProjectOnPlane(Random.onUnitSphere * num * 0.5f, surfaceNormal);
			ObjectPools.instance.Instantiate(rippleEffectPrefab, closestPositionOnSurface + vector, Quaternion.FromToRotation(Vector3.up, this.lastSurfaceQuery.surfaceNormal) * Quaternion.AngleAxis(-90f, Vector3.right), dripScale * this.scaleMultiplier, true);
		}

		// Token: 0x0600515F RID: 20831 RVA: 0x0018A1FF File Offset: 0x001883FF
		public Vector3 GetClosestPositionOnSurface(Vector3 surfacePoint, Vector3 surfaceNormal)
		{
			return Vector3.ProjectOnPlane(this.collider.transform.position - surfacePoint, surfaceNormal) + surfacePoint;
		}

		// Token: 0x06005160 RID: 20832 RVA: 0x0018A224 File Offset: 0x00188424
		private float GetBoundingRadiusOnSurface(Vector3 surfaceNormal)
		{
			if (this.overrideBoundingRadius)
			{
				this.lastBoundingRadius = this.boundingRadiusOverride;
				return this.boundingRadiusOverride;
			}
			Vector3 extents = this.collider.bounds.extents;
			Vector3 vector = Vector3.ProjectOnPlane(this.collider.transform.right * extents.x, surfaceNormal);
			Vector3 vector2 = Vector3.ProjectOnPlane(this.collider.transform.up * extents.y, surfaceNormal);
			Vector3 vector3 = Vector3.ProjectOnPlane(this.collider.transform.forward * extents.z, surfaceNormal);
			float sqrMagnitude = vector.sqrMagnitude;
			float sqrMagnitude2 = vector2.sqrMagnitude;
			float sqrMagnitude3 = vector3.sqrMagnitude;
			if (sqrMagnitude >= sqrMagnitude2 && sqrMagnitude >= sqrMagnitude3)
			{
				return vector.magnitude;
			}
			if (sqrMagnitude2 >= sqrMagnitude && sqrMagnitude2 >= sqrMagnitude3)
			{
				return vector2.magnitude;
			}
			return vector3.magnitude;
		}

		// Token: 0x04005561 RID: 21857
		public bool playBigSplash;

		// Token: 0x04005562 RID: 21858
		public bool playDripEffect;

		// Token: 0x04005563 RID: 21859
		public bool overrideBoundingRadius;

		// Token: 0x04005564 RID: 21860
		public float boundingRadiusOverride;

		// Token: 0x04005565 RID: 21861
		public float scaleMultiplier;

		// Token: 0x04005566 RID: 21862
		public Collider collider;

		// Token: 0x04005567 RID: 21863
		public GorillaVelocityTracker velocityTracker;

		// Token: 0x04005568 RID: 21864
		public WaterVolume.SurfaceQuery lastSurfaceQuery;

		// Token: 0x04005569 RID: 21865
		public NetworkView photonViewForRPC;

		// Token: 0x0400556A RID: 21866
		public bool surfaceDetected;

		// Token: 0x0400556B RID: 21867
		public bool inWater;

		// Token: 0x0400556C RID: 21868
		public bool inVolume;

		// Token: 0x0400556D RID: 21869
		public float lastBoundingRadius;

		// Token: 0x0400556E RID: 21870
		public Vector3 lastRipplePosition;

		// Token: 0x0400556F RID: 21871
		public float lastRippleScale;

		// Token: 0x04005570 RID: 21872
		public float lastRippleTime;

		// Token: 0x04005571 RID: 21873
		public float lastInWaterTime;

		// Token: 0x04005572 RID: 21874
		public float nextDripTime;
	}
}
