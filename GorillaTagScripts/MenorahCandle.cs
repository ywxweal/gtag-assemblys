using System;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000B1F RID: 2847
	public class MenorahCandle : MonoBehaviourPun
	{
		// Token: 0x06004614 RID: 17940 RVA: 0x000023F4 File Offset: 0x000005F4
		private void Awake()
		{
		}

		// Token: 0x06004615 RID: 17941 RVA: 0x0014D070 File Offset: 0x0014B270
		private void Start()
		{
			this.EnableCandle(false);
			this.EnableFlame(false);
			this.litDate = new DateTime(this.year, this.month, this.day);
			this.currentDate = DateTime.Now;
			this.EnableCandle(this.CandleShouldBeVisible());
			this.EnableFlame(false);
			GorillaComputer instance = GorillaComputer.instance;
			instance.OnServerTimeUpdated = (Action)Delegate.Combine(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
		}

		// Token: 0x06004616 RID: 17942 RVA: 0x0014D0EE File Offset: 0x0014B2EE
		private void UpdateMenorah()
		{
			this.EnableCandle(this.CandleShouldBeVisible());
			if (this.ShouldLightCandle())
			{
				this.EnableFlame(true);
				return;
			}
			if (this.ShouldSnuffCandle())
			{
				this.EnableFlame(false);
			}
		}

		// Token: 0x06004617 RID: 17943 RVA: 0x0014D11B File Offset: 0x0014B31B
		private void OnTimeChanged()
		{
			this.currentDate = GorillaComputer.instance.GetServerTime();
			this.UpdateMenorah();
		}

		// Token: 0x06004618 RID: 17944 RVA: 0x0014D135 File Offset: 0x0014B335
		public void OnTimeEventStart()
		{
			this.activeTimeEventDay = true;
			this.UpdateMenorah();
		}

		// Token: 0x06004619 RID: 17945 RVA: 0x0014D144 File Offset: 0x0014B344
		public void OnTimeEventEnd()
		{
			this.activeTimeEventDay = false;
			this.UpdateMenorah();
		}

		// Token: 0x0600461A RID: 17946 RVA: 0x0014D153 File Offset: 0x0014B353
		private void EnableCandle(bool enable)
		{
			if (this.candle)
			{
				this.candle.SetActive(enable);
			}
		}

		// Token: 0x0600461B RID: 17947 RVA: 0x0014D16E File Offset: 0x0014B36E
		private bool CandleShouldBeVisible()
		{
			return this.currentDate >= this.litDate;
		}

		// Token: 0x0600461C RID: 17948 RVA: 0x0014D181 File Offset: 0x0014B381
		private void EnableFlame(bool enable)
		{
			if (this.flame)
			{
				this.flame.SetActive(enable);
			}
		}

		// Token: 0x0600461D RID: 17949 RVA: 0x0014D19C File Offset: 0x0014B39C
		private bool ShouldLightCandle()
		{
			return !this.activeTimeEventDay && this.CandleShouldBeVisible() && !this.flame.activeSelf;
		}

		// Token: 0x0600461E RID: 17950 RVA: 0x0014D1BE File Offset: 0x0014B3BE
		private bool ShouldSnuffCandle()
		{
			return this.activeTimeEventDay && this.flame.activeSelf;
		}

		// Token: 0x0600461F RID: 17951 RVA: 0x0014D1D5 File Offset: 0x0014B3D5
		private void OnDestroy()
		{
			if (GorillaComputer.instance)
			{
				GorillaComputer instance = GorillaComputer.instance;
				instance.OnServerTimeUpdated = (Action)Delegate.Remove(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
			}
		}

		// Token: 0x040048B7 RID: 18615
		public int day;

		// Token: 0x040048B8 RID: 18616
		public int month;

		// Token: 0x040048B9 RID: 18617
		public int year;

		// Token: 0x040048BA RID: 18618
		public GameObject flame;

		// Token: 0x040048BB RID: 18619
		public GameObject candle;

		// Token: 0x040048BC RID: 18620
		private DateTime litDate;

		// Token: 0x040048BD RID: 18621
		private bool activeTimeEventDay;

		// Token: 0x040048BE RID: 18622
		private DateTime currentDate;
	}
}
