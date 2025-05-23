using System;
using UnityEngine;

// Token: 0x0200043C RID: 1084
public class RandomAudioStart : MonoBehaviour, IBuildValidation
{
	// Token: 0x06001ABC RID: 6844 RVA: 0x00082DBD File Offset: 0x00080FBD
	public bool BuildValidationCheck()
	{
		if (this.audioSource == null)
		{
			Debug.LogError("audio source is missing for RandomAudioStart, it won't work correctly", base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x06001ABD RID: 6845 RVA: 0x00082DE0 File Offset: 0x00080FE0
	private void OnEnable()
	{
		this.audioSource.time = Random.value * this.audioSource.clip.length;
	}

	// Token: 0x06001ABE RID: 6846 RVA: 0x00082E03 File Offset: 0x00081003
	[ContextMenu("Assign Audio Source")]
	public void AssignAudioSource()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x04001DCA RID: 7626
	public AudioSource audioSource;
}
