using System;
using System.Collections.Generic;
using UnityEngine;

namespace NetSynchrony
{
	// Token: 0x02000C9B RID: 3227
	[CreateAssetMenu(fileName = "RandomDispatcher", menuName = "NetSynchrony/RandomDispatcher", order = 0)]
	public class RandomDispatcher : ScriptableObject
	{
		// Token: 0x14000094 RID: 148
		// (add) Token: 0x06005008 RID: 20488 RVA: 0x0017D5C0 File Offset: 0x0017B7C0
		// (remove) Token: 0x06005009 RID: 20489 RVA: 0x0017D5F8 File Offset: 0x0017B7F8
		public event RandomDispatcher.RandomDispatcherEvent Dispatch;

		// Token: 0x0600500A RID: 20490 RVA: 0x0017D630 File Offset: 0x0017B830
		public void Init(double seconds)
		{
			seconds %= (double)(this.totalMinutes * 60f);
			this.index = 0;
			this.dispatchTimes = new List<float>();
			float num = 0f;
			float num2 = this.totalMinutes * 60f;
			Random.InitState(StaticHash.Compute(Application.buildGUID));
			while (num < num2)
			{
				float num3 = Random.Range(this.minWaitTime, this.maxWaitTime);
				num += num3;
				if ((double)num < seconds)
				{
					this.index = this.dispatchTimes.Count;
				}
				this.dispatchTimes.Add(num);
			}
			Random.InitState((int)DateTime.Now.Ticks);
		}

		// Token: 0x0600500B RID: 20491 RVA: 0x0017D6D4 File Offset: 0x0017B8D4
		public void Sync(double seconds)
		{
			seconds %= (double)(this.totalMinutes * 60f);
			this.index = 0;
			for (int i = 0; i < this.dispatchTimes.Count; i++)
			{
				if ((double)this.dispatchTimes[i] < seconds)
				{
					this.index = i;
				}
			}
		}

		// Token: 0x0600500C RID: 20492 RVA: 0x0017D728 File Offset: 0x0017B928
		public void Tick(double seconds)
		{
			seconds %= (double)(this.totalMinutes * 60f);
			if ((double)this.dispatchTimes[this.index] < seconds)
			{
				this.index = (this.index + 1) % this.dispatchTimes.Count;
				if (this.Dispatch != null)
				{
					this.Dispatch(this);
				}
			}
		}

		// Token: 0x04005303 RID: 21251
		[SerializeField]
		private float minWaitTime = 1f;

		// Token: 0x04005304 RID: 21252
		[SerializeField]
		private float maxWaitTime = 10f;

		// Token: 0x04005305 RID: 21253
		[SerializeField]
		private float totalMinutes = 60f;

		// Token: 0x04005306 RID: 21254
		private List<float> dispatchTimes;

		// Token: 0x04005307 RID: 21255
		private int index = -1;

		// Token: 0x02000C9C RID: 3228
		// (Invoke) Token: 0x0600500F RID: 20495
		public delegate void RandomDispatcherEvent(RandomDispatcher randomDispatcher);
	}
}
