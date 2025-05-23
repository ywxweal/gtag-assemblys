using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02000EBE RID: 3774
	[WeaverGenerated]
	[Serializable]
	internal class UnityDictionarySurrogate@ReaderWriter@Fusion_NetworkString`1<Fusion__32>@ReaderWriter@Fusion_NetworkString : UnityDictionarySurrogate<NetworkString<_32>, ReaderWriter@Fusion_NetworkString, NetworkString<_32>, ReaderWriter@Fusion_NetworkString>
	{
		// Token: 0x1700091E RID: 2334
		// (get) Token: 0x06005DBB RID: 23995 RVA: 0x001CD29D File Offset: 0x001CB49D
		// (set) Token: 0x06005DBC RID: 23996 RVA: 0x001CD2A5 File Offset: 0x001CB4A5
		[WeaverGenerated]
		public override SerializableDictionary<NetworkString<_32>, NetworkString<_32>> DataProperty
		{
			[WeaverGenerated]
			get
			{
				return this.Data;
			}
			[WeaverGenerated]
			set
			{
				this.Data = value;
			}
		}

		// Token: 0x06005DBD RID: 23997 RVA: 0x001CD2AE File Offset: 0x001CB4AE
		[WeaverGenerated]
		public UnityDictionarySurrogate@ReaderWriter@Fusion_NetworkString`1<Fusion__32>@ReaderWriter@Fusion_NetworkString()
		{
		}

		// Token: 0x04006282 RID: 25218
		[WeaverGenerated]
		public SerializableDictionary<NetworkString<_32>, NetworkString<_32>> Data = SerializableDictionary.Create<NetworkString<_32>, NetworkString<_32>>();
	}
}
