using System;
using UnityEngine;

// Token: 0x020001F9 RID: 505
public class RaceCheckpointManager : MonoBehaviour
{
	// Token: 0x06000BAE RID: 2990 RVA: 0x0003E2A4 File Offset: 0x0003C4A4
	private void Start()
	{
		this.visual = base.GetComponent<RaceVisual>();
		for (int i = 0; i < this.checkpoints.Length; i++)
		{
			this.checkpoints[i].Init(this, i);
		}
		this.OnRaceEnd();
	}

	// Token: 0x06000BAF RID: 2991 RVA: 0x0003E2E8 File Offset: 0x0003C4E8
	public void OnRaceStart()
	{
		for (int i = 0; i < this.checkpoints.Length; i++)
		{
			this.checkpoints[i].SetIsCorrectCheckpoint(i == 0);
		}
	}

	// Token: 0x06000BB0 RID: 2992 RVA: 0x0003E31C File Offset: 0x0003C51C
	public void OnRaceEnd()
	{
		for (int i = 0; i < this.checkpoints.Length; i++)
		{
			this.checkpoints[i].SetIsCorrectCheckpoint(false);
		}
	}

	// Token: 0x06000BB1 RID: 2993 RVA: 0x0003E34A File Offset: 0x0003C54A
	public void OnCheckpointReached(int index, SoundBankPlayer checkpointSound)
	{
		this.checkpoints[index].SetIsCorrectCheckpoint(false);
		this.checkpoints[(index + 1) % this.checkpoints.Length].SetIsCorrectCheckpoint(true);
		this.visual.OnCheckpointPassed(index, checkpointSound);
	}

	// Token: 0x06000BB2 RID: 2994 RVA: 0x0003E380 File Offset: 0x0003C580
	public bool IsPlayerNearCheckpoint(VRRig player, int checkpointIdx)
	{
		return checkpointIdx >= 0 && checkpointIdx < this.checkpoints.Length && player.IsPositionInRange(this.checkpoints[checkpointIdx].transform.position, 6f);
	}

	// Token: 0x04000E3F RID: 3647
	[SerializeField]
	private RaceCheckpoint[] checkpoints;

	// Token: 0x04000E40 RID: 3648
	private RaceVisual visual;
}
