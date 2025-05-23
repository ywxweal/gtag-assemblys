using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x02000948 RID: 2376
[CreateAssetMenu(menuName = "ScriptableObjects/RoomSystemSettings", order = 2)]
internal class RoomSystemSettings : ScriptableObject
{
	// Token: 0x170005AF RID: 1455
	// (get) Token: 0x060039AF RID: 14767 RVA: 0x001161A3 File Offset: 0x001143A3
	public ExpectedUsersDecayTimer ExpectedUsersTimer
	{
		get
		{
			return this.expectedUsersTimer;
		}
	}

	// Token: 0x170005B0 RID: 1456
	// (get) Token: 0x060039B0 RID: 14768 RVA: 0x001161AB File Offset: 0x001143AB
	public CallLimiterWithCooldown StatusEffectLimiter
	{
		get
		{
			return this.statusEffectLimiter;
		}
	}

	// Token: 0x170005B1 RID: 1457
	// (get) Token: 0x060039B1 RID: 14769 RVA: 0x001161B3 File Offset: 0x001143B3
	public CallLimiterWithCooldown SoundEffectLimiter
	{
		get
		{
			return this.soundEffectLimiter;
		}
	}

	// Token: 0x170005B2 RID: 1458
	// (get) Token: 0x060039B2 RID: 14770 RVA: 0x001161BB File Offset: 0x001143BB
	public CallLimiterWithCooldown SoundEffectOtherLimiter
	{
		get
		{
			return this.soundEffectOtherLimiter;
		}
	}

	// Token: 0x170005B3 RID: 1459
	// (get) Token: 0x060039B3 RID: 14771 RVA: 0x001161C3 File Offset: 0x001143C3
	public CallLimiterWithCooldown PlayerEffectLimiter
	{
		get
		{
			return this.playerEffectLimiter;
		}
	}

	// Token: 0x170005B4 RID: 1460
	// (get) Token: 0x060039B4 RID: 14772 RVA: 0x001161CB File Offset: 0x001143CB
	public GameObject PlayerImpactEffect
	{
		get
		{
			return this.playerImpactEffect;
		}
	}

	// Token: 0x170005B5 RID: 1461
	// (get) Token: 0x060039B5 RID: 14773 RVA: 0x001161D3 File Offset: 0x001143D3
	public List<RoomSystem.PlayerEffectConfig> PlayerEffects
	{
		get
		{
			return this.playerEffects;
		}
	}

	// Token: 0x170005B6 RID: 1462
	// (get) Token: 0x060039B6 RID: 14774 RVA: 0x001161DB File Offset: 0x001143DB
	public int PausedDCTimer
	{
		get
		{
			return this.pausedDCTimer;
		}
	}

	// Token: 0x04003ED6 RID: 16086
	[SerializeField]
	private ExpectedUsersDecayTimer expectedUsersTimer;

	// Token: 0x04003ED7 RID: 16087
	[SerializeField]
	private CallLimiterWithCooldown statusEffectLimiter;

	// Token: 0x04003ED8 RID: 16088
	[SerializeField]
	private CallLimiterWithCooldown soundEffectLimiter;

	// Token: 0x04003ED9 RID: 16089
	[SerializeField]
	private CallLimiterWithCooldown soundEffectOtherLimiter;

	// Token: 0x04003EDA RID: 16090
	[SerializeField]
	private CallLimiterWithCooldown playerEffectLimiter;

	// Token: 0x04003EDB RID: 16091
	[SerializeField]
	private GameObject playerImpactEffect;

	// Token: 0x04003EDC RID: 16092
	[SerializeField]
	private List<RoomSystem.PlayerEffectConfig> playerEffects = new List<RoomSystem.PlayerEffectConfig>();

	// Token: 0x04003EDD RID: 16093
	[SerializeField]
	private int pausedDCTimer;
}
