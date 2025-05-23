using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006A4 RID: 1700
public class RandomizeTest : MonoBehaviour
{
	// Token: 0x06002A85 RID: 10885 RVA: 0x000D1714 File Offset: 0x000CF914
	private void Start()
	{
		for (int i = 0; i < 10; i++)
		{
			this.testList.Add(i);
		}
		for (int j = 0; j < 10; j++)
		{
			this.testListArray[j] = 0;
		}
		for (int k = 0; k < this.testList.Count; k++)
		{
			this.testListArray[k] = this.testList[k];
		}
		this.RandomizeList(ref this.testList);
		for (int l = 0; l < 10; l++)
		{
			this.testListArray[l] = 0;
		}
		for (int m = 0; m < this.testList.Count; m++)
		{
			this.testListArray[m] = this.testList[m];
		}
	}

	// Token: 0x06002A86 RID: 10886 RVA: 0x000D17CC File Offset: 0x000CF9CC
	public void RandomizeList(ref List<int> listToRandomize)
	{
		this.randomIterator = 0;
		while (this.randomIterator < listToRandomize.Count)
		{
			this.tempRandIndex = Random.Range(this.randomIterator, listToRandomize.Count);
			this.tempRandValue = listToRandomize[this.randomIterator];
			listToRandomize[this.randomIterator] = listToRandomize[this.tempRandIndex];
			listToRandomize[this.tempRandIndex] = this.tempRandValue;
			this.randomIterator++;
		}
	}

	// Token: 0x04002F6E RID: 12142
	public List<int> testList = new List<int>();

	// Token: 0x04002F6F RID: 12143
	public int[] testListArray = new int[10];

	// Token: 0x04002F70 RID: 12144
	public int randomIterator;

	// Token: 0x04002F71 RID: 12145
	public int tempRandIndex;

	// Token: 0x04002F72 RID: 12146
	public int tempRandValue;
}
