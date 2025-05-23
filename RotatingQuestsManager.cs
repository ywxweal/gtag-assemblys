using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PlayFab;
using UnityEngine;

// Token: 0x02000146 RID: 326
public class RotatingQuestsManager : MonoBehaviour, ITickSystemTick
{
	// Token: 0x170000D0 RID: 208
	// (get) Token: 0x0600088C RID: 2188 RVA: 0x0002E496 File Offset: 0x0002C696
	// (set) Token: 0x0600088D RID: 2189 RVA: 0x0002E49E File Offset: 0x0002C69E
	public bool TickRunning { get; set; }

	// Token: 0x170000D1 RID: 209
	// (get) Token: 0x0600088E RID: 2190 RVA: 0x0002E4A7 File Offset: 0x0002C6A7
	// (set) Token: 0x0600088F RID: 2191 RVA: 0x0002E4AF File Offset: 0x0002C6AF
	public DateTime DailyQuestCountdown { get; private set; }

	// Token: 0x170000D2 RID: 210
	// (get) Token: 0x06000890 RID: 2192 RVA: 0x0002E4B8 File Offset: 0x0002C6B8
	// (set) Token: 0x06000891 RID: 2193 RVA: 0x0002E4C0 File Offset: 0x0002C6C0
	public DateTime WeeklyQuestCountdown { get; private set; }

	// Token: 0x06000892 RID: 2194 RVA: 0x0002E4C9 File Offset: 0x0002C6C9
	private void Start()
	{
		this._questAudio = base.GetComponent<AudioSource>();
		this.RequestQuestsFromTitleData();
	}

	// Token: 0x06000893 RID: 2195 RVA: 0x0002E4DD File Offset: 0x0002C6DD
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06000894 RID: 2196 RVA: 0x0002E4E5 File Offset: 0x0002C6E5
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x0002E4ED File Offset: 0x0002C6ED
	public void Tick()
	{
		if (this.hasQuest && this.nextQuestUpdateTime < DateTime.UtcNow)
		{
			this.SetupQuests();
		}
	}

	// Token: 0x06000896 RID: 2198 RVA: 0x0002E510 File Offset: 0x0002C710
	private void ProcessAllQuests(Action<RotatingQuestsManager.RotatingQuest> action)
	{
		RotatingQuestsManager.<>c__DisplayClass30_0 CS$<>8__locals1;
		CS$<>8__locals1.action = action;
		RotatingQuestsManager.<ProcessAllQuests>g__ProcessAllQuestsInList|30_0(this.quests.DailyQuests, ref CS$<>8__locals1);
		RotatingQuestsManager.<ProcessAllQuests>g__ProcessAllQuestsInList|30_0(this.quests.WeeklyQuests, ref CS$<>8__locals1);
	}

	// Token: 0x06000897 RID: 2199 RVA: 0x0002E549 File Offset: 0x0002C749
	private void QuestLoadPostProcess(RotatingQuestsManager.RotatingQuest quest)
	{
		if (quest.requiredZones.Count == 1 && quest.requiredZones[0] == GTZone.none)
		{
			quest.requiredZones.Clear();
		}
	}

	// Token: 0x06000898 RID: 2200 RVA: 0x0002E574 File Offset: 0x0002C774
	private void QuestSavePreProcess(RotatingQuestsManager.RotatingQuest quest)
	{
		if (quest.requiredZones.Count == 0)
		{
			quest.requiredZones.Add(GTZone.none);
		}
	}

	// Token: 0x06000899 RID: 2201 RVA: 0x0002E590 File Offset: 0x0002C790
	public void LoadTestQuestsFromFile()
	{
		TextAsset textAsset = Resources.Load<TextAsset>(this.localQuestPath);
		this.LoadQuestsFromJson(textAsset.text);
	}

	// Token: 0x0600089A RID: 2202 RVA: 0x0002E5B5 File Offset: 0x0002C7B5
	public void RequestQuestsFromTitleData()
	{
		PlayFabTitleDataCache.Instance.GetTitleData("AllActiveQuests", delegate(string data)
		{
			this.LoadQuestsFromJson(data);
		}, delegate(PlayFabError e)
		{
			Debug.LogError(string.Format("Error getting AllActiveQuests data: {0}", e));
		});
	}

