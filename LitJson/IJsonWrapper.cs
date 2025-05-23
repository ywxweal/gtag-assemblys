using System;
using System.Collections;
using System.Collections.Specialized;

namespace LitJson
{
	// Token: 0x02000A8A RID: 2698
	public interface IJsonWrapper : IList, ICollection, IEnumerable, IOrderedDictionary, IDictionary
	{
		// Token: 0x17000657 RID: 1623
		// (get) Token: 0x0600406D RID: 16493
		bool IsArray { get; }

		// Token: 0x17000658 RID: 1624
		// (get) Token: 0x0600406E RID: 16494
		bool IsBoolean { get; }

		// Token: 0x17000659 RID: 1625
		// (get) Token: 0x0600406F RID: 16495
		bool IsDouble { get; }

		// Token: 0x1700065A RID: 1626
		// (get) Token: 0x06004070 RID: 16496
		bool IsInt { get; }

		// Token: 0x1700065B RID: 1627
		// (get) Token: 0x06004071 RID: 16497
		bool IsLong { get; }

		// Token: 0x1700065C RID: 1628
		// (get) Token: 0x06004072 RID: 16498
		bool IsObject { get; }

		// Token: 0x1700065D RID: 1629
		// (get) Token: 0x06004073 RID: 16499
		bool IsString { get; }

		// Token: 0x06004074 RID: 16500
		bool GetBoolean();

		// Token: 0x06004075 RID: 16501
		double GetDouble();

		// Token: 0x06004076 RID: 16502
		int GetInt();

		// Token: 0x06004077 RID: 16503
		JsonType GetJsonType();

		// Token: 0x06004078 RID: 16504
		long GetLong();

		// Token: 0x06004079 RID: 16505
		string GetString();

		// Token: 0x0600407A RID: 16506
		void SetBoolean(bool val);

		// Token: 0x0600407B RID: 16507
		void SetDouble(double val);

		// Token: 0x0600407C RID: 16508
		void SetInt(int val);

		// Token: 0x0600407D RID: 16509
		void SetJsonType(JsonType type);

		// Token: 0x0600407E RID: 16510
		void SetLong(long val);

		// Token: 0x0600407F RID: 16511
		void SetString(string val);

		// Token: 0x06004080 RID: 16512
		string ToJson();

		// Token: 0x06004081 RID: 16513
		void ToJson(JsonWriter writer);
	}
}
