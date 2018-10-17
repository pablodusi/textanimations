using UnityEngine;
using UnityEngine.UI;

public class TextIndividualLetterConfig : MonoBehaviour 
{
	public int baseFontSize;
	public Vector3 distanceBetweenLetters;
	public Vector2 sizeRectTransformForThisFontSize;
	private Text _text;

	public Text text
	{
		get
		{
			return _text;
		}

		set
		{
			_text = value;
		}
	}


	void Awake()
	{
		_text = GetComponent<Text> ();
	}
}
