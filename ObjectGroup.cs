﻿using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000773 RID: 1907
public class ObjectGroup : MonoBehaviour
{
	// Token: 0x06002F87 RID: 12167 RVA: 0x000ECC5C File Offset: 0x000EAE5C
	private void OnEnable()
	{
		if (this.syncWithGroupState)
		{
			this.SetObjectStates(true);
		}
	}

	// Token: 0x06002F88 RID: 12168 RVA: 0x000ECC6D File Offset: 0x000EAE6D
	private void OnDisable()
	{
		if (this.syncWithGroupState)
		{
			this.SetObjectStates(false);
		}
	}

	// Token: 0x06002F89 RID: 12169 RVA: 0x000ECC80 File Offset: 0x000EAE80
	public void SetObjectStates(bool active)
	{
		int count = this.gameObjects.Count;
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = this.gameObjects[i];
			if (!(gameObject == null))
			{
				gameObject.SetActive(active);
			}
		}
		int count2 = this.behaviours.Count;
		for (int j = 0; j < count2; j++)
		{
			Behaviour behaviour = this.behaviours[j];
			if (!(behaviour == null))
			{
				behaviour.enabled = active;
			}
		}
		int count3 = this.renderers.Count;
		for (int k = 0; k < count3; k++)
		{
			Renderer renderer = this.renderers[k];
			if (!(renderer == null))
			{
				renderer.enabled = active;
			}
		}
		int count4 = this.colliders.Count;
		for (int l = 0; l < count4; l++)
		{
			Collider collider = this.colliders[l];
			if (!(collider == null))
			{
				collider.enabled = active;
			}
		}
	}

	// Token: 0x04003612 RID: 13842
	public List<GameObject> gameObjects = new List<GameObject>(16);

	// Token: 0x04003613 RID: 13843
	public List<Behaviour> behaviours = new List<Behaviour>(16);

	// Token: 0x04003614 RID: 13844
	public List<Renderer> renderers = new List<Renderer>(16);

	// Token: 0x04003615 RID: 13845
	public List<Collider> colliders = new List<Collider>(16);

	// Token: 0x04003616 RID: 13846
	public bool syncWithGroupState = true;
}
