﻿using System;

namespace LitJson
{
	// Token: 0x02000A8F RID: 2703
	internal struct ArrayMetadata
	{
		// Token: 0x1700067F RID: 1663
		// (get) Token: 0x060040ED RID: 16621 RVA: 0x0012C880 File Offset: 0x0012AA80
		// (set) Token: 0x060040EE RID: 16622 RVA: 0x0012C8A1 File Offset: 0x0012AAA1
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

		// Token: 0x17000680 RID: 1664
		// (get) Token: 0x060040EF RID: 16623 RVA: 0x0012C8AA File Offset: 0x0012AAAA
		// (set) Token: 0x060040F0 RID: 16624 RVA: 0x0012C8B2 File Offset: 0x0012AAB2
		public bool IsArray
		{
			get
			{
				return this.is_array;
			}
			set
			{
				this.is_array = value;
			}
		}

		// Token: 0x17000681 RID: 1665
		// (get) Token: 0x060040F1 RID: 16625 RVA: 0x0012C8BB File Offset: 0x0012AABB
		// (set) Token: 0x060040F2 RID: 16626 RVA: 0x0012C8C3 File Offset: 0x0012AAC3
		public bool IsList
		{
			get
			{
				return this.is_list;
			}
			set
			{
				this.is_list = value;
			}
		}

		// Token: 0x040043DE RID: 17374
		private Type element_type;

		// Token: 0x040043DF RID: 17375
		private bool is_array;

		// Token: 0x040043E0 RID: 17376
		private bool is_list;
	}
}
