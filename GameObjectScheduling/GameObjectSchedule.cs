using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x02000E26 RID: 3622
	[CreateAssetMenu(fileName = "New Game Object Schedule", menuName = "Game Object Scheduling/Game Object Schedule", order = 0)]
	public class GameObjectSchedule : ScriptableObject
	{
		// Token: 0x170008DA RID: 2266
		// (get) Token: 0x06005AA4 RID: 23204 RVA: 0x001B9896 File Offset: 0x001B7A96
		public GameObjectSchedule.GameObjectScheduleNode[] Nodes
		{
			get
			{
				return this.nodes;
			}
		}

		// Token: 0x170008DB RID: 2267
		// (get) Token: 0x06005AA5 RID: 23205 RVA: 0x001B989E File Offset: 0x001B7A9E
		public bool InitialState
		{
			get
			{
				return this.initialState;
			}
		}

		// Token: 0x06005AA6 RID: 23206 RVA: 0x001B98A8 File Offset: 0x001B7AA8
		public int GetCurrentNodeIndex(DateTime currentDate, int startFrom = 0)
		{
			if (startFrom >= this.nodes.Length)
			{
				return int.MaxValue;
			}
			for (int i = -1; i < this.nodes.Length - 1; i++)
			{
				if (currentDate < this.nodes[i + 1].DateTime)
				{
					return i;
				}
			}
			return int.MaxValue;
		}

		// Token: 0x06005AA7 RID: 23207 RVA: 0x001B98F9 File Offset: 0x001B7AF9
		public void Validate()
		{
			if (this.validated)
			{
				return;
			}
			this._validate();
			this.validated = true;
		}

		// Token: 0x06005AA8 RID: 23208 RVA: 0x001B9914 File Offset: 0x001B7B14
		private void _validate()
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				this.nodes[i].Validate();
			}
			List<GameObjectSchedule.GameObjectScheduleNode> list = new List<GameObjectSchedule.GameObjectScheduleNode>(this.nodes);
			list.Sort((GameObjectSchedule.GameObjectScheduleNode e1, GameObjectSchedule.GameObjectScheduleNode e2) => e1.DateTime.CompareTo(e2.DateTime));
			this.nodes = list.ToArray();
		}

		// Token: 0x06005AA9 RID: 23209 RVA: 0x001B9980 File Offset: 0x001B7B80
		public static void GenerateDailyShuffle(DateTime startDate, DateTime endDate, GameObjectSchedule[] schedules)
		{
			TimeSpan timeSpan = TimeSpan.FromDays(1.0);
			int num = schedules.Length - 1;
			int num2 = schedules.Length - 2;
			DateTime dateTime = startDate;
			List<GameObjectSchedule.GameObjectScheduleNode>[] array = new List<GameObjectSchedule.GameObjectScheduleNode>[schedules.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new List<GameObjectSchedule.GameObjectScheduleNode>();
			}
			while (dateTime < endDate)
			{
				int num3 = Random.Range(0, schedules.Length - 2);
				if (num <= num3)
				{
					num3++;
					if (num2 <= num3)
					{
						num3++;
					}
				}
				else if (num2 <= num3)
				{
					num3++;
					if (num <= num3)
					{
						num3++;
					}
				}
				array[num].Add(new GameObjectSchedule.GameObjectScheduleNode
				{
					activeDateTime = dateTime.ToString(),
					activeState = false
				});
				array[num3].Add(new GameObjectSchedule.GameObjectScheduleNode
				{
					activeDateTime = dateTime.ToString(),
					activeState = true
				});
				dateTime += timeSpan;
				num2 = num;
				num = num3;
			}
			array[num].Add(new GameObjectSchedule.GameObjectScheduleNode
			{
				activeDateTime = dateTime.ToString(),
				activeState = false
			});
			for (int j = 0; j < array.Length; j++)
			{
				schedules[j].nodes = array[j].ToArray();
			}
		}

		// Token: 0x04005EB5 RID: 24245
		[SerializeField]
		private bool initialState;

		// Token: 0x04005EB6 RID: 24246
		[SerializeField]
		private GameObjectSchedule.GameObjectScheduleNode[] nodes;

		// Token: 0x04005EB7 RID: 24247
		[SerializeField]
		private SchedulingOptions options;

		// Token: 0x04005EB8 RID: 24248
		private bool validated;

		// Token: 0x02000E27 RID: 3623
		[Serializable]
		public class GameObjectScheduleNode
		{
			// Token: 0x170008DC RID: 2268
			// (get) Token: 0x06005AAB RID: 23211 RVA: 0x001B9AB7 File Offset: 0x001B7CB7
			public bool ActiveState
			{
				get
				{
					return this.activeState;
				}
			}

			// Token: 0x170008DD RID: 2269
			// (get) Token: 0x06005AAC RID: 23212 RVA: 0x001B9ABF File Offset: 0x001B7CBF
			public DateTime DateTime
			{
				get
				{
					return this.dateTime;
				}
			}

			// Token: 0x06005AAD RID: 23213 RVA: 0x001B9AC8 File Offset: 0x001B7CC8
			public void Validate()
			{
				try
				{
					this.dateTime = DateTime.Parse(this.activeDateTime, CultureInfo.InvariantCulture);
				}
				catch
				{
					this.dateTime = DateTime.MinValue;
				}
			}

			// Token: 0x04005EB9 RID: 24249
			[SerializeField]
			public string activeDateTime = "1/1/0001 00:00:00";

			// Token: 0x04005EBA RID: 24250
			[SerializeField]
			[Tooltip("Check to turn on. Uncheck to turn off.")]
			public bool activeState = true;

			// Token: 0x04005EBB RID: 24251
			private DateTime dateTime;
		}
	}
}
