using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000173 RID: 371
public class ElfLauncherElf : MonoBehaviour
{
	// Token: 0x06000951 RID: 2385 RVA: 0x0003266C File Offset: 0x0003086C
	private void OnEnable()
	{
		base.StartCoroutine(this.ReturnToPoolAfterDelayCo());
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x0003267B File Offset: 0x0003087B
	private IEnumerator ReturnToPoolAfterDelayCo()
	{
		yield return new WaitForSeconds(this.destroyAfterDuration);
		ObjectPools.instance.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x0003268A File Offset: 0x0003088A
	private void OnCollisionEnter(Collision collision)
	{
		if (this.bounceAudioCoolingDownUntilTimestamp > Time.time)
		{
			return;
		}
		this.bounceAudio.Play();
		this.bounceAudioCoolingDownUntilTimestamp = Time.time + this.bounceAudioCooldownDuration;
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x000326B7 File Offset: 0x000308B7
	private void FixedUpdate()
	{
		this.rb.AddForce(base.transform.lossyScale.x * Physics.gravity, ForceMode.Acceleration);
	}

	// Token: 0x04000B35 RID: 2869
	[SerializeField]
	private Rigidbody rb;

	// Token: 0x04000B36 RID: 2870
	[SerializeField]
	private SoundBankPlayer bounceAudio;

	// Token: 0x04000B37 RID: 2871
	[SerializeField]
	private float bounceAudioCooldownDuration;

	// Token: 0x04000B38 RID: 2872
	[SerializeField]
	private float destroyAfterDuration;

	// Token: 0x04000B39 RID: 2873
	private float bounceAudioCoolingDownUntilTimestamp;
}
