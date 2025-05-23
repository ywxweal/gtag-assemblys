using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020006A9 RID: 1705
public class ScienceExperimentSceneElement : MonoBehaviour, ITickSystemPost
{
	// Token: 0x17000428 RID: 1064
	// (get) Token: 0x06002A9F RID: 10911 RVA: 0x000D1C61 File Offset: 0x000CFE61
	// (set) Token: 0x06002AA0 RID: 10912 RVA: 0x000D1C69 File Offset: 0x000CFE69
	bool ITickSystemPost.PostTickRunning { get; set; }

	// Token: 0x06002AA1 RID: 10913 RVA: 0x000D1C74 File Offset: 0x000CFE74
	void ITickSystemPost.PostTick()
	{
		base.transform.position = this.followElement.position;
		base.transform.rotation = this.followElement.rotation;
		base.transform.localScale = this.followElement.localScale;
	}

	// Token: 0x06002AA2 RID: 10914 RVA: 0x000D1CC3 File Offset: 0x000CFEC3
	private void Start()
	{
		this.followElement = ScienceExperimentManager.instance.GetElement(this.elementID);
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x06002AA3 RID: 10915 RVA: 0x000D1CE3 File Offset: 0x000CFEE3
	private void OnDestroy()
	{
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x04002F89 RID: 12169
	public ScienceExperimentElementID elementID;

	// Token: 0x04002F8A RID: 12170
	private Transform followElement;
}
