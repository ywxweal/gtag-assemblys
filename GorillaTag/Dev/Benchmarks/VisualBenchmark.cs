using System;
using System.Collections.Generic;
using System.Text;
using Unity.Profiling;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;

namespace GorillaTag.Dev.Benchmarks
{
	// Token: 0x02000D71 RID: 3441
	public class VisualBenchmark : MonoBehaviour
	{
		// Token: 0x060055D5 RID: 21973 RVA: 0x001A1AA0 File Offset: 0x0019FCA0
		protected void Awake()
		{
			Application.quitting += delegate
			{
				VisualBenchmark.isQuitting = true;
			};
			List<ProfilerRecorderHandle> list = new List<ProfilerRecorderHandle>(5500);
			ProfilerRecorderHandle.GetAvailable(list);
			Debug.Log(string.Format("poop Available stats: {0}", list.Count), this);
			List<VisualBenchmark.StatInfo> list2 = new List<VisualBenchmark.StatInfo>(600);
			foreach (ProfilerRecorderHandle profilerRecorderHandle in list)
			{
				ProfilerRecorderDescription description = ProfilerRecorderHandle.GetDescription(profilerRecorderHandle);
				if (description.Category == ProfilerCategory.Render)
				{
					list2.Add(new VisualBenchmark.StatInfo
					{
						name = description.Name,
						unit = description.UnitType
					});
				}
			}
			this.availableRenderStats = list2.ToArray();
			Debug.Log(string.Format("poop availableRenderStats: {0}", list2.Count), this);
			List<Transform> list3 = new List<Transform>(this.benchmarkLocations.Length);
			foreach (Transform transform in this.benchmarkLocations)
			{
				if (transform != null)
				{
					list3.Add(transform);
				}
			}
			this.benchmarkLocations = list3.ToArray();
		}

		// Token: 0x060055D6 RID: 21974 RVA: 0x001A1C04 File Offset: 0x0019FE04
		protected void OnEnable()
		{
			this.renderStatsRecorders = new ProfilerRecorder[this.availableRenderStats.Length];
			for (int i = 0; i < this.availableRenderStats.Length; i++)
			{
				this.renderStatsRecorders[i] = ProfilerRecorder.StartNew(ProfilerCategory.Render, this.availableRenderStats[i].name, 1, ProfilerRecorderOptions.Default);
			}
			this.state = VisualBenchmark.EState.Setup;
		}

		// Token: 0x060055D7 RID: 21975 RVA: 0x001A1C68 File Offset: 0x0019FE68
		protected void OnDisable()
		{
			foreach (ProfilerRecorder profilerRecorder in this.renderStatsRecorders)
			{
				profilerRecorder.Dispose();
			}
		}

