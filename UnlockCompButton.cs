using System;
using GorillaNetworking;

// Token: 0x020006F5 RID: 1781
public class UnlockCompButton : GorillaPressableButton
{
	// Token: 0x06002C58 RID: 11352 RVA: 0x000DAC05 File Offset: 0x000D8E05
	public override void Start()
	{
		this.initialized = false;
	}

	// Token: 0x06002C59 RID: 11353 RVA: 0x000DAC10 File Offset: 0x000D8E10
	public void Update()
	{
		if (this.testPress)
		{
			this.testPress = false;
			this.ButtonActivation();
		}
		if (!this.initialized && GorillaComputer.instance != null)
		{
			this.isOn = GorillaComputer.instance.allowedInCompetitive;
			this.UpdateColor();
			this.initialized = true;
		}
	}

	// Token: 0x06002C5A RID: 11354 RVA: 0x000DAC68 File Offset: 0x000D8E68
	public override void ButtonActivation()
	{
		if (!this.isOn)
		{
			base.ButtonActivation();
			GorillaComputer.instance.CompQueueUnlockButtonPress();
			this.isOn = true;
			this.UpdateColor();
		}
	}

	// Token: 0x040032A1 RID: 12961
	public string gameMode;

	// Token: 0x040032A2 RID: 12962
	private bool initialized;
}
