using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Service;

public class Main : MonoBehaviour {

	public QuestionView questionView;
	public TokenView tokenView;

	private DataService dataService;

	private bool _initialized = false;

	private string[] _questionIDs;
	private int _question = 0;
	private QuestionModel _questionModel;

	private Dictionary<string,TokenView> _tokens = new Dictionary<string,TokenView>();

	void Start () {
		dataService = DataService.CreateGO(); 
		dataService.GetQuestionIDs((string[] IDs)=>{_questionIDs = IDs;});
	}

	void Update () {
		if (!_initialized) {
			Initialize ();
		}
	}

	private void Initialize() {
		if (_questionIDs != null && _questionIDs.Length > 0) {
			_question = 0;
			dataService.GetQuestionModel(_questionIDs[_question],SetQuestion);
			_initialized = true;
		}
	}

	// MOVE TO CONTROLLER.

	private void NextQuestion()
	{
		if (++_question == _questionIDs.Length) {
			_question = 0;
		}
		dataService.GetQuestionModel(_questionIDs[_question],SetQuestion);
	}

	private void SetQuestion(QuestionModel model)
	{
		_questionModel = model;
		questionView.SetQuestion(model);
	}

	private void UpdateTokens(bool yes)
	{
		foreach(Token tkn in _questionModel.TokenList) {

			TokenView token;

			if (_tokens.ContainsKey (tkn.ID)) {
				token = _tokens[tkn.ID];
			} else {
				token = GameObject.Instantiate(tokenView);
				_tokens [tkn.ID] = token;
			}
			Debug.Log(tkn.ID);

			int multiplier = (yes ? tkn.yes : tkn.no);
			float s = token.gameObject.transform.localScale.x;
			s += s * (multiplier/10.0f);

			token.gameObject.transform.localScale =  new Vector3(s,s,s);
		}
	}

	public void OnClick_yes()
	{
		UpdateTokens(true);
		NextQuestion();
	}

	public void OnClick_no()
	{
		UpdateTokens(false);
		NextQuestion();
	}
}
