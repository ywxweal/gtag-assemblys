using System;
using System.Collections.Generic;

// Token: 0x02000244 RID: 580
[Serializable]
public class ScenePerformanceData
{
	// Token: 0x06000D51 RID: 3409 RVA: 0x00045B58 File Offset: 0x00043D58
	public ScenePerformanceData(string mapName, int gorillaCount, int droppedFrames, int msHigh, int medianMS, int medianFPS, int medianDrawCalls, List<int> msCaptures)
	{
		this._mapName = mapName;
		this._gorillaCount = gorillaCount;
		this._droppedFrames = droppedFrames;
		this._msHigh = msHigh;
		this._medianMS = medianMS;
		this._medianFPS = medianFPS;
		this._medianDrawCallCount = medianDrawCalls;
		this._msCaptures = new List<int>(msCaptures);
	}

	// Token: 0x040010DC RID: 4316
	public string _mapName;

	// Token: 0x040010DD RID: 4317
	public int _gorillaCount;

	// Token: 0x040010DE RID: 4318
	public int _droppedFrames;

	// Token: 0x040010DF RID: 4319
	public int _msHigh;

	// Token: 0x040010E0 RID: 4320
	public int _medianMS;

	// Token: 0x040010E1 RID: 4321
	public int _medianFPS;

	// Token: 0x040010E2 RID: 4322
	public int _medianDrawCallCount;

	// Token: 0x040010E3 RID: 4323
	public List<int> _msCaptures;
}
