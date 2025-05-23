using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000B16 RID: 2838
	public class GorillaTimer : MonoBehaviourPun
	{
		// Token: 0x060045CC RID: 17868 RVA: 0x0014BBED File Offset: 0x00149DED
		private void Awake()
		{
			this.ResetTimer();
		}

		// Token: 0x060045CD RID: 17869 RVA: 0x0014BBF5 File Offset: 0x00149DF5
		public void StartTimer()
		{
			this.startTimer = true;
			UnityEvent<GorillaTimer> unityEvent = this.onTimerStarted;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}

		// Token: 0x060045CE RID: 17870 RVA: 0x0014BC0F File Offset: 0x00149E0F
		public IEnumerator DelayedReStartTimer(float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			this.RestartTimer();
			yield break;
		}

		// Token: 0x060045CF RID: 17871 RVA: 0x0014BC25 File Offset: 0x00149E25
		private void StopTimer()
		{
			this.startTimer = false;
			UnityEvent<GorillaTimer> unityEvent = this.onTimerStopped;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}

		// Token: 0x060045D0 RID: 17872 RVA: 0x0014BC3F File Offset: 0x00149E3F
		private void ResetTimer()
		{
			this.passedTime = 0f;
		}

		// Token: 0x060045D1 RID: 17873 RVA: 0x0014BC4C File Offset: 0x00149E4C
		public void RestartTimer()
		{
			if (this.useRandomDuration)
			{
				this.SetTimerDuration(Random.Range(this.randTimeMin, this.randTimeMax));
			}
			this.ResetTimer();
			this.StartTimer();
		}

		// Token: 0x060045D2 RID: 17874 RVA: 0x0014BC79 File Offset: 0x00149E79
		public void SetTimerDuration(float timer)
		{
			this.timerDuration = timer;
		}

		// Token: 0x060045D3 RID: 17875 RVA: 0x0014BC82 File Offset: 0x00149E82
		public void InvokeUpdate()
		{
			if (this.startTimer)
			{
				this.passedTime += Time.deltaTime;
			}
			if (this.startTimer && this.passedTime >= this.timerDuration)
			{
				this.StopTimer();
				this.ResetTimer();
			}
		}

		// Token: 0x060045D4 RID: 17876 RVA: 0x0014BCC0 File Offset: 0x00149EC0
		public float GetPassedTime()
		{
			return this.passedTime;
		}

		// Token: 0x060045D5 RID: 17877 RVA: 0x0014BCC8 File Offset: 0x00149EC8
		public void SetPassedTime(float time)
		{
			this.passedTime = time;
		}

		// Token: 0x060045D6 RID: 17878 RVA: 0x0014BCD1 File Offset: 0x00149ED1
		public float GetRemainingTime()
		{
			return this.timerDuration - this.passedTime;
		}

		// Token: 0x060045D7 RID: 17879 RVA: 0x0014BCE0 File Offset: 0x00149EE0
		public void OnEnable()
		{
			GorillaTimerManager.RegisterGorillaTimer(this);
		}

		// Token: 0x060045D8 RID: 17880 RVA: 0x0014BCE8 File Offset: 0x00149EE8
		public void OnDisable()
		{
			GorillaTimerManager.UnregisterGorillaTimer(this);
		}

		// Token: 0x0400485F RID: 18527
		[SerializeField]
		private float timerDuration;

		// Token: 0x04004860 RID: 18528
		[SerializeField]
		private bool useRandomDuration;

		// Token: 0x04004861 RID: 18529
		[SerializeField]
		private float randTimeMin;

		// Token: 0x04004862 RID: 18530
		[SerializeField]
		private float randTimeMax;

		// Token: 0x04004863 RID: 18531
		private float passedTime;

		// Token: 0x04004864 RID: 18532
		private bool startTimer;

		// Token: 0x04004865 RID: 18533
		private bool resetTimer;

		// Token: 0x04004866 RID: 18534
		public UnityEvent<GorillaTimer> onTimerStarted;

		// Token: 0x04004867 RID: 18535
		public UnityEvent<GorillaTimer> onTimerStopped;
	}
}
