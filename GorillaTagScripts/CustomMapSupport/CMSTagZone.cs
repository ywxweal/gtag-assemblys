using System;
using GorillaGameModes;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000B34 RID: 2868
	public class CMSTagZone : CMSTrigger
	{
		// Token: 0x060046A3 RID: 18083 RVA: 0x0014FFCF File Offset: 0x0014E1CF
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
