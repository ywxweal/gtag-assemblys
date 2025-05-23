using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001A1 RID: 417
public class TriggerOnSpeed : MonoBehaviour, ITickSystemTick
{
	// Token: 0x06000A50 RID: 2640 RVA: 0x0001C94F File Offset: 0x0001AB4F
	private void OnEnable()
	{
		TickSystem<object>.AddCallbackTarget(this);
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x0001C957 File Offset: 0x0001AB57
	private void OnDisable()
	{
		TickSystem<object>.RemoveCallbackTarget(this);
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x000360F4 File Offset: 0x000342F4
	public void Tick()
	{
		bool flag = this.velocityEstimator.linearVelocity.IsLongerThan(this.speedThreshold);
		if (flag != this.wasFaster)
		{
			if (flag)
			{
				this.onFaster.Invoke();
			}
			else
			{
				this.onSlower.Invoke();
			}
			this.wasFaster = flag;
		}
	}

	// Token: 0x17000105 RID: 261
	// (get) Token: 0x06000A53 RID: 2643 RVA: 0x00036143 File Offset: 0x00034343
	// (set) Token: 0x06000A54 RID: 2644 RVA: 0x0003614B File Offset: 0x0003434B
	public bool TickRunning { get; set; }

	// Token: 0x04000C7B RID: 3195
	[SerializeField]
	private float speedThreshold;

	// Token: 0x04000C7C RID: 3196
	[SerializeField]
	private UnityEvent onFaster;

	// Token: 0x04000C7D RID: 3197
	[SerializeField]
	private UnityEvent onSlower;

	// Token: 0x04000C7E RID: 3198
	[SerializeField]
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04000C7F RID: 3199
	private bool wasFaster;
}
