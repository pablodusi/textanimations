using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/*
 * 			Porcentaje de subida y bajada de fontSize en vez de startSize y endSize
 * 			Que el deltaSize del RectTransform vaya cambiando proporcionalmente al cambio de fuente
 */ 		
//[ExecuteInEditMode]
public class WordAnimator : MonoBehaviour 
{
	public string word;
	public List<TextIndividualLetterConfig> fontsIndividualLetterConfig;
	public FontsEnum font;
	public AnimationTypeEnum animationType;
	public int fontSize;
	public bool startAtBegining;
	public bool isPlaying;
	public float speed = 1f;
	public float startRotation = 0f;
	public float endRotation = 360f;
	public int startSize = 76;
	public int endSize = 100;
	public Color startColor = Color.white;
	public Color endColor = Color.white;
	public bool loop = true;
	public float fontSizeNormalizedPercentDiff;
	public GameObject letterPrefab;
	public GameObject canvasPrefab;
    public Text textTest;

	private List<Text> lettersText;
	private Canvas canvas;
	private float lerp;
    private int fontSizeOld;


    public delegate void FontSizeChange(int newValue);
    public event FontSizeChange OnFontSizeChange;

    public delegate void FontChange(FontsEnum newFont);
    public event FontChange OnFontChange;

    public delegate void ChangeAnimation(AnimationTypeEnum newAnimation);
    public event ChangeAnimation OnChangeAnimation;

    public AnimationTypeEnum ÁnimationType
    {
        get
        {
            return animationType;
        }

        set
        {
            if(value != animationType)
            {
                animationType = value;

                if(OnChangeAnimation != null)
                {
                    OnChangeAnimation(animationType);
                }
            }
        }
    }

    public int FontSize
    {
        get
        {
            return fontSize;
        }

        set
        {
            if(fontSize != value)
            {
                fontSize = value;

                if(OnFontSizeChange != null)
                {
                    OnFontSizeChange(fontSize);
                }
            }
        }
    }

    public FontsEnum Font
    {
        get
        {
            return font;
        }

        set
        {
            if(font != value)
            {
                font = value;
                
                if(OnFontChange != null)
                {
                    OnFontChange(font);
                }
            }
        }
    }

	public float GetNormalizedPercentage(float baseValor,float referenceValor)
	{
		return (referenceValor / baseValor);
	}

	void Awake()
	{
        foreach(TextIndividualLetterConfig textIndividualLetterConfig in fontsIndividualLetterConfig)
        {
            textIndividualLetterConfig.ForceInitialize();
        }

        fontSizeOld = fontSize;

        CreateLetters ();		

		if (startAtBegining) 
		{
			Play ();
		}

        //textTest.text = "PROBANDO\nPROBANDO";

    }

    void Start()
    {
        OnFontSizeChange += HandleOnFontSizeChange;
        OnFontChange += HandleOnFontChange;
        OnChangeAnimation += HandleOnChangeAnimation;
    }

    void OnDestroy()
    {
        OnFontSizeChange -= HandleOnFontSizeChange;
        OnFontChange -= HandleOnFontChange;
        OnChangeAnimation -= HandleOnChangeAnimation;
    }

    private void HandleOnChangeAnimation(AnimationTypeEnum newAnimation)
    {
        Stop();
        Play();
    }

    private void HandleOnFontChange(FontsEnum newFont)
    {
        CreateLetters();
    }

