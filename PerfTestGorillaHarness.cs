using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x02000246 RID: 582
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST")]
public class PerfTestGorillaHarness : MonoBehaviour
{
	// Token: 0x06000D53 RID: 3411 RVA: 0x00045BB0 File Offset: 0x00043DB0
	private void Awake()
	{
		foreach (PerfTestGorillaSlot perfTestGorillaSlot in base.GetComponentsInChildren<PerfTestGorillaSlot>())
		{
			if (perfTestGorillaSlot.slotType == PerfTestGorillaSlot.SlotType.VR_PLAYER)
			{
				this._vrSlot = perfTestGorillaSlot;
			}
			else
			{
				this.dummySlots.Add(perfTestGorillaSlot);
			}
		}
	}

	// Token: 0x06000D54 RID: 3412 RVA: 0x00045BF4 File Offset: 0x00043DF4
	private void Update()
	{
		if (!this._isRecording)
		{
			return;
		}
		foreach (PerfTestGorillaSlot perfTestGorillaSlot in this.dummySlots)
		{
			float num = perfTestGorillaSlot.localStartPosition.y + Mathf.Sin(Time.time * this.bounceSpeed) * this.bounceAmplitude;
			perfTestGorillaSlot.transform.localPosition = new Vector3(perfTestGorillaSlot.localStartPosition.x, num, perfTestGorillaSlot.localStartPosition.z);
		}
	}

	// Token: 0x06000D55 RID: 3413 RVA: 0x00045C98 File Offset: 0x00043E98
	public void StartRecording()
	{
		this._isRecording = true;
	}

	// Token: 0x06000D56 RID: 3414 RVA: 0x00045CA4 File Offset: 0x00043EA4
	public void StopRecording()
	{
		foreach (PerfTestGorillaSlot perfTestGorillaSlot in this.dummySlots)
		{
			perfTestGorillaSlot.transform.localPosition = perfTestGorillaSlot.localStartPosition;
		}
		this._isRecording = false;
	}

	// Token: 0x040010E5 RID: 4325
	public PerfTestGorillaSlot _vrSlot;

	// Token: 0x040010E6 RID: 4326
	public List<PerfTestGorillaSlot> dummySlots = new List<PerfTestGorillaSlot>(9);

	// Token: 0x040010E7 RID: 4327
	[OnEnterPlay_Set(false)]
	private bool _isRecording;

	// Token: 0x040010E8 RID: 4328
	private float _nextRandomMoveTime;

	// Token: 0x040010E9 RID: 4329
	private float bounceSpeed = 5f;

	// Token: 0x040010EA RID: 4330
	private float bounceAmplitude = 0.5f;
}
