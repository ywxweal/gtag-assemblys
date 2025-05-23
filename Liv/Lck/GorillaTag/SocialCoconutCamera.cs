using System;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	// Token: 0x02000CAA RID: 3242
	public class SocialCoconutCamera : MonoBehaviour
	{
		// Token: 0x0600502F RID: 20527 RVA: 0x0017E8E7 File Offset: 0x0017CAE7
		private void Awake()
		{
			if (this._propertyBlock == null)
			{
				this._propertyBlock = new MaterialPropertyBlock();
			}
			this._propertyBlock.SetInt(this.IS_RECORDING, 0);
			this._bodyRenderer.SetPropertyBlock(this._propertyBlock);
		}

		// Token: 0x06005030 RID: 20528 RVA: 0x0017E91F File Offset: 0x0017CB1F
		public void SetVisualsActive(bool active)
		{
			this._isActive = active;
			this._visuals.SetActive(active);
		}

		// Token: 0x06005031 RID: 20529 RVA: 0x0017E934 File Offset: 0x0017CB34
		public void SetRecordingState(bool isRecording)
		{
			if (!this._isActive)
			{
				return;
			}
			this._propertyBlock.SetInt(this.IS_RECORDING, isRecording ? 1 : 0);
			this._bodyRenderer.SetPropertyBlock(this._propertyBlock);
		}

		// Token: 0x04005338 RID: 21304
		[SerializeField]
		private GameObject _visuals;

		// Token: 0x04005339 RID: 21305
		[SerializeField]
		private MeshRenderer _bodyRenderer;

		// Token: 0x0400533A RID: 21306
		private bool _isActive;

		// Token: 0x0400533B RID: 21307
		private MaterialPropertyBlock _propertyBlock;

		// Token: 0x0400533C RID: 21308
		private string IS_RECORDING = "_Is_Recording";
	}
}
