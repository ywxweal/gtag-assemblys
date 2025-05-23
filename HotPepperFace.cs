using System;
using UnityEngine;

// Token: 0x0200045F RID: 1119
public class HotPepperFace : MonoBehaviour
{
	// Token: 0x06001B7A RID: 7034 RVA: 0x00087028 File Offset: 0x00085228
	public void PlayFX(float delay)
	{
		if (delay < 0f)
		{
			this.PlayFX();
			return;
		}
		base.Invoke("PlayFX", delay);
	}

	// Token: 0x06001B7B RID: 7035 RVA: 0x00087048 File Offset: 0x00085248
	public void PlayFX()
	{
		this._faceMesh.SetActive(true);
		this._thermalSourceVolume.SetActive(true);
		this._fireFX.Play();
		this._flameSpeaker.GTPlay();
		this._breathSpeaker.GTPlay();
		base.Invoke("StopFX", this._effectLength);
	}

	// Token: 0x06001B7C RID: 7036 RVA: 0x0008709F File Offset: 0x0008529F
	public void StopFX()
	{
		this._faceMesh.SetActive(false);
		this._thermalSourceVolume.SetActive(false);
		this._fireFX.Stop();
		this._flameSpeaker.GTStop();
		this._breathSpeaker.GTStop();
	}

	// Token: 0x04001E73 RID: 7795
	[SerializeField]
	private GameObject _faceMesh;

	// Token: 0x04001E74 RID: 7796
	[SerializeField]
	private ParticleSystem _fireFX;

	// Token: 0x04001E75 RID: 7797
	[SerializeField]
	private AudioSource _flameSpeaker;

	// Token: 0x04001E76 RID: 7798
	[SerializeField]
	private AudioSource _breathSpeaker;

	// Token: 0x04001E77 RID: 7799
	[SerializeField]
	private float _effectLength = 1.5f;

	// Token: 0x04001E78 RID: 7800
	[SerializeField]
	private GameObject _thermalSourceVolume;
}
