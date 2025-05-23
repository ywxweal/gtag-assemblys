using System;

// Token: 0x020006F4 RID: 1780
public class SoundPostMuteButton : GorillaPressableButton
{
	// Token: 0x06002C56 RID: 11350 RVA: 0x000DABD4 File Offset: 0x000D8DD4
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		SynchedMusicController[] array = this.musicControllers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].MuteAudio(this);
		}
	}

	// Token: 0x040032A0 RID: 12960
	public SynchedMusicController[] musicControllers;
}
