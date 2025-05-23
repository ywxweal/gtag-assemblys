using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GT_CustomMapSupportRuntime;
using JetBrains.Annotations;
using UnityEngine;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000B35 RID: 2869
	public class CMSTeleporter : CMSTrigger
	{
		// Token: 0x060046A6 RID: 18086 RVA: 0x001500C4 File Offset: 0x0014E2C4
		public override void CopyTriggerSettings(TriggerSettings settings)
		{
			if (settings.GetType() == typeof(TeleporterSettings))
			{
				TeleporterSettings teleporterSettings = (TeleporterSettings)settings;
				this.TeleportPoints = teleporterSettings.TeleportPoints;
				this.matchTeleportPointRotation = teleporterSettings.matchTeleportPointRotation;
				this.maintainVelocity = teleporterSettings.maintainVelocity;
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

		// Token: 0x060046A7 RID: 18087 RVA: 0x00150154 File Offset: 0x0014E354
		public override void Trigger(double triggerTime = -1.0, bool originatedLocally = false, bool ignoreTriggerCount = false)
		{
			base.Trigger(triggerTime, originatedLocally, ignoreTriggerCount);
			if (originatedLocally && GTPlayer.hasInstance)
			{
				GTPlayer instance = GTPlayer.Instance;
				if (this.TeleportPoints.Count != 0)
				{
					Transform transform = this.TeleportPoints[Random.Range(0, this.TeleportPoints.Count)];
					if (transform != null)
					{
						instance.TeleportTo(transform, this.matchTeleportPointRotation, this.maintainVelocity);
					}
				}
			}
		}

		// Token: 0x04004929 RID: 18729
		[Tooltip("Teleport points used to return the player to the map. Chosen at random.")]
		[SerializeField]
		[NotNull]
		public List<Transform> TeleportPoints = new List<Transform>();

		// Token: 0x0400492A RID: 18730
		public bool matchTeleportPointRotation;

		// Token: 0x0400492B RID: 18731
		public bool maintainVelocity;
	}
}
