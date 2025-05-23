using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DE8 RID: 3560
	public class NearbyCosmeticsEffect : MonoBehaviour, ISpawnable, ITickSystemTick
	{
		// Token: 0x170008C5 RID: 2245
		// (get) Token: 0x06005822 RID: 22562 RVA: 0x001B22F1 File Offset: 0x001B04F1
		// (set) Token: 0x06005823 RID: 22563 RVA: 0x001B22F9 File Offset: 0x001B04F9
		public bool IsMatched { get; set; }

		// Token: 0x170008C6 RID: 2246
		// (get) Token: 0x06005824 RID: 22564 RVA: 0x001B2302 File Offset: 0x001B0502
		// (set) Token: 0x06005825 RID: 22565 RVA: 0x001B230A File Offset: 0x001B050A
		public VRRig MyRig { get; private set; }

		// Token: 0x170008C7 RID: 2247
		// (get) Token: 0x06005826 RID: 22566 RVA: 0x001B2313 File Offset: 0x001B0513
		// (set) Token: 0x06005827 RID: 22567 RVA: 0x001B231B File Offset: 0x001B051B
		public bool IsSpawned { get; set; }

		// Token: 0x170008C8 RID: 2248
		// (get) Token: 0x06005828 RID: 22568 RVA: 0x001B2324 File Offset: 0x001B0524
		// (set) Token: 0x06005829 RID: 22569 RVA: 0x001B232C File Offset: 0x001B052C
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x170008C9 RID: 2249
		// (get) Token: 0x0600582A RID: 22570 RVA: 0x001B2335 File Offset: 0x001B0535
		// (set) Token: 0x0600582B RID: 22571 RVA: 0x001B233D File Offset: 0x001B053D
		public bool TickRunning { get; set; }

		// Token: 0x0600582C RID: 22572 RVA: 0x001B2346 File Offset: 0x001B0546
		public void OnSpawn(VRRig rig)
		{
			this.MyRig = rig;
		}

		// Token: 0x0600582D RID: 22573 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnDespawn()
		{
		}

		// Token: 0x0600582E RID: 22574 RVA: 0x001B234F File Offset: 0x001B054F
		private void OnEnable()
		{
			TickSystem<object>.AddCallbackTarget(this);
			this.canPlayEffects = true;
			this.IsMatched = false;
			NearbyCosmeticsManager.Instance.Register(this);
		}

		// Token: 0x0600582F RID: 22575 RVA: 0x001B2370 File Offset: 0x001B0570
		public void Tick()
		{
			if (!this.canPlayEffects && Time.time - this.timer >= this.cooldownTime)
			{
				this.canPlayEffects = true;
			}
		}

		// Token: 0x06005830 RID: 22576 RVA: 0x001B2395 File Offset: 0x001B0595
		private void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
			if (NearbyCosmeticsManager.Instance)
			{
				NearbyCosmeticsManager.Instance.Unregister(this);
			}
		}

		// Token: 0x06005831 RID: 22577 RVA: 0x001B23B4 File Offset: 0x001B05B4
		public void PlayEffects(bool playAudio = false)
		{
			if (!this.canPlayEffects)
			{
				return;
			}
			this.timer = Time.time;
			if (this.particlesFX != null)
			{
				this.particlesFX.Play();
			}
			if (playAudio)
			{
				this.audioSource.GTPlay();
			}
			if (this.MyRig.isLocal)
			{
				this.PlayEffectLocal();
			}
			this.canPlayEffects = false;
		}

		// Token: 0x06005832 RID: 22578 RVA: 0x001B2416 File Offset: 0x001B0616
		private void PlayEffectLocal()
		{
			GorillaTagger.Instance.StartVibration(this.leftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x04005D5B RID: 23899
		[SerializeField]
		private bool leftHand;

		// Token: 0x04005D5C RID: 23900
		public string cosmeticType;

		// Token: 0x04005D5D RID: 23901
		[SerializeField]
		private ParticleSystem particlesFX;

		// Token: 0x04005D5E RID: 23902
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04005D5F RID: 23903
		[SerializeField]
		private float hapticStrength = 0.5f;

		// Token: 0x04005D60 RID: 23904
		[SerializeField]
		private float hapticDuration = 0.1f;

		// Token: 0x04005D61 RID: 23905
		[SerializeField]
		private float cooldownTime = 0.5f;

		// Token: 0x04005D62 RID: 23906
		public Transform cosmeticCenter;

		// Token: 0x04005D63 RID: 23907
		private float timer;

		// Token: 0x04005D64 RID: 23908
		private bool canPlayEffects;

		// Token: 0x04005D67 RID: 23911
		private RubberDuckEvents _events;
	}
}
