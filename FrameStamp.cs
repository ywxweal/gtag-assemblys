using System;
using UnityEngine;

// Token: 0x02000758 RID: 1880
[Serializable]
public struct FrameStamp
{
	// Token: 0x170004AF RID: 1199
	// (get) Token: 0x06002EF2 RID: 12018 RVA: 0x000EB029 File Offset: 0x000E9229
	public int framesElapsed
	{
		get
		{
			return Time.frameCount - this._lastFrame;
		}
	}

	// Token: 0x06002EF3 RID: 12019 RVA: 0x000EB038 File Offset: 0x000E9238
	public static FrameStamp Now()
	{
		return new FrameStamp
		{
			_lastFrame = Time.frameCount
		};
	}

	// Token: 0x06002EF4 RID: 12020 RVA: 0x000EB05A File Offset: 0x000E925A
	public override string ToString()
	{
		return string.Format("{0} frames elapsed", this.framesElapsed);
	}

	// Token: 0x06002EF5 RID: 12021 RVA: 0x000EB071 File Offset: 0x000E9271
	public override int GetHashCode()
	{
		return StaticHash.Compute(this._lastFrame);
	}

	// Token: 0x06002EF6 RID: 12022 RVA: 0x000EB07E File Offset: 0x000E927E
	public static implicit operator int(FrameStamp fs)
	{
		return fs.framesElapsed;
	}

	// Token: 0x06002EF7 RID: 12023 RVA: 0x000EB088 File Offset: 0x000E9288
	public static implicit operator FrameStamp(int framesElapsed)
	{
		return new FrameStamp
		{
			_lastFrame = Time.frameCount - framesElapsed
		};
	}

	// Token: 0x04003563 RID: 13667
	private int _lastFrame;
}
