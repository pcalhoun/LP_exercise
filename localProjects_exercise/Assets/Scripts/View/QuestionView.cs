using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class QuestionView : MonoBehaviour {

	public Text Prompt;

	public void SetQuestion(QuestionModel model)
	{
		Prompt.text = model.Prompt;
	}
}
