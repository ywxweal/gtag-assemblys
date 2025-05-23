using System;

namespace LitJson
{
	// Token: 0x02000A8D RID: 2701
	public class JsonException : ApplicationException
	{
		// Token: 0x060040E5 RID: 16613 RVA: 0x0012C729 File Offset: 0x0012A929
		public JsonException()
		{
		}

		// Token: 0x060040E6 RID: 16614 RVA: 0x0012C731 File Offset: 0x0012A931
		internal JsonException(ParserToken token)
			: base(string.Format("Invalid token '{0}' in input string", token))
		{
		}

		// Token: 0x060040E7 RID: 16615 RVA: 0x0012C749 File Offset: 0x0012A949
		internal JsonException(ParserToken token, Exception inner_exception)
			: base(string.Format("Invalid token '{0}' in input string", token), inner_exception)
		{
		}

		// Token: 0x060040E8 RID: 16616 RVA: 0x0012C762 File Offset: 0x0012A962
		internal JsonException(int c)
			: base(string.Format("Invalid character '{0}' in input string", (char)c))
		{
		}

		// Token: 0x060040E9 RID: 16617 RVA: 0x0012C77B File Offset: 0x0012A97B
		internal JsonException(int c, Exception inner_exception)
			: base(string.Format("Invalid character '{0}' in input string", (char)c), inner_exception)
		{
		}

		// Token: 0x060040EA RID: 16618 RVA: 0x0012C795 File Offset: 0x0012A995
		public JsonException(string message)
			: base(message)
		{
		}

		// Token: 0x060040EB RID: 16619 RVA: 0x0012C79E File Offset: 0x0012A99E
		public JsonException(string message, Exception inner_exception)
			: base(message, inner_exception)
		{
		}
	}
}
