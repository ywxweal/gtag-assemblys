using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000A01 RID: 2561
public class RandomLocalColliders : MonoBehaviour
{
	// Token: 0x06003D34 RID: 15668 RVA: 0x001229A3 File Offset: 0x00120BA3
	private void Start()
	{
		this.colliders = new List<Collider>();
		this.seekFreq = RandomLocalColliders.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
	}

	// Token: 0x06003D35 RID: 15669 RVA: 0x001229CC File Offset: 0x00120BCC
	private void Update()
	{
		this.timeSinceSeek += Time.deltaTime;
		if (this.timeSinceSeek > this.seekFreq)
		{
			this.seek();
			this.timeSinceSeek = 0f;
			this.seekFreq = RandomLocalColliders.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
		}
	}

	// Token: 0x06003D36 RID: 15670 RVA: 0x00122A28 File Offset: 0x00120C28
	private void seek()
	{
		float num = Mathf.Max(new float[]
		{
			base.transform.lossyScale.x,
			base.transform.lossyScale.y,
			base.transform.lossyScale.z
		});
		this.colliders.Clear();
		this.colliders.AddRange(Physics.OverlapSphere(base.transform.position, this.maxRadias * num));
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.minRadias * num);
		for (int i = 0; i < array.Length; i++)
		{
			this.colliders.Remove(array[i]);
		}
		if (this.colliders.Count > 0 && this.colliderFound != null)
		{
			this.colliderFound.Invoke(base.transform.position, this.colliders[RandomLocalColliders.rand.NextInt(this.colliders.Count)].transform.position);
		}
	}

	// Token: 0x040040EB RID: 16619
	private static SRand rand = new SRand("RandomLocalColliders");

	// Token: 0x040040EC RID: 16620
	[SerializeField]
	private float minseekFreq = 3f;

	// Token: 0x040040ED RID: 16621
	[SerializeField]
	private float maxseekFreq = 6f;

	// Token: 0x040040EE RID: 16622
	[SerializeField]
	private float minRadias = 1f;

	// Token: 0x040040EF RID: 16623
	[SerializeField]
	private float maxRadias = 10f;

	// Token: 0x040040F0 RID: 16624
	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	// Token: 0x040040F1 RID: 16625
	private List<Collider> colliders;

	// Token: 0x040040F2 RID: 16626
	private float timeSinceSeek;

	// Token: 0x040040F3 RID: 16627
	private float seekFreq;
}
