using System;
using UnityEngine;

// Token: 0x0200060D RID: 1549
public class GorillaHasUITransformFollow : MonoBehaviour
{
	// Token: 0x06002680 RID: 9856 RVA: 0x000BE540 File Offset: 0x000BC740
	private void Awake()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(base.gameObject.activeSelf);
		}
	}

	// Token: 0x06002681 RID: 9857 RVA: 0x000BE57C File Offset: 0x000BC77C
	private void OnDestroy()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i].gameObject);
		}
	}

	// Token: 0x06002682 RID: 9858 RVA: 0x000BE5AC File Offset: 0x000BC7AC
	private void OnEnable()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(true);
		}
	}

	// Token: 0x06002683 RID: 9859 RVA: 0x000BE5DC File Offset: 0x000BC7DC
	private void OnDisable()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x04002AE2 RID: 10978
	public GorillaUITransformFollow[] transformFollowers;
}
