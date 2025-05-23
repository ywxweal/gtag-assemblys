using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000325 RID: 805
public class HandsActiveChecker : MonoBehaviour
{
	// Token: 0x0600130D RID: 4877 RVA: 0x0005A604 File Offset: 0x00058804
	private void Awake()
	{
		this._notification = Object.Instantiate<GameObject>(this._notificationPrefab);
		base.StartCoroutine(this.GetCenterEye());
	}

	// Token: 0x0600130E RID: 4878 RVA: 0x0005A624 File Offset: 0x00058824
	private void Update()
	{
		if (OVRPlugin.GetHandTrackingEnabled())
		{
			this._notification.SetActive(false);
			return;
		}
		this._notification.SetActive(true);
		if (this._centerEye)
		{
			this._notification.transform.position = this._centerEye.position + this._centerEye.forward * 0.5f;
			this._notification.transform.rotation = this._centerEye.rotation;
		}
	}

	// Token: 0x0600130F RID: 4879 RVA: 0x0005A6AE File Offset: 0x000588AE
	private IEnumerator GetCenterEye()
	{
		if ((this._cameraRig = Object.FindObjectOfType<OVRCameraRig>()) != null)
		{
			while (!this._centerEye)
			{
				this._centerEye = this._cameraRig.centerEyeAnchor;
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x0400152F RID: 5423
	[SerializeField]
	private GameObject _notificationPrefab;

	// Token: 0x04001530 RID: 5424
	private GameObject _notification;

	// Token: 0x04001531 RID: 5425
	private OVRCameraRig _cameraRig;

	// Token: 0x04001532 RID: 5426
	private Transform _centerEye;
}
