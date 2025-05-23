using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006A4 RID: 1700
public class RandomizeTest : MonoBehaviour
{
	// Token: 0x06002A86 RID: 10886 RVA: 0x000D17B8 File Offset: 0x000CF9B8
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

	// Token: 0x06002A87 RID: 10887 RVA: 0x000D1870 File Offset: 0x000CFA70
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

	// Token: 0x04002F70 RID: 12144
	public List<int> testList = new List<int>();

	// Token: 0x04002F71 RID: 12145
	public int[] testListArray = new int[10];

	// Token: 0x04002F72 RID: 12146
	public int randomIterator;

	// Token: 0x04002F73 RID: 12147
	public int tempRandIndex;

	// Token: 0x04002F74 RID: 12148
	public int tempRandValue;
}
