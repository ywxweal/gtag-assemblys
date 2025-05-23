using System;
using System.Collections;
using UnityEngine;

// Token: 0x020006AE RID: 1710
public class SlowCameraUpdate : MonoBehaviour
{
	// Token: 0x06002AB1 RID: 10929 RVA: 0x000D1DFB File Offset: 0x000CFFFB
	public void Awake()
	{
		this.frameRate = 30f;
		this.timeToNextFrame = 1f / this.frameRate;
		this.myCamera = base.GetComponent<Camera>();
	}

	// Token: 0x06002AB2 RID: 10930 RVA: 0x000D1E26 File Offset: 0x000D0026
	public void OnEnable()
	{
		base.StartCoroutine(this.UpdateMirror());
	}

	// Token: 0x06002AB3 RID: 10931 RVA: 0x00004F01 File Offset: 0x00003101
	public void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06002AB4 RID: 10932 RVA: 0x000D1E35 File Offset: 0x000D0035
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

	// Token: 0x04002F98 RID: 12184
	private Camera myCamera;

	// Token: 0x04002F99 RID: 12185
	private float frameRate;

	// Token: 0x04002F9A RID: 12186
	private float timeToNextFrame;
}
