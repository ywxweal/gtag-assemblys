using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BE1 RID: 3041
	public class FingerTipPokeToolView : MonoBehaviour, InteractableToolView
	{
		// Token: 0x17000758 RID: 1880
		// (get) Token: 0x06004B13 RID: 19219 RVA: 0x00164E3C File Offset: 0x0016303C
		// (set) Token: 0x06004B14 RID: 19220 RVA: 0x00164E44 File Offset: 0x00163044
		public InteractableTool InteractableTool { get; set; }

		// Token: 0x17000759 RID: 1881
		// (get) Token: 0x06004B15 RID: 19221 RVA: 0x00164E4D File Offset: 0x0016304D
		// (set) Token: 0x06004B16 RID: 19222 RVA: 0x00164E5A File Offset: 0x0016305A
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
		// (get) Token: 0x06004B17 RID: 19223 RVA: 0x00164E68 File Offset: 0x00163068
		// (set) Token: 0x06004B18 RID: 19224 RVA: 0x00164E70 File Offset: 0x00163070
		public bool ToolActivateState { get; set; }

		// Token: 0x1700075B RID: 1883
		// (get) Token: 0x06004B19 RID: 19225 RVA: 0x00164E79 File Offset: 0x00163079
		// (set) Token: 0x06004B1A RID: 19226 RVA: 0x00164E81 File Offset: 0x00163081
		public float SphereRadius { get; private set; }

		// Token: 0x06004B1B RID: 19227 RVA: 0x00164E8A File Offset: 0x0016308A
		private void Awake()
		{
			this.SphereRadius = this._sphereMeshRenderer.transform.localScale.z * 0.5f;
		}

		// Token: 0x06004B1C RID: 19228 RVA: 0x000023F4 File Offset: 0x000005F4
		public void SetFocusedInteractable(Interactable interactable)
		{
		}

		// Token: 0x04004DBC RID: 19900
		[SerializeField]
		private MeshRenderer _sphereMeshRenderer;
	}
}
