using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{
    private Vector3 realPosition;
    private Text _text;
    private RectTransform _rectTransform;
    private LetterAnimations _letterAnimations;
    private float _realFontSize;

    void Awake()
    {
        ForceInitialize();
    }

    public void ForceInitialize()
    {
        _text = GetComponent<Text>();
        _rectTransform = GetComponent<RectTransform>();
        _letterAnimations = GetComponent<LetterAnimations>();
    }

    public float realFontSize
    {
        get
        {
            return _realFontSize;
        }

        set
        {
            _realFontSize = value;
        }
    }
    public LetterAnimations letterAnimations
    {
        get
        {
            return _letterAnimations;
        }
    }

    public Vector3 RealPosition
    {
        get
        {
            return realPosition;
        }

        set
        {
            if(text != null)
            { 
                realPosition = value;
            }
        }
    }

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

    public RectTransform rectTransform
    {
        get
        {
            return _rectTransform;
        }

        set
        {
            _rectTransform = value;
        }
    }

}
