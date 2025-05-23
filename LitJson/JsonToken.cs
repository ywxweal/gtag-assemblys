using System;

namespace LitJson
{
	// Token: 0x02000A9A RID: 2714
	public enum JsonToken
	{
		// Token: 0x04004411 RID: 17425
		None,
		// Token: 0x04004412 RID: 17426
		ObjectStart,
		// Token: 0x04004413 RID: 17427
		PropertyName,
		// Token: 0x04004414 RID: 17428
		ObjectEnd,
		// Token: 0x04004415 RID: 17429
		ArrayStart,
		// Token: 0x04004416 RID: 17430
		ArrayEnd,
		// Token: 0x04004417 RID: 17431
		Int,
		// Token: 0x04004418 RID: 17432
		Long,
		// Token: 0x04004419 RID: 17433
		Double,
		// Token: 0x0400441A RID: 17434
		String,
		// Token: 0x0400441B RID: 17435
		Boolean,
		// Token: 0x0400441C RID: 17436
		Null
	}
}
