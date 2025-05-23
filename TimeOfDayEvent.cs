using System;
using UnityEngine;

// Token: 0x020009F7 RID: 2551
public class TimeOfDayEvent : TimeEvent
{
	// Token: 0x170005F9 RID: 1529
	// (get) Token: 0x06003D04 RID: 15620 RVA: 0x00121FF5 File Offset: 0x001201F5
	public float currentTime
	{
		get
		{
			return this._currentTime;
		}
	}

	// Token: 0x170005FA RID: 1530
	// (get) Token: 0x06003D05 RID: 15621 RVA: 0x00121FFD File Offset: 0x001201FD
	// (set) Token: 0x06003D06 RID: 15622 RVA: 0x00122005 File Offset: 0x00120205
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
	// (get) Token: 0x06003D07 RID: 15623 RVA: 0x00122013 File Offset: 0x00120213
	// (set) Token: 0x06003D08 RID: 15624 RVA: 0x0012201B File Offset: 0x0012021B
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
	// (get) Token: 0x06003D09 RID: 15625 RVA: 0x00122029 File Offset: 0x00120229
	public bool isOngoing
	{
		get
		{
			return this._ongoing;
		}
	}

	// Token: 0x06003D0A RID: 15626 RVA: 0x00122034 File Offset: 0x00120234
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

	// Token: 0x06003D0B RID: 15627 RVA: 0x001220B6 File Offset: 0x001202B6
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

	// Token: 0x06003D0C RID: 15628 RVA: 0x001220EC File Offset: 0x001202EC
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

	// Token: 0x06003D0D RID: 15629 RVA: 0x00122183 File Offset: 0x00120383
	public static implicit operator bool(TimeOfDayEvent ev)
	{
		return ev && ev.isOngoing;
	}

	// Token: 0x040040C3 RID: 16579
	[SerializeField]
	[Range(0f, 1f)]
	private float _timeStart;

	// Token: 0x040040C4 RID: 16580
	[SerializeField]
	[Range(0f, 1f)]
	private float _timeEnd = 1f;

	// Token: 0x040040C5 RID: 16581
	[SerializeField]
	private float _currentTime = -1f;

	// Token: 0x040040C6 RID: 16582
	[Space]
	[SerializeField]
	private double _currentSeconds = -1.0;

	// Token: 0x040040C7 RID: 16583
	[SerializeField]
	private double _totalSecondsInRange = -1.0;

	// Token: 0x040040C8 RID: 16584
	[NonSerialized]
	private float _elapsed = -1f;

	// Token: 0x040040C9 RID: 16585
	[SerializeField]
	private BetterDayNightManager _dayNightManager;
}
