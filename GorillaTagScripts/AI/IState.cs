using System;

namespace GorillaTagScripts.AI
{
	// Token: 0x02000B99 RID: 2969
	public interface IState
	{
		// Token: 0x0600499D RID: 18845
		void Tick();

		// Token: 0x0600499E RID: 18846
		void OnEnter();

		// Token: 0x0600499F RID: 18847
		void OnExit();
	}
}