		// Token: 0x060055D8 RID: 21976 RVA: 0x001A1C9C File Offset: 0x0019FE9C
		protected void LateUpdate()
		{
			if (VisualBenchmark.isQuitting)
			{
				return;
			}
			switch (this.state)
			{
			case VisualBenchmark.EState.Setup:
				Debug.Log("poop start");
				this.sb.Clear();
				this.currentLocationIndex = 0;
				this.lastTime = Time.realtimeSinceStartup;
				this.state = VisualBenchmark.EState.WaitingBeforeCollectingGarbage;
				return;
			case VisualBenchmark.EState.WaitingBeforeCollectingGarbage:
				Debug.Log("poop wait 1");
				if (Time.realtimeSinceStartup - this.lastTime >= this.collectGarbageDelay)
				{
					this.lastTime = Time.time;
					GC.Collect();
					this.state = VisualBenchmark.EState.WaitingBeforeRecordingStats;
					return;
				}
				break;
			case VisualBenchmark.EState.WaitingBeforeRecordingStats:
				Debug.Log("poop wait 2");
				if (Time.time - this.lastTime >= this.recordStatsDelay)
				{
					this.lastTime = Time.time;
					this.RecordLocationStats(this.benchmarkLocations[this.currentLocationIndex]);
					if (this.currentLocationIndex < this.benchmarkLocations.Length - 1)
					{
						this.currentLocationIndex++;
						this.state = VisualBenchmark.EState.WaitingBeforeCollectingGarbage;
						return;
					}
					this.state = VisualBenchmark.EState.TearDown;
					return;
				}
				break;
			case VisualBenchmark.EState.TearDown:
				Debug.Log("poop teardown");
				Debug.Log(this.sb.ToString());
				this.state = VisualBenchmark.EState.Setup;
				if (this.sb.Length > this.sb.Capacity)
				{
					Debug.Log("Capacity exceeded on string builder, increase string builder's capacity. " + string.Format("capacity={0}, length={1}", this.sb.Capacity, this.sb.Length), this);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x060055D9 RID: 21977 RVA: 0x001A1E1C File Offset: 0x001A001C
		private void RecordLocationStats(Transform xform)
		{
			this.sb.Append("Location: ");
			this.sb.Append(xform.name);
			this.sb.Append("\n");
			this.sb.Append("pos=");
			this.sb.Append(xform.position.ToString("F3"));
			this.sb.Append(" rot=");
			this.sb.Append(xform.rotation.ToString("F3"));
			this.sb.Append(" scale=");
			this.sb.Append(xform.lossyScale.ToString("F3"));
			this.sb.Append("\n");
			for (int i = 0; i < this.renderStatsRecorders.Length; i++)
			{
				this.sb.Append(this.availableRenderStats[i].name);
				this.sb.Append(": ");
				ProfilerMarkerDataUnit unit = this.availableRenderStats[i].unit;
				if (unit != ProfilerMarkerDataUnit.TimeNanoseconds)
				{
					if (unit == ProfilerMarkerDataUnit.Bytes)
					{
						this.sb.Append((double)this.renderStatsRecorders[i].LastValue / 1024.0);
						this.sb.Append("kb");
					}
					else
					{
						this.sb.Append(this.renderStatsRecorders[i].LastValue);
						this.sb.Append(' ');
						this.sb.Append(this.availableRenderStats[i].unit.ToString());
					}
				}
				else
				{
					this.sb.Append((double)this.renderStatsRecorders[i].LastValue / 1000000.0);
					this.sb.Append("ms");
				}
				this.sb.Append('\n');
			}
		}

		// Token: 0x0400591D RID: 22813
		[Tooltip("the camera will be moved and rotated to these spots and record stats.")]
		public Transform[] benchmarkLocations;

		// Token: 0x0400591E RID: 22814
		[Tooltip("How long to wait before calling GC.Collect() to clean up memory.")]
		public float collectGarbageDelay = 2f;

		// Token: 0x0400591F RID: 22815
		[Tooltip("How long to wait before recording stats after the camera was moved to a new location.\nThis + collectGarbageDelay is the total time spent at each location.")]
		private float recordStatsDelay = 2f;

		// Token: 0x04005920 RID: 22816
		[Tooltip("The camera to use for profiling. If null, a new camera will be created.")]
		private Camera cam;

		// Token: 0x04005921 RID: 22817
		private VisualBenchmark.StatInfo[] availableRenderStats;

		// Token: 0x04005922 RID: 22818
		private ProfilerRecorder[] renderStatsRecorders;

		// Token: 0x04005923 RID: 22819
		private static bool isQuitting = true;

		// Token: 0x04005924 RID: 22820
		private int currentLocationIndex;

		// Token: 0x04005925 RID: 22821
		private VisualBenchmark.EState state = VisualBenchmark.EState.WaitingBeforeCollectingGarbage;

		// Token: 0x04005926 RID: 22822
		private float lastTime;

		// Token: 0x04005927 RID: 22823
		private readonly StringBuilder sb = new StringBuilder(1024);

		// Token: 0x02000D72 RID: 3442
		private struct StatInfo
		{
			// Token: 0x04005928 RID: 22824
			public string name;

			// Token: 0x04005929 RID: 22825
			public ProfilerMarkerDataUnit unit;
		}

		// Token: 0x02000D73 RID: 3443
		private enum EState
		{
			// Token: 0x0400592B RID: 22827
			Setup,
			// Token: 0x0400592C RID: 22828
			WaitingBeforeCollectingGarbage,
			// Token: 0x0400592D RID: 22829
			WaitingBeforeRecordingStats,
			// Token: 0x0400592E RID: 22830
			TearDown
		}
	}
}
