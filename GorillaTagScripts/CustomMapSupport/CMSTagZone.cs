using System;
using GorillaGameModes;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000B34 RID: 2868
	public class CMSTagZone : CMSTrigger
	{
		// Token: 0x060046A4 RID: 18084 RVA: 0x001500A7 File Offset: 0x0014E2A7
		public override void Trigger(double triggerTime = -1.0, bool originatedLocally = false, bool ignoreTriggerCount = false)
		{
			base.Trigger(triggerTime, originatedLocally, ignoreTriggerCount);
			if (originatedLocally)
			{
				GameMode.ReportHit();
			}
		}
	}
}
