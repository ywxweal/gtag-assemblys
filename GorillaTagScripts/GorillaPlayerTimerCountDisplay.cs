using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000B15 RID: 2837
	public class GorillaPlayerTimerCountDisplay : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x060045C1 RID: 17857 RVA: 0x0014BA14 File Offset: 0x00149C14
		private void Start()
		{
			this.TryInit();
		}

		// Token: 0x060045C2 RID: 17858 RVA: 0x0014BA14 File Offset: 0x00149C14
		private void OnEnable()
		{
			this.TryInit();
		}

		// Token: 0x060045C3 RID: 17859 RVA: 0x0014BA1C File Offset: 0x00149C1C
		private void TryInit()
		{
			if (this.isInitialized)
			{
				return;
			}
			if (PlayerTimerManager.instance == null)
			{
				return;
			}
			PlayerTimerManager.instance.OnTimerStopped.AddListener(new UnityAction<int, int>(this.OnTimerStopped));
			PlayerTimerManager.instance.OnLocalTimerStarted.AddListener(new UnityAction(this.OnLocalTimerStarted));
			this.displayText.text = "TIME: --.--.-";
			if (PlayerTimerManager.instance.IsLocalTimerStarted() && !this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
			this.isInitialized = true;
		}

		// Token: 0x060045C4 RID: 17860 RVA: 0x0014BAA8 File Offset: 0x00149CA8
		private void OnDisable()
		{
			if (PlayerTimerManager.instance != null)
			{
				PlayerTimerManager.instance.OnTimerStopped.RemoveListener(new UnityAction<int, int>(this.OnTimerStopped));
				PlayerTimerManager.instance.OnLocalTimerStarted.RemoveListener(new UnityAction(this.OnLocalTimerStarted));
			}
			this.isInitialized = false;
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
		}

		// Token: 0x060045C5 RID: 17861 RVA: 0x0014BB0D File Offset: 0x00149D0D
		private void OnLocalTimerStarted()
		{
			if (!this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x060045C6 RID: 17862 RVA: 0x0014BB20 File Offset: 0x00149D20
		private void OnTimerStopped(int actorNum, int timeDelta)
		{
			if (actorNum == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				double num = timeDelta / 1000.0;
				this.displayText.text = "TIME: " + TimeSpan.FromSeconds(num).ToString("mm\\:ss\\:f");
				if (this.TickRunning)
				{
					TickSystem<object>.RemoveTickCallback(this);
				}
			}
		}

		// Token: 0x060045C7 RID: 17863 RVA: 0x0014BB84 File Offset: 0x00149D84
		private void UpdateLatestTime()
		{
			float timeForPlayer = PlayerTimerManager.instance.GetTimeForPlayer(NetworkSystem.Instance.LocalPlayer.ActorNumber);
			this.displayText.text = "TIME: " + TimeSpan.FromSeconds((double)timeForPlayer).ToString("mm\\:ss\\:f");
		}

		// Token: 0x170006CD RID: 1741
		// (get) Token: 0x060045C8 RID: 17864 RVA: 0x0014BBD4 File Offset: 0x00149DD4
		// (set) Token: 0x060045C9 RID: 17865 RVA: 0x0014BBDC File Offset: 0x00149DDC
		public bool TickRunning { get; set; }

		// Token: 0x060045CA RID: 17866 RVA: 0x0014BBE5 File Offset: 0x00149DE5
		public void Tick()
		{
			this.UpdateLatestTime();
		}

		// Token: 0x0400485C RID: 18524
		[SerializeField]
		private TMP_Text displayText;

		// Token: 0x0400485D RID: 18525
		private bool isInitialized;
	}
}
