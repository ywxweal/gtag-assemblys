using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B62 RID: 2914
	public class BuilderPieceTimer : MonoBehaviour, IBuilderPieceComponent, ITickSystemTick
	{
		// Token: 0x06004808 RID: 18440 RVA: 0x001577E5 File Offset: 0x001559E5
		private void Awake()
		{
			this.buttonTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnButtonPressed));
		}

		// Token: 0x06004809 RID: 18441 RVA: 0x00157803 File Offset: 0x00155A03
		private void OnDestroy()
		{
			if (this.buttonTrigger != null)
			{
				this.buttonTrigger.TriggeredEvent.RemoveListener(new UnityAction(this.OnButtonPressed));
			}
		}

		// Token: 0x0600480A RID: 18442 RVA: 0x00157830 File Offset: 0x00155A30
		private void OnButtonPressed()
		{
			if (this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			if (Time.time > this.lastTriggeredTime + this.debounceTime)
			{
				this.lastTriggeredTime = Time.time;
				if (!this.isStart && this.stopSoundBank != null)
				{
					this.stopSoundBank.Play();
				}
				else if (this.activateSoundBank != null)
				{
					this.activateSoundBank.Play();
				}
				if (this.isBoth && this.isStart && this.displayText != null)
				{
					this.displayText.text = "TIME: 00:00:0";
				}
				PlayerTimerManager.instance.RequestTimerToggle(this.isStart);
			}
		}

		// Token: 0x0600480B RID: 18443 RVA: 0x001578E8 File Offset: 0x00155AE8
		private void OnTimerStopped(int actorNum, int timeDelta)
		{
			if (this.isStart && !this.isBoth)
			{
				return;
			}
			double num = timeDelta;
			this.latestTime = num / 1000.0;
			if (this.latestTime > 3599.989990234375)
			{
				this.latestTime = 3599.989990234375;
			}
			this.displayText.text = "TIME: " + TimeSpan.FromSeconds(this.latestTime).ToString("mm\\:ss\\:ff");
			if (this.isBoth && actorNum == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.isStart = true;
				if (this.TickRunning)
				{
					TickSystem<object>.RemoveTickCallback(this);
				}
			}
		}

		// Token: 0x0600480C RID: 18444 RVA: 0x00157997 File Offset: 0x00155B97
		private void OnLocalTimerStarted()
		{
			if (this.isBoth)
			{
				this.isStart = false;
			}
			if (this.myPiece.state == BuilderPiece.State.AttachedAndPlaced && !this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x0600480D RID: 18445 RVA: 0x001579C4 File Offset: 0x00155BC4
		private void OnZoneChanged()
		{
			bool flag = ZoneManagement.instance.IsZoneActive(this.myPiece.GetTable().tableZone);
			if (this.displayText != null)
			{
				this.displayText.gameObject.SetActive(flag);
			}
		}

		// Token: 0x0600480E RID: 18446 RVA: 0x00157A0C File Offset: 0x00155C0C
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.latestTime = double.MaxValue;
			if (this.displayText != null)
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
				this.OnZoneChanged();
				this.displayText.text = "TIME: __:__:_";
			}
		}

		// Token: 0x0600480F RID: 18447 RVA: 0x00157A72 File Offset: 0x00155C72
		public void OnPieceDestroy()
		{
			if (this.displayText != null)
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
			}
		}

		// Token: 0x06004810 RID: 18448 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06004811 RID: 18449 RVA: 0x00157AA8 File Offset: 0x00155CA8
		public void OnPieceActivate()
		{
			this.lastTriggeredTime = 0f;
			PlayerTimerManager.instance.OnTimerStopped.AddListener(new UnityAction<int, int>(this.OnTimerStopped));
			PlayerTimerManager.instance.OnLocalTimerStarted.AddListener(new UnityAction(this.OnLocalTimerStarted));
			if (this.isBoth)
			{
				this.isStart = !PlayerTimerManager.instance.IsLocalTimerStarted();
				if (!this.isStart && this.displayText != null)
				{
					this.displayText.text = "TIME: __:__:_";
				}
			}
			if (PlayerTimerManager.instance.IsLocalTimerStarted() && !this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06004812 RID: 18450 RVA: 0x00157B54 File Offset: 0x00155D54
		public void OnPieceDeactivate()
		{
			if (PlayerTimerManager.instance != null)
			{
				PlayerTimerManager.instance.OnTimerStopped.RemoveListener(new UnityAction<int, int>(this.OnTimerStopped));
				PlayerTimerManager.instance.OnLocalTimerStarted.RemoveListener(new UnityAction(this.OnLocalTimerStarted));
			}
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
			if (this.displayText != null)
			{
				this.displayText.text = "TIME: --:--:-";
			}
		}

		// Token: 0x170006EE RID: 1774
		// (get) Token: 0x06004813 RID: 18451 RVA: 0x00157BD0 File Offset: 0x00155DD0
		// (set) Token: 0x06004814 RID: 18452 RVA: 0x00157BD8 File Offset: 0x00155DD8
		public bool TickRunning { get; set; }

		// Token: 0x06004815 RID: 18453 RVA: 0x00157BE4 File Offset: 0x00155DE4
		public void Tick()
		{
			if (this.displayText != null)
			{
				float num = PlayerTimerManager.instance.GetTimeForPlayer(NetworkSystem.Instance.LocalPlayer.ActorNumber);
				num = Mathf.Clamp(num, 0f, 3599.99f);
				this.displayText.text = "TIME: " + TimeSpan.FromSeconds((double)num).ToString("mm\\:ss\\:f");
			}
		}

		// Token: 0x04004A83 RID: 19075
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x04004A84 RID: 19076
		[SerializeField]
		private bool isStart;

		// Token: 0x04004A85 RID: 19077
		[SerializeField]
		private bool isBoth;

		// Token: 0x04004A86 RID: 19078
		[SerializeField]
		private BuilderSmallHandTrigger buttonTrigger;

		// Token: 0x04004A87 RID: 19079
		[SerializeField]
		private SoundBankPlayer activateSoundBank;

		// Token: 0x04004A88 RID: 19080
		[SerializeField]
		private SoundBankPlayer stopSoundBank;

		// Token: 0x04004A89 RID: 19081
		[SerializeField]
		private float debounceTime = 0.5f;

		// Token: 0x04004A8A RID: 19082
		private float lastTriggeredTime;

		// Token: 0x04004A8B RID: 19083
		private double latestTime = 3.4028234663852886E+38;

		// Token: 0x04004A8C RID: 19084
		[SerializeField]
		private TMP_Text displayText;
	}
}
