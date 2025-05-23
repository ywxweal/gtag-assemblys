using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02000ED0 RID: 3792
	[WeaverGenerated]
	[Serializable]
	internal class UnityDictionarySurrogate@ReaderWriter@System_Int32@ReaderWriter@System_Int32 : UnityDictionarySurrogate<int, ReaderWriter@System_Int32, int, ReaderWriter@System_Int32>
	{
		// Token: 0x17000925 RID: 2341
		// (get) Token: 0x06005DE2 RID: 24034 RVA: 0x001CD4DD File Offset: 0x001CB6DD
		// (set) Token: 0x06005DE3 RID: 24035 RVA: 0x001CD4E5 File Offset: 0x001CB6E5
		[WeaverGenerated]
		public override SerializableDictionary<int, int> DataProperty
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

		// Token: 0x06005DE4 RID: 24036 RVA: 0x001CD4EE File Offset: 0x001CB6EE
		[WeaverGenerated]
		public UnityDictionarySurrogate@ReaderWriter@System_Int32@ReaderWriter@System_Int32()
		{
		}

		// Token: 0x040064A8 RID: 25768
		[WeaverGenerated]
		public SerializableDictionary<int, int> Data = SerializableDictionary.Create<int, int>();
	}
}
