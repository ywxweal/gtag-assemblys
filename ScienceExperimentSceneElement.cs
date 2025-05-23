using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020006A9 RID: 1705
public class ScienceExperimentSceneElement : MonoBehaviour, ITickSystemPost
{
	// Token: 0x17000428 RID: 1064
	// (get) Token: 0x06002AA0 RID: 10912 RVA: 0x000D1D05 File Offset: 0x000CFF05
	// (set) Token: 0x06002AA1 RID: 10913 RVA: 0x000D1D0D File Offset: 0x000CFF0D
	bool ITickSystemPost.PostTickRunning { get; set; }

	// Token: 0x06002AA2 RID: 10914 RVA: 0x000D1D18 File Offset: 0x000CFF18
	void ITickSystemPost.PostTick()
	{
		base.transform.position = this.followElement.position;
		base.transform.rotation = this.followElement.rotation;
		base.transform.localScale = this.followElement.localScale;
	}

	// Token: 0x06002AA3 RID: 10915 RVA: 0x000D1D67 File Offset: 0x000CFF67
	private void Start()
	{
		this.followElement = ScienceExperimentManager.instance.GetElement(this.elementID);
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x06002AA4 RID: 10916 RVA: 0x000D1D87 File Offset: 0x000CFF87
	private void OnDestroy()
	{
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x04002F8B RID: 12171
	public ScienceExperimentElementID elementID;

	// Token: 0x04002F8C RID: 12172
	private Transform followElement;
}
