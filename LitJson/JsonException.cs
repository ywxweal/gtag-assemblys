using System;

namespace LitJson
{
	// Token: 0x02000A8D RID: 2701
	public class JsonException : ApplicationException
	{
		// Token: 0x060040E6 RID: 16614 RVA: 0x0012C801 File Offset: 0x0012AA01
		public JsonException()
		{
		}

		// Token: 0x060040E7 RID: 16615 RVA: 0x0012C809 File Offset: 0x0012AA09
		internal JsonException(ParserToken token)
			: base(string.Format("Invalid token '{0}' in input string", token))
		{
		}

		// Token: 0x060040E8 RID: 16616 RVA: 0x0012C821 File Offset: 0x0012AA21
		internal JsonException(ParserToken token, Exception inner_exception)
			: base(string.Format("Invalid token '{0}' in input string", token), inner_exception)
		{
		}

		// Token: 0x060040E9 RID: 16617 RVA: 0x0012C83A File Offset: 0x0012AA3A
		internal JsonException(int c)
			: base(string.Format("Invalid character '{0}' in input string", (char)c))
		{
		}

		// Token: 0x060040EA RID: 16618 RVA: 0x0012C853 File Offset: 0x0012AA53
		internal JsonException(int c, Exception inner_exception)
			: base(string.Format("Invalid character '{0}' in input string", (char)c), inner_exception)
		{
		}

		// Token: 0x060040EB RID: 16619 RVA: 0x0012C86D File Offset: 0x0012AA6D
		public JsonException(string message)
			: base(message)
		{
		}

		// Token: 0x060040EC RID: 16620 RVA: 0x0012C876 File Offset: 0x0012AA76
		public JsonException(string message, Exception inner_exception)
			: base(message, inner_exception)
		{
		}
	}
}
