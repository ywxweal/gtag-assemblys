using System;

// Token: 0x0200017B RID: 379
[Flags]
public enum GestureNodeFlags : uint
{
	// Token: 0x04000B58 RID: 2904
	None = 0U,
	// Token: 0x04000B59 RID: 2905
	HandLeft = 1U,
	// Token: 0x04000B5A RID: 2906
	HandRight = 2U,
	// Token: 0x04000B5B RID: 2907
	HandOpen = 4U,
	// Token: 0x04000B5C RID: 2908
	HandClosed = 8U,
	// Token: 0x04000B5D RID: 2909
	DigitOpen = 16U,
	// Token: 0x04000B5E RID: 2910
	DigitClosed = 32U,
	// Token: 0x04000B5F RID: 2911
	DigitBent = 64U,
	// Token: 0x04000B60 RID: 2912
	TowardFace = 128U,
	// Token: 0x04000B61 RID: 2913
	AwayFromFace = 256U,
	// Token: 0x04000B62 RID: 2914
	AxisWorldUp = 512U,
	// Token: 0x04000B63 RID: 2915
	AxisWorldDown = 1024U
}
