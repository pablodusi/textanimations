using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{
    private Vector3 realPosition;
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
