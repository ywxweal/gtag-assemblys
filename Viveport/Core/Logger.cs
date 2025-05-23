using System;
using System.Reflection;

namespace Viveport.Core
{
	// Token: 0x02000A88 RID: 2696
	public class Logger
	{
		// Token: 0x06004066 RID: 16486 RVA: 0x0012B778 File Offset: 0x00129978
		public static void Log(string message)
		{
			if (!Logger._hasDetected || Logger._usingUnityLog)
			{
				Logger.UnityLog(message);
				return;
			}
			Logger.ConsoleLog(message);
		}

		// Token: 0x06004067 RID: 16487 RVA: 0x0012B795 File Offset: 0x00129995
		private static void ConsoleLog(string message)
		{
			Console.WriteLine(message);
			Logger._hasDetected = true;
		}

		// Token: 0x06004068 RID: 16488 RVA: 0x0012B7A4 File Offset: 0x001299A4
		private static void UnityLog(string message)
		{
			try
			{
				if (Logger._unityLogType == null)
				{
					Logger._unityLogType = Logger.GetType("UnityEngine.Debug");
				}
				Logger._unityLogType.GetMethod("Log", new Type[] { typeof(string) }).Invoke(null, new object[] { message });
				Logger._usingUnityLog = true;
			}
			catch (Exception)
			{
				Logger.ConsoleLog(message);
				Logger._usingUnityLog = false;
			}
			Logger._hasDetected = true;
		}

		// Token: 0x06004069 RID: 16489 RVA: 0x0012B830 File Offset: 0x00129A30
		private static Type GetType(string typeName)
		{
			Type type = Type.GetType(typeName);
			if (type != null)
			{
				return type;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				type = assemblies[i].GetType(typeName);
				if (type != null)
				{
					return type;
				}
			}
			return null;
		}

		// Token: 0x040043C2 RID: 17346
		private const string LoggerTypeNameUnity = "UnityEngine.Debug";

		// Token: 0x040043C3 RID: 17347
		private static bool _hasDetected;

		// Token: 0x040043C4 RID: 17348
		private static bool _usingUnityLog = true;

		// Token: 0x040043C5 RID: 17349
		private static Type _unityLogType;
	}
}
