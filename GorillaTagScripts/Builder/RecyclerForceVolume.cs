using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B70 RID: 2928
	public class RecyclerForceVolume : MonoBehaviour
	{
		// Token: 0x06004887 RID: 18567 RVA: 0x0015A7F6 File Offset: 0x001589F6
		private void Awake()
		{
			this.volume = base.GetComponent<Collider>();
			this.hasWindFX = this.windEffectRenderer != null;
			if (this.hasWindFX)
			{
				this.windEffectRenderer.enabled = false;
			}
		}

		// Token: 0x06004888 RID: 18568 RVA: 0x0015A82C File Offset: 0x00158A2C
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

		// Token: 0x06004889 RID: 18569 RVA: 0x0015A88C File Offset: 0x00158A8C
		public void OnTriggerEnter(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			this.enterPos = transform.position;
			ObjectPools.instance.Instantiate(this.windSFX, this.enterPos, true);
			if (this.hasWindFX)
			{
				this.windEffectRenderer.transform.position = base.transform.position + Vector3.Dot(this.enterPos - base.transform.position, base.transform.right) * base.transform.right;
				this.windEffectRenderer.enabled = true;
			}
		}

		// Token: 0x0600488A RID: 18570 RVA: 0x0015A93C File Offset: 0x00158B3C
		public void OnTriggerExit(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			if (this.hasWindFX)
			{
				this.windEffectRenderer.enabled = false;
			}
		}

		// Token: 0x0600488B RID: 18571 RVA: 0x0015A970 File Offset: 0x00158B70
		public void OnTriggerStay(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
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
			Vector3 vector2 = Vector3.Dot(base.transform.position - transform.position, base.transform.up) * base.transform.up;
			float num = vector2.magnitude + 0.0001f;
			Vector3 vector3 = vector2 / num;
			float num2 = Vector3.Dot(vector, vector3);
			float num3 = this.accel;
			if (this.maxDepth > -1f)
			{
				float num4 = Vector3.Dot(transform.position - this.enterPos, vector3);
				float num5 = this.maxDepth - num4;
				float num6 = 0f;
				if (num5 > 0.0001f)
				{
					num6 = num2 * num2 / num5;
				}
				num3 = Mathf.Max(this.accel, num6);
			}
			float deltaTime = Time.deltaTime;
			Vector3 vector4 = base.transform.forward * num3 * deltaTime;
			vector += vector4;
			Vector3 vector5 = Vector3.Dot(vector, base.transform.up) * base.transform.up;
			Vector3 vector6 = Vector3.Dot(vector, base.transform.right) * base.transform.right;
			Vector3 vector7 = Mathf.Clamp(Vector3.Dot(vector, base.transform.forward), -1f * this.maxSpeed, this.maxSpeed) * base.transform.forward;
			float num7 = 1f;
			float num8 = 1f;
			if (this.dampenLateralVelocity)
			{
				num7 = 1f - this.dampenXVelPerc * 0.01f * deltaTime;
				num8 = 1f - this.dampenYVelPerc * 0.01f * deltaTime;
			}
			vector = num8 * vector5 + num7 * vector6 + vector7;
			if (this.applyPullToCenterAcceleration && this.pullToCenterAccel > 0f && this.pullToCenterMaxSpeed > 0f)
			{
				vector -= num2 * vector3;
				if (num > this.pullTOCenterMinDistance)
				{
					num2 += this.pullToCenterAccel * deltaTime;
					float num9 = Mathf.Min(this.pullToCenterMaxSpeed, num / deltaTime);
					num2 = Mathf.Clamp(num2, -1f * num9, num9);
				}
				else
				{
					num2 = 0f;
				}
				vector += num2 * vector3;
			}
			if (this.scaleWithSize && sizeManager)
			{
				vector *= sizeManager.currentScale;
			}
			rigidbody.velocity = vector;
		}

		// Token: 0x04004B35 RID: 19253
		[SerializeField]
		public bool scaleWithSize = true;

		// Token: 0x04004B36 RID: 19254
		[SerializeField]
		private float accel;

		// Token: 0x04004B37 RID: 19255
		[SerializeField]
		private float maxDepth = -1f;

		// Token: 0x04004B38 RID: 19256
		[SerializeField]
		private float maxSpeed;

		// Token: 0x04004B39 RID: 19257
		[SerializeField]
		private bool disableGrip;

		// Token: 0x04004B3A RID: 19258
		[SerializeField]
		private bool dampenLateralVelocity = true;

		// Token: 0x04004B3B RID: 19259
		[SerializeField]
		private float dampenXVelPerc;

		// Token: 0x04004B3C RID: 19260
		[FormerlySerializedAs("dampenZVelPerc")]
		[SerializeField]
		private float dampenYVelPerc;

		// Token: 0x04004B3D RID: 19261
		[SerializeField]
		private bool applyPullToCenterAcceleration = true;

		// Token: 0x04004B3E RID: 19262
		[SerializeField]
		private float pullToCenterAccel;

		// Token: 0x04004B3F RID: 19263
		[SerializeField]
		private float pullToCenterMaxSpeed;

		// Token: 0x04004B40 RID: 19264
		[SerializeField]
		private float pullTOCenterMinDistance = 0.1f;

		// Token: 0x04004B41 RID: 19265
		private Collider volume;

		// Token: 0x04004B42 RID: 19266
		public GameObject windSFX;

		// Token: 0x04004B43 RID: 19267
		[SerializeField]
		private MeshRenderer windEffectRenderer;

		// Token: 0x04004B44 RID: 19268
		private bool hasWindFX;

		// Token: 0x04004B45 RID: 19269
		private Vector3 enterPos;
	}
}
