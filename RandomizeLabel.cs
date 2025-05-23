using System;
using TMPro;
using UnityEngine;

// Token: 0x02000468 RID: 1128
public class RandomizeLabel : MonoBehaviour
{
	// Token: 0x06001BB9 RID: 7097 RVA: 0x00088220 File Offset: 0x00086420
	public void Randomize()
	{
		this.strings.distinct = this.distinct;
		this.label.text = this.strings.NextItem();
	}

	// Token: 0x04001EC6 RID: 7878
	public TMP_Text label;

	// Token: 0x04001EC7 RID: 7879
	public RandomStrings strings;

	// Token: 0x04001EC8 RID: 7880
	public bool distinct;
}
