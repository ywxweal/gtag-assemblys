using System;
using GorillaNetworking;
using UnityEngine;

namespace NetSynchrony
{
	// Token: 0x02000C9D RID: 3229
	public class RandomDispatcherManager : MonoBehaviour
	{
		// Token: 0x06005011 RID: 20497 RVA: 0x0017D6E4 File Offset: 0x0017B8E4
		private void OnDisable()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (GorillaComputer.instance != null)
			{
				GorillaComputer instance = GorillaComputer.instance;
				instance.OnServerTimeUpdated = (Action)Delegate.Remove(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
			}
		}

		// Token: 0x06005012 RID: 20498 RVA: 0x0017D730 File Offset: 0x0017B930
		private void OnTimeChanged()
		{
			this.AdjustedServerTime();
			for (int i = 0; i < this.randomDispatchers.Length; i++)
			{
				this.randomDispatchers[i].Sync(this.serverTime);
			}
		}

		// Token: 0x06005013 RID: 20499 RVA: 0x0017D76C File Offset: 0x0017B96C
		private void AdjustedServerTime()
		{
			DateTime dateTime = new DateTime(2020, 1, 1);
			long num = GorillaComputer.instance.GetServerTime().Ticks - dateTime.Ticks;
			this.serverTime = (double)((float)num / 10000000f);
		}

		// Token: 0x06005014 RID: 20500 RVA: 0x0017D7B4 File Offset: 0x0017B9B4
		private void Start()
		{
			GorillaComputer instance = GorillaComputer.instance;
			instance.OnServerTimeUpdated = (Action)Delegate.Combine(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
			for (int i = 0; i < this.randomDispatchers.Length; i++)
			{
				this.randomDispatchers[i].Init(this.serverTime);
			}
		}

		// Token: 0x06005015 RID: 20501 RVA: 0x0017D810 File Offset: 0x0017BA10
		private void Update()
		{
			for (int i = 0; i < this.randomDispatchers.Length; i++)
			{
				this.randomDispatchers[i].Tick(this.serverTime);
			}
			this.serverTime += (double)Time.deltaTime;
		}

		// Token: 0x04005307 RID: 21255
		[SerializeField]
		private RandomDispatcher[] randomDispatchers;

		// Token: 0x04005308 RID: 21256
		private static RandomDispatcherManager __instance;

		// Token: 0x04005309 RID: 21257
		private double serverTime;
	}
}
