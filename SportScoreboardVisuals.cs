using System;
using GorillaTag.Sports;
using UnityEngine;

// Token: 0x020006B3 RID: 1715
public class SportScoreboardVisuals : MonoBehaviour
{
	// Token: 0x06002AD2 RID: 10962 RVA: 0x000D252C File Offset: 0x000D072C
	private void Awake()
	{
		SportScoreboard.Instance.RegisterTeamVisual(this.TeamIndex, this);
	}

	// Token: 0x04002FC2 RID: 12226
	[SerializeField]
	public MaterialUVOffsetListSetter score1s;

	// Token: 0x04002FC3 RID: 12227
	[SerializeField]
	public MaterialUVOffsetListSetter score10s;

	// Token: 0x04002FC4 RID: 12228
	[SerializeField]
	private int TeamIndex;
}
