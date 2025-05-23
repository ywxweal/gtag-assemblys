using System;
using UnityEngine;

// Token: 0x02000127 RID: 295
public class MonkeVoteProximityTrigger : GorillaTriggerBox
{
	// Token: 0x14000012 RID: 18
	// (add) Token: 0x060007BF RID: 1983 RVA: 0x0002B774 File Offset: 0x00029974
	// (remove) Token: 0x060007C0 RID: 1984 RVA: 0x0002B7AC File Offset: 0x000299AC
	public event Action OnEnter;

	// Token: 0x170000BD RID: 189
	// (get) Token: 0x060007C1 RID: 1985 RVA: 0x0002B7E1 File Offset: 0x000299E1
	// (set) Token: 0x060007C2 RID: 1986 RVA: 0x0002B7E9 File Offset: 0x000299E9
	public bool isPlayerNearby { get; private set; }

	// Token: 0x060007C3 RID: 1987 RVA: 0x0002B7F2 File Offset: 0x000299F2
	public override void OnBoxTriggered()
	{
		this.isPlayerNearby = true;
		if (this.triggerTime + this.retriggerDelay < Time.unscaledTime)
		{
			this.triggerTime = Time.unscaledTime;
			Action onEnter = this.OnEnter;
			if (onEnter == null)
			{
				return;
			}
			onEnter();
		}
	}

	// Token: 0x060007C4 RID: 1988 RVA: 0x0002B82A File Offset: 0x00029A2A
	public override void OnBoxExited()
	{
		this.isPlayerNearby = false;
	}

	// Token: 0x04000945 RID: 2373
	private float triggerTime = float.MinValue;

	// Token: 0x04000946 RID: 2374
	private float retriggerDelay = 0.25f;
}
