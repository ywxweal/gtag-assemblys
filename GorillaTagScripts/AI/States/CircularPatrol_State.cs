using System;
using UnityEngine;

namespace GorillaTagScripts.AI.States
{
	// Token: 0x02000B9D RID: 2973
	public class CircularPatrol_State : IState
	{
		// Token: 0x060049B2 RID: 18866 RVA: 0x001600B4 File Offset: 0x0015E2B4
		public CircularPatrol_State(AIEntity entity)
		{
			this.entity = entity;
		}

		// Token: 0x060049B3 RID: 18867 RVA: 0x001600C4 File Offset: 0x0015E2C4
		public void Tick()
		{
			Vector3 position = this.entity.circleCenter.position;
			float num = position.x + Mathf.Cos(this.angle) * this.entity.angularSpeed;
			float y = position.y;
			float num2 = position.z + Mathf.Sin(this.angle) * this.entity.angularSpeed;
			this.entity.transform.position = new Vector3(num, y, num2);
			this.angle += this.entity.angularSpeed * Time.deltaTime;
		}

		// Token: 0x060049B4 RID: 18868 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnEnter()
		{
		}

		// Token: 0x060049B5 RID: 18869 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnExit()
		{
		}

		// Token: 0x04004C8A RID: 19594
		private AIEntity entity;

		// Token: 0x04004C8B RID: 19595
		private float angle;
	}
}
