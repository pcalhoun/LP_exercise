using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestionModel
{
	public string ID;
	public string Prompt;

	private List<Token> _tokenList;
	public List<Token> TokenList {
		get { return _tokenList; }
	}

	private string _blob;
	public string Blob {
		set { 
			_blob = value;
			string[] tokenData = _blob.Split (',');
			_tokenList = new List<Token>();
			for (int i = 0; i < tokenData.Length; i += 3) {
				Token tkn = new Token();
				tkn.ID = tokenData[i];
				tkn.yes = int.Parse(tokenData[i+1]);
				tkn.no = int.Parse(tokenData[i+2]);
				_tokenList.Add(tkn);
			}
		}
		get { 
			// Not implemented.
			return _blob;
		}
	}
}
