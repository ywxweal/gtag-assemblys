using System;
using UnityEngine;

// Token: 0x02000370 RID: 880
public class RequestCaptureFlow : MonoBehaviour
{
	// Token: 0x06001465 RID: 5221 RVA: 0x0006387E File Offset: 0x00061A7E
	private void Start()
	{
		this._sceneManager = Object.FindObjectOfType<OVRSceneManager>();
	}

	// Token: 0x06001466 RID: 5222 RVA: 0x0006388B File Offset: 0x00061A8B
	private void Update()
	{
		if (OVRInput.GetUp(this.RequestCaptureBtn, OVRInput.Controller.Active))
		{
			this._sceneManager.RequestSceneCapture();
		}
	}

	// Token: 0x040016C1 RID: 5825
	public OVRInput.Button RequestCaptureBtn = OVRInput.Button.Two;

	// Token: 0x040016C2 RID: 5826
	private OVRSceneManager _sceneManager;
}
