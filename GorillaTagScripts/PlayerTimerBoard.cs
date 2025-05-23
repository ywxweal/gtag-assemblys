using System;
using System.Collections.Generic;
using System.Text;
using KID.Model;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000B20 RID: 2848
	public class PlayerTimerBoard : MonoBehaviour
	{
		// Token: 0x170006D5 RID: 1749
		// (get) Token: 0x06004621 RID: 17953 RVA: 0x0014D20D File Offset: 0x0014B40D
		// (set) Token: 0x06004622 RID: 17954 RVA: 0x0014D215 File Offset: 0x0014B415
		public bool IsDirty { get; set; } = true;

		// Token: 0x06004623 RID: 17955 RVA: 0x0014D21E File Offset: 0x0014B41E
		private void Start()
		{
			this.TryInit();
		}

		// Token: 0x06004624 RID: 17956 RVA: 0x0014D21E File Offset: 0x0014B41E
		private void OnEnable()
		{
			this.TryInit();
		}

		// Token: 0x06004625 RID: 17957 RVA: 0x0014D226 File Offset: 0x0014B426
		private void TryInit()
		{
			if (this.isInitialized)
			{
				return;
			}
			if (PlayerTimerManager.instance == null)
			{
				return;
			}
			PlayerTimerManager.instance.RegisterTimerBoard(this);
			this.isInitialized = true;
		}

		// Token: 0x06004626 RID: 17958 RVA: 0x0014D251 File Offset: 0x0014B451
		private void OnDisable()
		{
			if (PlayerTimerManager.instance != null)
			{
				PlayerTimerManager.instance.UnregisterTimerBoard(this);
			}
			this.isInitialized = false;
		}

		// Token: 0x06004627 RID: 17959 RVA: 0x0014D272 File Offset: 0x0014B472
		public void SetSleepState(bool awake)
		{
			this.playerColumn.enabled = awake;
			this.timeColumn.enabled = awake;
			if (this.linesParent != null)
			{
				this.linesParent.SetActive(awake);
			}
		}

		// Token: 0x06004628 RID: 17960 RVA: 0x0014D2A6 File Offset: 0x0014B4A6
		public void SortLines()
		{
			this.lines.Sort(new Comparison<PlayerTimerBoardLine>(PlayerTimerBoardLine.CompareByTotalTime));
		}

		// Token: 0x06004629 RID: 17961 RVA: 0x0014D2C0 File Offset: 0x0014B4C0
		public void RedrawPlayerLines()
		{
			this.stringBuilder.Clear();
			this.stringBuilderTime.Clear();
			this.stringBuilder.Append("<b><color=yellow>PLAYER</color></b>");
			this.stringBuilderTime.Append("<b><color=yellow>LATEST TIME</color></b>");
			this.SortLines();
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
			bool flag = (permissionDataByFeature.Enabled || permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PLAYER) && permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.PROHIBITED;
			for (int i = 0; i < this.lines.Count; i++)
			{
				try
				{
					if (this.lines[i].gameObject.activeInHierarchy)
					{
						this.lines[i].gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, (float)(this.startingYValue - this.lineHeight * i), 0f);
						if (this.lines[i].linePlayer != null && this.lines[i].linePlayer.InRoom)
						{
							this.stringBuilder.Append("\n ");
							this.stringBuilder.Append(flag ? this.lines[i].playerNameVisible : this.lines[i].linePlayer.DefaultName);
							this.stringBuilderTime.Append("\n ");
							this.stringBuilderTime.Append(this.lines[i].playerTimeStr);
						}
					}
				}
				catch
				{
				}
			}
			this.playerColumn.text = this.stringBuilder.ToString();
			this.timeColumn.text = this.stringBuilderTime.ToString();
			this.IsDirty = false;
		}

		// Token: 0x040048BF RID: 18623
		[SerializeField]
		private GameObject linesParent;

		// Token: 0x040048C0 RID: 18624
		public List<PlayerTimerBoardLine> lines;

		// Token: 0x040048C1 RID: 18625
		public TextMeshPro notInRoomText;

		// Token: 0x040048C2 RID: 18626
		public TextMeshPro playerColumn;

		// Token: 0x040048C3 RID: 18627
		public TextMeshPro timeColumn;

		// Token: 0x040048C4 RID: 18628
		[SerializeField]
		private int startingYValue;

		// Token: 0x040048C5 RID: 18629
		[SerializeField]
		private int lineHeight;

		// Token: 0x040048C6 RID: 18630
		private StringBuilder stringBuilder = new StringBuilder(220);

		// Token: 0x040048C7 RID: 18631
		private StringBuilder stringBuilderTime = new StringBuilder(220);

		// Token: 0x040048C8 RID: 18632
		private bool isInitialized;
	}
}
