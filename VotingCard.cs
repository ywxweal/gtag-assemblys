using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200012B RID: 299
public class VotingCard : MonoBehaviour
{
	// Token: 0x060007D0 RID: 2000 RVA: 0x0002BA8E File Offset: 0x00029C8E
	private void MoveToOffPosition()
	{
		this._card.transform.position = this._offPosition.position;
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x0002BAAB File Offset: 0x00029CAB
	private void MoveToOnPosition()
	{
		this._card.transform.position = this._onPosition.position;
	}

	// Token: 0x060007D2 RID: 2002 RVA: 0x0002BAC8 File Offset: 0x00029CC8
	public void SetVisible(bool showVote, bool instant)
	{
		if (this._isVisible != showVote)
		{
			base.StopAllCoroutines();
		}
		if (instant)
		{
			this._card.transform.position = (showVote ? this._onPosition.position : this._offPosition.position);
			this._card.SetActive(showVote);
		}
		else if (showVote)
		{
			if (this._isVisible != showVote)
			{
				base.StartCoroutine(this.DoActivate());
			}
		}
		else
		{
			this._card.SetActive(false);
			this._card.transform.position = this._offPosition.position;
		}
		this._isVisible = showVote;
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x0002BB69 File Offset: 0x00029D69
	private IEnumerator DoActivate()
	{
		Vector3 from = this._offPosition.position;
		Vector3 to = this._onPosition.position;
		this._card.transform.position = from;
		this._card.SetActive(true);
		float lerpVal = 0f;
		while (lerpVal < 1f)
		{
			lerpVal += Time.deltaTime / this.activationTime;
			this._card.transform.position = Vector3.Lerp(from, to, lerpVal);
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000958 RID: 2392
	[SerializeField]
	private GameObject _card;

	// Token: 0x04000959 RID: 2393
	[SerializeField]
	private Transform _offPosition;

	// Token: 0x0400095A RID: 2394
	[SerializeField]
	private Transform _onPosition;

	// Token: 0x0400095B RID: 2395
	[SerializeField]
	private float activationTime = 0.5f;

	// Token: 0x0400095C RID: 2396
	private bool _isVisible;
}
