using System;
using GorillaNetworking;

// Token: 0x020006F5 RID: 1781
public class UnlockCompButton : GorillaPressableButton
{
	// Token: 0x06002C59 RID: 11353 RVA: 0x000DACA9 File Offset: 0x000D8EA9
	public override void Start()
	{
		this.initialized = false;
	}

	// Token: 0x06002C5A RID: 11354 RVA: 0x000DACB4 File Offset: 0x000D8EB4
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

	// Token: 0x06002C5B RID: 11355 RVA: 0x000DAD0C File Offset: 0x000D8F0C
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

	// Token: 0x040032A3 RID: 12963
	public string gameMode;

	// Token: 0x040032A4 RID: 12964
	private bool initialized;
}
