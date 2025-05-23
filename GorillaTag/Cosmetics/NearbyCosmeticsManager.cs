using System;
using System.Collections.Generic;
using CjLib;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DE9 RID: 3561
	public class NearbyCosmeticsManager : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x170008CA RID: 2250
		// (get) Token: 0x06005835 RID: 22581 RVA: 0x001B2535 File Offset: 0x001B0735
		public static NearbyCosmeticsManager Instance
		{
			get
			{
				return NearbyCosmeticsManager._instance;
			}
		}

		// Token: 0x170008CB RID: 2251
		// (get) Token: 0x06005836 RID: 22582 RVA: 0x001B253C File Offset: 0x001B073C
		// (set) Token: 0x06005837 RID: 22583 RVA: 0x001B2544 File Offset: 0x001B0744
		public bool TickRunning { get; set; }

		// Token: 0x06005838 RID: 22584 RVA: 0x001B254D File Offset: 0x001B074D
		private void Awake()
		{
			if (NearbyCosmeticsManager._instance != null && NearbyCosmeticsManager._instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			NearbyCosmeticsManager._instance = this;
		}

		// Token: 0x06005839 RID: 22585 RVA: 0x0001C94F File Offset: 0x0001AB4F
		private void OnEnable()
		{
			TickSystem<object>.AddCallbackTarget(this);
		}

		// Token: 0x0600583A RID: 22586 RVA: 0x0001C957 File Offset: 0x0001AB57
		private void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
		}

		// Token: 0x0600583B RID: 22587 RVA: 0x001B257B File Offset: 0x001B077B
		private void OnDestroy()
		{
			if (NearbyCosmeticsManager._instance == this)
			{
				NearbyCosmeticsManager._instance = null;
			}
		}

		// Token: 0x0600583C RID: 22588 RVA: 0x001B2590 File Offset: 0x001B0790
		public void Register(NearbyCosmeticsEffect cosmetic)
		{
			this.cosmetics.Add(cosmetic);
		}

		// Token: 0x0600583D RID: 22589 RVA: 0x001B259E File Offset: 0x001B079E
		public void Unregister(NearbyCosmeticsEffect cosmetic)
		{
			this.cosmetics.Remove(cosmetic);
		}

		// Token: 0x0600583E RID: 22590 RVA: 0x001B25B0 File Offset: 0x001B07B0
		public void Tick()
		{
			if (this.cosmetics.Count == 0)
			{
				return;
			}
			this.CheckProximity();
			this.BreakTheBound();
			if (this.debug)
			{
				foreach (NearbyCosmeticsEffect nearbyCosmeticsEffect in this.cosmetics)
				{
					DebugUtil.DrawSphere(nearbyCosmeticsEffect.cosmeticCenter.position, this.proximityThreshold, 6, 6, Color.green, true, DebugUtil.Style.Wireframe);
				}
			}
		}

		// Token: 0x0600583F RID: 22591 RVA: 0x001B263C File Offset: 0x001B083C
		private void CheckProximity()
		{
			for (int i = 0; i < this.cosmetics.Count; i++)
			{
				NearbyCosmeticsEffect nearbyCosmeticsEffect = this.cosmetics[i];
				for (int j = i + 1; j < this.cosmetics.Count; j++)
				{
					NearbyCosmeticsEffect nearbyCosmeticsEffect2 = this.cosmetics[j];
					if ((!(nearbyCosmeticsEffect.MyRig != null) || !(nearbyCosmeticsEffect.MyRig == nearbyCosmeticsEffect2.MyRig)) && !nearbyCosmeticsEffect.IsMatched && !nearbyCosmeticsEffect2.IsMatched && (nearbyCosmeticsEffect.cosmeticCenter.position - nearbyCosmeticsEffect2.cosmeticCenter.position).IsShorterThan(this.proximityThreshold) && !string.IsNullOrEmpty(nearbyCosmeticsEffect.cosmeticType) && string.Equals(nearbyCosmeticsEffect.cosmeticType, nearbyCosmeticsEffect2.cosmeticType))
					{
						nearbyCosmeticsEffect.PlayEffects(true);
						nearbyCosmeticsEffect2.PlayEffects(false);
						nearbyCosmeticsEffect.IsMatched = true;
						nearbyCosmeticsEffect2.IsMatched = true;
					}
				}
			}
		}

		// Token: 0x06005840 RID: 22592 RVA: 0x001B2734 File Offset: 0x001B0934
		private void BreakTheBound()
		{
			for (int i = 0; i < this.cosmetics.Count; i++)
			{
				NearbyCosmeticsEffect nearbyCosmeticsEffect = this.cosmetics[i];
				bool flag = false;
				if (nearbyCosmeticsEffect.IsMatched)
				{
					for (int j = 0; j < this.cosmetics.Count; j++)
					{
						if (i != j)
						{
							NearbyCosmeticsEffect nearbyCosmeticsEffect2 = this.cosmetics[j];
							if ((nearbyCosmeticsEffect.cosmeticCenter.position - nearbyCosmeticsEffect2.cosmeticCenter.position).IsShorterThan(this.proximityThreshold) && !string.IsNullOrEmpty(nearbyCosmeticsEffect.cosmeticType) && string.Equals(nearbyCosmeticsEffect.cosmeticType, nearbyCosmeticsEffect2.cosmeticType))
							{
								flag = true;
								break;
							}
						}
					}
					nearbyCosmeticsEffect.IsMatched = flag;
				}
			}
		}

		// Token: 0x04005D6C RID: 23916
		[SerializeField]
		private float proximityThreshold = 0.1f;

		// Token: 0x04005D6D RID: 23917
		[SerializeField]
		private bool debug;

		// Token: 0x04005D6E RID: 23918
		private List<NearbyCosmeticsEffect> cosmetics = new List<NearbyCosmeticsEffect>();

		// Token: 0x04005D6F RID: 23919
		private static NearbyCosmeticsManager _instance;
	}
}
