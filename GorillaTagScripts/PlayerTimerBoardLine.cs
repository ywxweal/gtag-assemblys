using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000B21 RID: 2849
	public class PlayerTimerBoardLine : MonoBehaviour
	{
		// Token: 0x0600462B RID: 17963 RVA: 0x0014D4C3 File Offset: 0x0014B6C3
		public void ResetData()
		{
			this.linePlayer = null;
			this.currentNickname = string.Empty;
			this.playerTimeStr = string.Empty;
			this.playerTimeSeconds = 0f;
		}

		// Token: 0x0600462C RID: 17964 RVA: 0x0014D4F0 File Offset: 0x0014B6F0
		public void SetLineData(NetPlayer netPlayer)
		{
			if (!netPlayer.InRoom || netPlayer == this.linePlayer)
			{
				return;
			}
			this.linePlayer = netPlayer;
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
			{
				this.rigContainer = rigContainer;
				this.playerVRRig = rigContainer.Rig;
			}
			this.InitializeLine();
		}

		// Token: 0x0600462D RID: 17965 RVA: 0x0014D53E File Offset: 0x0014B73E
		public void InitializeLine()
		{
			this.currentNickname = string.Empty;
			this.UpdatePlayerText();
			this.UpdateTimeText();
		}

		// Token: 0x0600462E RID: 17966 RVA: 0x0014D558 File Offset: 0x0014B758
		public void UpdateLine()
		{
			if (this.linePlayer != null)
			{
				if (this.playerNameVisible != this.playerVRRig.playerNameVisible)
				{
					this.UpdatePlayerText();
					this.parentBoard.IsDirty = true;
				}
				string text = this.playerTimeStr;
				this.UpdateTimeText();
				if (!this.playerTimeStr.Equals(text))
				{
					this.parentBoard.IsDirty = true;
				}
			}
		}

		// Token: 0x0600462F RID: 17967 RVA: 0x0014D5C0 File Offset: 0x0014B7C0
		private void UpdatePlayerText()
		{
			try
			{
				if (this.rigContainer.IsNull() || this.playerVRRig.IsNull())
				{
					this.playerNameVisible = this.NormalizeName(this.linePlayer.NickName != this.currentNickname, this.linePlayer.NickName);
					this.currentNickname = this.linePlayer.NickName;
				}
				else if (this.rigContainer.Initialized)
				{
					this.playerNameVisible = this.playerVRRig.playerNameVisible;
				}
				else if (this.currentNickname.IsNullOrEmpty() || GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(this.linePlayer.UserId))
				{
					this.playerNameVisible = this.NormalizeName(this.linePlayer.NickName != this.currentNickname, this.linePlayer.NickName);
				}
			}
			catch (Exception)
			{
				this.playerNameVisible = this.linePlayer.DefaultName;
				GorillaNot.instance.SendReport("NmError", this.linePlayer.UserId, this.linePlayer.NickName);
			}
		}

		// Token: 0x06004630 RID: 17968 RVA: 0x0014D6F4 File Offset: 0x0014B8F4
		private void UpdateTimeText()
		{
			if (this.linePlayer == null || !(PlayerTimerManager.instance != null))
			{
				this.playerTimeStr = "--:--:--";
				return;
			}
			this.playerTimeSeconds = PlayerTimerManager.instance.GetLastDurationForPlayer(this.linePlayer.ActorNumber);
			if (this.playerTimeSeconds > 0f)
			{
				this.playerTimeStr = TimeSpan.FromSeconds((double)this.playerTimeSeconds).ToString("mm\\:ss\\:ff");
				return;
			}
			this.playerTimeStr = "--:--:--";
		}

		// Token: 0x06004631 RID: 17969 RVA: 0x0014D778 File Offset: 0x0014B978
		public string NormalizeName(bool doIt, string text)
		{
			if (doIt)
			{
				if (GorillaComputer.instance.CheckAutoBanListForName(text))
				{
					text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => Utils.IsASCIILetterOrDigit(c)));
					if (text.Length > 12)
					{
						text = text.Substring(0, 11);
					}
					text = text.ToUpper();
				}
				else
				{
					text = "BADGORILLA";
					GorillaNot.instance.SendReport("evading the name ban", this.linePlayer.UserId, this.linePlayer.NickName);
				}
			}
			return text;
		}

		// Token: 0x06004632 RID: 17970 RVA: 0x0014D81C File Offset: 0x0014BA1C
		public static int CompareByTotalTime(PlayerTimerBoardLine lineA, PlayerTimerBoardLine lineB)
		{
			if (lineA.playerTimeSeconds > 0f && lineB.playerTimeSeconds > 0f)
			{
				return lineA.playerTimeSeconds.CompareTo(lineB.playerTimeSeconds);
			}
			if (lineA.playerTimeSeconds <= 0f)
			{
				return 1;
			}
			if (lineB.playerTimeSeconds <= 0f)
			{
				return -1;
			}
			return 0;
		}

		// Token: 0x040048CA RID: 18634
		public string playerNameVisible;

		// Token: 0x040048CB RID: 18635
		public string playerTimeStr;

		// Token: 0x040048CC RID: 18636
		private float playerTimeSeconds;

		// Token: 0x040048CD RID: 18637
		public NetPlayer linePlayer;

		// Token: 0x040048CE RID: 18638
		public VRRig playerVRRig;

		// Token: 0x040048CF RID: 18639
		public PlayerTimerBoard parentBoard;

		// Token: 0x040048D0 RID: 18640
		internal RigContainer rigContainer;

		// Token: 0x040048D1 RID: 18641
		private string currentNickname;
	}
}
