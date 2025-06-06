﻿using System;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaLocomotion;
using GT_CustomMapSupportRuntime;
using JetBrains.Annotations;
using UnityEngine;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000B31 RID: 2865
	public class CMSMapBoundary : CMSTrigger
	{
		// Token: 0x06004688 RID: 18056 RVA: 0x0014F428 File Offset: 0x0014D628
		public override void CopyTriggerSettings(TriggerSettings settings)
		{
			if (settings.GetType() == typeof(MapBoundarySettings))
			{
				MapBoundarySettings mapBoundarySettings = (MapBoundarySettings)settings;
				this.TeleportPoints = mapBoundarySettings.TeleportPoints;
				this.ShouldTagPlayer = mapBoundarySettings.ShouldTagPlayer;
			}
			for (int i = this.TeleportPoints.Count - 1; i >= 0; i--)
			{
				if (this.TeleportPoints[i] == null)
				{
					this.TeleportPoints.RemoveAt(i);
				}
			}
			base.CopyTriggerSettings(settings);
		}

		// Token: 0x06004689 RID: 18057 RVA: 0x0014F4AC File Offset: 0x0014D6AC
		public override void Trigger(double triggerTime = -1.0, bool originatedLocally = false, bool ignoreTriggerCount = false)
		{
			base.Trigger(triggerTime, originatedLocally, ignoreTriggerCount);
			if (originatedLocally && GTPlayer.hasInstance)
			{
				GTPlayer instance = GTPlayer.Instance;
				Transform transform = CustomMapLoader.GetCustomMapsDefaultSpawnLocation();
				if (this.TeleportPoints.Count != 0)
				{
					transform = this.TeleportPoints[Random.Range(0, this.TeleportPoints.Count)];
				}
				if (transform != null)
				{
					instance.TeleportTo(transform, true, false);
				}
				if (this.ShouldTagPlayer)
				{
					GameMode.ReportHit();
				}
			}
		}

		// Token: 0x04004918 RID: 18712
		[Tooltip("Teleport points used to return the player to the map. Chosen at random.")]
		[SerializeField]
		[NotNull]
		public List<Transform> TeleportPoints = new List<Transform>();

		// Token: 0x04004919 RID: 18713
		public bool ShouldTagPlayer = true;
	}
}
