using System;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000B1F RID: 2847
	public class MenorahCandle : MonoBehaviourPun
	{
		// Token: 0x06004615 RID: 17941 RVA: 0x000023F4 File Offset: 0x000005F4
		private void Awake()
		{
		}

		// Token: 0x06004616 RID: 17942 RVA: 0x0014D148 File Offset: 0x0014B348
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

		// Token: 0x06004617 RID: 17943 RVA: 0x0014D1C6 File Offset: 0x0014B3C6
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

		// Token: 0x06004618 RID: 17944 RVA: 0x0014D1F3 File Offset: 0x0014B3F3
		private void OnTimeChanged()
		{
			this.currentDate = GorillaComputer.instance.GetServerTime();
			this.UpdateMenorah();
		}

		// Token: 0x06004619 RID: 17945 RVA: 0x0014D20D File Offset: 0x0014B40D
		public void OnTimeEventStart()
		{
			this.activeTimeEventDay = true;
			this.UpdateMenorah();
		}

		// Token: 0x0600461A RID: 17946 RVA: 0x0014D21C File Offset: 0x0014B41C
		public void OnTimeEventEnd()
		{
			this.activeTimeEventDay = false;
			this.UpdateMenorah();
		}

		// Token: 0x0600461B RID: 17947 RVA: 0x0014D22B File Offset: 0x0014B42B
		private void EnableCandle(bool enable)
		{
			if (this.candle)
			{
				this.candle.SetActive(enable);
			}
		}

		// Token: 0x0600461C RID: 17948 RVA: 0x0014D246 File Offset: 0x0014B446
		private bool CandleShouldBeVisible()
		{
			return this.currentDate >= this.litDate;
		}

		// Token: 0x0600461D RID: 17949 RVA: 0x0014D259 File Offset: 0x0014B459
		private void EnableFlame(bool enable)
		{
			if (this.flame)
			{
				this.flame.SetActive(enable);
			}
		}

		// Token: 0x0600461E RID: 17950 RVA: 0x0014D274 File Offset: 0x0014B474
		private bool ShouldLightCandle()
		{
			return !this.activeTimeEventDay && this.CandleShouldBeVisible() && !this.flame.activeSelf;
		}

		// Token: 0x0600461F RID: 17951 RVA: 0x0014D296 File Offset: 0x0014B496
		private bool ShouldSnuffCandle()
		{
			return this.activeTimeEventDay && this.flame.activeSelf;
		}

		// Token: 0x06004620 RID: 17952 RVA: 0x0014D2AD File Offset: 0x0014B4AD
		private void OnDestroy()
		{
			if (GorillaComputer.instance)
			{
				GorillaComputer instance = GorillaComputer.instance;
				instance.OnServerTimeUpdated = (Action)Delegate.Remove(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
			}
		}

		// Token: 0x040048B8 RID: 18616
		public int day;

		// Token: 0x040048B9 RID: 18617
		public int month;

		// Token: 0x040048BA RID: 18618
		public int year;

		// Token: 0x040048BB RID: 18619
		public GameObject flame;

		// Token: 0x040048BC RID: 18620
		public GameObject candle;

		// Token: 0x040048BD RID: 18621
		private DateTime litDate;

		// Token: 0x040048BE RID: 18622
		private bool activeTimeEventDay;

		// Token: 0x040048BF RID: 18623
		private DateTime currentDate;
	}
}
