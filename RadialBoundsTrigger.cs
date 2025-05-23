using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020006A3 RID: 1699
public class RadialBoundsTrigger : MonoBehaviour
{
	// Token: 0x06002A80 RID: 10880 RVA: 0x000D1454 File Offset: 0x000CF654
	public void TestOverlap()
	{
		this.TestOverlap(this._raiseEvents);
	}

	// Token: 0x06002A81 RID: 10881 RVA: 0x000D1464 File Offset: 0x000CF664
	public void TestOverlap(bool raiseEvents)
	{
		if (!this.object1 || !this.object2)
		{
			this._overlapping = false;
			this._timeOverlapStarted = -1f;
			this._timeOverlapStopped = -1f;
			this._timeSpentInOverlap = 0f;
			return;
		}
		float time = Time.time;
		float num = this.object1.radius + this.object2.radius;
		bool flag = (this.object2.center - this.object1.center).sqrMagnitude <= num * num;
		if (this._overlapping && flag)
		{
			this._overlapping = true;
			this._timeSpentInOverlap = time - this._timeOverlapStarted;
			if (raiseEvents)
			{
				UnityEvent<RadialBounds, float> onOverlapStay = this.object1.onOverlapStay;
				if (onOverlapStay != null)
				{
					onOverlapStay.Invoke(this.object2, this._timeSpentInOverlap);
				}
				UnityEvent<RadialBounds, float> onOverlapStay2 = this.object2.onOverlapStay;
				if (onOverlapStay2 == null)
				{
					return;
				}
				onOverlapStay2.Invoke(this.object1, this._timeSpentInOverlap);
				return;
			}
		}
		else if (!this._overlapping && flag)
		{
			if (time - this._timeOverlapStopped < this.hysteresis)
			{
				return;
			}
			this._overlapping = true;
			this._timeOverlapStarted = time;
			this._timeOverlapStopped = -1f;
			this._timeSpentInOverlap = 0f;
			if (raiseEvents)
			{
				UnityEvent<RadialBounds> onOverlapEnter = this.object1.onOverlapEnter;
				if (onOverlapEnter != null)
				{
					onOverlapEnter.Invoke(this.object2);
				}
				UnityEvent<RadialBounds> onOverlapEnter2 = this.object2.onOverlapEnter;
				if (onOverlapEnter2 == null)
				{
					return;
				}
				onOverlapEnter2.Invoke(this.object1);
				return;
			}
		}
		else if (!flag && this._overlapping)
		{
			this._overlapping = false;
			this._timeOverlapStarted = -1f;
			this._timeOverlapStopped = time;
			this._timeSpentInOverlap = 0f;
			if (raiseEvents)
			{
				UnityEvent<RadialBounds> onOverlapExit = this.object1.onOverlapExit;
				if (onOverlapExit != null)
				{
					onOverlapExit.Invoke(this.object2);
				}
				UnityEvent<RadialBounds> onOverlapExit2 = this.object2.onOverlapExit;
				if (onOverlapExit2 == null)
				{
					return;
				}
				onOverlapExit2.Invoke(this.object1);
			}
		}
	}

	// Token: 0x06002A82 RID: 10882 RVA: 0x000D1650 File Offset: 0x000CF850
	private void FixedUpdate()
	{
		this.TestOverlap();
	}

	// Token: 0x06002A83 RID: 10883 RVA: 0x000D1658 File Offset: 0x000CF858
	private void OnDisable()
	{
		if (this._raiseEvents && this.object1 && this.object2 && this._overlapping)
		{
			UnityEvent<RadialBounds> onOverlapExit = this.object1.onOverlapExit;
			if (onOverlapExit != null)
			{
				onOverlapExit.Invoke(this.object2);
			}
			UnityEvent<RadialBounds> onOverlapExit2 = this.object2.onOverlapExit;
			if (onOverlapExit2 != null)
			{
				onOverlapExit2.Invoke(this.object1);
			}
		}
		this._timeOverlapStarted = -1f;
		this._timeSpentInOverlap = 0f;
		this._overlapping = false;
	}

	// Token: 0x04002F65 RID: 12133
	[SerializeField]
	private Id32 _triggerID;

	// Token: 0x04002F66 RID: 12134
	[Space]
	public RadialBounds object1 = new RadialBounds();

	// Token: 0x04002F67 RID: 12135
	[Space]
	public RadialBounds object2 = new RadialBounds();

	// Token: 0x04002F68 RID: 12136
	[Space]
	public float hysteresis = 0.5f;

	// Token: 0x04002F69 RID: 12137
	[SerializeField]
	private bool _raiseEvents = true;

	// Token: 0x04002F6A RID: 12138
	[Space]
	private bool _overlapping;

	// Token: 0x04002F6B RID: 12139
	private float _timeSpentInOverlap;

	// Token: 0x04002F6C RID: 12140
	[Space]
	private float _timeOverlapStarted;

	// Token: 0x04002F6D RID: 12141
	private float _timeOverlapStopped;
}
