using System;
using System.Collections;
using GorillaTag.Cosmetics;
using UnityEngine;

namespace GorillaTag.Shared.Scripts
{
	// Token: 0x02000D6E RID: 3438
	public class FirecrackerProjectile : MonoBehaviour, ITickSystemTick, IProjectile
	{
		// Token: 0x17000897 RID: 2199
		// (get) Token: 0x060055C2 RID: 21954 RVA: 0x001A1711 File Offset: 0x0019F911
		// (set) Token: 0x060055C3 RID: 21955 RVA: 0x001A1719 File Offset: 0x0019F919
		public bool TickRunning { get; set; }

		// Token: 0x060055C4 RID: 21956 RVA: 0x001A1722 File Offset: 0x0019F922
		public void Tick()
		{
			if (Time.time - this.timeCreated > this.forceBackToPoolAfterSec || Time.time - this.timeExploded > this.explosionTime)
			{
				Action<FirecrackerProjectile> onHitComplete = this.OnHitComplete;
				if (onHitComplete == null)
				{
					return;
				}
				onHitComplete(this);
			}
		}

		// Token: 0x060055C5 RID: 21957 RVA: 0x001A1760 File Offset: 0x0019F960
		private void OnEnable()
		{
			TickSystem<object>.AddCallbackTarget(this);
			this.m_timer.Start();
			this.timeExploded = float.PositiveInfinity;
			this.timeCreated = float.PositiveInfinity;
			this.collisionEntered = false;
			if (this.disableWhenHit)
			{
				this.disableWhenHit.SetActive(true);
			}
		}

		// Token: 0x060055C6 RID: 21958 RVA: 0x001A17B4 File Offset: 0x0019F9B4
		private void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
			this.m_timer.Stop();
		}

		// Token: 0x060055C7 RID: 21959 RVA: 0x001A17C7 File Offset: 0x0019F9C7
		private void Awake()
		{
			this.rb = base.GetComponent<Rigidbody>();
			this.audioSource = base.GetComponent<AudioSource>();
			this.m_timer.callback = new Action(this.Detonate);
		}

		// Token: 0x060055C8 RID: 21960 RVA: 0x001A17F8 File Offset: 0x0019F9F8
		private void Detonate()
		{
			this.m_timer.Stop();
			this.timeExploded = Time.time;
			if (this.disableWhenHit)
			{
				this.disableWhenHit.SetActive(false);
			}
			this.collisionEntered = false;
		}

		// Token: 0x060055C9 RID: 21961 RVA: 0x001A1830 File Offset: 0x0019FA30
		public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float scale)
		{
			base.transform.position = startPosition;
			base.transform.rotation = startRotation;
			base.transform.localScale = Vector3.one * scale;
			this.rb.velocity = velocity;
		}

		// Token: 0x060055CA RID: 21962 RVA: 0x001A1870 File Offset: 0x0019FA70
		private void OnCollisionEnter(Collision other)
		{
			if (this.collisionEntered)
			{
				return;
			}
			Vector3 point = other.contacts[0].point;
			Vector3 normal = other.contacts[0].normal;
			if (this.sizzleDuration > 0f)
			{
				base.StartCoroutine(this.Sizzle(point, normal));
			}
			else
			{
				Action<FirecrackerProjectile, Vector3> onHitStart = this.OnHitStart;
				if (onHitStart != null)
				{
					onHitStart(this, point);
				}
				this.Detonate(point, normal);
			}
			this.collisionEntered = true;
		}

		// Token: 0x060055CB RID: 21963 RVA: 0x001A18EA File Offset: 0x0019FAEA
		private IEnumerator Sizzle(Vector3 contactPoint, Vector3 normal)
		{
			if (this.audioSource && this.sizzleAudioClip != null)
			{
				this.audioSource.GTPlayOneShot(this.sizzleAudioClip, 1f);
			}
			yield return new WaitForSeconds(this.sizzleDuration);
			Action<FirecrackerProjectile, Vector3> onHitStart = this.OnHitStart;
			if (onHitStart != null)
			{
				onHitStart(this, contactPoint);
			}
			this.Detonate(contactPoint, normal);
			yield break;
		}

		// Token: 0x060055CC RID: 21964 RVA: 0x001A1908 File Offset: 0x0019FB08
		private void Detonate(Vector3 contactPoint, Vector3 normal)
		{
			this.timeExploded = Time.time;
			GameObject gameObject = ObjectPools.instance.Instantiate(this.explosionEffect, contactPoint, true);
			gameObject.transform.up = normal;
			gameObject.transform.position = base.transform.position;
			SoundBankPlayer soundBankPlayer;
			if (gameObject.TryGetComponent<SoundBankPlayer>(out soundBankPlayer) && soundBankPlayer.soundBank)
			{
				soundBankPlayer.Play();
			}
			if (this.disableWhenHit)
			{
				this.disableWhenHit.SetActive(false);
			}
			this.collisionEntered = false;
		}

		// Token: 0x04005909 RID: 22793
		[SerializeField]
		private GameObject explosionEffect;

		// Token: 0x0400590A RID: 22794
		[SerializeField]
		private float forceBackToPoolAfterSec = 20f;

		// Token: 0x0400590B RID: 22795
		[SerializeField]
		private float explosionTime = 5f;

		// Token: 0x0400590C RID: 22796
		[SerializeField]
		private GameObject disableWhenHit;

		// Token: 0x0400590D RID: 22797
		[SerializeField]
		private float sizzleDuration;

		// Token: 0x0400590E RID: 22798
		[SerializeField]
		private AudioClip sizzleAudioClip;

		// Token: 0x0400590F RID: 22799
		public Action<FirecrackerProjectile> OnHitComplete;

		// Token: 0x04005910 RID: 22800
		public Action<FirecrackerProjectile, Vector3> OnHitStart;

		// Token: 0x04005911 RID: 22801
		private Rigidbody rb;

		// Token: 0x04005912 RID: 22802
		private float timeCreated = float.PositiveInfinity;

		// Token: 0x04005913 RID: 22803
		private float timeExploded = float.PositiveInfinity;

		// Token: 0x04005914 RID: 22804
		private AudioSource audioSource;

		// Token: 0x04005915 RID: 22805
		private TickSystemTimer m_timer = new TickSystemTimer(40f);

		// Token: 0x04005916 RID: 22806
		private bool collisionEntered;
	}
}
