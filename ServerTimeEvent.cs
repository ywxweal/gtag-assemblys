using System;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020009F4 RID: 2548
public class ServerTimeEvent : TimeEvent
{
	// Token: 0x06003CFC RID: 15612 RVA: 0x00121E19 File Offset: 0x00120019
	private void Awake()
	{
		this.eventTimes = new HashSet<ServerTimeEvent.EventTime>(this.times);
	}

	// Token: 0x06003CFD RID: 15613 RVA: 0x00121E2C File Offset: 0x0012002C
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

	// Token: 0x040040B9 RID: 16569
	[SerializeField]
	private ServerTimeEvent.EventTime[] times;

	// Token: 0x040040BA RID: 16570
	[SerializeField]
	private float queryTime = 60f;

	// Token: 0x040040BB RID: 16571
	private float lastQueryTime;

	// Token: 0x040040BC RID: 16572
	private HashSet<ServerTimeEvent.EventTime> eventTimes;

	// Token: 0x020009F5 RID: 2549
	[Serializable]
	public struct EventTime
	{
		// Token: 0x06003CFF RID: 15615 RVA: 0x00121EDB File Offset: 0x001200DB
		public EventTime(int h, int m)
		{
			this.hour = h;
			this.minute = m;
		}

		// Token: 0x040040BD RID: 16573
		public int hour;

		// Token: 0x040040BE RID: 16574
		public int minute;
	}
}
