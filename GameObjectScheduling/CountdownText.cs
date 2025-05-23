using System;
using System.Collections;
using System.Globalization;
using GorillaNetworking;
using TMPro;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x02000E20 RID: 3616
	public class CountdownText : MonoBehaviour
	{
		// Token: 0x170008D3 RID: 2259
		// (get) Token: 0x06005A7E RID: 23166 RVA: 0x001B91B9 File Offset: 0x001B73B9
		// (set) Token: 0x06005A7F RID: 23167 RVA: 0x001B91C4 File Offset: 0x001B73C4
		public CountdownTextDate Countdown
		{
			get
			{
				return this.CountdownTo;
			}
			set
			{
				this.CountdownTo = value;
				if (this.CountdownTo.FormatString.Length > 0)
				{
					this.displayTextFormat = this.CountdownTo.FormatString;
				}
				this.displayText.text = this.CountdownTo.DefaultString;
				if (base.gameObject.activeInHierarchy && !this.useExternalTime && this.monitor == null && this.CountdownTo != null)
				{
					this.monitor = base.StartCoroutine(this.MonitorTime());
				}
			}
		}

		// Token: 0x06005A80 RID: 23168 RVA: 0x001B9250 File Offset: 0x001B7450
		private void Awake()
		{
			this.displayText = base.GetComponent<TMP_Text>();
			this.displayTextFormat = this.displayText.text.Trim();
			this.displayText.text = string.Empty;
			if (this.CountdownTo == null)
			{
				return;
			}
			if (this.displayTextFormat.Length == 0 && this.CountdownTo.FormatString.Length > 0)
			{
				this.displayTextFormat = this.CountdownTo.FormatString;
			}
			this.displayText.text = this.CountdownTo.DefaultString;
		}

		// Token: 0x06005A81 RID: 23169 RVA: 0x001B92E5 File Offset: 0x001B74E5
		private void OnEnable()
		{
			if (this.CountdownTo == null)
			{
				return;
			}
			if (this.monitor == null && !this.useExternalTime)
			{
				this.monitor = base.StartCoroutine(this.MonitorTime());
			}
		}

		// Token: 0x06005A82 RID: 23170 RVA: 0x001B9318 File Offset: 0x001B7518
		private void OnDisable()
		{
			this.StopMonitorTime();
			this.StopDisplayRefresh();
		}

		// Token: 0x06005A83 RID: 23171 RVA: 0x001B9326 File Offset: 0x001B7526
		private IEnumerator MonitorTime()
		{
			while (GorillaComputer.instance == null || GorillaComputer.instance.startupMillis == 0L)
			{
				yield return null;
			}
			this.monitor = null;
			this.targetTime = this.TryParseDateTime();
			if (this.updateDisplay)
			{
				this.StartDisplayRefresh();
			}
			else
			{
				this.RefreshDisplay();
			}
			yield break;
		}

		// Token: 0x06005A84 RID: 23172 RVA: 0x001B9335 File Offset: 0x001B7535
		private IEnumerator MonitorExternalTime(DateTime countdown)
		{
			while (GorillaComputer.instance == null || GorillaComputer.instance.startupMillis == 0L)
			{
				yield return null;
			}
			this.monitor = null;
			this.targetTime = countdown;
			if (this.updateDisplay)
			{
				this.StartDisplayRefresh();
			}
			else
			{
				this.RefreshDisplay();
			}
			yield break;
		}

		// Token: 0x06005A85 RID: 23173 RVA: 0x001B934B File Offset: 0x001B754B
		private void StopMonitorTime()
		{
			if (this.monitor != null)
			{
				base.StopCoroutine(this.monitor);
			}
			this.monitor = null;
		}

		// Token: 0x06005A86 RID: 23174 RVA: 0x001B9368 File Offset: 0x001B7568
		public void SetCountdownTime(DateTime countdown)
		{
			this.StopMonitorTime();
			this.StopDisplayRefresh();
			this.monitor = base.StartCoroutine(this.MonitorExternalTime(countdown));
		}

		// Token: 0x06005A87 RID: 23175 RVA: 0x001B9389 File Offset: 0x001B7589
		public void SetFixedText(string text)
		{
			this.StopMonitorTime();
			this.StopDisplayRefresh();
			this.displayText.text = text;
		}

		// Token: 0x06005A88 RID: 23176 RVA: 0x001B93A3 File Offset: 0x001B75A3
		private void StartDisplayRefresh()
		{
			this.StopDisplayRefresh();
			this.displayRefresh = base.StartCoroutine(this.WaitForDisplayRefresh());
		}

		// Token: 0x06005A89 RID: 23177 RVA: 0x001B93BD File Offset: 0x001B75BD
		private void StopDisplayRefresh()
		{
			if (this.displayRefresh != null)
			{
				base.StopCoroutine(this.displayRefresh);
			}
			this.displayRefresh = null;
		}

		// Token: 0x06005A8A RID: 23178 RVA: 0x001B93DA File Offset: 0x001B75DA
		private IEnumerator WaitForDisplayRefresh()
		{
			for (;;)
			{
				this.RefreshDisplay();
				TimeSpan timeSpan;
				if (this.countdownTime.Days > 0)
				{
					timeSpan = this.countdownTime - TimeSpan.FromDays((double)this.countdownTime.Days);
				}
				else if (this.countdownTime.Hours > 0)
				{
					timeSpan = this.countdownTime - TimeSpan.FromHours((double)this.countdownTime.Hours);
				}
				else if (this.countdownTime.Minutes > 0)
				{
					timeSpan = this.countdownTime - TimeSpan.FromMinutes((double)this.countdownTime.Minutes);
				}
				else
				{
					if (this.countdownTime.Seconds <= 0)
					{
						break;
					}
					timeSpan = this.countdownTime - TimeSpan.FromSeconds((double)this.countdownTime.Seconds);
				}
				yield return new WaitForSeconds((float)timeSpan.TotalSeconds);
			}
			yield break;
		}

		// Token: 0x06005A8B RID: 23179 RVA: 0x001B93EC File Offset: 0x001B75EC
		private void RefreshDisplay()
		{
			this.countdownTime = this.targetTime.Subtract(GorillaComputer.instance.GetServerTime());
			this.displayText.text = CountdownText.GetTimeDisplay(this.countdownTime, this.displayTextFormat, this.CountdownTo.DaysThreshold, string.Empty, this.CountdownTo.DefaultString);
		}

		// Token: 0x06005A8C RID: 23180 RVA: 0x001B944D File Offset: 0x001B764D
		public static string GetTimeDisplay(TimeSpan ts, string format)
		{
			return CountdownText.GetTimeDisplay(ts, format, int.MaxValue, string.Empty, string.Empty);
		}

		// Token: 0x06005A8D RID: 23181 RVA: 0x001B9468 File Offset: 0x001B7668
		public static string GetTimeDisplay(TimeSpan ts, string format, int maxDaysToDisplay, string elapsedString, string overMaxString)
		{
			if (ts.TotalSeconds < 0.0)
			{
				return elapsedString;
			}
			if (ts.TotalDays < (double)maxDaysToDisplay)
			{
				if (ts.Days > 0)
				{
					return string.Format(format, ts.Days, CountdownText.getTimeChunkString(CountdownText.TimeChunk.DAY, ts.Days));
				}
				if (ts.Hours > 0)
				{
					return string.Format(format, ts.Hours, CountdownText.getTimeChunkString(CountdownText.TimeChunk.HOUR, ts.Hours));
				}
				if (ts.Minutes > 0)
				{
					return string.Format(format, ts.Minutes, CountdownText.getTimeChunkString(CountdownText.TimeChunk.MINUTE, ts.Minutes));
				}
				if (ts.Seconds > 0)
				{
					return string.Format(format, ts.Seconds, CountdownText.getTimeChunkString(CountdownText.TimeChunk.SECOND, ts.Seconds));
				}
			}
			return overMaxString;
		}

		// Token: 0x06005A8E RID: 23182 RVA: 0x001B9544 File Offset: 0x001B7744
		private static string getTimeChunkString(CountdownText.TimeChunk chunk, int n)
		{
			switch (chunk)
			{
			case CountdownText.TimeChunk.DAY:
				if (n == 1)
				{
					return "DAY";
				}
				return "DAYS";
			case CountdownText.TimeChunk.HOUR:
				if (n == 1)
				{
					return "HOUR";
				}
				return "HOURS";
			case CountdownText.TimeChunk.MINUTE:
				if (n == 1)
				{
					return "MINUTE";
				}
				return "MINUTES";
			case CountdownText.TimeChunk.SECOND:
				if (n == 1)
				{
					return "SECOND";
				}
				return "SECONDS";
			default:
				return string.Empty;
			}
		}

		// Token: 0x06005A8F RID: 23183 RVA: 0x001B95B0 File Offset: 0x001B77B0
		private DateTime TryParseDateTime()
		{
			DateTime dateTime;
			try
			{
				dateTime = DateTime.Parse(this.CountdownTo.CountdownTo, CultureInfo.InvariantCulture);
			}
			catch
			{
				dateTime = DateTime.MinValue;
			}
			return dateTime;
		}

		// Token: 0x04005E99 RID: 24217
		[SerializeField]
		private CountdownTextDate CountdownTo;

		// Token: 0x04005E9A RID: 24218
		[SerializeField]
		private bool updateDisplay;

		// Token: 0x04005E9B RID: 24219
		[SerializeField]
		private bool useExternalTime;

		// Token: 0x04005E9C RID: 24220
		private TMP_Text displayText;

		// Token: 0x04005E9D RID: 24221
		private string displayTextFormat;

		// Token: 0x04005E9E RID: 24222
		private DateTime targetTime;

		// Token: 0x04005E9F RID: 24223
		private TimeSpan countdownTime;

		// Token: 0x04005EA0 RID: 24224
		private Coroutine monitor;

		// Token: 0x04005EA1 RID: 24225
		private Coroutine displayRefresh;

		// Token: 0x02000E21 RID: 3617
		private enum TimeChunk
		{
			// Token: 0x04005EA3 RID: 24227
			DAY,
			// Token: 0x04005EA4 RID: 24228
			HOUR,
			// Token: 0x04005EA5 RID: 24229
			MINUTE,
			// Token: 0x04005EA6 RID: 24230
			SECOND
		}
	}
}
