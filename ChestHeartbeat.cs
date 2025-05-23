using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200052D RID: 1325
public class ChestHeartbeat : MonoBehaviour
{
	// Token: 0x06002023 RID: 8227 RVA: 0x000A20FC File Offset: 0x000A02FC
	public void Update()
	{
		if (PhotonNetwork.InRoom)
		{
			if ((PhotonNetwork.ServerTimestamp > this.lastShot + this.millisMin || Mathf.Abs(PhotonNetwork.ServerTimestamp - this.lastShot) > 10000) && PhotonNetwork.ServerTimestamp % 1500 <= 10)
			{
				this.lastShot = PhotonNetwork.ServerTimestamp;
				this.audioSource.GTPlayOneShot(this.audioSource.clip, 1f);
				base.StartCoroutine(this.HeartBeat());
				return;
			}
		}
		else if ((Time.time * 1000f > (float)(this.lastShot + this.millisMin) || Mathf.Abs(Time.time * 1000f - (float)this.lastShot) > 10000f) && Time.time * 1000f % 1500f <= 10f)
		{
			this.lastShot = PhotonNetwork.ServerTimestamp;
			this.audioSource.GTPlayOneShot(this.audioSource.clip, 1f);
			base.StartCoroutine(this.HeartBeat());
		}
	}

	// Token: 0x06002024 RID: 8228 RVA: 0x000A220A File Offset: 0x000A040A
	private IEnumerator HeartBeat()
	{
		float startTime = Time.time;
		while (Time.time < startTime + this.endtime)
		{
			if (Time.time < startTime + this.minTime)
			{
				this.deltaTime = Time.time - startTime;
				this.scaleTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * this.heartMinSize, this.deltaTime / this.minTime);
			}
			else if (Time.time < startTime + this.maxTime)
			{
				this.deltaTime = Time.time - startTime - this.minTime;
				this.scaleTransform.localScale = Vector3.Lerp(Vector3.one * this.heartMinSize, Vector3.one * this.heartMaxSize, this.deltaTime / (this.maxTime - this.minTime));
			}
			else if (Time.time < startTime + this.endtime)
			{
				this.deltaTime = Time.time - startTime - this.maxTime;
				this.scaleTransform.localScale = Vector3.Lerp(Vector3.one * this.heartMaxSize, Vector3.one, this.deltaTime / (this.endtime - this.maxTime));
			}
			yield return new WaitForFixedUpdate();
		}
		yield break;
	}

	// Token: 0x04002432 RID: 9266
	public int millisToWait;

	// Token: 0x04002433 RID: 9267
	public int millisMin = 300;

	// Token: 0x04002434 RID: 9268
	public int lastShot;

	// Token: 0x04002435 RID: 9269
	public AudioSource audioSource;

	// Token: 0x04002436 RID: 9270
	public Transform scaleTransform;

	// Token: 0x04002437 RID: 9271
	private float deltaTime;

	// Token: 0x04002438 RID: 9272
	private float heartMinSize = 0.9f;

	// Token: 0x04002439 RID: 9273
	private float heartMaxSize = 1.2f;

	// Token: 0x0400243A RID: 9274
	private float minTime = 0.05f;

	// Token: 0x0400243B RID: 9275
	private float maxTime = 0.1f;

	// Token: 0x0400243C RID: 9276
	private float endtime = 0.25f;

	// Token: 0x0400243D RID: 9277
	private float currentTime;
}
