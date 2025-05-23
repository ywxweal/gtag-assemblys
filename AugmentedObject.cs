using System;
using UnityEngine;

// Token: 0x0200032B RID: 811
public class AugmentedObject : MonoBehaviour
{
	// Token: 0x06001338 RID: 4920 RVA: 0x0005BD60 File Offset: 0x00059F60
	private void Start()
	{
		if (base.GetComponent<GrabObject>())
		{
			GrabObject component = base.GetComponent<GrabObject>();
			component.GrabbedObjectDelegate = (GrabObject.GrabbedObject)Delegate.Combine(component.GrabbedObjectDelegate, new GrabObject.GrabbedObject(this.Grab));
			GrabObject component2 = base.GetComponent<GrabObject>();
			component2.ReleasedObjectDelegate = (GrabObject.ReleasedObject)Delegate.Combine(component2.ReleasedObjectDelegate, new GrabObject.ReleasedObject(this.Release));
		}
	}

	// Token: 0x06001339 RID: 4921 RVA: 0x0005BDC8 File Offset: 0x00059FC8
	private void Update()
	{
		if (this.controllerHand != OVRInput.Controller.None && OVRInput.GetUp(OVRInput.Button.One, this.controllerHand))
		{
			this.ToggleShadowType();
		}
		if (this.shadow)
		{
			if (this.groundShadow)
			{
				this.shadow.transform.position = new Vector3(base.transform.position.x, 0f, base.transform.position.z);
				return;
			}
			this.shadow.transform.localPosition = Vector3.zero;
		}
	}

	// Token: 0x0600133A RID: 4922 RVA: 0x0005BE56 File Offset: 0x0005A056
	public void Grab(OVRInput.Controller grabHand)
	{
		this.controllerHand = grabHand;
	}

	// Token: 0x0600133B RID: 4923 RVA: 0x0005BE5F File Offset: 0x0005A05F
	public void Release()
	{
		this.controllerHand = OVRInput.Controller.None;
	}

	// Token: 0x0600133C RID: 4924 RVA: 0x0005BE68 File Offset: 0x0005A068
	private void ToggleShadowType()
	{
		this.groundShadow = !this.groundShadow;
	}

	// Token: 0x0400155B RID: 5467
	public OVRInput.Controller controllerHand;

	// Token: 0x0400155C RID: 5468
	public Transform shadow;

	// Token: 0x0400155D RID: 5469
	private bool groundShadow;
}
