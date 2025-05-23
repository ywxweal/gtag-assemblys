using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GameObjectScheduling;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000122 RID: 290
public class MonkeVoteMachine : MonoBehaviour
{
	// Token: 0x06000784 RID: 1924 RVA: 0x0002A30E File Offset: 0x0002850E
	private void Reset()
	{
		this.Configure();
	}

	// Token: 0x06000785 RID: 1925 RVA: 0x0002A316 File Offset: 0x00028516
	private void Awake()
	{
		this._proximityTrigger.OnEnter += this.OnPlayerEnteredVoteProximity;
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x0002A330 File Offset: 0x00028530
	private void Start()
	{
		MonkeVoteController.instance.OnPollsUpdated += this.HandleOnPollsUpdated;
		MonkeVoteController.instance.OnVoteAccepted += this.HandleOnVoteAccepted;
		MonkeVoteController.instance.OnVoteFailed += this.HandleOnVoteFailed;
		MonkeVoteController.instance.OnCurrentPollEnded += this.HandleCurrentPollEnded;
		this.Init();
	}

	// Token: 0x06000787 RID: 1927 RVA: 0x0002A39C File Offset: 0x0002859C
	private void OnDestroy()
	{
		this._proximityTrigger.OnEnter -= this.OnPlayerEnteredVoteProximity;
		MonkeVoteController.instance.OnPollsUpdated -= this.HandleOnPollsUpdated;
		MonkeVoteController.instance.OnVoteAccepted -= this.HandleOnVoteAccepted;
		MonkeVoteController.instance.OnVoteFailed -= this.HandleOnVoteFailed;
		MonkeVoteController.instance.OnCurrentPollEnded -= this.HandleCurrentPollEnded;
	}

	// Token: 0x06000788 RID: 1928 RVA: 0x0002A418 File Offset: 0x00028618
	public void Init()
	{
		this._isTestingPoll = false;
		this._previousPoll = (this._currentPoll = null);
		this._waitingOnVote = false;
		foreach (MonkeVoteOption monkeVoteOption in this._votingOptions)
		{
			monkeVoteOption.ResetState();
			monkeVoteOption.OnVote += this.OnVoteEntered;
		}
		this.UpdatePollDisplays();
	}

	// Token: 0x06000789 RID: 1929 RVA: 0x0002A478 File Offset: 0x00028678
	private void OnPlayerEnteredVoteProximity()
	{
		MonkeVoteController.instance.RequestPolls();
	}

	// Token: 0x0600078A RID: 1930 RVA: 0x0002A484 File Offset: 0x00028684
	private void HandleOnPollsUpdated()
	{
		this.UpdatePollDisplays();
	}

	// Token: 0x0600078B RID: 1931 RVA: 0x0002A48C File Offset: 0x0002868C
	private void UpdatePollDisplays()
	{
		if (MonkeVoteController.instance == null)
		{
			this.SetState(MonkeVoteMachine.VotingState.None, true);
			this.ShowResults(null);
			return;
		}
		MonkeVoteController.FetchPollsResponse lastPollData = MonkeVoteController.instance.GetLastPollData();
		if (lastPollData != null)
		{
			this._previousPoll = new MonkeVoteMachine.PollEntry(lastPollData);
			this.ShowResults(this._previousPoll);
		}
		else
		{
			this.ShowResults(null);
		}
		MonkeVoteController.FetchPollsResponse currentPollData = MonkeVoteController.instance.GetCurrentPollData();
		if (currentPollData == null)
		{
			this.SetState(MonkeVoteMachine.VotingState.None, true);
			return;
		}
		this._nextPollUpdate = MonkeVoteController.instance.GetCurrentPollCompletionTime();
		this._currentPoll = new MonkeVoteMachine.PollEntry(currentPollData);
		MonkeVoteMachine.PollEntry currentPoll = this._currentPoll;
		if (currentPoll != null && currentPoll.IsValid)
		{
			ValueTuple<int, int> vote = this.GetVote(this._currentPoll.PollId);
			int item = vote.Item1;
			int item2 = vote.Item2;
			MonkeVoteMachine.VotingState votingState = ((item < 0) ? MonkeVoteMachine.VotingState.Voting : ((item2 < 0) ? MonkeVoteMachine.VotingState.Predicting : MonkeVoteMachine.VotingState.Complete));
			this.SetState(votingState, true);
			return;
		}
		this.SetState(MonkeVoteMachine.VotingState.None, true);
	}

	// Token: 0x0600078C RID: 1932 RVA: 0x0002A570 File Offset: 0x00028770
	private void HandleOnVoteAccepted()
	{
		int lastVotePollId = MonkeVoteController.instance.GetLastVotePollId();
		int lastVoteSelectedOption = MonkeVoteController.instance.GetLastVoteSelectedOption();
		bool lastVoteWasPrediction = MonkeVoteController.instance.GetLastVoteWasPrediction();
		this.OnVoteResponseReceived(lastVotePollId, lastVoteSelectedOption, lastVoteWasPrediction, true);
	}

	// Token: 0x0600078D RID: 1933 RVA: 0x0002A5A8 File Offset: 0x000287A8
	private void HandleOnVoteFailed()
	{
		this._waitingOnVote = false;
		int lastVotePollId = MonkeVoteController.instance.GetLastVotePollId();
		int lastVoteSelectedOption = MonkeVoteController.instance.GetLastVoteSelectedOption();
		bool lastVoteWasPrediction = MonkeVoteController.instance.GetLastVoteWasPrediction();
		this.OnVoteResponseReceived(lastVotePollId, lastVoteSelectedOption, lastVoteWasPrediction, false);
	}

	// Token: 0x0600078E RID: 1934 RVA: 0x0002A5E7 File Offset: 0x000287E7
	private void HandleCurrentPollEnded()
	{
		if (this._proximityTrigger.isPlayerNearby)
		{
			MonkeVoteController.instance.RequestPolls();
		}
	}

	// Token: 0x0600078F RID: 1935 RVA: 0x0002A600 File Offset: 0x00028800
	[Tooltip("Hide dynamic child meshes to avoid them getting combined into the parent mesh on awake")]
	private void HideDynamicMeshes()
	{
		this.SetDynamicMeshesVisible(false);
	}

	// Token: 0x06000790 RID: 1936 RVA: 0x0002A609 File Offset: 0x00028809
	[Tooltip("Show dynamic child meshes to allow easy visualization")]
	private void ShowDynamicMeshes()
	{
		this.SetDynamicMeshesVisible(true);
	}

	// Token: 0x06000791 RID: 1937 RVA: 0x0002A614 File Offset: 0x00028814
	private void SetDynamicMeshesVisible(bool enabled)
	{
		MonkeVoteOption[] votingOptions = this._votingOptions;
		for (int i = 0; i < votingOptions.Length; i++)
		{
			votingOptions[i].SetDynamicMeshesVisible(enabled);
		}
		MonkeVoteResult[] results = this._results;
		for (int i = 0; i < results.Length; i++)
		{
			results[i].SetDynamicMeshesVisible(enabled);
		}
	}

	// Token: 0x06000792 RID: 1938 RVA: 0x0002A65D File Offset: 0x0002885D
	private void Configure()
	{
		this._audio = base.GetComponentInChildren<AudioSource>();
		this._audio.spatialBlend = 1f;
		this._votingOptions = base.GetComponentsInChildren<MonkeVoteOption>();
		this._results = base.GetComponentsInChildren<MonkeVoteResult>();
	}

	// Token: 0x06000793 RID: 1939 RVA: 0x0002A694 File Offset: 0x00028894
	public void CreateNextDummyPoll()
	{
		this._isTestingPoll = true;
		if (this._currentPoll != null)
		{
			this._previousPoll = this._currentPoll;
		}
		else
		{
			this._previousPoll = null;
		}
		this.ShowResults(this._previousPoll);
		int num = 0;
		if (this._previousPoll != null)
		{
			num = this._previousPoll.PollId + 1;
		}
		string text = "Test Question Number: " + Random.Range(1, 101).ToString();
		string text2 = "Answer " + Random.Range(1, 101).ToString();
		string text3 = "Answer " + Random.Range(1, 101).ToString();
		string[] array = new string[] { text2, text3 };
		this._currentPoll = new MonkeVoteMachine.PollEntry(num, text, array);
		MonkeVoteMachine.PollEntry currentPoll = this._currentPoll;
		if (currentPoll != null && currentPoll.IsValid)
		{
			ValueTuple<int, int> vote = this.GetVote(this._currentPoll.PollId);
			int item = vote.Item1;
			int item2 = vote.Item2;
			MonkeVoteMachine.VotingState votingState = ((item < 0) ? MonkeVoteMachine.VotingState.Voting : ((item2 < 0) ? MonkeVoteMachine.VotingState.Predicting : MonkeVoteMachine.VotingState.Complete));
			this.SetState(votingState, true);
			return;
		}
		this.SetState(MonkeVoteMachine.VotingState.None, true);
	}

	// Token: 0x06000794 RID: 1940 RVA: 0x0002A7B6 File Offset: 0x000289B6
	private void VoteLeft()
	{
		this.OnVoteEntered(this._votingOptions[0], null);
	}

	// Token: 0x06000795 RID: 1941 RVA: 0x0002A7C7 File Offset: 0x000289C7
	private void VoteRight()
	{
		this.OnVoteEntered(this._votingOptions[1], null);
	}

	// Token: 0x06000796 RID: 1942 RVA: 0x0002A7D8 File Offset: 0x000289D8
	private void VoteWinner()
	{
		if (this._currentPoll != null)
		{
			if (this._currentPoll.VoteCount[0] > this._currentPoll.VoteCount[1])
			{
				this.OnVoteEntered(this._votingOptions[0], null);
				return;
			}
			this.OnVoteEntered(this._votingOptions[1], null);
		}
	}

	// Token: 0x06000797 RID: 1943 RVA: 0x0002A828 File Offset: 0x00028A28
	private void ClearLocalData()
	{
		this.ClearLocalVoteAndPredictionData();
		this.UpdatePollDisplays();
	}

	// Token: 0x06000798 RID: 1944 RVA: 0x0002A838 File Offset: 0x00028A38
	private void SetState(MonkeVoteMachine.VotingState newState, bool instant = true)
	{
		this._state = newState;
		MonkeVoteMachine.PollEntry currentPoll = this._currentPoll;
		bool flag = currentPoll != null && currentPoll.IsValid;
		if (this._state < MonkeVoteMachine.VotingState.None || this._state > MonkeVoteMachine.VotingState.Complete || (this._state != MonkeVoteMachine.VotingState.None && !flag))
		{
			this._state = MonkeVoteMachine.VotingState.None;
		}
		if (flag)
		{
			int item = this.GetVote(this._currentPoll.PollId).Item2;
			if (this._state < MonkeVoteMachine.VotingState.Predicting)
			{
				this.SaveVote(this._currentPoll.PollId, -1, item);
			}
			int item2 = this.GetVote(this._currentPoll.PollId).Item1;
			if (this._state < MonkeVoteMachine.VotingState.Complete)
			{
				this.SaveVote(this._currentPoll.PollId, item2, -1);
			}
		}
		bool flag2 = true;
		switch (this._state)
		{
		case MonkeVoteMachine.VotingState.None:
			this._timerText.SetFixedText(this._pollsClosedText);
			this._titleText.text = this._defaultTitle;
			this._questionText.text = this._defaultQuestion;
			flag2 = false;
			break;
		case MonkeVoteMachine.VotingState.Voting:
			this._timerText.SetCountdownTime(this._nextPollUpdate);
			this._titleText.text = this._voteTitle;
			this._questionText.text = this._currentPoll.Question;
			break;
		case MonkeVoteMachine.VotingState.Predicting:
			this._timerText.SetCountdownTime(this._nextPollUpdate);
			this._titleText.text = this._predictTitle;
			this._questionText.text = this._predictQuestion;
			break;
		case MonkeVoteMachine.VotingState.Complete:
			this._timerText.SetCountdownTime(this._nextPollUpdate);
			this._titleText.text = this._completeTitle;
			this._questionText.text = this._currentPoll.Question;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		int num;
		int num2;
		if (!flag)
		{
			num = -1;
			num2 = -1;
		}
		else
		{
			ValueTuple<int, int> vote = this.GetVote(this._currentPoll.PollId);
			num = vote.Item1;
			num2 = vote.Item2;
		}
		if (flag2)
		{
			for (int i = 0; i < this._votingOptions.Length; i++)
			{
				this._votingOptions[i].Text = this._currentPoll.VoteOptions[i];
				this._votingOptions[i].ShowIndicators(num == i, num2 == i, instant);
			}
			return;
		}
		foreach (MonkeVoteOption monkeVoteOption in this._votingOptions)
		{
			monkeVoteOption.Text = string.Empty;
			monkeVoteOption.ShowIndicators(false, false, true);
		}
	}

	// Token: 0x06000799 RID: 1945 RVA: 0x0002AAB4 File Offset: 0x00028CB4
	private void ShowResults(MonkeVoteMachine.PollEntry entry)
	{
		if (entry != null && entry.IsValid)
		{
			ValueTuple<int, int> vote = this.GetVote(entry.PollId);
			int item = vote.Item1;
			int item2 = vote.Item2;
			GTDev.Log<string>(string.Format("Showing {0} V:{1} P:{2}", entry.Question, item, item2), null);
			List<int> list = this.ConvertToPercentages(entry.VoteCount);
			int num = 0;
			int num2 = -1;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] > num)
				{
					num = list[i];
					num2 = i;
				}
			}
			this._resultsTitleText.text = this._defaultResultsTitle;
			this._resultsQuestionText.text = entry.Question;
			for (int j = 0; j < entry.VoteOptions.Length; j++)
			{
				this._results[j].ShowResult(entry.VoteOptions[j], list[j], item == j, item2 == j, num2 == j);
			}
			int prePollStreak = this.GetPrePollStreak(entry.PollId);
			int postPollStreak = this.GetPostPollStreak(entry);
			this._resultsStreakText.text = ((postPollStreak >= prePollStreak) ? string.Format(this._streakBlurb, postPollStreak) : string.Format(this._streakLostBlurb, prePollStreak, postPollStreak));
			return;
		}
		this._resultsTitleText.text = this._defaultResultsTitle;
		this._resultsQuestionText.text = this._defaultQuestion;
		this._resultsStreakText.text = string.Empty;
		MonkeVoteResult[] results = this._results;
		for (int k = 0; k < results.Length; k++)
		{
			results[k].HideResult();
		}
	}

	// Token: 0x0600079A RID: 1946 RVA: 0x0002AC64 File Offset: 0x00028E64
	private List<int> ConvertToPercentages(int[] votes)
	{
		List<int> list = new List<int>();
		List<float> list2 = new List<float>();
		if (votes == null || votes.Length == 0)
		{
			list.Add(-1);
			list.Add(-1);
			return list;
		}
		if (votes.Length == 1)
		{
			list.Add(100);
			list.Add(0);
			return list;
		}
		int num = MonkeVoteMachine.<ConvertToPercentages>g__Sum|64_0(votes);
		if (num == 0)
		{
			list.Add(-1);
			list.Add(-1);
			return list;
		}
		int num2 = -1;
		int num3 = 0;
		for (int i = 0; i < votes.Length; i++)
		{
			if (votes[i] > num2)
			{
				num2 = votes[i];
				num3 = i;
			}
			float num4 = (float)votes[i] / (float)num * 100f;
			list.Add((int)num4);
			list2.Add(num4 - (float)((int)num4));
		}
		int num5 = MonkeVoteMachine.<ConvertToPercentages>g__Sum|64_0(list);
		int num6 = 100 - num5;
		for (int j = 0; j < num6; j++)
		{
			int num7 = MonkeVoteMachine.<ConvertToPercentages>g__LargestFractionIndex|64_1(list2);
			List<int> list3 = list;
			int num8 = num7;
			int num9 = list3[num8];
			list3[num8] = num9 + 1;
			list2[num7] = 0f;
		}
		if (list.Count == 2 && list[num3] == 50)
		{
			List<int> list4 = list;
			int num9 = num3;
			list4[num9]++;
			list4 = list;
			num9 = 1 - num3;
			list4[num9]--;
		}
		return list;
	}

	// Token: 0x0600079B RID: 1947 RVA: 0x0002ADB0 File Offset: 0x00028FB0
	private void OnVoteEntered(MonkeVoteOption option, Collider votingCollider)
	{
		if (this._waitingOnVote || (Time.time < this._voteCooldownEnd && !this._isTestingPoll))
		{
			this.PlayVoteFailEffects();
			return;
		}
		int num = Array.IndexOf<MonkeVoteOption>(this._votingOptions, option);
		if (num < 0)
		{
			return;
		}
		switch (this._state)
		{
		case MonkeVoteMachine.VotingState.Voting:
			this.Vote(this._currentPoll.PollId, num, false);
			return;
		case MonkeVoteMachine.VotingState.Predicting:
			this.Vote(this._currentPoll.PollId, num, true);
			return;
		}
		this.PlayVoteFailEffects();
	}

	// Token: 0x0600079C RID: 1948 RVA: 0x0002AE40 File Offset: 0x00029040
	private void Vote(int id, int option, bool isPrediction)
	{
		if (option < 0 || this._waitingOnVote)
		{
			return;
		}
		this._waitingOnVote = true;
		if (this._isTestingPoll)
		{
			this.OnVoteResponseReceived(id, option, isPrediction, true);
			return;
		}
		MonkeVoteController.instance.Vote(id, option, isPrediction);
	}

	// Token: 0x0600079D RID: 1949 RVA: 0x0002AE78 File Offset: 0x00029078
	private void OnVoteResponseReceived(int id, int option, bool isPrediction, bool success)
	{
		this._waitingOnVote = false;
		if (success)
		{
			this.PlayVoteSuccessEffects();
			this._voteCooldownEnd = Time.time + this._voteCooldown;
			ValueTuple<int, int> vote = this.GetVote(id);
			int num = vote.Item1;
			int num2 = vote.Item2;
			if (!isPrediction)
			{
				int num3 = num2;
				num = option;
				num2 = num3;
			}
			else
			{
				num = num;
				num2 = option;
			}
			this.SaveVote(id, num, num2);
			MonkeVoteMachine.VotingState state = this._state;
			if (state != MonkeVoteMachine.VotingState.Voting)
			{
				if (state == MonkeVoteMachine.VotingState.Predicting)
				{
					this.SetState(MonkeVoteMachine.VotingState.Complete, false);
				}
			}
			else
			{
				this.SetState(MonkeVoteMachine.VotingState.Predicting, false);
			}
			if (isPrediction && id == this._currentPoll.PollId)
			{
				this.SavePrePollStreak(id, this.GetPostPollStreak(this._previousPoll));
				return;
			}
		}
		else
		{
			this.PlayVoteFailEffects();
		}
	}

	// Token: 0x0600079E RID: 1950 RVA: 0x0002AF2C File Offset: 0x0002912C
	private async void PlayVoteSuccessEffects()
	{
		this._audio.GTPlayOneShot(this._voteSuccessDing, this._audio.volume);
		await Task.Delay(500);
		this._voteTubeAudio.GTPlayOneShot(this._voteProcessingSound, this._voteTubeAudio.volume);
	}

	// Token: 0x0600079F RID: 1951 RVA: 0x0002AF63 File Offset: 0x00029163
	private void PlayVoteFailEffects()
	{
		this._audio.GTPlayOneShot(this._voteFailSound, this._audio.volume);
	}

	// Token: 0x060007A0 RID: 1952 RVA: 0x0002AF84 File Offset: 0x00029184
	private void SaveVote(int id, int voteOption, int predictionOption)
	{
		int @int = PlayerPrefs.GetInt("Vote_Current_Id", -1);
		if (@int == -1 || @int == id)
		{
			PlayerPrefs.SetInt("Vote_Current_Id", id);
			PlayerPrefs.SetInt("Vote_Current_Option", voteOption);
			PlayerPrefs.SetInt("Vote_Current_Prediction", predictionOption);
		}
		else
		{
			PlayerPrefs.SetInt("Vote_Previous_Id", @int);
			PlayerPrefs.SetInt("Vote_Previous_Option", PlayerPrefs.GetInt("Vote_Current_Option"));
			PlayerPrefs.SetInt("Vote_Previous_Prediction", PlayerPrefs.GetInt("Vote_Current_Prediction"));
			PlayerPrefs.SetInt("Vote_Previous_Streak", PlayerPrefs.GetInt("Vote_Current_Streak"));
			PlayerPrefs.SetInt("Vote_Current_Id", id);
			PlayerPrefs.SetInt("Vote_Current_Option", voteOption);
			PlayerPrefs.SetInt("Vote_Current_Prediction", predictionOption);
			PlayerPrefs.SetInt("Vote_Current_Streak", 0);
		}
		PlayerPrefs.Save();
	}

	// Token: 0x060007A1 RID: 1953 RVA: 0x0002B040 File Offset: 0x00029240
	[return: TupleElementNames(new string[] { "voteOption", "predictionOption" })]
	private ValueTuple<int, int> GetVote(int voteId)
	{
		if (PlayerPrefs.GetInt("Vote_Current_Id", -1) == voteId)
		{
			int @int = PlayerPrefs.GetInt("Vote_Current_Option", -1);
			int int2 = PlayerPrefs.GetInt("Vote_Current_Prediction", -1);
			return new ValueTuple<int, int>(@int, int2);
		}
		if (PlayerPrefs.GetInt("Vote_Previous_Id", -1) == voteId)
		{
			int int3 = PlayerPrefs.GetInt("Vote_Previous_Option", -1);
			int int4 = PlayerPrefs.GetInt("Vote_Previous_Prediction", -1);
			return new ValueTuple<int, int>(int3, int4);
		}
		return new ValueTuple<int, int>(-1, -1);
	}

	// Token: 0x060007A2 RID: 1954 RVA: 0x0002B0AC File Offset: 0x000292AC
	private void SavePrePollStreak(int id, int streak)
	{
		if (id < 0)
		{
			return;
		}
		if (PlayerPrefs.GetInt("Vote_Current_Id", -1) == id)
		{
			PlayerPrefs.SetInt("Vote_Current_Streak", streak);
			return;
		}
		if (PlayerPrefs.GetInt("Vote_Previous_Id", -1) == id)
		{
			PlayerPrefs.SetInt("Vote_Previous_Streak", streak);
		}
	}

	// Token: 0x060007A3 RID: 1955 RVA: 0x0002B0E6 File Offset: 0x000292E6
	private int GetPrePollStreak(int id)
	{
		if (id < 0)
		{
			return 0;
		}
		if (PlayerPrefs.GetInt("Vote_Current_Id", -1) == id)
		{
			return PlayerPrefs.GetInt("Vote_Current_Streak", 0);
		}
		if (PlayerPrefs.GetInt("Vote_Previous_Id", -1) == id)
		{
			return PlayerPrefs.GetInt("Vote_Previous_Streak", 0);
		}
		return 0;
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x0002B124 File Offset: 0x00029324
	private int GetPostPollStreak(MonkeVoteMachine.PollEntry entry)
	{
		if (entry == null || !entry.IsValid)
		{
			return 0;
		}
		int item = this.GetVote(entry.PollId).Item2;
		if (item < 0)
		{
			return 0;
		}
		int prePollStreak = this.GetPrePollStreak(entry.PollId);
		if (item != entry.GetWinner())
		{
			return 0;
		}
		return prePollStreak + 1;
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x0002B174 File Offset: 0x00029374
	private void ClearLocalVoteAndPredictionData()
	{
		PlayerPrefs.DeleteKey("Vote_Current_Id");
		PlayerPrefs.DeleteKey("Vote_Current_Option");
		PlayerPrefs.DeleteKey("Vote_Current_Prediction");
		PlayerPrefs.DeleteKey("Vote_Current_Streak");
		PlayerPrefs.DeleteKey("Vote_Previous_Id");
		PlayerPrefs.DeleteKey("Vote_Previous_Option");
		PlayerPrefs.DeleteKey("Vote_Previous_Prediction");
		PlayerPrefs.DeleteKey("Vote_Previous_Streak");
	}

	// Token: 0x060007A7 RID: 1959 RVA: 0x0002B260 File Offset: 0x00029460
	[CompilerGenerated]
	internal static int <ConvertToPercentages>g__Sum|64_0(IList<int> items)
	{
		int num = 0;
		foreach (int num2 in items)
		{
			num += num2;
		}
		return num;
	}

	// Token: 0x060007A8 RID: 1960 RVA: 0x0002B2A8 File Offset: 0x000294A8
	[CompilerGenerated]
	internal static int <ConvertToPercentages>g__LargestFractionIndex|64_1(IList<float> fractions)
	{
		float num = float.NegativeInfinity;
		int num2 = -1;
		for (int i = 0; i < fractions.Count; i++)
		{
			if (fractions[i] > num)
			{
				num = fractions[i];
				num2 = i;
			}
		}
		return num2;
	}

	// Token: 0x04000904 RID: 2308
	private const string kVoteCurrentIdKey = "Vote_Current_Id";

	// Token: 0x04000905 RID: 2309
	private const string kVoteCurrentOptionKey = "Vote_Current_Option";

	// Token: 0x04000906 RID: 2310
	private const string kVoteCurrentPredictionKey = "Vote_Current_Prediction";

	// Token: 0x04000907 RID: 2311
	private const string kVoteCurrentStreak = "Vote_Current_Streak";

	// Token: 0x04000908 RID: 2312
	private const string kVotePreviousIdKey = "Vote_Previous_Id";

	// Token: 0x04000909 RID: 2313
	private const string kVotePreviousOptionKey = "Vote_Previous_Option";

	// Token: 0x0400090A RID: 2314
	private const string kVotePreviousPredictionKey = "Vote_Previous_Prediction";

	// Token: 0x0400090B RID: 2315
	private const string kVotePreviousStreak = "Vote_Previous_Streak";

	// Token: 0x0400090C RID: 2316
	[SerializeField]
	private MonkeVoteProximityTrigger _proximityTrigger;

	// Token: 0x0400090D RID: 2317
	[Header("VOTING")]
	[SerializeField]
	private string _pollsClosedText = "POLLS CLOSED";

	// Token: 0x0400090E RID: 2318
	[SerializeField]
	private string _defaultTitle = "MONKE VOTE";

	// Token: 0x0400090F RID: 2319
	[SerializeField]
	private string _voteTitle = "VOTE";

	// Token: 0x04000910 RID: 2320
	[SerializeField]
	private string _predictTitle = "GUESS";

	// Token: 0x04000911 RID: 2321
	[SerializeField]
	private string _completeTitle = "VOTING COMPLETE";

	// Token: 0x04000912 RID: 2322
	[SerializeField]
	private string _defaultQuestion = "COME BACK LATER";

	// Token: 0x04000913 RID: 2323
	[SerializeField]
	private string _predictQuestion = "WHICH WILL BE MORE POPULAR?";

	// Token: 0x04000914 RID: 2324
	[Tooltip("Must be in the format \"STREAK: {0}\"")]
	[SerializeField]
	private string _streakBlurb = "PREDICTION STREAK: {0}";

	// Token: 0x04000915 RID: 2325
	[Tooltip("Must be in the format \"LOST {0} PREDICTION STREAK! STREAK: {1}\"")]
	[SerializeField]
	private string _streakLostBlurb = "<color=red>{0} POLL STREAK LOST!</color>  STREAK: {1}";

	// Token: 0x04000916 RID: 2326
	[SerializeField]
	private float _voteCooldown = 1f;

	// Token: 0x04000917 RID: 2327
	[SerializeField]
	private MonkeVoteOption[] _votingOptions;

	// Token: 0x04000918 RID: 2328
	[SerializeField]
	private CountdownText _timerText;

	// Token: 0x04000919 RID: 2329
	[SerializeField]
	private TMP_Text _titleText;

	// Token: 0x0400091A RID: 2330
	[SerializeField]
	private TMP_Text _questionText;

	// Token: 0x0400091B RID: 2331
	[Header("RESULTS")]
	[SerializeField]
	private string _defaultResultsTitle = "PREVIOUS QUESTION";

	// Token: 0x0400091C RID: 2332
	[SerializeField]
	private TMP_Text _resultsTitleText;

	// Token: 0x0400091D RID: 2333
	[SerializeField]
	private TMP_Text _resultsQuestionText;

	// Token: 0x0400091E RID: 2334
	[SerializeField]
	private TMP_Text _resultsStreakText;

	// Token: 0x0400091F RID: 2335
	[SerializeField]
	private MonkeVoteResult[] _results;

	// Token: 0x04000920 RID: 2336
	[FormerlySerializedAs("_sound")]
	[Header("FX")]
	[SerializeField]
	private AudioSource _audio;

	// Token: 0x04000921 RID: 2337
	[FormerlySerializedAs("_voteProcessingAudio")]
	[SerializeField]
	private AudioSource _voteTubeAudio;

	// Token: 0x04000922 RID: 2338
	[SerializeField]
	private AudioClip[] _voteFailSound;

	// Token: 0x04000923 RID: 2339
	[SerializeField]
	private AudioClip[] _voteSuccessDing;

	// Token: 0x04000924 RID: 2340
	[FormerlySerializedAs("_voteSuccessSound")]
	[SerializeField]
	private AudioClip[] _voteProcessingSound;

	// Token: 0x04000925 RID: 2341
	private MonkeVoteMachine.VotingState _state;

	// Token: 0x04000926 RID: 2342
	private float _voteCooldownEnd;

	// Token: 0x04000927 RID: 2343
	private bool _waitingOnVote;

	// Token: 0x04000928 RID: 2344
	private MonkeVoteMachine.PollEntry _currentPoll;

	// Token: 0x04000929 RID: 2345
	private MonkeVoteMachine.PollEntry _previousPoll;

	// Token: 0x0400092A RID: 2346
	private DateTime _nextPollUpdate;

	// Token: 0x0400092B RID: 2347
	private bool _isTestingPoll;

	// Token: 0x02000123 RID: 291
	public enum VotingState
	{
		// Token: 0x0400092D RID: 2349
		None,
		// Token: 0x0400092E RID: 2350
		Voting,
		// Token: 0x0400092F RID: 2351
		Predicting,
		// Token: 0x04000930 RID: 2352
		Complete
	}

	// Token: 0x02000124 RID: 292
	public class PollEntry
	{
		// Token: 0x060007A9 RID: 1961 RVA: 0x0002B2E4 File Offset: 0x000294E4
		public PollEntry(int pollId, string question, string[] voteOptions)
		{
			this.PollId = pollId;
			this.Question = question;
			this.VoteOptions = voteOptions;
			this.VoteCount = new int[2];
			this.VoteCount[0] = Random.Range(0, 50000);
			this.VoteCount[1] = Random.Range(0, 50000);
			this.PredictionCount = new int[2];
			this.PredictionCount[0] = Random.Range(0, 50000);
			this.PredictionCount[1] = Random.Range(0, 50000);
			this.StartTime = DateTime.Now;
			this.EndTime = DateTime.Now + TimeSpan.FromSeconds(20.0);
		}

		// Token: 0x060007AA RID: 1962 RVA: 0x0002B39C File Offset: 0x0002959C
		public PollEntry(MonkeVoteController.FetchPollsResponse poll)
		{
			this.PollId = poll.PollId;
			this.Question = poll.Question;
			this.VoteOptions = poll.VoteOptions.ToArray();
			this.VoteCount = poll.VoteCount.ToArray();
			this.PredictionCount = poll.PredictionCount.ToArray();
			this.StartTime = poll.StartTime;
			this.EndTime = poll.EndTime;
		}

		// Token: 0x060007AB RID: 1963 RVA: 0x0002B414 File Offset: 0x00029614
		public int GetWinner()
		{
			if (this.VoteCount == null || this.VoteCount.Length == 0)
			{
				return -1;
			}
			int num = int.MinValue;
			int num2 = -1;
			for (int i = 0; i < this.VoteCount.Length; i++)
			{
				if (this.VoteCount[i] > num)
				{
					num = this.VoteCount[i];
					num2 = i;
				}
			}
			return num2;
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x060007AC RID: 1964 RVA: 0x0002B468 File Offset: 0x00029668
		public bool IsValid
		{
			get
			{
				string[] voteOptions = this.VoteOptions;
				return voteOptions != null && voteOptions.Length == 2;
			}
		}

		// Token: 0x04000931 RID: 2353
		public int PollId;

		// Token: 0x04000932 RID: 2354
		public string Question;

		// Token: 0x04000933 RID: 2355
		public string[] VoteOptions;

		// Token: 0x04000934 RID: 2356
		public int[] VoteCount;

		// Token: 0x04000935 RID: 2357
		public int[] PredictionCount;

		// Token: 0x04000936 RID: 2358
		public DateTime StartTime;

		// Token: 0x04000937 RID: 2359
		public DateTime EndTime;
	}
}
