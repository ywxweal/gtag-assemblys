using System;

// Token: 0x020006F4 RID: 1780
public class SoundPostMuteButton : GorillaPressableButton
{
	// Token: 0x06002C57 RID: 11351 RVA: 0x000DAC78 File Offset: 0x000D8E78
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		SynchedMusicController[] array = this.musicControllers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].MuteAudio(this);
		}
	}

	// Token: 0x040032A2 RID: 12962
	public SynchedMusicController[] musicControllers;
}
