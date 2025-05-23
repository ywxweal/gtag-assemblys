using System;
using GorillaTag.Sports;
using UnityEngine;

// Token: 0x020006B3 RID: 1715
public class SportScoreboardVisuals : MonoBehaviour
{
	// Token: 0x06002AD3 RID: 10963 RVA: 0x000D25D0 File Offset: 0x000D07D0
	private void Awake()
	{
		SportScoreboard.Instance.RegisterTeamVisual(this.TeamIndex, this);
	}

	// Token: 0x04002FC4 RID: 12228
	[SerializeField]
	public MaterialUVOffsetListSetter score1s;

	// Token: 0x04002FC5 RID: 12229
	[SerializeField]
	public MaterialUVOffsetListSetter score10s;

	// Token: 0x04002FC6 RID: 12230
	[SerializeField]
	private int TeamIndex;
}
