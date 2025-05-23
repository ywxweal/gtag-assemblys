using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BE1 RID: 3041
	public class FingerTipPokeToolView : MonoBehaviour, InteractableToolView
	{
		// Token: 0x17000758 RID: 1880
		// (get) Token: 0x06004B12 RID: 19218 RVA: 0x00164D64 File Offset: 0x00162F64
		// (set) Token: 0x06004B13 RID: 19219 RVA: 0x00164D6C File Offset: 0x00162F6C
		public InteractableTool InteractableTool { get; set; }

		// Token: 0x17000759 RID: 1881
		// (get) Token: 0x06004B14 RID: 19220 RVA: 0x00164D75 File Offset: 0x00162F75
		// (set) Token: 0x06004B15 RID: 19221 RVA: 0x00164D82 File Offset: 0x00162F82
		public bool EnableState
		{
			get
			{
				return this._sphereMeshRenderer.enabled;
			}
			set
			{
				this._sphereMeshRenderer.enabled = value;
			}
		}

		// Token: 0x1700075A RID: 1882
		// (get) Token: 0x06004B16 RID: 19222 RVA: 0x00164D90 File Offset: 0x00162F90
		// (set) Token: 0x06004B17 RID: 19223 RVA: 0x00164D98 File Offset: 0x00162F98
		public bool ToolActivateState { get; set; }

		// Token: 0x1700075B RID: 1883
		// (get) Token: 0x06004B18 RID: 19224 RVA: 0x00164DA1 File Offset: 0x00162FA1
		// (set) Token: 0x06004B19 RID: 19225 RVA: 0x00164DA9 File Offset: 0x00162FA9
		public float SphereRadius { get; private set; }

		// Token: 0x06004B1A RID: 19226 RVA: 0x00164DB2 File Offset: 0x00162FB2
		private void Awake()
		{
			this.SphereRadius = this._sphereMeshRenderer.transform.localScale.z * 0.5f;
		}

		// Token: 0x06004B1B RID: 19227 RVA: 0x000023F4 File Offset: 0x000005F4
		public void SetFocusedInteractable(Interactable interactable)
		{
		}

		// Token: 0x04004DBB RID: 19899
		[SerializeField]
		private MeshRenderer _sphereMeshRenderer;
	}
}
