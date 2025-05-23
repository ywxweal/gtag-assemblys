using System;
using System.Collections;
using TMPro;
using UnityEngine;

// Token: 0x02000590 RID: 1424
public class GRBadge : MonoBehaviour
{
	// Token: 0x060022E6 RID: 8934 RVA: 0x000AEDD5 File Offset: 0x000ACFD5
	private void OnDestroy()
	{
		GhostReactor.instance.employeeBadges.RemoveBadge(this);
	}

	// Token: 0x060022E7 RID: 8935 RVA: 0x000AEDE8 File Offset: 0x000ACFE8
	public void Setup(NetPlayer player, int index)
	{
		this.playerName.text = player.SanitizedNickName;
		this.gameEntity.onlyGrabActorNumber = player.ActorNumber;
		this.dispenserIndex = index;
		this.actorNr = player.ActorNumber;
		GRPlayer grplayer = GRPlayer.Get(player.ActorNumber);
		if (grplayer != null && (int)this.gameEntity.GetState() == 1)
		{
			base.transform.position = grplayer.badgeBodyAnchor.position;
			grplayer.AttachBadge(this);
		}
	}

	// Token: 0x060022E8 RID: 8936 RVA: 0x000AEE6B File Offset: 0x000AD06B
	public void Hide()
	{
		this.badgeMesh.enabled = false;
		this.playerName.gameObject.SetActive(false);
	}

	// Token: 0x060022E9 RID: 8937 RVA: 0x000AEE8A File Offset: 0x000AD08A
	public void UnHide()
	{
		this.badgeMesh.enabled = true;
		this.playerName.gameObject.SetActive(true);
	}

	// Token: 0x060022EA RID: 8938 RVA: 0x000AEEAC File Offset: 0x000AD0AC
	public void StartRetracting()
	{
		GameEntityManager.instance.RequestState(this.gameEntity.id, 1L);
		this.PlayAttachFx();
		if (this.retractCoroutine != null)
		{
			base.StopCoroutine(this.retractCoroutine);
		}
		this.retractCoroutine = base.StartCoroutine(this.RetractCoroutine());
	}

	// Token: 0x060022EB RID: 8939 RVA: 0x000AEEFE File Offset: 0x000AD0FE
	private IEnumerator RetractCoroutine()
	{
		base.transform.localRotation = Quaternion.identity;
		Vector3 vector = base.transform.localPosition;
		for (float num = vector.sqrMagnitude; num > 1E-05f; num = vector.sqrMagnitude)
		{
			vector = Vector3.MoveTowards(vector, Vector3.zero, this.retractSpeed * Time.deltaTime);
			base.transform.localPosition = vector;
			yield return null;
			vector = base.transform.localPosition;
		}
		base.transform.localPosition = Vector3.zero;
		yield break;
	}

	// Token: 0x060022EC RID: 8940 RVA: 0x000AEF0D File Offset: 0x000AD10D
	private void PlayAttachFx()
	{
		if (this.audioSource != null)
		{
			this.audioSource.volume = this.badgeAttachSoundVolume;
			this.audioSource.clip = this.badgeAttachSound;
			this.audioSource.Play();
		}
	}

	// Token: 0x040026FD RID: 9981
	[SerializeField]
	private GameEntity gameEntity;

	// Token: 0x040026FE RID: 9982
	[SerializeField]
	public TMP_Text playerName;

	// Token: 0x040026FF RID: 9983
	[SerializeField]
	private MeshRenderer badgeMesh;

	// Token: 0x04002700 RID: 9984
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04002701 RID: 9985
	[SerializeField]
	private float retractSpeed = 4f;

	// Token: 0x04002702 RID: 9986
	[SerializeField]
	private AudioClip badgeAttachSound;

	// Token: 0x04002703 RID: 9987
	[SerializeField]
	private float badgeAttachSoundVolume;

	// Token: 0x04002704 RID: 9988
	[SerializeField]
	public int dispenserIndex;

	// Token: 0x04002705 RID: 9989
	public int actorNr;

	// Token: 0x04002706 RID: 9990
	private Coroutine retractCoroutine;

	// Token: 0x02000591 RID: 1425
	public enum BadgeState
	{
		// Token: 0x04002708 RID: 9992
		AtDispenser,
		// Token: 0x04002709 RID: 9993
		WithPlayer
	}
}
