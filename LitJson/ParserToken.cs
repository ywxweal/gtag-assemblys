using System;

namespace LitJson
{
	// Token: 0x02000AA2 RID: 2722
	internal enum ParserToken
	{
		// Token: 0x04004454 RID: 17492
		None = 65536,
		// Token: 0x04004455 RID: 17493
		Number,
		// Token: 0x04004456 RID: 17494
		True,
		// Token: 0x04004457 RID: 17495
		False,
		// Token: 0x04004458 RID: 17496
		Null,
		// Token: 0x04004459 RID: 17497
		CharSeq,
		// Token: 0x0400445A RID: 17498
		Char,
		// Token: 0x0400445B RID: 17499
		Text,
		// Token: 0x0400445C RID: 17500
		Object,
		// Token: 0x0400445D RID: 17501
		ObjectPrime,
		// Token: 0x0400445E RID: 17502
		Pair,
		// Token: 0x0400445F RID: 17503
		PairRest,
		// Token: 0x04004460 RID: 17504
		Array,
		// Token: 0x04004461 RID: 17505
		ArrayPrime,
		// Token: 0x04004462 RID: 17506
		Value,
		// Token: 0x04004463 RID: 17507
		ValueRest,
		// Token: 0x04004464 RID: 17508
		String,
		// Token: 0x04004465 RID: 17509
		End,
		// Token: 0x04004466 RID: 17510
		Epsilon
	}
}
