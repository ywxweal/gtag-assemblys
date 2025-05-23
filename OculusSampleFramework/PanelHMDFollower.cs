using System;
using System.Collections;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BEE RID: 3054
	public class PanelHMDFollower : MonoBehaviour
	{
		// Token: 0x06004B71 RID: 19313 RVA: 0x00165D46 File Offset: 0x00163F46
		private void Awake()
		{
			this._cameraRig = Object.FindObjectOfType<OVRCameraRig>();
			this._panelInitialPosition = base.transform.position;
		}

		// Token: 0x06004B72 RID: 19314 RVA: 0x00165D64 File Offset: 0x00163F64
		private void Update()
		{
			Vector3 position = this._cameraRig.centerEyeAnchor.position;
			Vector3 position2 = base.transform.position;
			float num = Vector3.Distance(position, this._lastMovedToPos);
			float num2 = (this._cameraRig.centerEyeAnchor.position - this._prevPos).magnitude / Time.deltaTime;
			Vector3 vector = base.transform.position - position;
			float magnitude = vector.magnitude;
			if ((num > this._maxDistance || this._minZDistance > vector.z || this._minDistance > magnitude) && num2 < 0.3f && this._coroutine == null && this._coroutine == null)
			{
				this._coroutine = base.StartCoroutine(this.LerpToHMD());
			}
			this._prevPos = this._cameraRig.centerEyeAnchor.position;
		}

		// Token: 0x06004B73 RID: 19315 RVA: 0x00165E3E File Offset: 0x0016403E
		private Vector3 CalculateIdealAnchorPosition()
		{
			return this._cameraRig.centerEyeAnchor.position + this._panelInitialPosition;
		}

		// Token: 0x06004B74 RID: 19316 RVA: 0x00165E5B File Offset: 0x0016405B
		private IEnumerator LerpToHMD()
		{
			Vector3 newPanelPosition = this.CalculateIdealAnchorPosition();
			this._lastMovedToPos = this._cameraRig.centerEyeAnchor.position;
			float startTime = Time.time;
			float endTime = Time.time + 3f;
			while (Time.time < endTime)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, newPanelPosition, (Time.time - startTime) / 3f);
				yield return null;
			}
			base.transform.position = newPanelPosition;
			this._coroutine = null;
			yield break;
		}

		// Token: 0x04004DFE RID: 19966
		private const float TOTAL_DURATION = 3f;

		// Token: 0x04004DFF RID: 19967
		private const float HMD_MOVEMENT_THRESHOLD = 0.3f;

		// Token: 0x04004E00 RID: 19968
		[SerializeField]
		private float _maxDistance = 0.3f;

		// Token: 0x04004E01 RID: 19969
		[SerializeField]
		private float _minDistance = 0.05f;

		// Token: 0x04004E02 RID: 19970
		[SerializeField]
		private float _minZDistance = 0.05f;

		// Token: 0x04004E03 RID: 19971
		private OVRCameraRig _cameraRig;

		// Token: 0x04004E04 RID: 19972
		private Vector3 _panelInitialPosition = Vector3.zero;

		// Token: 0x04004E05 RID: 19973
		private Coroutine _coroutine;

		// Token: 0x04004E06 RID: 19974
		private Vector3 _prevPos = Vector3.zero;

		// Token: 0x04004E07 RID: 19975
		private Vector3 _lastMovedToPos = Vector3.zero;
	}
}