	// Token: 0x0600089B RID: 2203 RVA: 0x0002E5F4 File Offset: 0x0002C7F4
	private void LoadQuestsFromJson(string jsonString)
	{
		this.quests = JsonConvert.DeserializeObject<RotatingQuestsManager.RotatingQuestList>(jsonString);
		this.ProcessAllQuests(new Action<RotatingQuestsManager.RotatingQuest>(this.QuestLoadPostProcess));
		if (this.quests == null)
		{
			Debug.LogError("Error: Quests failed to parse!");
			return;
		}
		this.hasQuest = true;
		this.quests.Init();
		if (Application.isPlaying)
		{
			this.SetupQuests();
		}
	}

	// Token: 0x0600089C RID: 2204 RVA: 0x0002E654 File Offset: 0x0002C854
	private void SetupQuests()
	{
		this.ClearAllQuestEventListeners();
		this.SelectActiveQuests();
		this.LoadQuestProgress();
		this.HandleQuestProgressChanged(true);
		this.SetupAllQuestEventListeners();
		this.nextQuestUpdateTime = this.DailyQuestCountdown;
		this.nextQuestUpdateTime = this.nextQuestUpdateTime.AddMinutes(1.0);
	}

	// Token: 0x0600089D RID: 2205 RVA: 0x0002E6A8 File Offset: 0x0002C8A8
	private void SelectActiveQuests()
	{
		DateTime dateTime = new DateTime(2025, 1, 10, 18, 0, 0, DateTimeKind.Utc);
		TimeSpan timeSpan = TimeSpan.FromHours(-8.0);
		DateTime dateTime2 = new DateTime(1, 1, 1, 0, 0, 0);
		DateTime dateTime3 = new DateTime(2006, 12, 31, 0, 0, 0);
		TimeSpan timeSpan2 = TimeSpan.FromHours(1.0);
		TimeZoneInfo.TransitionTime transitionTime = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 4, 1, DayOfWeek.Sunday);
		TimeZoneInfo.TransitionTime transitionTime2 = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 10, 5, DayOfWeek.Sunday);
		DateTime dateTime4 = new DateTime(2007, 1, 1, 0, 0, 0);
		DateTime dateTime5 = new DateTime(9999, 12, 31, 0, 0, 0);
		TimeSpan timeSpan3 = TimeSpan.FromHours(1.0);
		TimeZoneInfo.TransitionTime transitionTime3 = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 3, 2, DayOfWeek.Sunday);
		TimeZoneInfo.TransitionTime transitionTime4 = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 11, 1, DayOfWeek.Sunday);
		TimeZoneInfo timeZoneInfo = TimeZoneInfo.CreateCustomTimeZone("Pacific Standard Time", timeSpan, "Pacific Standard Time", "Pacific Standard Time", "Pacific Standard Time", new TimeZoneInfo.AdjustmentRule[]
		{
			TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(dateTime2, dateTime3, timeSpan2, transitionTime, transitionTime2),
			TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(dateTime4, dateTime5, timeSpan3, transitionTime3, transitionTime4)
		});
		if (timeZoneInfo != null && timeZoneInfo.IsDaylightSavingTime(DateTime.UtcNow - timeSpan))
		{
			dateTime -= TimeSpan.FromHours(1.0);
		}
		TimeSpan timeSpan4 = DateTime.UtcNow - dateTime;
		this.RemoveDisabledQuests();
		int days = timeSpan4.Days;
		this.dailyQuestSetID = days;
		this.weeklyQuestSetID = days / 7;
		RotatingQuestsManager.LastQuestDailyID = this.dailyQuestSetID;
		this.DailyQuestCountdown = dateTime + TimeSpan.FromDays((double)(this.dailyQuestSetID + 1));
		this.WeeklyQuestCountdown = dateTime + TimeSpan.FromDays((double)((this.weeklyQuestSetID + 1) * 7));
		Random.InitState(this.dailyQuestSetID);
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in this.quests.DailyQuests)
		{
			int num = Math.Min(rotatingQuestGroup.selectCount, rotatingQuestGroup.quests.Count);
			float num2 = 0f;
			List<ValueTuple<int, float>> list = new List<ValueTuple<int, float>>(rotatingQuestGroup.quests.Count);
			for (int i = 0; i < rotatingQuestGroup.quests.Count; i++)
			{
				rotatingQuestGroup.quests[i].isQuestActive = false;
				num2 += rotatingQuestGroup.quests[i].weight;
				list.Add(new ValueTuple<int, float>(i, rotatingQuestGroup.quests[i].weight));
			}
			for (int j = 0; j < num; j++)
			{
				float num3 = Random.Range(0f, num2);
				for (int k = 0; k < list.Count; k++)
				{
					float item = list[k].Item2;
					if (num3 <= item || k == list.Count - 1)
					{
						num2 -= item;
						int item2 = list[k].Item1;
						list.RemoveAt(k);
						rotatingQuestGroup.quests[item2].isQuestActive = true;
						rotatingQuestGroup.quests[item2].SetRequiredZone();
						break;
					}
					num3 -= item;
				}
			}
		}
		Random.InitState(this.weeklyQuestSetID);
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup2 in this.quests.WeeklyQuests)
		{
			int num4 = Math.Min(rotatingQuestGroup2.selectCount, rotatingQuestGroup2.quests.Count);
			float num5 = 0f;
			List<ValueTuple<int, float>> list2 = new List<ValueTuple<int, float>>(rotatingQuestGroup2.quests.Count);
			for (int l = 0; l < rotatingQuestGroup2.quests.Count; l++)
			{
				rotatingQuestGroup2.quests[l].isQuestActive = false;
				num5 += rotatingQuestGroup2.quests[l].weight;
				list2.Add(new ValueTuple<int, float>(l, rotatingQuestGroup2.quests[l].weight));
			}
			for (int m = 0; m < num4; m++)
			{
				float num6 = Random.Range(0f, num5);
				for (int n = 0; n < list2.Count; n++)
				{
					float item3 = list2[n].Item2;
					if (num6 <= item3 || n == list2.Count - 1)
					{
						num5 -= item3;
						int item4 = list2[n].Item1;
						list2.RemoveAt(n);
						rotatingQuestGroup2.quests[item4].isQuestActive = true;
						rotatingQuestGroup2.quests[item4].SetRequiredZone();
						break;
					}
					num6 -= item3;
				}
			}
		}
		ProgressionController.ReportQuestSelectionChanged();
	}

	// Token: 0x0600089E RID: 2206 RVA: 0x0002EBDC File Offset: 0x0002CDDC
	private void RemoveDisabledQuests()
	{
		RotatingQuestsManager.<RemoveDisabledQuests>g__RemoveDisabledQuestsFromGroupList|38_0(this.quests.DailyQuests);
		RotatingQuestsManager.<RemoveDisabledQuests>g__RemoveDisabledQuestsFromGroupList|38_0(this.quests.WeeklyQuests);
	}

	// Token: 0x0600089F RID: 2207 RVA: 0x0002EC00 File Offset: 0x0002CE00
	private void LoadQuestProgress()
	{
		int @int = PlayerPrefs.GetInt("Rotating_Quest_Daily_SetID_Key", -1);
		int int2 = PlayerPrefs.GetInt("Rotating_Quest_Daily_SaveCount_Key", -1);
		if (@int == this.dailyQuestSetID)
		{
			for (int i = 0; i < int2; i++)
			{
				int int3 = PlayerPrefs.GetInt(string.Format("{0}{1}", "Rotating_Quest_Daily_ID_Key", i), -1);
				int int4 = PlayerPrefs.GetInt(string.Format("{0}{1}", "Rotating_Quest_Daily_Progress_Key", i), -1);
				if (int3 != -1)
				{
					for (int j = 0; j < this.quests.DailyQuests.Count; j++)
					{
						for (int k = 0; k < this.quests.DailyQuests[j].quests.Count; k++)
						{
							RotatingQuestsManager.RotatingQuest rotatingQuest = this.quests.DailyQuests[j].quests[k];
							if (rotatingQuest.questID == int3)
							{
								rotatingQuest.ApplySavedProgress(int4);
								break;
							}
						}
					}
				}
			}
		}
		int int5 = PlayerPrefs.GetInt("Rotating_Quest_Weekly_SetID_Key", -1);
		int int6 = PlayerPrefs.GetInt("Rotating_Quest_Weekly_SaveCount_Key", -1);
		if (int5 == this.weeklyQuestSetID)
		{
			for (int l = 0; l < int6; l++)
			{
				int int7 = PlayerPrefs.GetInt(string.Format("{0}{1}", "Rotating_Quest_Weekly_ID_Key", l), -1);
				int int8 = PlayerPrefs.GetInt(string.Format("{0}{1}", "Rotating_Quest_Weekly_Progress_Key", l), -1);
				if (int7 != -1)
				{
					for (int m = 0; m < this.quests.WeeklyQuests.Count; m++)
					{
						for (int n = 0; n < this.quests.WeeklyQuests[m].quests.Count; n++)
						{
							RotatingQuestsManager.RotatingQuest rotatingQuest2 = this.quests.WeeklyQuests[m].quests[n];
							if (rotatingQuest2.questID == int7)
							{
								rotatingQuest2.ApplySavedProgress(int8);
								break;
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060008A0 RID: 2208 RVA: 0x0002EDFC File Offset: 0x0002CFFC
	private void SaveQuestProgress()
	{
		int num = 0;
		for (int i = 0; i < this.quests.DailyQuests.Count; i++)
		{
			for (int j = 0; j < this.quests.DailyQuests[i].quests.Count; j++)
			{
				RotatingQuestsManager.RotatingQuest rotatingQuest = this.quests.DailyQuests[i].quests[j];
				int progress = rotatingQuest.GetProgress();
				if (progress > 0)
				{
					PlayerPrefs.SetInt(string.Format("{0}{1}", "Rotating_Quest_Daily_ID_Key", num), rotatingQuest.questID);
					PlayerPrefs.SetInt(string.Format("{0}{1}", "Rotating_Quest_Daily_Progress_Key", num), progress);
					num++;
				}
			}
		}
		if (num > 0)
		{
			PlayerPrefs.SetInt("Rotating_Quest_Daily_SetID_Key", this.dailyQuestSetID);
			PlayerPrefs.SetInt("Rotating_Quest_Daily_SaveCount_Key", num);
		}
		int num2 = 0;
		for (int k = 0; k < this.quests.WeeklyQuests.Count; k++)
		{
			for (int l = 0; l < this.quests.WeeklyQuests[k].quests.Count; l++)
			{
				RotatingQuestsManager.RotatingQuest rotatingQuest2 = this.quests.WeeklyQuests[k].quests[l];
				int progress2 = rotatingQuest2.GetProgress();
				if (progress2 > 0)
				{
					PlayerPrefs.SetInt(string.Format("{0}{1}", "Rotating_Quest_Weekly_ID_Key", num2), rotatingQuest2.questID);
					PlayerPrefs.SetInt(string.Format("{0}{1}", "Rotating_Quest_Weekly_Progress_Key", num2), progress2);
					num2++;
				}
			}
		}
		if (num2 > 0)
		{
			PlayerPrefs.SetInt("Rotating_Quest_Weekly_SetID_Key", this.weeklyQuestSetID);
			PlayerPrefs.SetInt("Rotating_Quest_Weekly_SaveCount_Key", num2);
		}
		PlayerPrefs.Save();
	}

	// Token: 0x060008A1 RID: 2209 RVA: 0x0002EFCC File Offset: 0x0002D1CC
	private void SetupAllQuestEventListeners()
	{
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in this.quests.DailyQuests)
		{
			foreach (RotatingQuestsManager.RotatingQuest rotatingQuest in rotatingQuestGroup.quests)
			{
				rotatingQuest.questManager = this;
				if (rotatingQuest.isQuestActive && !rotatingQuest.isQuestComplete)
				{
					rotatingQuest.AddEventListener();
				}
			}
		}
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup2 in this.quests.WeeklyQuests)
		{
			foreach (RotatingQuestsManager.RotatingQuest rotatingQuest2 in rotatingQuestGroup2.quests)
			{
				rotatingQuest2.questManager = this;
				if (rotatingQuest2.isQuestActive && !rotatingQuest2.isQuestComplete)
				{
					rotatingQuest2.AddEventListener();
				}
			}
		}
	}

	// Token: 0x060008A2 RID: 2210 RVA: 0x0002F10C File Offset: 0x0002D30C
	private void ClearAllQuestEventListeners()
	{
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in this.quests.DailyQuests)
		{
			foreach (RotatingQuestsManager.RotatingQuest rotatingQuest in rotatingQuestGroup.quests)
			{
				rotatingQuest.RemoveEventListener();
			}
		}
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup2 in this.quests.WeeklyQuests)
		{
			foreach (RotatingQuestsManager.RotatingQuest rotatingQuest2 in rotatingQuestGroup2.quests)
			{
				rotatingQuest2.RemoveEventListener();
			}
		}
	}

	// Token: 0x060008A3 RID: 2211 RVA: 0x0002F218 File Offset: 0x0002D418
	private void HandleQuestCompleted(int questID)
	{
		RotatingQuestsManager.RotatingQuest quest = this.quests.GetQuest(questID);
		if (quest == null)
		{
			return;
		}
		ProgressionController.ReportQuestComplete(questID, quest.isDailyQuest);
		if (this._playQuestSounds)
		{
			AudioSource questAudio = this._questAudio;
			if (questAudio == null)
			{
				return;
			}
			questAudio.Play();
		}
	}

	// Token: 0x060008A4 RID: 2212 RVA: 0x0002F25A File Offset: 0x0002D45A
	private void HandleQuestProgressChanged(bool initialLoad)
	{
		if (!initialLoad)
		{
			this.SaveQuestProgress();
		}
		RotatingQuestsManager.LastQuestChange = Time.frameCount;
		ProgressionController.ReportQuestChanged(initialLoad);
	}

	// Token: 0x060008A6 RID: 2214 RVA: 0x0002F288 File Offset: 0x0002D488
	[CompilerGenerated]
	internal static void <ProcessAllQuests>g__ProcessAllQuestsInList|30_0(List<RotatingQuestsManager.RotatingQuestGroup> questGroups, ref RotatingQuestsManager.<>c__DisplayClass30_0 A_1)
	{
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in questGroups)
		{
			foreach (RotatingQuestsManager.RotatingQuest rotatingQuest in rotatingQuestGroup.quests)
			{
				A_1.action(rotatingQuest);
			}
		}
	}

	// Token: 0x060008A8 RID: 2216 RVA: 0x0002F320 File Offset: 0x0002D520
	[CompilerGenerated]
	internal static void <RemoveDisabledQuests>g__RemoveDisabledQuestsFromGroupList|38_0(List<RotatingQuestsManager.RotatingQuestGroup> questList)
	{
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in questList)
		{
			for (int i = rotatingQuestGroup.quests.Count - 1; i >= 0; i--)
			{
				if (rotatingQuestGroup.quests[i].disable)
				{
					rotatingQuestGroup.quests.RemoveAt(i);
				}
			}
		}
	}

	// Token: 0x04000A17 RID: 2583
	private bool hasQuest;

	// Token: 0x04000A18 RID: 2584
	[SerializeField]
	private bool useTestLocalQuests;

	// Token: 0x04000A19 RID: 2585
	[SerializeField]
	private string localQuestPath = "TestingRotatingQuests";

	// Token: 0x04000A1A RID: 2586
	public static int LastQuestChange;

	// Token: 0x04000A1B RID: 2587
	public static int LastQuestDailyID;

	// Token: 0x04000A1C RID: 2588
	public RotatingQuestsManager.RotatingQuestList quests;

	// Token: 0x04000A1D RID: 2589
	public int dailyQuestSetID;

	// Token: 0x04000A1E RID: 2590
	public int weeklyQuestSetID;

	// Token: 0x04000A1F RID: 2591
	[SerializeField]
	private bool _playQuestSounds;

	// Token: 0x04000A20 RID: 2592
	private AudioSource _questAudio;

	// Token: 0x04000A23 RID: 2595
	private DateTime nextQuestUpdateTime;

	// Token: 0x04000A24 RID: 2596
	private const string kDailyQuestSetIDKey = "Rotating_Quest_Daily_SetID_Key";

	// Token: 0x04000A25 RID: 2597
	private const string kDailyQuestSaveCountKey = "Rotating_Quest_Daily_SaveCount_Key";

	// Token: 0x04000A26 RID: 2598
	private const string kDailyQuestIDKey = "Rotating_Quest_Daily_ID_Key";

	// Token: 0x04000A27 RID: 2599
	private const string kDailyQuestProgressKey = "Rotating_Quest_Daily_Progress_Key";

	// Token: 0x04000A28 RID: 2600
	private const string kWeeklyQuestSetIDKey = "Rotating_Quest_Weekly_SetID_Key";

	// Token: 0x04000A29 RID: 2601
	private const string kWeeklyQuestSaveCountKey = "Rotating_Quest_Weekly_SaveCount_Key";

	// Token: 0x04000A2A RID: 2602
	private const string kWeeklyQuestIDKey = "Rotating_Quest_Weekly_ID_Key";

	// Token: 0x04000A2B RID: 2603
	private const string kWeeklyQuestProgressKey = "Rotating_Quest_Weekly_Progress_Key";

	// Token: 0x02000147 RID: 327
	[Serializable]
	public class RotatingQuest
	{
		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x060008A9 RID: 2217 RVA: 0x0002F3A0 File Offset: 0x0002D5A0
		[JsonIgnore]
		public bool IsMovementQuest
		{
			get
			{
				return this.questType == QuestType.moveDistance || this.questType == QuestType.swimDistance;
			}
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x060008AA RID: 2218 RVA: 0x0002F3B7 File Offset: 0x0002D5B7
		// (set) Token: 0x060008AB RID: 2219 RVA: 0x0002F3BF File Offset: 0x0002D5BF
		[JsonIgnore]
		public GTZone RequiredZone { get; private set; } = GTZone.none;

		// Token: 0x060008AC RID: 2220 RVA: 0x0002F3C8 File Offset: 0x0002D5C8
		public void SetRequiredZone()
		{
			this.RequiredZone = ((this.requiredZones.Count > 0) ? this.requiredZones[Random.Range(0, this.requiredZones.Count)] : GTZone.none);
		}

		// Token: 0x060008AD RID: 2221 RVA: 0x0002F400 File Offset: 0x0002D600
		public void AddEventListener()
		{
			if (this.isQuestComplete)
			{
				return;
			}
			switch (this.questType)
			{
			case QuestType.gameModeObjective:
				PlayerGameEvents.OnGameModeObjectiveTrigger += this.OnGameEventOccurence;
				return;
			case QuestType.gameModeRound:
				PlayerGameEvents.OnGameModeCompleteRound += this.OnGameEventOccurence;
				return;
			case QuestType.grabObject:
				PlayerGameEvents.OnGrabbedObject += this.OnGameEventOccurence;
				return;
			case QuestType.dropObject:
				PlayerGameEvents.OnDroppedObject += this.OnGameEventOccurence;
				return;
			case QuestType.eatObject:
				PlayerGameEvents.OnEatObject += this.OnGameEventOccurence;
				return;
			case QuestType.tapObject:
				PlayerGameEvents.OnTapObject += this.OnGameEventOccurence;
				return;
			case QuestType.launchedProjectile:
				PlayerGameEvents.OnLaunchedProjectile += this.OnGameEventOccurence;
				return;
			case QuestType.moveDistance:
				PlayerGameEvents.OnPlayerMoved += this.OnGameMoveEvent;
				return;
			case QuestType.swimDistance:
				PlayerGameEvents.OnPlayerSwam += this.OnGameMoveEvent;
				return;
			case QuestType.triggerHandEffect:
				PlayerGameEvents.OnTriggerHandEffect += this.OnGameEventOccurence;
				return;
			case QuestType.enterLocation:
				PlayerGameEvents.OnEnterLocation += this.OnGameEventOccurence;
				return;
			case QuestType.misc:
				PlayerGameEvents.OnMiscEvent += this.OnGameEventOccurence;
				return;
			case QuestType.critter:
				PlayerGameEvents.OnCritterEvent += this.OnGameEventOccurence;
				return;
			default:
				return;
			}
		}

		// Token: 0x060008AE RID: 2222 RVA: 0x0002F544 File Offset: 0x0002D744
		public void RemoveEventListener()
		{
			switch (this.questType)
			{
			case QuestType.gameModeObjective:
				PlayerGameEvents.OnGameModeObjectiveTrigger -= this.OnGameEventOccurence;
				return;
			case QuestType.gameModeRound:
				PlayerGameEvents.OnGameModeCompleteRound -= this.OnGameEventOccurence;
				return;
			case QuestType.grabObject:
				PlayerGameEvents.OnGrabbedObject -= this.OnGameEventOccurence;
				return;
			case QuestType.dropObject:
				PlayerGameEvents.OnDroppedObject -= this.OnGameEventOccurence;
				return;
			case QuestType.eatObject:
				PlayerGameEvents.OnEatObject -= this.OnGameEventOccurence;
				return;
			case QuestType.tapObject:
				PlayerGameEvents.OnTapObject -= this.OnGameEventOccurence;
				return;
			case QuestType.launchedProjectile:
				PlayerGameEvents.OnLaunchedProjectile -= this.OnGameEventOccurence;
				return;
			case QuestType.moveDistance:
				PlayerGameEvents.OnPlayerMoved -= this.OnGameMoveEvent;
				return;
			case QuestType.swimDistance:
				PlayerGameEvents.OnPlayerSwam -= this.OnGameMoveEvent;
				return;
			case QuestType.triggerHandEffect:
				PlayerGameEvents.OnTriggerHandEffect -= this.OnGameEventOccurence;
				return;
			case QuestType.enterLocation:
				PlayerGameEvents.OnEnterLocation -= this.OnGameEventOccurence;
				return;
			case QuestType.misc:
				PlayerGameEvents.OnMiscEvent -= this.OnGameEventOccurence;
				return;
			case QuestType.critter:
				PlayerGameEvents.OnCritterEvent -= this.OnGameEventOccurence;
				return;
			default:
				return;
			}
		}

		// Token: 0x060008AF RID: 2223 RVA: 0x0002F680 File Offset: 0x0002D880
		public void ApplySavedProgress(int progress)
		{
			if (this.questType == QuestType.moveDistance || this.questType == QuestType.swimDistance)
			{
				this.moveDistance = (float)progress;
				this.occurenceCount = Mathf.FloorToInt(this.moveDistance);
				this.isQuestComplete = this.occurenceCount >= this.requiredOccurenceCount;
				return;
			}
			this.occurenceCount = progress;
			this.isQuestComplete = this.occurenceCount >= this.requiredOccurenceCount;
		}

		// Token: 0x060008B0 RID: 2224 RVA: 0x0002F6EF File Offset: 0x0002D8EF
		public int GetProgress()
		{
			if (this.questType == QuestType.moveDistance || this.questType == QuestType.swimDistance)
			{
				return Mathf.FloorToInt(this.moveDistance);
			}
			return this.occurenceCount;
		}

		// Token: 0x060008B1 RID: 2225 RVA: 0x0002F718 File Offset: 0x0002D918
		private void OnGameEventOccurence(string eventName)
		{
			if (this.RequiredZone != GTZone.none && !ZoneManagement.IsInZone(this.RequiredZone))
			{
				return;
			}
			string.IsNullOrEmpty(this.questOccurenceFilter);
			if (eventName.StartsWith(this.questOccurenceFilter))
			{
				this.SetProgress(this.occurenceCount + 1);
			}
		}

		// Token: 0x060008B2 RID: 2226 RVA: 0x0002F765 File Offset: 0x0002D965
		private void OnGameMoveEvent(float distance, float speed)
		{
			if (this.RequiredZone != GTZone.none && !ZoneManagement.IsInZone(this.RequiredZone))
			{
				return;
			}
			this.moveDistance += distance;
			this.SetProgress(Mathf.FloorToInt(this.moveDistance));
		}

		// Token: 0x060008B3 RID: 2227 RVA: 0x0002F7A0 File Offset: 0x0002D9A0
		private void SetProgress(int progress)
		{
			if (this.isQuestComplete)
			{
				return;
			}
			if (this.occurenceCount == progress)
			{
				return;
			}
			this.lastChange = Time.frameCount;
			this.occurenceCount = progress;
			if (this.occurenceCount >= this.requiredOccurenceCount)
			{
				this.Complete();
			}
			this.questManager.HandleQuestProgressChanged(false);
		}

		// Token: 0x060008B4 RID: 2228 RVA: 0x0002F7F2 File Offset: 0x0002D9F2
		private void Complete()
		{
			if (this.isQuestComplete)
			{
				return;
			}
			this.isQuestComplete = true;
			this.RemoveEventListener();
			this.questManager.HandleQuestCompleted(this.questID);
		}

		// Token: 0x060008B5 RID: 2229 RVA: 0x0002F81B File Offset: 0x0002DA1B
		public string GetTextDescription()
		{
			return this.<GetTextDescription>g__GetActionName|30_0().ToUpper() + this.<GetTextDescription>g__GetLocationText|30_1().ToUpper();
		}

		// Token: 0x060008B6 RID: 2230 RVA: 0x0002F838 File Offset: 0x0002DA38
		public string GetProgressText()
		{
			if (!this.isQuestComplete)
			{
				return string.Format("{0}/{1}", this.occurenceCount, this.requiredOccurenceCount);
			}
			return "[DONE]";
		}

		// Token: 0x060008B8 RID: 2232 RVA: 0x0002F898 File Offset: 0x0002DA98
		[CompilerGenerated]
		private string <GetTextDescription>g__GetActionName|30_0()
		{
			switch (this.questType)
			{
			case QuestType.none:
				return "[UNDEFINED]";
			case QuestType.gameModeObjective:
				return this.questName;
			case QuestType.gameModeRound:
				return this.questName;
			case QuestType.grabObject:
				return this.questName;
			case QuestType.dropObject:
				return this.questName;
			case QuestType.eatObject:
				return this.questName;
			case QuestType.launchedProjectile:
				return this.questName;
			case QuestType.moveDistance:
				return this.questName;
			case QuestType.swimDistance:
				return this.questName;
			case QuestType.triggerHandEffect:
				return this.questName;
			case QuestType.enterLocation:
				return this.questName;
			case QuestType.misc:
				return this.questName;
			}
			return this.questName;
		}

		// Token: 0x060008B9 RID: 2233 RVA: 0x0002F95B File Offset: 0x0002DB5B
		[CompilerGenerated]
		private string <GetTextDescription>g__GetLocationText|30_1()
		{
			if (this.RequiredZone == GTZone.none)
			{
				return "";
			}
			return string.Format(" IN {0}", this.RequiredZone);
		}

		// Token: 0x04000A2C RID: 2604
		public bool disable;

		// Token: 0x04000A2D RID: 2605
		public int questID;

		// Token: 0x04000A2E RID: 2606
		public float weight = 1f;

		// Token: 0x04000A2F RID: 2607
		public string questName = "UNNAMED QUEST";

		// Token: 0x04000A30 RID: 2608
		public QuestType questType;

		// Token: 0x04000A31 RID: 2609
		public string questOccurenceFilter;

		// Token: 0x04000A32 RID: 2610
		public int requiredOccurenceCount = 1;

		// Token: 0x04000A33 RID: 2611
		[JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
		public List<GTZone> requiredZones;

		// Token: 0x04000A34 RID: 2612
		[Space]
		[NonSerialized]
		public bool isQuestActive;

		// Token: 0x04000A35 RID: 2613
		[NonSerialized]
		public bool isQuestComplete;

		// Token: 0x04000A36 RID: 2614
		[NonSerialized]
		public bool isDailyQuest;

		// Token: 0x04000A37 RID: 2615
		[NonSerialized]
		public int lastChange;

		// Token: 0x04000A39 RID: 2617
		[NonSerialized]
		public int occurenceCount;

		// Token: 0x04000A3A RID: 2618
		private float moveDistance;

		// Token: 0x04000A3B RID: 2619
		[NonSerialized]
		public RotatingQuestsManager questManager;
	}

	// Token: 0x02000148 RID: 328
	[Serializable]
	public class RotatingQuestGroup
	{
		// Token: 0x04000A3C RID: 2620
		public int selectCount;

		// Token: 0x04000A3D RID: 2621
		public string name;

		// Token: 0x04000A3E RID: 2622
		public List<RotatingQuestsManager.RotatingQuest> quests;
	}

	// Token: 0x02000149 RID: 329
	[Serializable]
	public class RotatingQuestList
	{
		// Token: 0x060008BB RID: 2235 RVA: 0x0002F982 File Offset: 0x0002DB82
		public void Init()
		{
			RotatingQuestsManager.RotatingQuestList.<Init>g__SetIsDaily|2_0(this.DailyQuests, true);
			RotatingQuestsManager.RotatingQuestList.<Init>g__SetIsDaily|2_0(this.WeeklyQuests, false);
		}

		// Token: 0x060008BC RID: 2236 RVA: 0x0002F99C File Offset: 0x0002DB9C
		public RotatingQuestsManager.RotatingQuest GetQuest(int questID)
		{
			RotatingQuestsManager.RotatingQuestList.<>c__DisplayClass3_0 CS$<>8__locals1;
			CS$<>8__locals1.questID = questID;
			RotatingQuestsManager.RotatingQuest rotatingQuest = RotatingQuestsManager.RotatingQuestList.<GetQuest>g__GetQuestFrom|3_0(this.DailyQuests, ref CS$<>8__locals1);
			if (rotatingQuest == null)
			{
				rotatingQuest = RotatingQuestsManager.RotatingQuestList.<GetQuest>g__GetQuestFrom|3_0(this.WeeklyQuests, ref CS$<>8__locals1);
			}
			return rotatingQuest;
		}

		// Token: 0x060008BE RID: 2238 RVA: 0x0002F9D4 File Offset: 0x0002DBD4
		[CompilerGenerated]
		internal static void <Init>g__SetIsDaily|2_0(List<RotatingQuestsManager.RotatingQuestGroup> questList, bool isDaily)
		{
			foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in questList)
			{
				foreach (RotatingQuestsManager.RotatingQuest rotatingQuest in rotatingQuestGroup.quests)
				{
					rotatingQuest.isDailyQuest = isDaily;
				}
			}
		}

		// Token: 0x060008BF RID: 2239 RVA: 0x0002FA5C File Offset: 0x0002DC5C
		[CompilerGenerated]
		internal static RotatingQuestsManager.RotatingQuest <GetQuest>g__GetQuestFrom|3_0(List<RotatingQuestsManager.RotatingQuestGroup> list, ref RotatingQuestsManager.RotatingQuestList.<>c__DisplayClass3_0 A_1)
		{
			foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in list)
			{
				foreach (RotatingQuestsManager.RotatingQuest rotatingQuest in rotatingQuestGroup.quests)
				{
					if (rotatingQuest.questID == A_1.questID)
					{
						return rotatingQuest;
					}
				}
			}
			return null;
		}

		// Token: 0x04000A3F RID: 2623
		public List<RotatingQuestsManager.RotatingQuestGroup> DailyQuests;

		// Token: 0x04000A40 RID: 2624
		public List<RotatingQuestsManager.RotatingQuestGroup> WeeklyQuests;
	}
}
