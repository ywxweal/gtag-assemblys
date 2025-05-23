using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameObjectScheduling;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200012D RID: 301
public class MonkeBusinessStation : MonoBehaviourPunCallbacks
{
	// Token: 0x060007DB RID: 2011 RVA: 0x0002BC88 File Offset: 0x00029E88
	public override void OnEnable()
	{
		base.OnEnable();
		this.FindQuestManager();
		ProgressionController.OnQuestSelectionChanged += this.OnQuestSelectionChanged;
		ProgressionController.OnProgressEvent += this.OnProgress;
		ProgressionController.RequestProgressUpdate();
		this.UpdateCountdownTimers();
	}

	// Token: 0x060007DC RID: 2012 RVA: 0x0002BCC3 File Offset: 0x00029EC3
	public override void OnDisable()
	{
		base.OnDisable();
		ProgressionController.OnQuestSelectionChanged -= this.OnQuestSelectionChanged;
		ProgressionController.OnProgressEvent -= this.OnProgress;
	}

	// Token: 0x060007DD RID: 2013 RVA: 0x0002BCED File Offset: 0x00029EED
	private void FindQuestManager()
	{
		if (!this._questManager)
		{
			this._questManager = Object.FindObjectOfType<RotatingQuestsManager>();
		}
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x0002BD07 File Offset: 0x00029F07
	private void UpdateCountdownTimers()
	{
		this._dailyCountdown.SetCountdownTime(this._questManager.DailyQuestCountdown);
		this._weeklyCountdown.SetCountdownTime(this._questManager.WeeklyQuestCountdown);
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x0002BD35 File Offset: 0x00029F35
	private void OnQuestSelectionChanged()
	{
		this.UpdateCountdownTimers();
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x0002BD3D File Offset: 0x00029F3D
	private void OnProgress()
	{
		this.UpdateQuestStatus();
		this.UpdateProgressDisplays();
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x0002BD4C File Offset: 0x00029F4C
	private void UpdateProgressDisplays()
	{
		ValueTuple<int, int, int> progressionData = ProgressionController.GetProgressionData();
		int item = progressionData.Item1;
		int item2 = progressionData.Item2;
		this._weeklyProgress.SetProgress(item, ProgressionController.WeeklyCap);
		if (!this._isUpdatingPointCount)
		{
			this._unclaimedPoints.text = item2.ToString();
			this._claimButton.isOn = item2 > 0;
		}
		bool flag = item2 > 0;
		this._claimablePointsObject.SetActive(flag);
		this._noClaimablePointsObject.SetActive(!flag);
		this._badgeMount.position = (flag ? this._claimablePointsBadgePosition.position : this._noClaimablePointsBadgePosition.position);
		this._claimButton.gameObject.SetActive(flag);
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x0002BE00 File Offset: 0x0002A000
	private void UpdateQuestStatus()
	{
		if (this._lastQuestChange >= RotatingQuestsManager.LastQuestChange)
		{
			return;
		}
		this.FindQuestManager();
		if (this._quests.Count == 0 || this._lastQuestDailyID != RotatingQuestsManager.LastQuestDailyID)
		{
			this.BuildQuestList();
		}
		foreach (QuestDisplay questDisplay in this._quests)
		{
			if (questDisplay.IsChanged)
			{
				questDisplay.UpdateDisplay();
			}
		}
		this._lastQuestChange = Time.frameCount;
		this._lastQuestDailyID = RotatingQuestsManager.LastQuestDailyID;
	}

	// Token: 0x060007E3 RID: 2019 RVA: 0x0002BEA4 File Offset: 0x0002A0A4
	public void RedeemProgress()
	{
		if (this._claimButton.isOn)
		{
			this._isUpdatingPointCount = true;
			ValueTuple<int, int, int> progressionData = ProgressionController.GetProgressionData();
			int item = progressionData.Item2;
			int item2 = progressionData.Item3;
			this._tempUnclaimedPoints = item;
			this._tempTotalPoints = item2;
			this._claimButton.isOn = false;
			ProgressionController.RedeemProgress();
			if (PhotonNetwork.InRoom)
			{
				base.photonView.RPC("BroadcastRedeemQuestPoints", RpcTarget.Others, new object[] { this._tempUnclaimedPoints });
			}
			base.StartCoroutine(this.PerformPointRedemptionSequence());
		}
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x0002BF2F File Offset: 0x0002A12F
	private IEnumerator PerformPointRedemptionSequence()
	{
		while (this._tempUnclaimedPoints > 0)
		{
			this._tempUnclaimedPoints--;
			this._tempTotalPoints++;
			this._unclaimedPoints.text = this._tempUnclaimedPoints.ToString();
			if (this._tempUnclaimedPoints == 0)
			{
				this._audioSource.PlayOneShot(this._claimPointFinalSFX);
			}
			else
			{
				this._audioSource.PlayOneShot(this._claimPointDefaultSFX);
			}
			yield return new WaitForSeconds(this._claimDelayPerPoint);
		}
		this._isUpdatingPointCount = false;
		this.UpdateProgressDisplays();
		yield break;
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x0002BF40 File Offset: 0x0002A140
	[PunRPC]
	private void BroadcastRedeemQuestPoints(int redeemedPointCount, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "BroadcastRedeemQuestPoints");
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		RigContainer rigContainer;
		if (photonMessageInfoWrapped.Sender != null && VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			if (!FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 10, (double)Time.unscaledTime))
			{
				return;
			}
			redeemedPointCount = Mathf.Min(redeemedPointCount, 50);
			Coroutine coroutine;
			if (this.perPlayerRedemptionSequence.TryGetValue(info.Sender, out coroutine))
			{
				if (coroutine != null)
				{
					base.StopCoroutine(coroutine);
				}
				this.perPlayerRedemptionSequence.Remove(info.Sender);
			}
			if (base.gameObject.activeInHierarchy)
			{
				Coroutine coroutine2 = base.StartCoroutine(this.PerformRemotePointRedemptionSequence(info.Sender, redeemedPointCount));
				this.perPlayerRedemptionSequence.Add(info.Sender, coroutine2);
			}
		}
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x0002C020 File Offset: 0x0002A220
	private IEnumerator PerformRemotePointRedemptionSequence(NetPlayer player, int redeemedPointCount)
	{
		while (redeemedPointCount > 0)
		{
			int num = redeemedPointCount;
			redeemedPointCount = num - 1;
			if (redeemedPointCount == 0)
			{
				this._audioSource.PlayOneShot(this._claimPointFinalSFX);
			}
			else
			{
				this._audioSource.PlayOneShot(this._claimPointDefaultSFX);
			}
			yield return new WaitForSeconds(this._claimDelayPerPoint);
		}
		this.perPlayerRedemptionSequence.Remove(player);
		yield break;
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x0002C040 File Offset: 0x0002A240
	private void BuildQuestList()
	{
		this.DestroyQuestList();
		RotatingQuestsManager.RotatingQuestList quests = this._questManager.quests;
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in quests.DailyQuests)
		{
			foreach (RotatingQuestsManager.RotatingQuest rotatingQuest in rotatingQuestGroup.quests)
			{
				if (rotatingQuest.isQuestActive)
				{
					QuestDisplay questDisplay = Object.Instantiate<QuestDisplay>(this._questDisplayPrefab, this._dailyQuestContainer);
					questDisplay.quest = rotatingQuest;
					this._quests.Add(questDisplay);
				}
			}
		}
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup2 in quests.WeeklyQuests)
		{
			foreach (RotatingQuestsManager.RotatingQuest rotatingQuest2 in rotatingQuestGroup2.quests)
			{
				if (rotatingQuest2.isQuestActive)
				{
					QuestDisplay questDisplay2 = Object.Instantiate<QuestDisplay>(this._questDisplayPrefab, this._weeklyQuestContainer);
					questDisplay2.quest = rotatingQuest2;
					this._quests.Add(questDisplay2);
				}
			}
		}
		foreach (QuestDisplay questDisplay3 in this._quests)
		{
			questDisplay3.UpdateDisplay();
		}
		if (!this._hasBuiltQuestList)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(this._questContainerParent);
			this._hasBuiltQuestList = true;
			return;
		}
		LayoutRebuilder.MarkLayoutForRebuild(this._questContainerParent);
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x0002C214 File Offset: 0x0002A414
	private void DestroyQuestList()
	{
		MonkeBusinessStation.<DestroyQuestList>g__DestroyChildren|40_0(this._dailyQuestContainer);
		MonkeBusinessStation.<DestroyQuestList>g__DestroyChildren|40_0(this._weeklyQuestContainer);
		this._quests.Clear();
	}

	// Token: 0x060007EA RID: 2026 RVA: 0x0002C264 File Offset: 0x0002A464
	[CompilerGenerated]
	internal static void <DestroyQuestList>g__DestroyChildren|40_0(Transform parent)
	{
		for (int i = parent.childCount - 1; i >= 0; i--)
		{
			Object.Destroy(parent.GetChild(i).gameObject);
		}
	}

	// Token: 0x04000963 RID: 2403
	[SerializeField]
	private RectTransform _questContainerParent;

	// Token: 0x04000964 RID: 2404
	[SerializeField]
	private RectTransform _dailyQuestContainer;

	// Token: 0x04000965 RID: 2405
	[SerializeField]
	private RectTransform _weeklyQuestContainer;

	// Token: 0x04000966 RID: 2406
	[SerializeField]
	private QuestDisplay _questDisplayPrefab;

	// Token: 0x04000967 RID: 2407
	[SerializeField]
	private List<QuestDisplay> _quests;

	// Token: 0x04000968 RID: 2408
	[SerializeField]
	private ProgressDisplay _weeklyProgress;

	// Token: 0x04000969 RID: 2409
	[SerializeField]
	private TMP_Text _unclaimedPoints;

	// Token: 0x0400096A RID: 2410
	[SerializeField]
	private GorillaPressableButton _claimButton;

	// Token: 0x0400096B RID: 2411
	[SerializeField]
	private AudioSource _audioSource;

	// Token: 0x0400096C RID: 2412
	[SerializeField]
	private GameObject _claimablePointsObject;

	// Token: 0x0400096D RID: 2413
	[SerializeField]
	private GameObject _noClaimablePointsObject;

	// Token: 0x0400096E RID: 2414
	[SerializeField]
	private Transform _claimablePointsBadgePosition;

	// Token: 0x0400096F RID: 2415
	[SerializeField]
	private Transform _noClaimablePointsBadgePosition;

	// Token: 0x04000970 RID: 2416
	[SerializeField]
	private Transform _badgeMount;

	// Token: 0x04000971 RID: 2417
	[Space]
	[SerializeField]
	private float _claimDelayPerPoint = 0.12f;

	// Token: 0x04000972 RID: 2418
	[SerializeField]
	private AudioClip _claimPointDefaultSFX;

	// Token: 0x04000973 RID: 2419
	[SerializeField]
	private AudioClip _claimPointFinalSFX;

	// Token: 0x04000974 RID: 2420
	[Header("Quest Timers")]
	[SerializeField]
	private CountdownText _dailyCountdown;

	// Token: 0x04000975 RID: 2421
	[SerializeField]
	private CountdownText _weeklyCountdown;

	// Token: 0x04000976 RID: 2422
	private RotatingQuestsManager _questManager;

	// Token: 0x04000977 RID: 2423
	private int _lastQuestChange = -1;

	// Token: 0x04000978 RID: 2424
	private int _lastQuestDailyID = -1;

	// Token: 0x04000979 RID: 2425
	private bool _isUpdatingPointCount;

	// Token: 0x0400097A RID: 2426
	private int _tempUnclaimedPoints;

	// Token: 0x0400097B RID: 2427
	private int _tempTotalPoints;

	// Token: 0x0400097C RID: 2428
	private bool _hasBuiltQuestList;

	// Token: 0x0400097D RID: 2429
	private Dictionary<NetPlayer, Coroutine> perPlayerRedemptionSequence = new Dictionary<NetPlayer, Coroutine>();
}
