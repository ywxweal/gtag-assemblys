using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using LitJson;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000C2A RID: 3114
	public class CreditsView : MonoBehaviour
	{
		// Token: 0x170007AA RID: 1962
		// (get) Token: 0x06004D1B RID: 19739 RVA: 0x0016F075 File Offset: 0x0016D275
		private int TotalPages
		{
			get
			{
				return this.creditsSections.Sum((CreditsSection section) => this.PagesPerSection(section));
			}
		}

		// Token: 0x06004D1C RID: 19740 RVA: 0x0016F090 File Offset: 0x0016D290
		private void Start()
		{
			this.creditsSections = new CreditsSection[]
			{
				new CreditsSection
				{
					Title = "DEV TEAM",
					Entries = new List<string>
					{
						"Anton \"NtsFranz\" Franzluebbers", "Carlo Grossi Jr", "Cody O'Quinn", "David Neubelt", "David \"AA_DavidY\" Yee", "Derek \"DunkTrain\" Arabian", "Elie Arabian", "John Sleeper", "Haunted Army", "Kerestell Smith",
						"Keith \"ElectronicWall\" Taylor", "Laura \"Poppy\" Lorian", "Lilly Tothill", "Matt \"Crimity\" Ostgard", "Nick Taylor", "Ross Furmidge", "Sasha \"Kayze\" Sanders"
					}
				},
				new CreditsSection
				{
					Title = "SPECIAL THANKS",
					Entries = new List<string> { "The \"Sticks\"", "Alpha Squad", "Meta", "Scout House", "Mighty PR", "Caroline Arabian", "Clarissa & Declan", "Calum Haigh", "EZ ICE", "Gwen" }
				},
				new CreditsSection
				{
					Title = "MUSIC BY",
					Entries = new List<string> { "Stunshine", "David Anderson Kirk", "Jaguar Jen", "Audiopfeil", "Owlobe" }
				}
			};
			PlayFabTitleDataCache.Instance.GetTitleData("CreditsData", delegate(string result)
			{
				this.creditsSections = JsonMapper.ToObject<CreditsSection[]>(result);
			}, delegate(PlayFabError error)
			{
				Debug.Log("Error fetching credits data: " + error.ErrorMessage);
			});
		}

		// Token: 0x06004D1D RID: 19741 RVA: 0x0016F29D File Offset: 0x0016D49D
		private int PagesPerSection(CreditsSection section)
		{
			return (int)Math.Ceiling((double)section.Entries.Count / (double)this.pageSize);
		}

		// Token: 0x06004D1E RID: 19742 RVA: 0x0016F2B9 File Offset: 0x0016D4B9
		private IEnumerable<string> PageOfSection(CreditsSection section, int page)
		{
			return section.Entries.Skip(this.pageSize * page).Take(this.pageSize);
		}

		// Token: 0x06004D1F RID: 19743 RVA: 0x0016F2DC File Offset: 0x0016D4DC
		[return: TupleElementNames(new string[] { "creditsSection", "subPage" })]
		private ValueTuple<CreditsSection, int> GetPageEntries(int page)
		{
			int num = 0;
			foreach (CreditsSection creditsSection in this.creditsSections)
			{
				int num2 = this.PagesPerSection(creditsSection);
				if (num + num2 > page)
				{
					int num3 = page - num;
					return new ValueTuple<CreditsSection, int>(creditsSection, num3);
				}
				num += num2;
			}
			return new ValueTuple<CreditsSection, int>(this.creditsSections.First<CreditsSection>(), 0);
		}

		// Token: 0x06004D20 RID: 19744 RVA: 0x0016F338 File Offset: 0x0016D538
		public void ProcessButtonPress(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.enter)
			{
				this.currentPage++;
				this.currentPage %= this.TotalPages;
			}
		}

		// Token: 0x06004D21 RID: 19745 RVA: 0x0016F360 File Offset: 0x0016D560
		public string GetScreenText()
		{
			return this.GetPage(this.currentPage);
		}

		// Token: 0x06004D22 RID: 19746 RVA: 0x0016F370 File Offset: 0x0016D570
		private string GetPage(int page)
		{
			ValueTuple<CreditsSection, int> pageEntries = this.GetPageEntries(page);
			CreditsSection item = pageEntries.Item1;
			int item2 = pageEntries.Item2;
			IEnumerable<string> enumerable = this.PageOfSection(item, item2);
			string text = "CREDITS - " + ((item2 == 0) ? item.Title : (item.Title + " (CONT)"));
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(text);
			stringBuilder.AppendLine();
			foreach (string text2 in enumerable)
			{
				stringBuilder.AppendLine(text2);
			}
			for (int i = 0; i < this.pageSize - enumerable.Count<string>(); i++)
			{
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("PRESS ENTER TO CHANGE PAGES");
			return stringBuilder.ToString();
		}

		// Token: 0x04005004 RID: 20484
		private CreditsSection[] creditsSections;

		// Token: 0x04005005 RID: 20485
		public int pageSize = 7;

		// Token: 0x04005006 RID: 20486
		private int currentPage;

		// Token: 0x04005007 RID: 20487
		private const string PlayFabKey = "CreditsData";
	}
}
