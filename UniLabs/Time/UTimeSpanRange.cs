using System;
using Newtonsoft.Json;
using UnityEngine;

namespace UniLabs.Time
{
	// Token: 0x02000AAF RID: 2735
	[JsonObject(MemberSerialization.OptIn)]
	[Serializable]
	public class UTimeSpanRange
	{
		// Token: 0x17000696 RID: 1686
		// (get) Token: 0x060041EA RID: 16874 RVA: 0x00130D57 File Offset: 0x0012EF57
		// (set) Token: 0x060041EB RID: 16875 RVA: 0x00130D64 File Offset: 0x0012EF64
		public TimeSpan Start
		{
			get
			{
				return this._Start;
			}
			set
			{
				this._Start = value;
			}
		}

		// Token: 0x17000697 RID: 1687
		// (get) Token: 0x060041EC RID: 16876 RVA: 0x00130D72 File Offset: 0x0012EF72
		// (set) Token: 0x060041ED RID: 16877 RVA: 0x00130D7F File Offset: 0x0012EF7F
		public TimeSpan End
		{
			get
			{
				return this._End;
			}
			set
			{
				this._End = value;
			}
		}

		// Token: 0x17000698 RID: 1688
		// (get) Token: 0x060041EE RID: 16878 RVA: 0x00130D8D File Offset: 0x0012EF8D
		public TimeSpan Duration
		{
			get
			{
				return this.End - this.Start;
			}
		}

		// Token: 0x060041EF RID: 16879 RVA: 0x00130DA0 File Offset: 0x0012EFA0
		public bool IsInRange(TimeSpan time)
		{
			return time >= this.Start && time <= this.End;
		}

		// Token: 0x060041F0 RID: 16880 RVA: 0x00002050 File Offset: 0x00000250
		[JsonConstructor]
		public UTimeSpanRange()
		{
		}

		// Token: 0x060041F1 RID: 16881 RVA: 0x00130DBE File Offset: 0x0012EFBE
		public UTimeSpanRange(TimeSpan start)
		{
			this._Start = start;
			this._End = start;
		}

		// Token: 0x060041F2 RID: 16882 RVA: 0x00130DDE File Offset: 0x0012EFDE
		public UTimeSpanRange(TimeSpan start, TimeSpan end)
		{
			this._Start = start;
			this._End = end;
		}

		// Token: 0x060041F3 RID: 16883 RVA: 0x00130DFE File Offset: 0x0012EFFE
		private void OnStartChanged()
		{
			if (this._Start.CompareTo(this._End) > 0)
			{
				this._End.TimeSpan = this._Start.TimeSpan;
			}
		}

		// Token: 0x060041F4 RID: 16884 RVA: 0x00130E2A File Offset: 0x0012F02A
		private void OnEndChanged()
		{
			if (this._End.CompareTo(this._Start) < 0)
			{
				this._Start.TimeSpan = this._End.TimeSpan;
			}
		}

		// Token: 0x04004488 RID: 17544
		[JsonProperty("Start")]
		[SerializeField]
		private UTimeSpan _Start;

		// Token: 0x04004489 RID: 17545
		[JsonProperty("End")]
		[SerializeField]
		private UTimeSpan _End;
	}
}
