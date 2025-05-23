using System;
using System.Collections;
using UnityEngine;

// Token: 0x020006AC RID: 1708
public class SodaBubble : MonoBehaviour
{
	// Token: 0x06002AA8 RID: 10920 RVA: 0x000D1D1B File Offset: 0x000CFF1B
	public void Pop()
	{
		base.StartCoroutine(this.PopCoroutine());
	}

	// Token: 0x06002AA9 RID: 10921 RVA: 0x000D1D2A File Offset: 0x000CFF2A
	private IEnumerator PopCoroutine()
	{
		this.audioSource.GTPlay();
		this.bubbleMesh.gameObject.SetActive(false);
		this.bubbleCollider.gameObject.SetActive(false);
		yield return new WaitForSeconds(1f);
		this.bubbleMesh.gameObject.SetActive(true);
		this.bubbleCollider.gameObject.SetActive(true);
		ObjectPools.instance.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x04002F91 RID: 12177
	public MeshRenderer bubbleMesh;

	// Token: 0x04002F92 RID: 12178
	public Rigidbody body;

	// Token: 0x04002F93 RID: 12179
	public MeshCollider bubbleCollider;

	// Token: 0x04002F94 RID: 12180
	public AudioSource audioSource;
}
