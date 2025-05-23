using System;
using System.Collections;
using UnityEngine;

// Token: 0x020006AE RID: 1710
public class SlowCameraUpdate : MonoBehaviour
{
	// Token: 0x06002AB2 RID: 10930 RVA: 0x000D1E9F File Offset: 0x000D009F
	public void Awake()
	{
		this.frameRate = 30f;
		this.timeToNextFrame = 1f / this.frameRate;
		this.myCamera = base.GetComponent<Camera>();
	}

	// Token: 0x06002AB3 RID: 10931 RVA: 0x000D1ECA File Offset: 0x000D00CA
	public void OnEnable()
	{
		base.StartCoroutine(this.UpdateMirror());
	}

	// Token: 0x06002AB4 RID: 10932 RVA: 0x00004F01 File Offset: 0x00003101
	public void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06002AB5 RID: 10933 RVA: 0x000D1ED9 File Offset: 0x000D00D9
	public IEnumerator UpdateMirror()
	{
		for (;;)
		{
			if (base.gameObject.activeSelf)
			{
				Debug.Log("rendering camera!");
				this.myCamera.Render();
			}
			yield return new WaitForSeconds(this.timeToNextFrame);
		}
		yield break;
	}

	// Token: 0x04002F9A RID: 12186
	private Camera myCamera;

	// Token: 0x04002F9B RID: 12187
	private float frameRate;

	// Token: 0x04002F9C RID: 12188
	private float timeToNextFrame;
}
