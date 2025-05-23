using System;
using UnityEngine;

// Token: 0x020009F7 RID: 2551
public class TimeOfDayEvent : TimeEvent
{
	// Token: 0x170005F9 RID: 1529
	// (get) Token: 0x06003D03 RID: 15619 RVA: 0x00121F1D File Offset: 0x0012011D
	public float currentTime
	{
		get
		{
			return this._currentTime;
		}
	}

	// Token: 0x170005FA RID: 1530
	// (get) Token: 0x06003D04 RID: 15620 RVA: 0x00121F25 File Offset: 0x00120125
	// (set) Token: 0x06003D05 RID: 15621 RVA: 0x00121F2D File Offset: 0x0012012D
	public float timeStart
	{
		get
		{
			return this._timeStart;
		}
		set
		{
			this._timeStart = Mathf.Clamp01(value);
		}
	}

	// Token: 0x170005FB RID: 1531
	// (get) Token: 0x06003D06 RID: 15622 RVA: 0x00121F3B File Offset: 0x0012013B
	// (set) Token: 0x06003D07 RID: 15623 RVA: 0x00121F43 File Offset: 0x00120143
	public float timeEnd
	{
		get
		{
			return this._timeEnd;
		}
		set
		{
			this._timeEnd = Mathf.Clamp01(value);
		}
	}

	// Token: 0x170005FC RID: 1532
	// (get) Token: 0x06003D08 RID: 15624 RVA: 0x00121F51 File Offset: 0x00120151
	public bool isOngoing
	{
		get
		{
			return this._ongoing;
		}
	}

	// Token: 0x06003D09 RID: 15625 RVA: 0x00121F5C File Offset: 0x0012015C
	private void Start()
	{
		if (!this._dayNightManager)
		{
			this._dayNightManager = BetterDayNightManager.instance;
		}
		if (!this._dayNightManager)
		{
			return;
		}
		for (int i = 0; i < this._dayNightManager.timeOfDayRange.Length; i++)
		{
			this._totalSecondsInRange += this._dayNightManager.timeOfDayRange[i] * 3600.0;
		}
		this._totalSecondsInRange = Math.Floor(this._totalSecondsInRange);
	}

	// Token: 0x06003D0A RID: 15626 RVA: 0x00121FDE File Offset: 0x001201DE
	private void Update()
	{
		this._elapsed += Time.deltaTime;
		if (this._elapsed < 1f)
		{
			return;
		}
		this._elapsed = 0f;
		this.UpdateTime();
	}

	// Token: 0x06003D0B RID: 15627 RVA: 0x00122014 File Offset: 0x00120214
	private void UpdateTime()
	{
		this._currentSeconds = ((ITimeOfDaySystem)this._dayNightManager).currentTimeInSeconds;
		this._currentSeconds = Math.Floor(this._currentSeconds);
		this._currentTime = (float)(this._currentSeconds / this._totalSecondsInRange);
		bool flag = this._currentTime >= 0f && this._currentTime >= this._timeStart && this._currentTime <= this._timeEnd;
		if (!this._ongoing && flag)
		{
			base.StartEvent();
		}
		if (this._ongoing && !flag)
		{
			base.StopEvent();
		}
	}

	// Token: 0x06003D0C RID: 15628 RVA: 0x001220AB File Offset: 0x001202AB
	public static implicit operator bool(TimeOfDayEvent ev)
	{
		return ev && ev.isOngoing;
	}

	// Token: 0x040040C2 RID: 16578
	[SerializeField]
	[Range(0f, 1f)]
	private float _timeStart;

	// Token: 0x040040C3 RID: 16579
	[SerializeField]
	[Range(0f, 1f)]
	private float _timeEnd = 1f;

	// Token: 0x040040C4 RID: 16580
	[SerializeField]
	private float _currentTime = -1f;

	// Token: 0x040040C5 RID: 16581
	[Space]
	[SerializeField]
	private double _currentSeconds = -1.0;

	// Token: 0x040040C6 RID: 16582
	[SerializeField]
	private double _totalSecondsInRange = -1.0;

	// Token: 0x040040C7 RID: 16583
	[NonSerialized]
	private float _elapsed = -1f;

	// Token: 0x040040C8 RID: 16584
	[SerializeField]
	private BetterDayNightManager _dayNightManager;
}
