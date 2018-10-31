using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{
    private Vector3 localPosition;
    private Text _text;
    private RectTransform _rectTransform;

    void Awake()
    {
        ForceInitialize();
    }

    public void ForceInitialize()
    {
        _text = GetComponent<Text>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public Vector3 LocalPosition
    {
        get
        {
            return localPosition;
        }

        set
        {
            if(text != null)
            { 
                localPosition = value;
                text.rectTransform.localPosition = localPosition;
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
