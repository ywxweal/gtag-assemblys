using System;
using System.Collections;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using TMPro;
using UnityEngine;

// Token: 0x02000142 RID: 322
public class RotatingQuestBadge : MonoBehaviour, ISpawnable
{
	// Token: 0x170000CC RID: 204
	// (get) Token: 0x06000878 RID: 2168 RVA: 0x0002E1F7 File Offset: 0x0002C3F7
	// (set) Token: 0x06000879 RID: 2169 RVA: 0x0002E1FF File Offset: 0x0002C3FF
	public bool IsSpawned { get; set; }

	// Token: 0x170000CD RID: 205
	// (get) Token: 0x0600087A RID: 2170 RVA: 0x0002E208 File Offset: 0x0002C408
	// (set) Token: 0x0600087B RID: 2171 RVA: 0x0002E210 File Offset: 0x0002C410
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x0600087C RID: 2172 RVA: 0x0002E21C File Offset: 0x0002C41C
	public void OnSpawn(VRRig rig)
	{
		if (this.forWardrobe && !this.myRig)
		{
			this.TryGetRig();
			return;
		}
		this.myRig = rig;
		this.myRig.OnQuestScoreChanged += this.OnProgressScoreChanged;
		this.OnProgressScoreChanged(this.myRig.GetCurrentQuestScore());
	}

	// Token: 0x0600087D RID: 2173 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnDespawn()
	{
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x0002E275 File Offset: 0x0002C475
	private void OnEnable()
	{
		if (this.forWardrobe)
		{
			this.SetBadgeLevel(-1);
			if (!this.TryGetRig())
			{
				base.StartCoroutine(this.DoFindRig());
			}
		}
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x0002E29B File Offset: 0x0002C49B
	private void OnDisable()
	{
		if (this.forWardrobe && this.myRig)
		{
			this.myRig.OnQuestScoreChanged -= this.OnProgressScoreChanged;
			this.myRig = null;
		}
	}

	// Token: 0x06000880 RID: 2176 RVA: 0x0002E2D0 File Offset: 0x0002C4D0
	private IEnumerator DoFindRig()
	{
		WaitForSeconds intervalWait = new WaitForSeconds(0.1f);
		while (!this.TryGetRig())
		{
			yield return intervalWait;
		}
		yield break;
	}

	// Token: 0x06000881 RID: 2177 RVA: 0x0002E2E0 File Offset: 0x0002C4E0
	private bool TryGetRig()
	{
		GorillaTagger instance = GorillaTagger.Instance;
		this.myRig = ((instance != null) ? instance.offlineVRRig : null);
		if (this.myRig)
		{
			this.myRig.OnQuestScoreChanged += this.OnProgressScoreChanged;
			this.OnProgressScoreChanged(this.myRig.GetCurrentQuestScore());
			return true;
		}
		return false;
	}

	// Token: 0x06000882 RID: 2178 RVA: 0x0002E33C File Offset: 0x0002C53C
	private void OnProgressScoreChanged(int score)
	{
		score = Mathf.Clamp(score, 0, 99999);
		this.displayField.text = score.ToString();
		this.UpdateBadge(score);
	}

	// Token: 0x06000883 RID: 2179 RVA: 0x0002E368 File Offset: 0x0002C568
	private void UpdateBadge(int score)
	{
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < this.badgeLevels.Length; i++)
		{
			if (this.badgeLevels[i].requiredPoints <= score && this.badgeLevels[i].requiredPoints > num)
			{
				num = this.badgeLevels[i].requiredPoints;
				num2 = i;
			}
		}
		this.SetBadgeLevel(num2);
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x0002E3D0 File Offset: 0x0002C5D0
	private void SetBadgeLevel(int level)
	{
		level = Mathf.Clamp(level, 0, this.badgeLevels.Length - 1);
		for (int i = 0; i < this.badgeLevels.Length; i++)
		{
			this.badgeLevels[i].badge.SetActive(i == level);
		}
	}

	// Token: 0x040009FB RID: 2555
	[SerializeField]
	private TextMeshPro displayField;

	// Token: 0x040009FC RID: 2556
	[SerializeField]
	private bool forWardrobe;

	// Token: 0x040009FD RID: 2557
	[SerializeField]
	private VRRig myRig;

	// Token: 0x040009FE RID: 2558
	[SerializeField]
	private RotatingQuestBadge.BadgeLevel[] badgeLevels;

	// Token: 0x02000143 RID: 323
	[Serializable]
	public struct BadgeLevel
	{
		// Token: 0x04000A01 RID: 2561
		public GameObject badge;

		// Token: 0x04000A02 RID: 2562
		public int requiredPoints;
	}
}
