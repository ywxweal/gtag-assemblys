using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000128 RID: 296
public class MonkeVoteResult : MonoBehaviour
{
	// Token: 0x170000BE RID: 190
	// (get) Token: 0x060007C6 RID: 1990 RVA: 0x0002B851 File Offset: 0x00029A51
	// (set) Token: 0x060007C7 RID: 1991 RVA: 0x0002B85C File Offset: 0x00029A5C
	public string Text
	{
		get
		{
			return this._text;
		}
		set
		{
			TMP_Text optionText = this._optionText;
			this._text = value;
			optionText.text = value;
		}
	}

	// Token: 0x060007C8 RID: 1992 RVA: 0x0002B880 File Offset: 0x00029A80
	public void ShowResult(string questionOption, int percentage, bool showVote, bool showPrediction, bool isWinner)
	{
		this._optionText.text = questionOption;
		this._optionIndicator.SetActive(true);
		this._scoreText.text = ((percentage >= 0) ? string.Format("{0}%", percentage) : "--");
		this._voteIndicator.SetActive(showVote);
		this._guessWinIndicator.SetActive(showPrediction && isWinner);
		this._guessLoseIndicator.SetActive(showPrediction && !isWinner);
		this._youWinIndicator.SetActive(isWinner && showPrediction);
		this._mostPopularIndicator.SetActive(isWinner);
		this.ShowRockPile(percentage);
	}

	// Token: 0x060007C9 RID: 1993 RVA: 0x0002B924 File Offset: 0x00029B24
	public void HideResult()
	{
		this._optionIndicator.SetActive(false);
		this._voteIndicator.SetActive(false);
		this._guessWinIndicator.SetActive(false);
		this._guessLoseIndicator.SetActive(false);
		this._youWinIndicator.SetActive(false);
		this._mostPopularIndicator.SetActive(false);
		this.ShowRockPile(0);
	}

	// Token: 0x060007CA RID: 1994 RVA: 0x0002B980 File Offset: 0x00029B80
	private void ShowRockPile(int percentage)
	{
		this._rockPiles.Show(percentage);
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x0002B990 File Offset: 0x00029B90
	public void SetDynamicMeshesVisible(bool visible)
	{
		this._mostPopularIndicator.SetActive(visible);
		this._voteIndicator.SetActive(visible);
		this._guessWinIndicator.SetActive(visible);
		this._guessLoseIndicator.SetActive(visible);
		this._rockPiles.Show(visible ? 100 : (-1));
	}

	// Token: 0x04000947 RID: 2375
	[SerializeField]
	private GameObject _optionIndicator;

	// Token: 0x04000948 RID: 2376
	[SerializeField]
	private TMP_Text _optionText;

	// Token: 0x04000949 RID: 2377
	[FormerlySerializedAs("_scoreLabelPost")]
	[SerializeField]
	private GameObject _scoreIndicator;

	// Token: 0x0400094A RID: 2378
	[SerializeField]
	private TMP_Text _scoreText;

	// Token: 0x0400094B RID: 2379
	[SerializeField]
	private GameObject _voteIndicator;

	// Token: 0x0400094C RID: 2380
	[SerializeField]
	private GameObject _guessWinIndicator;

	// Token: 0x0400094D RID: 2381
	[SerializeField]
	private GameObject _guessLoseIndicator;

	// Token: 0x0400094E RID: 2382
	[SerializeField]
	private GameObject _mostPopularIndicator;

	// Token: 0x0400094F RID: 2383
	[SerializeField]
	private GameObject _youWinIndicator;

	// Token: 0x04000950 RID: 2384
	[SerializeField]
	private RockPiles _rockPiles;

	// Token: 0x04000951 RID: 2385
	private MonkeVoteMachine _machine;

	// Token: 0x04000952 RID: 2386
	private string _text = string.Empty;

	// Token: 0x04000953 RID: 2387
	private bool _canVote;

	// Token: 0x04000954 RID: 2388
	private float _rockPileHeight;
}
