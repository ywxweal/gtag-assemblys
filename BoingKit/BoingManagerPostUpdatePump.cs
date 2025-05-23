using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E6A RID: 3690
	public class BoingManagerPostUpdatePump : MonoBehaviour
	{
		// Token: 0x06005C53 RID: 23635 RVA: 0x0005FFE0 File Offset: 0x0005E1E0
		private void Start()
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}

		// Token: 0x06005C54 RID: 23636 RVA: 0x001C509F File Offset: 0x001C329F
		private bool TryDestroyDuplicate()
		{
			if (BoingManager.s_managerGo == base.gameObject)
			{
				return false;
			}
			Object.Destroy(base.gameObject);
			return true;
		}

		// Token: 0x06005C55 RID: 23637 RVA: 0x001C50C1 File Offset: 0x001C32C1
		private void FixedUpdate()
		{
			if (this.TryDestroyDuplicate())
			{
				return;
			}
			BoingManager.Execute(BoingManager.UpdateMode.FixedUpdate);
		}

		// Token: 0x06005C56 RID: 23638 RVA: 0x001C50D2 File Offset: 0x001C32D2
		private void Update()
		{
			if (this.TryDestroyDuplicate())
			{
				return;
			}
			BoingManager.Execute(BoingManager.UpdateMode.EarlyUpdate);
			BoingManager.PullBehaviorResults(BoingManager.UpdateMode.EarlyUpdate);
			BoingManager.PullReactorResults(BoingManager.UpdateMode.EarlyUpdate);
			BoingManager.PullBonesResults(BoingManager.UpdateMode.EarlyUpdate);
		}

		// Token: 0x06005C57 RID: 23639 RVA: 0x001C50F5 File Offset: 0x001C32F5
		private void LateUpdate()
		{
			if (this.TryDestroyDuplicate())
			{
				return;
			}
			BoingManager.PullBehaviorResults(BoingManager.UpdateMode.FixedUpdate);
			BoingManager.PullReactorResults(BoingManager.UpdateMode.FixedUpdate);
			BoingManager.PullBonesResults(BoingManager.UpdateMode.FixedUpdate);
			BoingManager.Execute(BoingManager.UpdateMode.LateUpdate);
			BoingManager.PullBehaviorResults(BoingManager.UpdateMode.LateUpdate);
			BoingManager.PullReactorResults(BoingManager.UpdateMode.LateUpdate);
			BoingManager.PullBonesResults(BoingManager.UpdateMode.LateUpdate);
		}
	}
}
