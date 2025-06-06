﻿using System;
using UnityEngine;

// Token: 0x020009FB RID: 2555
public class GrowUntilCollision : MonoBehaviour
{
	// Token: 0x06003D19 RID: 15641 RVA: 0x00122350 File Offset: 0x00120550
	private void Start()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		if (this.audioSource != null)
		{
			this.maxVolume = this.audioSource.volume;
			this.maxPitch = this.audioSource.pitch;
		}
		this.zero();
	}

	// Token: 0x06003D1A RID: 15642 RVA: 0x001223A0 File Offset: 0x001205A0
	private void zero()
	{
		base.transform.localScale = Vector3.one * this.initialRadius;
		if (this.audioSource != null)
		{
			this.audioSource.volume = 0f;
			this.audioSource.pitch = 1f;
		}
		this.timeSinceTrigger = 0f;
	}

	// Token: 0x06003D1B RID: 15643 RVA: 0x00122401 File Offset: 0x00120601
	private void OnTriggerEnter(Collider other)
	{
		this.tryToTrigger(base.transform.position, other.transform.position);
	}

	// Token: 0x06003D1C RID: 15644 RVA: 0x00122401 File Offset: 0x00120601
	private void OnTriggerExit(Collider other)
	{
		this.tryToTrigger(base.transform.position, other.transform.position);
	}

	// Token: 0x06003D1D RID: 15645 RVA: 0x00122420 File Offset: 0x00120620
	private void OnCollisionEnter(Collision collision)
	{
		this.tryToTrigger(base.transform.position, collision.GetContact(0).point);
	}

	// Token: 0x06003D1E RID: 15646 RVA: 0x00122450 File Offset: 0x00120650
	private void OnCollisionExit(Collision collision)
	{
		this.tryToTrigger(base.transform.position, collision.GetContact(0).point);
	}

	// Token: 0x06003D1F RID: 15647 RVA: 0x0012247D File Offset: 0x0012067D
	private void tryToTrigger(Vector3 p1, Vector3 p2)
	{
		if (this.timeSinceTrigger > this.minRetriggerTime)
		{
			if (this.colliderFound != null)
			{
				this.colliderFound.Invoke(p1, p2);
			}
			this.zero();
		}
	}

	// Token: 0x06003D20 RID: 15648 RVA: 0x001224A8 File Offset: 0x001206A8
	private void Update()
	{
		float num = Mathf.Max(new float[]
		{
			base.transform.lossyScale.x,
			base.transform.lossyScale.y,
			base.transform.lossyScale.z
		});
		if (base.transform.localScale.x < this.maxSize * num)
		{
			base.transform.localScale += Vector3.one * Time.deltaTime * num;
			if (this.audioSource != null)
			{
				this.audioSource.volume = this.maxVolume * (base.transform.localScale.x / this.maxSize);
				this.audioSource.pitch = 1f + this.maxPitch * (base.transform.localScale.x / this.maxSize);
			}
		}
		this.timeSinceTrigger += Time.deltaTime;
	}

	// Token: 0x040040D3 RID: 16595
	[SerializeField]
	private float maxSize = 10f;

	// Token: 0x040040D4 RID: 16596
	[SerializeField]
	private float initialRadius = 1f;

	// Token: 0x040040D5 RID: 16597
	[SerializeField]
	private float minRetriggerTime = 1f;

	// Token: 0x040040D6 RID: 16598
	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	// Token: 0x040040D7 RID: 16599
	private AudioSource audioSource;

	// Token: 0x040040D8 RID: 16600
	private float maxVolume;

	// Token: 0x040040D9 RID: 16601
	private float maxPitch;

	// Token: 0x040040DA RID: 16602
	private float timeSinceTrigger;
}
