using System;
using System.Collections;
using UnityEngine;

// Token: 0x020006AC RID: 1708
public class SodaBubble : MonoBehaviour
{
	// Token: 0x06002AA9 RID: 10921 RVA: 0x000D1DBF File Offset: 0x000CFFBF
	public void Pop()
	{
		base.StartCoroutine(this.PopCoroutine());
	}

	// Token: 0x06002AAA RID: 10922 RVA: 0x000D1DCE File Offset: 0x000CFFCE
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

	// Token: 0x04002F93 RID: 12179
	public MeshRenderer bubbleMesh;

	// Token: 0x04002F94 RID: 12180
	public Rigidbody body;

	// Token: 0x04002F95 RID: 12181
	public MeshCollider bubbleCollider;

	// Token: 0x04002F96 RID: 12182
	public AudioSource audioSource;
}
