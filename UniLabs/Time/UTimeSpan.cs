using System;
using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace UniLabs.Time
{
	// Token: 0x02000AAE RID: 2734
	[JsonObject(MemberSerialization.OptIn)]
	[Serializable]
	public class UTimeSpan : ISerializationCallbackReceiver, IComparable<UTimeSpan>, IComparable<TimeSpan>
	{
		// Token: 0x17000695 RID: 1685
		// (get) Token: 0x060041D7 RID: 16855 RVA: 0x00130BA3 File Offset: 0x0012EDA3
		// (set) Token: 0x060041D8 RID: 16856 RVA: 0x00130BAB File Offset: 0x0012EDAB
		[JsonProperty("TimeSpan")]
		public TimeSpan TimeSpan { get; set; }

		// Token: 0x060041D9 RID: 16857 RVA: 0x00130BB4 File Offset: 0x0012EDB4
		[JsonConstructor]
		public UTimeSpan()
		{
			this.TimeSpan = TimeSpan.Zero;
		}

		// Token: 0x060041DA RID: 16858 RVA: 0x00130BC7 File Offset: 0x0012EDC7
		public UTimeSpan(TimeSpan timeSpan)
		{
			this.TimeSpan = timeSpan;
		}

		// Token: 0x060041DB RID: 16859 RVA: 0x00130BD6 File Offset: 0x0012EDD6
		public UTimeSpan(long ticks)
			: this(new TimeSpan(ticks))
		{
		}

		// Token: 0x060041DC RID: 16860 RVA: 0x00130BE4 File Offset: 0x0012EDE4
		public UTimeSpan(int hours, int minutes, int seconds)
			: this(new TimeSpan(hours, minutes, seconds))
		{
		}

		// Token: 0x060041DD RID: 16861 RVA: 0x00130BF4 File Offset: 0x0012EDF4
		public UTimeSpan(int days, int hours, int minutes, int seconds)
			: this(new TimeSpan(days, hours, minutes, seconds))
		{
		}

		// Token: 0x060041DE RID: 16862 RVA: 0x00130C06 File Offset: 0x0012EE06
		public UTimeSpan(int days, int hours, int minutes, int seconds, int milliseconds)
			: this(new TimeSpan(days, hours, minutes, seconds, milliseconds))
		{
		}

		// Token: 0x060041DF RID: 16863 RVA: 0x00130C1A File Offset: 0x0012EE1A
		public static implicit operator TimeSpan(UTimeSpan uTimeSpan)
		{
			if (uTimeSpan == null)
			{
				return TimeSpan.Zero;
			}
			return uTimeSpan.TimeSpan;
		}

		// Token: 0x060041E0 RID: 16864 RVA: 0x00130C2B File Offset: 0x0012EE2B
		public static implicit operator UTimeSpan(TimeSpan timeSpan)
		{
			return new UTimeSpan(timeSpan);
		}

		// Token: 0x060041E1 RID: 16865 RVA: 0x00130C34 File Offset: 0x0012EE34
		public int CompareTo(TimeSpan other)
		{
			return this.TimeSpan.CompareTo(other);
		}

		// Token: 0x060041E2 RID: 16866 RVA: 0x00130C50 File Offset: 0x0012EE50
		public int CompareTo(UTimeSpan other)
		{
			if (this == other)
			{
				return 0;
			}
			if (other == null)
			{
				return 1;
			}
			return this.TimeSpan.CompareTo(other.TimeSpan);
		}

		// Token: 0x060041E3 RID: 16867 RVA: 0x00130C7C File Offset: 0x0012EE7C
		protected bool Equals(UTimeSpan other)
		{
			return this.TimeSpan.Equals(other.TimeSpan);
		}

		// Token: 0x060041E4 RID: 16868 RVA: 0x00130C9D File Offset: 0x0012EE9D
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((UTimeSpan)obj)));
		}

		// Token: 0x060041E5 RID: 16869 RVA: 0x00130CCC File Offset: 0x0012EECC
		public override int GetHashCode()
		{
			return this.TimeSpan.GetHashCode();
		}

		// Token: 0x060041E6 RID: 16870 RVA: 0x00130CF0 File Offset: 0x0012EEF0
		public void OnAfterDeserialize()
		{
			TimeSpan timeSpan;
			this.TimeSpan = (TimeSpan.TryParse(this._TimeSpan, CultureInfo.InvariantCulture, out timeSpan) ? timeSpan : TimeSpan.Zero);
		}

		// Token: 0x060041E7 RID: 16871 RVA: 0x00130D20 File Offset: 0x0012EF20
		public void OnBeforeSerialize()
		{
			this._TimeSpan = this.TimeSpan.ToString();
		}

		// Token: 0x060041E8 RID: 16872 RVA: 0x00130D47 File Offset: 0x0012EF47
		[OnSerializing]
		internal void OnSerializingMethod(StreamingContext context)
		{
			this.OnBeforeSerialize();
		}

		// Token: 0x060041E9 RID: 16873 RVA: 0x00130D4F File Offset: 0x0012EF4F
		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			this.OnAfterDeserialize();
		}

		// Token: 0x04004487 RID: 17543
		[HideInInspector]
		[SerializeField]
		private string _TimeSpan;
	}
}
