using System;
using UnityEngine;

// Token: 0x0200034B RID: 843
public class BouncingBallMgr : MonoBehaviour
{
	// Token: 0x060013E4 RID: 5092 RVA: 0x000604C8 File Offset: 0x0005E6C8
	private void Update()
	{
		if (!this.ballGrabbed && OVRInput.GetDown(this.actionBtn, OVRInput.Controller.Active))
		{
			this.currentBall = Object.Instantiate<GameObject>(this.ball, this.rightControllerPivot.transform.position, Quaternion.identity);
			this.currentBall.transform.parent = this.rightControllerPivot.transform;
			this.ballGrabbed = true;
		}
		if (this.ballGrabbed && OVRInput.GetUp(this.actionBtn, OVRInput.Controller.Active))
		{
			this.currentBall.transform.parent = null;
			Vector3 position = this.currentBall.transform.position;
			Vector3 vector = this.trackingspace.rotation * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
			Vector3 localControllerAngularVelocity = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
			this.currentBall.GetComponent<BouncingBallLogic>().Release(position, vector, localControllerAngularVelocity);
			this.ballGrabbed = false;
		}
	}

	// Token: 0x04001617 RID: 5655
	[SerializeField]
	private Transform trackingspace;

	// Token: 0x04001618 RID: 5656
	[SerializeField]
	private GameObject rightControllerPivot;

	// Token: 0x04001619 RID: 5657
	[SerializeField]
	private OVRInput.RawButton actionBtn;

	// Token: 0x0400161A RID: 5658
	[SerializeField]
	private GameObject ball;

	// Token: 0x0400161B RID: 5659
	private GameObject currentBall;

	// Token: 0x0400161C RID: 5660
	private bool ballGrabbed;
}
