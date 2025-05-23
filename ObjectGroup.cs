using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000773 RID: 1907
public class ObjectGroup : MonoBehaviour
{
	// Token: 0x06002F86 RID: 12166 RVA: 0x000ECBB8 File Offset: 0x000EADB8
	private void OnEnable()
	{
		if (this.syncWithGroupState)
		{
			this.SetObjectStates(true);
		}
	}

	// Token: 0x06002F87 RID: 12167 RVA: 0x000ECBC9 File Offset: 0x000EADC9
	private void OnDisable()
	{
		if (this.syncWithGroupState)
		{
			this.SetObjectStates(false);
		}
	}

	// Token: 0x06002F88 RID: 12168 RVA: 0x000ECBDC File Offset: 0x000EADDC
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

	// Token: 0x04003610 RID: 13840
	public List<GameObject> gameObjects = new List<GameObject>(16);

	// Token: 0x04003611 RID: 13841
	public List<Behaviour> behaviours = new List<Behaviour>(16);

	// Token: 0x04003612 RID: 13842
	public List<Renderer> renderers = new List<Renderer>(16);

	// Token: 0x04003613 RID: 13843
	public List<Collider> colliders = new List<Collider>(16);

	// Token: 0x04003614 RID: 13844
	public bool syncWithGroupState = true;
}
