using System;
using UnityEngine;

// Token: 0x02000758 RID: 1880
[Serializable]
public struct FrameStamp
{
	// Token: 0x170004AF RID: 1199
	// (get) Token: 0x06002EF1 RID: 12017 RVA: 0x000EAF85 File Offset: 0x000E9185
	public int framesElapsed
	{
		get
		{
			return Time.frameCount - this._lastFrame;
		}
	}

	// Token: 0x06002EF2 RID: 12018 RVA: 0x000EAF94 File Offset: 0x000E9194
	public static FrameStamp Now()
	{
		return new FrameStamp
		{
			_lastFrame = Time.frameCount
		};
	}

	// Token: 0x06002EF3 RID: 12019 RVA: 0x000EAFB6 File Offset: 0x000E91B6
	public override string ToString()
	{
		return string.Format("{0} frames elapsed", this.framesElapsed);
	}

	// Token: 0x06002EF4 RID: 12020 RVA: 0x000EAFCD File Offset: 0x000E91CD
	public override int GetHashCode()
	{
		return StaticHash.Compute(this._lastFrame);
	}

	// Token: 0x06002EF5 RID: 12021 RVA: 0x000EAFDA File Offset: 0x000E91DA
	public static implicit operator int(FrameStamp fs)
	{
		return fs.framesElapsed;
	}

	// Token: 0x06002EF6 RID: 12022 RVA: 0x000EAFE4 File Offset: 0x000E91E4
	public static implicit operator FrameStamp(int framesElapsed)
	{
		return new FrameStamp
		{
			_lastFrame = Time.frameCount - framesElapsed
		};
	}

	// Token: 0x04003561 RID: 13665
	private int _lastFrame;
}
