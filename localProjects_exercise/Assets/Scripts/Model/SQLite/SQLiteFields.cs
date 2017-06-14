using System;

namespace Service {

	public static class SQLiteFields {

		// Question fields
		public struct Name
		{
			public const string QUESTION_ID		= "questionID";
			public const string QUESTION_PROMPT	= "questionPrompt";
			public const string QUESTION_BLOB	= "questionBlob";
			public const string TOKEN_ID		= "tokenID";
			public const string TOKEN_TEXT		= "tokenText";
			public const string TOKEN_COLOR		= "tokenColor";
			public const string TOKEN_SUM		= "tokenSum";
		}
	}
}
