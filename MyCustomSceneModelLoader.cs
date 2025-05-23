using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200036D RID: 877
public class MyCustomSceneModelLoader : OVRSceneModelLoader
{
	// Token: 0x0600145A RID: 5210 RVA: 0x000637D1 File Offset: 0x000619D1
	private IEnumerator DelayedLoad()
	{
		yield return new WaitForSeconds(1f);
		Debug.Log("[MyCustomSceneLoader] calling OVRSceneManager.LoadSceneModel() delayed by 1 second");
		base.SceneManager.LoadSceneModel();
		yield break;
	}

	// Token: 0x0600145B RID: 5211 RVA: 0x000637E0 File Offset: 0x000619E0
	protected override void OnStart()
	{
		base.StartCoroutine(this.DelayedLoad());
	}

	// Token: 0x0600145C RID: 5212 RVA: 0x000637EF File Offset: 0x000619EF
	protected override void OnNoSceneModelToLoad()
	{
		Debug.Log("[MyCustomSceneLoader] There is no scene to load, but we don't want to trigger scene capture.");
	}
}
