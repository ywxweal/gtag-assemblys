using System;
using UnityEngine;

// Token: 0x02000936 RID: 2358
public class GTDelayedExec : ITickSystemTick
{
	// Token: 0x170005A2 RID: 1442
	// (get) Token: 0x06003938 RID: 14648 RVA: 0x0011347A File Offset: 0x0011167A
	// (set) Token: 0x06003939 RID: 14649 RVA: 0x00113481 File Offset: 0x00111681
	public static GTDelayedExec instance { get; private set; }

	// Token: 0x170005A3 RID: 1443
	// (get) Token: 0x0600393A RID: 14650 RVA: 0x00113489 File Offset: 0x00111689
	// (set) Token: 0x0600393B RID: 14651 RVA: 0x00113491 File Offset: 0x00111691
	public int listenerCount { get; private set; }

	// Token: 0x0600393C RID: 14652 RVA: 0x0011349A File Offset: 0x0011169A
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void InitializeAfterAssemblies()
	{
		GTDelayedExec.instance = new GTDelayedExec();
		TickSystem<object>.AddTickCallback(GTDelayedExec.instance);
	}

	// Token: 0x0600393D RID: 14653 RVA: 0x001134B0 File Offset: 0x001116B0
	internal static void Add(IDelayedExecListener listener, float delay, int contextId)
	{
		if (GTDelayedExec.instance.listenerCount >= 1024)
		{
			Debug.LogError("Maximum number of delayed listeners reached.");
			return;
		}
		GTDelayedExec._listenerDelays[GTDelayedExec.instance.listenerCount] = Time.unscaledTime + delay;
		GTDelayedExec._listeners[GTDelayedExec.instance.listenerCount] = new GTDelayedExec.Listener(listener, contextId);
		GTDelayedExec instance = GTDelayedExec.instance;
		int listenerCount = instance.listenerCount;
		instance.listenerCount = listenerCount + 1;
	}

	// Token: 0x170005A4 RID: 1444
	// (get) Token: 0x0600393E RID: 14654 RVA: 0x0011351F File Offset: 0x0011171F
	// (set) Token: 0x0600393F RID: 14655 RVA: 0x00113527 File Offset: 0x00111727
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x06003940 RID: 14656 RVA: 0x00113530 File Offset: 0x00111730
	void ITickSystemTick.Tick()
	{
		for (int i = 0; i < this.listenerCount; i++)
		{
			if (Time.unscaledTime >= GTDelayedExec._listenerDelays[i])
			{
				try
				{
					GTDelayedExec._listeners[i].listener.OnDelayedAction(GTDelayedExec._listeners[i].contextId);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
				int listenerCount = this.listenerCount;
				this.listenerCount = listenerCount - 1;
				GTDelayedExec._listenerDelays[i] = GTDelayedExec._listenerDelays[this.listenerCount];
				GTDelayedExec._listeners[i] = GTDelayedExec._listeners[this.listenerCount];
				i--;
			}
		}
	}

	// Token: 0x04003E61 RID: 15969
	public const int kMaxListeners = 1024;

	// Token: 0x04003E63 RID: 15971
	private static readonly float[] _listenerDelays = new float[1024];

	// Token: 0x04003E64 RID: 15972
	private static readonly GTDelayedExec.Listener[] _listeners = new GTDelayedExec.Listener[1024];

	// Token: 0x02000937 RID: 2359
	private struct Listener
	{
		// Token: 0x06003943 RID: 14659 RVA: 0x00113604 File Offset: 0x00111804
		public Listener(IDelayedExecListener listener, int contextId)
		{
			this.listener = listener;
			this.contextId = contextId;
		}

		// Token: 0x04003E66 RID: 15974
		public readonly IDelayedExecListener listener;

		// Token: 0x04003E67 RID: 15975
		public readonly int contextId;
	}
}
