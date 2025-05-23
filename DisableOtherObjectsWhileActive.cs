using System;
using UnityEngine;

// Token: 0x020001C3 RID: 451
public class DisableOtherObjectsWhileActive : MonoBehaviour
{
	// Token: 0x06000AAC RID: 2732 RVA: 0x0003A3B2 File Offset: 0x000385B2
	private void OnEnable()
	{
		this.SetAllActive(false);
	}

	// Token: 0x06000AAD RID: 2733 RVA: 0x0003A3BB File Offset: 0x000385BB
	private void OnDisable()
	{
		this.SetAllActive(true);
	}

	// Token: 0x06000AAE RID: 2734 RVA: 0x0003A3C4 File Offset: 0x000385C4
	private void SetAllActive(bool active)
	{
		foreach (GameObject gameObject in this.otherObjects)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(active);
			}
		}
		foreach (XSceneRef xsceneRef in this.otherXSceneObjects)
		{
			GameObject gameObject2;
			if (xsceneRef.TryResolve(out gameObject2))
			{
				gameObject2.SetActive(active);
			}
		}
	}

	// Token: 0x04000D34 RID: 3380
	public GameObject[] otherObjects;

	// Token: 0x04000D35 RID: 3381
	public XSceneRef[] otherXSceneObjects;
}
