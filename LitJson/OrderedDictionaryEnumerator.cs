using System;
using System.Collections;
using System.Collections.Generic;

namespace LitJson
{
	// Token: 0x02000A8C RID: 2700
	internal class OrderedDictionaryEnumerator : IDictionaryEnumerator, IEnumerator
	{
		// Token: 0x1700067B RID: 1659
		// (get) Token: 0x060040DF RID: 16607 RVA: 0x0012C75E File Offset: 0x0012A95E
		public object Current
		{
			get
			{
				return this.Entry;
			}
		}

		// Token: 0x1700067C RID: 1660
		// (get) Token: 0x060040E0 RID: 16608 RVA: 0x0012C76C File Offset: 0x0012A96C
		public DictionaryEntry Entry
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return new DictionaryEntry(keyValuePair.Key, keyValuePair.Value);
			}
		}

		// Token: 0x1700067D RID: 1661
		// (get) Token: 0x060040E1 RID: 16609 RVA: 0x0012C798 File Offset: 0x0012A998
		public object Key
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return keyValuePair.Key;
			}
		}

		// Token: 0x1700067E RID: 1662
		// (get) Token: 0x060040E2 RID: 16610 RVA: 0x0012C7B8 File Offset: 0x0012A9B8
		public object Value
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return keyValuePair.Value;
			}
		}

		// Token: 0x060040E3 RID: 16611 RVA: 0x0012C7D8 File Offset: 0x0012A9D8
		public OrderedDictionaryEnumerator(IEnumerator<KeyValuePair<string, JsonData>> enumerator)
		{
			this.list_enumerator = enumerator;
		}

		// Token: 0x060040E4 RID: 16612 RVA: 0x0012C7E7 File Offset: 0x0012A9E7
		public bool MoveNext()
		{
			return this.list_enumerator.MoveNext();
		}

		// Token: 0x060040E5 RID: 16613 RVA: 0x0012C7F4 File Offset: 0x0012A9F4
		public void Reset()
		{
			this.list_enumerator.Reset();
		}

		// Token: 0x040043DA RID: 17370
		private IEnumerator<KeyValuePair<string, JsonData>> list_enumerator;
	}
}
