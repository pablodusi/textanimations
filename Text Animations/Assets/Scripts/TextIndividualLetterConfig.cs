using UnityEngine;
using UnityEngine.UI;

public class TextIndividualLetterConfig : MonoBehaviour 
{
	public int baseFontSize;
	public Vector3 distanceBetweenLetters;
	public Vector2 sizeRectTransformForThisFontSize;
    public string fontName;
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
        Initialize();
	}

    public void ForceInitialize()
    {
        Initialize();
    }

    private void Initialize()
    {
        _text = GetComponent<Text>();
    }
}
