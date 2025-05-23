using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E8A RID: 3722
	[AttributeUsage(AttributeTargets.Field)]
	public class ConditionalFieldAttribute : PropertyAttribute
	{
		// Token: 0x17000914 RID: 2324
		// (get) Token: 0x06005D18 RID: 23832 RVA: 0x001CB5FB File Offset: 0x001C97FB
		public bool ShowRange
		{
			get
			{
				return this.Min != this.Max;
			}
		}

		// Token: 0x06005D19 RID: 23833 RVA: 0x001CB610 File Offset: 0x001C9810
		public ConditionalFieldAttribute(string propertyToCheck = null, object compareValue = null, object compareValue2 = null, object compareValue3 = null, object compareValue4 = null, object compareValue5 = null, object compareValue6 = null)
		{
			this.PropertyToCheck = propertyToCheck;
			this.CompareValue = compareValue;
			this.CompareValue2 = compareValue2;
			this.CompareValue3 = compareValue3;
			this.CompareValue4 = compareValue4;
			this.CompareValue5 = compareValue5;
			this.CompareValue6 = compareValue6;
			this.Label = "";
			this.Tooltip = "";
			this.Min = 0f;
			this.Max = 0f;
		}

		// Token: 0x04006121 RID: 24865
		public string PropertyToCheck;

		// Token: 0x04006122 RID: 24866
		public object CompareValue;

		// Token: 0x04006123 RID: 24867
		public object CompareValue2;

		// Token: 0x04006124 RID: 24868
		public object CompareValue3;

		// Token: 0x04006125 RID: 24869
		public object CompareValue4;

		// Token: 0x04006126 RID: 24870
		public object CompareValue5;

		// Token: 0x04006127 RID: 24871
		public object CompareValue6;

		// Token: 0x04006128 RID: 24872
		public string Label;

		// Token: 0x04006129 RID: 24873
		public string Tooltip;

		// Token: 0x0400612A RID: 24874
		public float Min;

		// Token: 0x0400612B RID: 24875
		public float Max;
	}
}
