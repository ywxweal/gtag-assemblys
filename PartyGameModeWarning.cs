using System;
using System.Collections;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x020001EE RID: 494
public class PartyGameModeWarning : MonoBehaviour
{
	// Token: 0x17000121 RID: 289
	// (get) Token: 0x06000B70 RID: 2928 RVA: 0x0003D3B0 File Offset: 0x0003B5B0
	public bool ShouldShowWarning
	{
		get
		{
			return FriendshipGroupDetection.Instance.IsInParty && FriendshipGroupDetection.Instance.AnyPartyMembersOutsideFriendCollider();
		}
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x0003D3CC File Offset: 0x0003B5CC
	private void Awake()
	{
		GameObject[] array = this.showParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x0003D3F7 File Offset: 0x0003B5F7
	public void Show()
	{
		this.visibleUntilTimestamp = Time.time + this.visibleDuration;
		if (this.hideCoroutine == null)
		{
			this.hideCoroutine = base.StartCoroutine(this.HideCo());
		}
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x0003D425 File Offset: 0x0003B625
	private IEnumerator HideCo()
	{
		GameObject[] array = this.showParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(true);
		}
		array = this.hideParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		float lastVisible;
		do
		{
			lastVisible = this.visibleUntilTimestamp;
			yield return new WaitForSeconds(this.visibleUntilTimestamp - Time.time);
		}
		while (lastVisible != this.visibleUntilTimestamp);
		array = this.showParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		array = this.hideParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(true);
		}
		this.hideCoroutine = null;
		yield break;
	}

	// Token: 0x04000E0B RID: 3595
	[SerializeField]
	private GameObject[] showParts;

	// Token: 0x04000E0C RID: 3596
	[SerializeField]
	private GameObject[] hideParts;

	// Token: 0x04000E0D RID: 3597
	[SerializeField]
	private float visibleDuration;

	// Token: 0x04000E0E RID: 3598
	private float visibleUntilTimestamp;

	// Token: 0x04000E0F RID: 3599
	private Coroutine hideCoroutine;
}
