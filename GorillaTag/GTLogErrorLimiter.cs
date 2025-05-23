using System;
using System.Runtime.CompilerServices;
using System.Text;
using Cysharp.Text;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D19 RID: 3353
	public class GTLogErrorLimiter
	{
		// Token: 0x1700085F RID: 2143
		// (get) Token: 0x060053E4 RID: 21476 RVA: 0x001968DB File Offset: 0x00194ADB
		// (set) Token: 0x060053E5 RID: 21477 RVA: 0x001968E3 File Offset: 0x00194AE3
		public string baseMessage
		{
			get
			{
				return this._baseMessage;
			}
			set
			{
				this._baseMessage = value ?? "__NULL__";
			}
		}

		// Token: 0x060053E6 RID: 21478 RVA: 0x001968F5 File Offset: 0x00194AF5
		public GTLogErrorLimiter(string baseMessage, int countdown = 10, string occurrencesJoinString = "\n- ")
		{
			this.baseMessage = baseMessage;
			this.countdown = countdown;
			this.sb = ZString.CreateStringBuilder();
			this.sb.Append(this.baseMessage);
			this.occurrencesJoinString = occurrencesJoinString;
		}

		// Token: 0x060053E7 RID: 21479 RVA: 0x00196930 File Offset: 0x00194B30
		public void Log(string subMessage = "", Object context = null, [CallerMemberName] string caller = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int line = 0)
		{
			if (this.countdown < 0)
			{
				return;
			}
			if (this.countdown == 0)
			{
				this.sb.Insert(0, "!!!! THIS MESSAGE HAS REACHED MAX SPAM COUNT AND WILL NO LONGER BE LOGGED !!!!\n");
			}
			this.sb.Append(subMessage ?? "__NULL__");
			this.sb.Append("\n\nError origin - Caller: ");
			this.sb.Append(caller ?? "__NULL__");
			this.sb.Append(", Line: ");
			this.sb.Append(line);
			this.sb.Append("File: ");
			this.sb.Append(sourceFilePath ?? "__NULL__");
			Debug.LogError(this.sb.ToString(), context);
			this.sb.Clear();
			this.sb.Append(this.baseMessage);
			this.countdown--;
			this.occurrenceCount = 0;
		}

		// Token: 0x060053E8 RID: 21480 RVA: 0x00196A25 File Offset: 0x00194C25
		public void Log(Object obj, Object context = null, [CallerMemberName] string caller = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int line = 0)
		{
			if (!obj)
			{
				this.Log("__NULL__", context, caller, sourceFilePath, line);
				return;
			}
			this.Log(obj.ToString(), null, "Log", "C:\\Users\\root\\GT\\Assets\\GorillaTag\\Shared\\Scripts\\MonkeFX\\GTLogErrorLimiter.cs", 137);
		}

		// Token: 0x060053E9 RID: 21481 RVA: 0x00196A5D File Offset: 0x00194C5D
		public void AddOccurrence(string s)
		{
			this.occurrenceCount++;
			this.sb.Append(this.occurrencesJoinString ?? "\n- ");
			this.sb.Append(s);
		}

		// Token: 0x060053EA RID: 21482 RVA: 0x00196A93 File Offset: 0x00194C93
		public void AddOccurrence(StringBuilder stringBuilder)
		{
			this.occurrenceCount++;
			this.sb.Append(this.occurrencesJoinString ?? "\n- ");
			this.sb.Append<StringBuilder>(stringBuilder);
		}

		// Token: 0x060053EB RID: 21483 RVA: 0x00196ACC File Offset: 0x00194CCC
		public void AddOccurence(GameObject gObj)
		{
			this.occurrenceCount++;
			if (gObj == null)
			{
				this.AddOccurrence("__NULL__");
				return;
			}
			this.sb.Append(this.occurrencesJoinString ?? "\n- ");
			this.sb.Q(gObj.GetPath());
		}

		// Token: 0x060053EC RID: 21484 RVA: 0x00196B28 File Offset: 0x00194D28
		public void LogOccurrences(Component component = null, Object obj = null, [CallerMemberName] string caller = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int line = 0)
		{
			if (this.occurrenceCount <= 0)
			{
				return;
			}
			this.sb.Insert(0, string.Format("Occurred {0} times: ", this.occurrenceCount));
			this.Log("\"" + component.GetComponentPath(int.MaxValue) + "\"", obj, caller, sourceFilePath, line);
		}

		// Token: 0x060053ED RID: 21485 RVA: 0x00196B88 File Offset: 0x00194D88
		public void LogOccurrences(Utf16ValueStringBuilder subMessage, Object obj = null, [CallerMemberName] string caller = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int line = 0)
		{
			if (this.occurrenceCount <= 0)
			{
				return;
			}
			this.sb.Insert(0, string.Format("Occurred {0} times: ", this.occurrenceCount));
			this.sb.Append<Utf16ValueStringBuilder>(subMessage);
			this.Log("", obj, caller, sourceFilePath, line);
		}

		// Token: 0x040056D9 RID: 22233
		private const string __NULL__ = "__NULL__";

		// Token: 0x040056DA RID: 22234
		public int countdown;

		// Token: 0x040056DB RID: 22235
		public int occurrenceCount;

		// Token: 0x040056DC RID: 22236
		public string occurrencesJoinString;

		// Token: 0x040056DD RID: 22237
		private string _baseMessage;

		// Token: 0x040056DE RID: 22238
		public Utf16ValueStringBuilder sb;

		// Token: 0x040056DF RID: 22239
		private const string k_lastMsgHeader = "!!!! THIS MESSAGE HAS REACHED MAX SPAM COUNT AND WILL NO LONGER BE LOGGED !!!!\n";
	}
}
