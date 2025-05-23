using System;
using System.Collections.Generic;

namespace LitJson
{
	// Token: 0x02000A90 RID: 2704
	internal struct ObjectMetadata
	{
		// Token: 0x17000682 RID: 1666
		// (get) Token: 0x060040F2 RID: 16626 RVA: 0x0012C7F4 File Offset: 0x0012A9F4
		// (set) Token: 0x060040F3 RID: 16627 RVA: 0x0012C815 File Offset: 0x0012AA15
		public Type ElementType
		{
			get
			{
				if (this.element_type == null)
				{
					return typeof(JsonData);
				}
				return this.element_type;
			}
			set
			{
				this.element_type = value;
			}
		}

		// Token: 0x17000683 RID: 1667
		// (get) Token: 0x060040F4 RID: 16628 RVA: 0x0012C81E File Offset: 0x0012AA1E
		// (set) Token: 0x060040F5 RID: 16629 RVA: 0x0012C826 File Offset: 0x0012AA26
		public bool IsDictionary
		{
			get
			{
				return this.is_dictionary;
			}
			set
			{
				this.is_dictionary = value;
			}
		}

		// Token: 0x17000684 RID: 1668
		// (get) Token: 0x060040F6 RID: 16630 RVA: 0x0012C82F File Offset: 0x0012AA2F
		// (set) Token: 0x060040F7 RID: 16631 RVA: 0x0012C837 File Offset: 0x0012AA37
		public IDictionary<string, PropertyMetadata> Properties
		{
			get
			{
				return this.properties;
			}
			set
			{
				this.properties = value;
			}
		}

		// Token: 0x040043E0 RID: 17376
		private Type element_type;

		// Token: 0x040043E1 RID: 17377
		private bool is_dictionary;

		// Token: 0x040043E2 RID: 17378
		private IDictionary<string, PropertyMetadata> properties;
	}
}
