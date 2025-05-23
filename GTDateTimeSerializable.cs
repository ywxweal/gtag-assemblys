using System;
using System.Globalization;
using UnityEngine;

// Token: 0x020001A7 RID: 423
[Serializable]
public struct GTDateTimeSerializable : ISerializationCallbackReceiver
{
	// Token: 0x1700010A RID: 266
	// (get) Token: 0x06000A73 RID: 2675 RVA: 0x00039B1A File Offset: 0x00037D1A
	// (set) Token: 0x06000A74 RID: 2676 RVA: 0x00039B22 File Offset: 0x00037D22
	public DateTime dateTime
	{
		get
		{
			return this._dateTime;
		}
		set
		{
			this._dateTime = value;
			this._dateTimeString = GTDateTimeSerializable.FormatDateTime(this._dateTime);
		}
	}

	// Token: 0x06000A75 RID: 2677 RVA: 0x00039B3C File Offset: 0x00037D3C
	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
		this._dateTimeString = GTDateTimeSerializable.FormatDateTime(this._dateTime);
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x00039B50 File Offset: 0x00037D50
	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		DateTime dateTime;
		if (GTDateTimeSerializable.TryParseDateTime(this._dateTimeString, out dateTime))
		{
			this._dateTime = dateTime;
		}
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x00039B74 File Offset: 0x00037D74
	public GTDateTimeSerializable(int dummyValue)
	{
		DateTime now = DateTime.Now;
		this._dateTime = new DateTime(now.Year, now.Month, now.Day, 11, 0, 0);
		this._dateTimeString = GTDateTimeSerializable.FormatDateTime(this._dateTime);
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x00039BBC File Offset: 0x00037DBC
	private static string FormatDateTime(DateTime dateTime)
	{
		return dateTime.ToString("yyyy-MM-dd HH:mm");
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x00039BCC File Offset: 0x00037DCC
	private static bool TryParseDateTime(string value, out DateTime result)
	{
		if (DateTime.TryParseExact(value, new string[] { "yyyy-MM-dd HH:mm", "yyyy-MM-dd", "yyyy-MM" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
		{
			DateTime dateTime = result;
			if (dateTime.Hour == 0 && dateTime.Minute == 0)
			{
				result = result.AddHours(11.0);
			}
			return true;
		}
		return false;
	}

	// Token: 0x04000CB1 RID: 3249
	[HideInInspector]
	[SerializeField]
	private string _dateTimeString;

	// Token: 0x04000CB2 RID: 3250
	private DateTime _dateTime;
}
