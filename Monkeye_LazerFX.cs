using System;
using UnityEngine;

// Token: 0x020000BE RID: 190
public class Monkeye_LazerFX : MonoBehaviour
{
	// Token: 0x060004C6 RID: 1222 RVA: 0x0001BCEA File Offset: 0x00019EEA
	private void Awake()
	{
		base.enabled = false;
	}

	// Token: 0x060004C7 RID: 1223 RVA: 0x0001BCF4 File Offset: 0x00019EF4
	public void EnableLazer(Transform[] eyes_, VRRig rig_)
	{
		if (rig_ == this.rig)
		{
			return;
		}
		this.eyeBones = eyes_;
		this.rig = rig_;
		base.enabled = true;
		LineRenderer[] array = this.lines;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].positionCount = 2;
		}
	}

	// Token: 0x060004C8 RID: 1224 RVA: 0x0001BD44 File Offset: 0x00019F44
	public void DisableLazer()
	{
		if (base.enabled)
		{
			base.enabled = false;
			LineRenderer[] array = this.lines;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].positionCount = 0;
			}
		}
	}

	// Token: 0x060004C9 RID: 1225 RVA: 0x0001BD80 File Offset: 0x00019F80
	private void Update()
	{
		for (int i = 0; i < this.lines.Length; i++)
		{
			this.lines[i].SetPosition(0, this.eyeBones[i].transform.position);
			this.lines[i].SetPosition(1, this.rig.transform.position);
		}
	}

	// Token: 0x04000595 RID: 1429
	private Transform[] eyeBones;

	// Token: 0x04000596 RID: 1430
	private VRRig rig;

	// Token: 0x04000597 RID: 1431
	public LineRenderer[] lines;
}
