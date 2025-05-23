using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000774 RID: 1908
public class ObjectToggle : MonoBehaviour
{
	// Token: 0x06002F8B RID: 12171 RVA: 0x000ECDD2 File Offset: 0x000EAFD2
	public void Toggle(bool initialState = true)
	{
		if (this._toggled == null)
		{
			if (initialState)
			{
				this.Enable();
				return;
			}
			this.Disable();
			return;
		}
		else
		{
			if (this._toggled.Value)
			{
				this.Disable();
				return;
			}
			this.Enable();
			return;
		}
	}

	// Token: 0x06002F8C RID: 12172 RVA: 0x000ECE0C File Offset: 0x000EB00C
	public void Enable()
	{
		if (this.objectsToToggle == null)
		{
			return;
		}
		for (int i = 0; i < this.objectsToToggle.Count; i++)
		{
			GameObject gameObject = this.objectsToToggle[i];
			if (!(gameObject == null))
			{
				if (this._ignoreHierarchyState)
				{
					gameObject.SetActive(true);
				}
				else if (!gameObject.activeInHierarchy)
				{
					gameObject.SetActive(true);
				}
			}
		}
		this._toggled = new bool?(true);
	}

	// Token: 0x06002F8D RID: 12173 RVA: 0x000ECE7C File Offset: 0x000EB07C
	public void Disable()
	{
		if (this.objectsToToggle == null)
		{
			return;
		}
		for (int i = 0; i < this.objectsToToggle.Count; i++)
		{
			GameObject gameObject = this.objectsToToggle[i];
			if (!(gameObject == null))
			{
				if (this._ignoreHierarchyState)
				{
					gameObject.SetActive(false);
				}
				else if (gameObject.activeInHierarchy)
				{
					gameObject.SetActive(false);
				}
			}
		}
		this._toggled = new bool?(false);
	}

	// Token: 0x04003617 RID: 13847
	public List<GameObject> objectsToToggle = new List<GameObject>();

	// Token: 0x04003618 RID: 13848
	[SerializeField]
	private bool _ignoreHierarchyState;

	// Token: 0x04003619 RID: 13849
	[NonSerialized]
	private bool? _toggled;
}
