using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200045D RID: 1117
public class HotPepperEvents : MonoBehaviour
{
	// Token: 0x06001B74 RID: 7028 RVA: 0x00086F53 File Offset: 0x00085153
	private void OnEnable()
	{
		this._pepper.onBiteWorld.AddListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._pepper.onBiteView.AddListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x06001B75 RID: 7029 RVA: 0x00086F8D File Offset: 0x0008518D
	private void OnDisable()
	{
		this._pepper.onBiteWorld.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._pepper.onBiteView.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x06001B76 RID: 7030 RVA: 0x00086FC7 File Offset: 0x000851C7
	public void OnBiteView(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, true);
	}

	// Token: 0x06001B77 RID: 7031 RVA: 0x00086FD2 File Offset: 0x000851D2
	public void OnBiteWorld(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, false);
	}

	// Token: 0x06001B78 RID: 7032 RVA: 0x00086FDD File Offset: 0x000851DD
	public void OnBite(VRRig rig, int nextState, bool isViewRig)
	{
		if (nextState != 8)
		{
			return;
		}
		rig.transform.Find("RigAnchor/rig/body/head/gorillaface/spicy").gameObject.GetComponent<HotPepperFace>().PlayFX(1f);
	}

	// Token: 0x04001E6D RID: 7789
	[SerializeField]
	private EdibleHoldable _pepper;

	// Token: 0x0200045E RID: 1118
	public enum EdibleState
	{
		// Token: 0x04001E6F RID: 7791
		A = 1,
		// Token: 0x04001E70 RID: 7792
		B,
		// Token: 0x04001E71 RID: 7793
		C = 4,
		// Token: 0x04001E72 RID: 7794
		D = 8
	}
}
