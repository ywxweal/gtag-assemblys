using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DE8 RID: 3560
	public class NearbyCosmeticsEffect : MonoBehaviour, ISpawnable, ITickSystemTick
	{
		// Token: 0x170008C5 RID: 2245
		// (get) Token: 0x06005823 RID: 22563 RVA: 0x001B23C9 File Offset: 0x001B05C9
		// (set) Token: 0x06005824 RID: 22564 RVA: 0x001B23D1 File Offset: 0x001B05D1
		public bool IsMatched { get; set; }

		// Token: 0x170008C6 RID: 2246
		// (get) Token: 0x06005825 RID: 22565 RVA: 0x001B23DA File Offset: 0x001B05DA
		// (set) Token: 0x06005826 RID: 22566 RVA: 0x001B23E2 File Offset: 0x001B05E2
		public VRRig MyRig { get; private set; }

		// Token: 0x170008C7 RID: 2247
		// (get) Token: 0x06005827 RID: 22567 RVA: 0x001B23EB File Offset: 0x001B05EB
		// (set) Token: 0x06005828 RID: 22568 RVA: 0x001B23F3 File Offset: 0x001B05F3
		public bool IsSpawned { get; set; }

		// Token: 0x170008C8 RID: 2248
		// (get) Token: 0x06005829 RID: 22569 RVA: 0x001B23FC File Offset: 0x001B05FC
		// (set) Token: 0x0600582A RID: 22570 RVA: 0x001B2404 File Offset: 0x001B0604
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x170008C9 RID: 2249
		// (get) Token: 0x0600582B RID: 22571 RVA: 0x001B240D File Offset: 0x001B060D
		// (set) Token: 0x0600582C RID: 22572 RVA: 0x001B2415 File Offset: 0x001B0615
		public bool TickRunning { get; set; }

		// Token: 0x0600582D RID: 22573 RVA: 0x001B241E File Offset: 0x001B061E
		public void OnSpawn(VRRig rig)
		{
			this.MyRig = rig;
		}

		// Token: 0x0600582E RID: 22574 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnDespawn()
		{
		}

		// Token: 0x0600582F RID: 22575 RVA: 0x001B2427 File Offset: 0x001B0627
		private void OnEnable()
		{
			TickSystem<object>.AddCallbackTarget(this);
			this.canPlayEffects = true;
			this.IsMatched = false;
			NearbyCosmeticsManager.Instance.Register(this);
		}

		// Token: 0x06005830 RID: 22576 RVA: 0x001B2448 File Offset: 0x001B0648
		public void Tick()
		{
			if (!this.canPlayEffects && Time.time - this.timer >= this.cooldownTime)
			{
				this.canPlayEffects = true;
			}
		}

		// Token: 0x06005831 RID: 22577 RVA: 0x001B246D File Offset: 0x001B066D
		private void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
			if (NearbyCosmeticsManager.Instance)
			{
				NearbyCosmeticsManager.Instance.Unregister(this);
			}
		}

		// Token: 0x06005832 RID: 22578 RVA: 0x001B248C File Offset: 0x001B068C
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

		// Token: 0x06005833 RID: 22579 RVA: 0x001B24EE File Offset: 0x001B06EE
		private void PlayEffectLocal()
		{
			GorillaTagger.Instance.StartVibration(this.leftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x04005D5C RID: 23900
		[SerializeField]
		private bool leftHand;

		// Token: 0x04005D5D RID: 23901
		public string cosmeticType;

		// Token: 0x04005D5E RID: 23902
		[SerializeField]
		private ParticleSystem particlesFX;

		// Token: 0x04005D5F RID: 23903
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04005D60 RID: 23904
		[SerializeField]
		private float hapticStrength = 0.5f;

		// Token: 0x04005D61 RID: 23905
		[SerializeField]
		private float hapticDuration = 0.1f;

		// Token: 0x04005D62 RID: 23906
		[SerializeField]
		private float cooldownTime = 0.5f;

		// Token: 0x04005D63 RID: 23907
		public Transform cosmeticCenter;

		// Token: 0x04005D64 RID: 23908
		private float timer;

		// Token: 0x04005D65 RID: 23909
		private bool canPlayEffects;

		// Token: 0x04005D68 RID: 23912
		private RubberDuckEvents _events;
	}
}
