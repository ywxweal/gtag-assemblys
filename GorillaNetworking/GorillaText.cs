using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaNetworking
{
	// Token: 0x02000C3D RID: 3133
	[Serializable]
	public class GorillaText
	{
		// Token: 0x06004DF9 RID: 19961 RVA: 0x00174098 File Offset: 0x00172298
		public void Initialize(Material[] originalMaterials, Material failureMaterial, UnityEvent<string> callback = null, UnityEvent<Material[]> materialCallback = null)
		{
			this.failureMaterial = failureMaterial;
			this.originalMaterials = originalMaterials;
			this.currentMaterials = originalMaterials;
			Debug.Log("Original text = " + this.originalText);
			this.updateTextCallback = callback;
			this.updateMaterialCallback = materialCallback;
		}

		// Token: 0x170007B5 RID: 1973
		// (get) Token: 0x06004DFA RID: 19962 RVA: 0x001740D3 File Offset: 0x001722D3
		// (set) Token: 0x06004DFB RID: 19963 RVA: 0x001740DB File Offset: 0x001722DB
		public string Text
		{
			get
			{
				return this.originalText;
			}
			set
			{
				if (this.originalText == value)
				{
					return;
				}
				this.originalText = value;
				if (!this.failedState)
				{
					UnityEvent<string> unityEvent = this.updateTextCallback;
					if (unityEvent == null)
					{
						return;
					}
					unityEvent.Invoke(value);
				}
			}
		}

		// Token: 0x06004DFC RID: 19964 RVA: 0x0017410C File Offset: 0x0017230C
		public void EnableFailedState(string failText)
		{
			this.failedState = true;
			this.failureText = failText;
			UnityEvent<string> unityEvent = this.updateTextCallback;
			if (unityEvent != null)
			{
				unityEvent.Invoke(failText);
			}
			this.currentMaterials = (Material[])this.originalMaterials.Clone();
			this.currentMaterials[0] = this.failureMaterial;
			UnityEvent<Material[]> unityEvent2 = this.updateMaterialCallback;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke(this.currentMaterials);
		}

		// Token: 0x06004DFD RID: 19965 RVA: 0x00174174 File Offset: 0x00172374
		public void DisableFailedState()
		{
			this.failedState = true;
			UnityEvent<string> unityEvent = this.updateTextCallback;
			if (unityEvent != null)
			{
				unityEvent.Invoke(this.originalText);
			}
			this.failureText = "";
			this.currentMaterials = this.originalMaterials;
			UnityEvent<Material[]> unityEvent2 = this.updateMaterialCallback;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke(this.currentMaterials);
		}

		// Token: 0x04005103 RID: 20739
		private string failureText;

		// Token: 0x04005104 RID: 20740
		private string originalText = string.Empty;

		// Token: 0x04005105 RID: 20741
		private bool failedState;

		// Token: 0x04005106 RID: 20742
		private Material[] originalMaterials;

		// Token: 0x04005107 RID: 20743
		private Material failureMaterial;

		// Token: 0x04005108 RID: 20744
		internal Material[] currentMaterials;

		// Token: 0x04005109 RID: 20745
		private UnityEvent<string> updateTextCallback;

		// Token: 0x0400510A RID: 20746
		private UnityEvent<Material[]> updateMaterialCallback;
	}
}
