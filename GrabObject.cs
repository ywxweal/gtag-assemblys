using System;
using UnityEngine;

// Token: 0x02000333 RID: 819
public class GrabObject : MonoBehaviour
{
	// Token: 0x06001366 RID: 4966 RVA: 0x0005C8EF File Offset: 0x0005AAEF
	public void Grab(OVRInput.Controller grabHand)
	{
		this.grabbedRotation = base.transform.rotation;
		GrabObject.GrabbedObject grabbedObjectDelegate = this.GrabbedObjectDelegate;
		if (grabbedObjectDelegate == null)
		{
			return;
		}
		grabbedObjectDelegate(grabHand);
	}

	// Token: 0x06001367 RID: 4967 RVA: 0x0005C913 File Offset: 0x0005AB13
	public void Release()
	{
		GrabObject.ReleasedObject releasedObjectDelegate = this.ReleasedObjectDelegate;
		if (releasedObjectDelegate == null)
		{
			return;
		}
		releasedObjectDelegate();
	}

	// Token: 0x06001368 RID: 4968 RVA: 0x0005C925 File Offset: 0x0005AB25
	public void CursorPos(Vector3 cursorPos)
	{
		GrabObject.SetCursorPosition cursorPositionDelegate = this.CursorPositionDelegate;
		if (cursorPositionDelegate == null)
		{
			return;
		}
		cursorPositionDelegate(cursorPos);
	}

	// Token: 0x04001586 RID: 5510
	[TextArea]
	public string ObjectName;

	// Token: 0x04001587 RID: 5511
	[TextArea]
	public string ObjectInstructions;

	// Token: 0x04001588 RID: 5512
	public GrabObject.ManipulationType objectManipulationType;

	// Token: 0x04001589 RID: 5513
	public bool showLaserWhileGrabbed;

	// Token: 0x0400158A RID: 5514
	[HideInInspector]
	public Quaternion grabbedRotation = Quaternion.identity;

	// Token: 0x0400158B RID: 5515
	public GrabObject.GrabbedObject GrabbedObjectDelegate;

	// Token: 0x0400158C RID: 5516
	public GrabObject.ReleasedObject ReleasedObjectDelegate;

	// Token: 0x0400158D RID: 5517
	public GrabObject.SetCursorPosition CursorPositionDelegate;

	// Token: 0x02000334 RID: 820
	public enum ManipulationType
	{
		// Token: 0x0400158F RID: 5519
		Default,
		// Token: 0x04001590 RID: 5520
		ForcedHand,
		// Token: 0x04001591 RID: 5521
		DollyHand,
		// Token: 0x04001592 RID: 5522
		DollyAttached,
		// Token: 0x04001593 RID: 5523
		HorizontalScaled,
		// Token: 0x04001594 RID: 5524
		VerticalScaled,
		// Token: 0x04001595 RID: 5525
		Menu
	}

	// Token: 0x02000335 RID: 821
	// (Invoke) Token: 0x0600136B RID: 4971
	public delegate void GrabbedObject(OVRInput.Controller grabHand);

	// Token: 0x02000336 RID: 822
	// (Invoke) Token: 0x0600136F RID: 4975
	public delegate void ReleasedObject();

	// Token: 0x02000337 RID: 823
	// (Invoke) Token: 0x06001373 RID: 4979
	public delegate void SetCursorPosition(Vector3 cursorPosition);
}
