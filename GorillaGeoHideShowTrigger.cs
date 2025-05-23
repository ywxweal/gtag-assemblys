using System;
using GorillaExtensions;
using GorillaTag.GuidedRefs;
using UnityEngine;

// Token: 0x02000603 RID: 1539
public class GorillaGeoHideShowTrigger : GorillaTriggerBox, IGuidedRefReceiverMono, IGuidedRefMonoBehaviour, IGuidedRefObject
{
	// Token: 0x06002612 RID: 9746 RVA: 0x000BC84C File Offset: 0x000BAA4C
	protected void Awake()
	{
		((IGuidedRefObject)this).GuidedRefInitialize();
	}

	// Token: 0x06002613 RID: 9747 RVA: 0x000BC854 File Offset: 0x000BAA54
	public override void OnBoxTriggered()
	{
		if (!this._guidedRefsAreFullyResolved)
		{
			return;
		}
		if (this.makeSureThisIsDisabled != null)
		{
			foreach (GameObject gameObject in this.makeSureThisIsDisabled)
			{
				if (gameObject == null)
				{
					Debug.LogError("GorillaGeoHideShowTrigger: null item in makeSureThisIsDisabled. \"" + base.transform.GetPath() + "\"", this);
					return;
				}
				gameObject.SetActive(false);
			}
		}
		if (this.makeSureThisIsEnabled != null)
		{
			foreach (GameObject gameObject2 in this.makeSureThisIsEnabled)
			{
				if (gameObject2 == null)
				{
					Debug.LogError("GorillaGeoHideShowTrigger: null item in makeSureThisIsDisabled. \"" + base.transform.GetPath() + "\"", this);
					return;
				}
				gameObject2.SetActive(true);
			}
		}
	}

	// Token: 0x06002614 RID: 9748 RVA: 0x000BC90E File Offset: 0x000BAB0E
	void IGuidedRefObject.GuidedRefInitialize()
	{
		GuidedRefHub.RegisterReceiverArray<GorillaGeoHideShowTrigger, GameObject>(this, "makeSureThisIsDisabled", ref this.makeSureThisIsDisabled, ref this.makeSureThisIsDisabled_gRefs);
		GuidedRefHub.RegisterReceiverArray<GorillaGeoHideShowTrigger, GameObject>(this, "makeSureThisIsEnabled", ref this.makeSureThisIsEnabled, ref this.makeSureThisIsEnabled_gRefs);
		GuidedRefHub.ReceiverFullyRegistered<GorillaGeoHideShowTrigger>(this);
	}

	// Token: 0x06002615 RID: 9749 RVA: 0x000BC944 File Offset: 0x000BAB44
	bool IGuidedRefReceiverMono.GuidedRefTryResolveReference(GuidedRefTryResolveInfo target)
	{
		return GuidedRefHub.TryResolveArrayItem<GorillaGeoHideShowTrigger, GameObject>(this, this.makeSureThisIsDisabled, this.makeSureThisIsDisabled_gRefs, target) || GuidedRefHub.TryResolveArrayItem<GorillaGeoHideShowTrigger, GameObject>(this, this.makeSureThisIsDisabled, this.makeSureThisIsEnabled_gRefs, target);
	}

	// Token: 0x06002616 RID: 9750 RVA: 0x000BC975 File Offset: 0x000BAB75
	void IGuidedRefReceiverMono.OnAllGuidedRefsResolved()
	{
		this._guidedRefsAreFullyResolved = true;
	}

	// Token: 0x06002617 RID: 9751 RVA: 0x000BC97E File Offset: 0x000BAB7E
	void IGuidedRefReceiverMono.OnGuidedRefTargetDestroyed(int fieldId)
	{
		this._guidedRefsAreFullyResolved = false;
	}

	// Token: 0x170003A0 RID: 928
	// (get) Token: 0x06002618 RID: 9752 RVA: 0x000BC987 File Offset: 0x000BAB87
	// (set) Token: 0x06002619 RID: 9753 RVA: 0x000BC98F File Offset: 0x000BAB8F
	int IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount { get; set; }

	// Token: 0x0600261B RID: 9755 RVA: 0x00045F89 File Offset: 0x00044189
	Transform IGuidedRefMonoBehaviour.get_transform()
	{
		return base.transform;
	}

	// Token: 0x0600261C RID: 9756 RVA: 0x00017401 File Offset: 0x00015601
	int IGuidedRefObject.GetInstanceID()
	{
		return base.GetInstanceID();
	}

	// Token: 0x04002A98 RID: 10904
	[SerializeField]
	private GameObject[] makeSureThisIsDisabled;

	// Token: 0x04002A99 RID: 10905
	[SerializeField]
	private GuidedRefReceiverArrayInfo makeSureThisIsDisabled_gRefs = new GuidedRefReceiverArrayInfo(false);

	// Token: 0x04002A9A RID: 10906
	[SerializeField]
	private GameObject[] makeSureThisIsEnabled;

	// Token: 0x04002A9B RID: 10907
	[SerializeField]
	private GuidedRefReceiverArrayInfo makeSureThisIsEnabled_gRefs = new GuidedRefReceiverArrayInfo(false);

	// Token: 0x04002A9C RID: 10908
	private bool _guidedRefsAreFullyResolved;
}
