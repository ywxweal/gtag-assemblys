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
		// Token: 0x060045CD RID: 17869 RVA: 0x0014BCC5 File Offset: 0x00149EC5
		private void Awake()
		{
			this.ResetTimer();
		}

		// Token: 0x060045CE RID: 17870 RVA: 0x0014BCCD File Offset: 0x00149ECD
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

		// Token: 0x060045CF RID: 17871 RVA: 0x0014BCE7 File Offset: 0x00149EE7
		public IEnumerator DelayedReStartTimer(float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			this.RestartTimer();
			yield break;
		}

		// Token: 0x060045D0 RID: 17872 RVA: 0x0014BCFD File Offset: 0x00149EFD
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

		// Token: 0x060045D1 RID: 17873 RVA: 0x0014BD17 File Offset: 0x00149F17
		private void ResetTimer()
		{
			this.passedTime = 0f;
		}

		// Token: 0x060045D2 RID: 17874 RVA: 0x0014BD24 File Offset: 0x00149F24
		public void RestartTimer()
		{
			if (this.useRandomDuration)
			{
				this.SetTimerDuration(Random.Range(this.randTimeMin, this.randTimeMax));
			}
			this.ResetTimer();
			this.StartTimer();
		}

		// Token: 0x060045D3 RID: 17875 RVA: 0x0014BD51 File Offset: 0x00149F51
		public void SetTimerDuration(float timer)
		{
			this.timerDuration = timer;
		}

		// Token: 0x060045D4 RID: 17876 RVA: 0x0014BD5A File Offset: 0x00149F5A
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

		// Token: 0x060045D5 RID: 17877 RVA: 0x0014BD98 File Offset: 0x00149F98
		public float GetPassedTime()
		{
			return this.passedTime;
		}

		// Token: 0x060045D6 RID: 17878 RVA: 0x0014BDA0 File Offset: 0x00149FA0
		public void SetPassedTime(float time)
		{
			this.passedTime = time;
		}

		// Token: 0x060045D7 RID: 17879 RVA: 0x0014BDA9 File Offset: 0x00149FA9
		public float GetRemainingTime()
		{
			return this.timerDuration - this.passedTime;
		}

		// Token: 0x060045D8 RID: 17880 RVA: 0x0014BDB8 File Offset: 0x00149FB8
		public void OnEnable()
		{
			GorillaTimerManager.RegisterGorillaTimer(this);
		}

		// Token: 0x060045D9 RID: 17881 RVA: 0x0014BDC0 File Offset: 0x00149FC0
		public void OnDisable()
		{
			GorillaTimerManager.UnregisterGorillaTimer(this);
		}

		// Token: 0x04004860 RID: 18528
		[SerializeField]
		private float timerDuration;

		// Token: 0x04004861 RID: 18529
		[SerializeField]
		private bool useRandomDuration;

		// Token: 0x04004862 RID: 18530
		[SerializeField]
		private float randTimeMin;

		// Token: 0x04004863 RID: 18531
		[SerializeField]
		private float randTimeMax;

		// Token: 0x04004864 RID: 18532
		private float passedTime;

		// Token: 0x04004865 RID: 18533
		private bool startTimer;

		// Token: 0x04004866 RID: 18534
		private bool resetTimer;

		// Token: 0x04004867 RID: 18535
		public UnityEvent<GorillaTimer> onTimerStarted;

		// Token: 0x04004868 RID: 18536
		public UnityEvent<GorillaTimer> onTimerStopped;
	}
}
