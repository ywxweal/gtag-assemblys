using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000458 RID: 1112
public class BubbleGumEvents : MonoBehaviour
{
	// Token: 0x06001B5F RID: 7007 RVA: 0x00086B29 File Offset: 0x00084D29
	private void OnEnable()
	{
		this._edible.onBiteWorld.AddListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._edible.onBiteView.AddListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x06001B60 RID: 7008 RVA: 0x00086B63 File Offset: 0x00084D63
	private void OnDisable()
	{
		this._edible.onBiteWorld.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._edible.onBiteView.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x06001B61 RID: 7009 RVA: 0x00086B9D File Offset: 0x00084D9D
	public void OnBiteView(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, true);
	}

	// Token: 0x06001B62 RID: 7010 RVA: 0x00086BA8 File Offset: 0x00084DA8
	public void OnBiteWorld(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, false);
	}

	// Token: 0x06001B63 RID: 7011 RVA: 0x00086BB4 File Offset: 0x00084DB4
	public void OnBite(VRRig rig, int nextState, bool isViewRig)
	{
		GorillaTagger instance = GorillaTagger.Instance;
		GameObject gameObject = null;
		if (isViewRig && instance != null)
		{
			gameObject = instance.gameObject;
		}
		else if (!isViewRig)
		{
			gameObject = rig.gameObject;
		}
		if (!BubbleGumEvents.gTargetCache.TryGetValue(gameObject, out this._bubble))
		{
			this._bubble = gameObject.GetComponentsInChildren<GumBubble>(true).FirstOrDefault((GumBubble g) => g.transform.parent.name == "$gum");
			if (isViewRig)
			{
				this._bubble.audioSource = instance.offlineVRRig.tagSound;
				this._bubble.targetScale = Vector3.one * 1.36f;
			}
			else
			{
				this._bubble.audioSource = rig.tagSound;
				this._bubble.targetScale = Vector3.one * 2f;
			}
			BubbleGumEvents.gTargetCache.Add(gameObject, this._bubble);
		}
		GumBubble bubble = this._bubble;
		if (bubble != null)
		{
			bubble.transform.parent.gameObject.SetActive(true);
		}
		GumBubble bubble2 = this._bubble;
		if (bubble2 == null)
		{
			return;
		}
		bubble2.InflateDelayed();
	}

	// Token: 0x04001E56 RID: 7766
	[SerializeField]
	private EdibleHoldable _edible;

	// Token: 0x04001E57 RID: 7767
	[SerializeField]
	private GumBubble _bubble;

	// Token: 0x04001E58 RID: 7768
	private static Dictionary<GameObject, GumBubble> gTargetCache = new Dictionary<GameObject, GumBubble>(16);

	// Token: 0x02000459 RID: 1113
	public enum EdibleState
	{
		// Token: 0x04001E5A RID: 7770
		A = 1,
		// Token: 0x04001E5B RID: 7771
		B,
		// Token: 0x04001E5C RID: 7772
		C = 4,
		// Token: 0x04001E5D RID: 7773
		D = 8
	}
}
