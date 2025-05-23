using System;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020009F4 RID: 2548
public class ServerTimeEvent : TimeEvent
{
	// Token: 0x06003CFD RID: 15613 RVA: 0x00121EF1 File Offset: 0x001200F1
	private void Awake()
	{
		this.eventTimes = new HashSet<ServerTimeEvent.EventTime>(this.times);
	}

	// Token: 0x06003CFE RID: 15614 RVA: 0x00121F04 File Offset: 0x00120104
	private void Update()
	{
		if (GorillaComputer.instance == null || Time.time - this.lastQueryTime < this.queryTime)
		{
			return;
		}
		ServerTimeEvent.EventTime eventTime = new ServerTimeEvent.EventTime(GorillaComputer.instance.GetServerTime().Hour, GorillaComputer.instance.GetServerTime().Minute);
		bool flag = this.eventTimes.Contains(eventTime);
		if (!this._ongoing && flag)
		{
			base.StartEvent();
		}
		if (this._ongoing && !flag)
		{
			base.StopEvent();
		}
		this.lastQueryTime = Time.time;
	}

	// Token: 0x040040BA RID: 16570
	[SerializeField]
	private ServerTimeEvent.EventTime[] times;

	// Token: 0x040040BB RID: 16571
	[SerializeField]
	private float queryTime = 60f;

	// Token: 0x040040BC RID: 16572
	private float lastQueryTime;

	// Token: 0x040040BD RID: 16573
	private HashSet<ServerTimeEvent.EventTime> eventTimes;

	// Token: 0x020009F5 RID: 2549
	[Serializable]
	public struct EventTime
	{
		// Token: 0x06003D00 RID: 15616 RVA: 0x00121FB3 File Offset: 0x001201B3
		public EventTime(int h, int m)
		{
			this.hour = h;
			this.minute = m;
		}

		// Token: 0x040040BE RID: 16574
		public int hour;

		// Token: 0x040040BF RID: 16575
		public int minute;
	}
}
