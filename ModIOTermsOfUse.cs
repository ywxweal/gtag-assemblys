using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ModIO;
using Runtime.Enums;
using TMPro;
using UnityEngine;

// Token: 0x02000722 RID: 1826
public class ModIOTermsOfUse : MonoBehaviour
{
	// Token: 0x06002D7D RID: 11645 RVA: 0x000E0E02 File Offset: 0x000DF002
	private void Awake()
	{
		this.controllerBehaviour = base.GetComponentInChildren<ControllerBehaviour>(true);
	}

	// Token: 0x06002D7E RID: 11646 RVA: 0x000E0E11 File Offset: 0x000DF011
	private void OnEnable()
	{
		this.controllerBehaviour.OnAction += this.PostUpdate;
	}

	// Token: 0x06002D7F RID: 11647 RVA: 0x000E0E2A File Offset: 0x000DF02A
	private void OnDisable()
	{
		this.controllerBehaviour.OnAction -= this.PostUpdate;
	}

	// Token: 0x06002D80 RID: 11648 RVA: 0x000E0E43 File Offset: 0x000DF043
	public void Initialize(TermsOfUse terms, Action<bool> callback)
	{
		if (terms.hash.md5hash.Length != 0)
		{
			this.termsOfUse = terms;
			this.hasTermsOfUse = true;
			this.termsAcknowledgedCallback = callback;
		}
	}

	// Token: 0x06002D81 RID: 11649 RVA: 0x000E0E6C File Offset: 0x000DF06C
	private void PostUpdate()
	{
		if (this.controllerBehaviour.IsLeftStick)
		{
			this.TurnPage(-1);
		}
		if (this.controllerBehaviour.IsRightStick)
		{
			this.TurnPage(1);
		}
		if (this.waitingForAcknowledge)
		{
			this.acceptButtonDown = this.controllerBehaviour.ButtonDown;
		}
	}

	// Token: 0x06002D82 RID: 11650 RVA: 0x000E0EBC File Offset: 0x000DF0BC
	private async void Start()
	{
		while (!this.hasTermsOfUse)
		{
			await Task.Yield();
		}
		PrivateUIRoom.AddUI(this.uiParent);
		TaskAwaiter<bool> taskAwaiter = this.UpdateTextFromTerms().GetAwaiter();
		if (!taskAwaiter.IsCompleted)
		{
			await taskAwaiter;
			TaskAwaiter<bool> taskAwaiter2;
			taskAwaiter = taskAwaiter2;
			taskAwaiter2 = default(TaskAwaiter<bool>);
		}
		if (taskAwaiter.GetResult())
		{
			await this.WaitForAcknowledgement();
			Action<bool> action = this.termsAcknowledgedCallback;
			if (action != null)
			{
				action(this.accepted);
			}
			PrivateUIRoom.RemoveUI(this.uiParent);
			Object.Destroy(base.gameObject);
			return;
		}
		for (;;)
		{
			await Task.Yield();
		}
	}

	// Token: 0x06002D83 RID: 11651 RVA: 0x000E0EF4 File Offset: 0x000DF0F4
	private async Task<bool> UpdateTextFromTerms()
	{
		this.tmpTitle.text = this.title;
		this.tmpBody.text = "Loading...";
		this.cachedTermsText = this.termsOfUse.termsOfUse + "\n\n";
		bool flag = await this.UpdateTextWithFullTerms();
		if (!flag)
		{
			this.tmpBody.text = "Failed to retrieve full Terms of Use text from mod.io.\n\nPlease restart the game and try again.";
			this.tmpBody.pageToDisplay = 1;
			this.tmpPage.text = string.Empty;
		}
		return flag;
	}

	// Token: 0x06002D84 RID: 11652 RVA: 0x000E0F38 File Offset: 0x000DF138
	public async Task<bool> UpdateTextWithFullTerms()
	{
		ResultAnd<CurrentAgreement> resultAnd = await ModIOUnityAsync.GetCurrentAgreement(AgreementType.TermsOfUse);
		bool flag;
		if (resultAnd.result.Succeeded())
		{
			this.cachedTermsText = this.cachedTermsText + this.FormatAgreementText(resultAnd.value) + "\n\n\n";
			ResultAnd<CurrentAgreement> resultAnd2 = await ModIOUnityAsync.GetCurrentAgreement(AgreementType.PrivacyPolicy);
			if (resultAnd2.result.Succeeded())
			{
				this.cachedTermsText += this.FormatAgreementText(resultAnd2.value);
				this.tmpBody.text = this.cachedTermsText;
				this.tmpBody.pageToDisplay = 1;
				await Task.Delay(100);
				this.tmpPage.text = string.Format("page {0} of {1}", this.tmpBody.pageToDisplay, this.tmpBody.textInfo.pageCount);
				this.nextButton.SetActive(this.tmpBody.textInfo.pageCount > 1);
				this.prevButton.SetActive(false);
				this.ActivateAcceptButtonGroup();
				flag = true;
			}
			else
			{
				flag = false;
			}
		}
		else
		{
			flag = false;
		}
		return flag;
	}

