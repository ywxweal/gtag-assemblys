using System;
using UnityEngine;

// Token: 0x020001F7 RID: 503
public class PositionVolumeModifier : MonoBehaviour
{
	// Token: 0x06000BA8 RID: 2984 RVA: 0x0003E215 File Offset: 0x0003C415
	public void OnTriggerStay(Collider other)
	{
		this.audioToMod.isModified = true;
	}

	// Token: 0x04000E36 RID: 3638
	public TimeOfDayDependentAudio audioToMod;
}
