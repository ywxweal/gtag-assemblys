using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Cosmetics.Summer
{
	// Token: 0x02000E01 RID: 3585
	public class Projectile : MonoBehaviour, IProjectile
	{
		// Token: 0x060058B3 RID: 22707 RVA: 0x001B47D3 File Offset: 0x001B29D3
		protected void Awake()
		{
			this.rigidbody = base.GetComponentInChildren<Rigidbody>();
			this.impactEffectSpawned = false;
		}

		// Token: 0x060058B4 RID: 22708 RVA: 0x000023F4 File Offset: 0x000005F4
		protected void OnEnable()
		{
		}

		// Token: 0x060058B5 RID: 22709 RVA: 0x001B47E8 File Offset: 0x001B29E8
		public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float scale)
		{
			Transform transform = base.transform;
			transform.position = startPosition;
			transform.rotation = startRotation;
			transform.localScale = Vector3.one * scale;
			if (this.rigidbody != null)
			{
				this.rigidbody.velocity = velocity;
			}
			if (this.audioSource)
			{
				this.audioSource.GTPlayOneShot(this.launchAudio, 1f);
			}
		}

		// Token: 0x060058B6 RID: 22710 RVA: 0x001B4857 File Offset: 0x001B2A57
		private bool IsTagValid(GameObject obj)
		{
			return this.collisionTags.Contains(obj.tag);
		}

		// Token: 0x060058B7 RID: 22711 RVA: 0x001B486C File Offset: 0x001B2A6C
		private void OnCollisionEnter(Collision other)
		{
			if (this.impactEffectSpawned)
			{
				return;
			}
			if (this.collisionTags.Count > 0 && !this.IsTagValid(other.gameObject))
			{
				return;
			}
			if (((1 << other.gameObject.layer) & this.collisionLayerMasks) == 0)
			{
				return;
			}
			ContactPoint contact = other.GetContact(0);
			this.SpawnImpactEffect(this.impactEffect, contact.point, contact.normal);
			SoundBankPlayer component = this.impactEffect.GetComponent<SoundBankPlayer>();
			if (component != null && !component.playOnEnable)
			{
				component.Play();
			}
			this.impactEffectSpawned = true;
			if (this.destroyOnCollisionEnter)
			{
				if (this.destroyDelay > 0f)
				{
					base.Invoke("DestroyProjectile", this.destroyDelay);
					return;
				}
				this.DestroyProjectile();
			}
		}

		// Token: 0x060058B8 RID: 22712 RVA: 0x001B4938 File Offset: 0x001B2B38
		private void OnCollisionStay(Collision other)
		{
			if (this.impactEffectSpawned)
			{
				return;
			}
			if (this.collisionTags.Count > 0 && !this.IsTagValid(other.gameObject))
			{
				return;
			}
			if (((1 << other.gameObject.layer) & this.collisionLayerMasks) == 0)
			{
				return;
			}
			ContactPoint contact = other.GetContact(0);
			this.SpawnImpactEffect(this.impactEffect, contact.point, contact.normal);
			SoundBankPlayer component = this.impactEffect.GetComponent<SoundBankPlayer>();
			if (component != null && !component.playOnEnable)
			{
				component.Play();
			}
			this.impactEffectSpawned = true;
			if (this.destroyOnCollisionEnter)
			{
				if (this.destroyDelay > 0f)
				{
					base.Invoke("DestroyProjectile", this.destroyDelay);
					return;
				}
				this.DestroyProjectile();
			}
		}

		// Token: 0x060058B9 RID: 22713 RVA: 0x001B4A04 File Offset: 0x001B2C04
		private void SpawnImpactEffect(GameObject prefab, Vector3 position, Vector3 normal)
		{
			Vector3 vector = position + normal * this.impactEffectOffset;
			GameObject gameObject = ObjectPools.instance.Instantiate(prefab, vector, true);
			gameObject.transform.up = normal;
			gameObject.transform.position = vector;
		}

		// Token: 0x060058BA RID: 22714 RVA: 0x001B4A48 File Offset: 0x001B2C48
		private void DestroyProjectile()
		{
			this.impactEffectSpawned = false;
			if (ObjectPools.instance.DoesPoolExist(base.gameObject))
			{
				ObjectPools.instance.Destroy(base.gameObject);
				return;
			}
			Object.Destroy(base.gameObject);
		}

		// Token: 0x04005E22 RID: 24098
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04005E23 RID: 24099
		[SerializeField]
		private GameObject impactEffect;

		// Token: 0x04005E24 RID: 24100
		[SerializeField]
		private AudioClip launchAudio;

		// Token: 0x04005E25 RID: 24101
		[SerializeField]
		private LayerMask collisionLayerMasks;

		// Token: 0x04005E26 RID: 24102
		[SerializeField]
		private List<string> collisionTags = new List<string>();

		// Token: 0x04005E27 RID: 24103
		[SerializeField]
		private bool destroyOnCollisionEnter;

		// Token: 0x04005E28 RID: 24104
		[SerializeField]
		private float destroyDelay = 1f;

		// Token: 0x04005E29 RID: 24105
		[Tooltip("Distance from the surface that the particle should spawn.")]
		[SerializeField]
		private float impactEffectOffset = 0.1f;

		// Token: 0x04005E2A RID: 24106
		private bool impactEffectSpawned;

		// Token: 0x04005E2B RID: 24107
		private Rigidbody rigidbody;
	}
}
