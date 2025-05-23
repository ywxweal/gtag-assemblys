using System;

namespace GorillaTag
{
	// Token: 0x02000D01 RID: 3329
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class GTStripGameObjectFromBuildAttribute : Attribute
	{
		// Token: 0x17000850 RID: 2128
		// (get) Token: 0x0600538A RID: 21386 RVA: 0x00195837 File Offset: 0x00193A37
		public string Condition { get; }

		// Token: 0x0600538B RID: 21387 RVA: 0x0019583F File Offset: 0x00193A3F
		public GTStripGameObjectFromBuildAttribute(string condition = "")
		{
			this.Condition = condition;
		}
	}
}
