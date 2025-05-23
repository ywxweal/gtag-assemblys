using System;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B6E RID: 2926
	public class BuilderSpeedBooster : MonoBehaviour
	{
		// Token: 0x06004879 RID: 18553 RVA: 0x00159EDB File Offset: 0x001580DB
		private void Awake()
		{
			this.volume = base.GetComponent<Collider>();
			this.windRenderer.enabled = false;
			this.boosting = false;
		}

		// Token: 0x0600487A RID: 18554 RVA: 0x00159EFC File Offset: 0x001580FC
		private void LateUpdate()
		{
			if (this.audioSource && this.audioSource != null && !this.audioSource.isPlaying && this.audioSource.enabled)
			{
				this.audioSource.enabled = false;
			}
		}

		// Token: 0x0600487B RID: 18555 RVA: 0x00159F4C File Offset: 0x0015814C
		private bool TriggerFilter(Collider other, out Rigidbody rb, out Transform xf)
		{
			rb = null;
			xf = null;
			if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
			{
				rb = GorillaTagger.Instance.GetComponent<Rigidbody>();
				xf = GorillaTagger.Instance.headCollider.GetComponent<Transform>();
			}
			return rb != null && xf != null;
		}

		// Token: 0x0600487C RID: 18556 RVA: 0x00159FAC File Offset: 0x001581AC
		private void CheckTableZone()
		{
			if (this.hasCheckedZone)
			{
				return;
			}
			BuilderTable builderTable;
			if (BuilderTable.TryGetBuilderTableForZone(GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone, out builderTable))
			{
				this.ignoreMonkeScale = !builderTable.isTableMutable;
			}
			this.hasCheckedZone = true;
		}

		// Token: 0x0600487D RID: 18557 RVA: 0x00159FF8 File Offset: 0x001581F8
		public void OnTriggerEnter(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			this.CheckTableZone();
			if (!this.ignoreMonkeScale && (double)GorillaTagger.Instance.offlineVRRig.scaleFactor > 0.99)
			{
				return;
			}
			this.positiveForce = Vector3.Dot(base.transform.up, rigidbody.velocity) > 0f;
			if (this.positiveForce)
			{
				this.windRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
			}
			else
			{
				this.windRenderer.transform.localRotation = Quaternion.Euler(0f, 180f, -90f);
			}
			this.windRenderer.enabled = true;
			this.enterPos = transform.position;
			if (!this.boosting)
			{
				this.boosting = true;
				this.enterTime = Time.timeAsDouble;
			}
		}

		// Token: 0x0600487E RID: 18558 RVA: 0x0015A0E8 File Offset: 0x001582E8
		public void OnTriggerExit(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			this.windRenderer.enabled = false;
			this.CheckTableZone();
			if (!this.ignoreMonkeScale && (double)GorillaTagger.Instance.offlineVRRig.scaleFactor > 0.99)
			{
				return;
			}
			if (this.boosting && this.audioSource)
			{
				this.audioSource.enabled = true;
				this.audioSource.Stop();
				this.audioSource.GTPlayOneShot(this.exitClip, 1f);
			}
			this.boosting = false;
		}

		// Token: 0x0600487F RID: 18559 RVA: 0x0015A188 File Offset: 0x00158388
		public void OnTriggerStay(Collider other)
		{
			if (!this.boosting)
			{
				return;
			}
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			if (!this.ignoreMonkeScale && (double)GorillaTagger.Instance.offlineVRRig.scaleFactor > 0.99)
			{
				this.OnTriggerExit(other);
				return;
			}
			if (Time.timeAsDouble > this.enterTime + (double)this.maxBoostDuration)
			{
				this.OnTriggerExit(other);
				return;
			}
			if (this.disableGrip)
			{
				GTPlayer.Instance.SetMaximumSlipThisFrame();
			}
			SizeManager sizeManager = null;
			if (this.scaleWithSize)
			{
				sizeManager = rigidbody.GetComponent<SizeManager>();
			}
			Vector3 vector = rigidbody.velocity;
			if (this.scaleWithSize && sizeManager)
			{
				vector /= sizeManager.currentScale;
			}
			Vector3 vector2 = Vector3.Dot(transform.position - base.transform.position, base.transform.up) * base.transform.up;
			Vector3 vector3 = base.transform.position + vector2 - transform.position;
			float num = vector3.magnitude + 0.0001f;
			Vector3 vector4 = vector3 / num;
			float num2 = Vector3.Dot(vector, vector4);
			float num3 = this.accel;
			if (this.maxDepth > -1f)
			{
				float num4 = Vector3.Dot(transform.position - this.enterPos, vector4);
				float num5 = this.maxDepth - num4;
				float num6 = 0f;
				if (num5 > 0.0001f)
				{
					num6 = num2 * num2 / num5;
				}
				num3 = Mathf.Max(this.accel, num6);
			}
			float deltaTime = Time.deltaTime;
			Vector3 vector5 = base.transform.up * num3 * deltaTime;
			if (!this.positiveForce)
			{
				vector5 *= -1f;
			}
			vector += vector5;
			if ((double)Vector3.Dot(vector5, Vector3.down) <= 0.1)
			{
				vector += Vector3.up * this.addedWorldUpVelocity * deltaTime;
			}
			Vector3 vector6 = Mathf.Min(Vector3.Dot(vector, base.transform.up), this.maxSpeed) * base.transform.up;
			Vector3 vector7 = Vector3.Dot(vector, base.transform.right) * base.transform.right;
			Vector3 vector8 = Vector3.Dot(vector, base.transform.forward) * base.transform.forward;
			float num7 = 1f;
			float num8 = 1f;
			if (this.dampenLateralVelocity)
			{
				num7 = 1f - this.dampenXVelPerc * 0.01f * deltaTime;
				num8 = 1f - this.dampenZVelPerc * 0.01f * deltaTime;
			}
			vector = vector6 + num7 * vector7 + num8 * vector8;
			if (this.applyPullToCenterAcceleration && this.pullToCenterAccel > 0f && this.pullToCenterMaxSpeed > 0f)
			{
				vector -= num2 * vector4;
				if (num > this.pullTOCenterMinDistance)
				{
					num2 += this.pullToCenterAccel * deltaTime;
					float num9 = Mathf.Min(this.pullToCenterMaxSpeed, num / deltaTime);
					num2 = Mathf.Min(num2, num9);
				}
				else
				{
					num2 = 0f;
				}
				vector += num2 * vector4;
				if (vector.magnitude > 0.0001f)
				{
					Vector3 vector9 = Vector3.Cross(base.transform.up, vector4);
					float magnitude = vector9.magnitude;
					if (magnitude > 0.0001f)
					{
						vector9 /= magnitude;
						num2 = Vector3.Dot(vector, vector9);
						vector -= num2 * vector9;
						num2 -= this.pullToCenterAccel * deltaTime;
						num2 = Mathf.Max(0f, num2);
						vector += num2 * vector9;
					}
				}
			}
			if (this.scaleWithSize && sizeManager)
			{
				vector *= sizeManager.currentScale;
			}
			rigidbody.velocity = vector;
		}

		// Token: 0x06004880 RID: 18560 RVA: 0x0015A59C File Offset: 0x0015879C
		public void OnDrawGizmosSelected()
		{
			base.GetComponents<Collider>();
			Gizmos.color = Color.magenta;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawWireCube(Vector3.zero, new Vector3(this.pullTOCenterMinDistance / base.transform.lossyScale.x, 1f, this.pullTOCenterMinDistance / base.transform.lossyScale.z));
		}

		// Token: 0x04004B15 RID: 19221
		[SerializeField]
		public bool scaleWithSize = true;

		// Token: 0x04004B16 RID: 19222
		[SerializeField]
		private float accel;

		// Token: 0x04004B17 RID: 19223
		[SerializeField]
		private float maxDepth = -1f;

		// Token: 0x04004B18 RID: 19224
		[SerializeField]
		private float maxSpeed;

		// Token: 0x04004B19 RID: 19225
		[SerializeField]
		private bool disableGrip;

		// Token: 0x04004B1A RID: 19226
		[SerializeField]
		private bool dampenLateralVelocity = true;

		// Token: 0x04004B1B RID: 19227
		[SerializeField]
		private float dampenXVelPerc;

		// Token: 0x04004B1C RID: 19228
		[SerializeField]
		private float dampenZVelPerc;

		// Token: 0x04004B1D RID: 19229
		[SerializeField]
		private bool applyPullToCenterAcceleration = true;

		// Token: 0x04004B1E RID: 19230
		[SerializeField]
		private float pullToCenterAccel;

		// Token: 0x04004B1F RID: 19231
		[SerializeField]
		private float pullToCenterMaxSpeed;

		// Token: 0x04004B20 RID: 19232
		[SerializeField]
		private float pullTOCenterMinDistance = 0.1f;

		// Token: 0x04004B21 RID: 19233
		[SerializeField]
		private float addedWorldUpVelocity = 10f;

		// Token: 0x04004B22 RID: 19234
		[SerializeField]
		private float maxBoostDuration = 2f;

		// Token: 0x04004B23 RID: 19235
		private bool boosting;

		// Token: 0x04004B24 RID: 19236
		private double enterTime;

		// Token: 0x04004B25 RID: 19237
		private Collider volume;

		// Token: 0x04004B26 RID: 19238
		public AudioClip exitClip;

		// Token: 0x04004B27 RID: 19239
		public AudioSource audioSource;

		// Token: 0x04004B28 RID: 19240
		public MeshRenderer windRenderer;

		// Token: 0x04004B29 RID: 19241
		private Vector3 enterPos;

		// Token: 0x04004B2A RID: 19242
		private bool positiveForce = true;

		// Token: 0x04004B2B RID: 19243
		private bool ignoreMonkeScale;

		// Token: 0x04004B2C RID: 19244
		private bool hasCheckedZone;
	}
}
