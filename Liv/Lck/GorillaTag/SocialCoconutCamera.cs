using System;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	// Token: 0x02000CAA RID: 3242
	public class SocialCoconutCamera : MonoBehaviour
	{
		// Token: 0x0600502E RID: 20526 RVA: 0x0017E80F File Offset: 0x0017CA0F
		private void Awake()
		{
			if (this._propertyBlock == null)
			{
				this._propertyBlock = new MaterialPropertyBlock();
			}
			this._propertyBlock.SetInt(this.IS_RECORDING, 0);
			this._bodyRenderer.SetPropertyBlock(this._propertyBlock);
		}

		// Token: 0x0600502F RID: 20527 RVA: 0x0017E847 File Offset: 0x0017CA47
		public void SetVisualsActive(bool active)
		{
			this._isActive = active;
			this._visuals.SetActive(active);
		}

		// Token: 0x06005030 RID: 20528 RVA: 0x0017E85C File Offset: 0x0017CA5C
		public void SetRecordingState(bool isRecording)
		{
			if (!this._isActive)
			{
				return;
			}
			this._propertyBlock.SetInt(this.IS_RECORDING, isRecording ? 1 : 0);
			this._bodyRenderer.SetPropertyBlock(this._propertyBlock);
		}

		// Token: 0x04005337 RID: 21303
		[SerializeField]
		private GameObject _visuals;

		// Token: 0x04005338 RID: 21304
		[SerializeField]
		private MeshRenderer _bodyRenderer;

		// Token: 0x04005339 RID: 21305
		private bool _isActive;

		// Token: 0x0400533A RID: 21306
		private MaterialPropertyBlock _propertyBlock;

		// Token: 0x0400533B RID: 21307
		private string IS_RECORDING = "_Is_Recording";
	}
}
