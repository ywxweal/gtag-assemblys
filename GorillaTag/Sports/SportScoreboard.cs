using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.CodeGen;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x02000D46 RID: 3398
	[RequireComponent(typeof(AudioSource))]
	[NetworkBehaviourWeaved(2)]
	public class SportScoreboard : NetworkComponent
	{
		// Token: 0x0600550C RID: 21772 RVA: 0x0019E184 File Offset: 0x0019C384
		protected override void Awake()
		{
			base.Awake();
			SportScoreboard.Instance = this;
			this.audioSource = base.GetComponent<AudioSource>();
			this.scoreVisuals = new SportScoreboardVisuals[this.teamParameters.Count];
			for (int i = 0; i < this.teamParameters.Count; i++)
			{
				this.teamScores.Add(0);
				this.teamScoresPrev.Add(0);
			}
		}

		// Token: 0x0600550D RID: 21773 RVA: 0x0019E1ED File Offset: 0x0019C3ED
		public void RegisterTeamVisual(int TeamIndex, SportScoreboardVisuals visuals)
		{
			this.scoreVisuals[TeamIndex] = visuals;
			this.UpdateScoreboard();
		}

		// Token: 0x0600550E RID: 21774 RVA: 0x0019E200 File Offset: 0x0019C400
		private void UpdateScoreboard()
		{
			for (int i = 0; i < this.teamParameters.Count; i++)
			{
				if (!(this.scoreVisuals[i] == null))
				{
					int num = this.teamScores[i];
					if (this.scoreVisuals[i].score1s != null)
					{
						this.scoreVisuals[i].score1s.SetUVOffset(num % 10);
					}
					if (this.scoreVisuals[i].score10s != null)
					{
						this.scoreVisuals[i].score10s.SetUVOffset(num / 10 % 10);
					}
				}
			}
		}

		// Token: 0x0600550F RID: 21775 RVA: 0x0019E29C File Offset: 0x0019C49C
		private void OnScoreUpdated()
		{
			for (int i = 0; i < this.teamScores.Count; i++)
			{
				if (this.teamScores[i] > this.teamScoresPrev[i] && this.teamParameters[i].goalScoredAudio != null && this.teamScores[i] < this.matchEndScore)
				{
					this.audioSource.GTPlayOneShot(this.teamParameters[i].goalScoredAudio, 1f);
				}
				this.teamScoresPrev[i] = this.teamScores[i];
			}
			if (!this.runningMatchEndCoroutine)
			{
				for (int j = 0; j < this.teamScores.Count; j++)
				{
					if (this.teamScores[j] >= this.matchEndScore)
					{
						base.StartCoroutine(this.MatchEndCoroutine(j));
						break;
					}
				}
			}
			this.UpdateScoreboard();
		}

		// Token: 0x06005510 RID: 21776 RVA: 0x0019E390 File Offset: 0x0019C590
		public void TeamScored(int team)
		{
			if (base.IsMine && !this.runningMatchEndCoroutine)
			{
				if (team >= 0 && team < this.teamScores.Count)
				{
					this.teamScores[team] = this.teamScores[team] + 1;
				}
				this.OnScoreUpdated();
			}
		}

		// Token: 0x06005511 RID: 21777 RVA: 0x0019E3E0 File Offset: 0x0019C5E0
		public void ResetScores()
		{
			if (base.IsMine && !this.runningMatchEndCoroutine)
			{
				for (int i = 0; i < this.teamScores.Count; i++)
				{
					this.teamScores[i] = 0;
				}
				this.OnScoreUpdated();
			}
		}

		// Token: 0x06005512 RID: 21778 RVA: 0x0019E426 File Offset: 0x0019C626
		private IEnumerator MatchEndCoroutine(int winningTeam)
		{
			this.runningMatchEndCoroutine = true;
			if (winningTeam >= 0 && winningTeam < this.teamParameters.Count && this.teamParameters[winningTeam].matchWonAudio != null)
			{
				this.audioSource.GTPlayOneShot(this.teamParameters[winningTeam].matchWonAudio, 1f);
			}
			yield return new WaitForSeconds(this.matchEndScoreResetDelayTime);
			this.runningMatchEndCoroutine = false;
			this.ResetScores();
			yield break;
		}

		// Token: 0x1700087F RID: 2175
		// (get) Token: 0x06005513 RID: 21779 RVA: 0x0019E43C File Offset: 0x0019C63C
		[Networked]
		[Capacity(2)]
		[NetworkedWeaved(0, 2)]
		public unsafe NetworkArray<int> Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing SportScoreboard.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return new NetworkArray<int>((byte*)(this.Ptr + 0), 2, ReaderWriter@System_Int32.GetInstance());
			}
		}

		// Token: 0x06005514 RID: 21780 RVA: 0x0019E478 File Offset: 0x0019C678
		public override void WriteDataFusion()
		{
			this.Data.CopyFrom(this.teamScores, 0, this.teamScores.Count);
		}

		// Token: 0x06005515 RID: 21781 RVA: 0x0019E4A8 File Offset: 0x0019C6A8
		public override void ReadDataFusion()
		{
			this.teamScores.Clear();
			this.Data.CopyTo(this.teamScores);
			this.OnScoreUpdated();
		}

		// Token: 0x06005516 RID: 21782 RVA: 0x0019E4DC File Offset: 0x0019C6DC
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			for (int i = 0; i < this.teamScores.Count; i++)
			{
				stream.SendNext(this.teamScores[i]);
			}
		}

		// Token: 0x06005517 RID: 21783 RVA: 0x0019E518 File Offset: 0x0019C718
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			for (int i = 0; i < this.teamScores.Count; i++)
			{
				this.teamScores[i] = (int)stream.ReceiveNext();
			}
			this.OnScoreUpdated();
		}

		// Token: 0x06005519 RID: 21785 RVA: 0x0019E593 File Offset: 0x0019C793
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			NetworkBehaviourUtils.InitializeNetworkArray<int>(this.Data, this._Data, "Data");
		}

		// Token: 0x0600551A RID: 21786 RVA: 0x0019E5B5 File Offset: 0x0019C7B5
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			NetworkBehaviourUtils.CopyFromNetworkArray<int>(this.Data, ref this._Data);
		}

		// Token: 0x04005853 RID: 22611
		[OnEnterPlay_SetNull]
		public static SportScoreboard Instance;

		// Token: 0x04005854 RID: 22612
		[SerializeField]
		private List<SportScoreboard.TeamParameters> teamParameters = new List<SportScoreboard.TeamParameters>();

		// Token: 0x04005855 RID: 22613
		[SerializeField]
		private int matchEndScore = 3;

		// Token: 0x04005856 RID: 22614
		[SerializeField]
		private float matchEndScoreResetDelayTime = 3f;

		// Token: 0x04005857 RID: 22615
		private List<int> teamScores = new List<int>();

		// Token: 0x04005858 RID: 22616
		private List<int> teamScoresPrev = new List<int>();

		// Token: 0x04005859 RID: 22617
		private bool runningMatchEndCoroutine;

		// Token: 0x0400585A RID: 22618
		private AudioSource audioSource;

		// Token: 0x0400585B RID: 22619
		private SportScoreboardVisuals[] scoreVisuals;

		// Token: 0x0400585C RID: 22620
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 2)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private int[] _Data;

		// Token: 0x02000D47 RID: 3399
		[Serializable]
		private class TeamParameters
		{
			// Token: 0x0400585D RID: 22621
			[SerializeField]
			public AudioClip matchWonAudio;

			// Token: 0x0400585E RID: 22622
			[SerializeField]
			public AudioClip goalScoredAudio;
		}
	}
}
