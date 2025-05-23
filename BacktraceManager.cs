﻿using System;
using System.Globalization;
using Backtrace.Unity;
using Backtrace.Unity.Model;
using GorillaNetworking;
using PlayFab;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000970 RID: 2416
public class BacktraceManager : MonoBehaviour
{
	// Token: 0x06003A3B RID: 14907 RVA: 0x00117186 File Offset: 0x00115386
	public virtual void Awake()
	{
		base.GetComponent<BacktraceClient>().BeforeSend = delegate(BacktraceData data)
		{
			if (new Unity.Mathematics.Random((uint)(Time.realtimeSinceStartupAsDouble * 1000.0)).NextDouble() > this.backtraceSampleRate)
			{
				return null;
			}
			return data;
		};
	}

	// Token: 0x06003A3C RID: 14908 RVA: 0x0011719F File Offset: 0x0011539F
	private void Start()
	{
		PlayFabTitleDataCache.Instance.GetTitleData("BacktraceSampleRate", delegate(string data)
		{
			if (data != null)
			{
				double.TryParse(data.Trim('"'), NumberStyles.Any, CultureInfo.InvariantCulture, out this.backtraceSampleRate);
				Debug.Log(string.Format("Set backtrace sample rate to: {0}", this.backtraceSampleRate));
			}
		}, delegate(PlayFabError e)
		{
			Debug.LogError(string.Format("Error getting Backtrace sample rate: {0}", e));
		});
	}

	// Token: 0x04003F3F RID: 16191
	public double backtraceSampleRate = 0.01;
}
