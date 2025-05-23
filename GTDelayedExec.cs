using System;
using UnityEngine;

// Token: 0x02000936 RID: 2358
public class GTDelayedExec : ITickSystemTick
{
	// Token: 0x170005A2 RID: 1442
	// (get) Token: 0x06003937 RID: 14647 RVA: 0x001133A2 File Offset: 0x001115A2
	// (set) Token: 0x06003938 RID: 14648 RVA: 0x001133A9 File Offset: 0x001115A9
	public static GTDelayedExec instance { get; private set; }

	// Token: 0x170005A3 RID: 1443
	// (get) Token: 0x06003939 RID: 14649 RVA: 0x001133B1 File Offset: 0x001115B1
	// (set) Token: 0x0600393A RID: 14650 RVA: 0x001133B9 File Offset: 0x001115B9
	public int listenerCount { get; private set; }

	// Token: 0x0600393B RID: 14651 RVA: 0x001133C2 File Offset: 0x001115C2
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void InitializeAfterAssemblies()
	{
		GTDelayedExec.instance = new GTDelayedExec();
		TickSystem<object>.AddTickCallback(GTDelayedExec.instance);
	}

	// Token: 0x0600393C RID: 14652 RVA: 0x001133D8 File Offset: 0x001115D8
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
	// (get) Token: 0x0600393D RID: 14653 RVA: 0x00113447 File Offset: 0x00111647
	// (set) Token: 0x0600393E RID: 14654 RVA: 0x0011344F File Offset: 0x0011164F
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x0600393F RID: 14655 RVA: 0x00113458 File Offset: 0x00111658
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

	// Token: 0x04003E60 RID: 15968
	public const int kMaxListeners = 1024;

	// Token: 0x04003E62 RID: 15970
	private static readonly float[] _listenerDelays = new float[1024];

	// Token: 0x04003E63 RID: 15971
	private static readonly GTDelayedExec.Listener[] _listeners = new GTDelayedExec.Listener[1024];

	// Token: 0x02000937 RID: 2359
	private struct Listener
	{
		// Token: 0x06003942 RID: 14658 RVA: 0x0011352C File Offset: 0x0011172C
		public Listener(IDelayedExecListener listener, int contextId)
		{
			this.listener = listener;
			this.contextId = contextId;
		}

		// Token: 0x04003E65 RID: 15973
		public readonly IDelayedExecListener listener;

		// Token: 0x04003E66 RID: 15974
		public readonly int contextId;
	}
}