	// Token: 0x06002D85 RID: 11653 RVA: 0x000E0F7C File Offset: 0x000DF17C
	private string FormatAgreementText(CurrentAgreement agreement)
	{
		string text = string.Concat(new string[]
		{
			agreement.name,
			"\n\nEffective Date: ",
			agreement.dateLive.ToLongDateString(),
			"\n\n",
			agreement.content
		});
		text = Regex.Replace(text, "<!--[^>]*(-->)", "");
		text = text.Replace("<h1>", "<b>");
		text = text.Replace("</h1>", "</b>");
		text = text.Replace("<h2>", "<b>");
		text = text.Replace("</h2>", "</b>");
		text = text.Replace("<h3>", "<b>");
		text = text.Replace("</h3>", "</b>");
		text = text.Replace("<hr>", "");
		text = text.Replace("<br>", "\n");
		text = text.Replace("</li>", "</indent>\n");
		text = text.Replace("<strong>", "<b>");
		text = text.Replace("</strong>", "</b>");
		text = text.Replace("<em>", "<i>");
		text = text.Replace("</em>", "</i>");
		text = Regex.Replace(text, "<a[^>]*>{1}", "");
		text = text.Replace("</a>", "");
		Match match = Regex.Match(text, "<p[^>]*align:center[^>]*>{1}");
		while (match.Success)
		{
			text = text.Remove(match.Index, match.Length);
			text = text.Insert(match.Index, "\n<align=\"center\">");
			int num = text.IndexOf("</p>", match.Index, StringComparison.Ordinal);
			text = text.Remove(num, 4);
			text = text.Insert(num, "</align>");
			match = Regex.Match(text, "<p[^>]*align:center[^>]*>{1}");
		}
		text = text.Replace("<p>", "\n");
		text = text.Replace("</p>", "");
		text = Regex.Replace(text, "<ol[^>]*>{1}", "<ol>");
		int num2 = text.IndexOf("<ol>", StringComparison.OrdinalIgnoreCase);
		bool flag = num2 != -1;
		while (flag)
		{
			int num3 = text.IndexOf("</ol>", num2, StringComparison.OrdinalIgnoreCase);
			text = text.Remove(num2, "<ol>".Length);
			int num4 = text.IndexOf("<li>", num2, StringComparison.OrdinalIgnoreCase);
			bool flag2 = num4 != -1;
			int num5 = 0;
			while (flag2)
			{
				text = text.Remove(num4, "<li>".Length);
				text = text.Insert(num4, this.GetStringForListItemIdx_LowerAlpha(num5++));
				num3 = text.IndexOf("</ol>", num2, StringComparison.OrdinalIgnoreCase);
				num4 = text.IndexOf("<li>", num2, StringComparison.OrdinalIgnoreCase);
				flag2 = num4 != -1 && num4 < num3;
			}
			text = text.Remove(num3, "</ol>".Length);
			num2 = text.IndexOf("<ol>", StringComparison.OrdinalIgnoreCase);
			flag = num2 != -1;
		}
		text = Regex.Replace(text, "<ul[^>]*>{1}", "<ul>");
		int num6 = text.IndexOf("<ul>", StringComparison.OrdinalIgnoreCase);
		bool flag3 = num6 != -1;
		while (flag3)
		{
			int num7 = text.IndexOf("</ul>", num6, StringComparison.OrdinalIgnoreCase);
			text = text.Remove(num6, "<ul>".Length);
			int num8 = text.IndexOf("<li>", num6, StringComparison.OrdinalIgnoreCase);
			bool flag4 = num8 != -1;
			while (flag4)
			{
				text = text.Remove(num8, "<li>".Length);
				text = text.Insert(num8, "  - <indent=5%>");
				num7 = text.IndexOf("</ul>", num6, StringComparison.OrdinalIgnoreCase);
				num8 = text.IndexOf("<li>", num6, StringComparison.OrdinalIgnoreCase);
				flag4 = num8 != -1 && num8 < num7;
			}
			text = text.Remove(num7, "</ul>".Length);
			num6 = text.IndexOf("<ul>", StringComparison.OrdinalIgnoreCase);
			flag3 = num6 != -1;
		}
		text = Regex.Replace(text, "<table[^>]*>{1}", "");
		text = text.Replace("<tbody>", "");
		text = text.Replace("<tr>", "");
		text = text.Replace("<td>", "");
		text = text.Replace("<center>", "");
		text = text.Replace("</table>", "");
		text = text.Replace("</tbody>", "");
		text = text.Replace("</tr>", "\n");
		text = text.Replace("</td>", "");
		return text.Replace("</center>", "");
	}

