using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000761 RID: 1889
[Serializable]
public struct GTSturdyEnum<TEnum> : ISerializationCallbackReceiver where TEnum : struct, Enum
{
	// Token: 0x170004B0 RID: 1200
	// (get) Token: 0x06002F0A RID: 12042 RVA: 0x000EB555 File Offset: 0x000E9755
	// (set) Token: 0x06002F0B RID: 12043 RVA: 0x000EB55D File Offset: 0x000E975D
	public TEnum Value { readonly get; private set; }

	// Token: 0x06002F0C RID: 12044 RVA: 0x000EB568 File Offset: 0x000E9768
	public static implicit operator GTSturdyEnum<TEnum>(TEnum value)
	{
		return new GTSturdyEnum<TEnum>
		{
			Value = value
		};
	}

	// Token: 0x06002F0D RID: 12045 RVA: 0x000EB586 File Offset: 0x000E9786
	public static implicit operator TEnum(GTSturdyEnum<TEnum> sturdyEnum)
	{
		return sturdyEnum.Value;
	}

	// Token: 0x06002F0E RID: 12046 RVA: 0x000EB590 File Offset: 0x000E9790
	public void OnBeforeSerialize()
	{
		EnumData<TEnum> shared = EnumData<TEnum>.Shared;
		if (!shared.IsBitMaskCompatible)
		{
			this.m_stringValuePairs = new GTSturdyEnum<TEnum>.EnumPair[1];
			GTSturdyEnum<TEnum>.EnumPair[] stringValuePairs = this.m_stringValuePairs;
			int num = 0;
			GTSturdyEnum<TEnum>.EnumPair enumPair = default(GTSturdyEnum<TEnum>.EnumPair);
			TEnum tenum = this.Value;
			enumPair.Name = tenum.ToString();
			enumPair.FallbackValue = this.Value;
			stringValuePairs[num] = enumPair;
			return;
		}
		long num2 = Convert.ToInt64(this.Value);
		if (num2 == 0L)
		{
			GTSturdyEnum<TEnum>.EnumPair[] array = new GTSturdyEnum<TEnum>.EnumPair[1];
			int num3 = 0;
			GTSturdyEnum<TEnum>.EnumPair enumPair = default(GTSturdyEnum<TEnum>.EnumPair);
			TEnum tenum = this.Value;
			enumPair.Name = tenum.ToString();
			enumPair.FallbackValue = this.Value;
			array[num3] = enumPair;
			this.m_stringValuePairs = array;
			return;
		}
		List<GTSturdyEnum<TEnum>.EnumPair> list = new List<GTSturdyEnum<TEnum>.EnumPair>(shared.Values.Length);
		for (int i = 0; i < shared.Values.Length; i++)
		{
			long num4 = shared.LongValues[i];
			if (num4 != 0L && (num2 & num4) == num4)
			{
				TEnum tenum2 = shared.Values[i];
				list.Add(new GTSturdyEnum<TEnum>.EnumPair
				{
					Name = tenum2.ToString(),
					FallbackValue = tenum2
				});
			}
		}
		this.m_stringValuePairs = list.ToArray();
	}

	// Token: 0x06002F0F RID: 12047 RVA: 0x000EB6D8 File Offset: 0x000E98D8
	public void OnAfterDeserialize()
	{
		EnumData<TEnum> shared = EnumData<TEnum>.Shared;
		if (this.m_stringValuePairs == null || this.m_stringValuePairs.Length == 0)
		{
			if (shared.IsBitMaskCompatible)
			{
				this.Value = (TEnum)((object)Enum.ToObject(typeof(TEnum), 0L));
				return;
			}
			this.Value = default(TEnum);
			return;
		}
		else
		{
			if (shared.IsBitMaskCompatible)
			{
				long num = 0L;
				foreach (GTSturdyEnum<TEnum>.EnumPair enumPair in this.m_stringValuePairs)
				{
					TEnum tenum;
					long num2;
					if (shared.NameToEnum.TryGetValue(enumPair.Name, out tenum))
					{
						num |= shared.EnumToLong[tenum];
					}
					else if (shared.EnumToLong.TryGetValue(enumPair.FallbackValue, out num2))
					{
						num |= num2;
					}
				}
				this.Value = (TEnum)((object)Enum.ToObject(typeof(TEnum), num));
				return;
			}
			GTSturdyEnum<TEnum>.EnumPair enumPair2 = this.m_stringValuePairs[0];
			TEnum tenum2;
			if (shared.NameToEnum.TryGetValue(enumPair2.Name, out tenum2))
			{
				this.Value = tenum2;
				return;
			}
			this.Value = enumPair2.FallbackValue;
			return;
		}
	}

	// Token: 0x0400359A RID: 13722
	[SerializeField]
	private GTSturdyEnum<TEnum>.EnumPair[] m_stringValuePairs;

	// Token: 0x02000762 RID: 1890
	[Serializable]
	private struct EnumPair
	{
		// Token: 0x0400359B RID: 13723
		public string Name;

		// Token: 0x0400359C RID: 13724
		public TEnum FallbackValue;
	}
}
