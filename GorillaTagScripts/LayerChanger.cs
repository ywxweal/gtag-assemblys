using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000B1A RID: 2842
	public class LayerChanger : MonoBehaviour
	{
		// Token: 0x060045E9 RID: 17897 RVA: 0x0014BE71 File Offset: 0x0014A071
		public void InitializeLayers(Transform parent)
		{
			if (!this.layersStored)
			{
				this.StoreOriginalLayers(parent);
				this.layersStored = true;
			}
		}

		// Token: 0x060045EA RID: 17898 RVA: 0x0014BE8C File Offset: 0x0014A08C
		private void StoreOriginalLayers(Transform parent)
		{
			if (!this.includeChildren)
			{
				this.StoreOriginalLayers(parent);
				return;
			}
			foreach (object obj in parent)
			{
				Transform transform = (Transform)obj;
				this.originalLayers[transform] = transform.gameObject.layer;
				this.StoreOriginalLayers(transform);
			}
		}

		// Token: 0x060045EB RID: 17899 RVA: 0x0014BF08 File Offset: 0x0014A108
		public void ChangeLayer(Transform parent, string newLayer)
		{
			if (!this.layersStored)
			{
				Debug.LogWarning("Layers have not been initialized. Call InitializeLayers first.");
				return;
			}
			this.ChangeLayers(parent, LayerMask.NameToLayer(newLayer));
		}

		// Token: 0x060045EC RID: 17900 RVA: 0x0014BF2C File Offset: 0x0014A12C
		private void ChangeLayers(Transform parent, int newLayer)
		{
			if (!this.includeChildren)
			{
				if (!this.restrictedLayers.Contains(parent.gameObject.layer))
				{
					parent.gameObject.layer = newLayer;
				}
				return;
			}
			foreach (object obj in parent)
			{
				Transform transform = (Transform)obj;
				if (!this.restrictedLayers.Contains(transform.gameObject.layer))
				{
					transform.gameObject.layer = newLayer;
					this.ChangeLayers(transform, newLayer);
				}
			}
		}

		// Token: 0x060045ED RID: 17901 RVA: 0x0014BFD4 File Offset: 0x0014A1D4
		public void RestoreOriginalLayers()
		{
			if (!this.layersStored)
			{
				Debug.LogWarning("Layers have not been initialized. Call InitializeLayers first.");
				return;
			}
			foreach (KeyValuePair<Transform, int> keyValuePair in this.originalLayers)
			{
				keyValuePair.Key.gameObject.layer = keyValuePair.Value;
			}
		}

		// Token: 0x04004872 RID: 18546
		public LayerMask restrictedLayers;

		// Token: 0x04004873 RID: 18547
		public bool includeChildren = true;

		// Token: 0x04004874 RID: 18548
		private Dictionary<Transform, int> originalLayers = new Dictionary<Transform, int>();

		// Token: 0x04004875 RID: 18549
		private bool layersStored;
	}
}
