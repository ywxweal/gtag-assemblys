using System;
using UnityEngine;

// Token: 0x0200049B RID: 1179
public class GorillaSceneCamera : MonoBehaviour
{
	// Token: 0x06001CAA RID: 7338 RVA: 0x0008B8F0 File Offset: 0x00089AF0
	public void SetSceneCamera(int sceneIndex)
	{
		base.transform.position = this.sceneTransforms[sceneIndex].scenePosition;
		base.transform.eulerAngles = this.sceneTransforms[sceneIndex].sceneRotation;
	}

	// Token: 0x04001FE7 RID: 8167
	public GorillaSceneTransform[] sceneTransforms;
}
