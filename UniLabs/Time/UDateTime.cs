using System;
using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace UniLabs.Time
{
	// Token: 0x02000AAD RID: 2733
	[JsonObject(MemberSerialization.OptIn)]
	[Serializable]
	public class UDateTime : ISerializationCallbackReceiver, IComparable<UDateTime>, IComparable<DateTime>
	{
		// Token: 0x17000694 RID: 1684
		// (get) Token: 0x060041C8 RID: 16840 RVA: 0x00130AEF File Offset: 0x0012ECEF
		// (set) Token: 0x060041C9 RID: 16841 RVA: 0x00130AF7 File Offset: 0x0012ECF7
		[JsonProperty("DateTime")]
		public DateTime DateTime { get; set; }

		// Token: 0x060041CA RID: 16842 RVA: 0x00130B00 File Offset: 0x0012ED00
		[JsonConstructor]
		public UDateTime()
		{
			this.DateTime = DateTime.UnixEpoch;
		}

		// Token: 0x060041CB RID: 16843 RVA: 0x00130B13 File Offset: 0x0012ED13
		public UDateTime(DateTime dateTime)
		{
			this.DateTime = dateTime;
		}

		// Token: 0x060041CC RID: 16844 RVA: 0x00130B22 File Offset: 0x0012ED22
		public static implicit operator DateTime(UDateTime udt)
		{
			return udt.DateTime;
		}

		// Token: 0x060041CD RID: 16845 RVA: 0x00130B2A File Offset: 0x0012ED2A
		public static implicit operator UDateTime(DateTime dt)
		{
			return new UDateTime
			{
				DateTime = dt
			};
		}

		// Token: 0x060041CE RID: 16846 RVA: 0x00130B38 File Offset: 0x0012ED38
		public int CompareTo(DateTime other)
		{
			return this.DateTime.CompareTo(other);
		}

		// Token: 0x060041CF RID: 16847 RVA: 0x00130B54 File Offset: 0x0012ED54
		public int CompareTo(UDateTime other)
		{
			if (this == other)
			{
				return 0;
			}
			if (other == null)
			{
				return 1;
			}
			return this.DateTime.CompareTo(other.DateTime);
		}

		// Token: 0x060041D0 RID: 16848 RVA: 0x00130B80 File Offset: 0x0012ED80
		protected bool Equals(UDateTime other)
		{
			return this.DateTime.Equals(other.DateTime);
		}

		// Token: 0x060041D1 RID: 16849 RVA: 0x00130BA1 File Offset: 0x0012EDA1
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((UDateTime)obj)));
		}

		// Token: 0x060041D2 RID: 16850 RVA: 0x00130BD0 File Offset: 0x0012EDD0
		public override int GetHashCode()
		{
			return this.DateTime.GetHashCode();
		}

		// Token: 0x060041D3 RID: 16851 RVA: 0x00130BEC File Offset: 0x0012EDEC
		public override string ToString()
		{
			return this.DateTime.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x060041D4 RID: 16852 RVA: 0x00130C0C File Offset: 0x0012EE0C
		public void OnAfterDeserialize()
		{
			DateTime dateTime;
			this.DateTime = (DateTime.TryParse(this._DateTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTime) ? dateTime : DateTime.MinValue);
		}

		// Token: 0x060041D5 RID: 16853 RVA: 0x00130C40 File Offset: 0x0012EE40
		public void OnBeforeSerialize()
		{
			this._DateTime = this.DateTime.ToString("o", CultureInfo.InvariantCulture);
		}

		// Token: 0x060041D6 RID: 16854 RVA: 0x00130C6B File Offset: 0x0012EE6B
		[OnSerializing]
		internal void OnSerializing(StreamingContext context)
		{
			this.OnBeforeSerialize();
		}

		// Token: 0x060041D7 RID: 16855 RVA: 0x00130C73 File Offset: 0x0012EE73
		[OnDeserialized]
		internal void OnDeserialized(StreamingContext context)
		{
			this.OnAfterDeserialize();
		}

		// Token: 0x04004486 RID: 17542
		[HideInInspector]
		[SerializeField]
		private string _DateTime;
	}
}
