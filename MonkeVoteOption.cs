using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000126 RID: 294
public class MonkeVoteOption : MonoBehaviour
{
	// Token: 0x14000011 RID: 17
	// (add) Token: 0x060007AF RID: 1967 RVA: 0x0002B588 File Offset: 0x00029788
	// (remove) Token: 0x060007B0 RID: 1968 RVA: 0x0002B5C0 File Offset: 0x000297C0
	public event Action<MonkeVoteOption, Collider> OnVote;

	// Token: 0x170000BB RID: 187
	// (get) Token: 0x060007B1 RID: 1969 RVA: 0x0002B5F5 File Offset: 0x000297F5
	// (set) Token: 0x060007B2 RID: 1970 RVA: 0x0002B600 File Offset: 0x00029800
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

	// Token: 0x170000BC RID: 188
	// (get) Token: 0x060007B3 RID: 1971 RVA: 0x0002B622 File Offset: 0x00029822
	// (set) Token: 0x060007B4 RID: 1972 RVA: 0x0002B62C File Offset: 0x0002982C
	public bool CanVote
	{
		get
		{
			return this._canVote;
		}
		set
		{
			Collider trigger = this._trigger;
			this._canVote = value;
			trigger.enabled = value;
		}
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x0002B64E File Offset: 0x0002984E
	private void Reset()
	{
		this.Configure();
	}

	// Token: 0x060007B6 RID: 1974 RVA: 0x0002B658 File Offset: 0x00029858
	private void Configure()
	{
		foreach (Collider collider in base.GetComponentsInChildren<Collider>())
		{
			if (collider.isTrigger)
			{
				this._trigger = collider;
				break;
			}
		}
		if (!this._optionText)
		{
			this._optionText = base.GetComponentInChildren<TMP_Text>();
		}
	}

	// Token: 0x060007B7 RID: 1975 RVA: 0x0002B6A8 File Offset: 0x000298A8
	private void OnTriggerEnter(Collider other)
	{
		if (!this.IsValidVotingRock(other))
		{
			return;
		}
		Action<MonkeVoteOption, Collider> onVote = this.OnVote;
		if (onVote == null)
		{
			return;
		}
		onVote(this, other);
	}

	// Token: 0x060007B8 RID: 1976 RVA: 0x0002B6C8 File Offset: 0x000298C8
	private bool IsValidVotingRock(Collider other)
	{
		SlingshotProjectile component = other.GetComponent<SlingshotProjectile>();
		return component && component.projectileOwner.IsLocal;
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x0002B6F1 File Offset: 0x000298F1
	public void ResetState()
	{
		this.OnVote = null;
		this.ShowIndicators(false, false, true);
	}

	// Token: 0x060007BA RID: 1978 RVA: 0x0002B703 File Offset: 0x00029903
	public void ShowIndicators(bool showVote, bool showPrediction, bool instant = true)
	{
		this._voteIndicator.SetVisible(showVote, instant);
		this._guessIndicator.SetVisible(showPrediction, instant);
	}

	// Token: 0x060007BB RID: 1979 RVA: 0x0002B71F File Offset: 0x0002991F
	private void Vote()
	{
		this.SendVote(null);
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x0002B728 File Offset: 0x00029928
	private void SendVote(Collider other)
	{
		if (!this._canVote)
		{
			return;
		}
		Action<MonkeVoteOption, Collider> onVote = this.OnVote;
		if (onVote == null)
		{
			return;
		}
		onVote(this, other);
	}

	// Token: 0x060007BD RID: 1981 RVA: 0x0002B745 File Offset: 0x00029945
	public void SetDynamicMeshesVisible(bool visible)
	{
		this._voteIndicator.SetVisible(visible, true);
		this._guessIndicator.SetVisible(visible, true);
	}

	// Token: 0x0400093C RID: 2364
	[SerializeField]
	private Collider _trigger;

	// Token: 0x0400093D RID: 2365
	[SerializeField]
	private TMP_Text _optionText;

	// Token: 0x0400093E RID: 2366
	[SerializeField]
	private VotingCard _voteIndicator;

	// Token: 0x0400093F RID: 2367
	[FormerlySerializedAs("_predictionIndicator")]
	[SerializeField]
	private VotingCard _guessIndicator;

	// Token: 0x04000941 RID: 2369
	private string _text = string.Empty;

	// Token: 0x04000942 RID: 2370
	private bool _canVote;
}