	// Token: 0x06002D86 RID: 11654 RVA: 0x000E1400 File Offset: 0x000DF600
	private string GetStringForListItemIdx_LowerAlpha(int idx)
	{
		switch (idx)
		{
		case 0:
			return "  a. <indent=5%>";
		case 1:
			return "  b. <indent=5%>";
		case 2:
			return "  c. <indent=5%>";
		case 3:
			return "  d. <indent=5%>";
		case 4:
			return "  e. <indent=5%>";
		case 5:
			return "  f. <indent=5%>";
		case 6:
			return "  g. <indent=5%>";
		case 7:
			return "  h. <indent=5%>";
		case 8:
			return "  i. <indent=5%>";
		case 9:
			return "  j. <indent=5%>";
		case 10:
			return "  k. <indent=5%>";
		case 11:
			return "  l. <indent=5%>";
		case 12:
			return "  m. <indent=5%>";
		case 13:
			return "  n. <indent=5%>";
		case 14:
			return "  o. <indent=5%>";
		case 15:
			return "  p. <indent=5%>";
		case 16:
			return "  q. <indent=5%>";
		case 17:
			return "  r. <indent=5%>";
		case 18:
			return "  s. <indent=5%>";
		case 19:
			return "  t. <indent=5%>";
		case 20:
			return "  u. <indent=5%>";
		case 21:
			return "  v. <indent=5%>";
		case 22:
			return "  w. <indent=5%>";
		case 23:
			return "  x. <indent=5%>";
		case 24:
			return "  y. <indent=5%>";
		case 25:
			return "  z. <indent=5%>";
		default:
			return "";
		}
	}

	// Token: 0x06002D87 RID: 11655 RVA: 0x000E1524 File Offset: 0x000DF724
	private async Task WaitForAcknowledgement()
	{
		this.accepted = false;
		float progress = 0f;
		this.progressBar.transform.localScale = new Vector3(0f, 1f, 1f);
		while (progress < 1f)
		{
			if (this.acceptButtonDown)
			{
				progress += Time.deltaTime / this.holdTime;
			}
			else
			{
				progress = 0f;
			}
			this.progressBar.transform.localScale = new Vector3(Mathf.Clamp01(progress), 1f, 1f);
			this.progressBar.textureScale = new Vector2(Mathf.Clamp01(progress), -1f);
			await Task.Yield();
		}
		if (progress >= 1f)
		{
			this.Acknowledge(this.acceptButtonDown);
		}
	}

	// Token: 0x06002D88 RID: 11656 RVA: 0x000E1568 File Offset: 0x000DF768
	public void TurnPage(int i)
	{
		this.tmpBody.pageToDisplay = Mathf.Clamp(this.tmpBody.pageToDisplay + i, 1, this.tmpBody.textInfo.pageCount);
		this.tmpPage.text = string.Format("page {0} of {1}", this.tmpBody.pageToDisplay, this.tmpBody.textInfo.pageCount);
		this.nextButton.SetActive(this.tmpBody.pageToDisplay < this.tmpBody.textInfo.pageCount);
		this.prevButton.SetActive(this.tmpBody.pageToDisplay > 1);
		this.ActivateAcceptButtonGroup();
	}

	// Token: 0x06002D89 RID: 11657 RVA: 0x000E1624 File Offset: 0x000DF824
	private void ActivateAcceptButtonGroup()
	{
		bool flag = this.tmpBody.pageToDisplay == this.tmpBody.textInfo.pageCount;
		this.yesNoButtons.SetActive(flag);
		this.waitingForAcknowledge = flag;
	}

	// Token: 0x06002D8A RID: 11658 RVA: 0x000E1662 File Offset: 0x000DF862
	public void Acknowledge(bool didAccept)
	{
		this.accepted = didAccept;
	}

	// Token: 0x040033AD RID: 13229
	[SerializeField]
	private Transform uiParent;

	// Token: 0x040033AE RID: 13230
	[SerializeField]
	private string title;

	// Token: 0x040033AF RID: 13231
	[SerializeField]
	private TMP_Text tmpBody;

	// Token: 0x040033B0 RID: 13232
	[SerializeField]
	private TMP_Text tmpTitle;

	// Token: 0x040033B1 RID: 13233
	[SerializeField]
	private TMP_Text tmpPage;

	// Token: 0x040033B2 RID: 13234
	[SerializeField]
	public GameObject yesNoButtons;

	// Token: 0x040033B3 RID: 13235
	[SerializeField]
	public GameObject nextButton;

	// Token: 0x040033B4 RID: 13236
	[SerializeField]
	public GameObject prevButton;

	// Token: 0x040033B5 RID: 13237
	private TermsOfUse termsOfUse;

	// Token: 0x040033B6 RID: 13238
	private bool hasTermsOfUse;

	// Token: 0x040033B7 RID: 13239
	private Action<bool> termsAcknowledgedCallback;

	// Token: 0x040033B8 RID: 13240
	private string cachedTermsText;

	// Token: 0x040033B9 RID: 13241
	private bool waitingForAcknowledge;

	// Token: 0x040033BA RID: 13242
	private bool accepted;

	// Token: 0x040033BB RID: 13243
	private bool acceptButtonDown;

	// Token: 0x040033BC RID: 13244
	[SerializeField]
	private float holdTime = 5f;

	// Token: 0x040033BD RID: 13245
	[SerializeField]
	private LineRenderer progressBar;

	// Token: 0x040033BE RID: 13246
	private ControllerBehaviour controllerBehaviour;
}
