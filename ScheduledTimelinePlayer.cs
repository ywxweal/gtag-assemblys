using System;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x020000F0 RID: 240
public class ScheduledTimelinePlayer : MonoBehaviour
{
	// Token: 0x06000613 RID: 1555 RVA: 0x0002306F File Offset: 0x0002126F
	protected void OnEnable()
	{
		this.scheduledEventID = BetterDayNightManager.RegisterScheduledEvent(this.eventHour, new Action(this.HandleScheduledEvent));
	}

	// Token: 0x06000614 RID: 1556 RVA: 0x0002308E File Offset: 0x0002128E
	protected void OnDisable()
	{
		BetterDayNightManager.UnregisterScheduledEvent(this.scheduledEventID);
	}

	// Token: 0x06000615 RID: 1557 RVA: 0x0002309B File Offset: 0x0002129B
	private void HandleScheduledEvent()
	{
		this.timeline.Play();
	}

	// Token: 0x0400071E RID: 1822
	public PlayableDirector timeline;

	// Token: 0x0400071F RID: 1823
	public int eventHour = 7;

	// Token: 0x04000720 RID: 1824
	private int scheduledEventID;
}