    private void HandleOnFontSizeChange(int newValue)
    {

          for(int i = 0; i < lettersText.Count; i++)
          {
                lettersText[i].rectTransform.localPosition = fontsIndividualLetterConfig[(int)font].text.rectTransform.localPosition + (i * GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize,fontSize) * fontsIndividualLetterConfig[(int)font].distanceBetweenLetters);
                //  ATTENTION: The sizeDelta has to be set on the font letter prefab with the exact amount it's setted in the WordAnimator script. Otherwise, it will be errors.
                lettersText[i].rectTransform.sizeDelta = new Vector2(GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize, fontSize) * fontsIndividualLetterConfig[(int)font].sizeRectTransformForThisFontSize.x, GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize, fontSize) * fontsIndividualLetterConfig[(int)font].sizeRectTransformForThisFontSize.y);
                lettersText[i].fontSize = newValue;
          }

        //fontsIndividualLetterConfig[(int)font].baseFontSize;
        //fontsIndividualLetterConfig[(int)font].distanceBetweenLetters
        //fontsIndividualLetterConfig[¨(int)font].sizeRectTransformForThisFontSize

        //letter.rectTransform.localPosition =    
    }

    public void CreateLetters()
	{
		if (lettersText != null) 
		{
			foreach (Text letter in lettersText)
			{
				Destroy(letter.gameObject);
			}

			lettersText.Clear ();
		}
		

		if (canvas != null)
		{
			Destroy (canvas.gameObject);
			canvas = null;
		}
		
		lettersText = new List<Text> ();
		lettersText.Clear ();
		canvas = (Canvas)GameObject.Instantiate (canvasPrefab, Vector3.zero, Quaternion.identity).GetComponent<Canvas>();
		

        canvas.name = fontsIndividualLetterConfig[(int)font].fontName;
		
		Vector3 adddeltaPosition = Vector3.zero;

        int newLineCounter = 0;
        int current = 0;

        //for (char letter in word) 
        while(current < word.Length)
        {
            if(word[current] != '/')
            {
                string lineBreaks = string.Empty;

                for(int i = 0; i < newLineCounter; i++)
                {
                    lineBreaks += "\n";
                }

                Text letterText = (Text)GameObject.Instantiate(letterPrefab,Vector3.zero,Quaternion.identity).GetComponent<Text>();
			    letterText.transform.name = "Letter";
			    letterText.transform.SetParent(canvas.gameObject.transform);

                letterText.font = fontsIndividualLetterConfig[(int)font].text.font;
                letterText.fontStyle = fontsIndividualLetterConfig[(int)font].text.fontStyle;
			    letterText.fontSize = fontSize;
			    letterText.rectTransform.sizeDelta = new Vector2(GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize,fontSize) * fontsIndividualLetterConfig[(int)font].sizeRectTransformForThisFontSize.x,GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize,fontSize) * fontsIndividualLetterConfig[(int)font].sizeRectTransformForThisFontSize.y);

			    letterText.lineSpacing = fontsIndividualLetterConfig[(int)font].text.lineSpacing;
			    letterText.alignment = fontsIndividualLetterConfig[(int)font].text.alignment;
			    letterText.alignByGeometry = fontsIndividualLetterConfig[(int)font].text.alignByGeometry;
			    letterText.horizontalOverflow = fontsIndividualLetterConfig[(int)font].text.horizontalOverflow;
			    letterText.verticalOverflow = fontsIndividualLetterConfig[(int)font].text.verticalOverflow;
			    letterText.color = fontsIndividualLetterConfig[(int)font].text.color;
			    letterText.material = fontsIndividualLetterConfig[(int)font].text.material;
			    letterText.raycastTarget = fontsIndividualLetterConfig[(int)font].text.raycastTarget;
			    letterText.text = lineBreaks + word[current].ToString();
			    //Debug.Log(letter.ToString());
			    letterText.rectTransform.position = fontsIndividualLetterConfig[(int)font].text.rectTransform.position;
			    //Debug.Log(text.rectTransform.position);
			    letterText.rectTransform.rotation = fontsIndividualLetterConfig[(int)font].text.rectTransform.rotation;
			    letterText.rectTransform.localScale = fontsIndividualLetterConfig[(int)font].text.rectTransform.localScale;
			    letterText.rectTransform.localPosition = letterText.rectTransform.position + adddeltaPosition;
			    adddeltaPosition += GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize,fontSize) * fontsIndividualLetterConfig[(int)font].distanceBetweenLetters;

			    lettersText.Add(letterText);
                //Debug.Log(adddeltaPosition);
                //Debug.Log(letterText.rectTransform.anchoredPosition);
                //Debug.Log(letterText.rectTransform.localPosition);
                //Debug.Log(adddeltaPosition);
                //Debug.Log(letterText.rectTransform.localPosition);
                //letterText.rectTransform.anchoredPosition += new Vector2(adddeltaPosition.x,adddeltaPosition.y);
            }
            else
            {
                int next = current + 1;

                if(next < word.Length)
                {
                    if (word[next] == 'n')
                    {
                        adddeltaPosition = new Vector3(0f, adddeltaPosition.y, adddeltaPosition.z);
                        newLineCounter++;
                    }

                    current++;
                }

            }
            current++;
        }
	}

	private void ClearSizeRectTransform()
	{
		foreach (Text text in lettersText) 
		{
			text.rectTransform.sizeDelta = new Vector2(512f,512f);	
		}
	}


	public void Play()
	{
		isPlaying = true;
		lerp = 0f;

        SetLettersVisible();

		switch (animationType) 
		{
			case AnimationTypeEnum.ANIMATION1:
				ClearSizeRectTransform();
				break;
			case AnimationTypeEnum.ANIMATION2:
				ClearSizeRectTransform();
				break;
			case AnimationTypeEnum.ANIMATION3:
				ClearSizeRectTransform();
				break;
			case AnimationTypeEnum.ANIMATION4:
				ClearSizeRectTransform();	
				break;
			case AnimationTypeEnum.NONE:
				break;
		}
	}

	public void Pause()
	{
		isPlaying = false;
	}

	public void Stop()
	{
		isPlaying = false;
		lerp = 0f;
	}

	void Update()
	{
        if(fontSizeOld != fontSize)
        {
            fontSizeOld = fontSize;

            if (OnFontSizeChange != null)
            {
                OnFontSizeChange(fontSize);
            }
        }

		if (isPlaying) 
		{
			lerp += Time.deltaTime * speed;

			if(lerp < 1f)
			{
				PlayFrame();
			}
			else
			{
				if(loop)
				{
					Play();
				}
				else
				{
					Stop();
                    SetLettersInvisible();
				}
			}
		}
	}

    private void SetLettersInvisible()
    {
        foreach (Text letterText in lettersText)
        {
            letterText.color = new Color(letterText.color.r,letterText.color.g,letterText.color.b,0f);
        }
    }

    private void SetLettersVisible()
    {
        foreach (Text letterText in lettersText)
        {
            letterText.color = new Color(letterText.color.r, letterText.color.g, letterText.color.b, 1f);
        }
    }

	private void PlayFrame()
	{
		switch (animationType) 
		{
			case AnimationTypeEnum.ANIMATION1:
				Animation1();	
			break;
			case AnimationTypeEnum.ANIMATION2:
				Animation2();
			break;
			case AnimationTypeEnum.ANIMATION3:
				Animation3();
				break;
			case AnimationTypeEnum.ANIMATION4:
				Animation4();
				break;
			case AnimationTypeEnum.NONE:
				break;
		}
	}

	private void Animation1()
	{
		foreach (Text t in lettersText) 
		{
            //t.fontSize = ((int)Mathf.Lerp((float)fontsIndividualLetterConfig[(int)font].text.fontSize,(float)fontsIndividualLetterConfig[(int)font].text.fontSize * (1f + fontSizeNormalizedPercentDiff),lerp));
            t.fontSize = ((int)Mathf.Lerp((float)fontSize,(float)fontSize * (1f + fontSizeNormalizedPercentDiff),lerp));
            t.color = new Color(t.color.r,t.color.g,fontsIndividualLetterConfig[(int)font].text.color.b,Mathf.Lerp(1f,0f,lerp));
			// Debug.Log("Lerp " + lerp.ToString());
		}

		//Debug.Log ((float)text.fontSize * (1f + fontSizeNormalizedPercentDiff));
	}

	private void Animation2()
	{
		foreach (Text t in lettersText) 
		{
			t.fontSize = ((int)Mathf.Lerp((float)fontSize, (float)fontSize * (1f - fontSizeNormalizedPercentDiff),lerp));
			t.color = new Color(t.color.r,t.color.g,fontsIndividualLetterConfig[(int)font].text.color.b,Mathf.Lerp(1f,0f,lerp));
			// Debug.Log("Lerp " + lerp.ToString());
		}

	}

	private void Animation3()
	{
		foreach (Text t in lettersText)
		{
			t.fontSize = ((int)Mathf.Lerp((float)fontSize,(float)fontSize * (1f + fontSizeNormalizedPercentDiff),lerp));
			t.color = new Color(t.color.r,t.color.g,t.color.b,Mathf.Lerp(0f,1f,lerp));
		}
	}

	private void Animation4()
	{
		foreach (Text t in lettersText)
		{
			t.fontSize = ((int)Mathf.Lerp((float)fontSize,(float)fontSize * (1f - fontSizeNormalizedPercentDiff),lerp));
			t.color = new Color(t.color.r,t.color.g,t.color.b,Mathf.Lerp(0f,1f,lerp));
		}
	}
}
