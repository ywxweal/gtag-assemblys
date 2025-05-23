using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020000FA RID: 250
public class HorseStickNoiseMaker : MonoBehaviour
{
	// Token: 0x0600063A RID: 1594 RVA: 0x00023C4C File Offset: 0x00021E4C
	protected void OnEnable()
	{
		if (!this.gorillaPlayerXform && !base.transform.TryFindByPath(this.gorillaPlayerXform_path, out this.gorillaPlayerXform, false))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"HorseStickNoiseMaker: DEACTIVATING! Could not find gorillaPlayerXform using path: \"",
				this.gorillaPlayerXform_path,
				"\"\nThis component's transform path: \"",
				base.transform.GetPath(),
				"\""
			}));
			base.gameObject.SetActive(false);
			return;
		}
		this.oldPos = this.gorillaPlayerXform.position;
		this.distElapsed = 0f;
		this.timeSincePlay = 0f;
	}

	// Token: 0x0600063B RID: 1595 RVA: 0x00023CF4 File Offset: 0x00021EF4
	protected void LateUpdate()
	{
		Vector3 position = this.gorillaPlayerXform.position;
		Vector3 vector = position - this.oldPos;
		this.distElapsed += vector.magnitude;
		this.timeSincePlay += Time.deltaTime;
		this.oldPos = position;
		if (this.distElapsed >= this.metersPerClip && this.timeSincePlay >= this.minSecBetweenClips)
		{
			this.soundBankPlayer.Play();
			this.distElapsed = 0f;
			this.timeSincePlay = 0f;
			if (this.particleFX != null)
			{
				this.particleFX.Play();
			}
		}
	}

	// Token: 0x04000763 RID: 1891
	[Tooltip("Meters the object should traverse between playing a provided audio clip.")]
	public float metersPerClip = 4f;

	// Token: 0x04000764 RID: 1892
	[Tooltip("Number of seconds that must elapse before playing another audio clip.")]
	public float minSecBetweenClips = 1.5f;

	// Token: 0x04000765 RID: 1893
	public SoundBankPlayer soundBankPlayer;

	// Token: 0x04000766 RID: 1894
	[Tooltip("Transform assigned in Gorilla Player Networked Prefab to the Gorilla Player Networked parent to keep track of distance traveled.")]
	public Transform gorillaPlayerXform;

	// Token: 0x04000767 RID: 1895
	[Delayed]
	public string gorillaPlayerXform_path;

	// Token: 0x04000768 RID: 1896
	[Tooltip("Optional particle FX to spawn when sound plays")]
	public ParticleSystem particleFX;

	// Token: 0x04000769 RID: 1897
	private Vector3 oldPos;

	// Token: 0x0400076A RID: 1898
	private float timeSincePlay;

	// Token: 0x0400076B RID: 1899
	private float distElapsed;
}
