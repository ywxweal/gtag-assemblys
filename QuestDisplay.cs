using System;
using TMPro;
using UnityEngine;

// Token: 0x02000141 RID: 321
public class QuestDisplay : MonoBehaviour
{
	// Token: 0x170000CB RID: 203
	// (get) Token: 0x06000874 RID: 2164 RVA: 0x0002E0A6 File Offset: 0x0002C2A6
	public bool IsChanged
	{
		get
		{
			return this.quest.lastChange > this._lastUpdate;
		}
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x0002E0BC File Offset: 0x0002C2BC
	public void UpdateDisplay()
	{
		this.text.text = this.quest.GetTextDescription();
		if (this.quest.isQuestComplete)
		{
			this.progressDisplay.SetVisible(false);
		}
		else if (this.quest.requiredOccurenceCount > 1)
		{
			this.progressDisplay.SetProgress(this.quest.occurenceCount, this.quest.requiredOccurenceCount);
			this.progressDisplay.SetVisible(true);
		}
		else
		{
			this.progressDisplay.SetVisible(false);
		}
		this.UpdateCompletionIndicator();
		this._lastUpdate = Time.frameCount;
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x0002E154 File Offset: 0x0002C354
	private void UpdateCompletionIndicator()
	{
		bool isQuestComplete = this.quest.isQuestComplete;
		bool flag = !isQuestComplete && this.quest.requiredOccurenceCount == 1;
		this.dailyIncompleteIndicator.SetActive(this.quest.isDailyQuest && flag);
		this.dailyCompleteIndicator.SetActive(this.quest.isDailyQuest && isQuestComplete);
		this.weeklyIncompleteIndicator.SetActive(!this.quest.isDailyQuest && flag);
		this.weeklyCompleteIndicator.SetActive(!this.quest.isDailyQuest && isQuestComplete);
	}

	// Token: 0x040009F2 RID: 2546
	[SerializeField]
	private ProgressDisplay progressDisplay;

	// Token: 0x040009F3 RID: 2547
	[SerializeField]
	private TMP_Text text;

	// Token: 0x040009F4 RID: 2548
	[SerializeField]
	private TMP_Text statusText;

	// Token: 0x040009F5 RID: 2549
	[SerializeField]
	private GameObject dailyIncompleteIndicator;

	// Token: 0x040009F6 RID: 2550
	[SerializeField]
	private GameObject dailyCompleteIndicator;

	// Token: 0x040009F7 RID: 2551
	[SerializeField]
	private GameObject weeklyIncompleteIndicator;

	// Token: 0x040009F8 RID: 2552
	[SerializeField]
	private GameObject weeklyCompleteIndicator;

	// Token: 0x040009F9 RID: 2553
	[NonSerialized]
	public RotatingQuestsManager.RotatingQuest quest;

	// Token: 0x040009FA RID: 2554
	private int _lastUpdate = -1;
}
