using System;
using System.Collections;
using System.Collections.Generic;

namespace LitJson
{
	// Token: 0x02000A8C RID: 2700
	internal class OrderedDictionaryEnumerator : IDictionaryEnumerator, IEnumerator
	{
		// Token: 0x1700067B RID: 1659
		// (get) Token: 0x060040DE RID: 16606 RVA: 0x0012C686 File Offset: 0x0012A886
		public object Current
		{
			get
			{
				return this.Entry;
			}
		}

		// Token: 0x1700067C RID: 1660
		// (get) Token: 0x060040DF RID: 16607 RVA: 0x0012C694 File Offset: 0x0012A894
		public DictionaryEntry Entry
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return new DictionaryEntry(keyValuePair.Key, keyValuePair.Value);
			}
		}

		// Token: 0x1700067D RID: 1661
		// (get) Token: 0x060040E0 RID: 16608 RVA: 0x0012C6C0 File Offset: 0x0012A8C0
		public object Key
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return keyValuePair.Key;
			}
		}

		// Token: 0x1700067E RID: 1662
		// (get) Token: 0x060040E1 RID: 16609 RVA: 0x0012C6E0 File Offset: 0x0012A8E0
		public object Value
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return keyValuePair.Value;
			}
		}

		// Token: 0x060040E2 RID: 16610 RVA: 0x0012C700 File Offset: 0x0012A900
		public OrderedDictionaryEnumerator(IEnumerator<KeyValuePair<string, JsonData>> enumerator)
		{
			this.list_enumerator = enumerator;
		}

		// Token: 0x060040E3 RID: 16611 RVA: 0x0012C70F File Offset: 0x0012A90F
		public bool MoveNext()
		{
			return this.list_enumerator.MoveNext();
		}

		// Token: 0x060040E4 RID: 16612 RVA: 0x0012C71C File Offset: 0x0012A91C
		public void Reset()
		{
			this.list_enumerator.Reset();
		}

		// Token: 0x040043D9 RID: 17369
		private IEnumerator<KeyValuePair<string, JsonData>> list_enumerator;
	}
}
