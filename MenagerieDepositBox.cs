using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200007E RID: 126
public class MenagerieDepositBox : MonoBehaviour
{
	// Token: 0x06000338 RID: 824 RVA: 0x0001397C File Offset: 0x00011B7C
	public void OnTriggerEnter(Collider other)
	{
		MenagerieCritter component = other.transform.parent.parent.GetComponent<MenagerieCritter>();
		if (component.IsNotNull())
		{
			MenagerieCritter menagerieCritter = component;
			menagerieCritter.OnReleased = (Action<MenagerieCritter>)Delegate.Combine(menagerieCritter.OnReleased, this.OnCritterInserted);
		}
	}

	// Token: 0x06000339 RID: 825 RVA: 0x000139C4 File Offset: 0x00011BC4
	public void OnTriggerExit(Collider other)
	{
		MenagerieCritter component = other.transform.parent.GetComponent<MenagerieCritter>();
		if (component.IsNotNull())
		{
			MenagerieCritter menagerieCritter = component;
			menagerieCritter.OnReleased = (Action<MenagerieCritter>)Delegate.Remove(menagerieCritter.OnReleased, this.OnCritterInserted);
		}
	}

	// Token: 0x040003CB RID: 971
	public Action<MenagerieCritter> OnCritterInserted;
}
