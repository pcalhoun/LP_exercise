using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class TokenView : MonoBehaviour {

	public Text ID;

	public void Set(TokenModel model)
	{
		ID.text = model.ID;
	}
}
