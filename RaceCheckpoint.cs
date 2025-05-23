using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020001F8 RID: 504
public class RaceCheckpoint : MonoBehaviour
{
	// Token: 0x06000BAA RID: 2986 RVA: 0x0003E223 File Offset: 0x0003C423
	public void Init(RaceCheckpointManager manager, int index)
	{
		this.manager = manager;
		this.checkpointIndex = index;
		this.SetIsCorrectCheckpoint(index == 0);
	}

	// Token: 0x06000BAB RID: 2987 RVA: 0x0003E23D File Offset: 0x0003C43D
	public void SetIsCorrectCheckpoint(bool isCorrect)
	{
		this.isCorrect = isCorrect;
		this.banner.sharedMaterial = (isCorrect ? this.activeCheckpointMat : this.wrongCheckpointMat);
	}

	// Token: 0x06000BAC RID: 2988 RVA: 0x0003E262 File Offset: 0x0003C462
	private void OnTriggerEnter(Collider other)
	{
		if (other != GTPlayer.Instance.headCollider)
		{
			return;
		}
		if (this.isCorrect)
		{
			this.manager.OnCheckpointReached(this.checkpointIndex, this.checkpointSound);
			return;
		}
		this.wrongCheckpointSound.Play();
	}

	// Token: 0x04000E37 RID: 3639
	[SerializeField]
	private MeshRenderer banner;

	// Token: 0x04000E38 RID: 3640
	[SerializeField]
	private Material activeCheckpointMat;

	// Token: 0x04000E39 RID: 3641
	[SerializeField]
	private Material wrongCheckpointMat;

	// Token: 0x04000E3A RID: 3642
	[SerializeField]
	private SoundBankPlayer checkpointSound;

	// Token: 0x04000E3B RID: 3643
	[SerializeField]
	private SoundBankPlayer wrongCheckpointSound;

	// Token: 0x04000E3C RID: 3644
	private RaceCheckpointManager manager;

	// Token: 0x04000E3D RID: 3645
	private int checkpointIndex;

	// Token: 0x04000E3E RID: 3646
	private bool isCorrect;
}
