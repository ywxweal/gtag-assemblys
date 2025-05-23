using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000A0F RID: 2575
public class ThrowableBugBeaconActivation : MonoBehaviour
{
	// Token: 0x06003D84 RID: 15748 RVA: 0x001241DC File Offset: 0x001223DC
	private void Awake()
	{
		this.tbb = base.GetComponent<ThrowableBugBeacon>();
	}

	// Token: 0x06003D85 RID: 15749 RVA: 0x001241EA File Offset: 0x001223EA
	private void OnEnable()
	{
		base.StartCoroutine(this.SendSignals());
	}

	// Token: 0x06003D86 RID: 15750 RVA: 0x00004F01 File Offset: 0x00003101
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06003D87 RID: 15751 RVA: 0x001241F9 File Offset: 0x001223F9
	private IEnumerator SendSignals()
	{
		uint count = 0U;
		while (this.signalCount == 0U || count < this.signalCount)
		{
			yield return new WaitForSeconds(Random.Range(this.minCallTime, this.maxCallTime));
			switch (this.mode)
			{
			case ThrowableBugBeaconActivation.ActivationMode.CALL:
				this.tbb.Call();
				break;
			case ThrowableBugBeaconActivation.ActivationMode.DISMISS:
				this.tbb.Dismiss();
				break;
			case ThrowableBugBeaconActivation.ActivationMode.LOCK:
				this.tbb.Lock();
				break;
			}
			uint num = count;
			count = num + 1U;
		}
		yield break;
	}

	// Token: 0x0400415C RID: 16732
	[SerializeField]
	private float minCallTime = 1f;

	// Token: 0x0400415D RID: 16733
	[SerializeField]
	private float maxCallTime = 5f;

	// Token: 0x0400415E RID: 16734
	[SerializeField]
	private uint signalCount;

	// Token: 0x0400415F RID: 16735
	[SerializeField]
	private ThrowableBugBeaconActivation.ActivationMode mode;

	// Token: 0x04004160 RID: 16736
	private ThrowableBugBeacon tbb;

	// Token: 0x02000A10 RID: 2576
	private enum ActivationMode
	{
		// Token: 0x04004162 RID: 16738
		CALL,
		// Token: 0x04004163 RID: 16739
		DISMISS,
		// Token: 0x04004164 RID: 16740
		LOCK
	}
}
