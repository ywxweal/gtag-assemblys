using System;

// Token: 0x0200083C RID: 2108
internal struct PlayerAgeGateWarningStatus
{
	// Token: 0x04003A5B RID: 14939
	public string header;

	// Token: 0x04003A5C RID: 14940
	public string body;

	// Token: 0x04003A5D RID: 14941
	public string leftButtonText;

	// Token: 0x04003A5E RID: 14942
	public string rightButtonText;

	// Token: 0x04003A5F RID: 14943
	public WarningButtonResult leftButtonResult;

	// Token: 0x04003A60 RID: 14944
	public WarningButtonResult rightButtonResult;

	// Token: 0x04003A61 RID: 14945
	public bool showImage;

	// Token: 0x04003A62 RID: 14946
	public Action onLeftButtonPressedAction;

	// Token: 0x04003A63 RID: 14947
	public Action onRightButtonPressedAction;
}
