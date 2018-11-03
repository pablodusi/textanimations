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
	//public string word;                                 //
	public FontsEnum font;                              
	public AnimationTypeEnum animationType;
	//public int fontSize;                                //
    //public Vector3 position;                            //
	public bool startAtBegining;
	public bool isPlaying;
    public bool loop = true;
    public bool setInvisibleWhenStops;
    public float speed = 1f;
    public float interval1;
    public float intervalBetweenAnimations = 1f;
    public float min1;
    public float max1;
    //public Color textColor;                             //
	public float fontSizeNormalizedPercentDiff;
    public List<TextIndividualLetterConfig> fontsIndividualLetterConfig;
    public GameObject letterPrefab;
	public GameObject canvasPrefab;

    private Text text;
    private RectTransform rectTransform;
	private List<Letter> lettersText;
	private Canvas canvas;
	private float lerp;
    private int fontSizeOld;
    private string oldText;
    private Vector3 oldPosition;
    private float oldTimeShake;
    private Color oldColor;
    private Material oldMaterial;
    private bool isPlayingForward;          // False is reverse
    private float timeLastAnimation;        // The time when the last animation stopped
    private AnimationTypeEnum oldAnimation;

    public delegate void FontSizeChange(int newValue);
    public event FontSizeChange OnFontSizeChange;

    public delegate void FontChange(FontsEnum newFont);
    public event FontChange OnFontChange;

    public delegate void ChangeAnimation(AnimationTypeEnum newAnimation);
    public event ChangeAnimation OnChangeAnimation;

    public delegate void ChangeText(string newText);
    public event ChangeText OnChangeText;

    public delegate void ChangePosition(Vector3 newPosition);
    public event ChangePosition OnChangePosition;

    public delegate void ChangeTextColor(Color newColor);
    public event ChangeTextColor OnChangeTextColor;

    public delegate void ChangeMaterial(Material newMaterial);
    public event ChangeMaterial OnChangeMaterial;

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
            return text.fontSize;
        }

        set
        {
            if(text.fontSize != value)
            {
                text.fontSize = value;

                if(OnFontSizeChange != null)
                {
                    OnFontSizeChange(text.fontSize);
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
        text = GetComponent<Text>();
        rectTransform = GetComponent<RectTransform>();

        foreach (TextIndividualLetterConfig textIndividualLetterConfig in fontsIndividualLetterConfig)
        {
            textIndividualLetterConfig.ForceInitialize();
        }

        fontSizeOld = text.fontSize;
        oldText = text.text;
        oldPosition = rectTransform.localPosition;
        oldAnimation = animationType;
        oldMaterial = text.material;

        isPlayingForward = true;
        timeLastAnimation = Time.time;

        CreateLetters ();		

		if (startAtBegining) 
		{
			Play ();
		}
    }

    void Start()
    {
        OnFontSizeChange += HandleOnFontSizeChange;
        OnFontChange += HandleOnFontChange;
        OnChangeAnimation += HandleOnChangeAnimation;
        OnChangeText += HandleOnChangeText;
        OnChangePosition += HandleChangePosition;
        OnChangeTextColor += HandleChangeTextColor;
        OnChangeMaterial += HandleOnChangeMaterial;
    }

    void OnDestroy()
    {
        OnFontSizeChange -= HandleOnFontSizeChange;
        OnFontChange -= HandleOnFontChange;
        OnChangeAnimation -= HandleOnChangeAnimation;
        OnChangeText -= HandleOnChangeText;
        OnChangePosition -= HandleChangePosition;
        OnChangeTextColor -= HandleChangeTextColor;
        OnChangeMaterial -= HandleOnChangeMaterial;
    }

    private void HandleChangeTextColor(Color newColor)
    {
        foreach(Letter letter in lettersText)
        {
            letter.text.color = newColor;
        }
    }

    private void HandleChangePosition(Vector3 newPosition)
    {
        UpdateLetters();
    }

    private void HandleOnChangeAnimation(AnimationTypeEnum newAnimation)
    {
        Stop();
        //Debug.Log("HandleOnChangeAnimation");
        UpdateLetters();
        Play();
    }

    private void HandleOnFontChange(FontsEnum newFont)
    {
        CreateLetters();
    }

    private void HandleOnFontSizeChange(int newValue)
    {
          UpdateLetters();
            /*
          for(int i = 0; i < lettersText.Count; i++)
          {
            //lettersText[i].rectTransform.localPosition = fontsIndividualLetterConfig[(int)font].text.rectTransform.localPosition + (i * GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize,fontSize) * fontsIndividualLetterConfig[(int)font].distanceBetweenLetters);
            lettersText[i].rectTransform.localPosition = position + (i * GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize,fontSize) * fontsIndividualLetterConfig[(int)font].distanceBetweenLetters);


            //  ATTENTION: The sizeDelta has to be set on the font letter prefab with the exact amount it's setted in the WordAnimator script. Otherwise, it will be errors.
            lettersText[i].rectTransform.sizeDelta = new Vector2(GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize, fontSize) * fontsIndividualLetterConfig[(int)font].sizeRectTransformForThisFontSize.x, GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize, fontSize) * fontsIndividualLetterConfig[(int)font].sizeRectTransformForThisFontSize.y);
            lettersText[i].fontSize = newValue;
          }
          */

        //fontsIndividualLetterConfig[(int)font].baseFontSize;
        //fontsIndividualLetterConfig[(int)font].distanceBetweenLetters
        //fontsIndividualLetterConfig[¨(int)font].sizeRectTransformForThisFontSize

        //letter.rectTransform.localPosition =    
    }

    public void EraseLetters()
    {
        if (lettersText != null)
        {
            foreach (Letter letter in lettersText)
            {
                Destroy(letter.gameObject);
            }

            lettersText.Clear();
        }


        if (canvas != null)
        {
            Destroy(canvas.gameObject);
            canvas = null;
        }
    }

    private void UpdateLetters()
    {
        if(lettersText != null)
        {
            if(canvas != null)
            {
                Vector3 adddeltaPosition = Vector3.zero;

                int newLineCounter = 0;
                int current = 0;
                int currentLetter = 0;

                while (current < text.text.Length)
                {
                    if (text.text[current] != '/')
                    {
                        string lineBreaks = string.Empty;

                        for (int i = 0; i < newLineCounter; i++)
                        {
                            lineBreaks += "\n";
                        }

                        lettersText[currentLetter].text.text = lineBreaks + text.text[current].ToString();
                        lettersText[currentLetter].rectTransform.position = rectTransform.localPosition;
                        lettersText[currentLetter].rectTransform.localPosition = lettersText[currentLetter].rectTransform.position + adddeltaPosition;
                        lettersText[currentLetter].RealPosition = lettersText[currentLetter].rectTransform.localPosition;
                        adddeltaPosition += GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize, text.fontSize) * fontsIndividualLetterConfig[(int)font].distanceBetweenLetters;
                        lettersText[currentLetter].rectTransform.sizeDelta = new Vector2(GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize, text.fontSize) * fontsIndividualLetterConfig[(int)font].sizeRectTransformForThisFontSize.x, GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize, text.fontSize) * fontsIndividualLetterConfig[(int)font].sizeRectTransformForThisFontSize.y);
                        //lettersText[currentLetter].rectTransform.sizeDelta = new Vector2(GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize, fontSize) * fontsIndividualLetterConfig[(int)font].sizeRectTransformForThisFontSize.x, GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize, fontSize) * fontsIndividualLetterConfig[(int)font].sizeRectTransformForThisFontSize.y);

                        //Debug.Log("UpdateLetters " + lettersText[currentLetter].text.text.ToString());
                        lettersText[currentLetter].text.fontSize = text.fontSize;

                        currentLetter++;
                    }
                    else
                    {
                            int next = current + 1;

                            if (next < text.text.Length)
                            {
                                if (text.text[next] == 'n')
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
        }
    }

    public void CreateLetters()
	{
        EraseLetters();
		
		lettersText = new List<Letter> ();
		lettersText.Clear ();
		canvas = (Canvas)GameObject.Instantiate (canvasPrefab, Vector3.zero, Quaternion.identity).GetComponent<Canvas>();
		
        canvas.name = fontsIndividualLetterConfig[(int)font].fontName;
		
		Vector3 adddeltaPosition = Vector3.zero;

        int newLineCounter = 0;
        int current = 0;


        while(current < text.text.Length)
        {
            if(text.text[current] != '/')
            {
                string lineBreaks = string.Empty;

                for(int i = 0; i < newLineCounter; i++)
                {
                    lineBreaks += "\n";
                }

                Letter letterText = (Letter)GameObject.Instantiate(letterPrefab,Vector3.zero,Quaternion.identity).GetComponent<Letter>();
			    letterText.transform.name = "Letter";
			    letterText.transform.SetParent(canvas.gameObject.transform);

                letterText.text.font = fontsIndividualLetterConfig[(int)font].text.font;
                letterText.text.fontStyle = text.fontStyle;
			    letterText.text.fontSize = text.fontSize;
			    letterText.rectTransform.sizeDelta = new Vector2(GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize,text.fontSize) * fontsIndividualLetterConfig[(int)font].sizeRectTransformForThisFontSize.x,GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize,text.fontSize) * fontsIndividualLetterConfig[(int)font].sizeRectTransformForThisFontSize.y);

			    letterText.text.lineSpacing = text.lineSpacing;
			    letterText.text.alignment = text.alignment;
			    letterText.text.alignByGeometry = text.alignByGeometry;
			    letterText.text.horizontalOverflow = text.horizontalOverflow;
			    letterText.text.verticalOverflow = text.verticalOverflow;
			    letterText.text.color = text.color;
			    letterText.text.material = text.material;
			    letterText.text.raycastTarget = text.raycastTarget;
			    letterText.text.text = lineBreaks + text.text[current].ToString();
			    //Debug.Log(letter.ToString());
			    letterText.rectTransform.position = rectTransform.localPosition;
			    //Debug.Log(text.rectTransform.position);
			    letterText.rectTransform.rotation = rectTransform.rotation;
			    letterText.rectTransform.localScale = rectTransform.localScale;
			    letterText.rectTransform.localPosition = letterText.rectTransform.position + adddeltaPosition;
                letterText.RealPosition = letterText.rectTransform.localPosition;
                adddeltaPosition += GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize,text.fontSize) * fontsIndividualLetterConfig[(int)font].distanceBetweenLetters;

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

                if(next < text.text.Length)
                {
                    if (text.text[next] == 'n')
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
		foreach (Letter letter in lettersText) 
		{
			letter.rectTransform.sizeDelta = new Vector2(512f,512f);	
		}
	}


	public void Play()
	{
		isPlaying = true;
		lerp = 0f;

        SetLettersInvisible();

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
            case AnimationTypeEnum.ANIMATION5:
                ClearSizeRectTransform();
                oldTimeShake = Time.time;
                break;
            case AnimationTypeEnum.ANIMATION6:
                ClearSizeRectTransform();
                break;
            case AnimationTypeEnum.ANIMATION7:
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
        if(setInvisibleWhenStops)
        {
            SetLettersInvisible();
        }
        else
        {
            lerp = 1f;
            PlayFrame();
        }

        isPlaying = false;
        lerp = 0f;
        isPlayingForward = true;
    }

    private void UpdateLettersEveryFrame()
    {
        foreach(Letter letter in lettersText)
        {
            letter.text.fontStyle = text.fontStyle;
            letter.text.lineSpacing = text.lineSpacing;
            letter.text.supportRichText = text.supportRichText;
            letter.text.alignment = text.alignment;
            letter.text.alignByGeometry = text.alignByGeometry;
            letter.text.horizontalOverflow = text.horizontalOverflow;
            letter.text.resizeTextForBestFit = text.resizeTextForBestFit;
            letter.text.raycastTarget = text.raycastTarget;

            letter.rectTransform.anchorMin = rectTransform.anchorMin;
            letter.rectTransform.anchorMax = rectTransform.anchorMax;
            letter.rectTransform.pivot = rectTransform.pivot;
            letter.rectTransform.localScale = rectTransform.localScale;
        }
    }

    private void HandleOnChangeMaterial(Material newMaterial)
    {
        foreach(Letter letter in lettersText)
        {
            letter.text.material = newMaterial;
        }
    }

	void Update()
	{
        if(fontSizeOld != text.fontSize)
        {
            fontSizeOld = text.fontSize;

            if (OnFontSizeChange != null)
            {
                OnFontSizeChange(text.fontSize);
            }
        }

        if(oldText != text.text)
        {
            oldText = text.text;

            if(OnChangeText != null)
            {
                OnChangeText(text.text);
            }
        }

        if(oldPosition != rectTransform.localPosition)
        {
            oldPosition = rectTransform.localPosition;

            if(OnChangePosition != null)
            {
                OnChangePosition(rectTransform.localPosition);
            }
        }

        if(oldAnimation != animationType)
        {
            oldAnimation = animationType;

            if(OnChangeAnimation != null)
            {
                OnChangeAnimation(animationType);
            }
        }

        if(oldColor != text.color)
        {
            oldColor = text.color;

            if(OnChangeTextColor != null)
            {
                OnChangeTextColor(text.color);
            }
        }

        if(oldMaterial != text.material)
        {
            oldMaterial = text.material;

            if(OnChangeMaterial != null)
            {
                OnChangeMaterial(text.material);
            }
        }

        UpdateLettersEveryFrame();


		if (isPlaying) 
		{
            if(Time.time >= timeLastAnimation + intervalBetweenAnimations)
            { 
			    lerp += Time.deltaTime * speed;
            
			    if(lerp < 1f)
			    {
				    PlayFrame();
			    }
			    else
			    {
                    isPlayingForward = !isPlayingForward;
                    timeLastAnimation = Time.time;
                    
				    if(loop)
				    {
					    Play();
				    }
				    else
				    {
					    Stop();
                        //SetLettersInvisible();
				    }
			    }
            }
        }
	}

    private void HandleOnChangeText(string newText)
    {
        Stop();
        CreateLetters();
        Play();
    }

    private void SetLettersInvisible()
    {
        foreach (Letter letterText in lettersText)
        {
            letterText.text.color = new Color(letterText.text.color.r,letterText.text.color.g,letterText.text.color.b,0f);
        }
    }

    private void SetLettersVisible()
    {
        foreach (Letter letterText in lettersText)
        {
            letterText.text.color = new Color(letterText.text.color.r, letterText.text.color.g, letterText.text.color.b, 1f);
        }
    }

	private void PlayFrame()
	{
        SetLettersVisible();
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
            case AnimationTypeEnum.ANIMATION5:
                Animation5();
                break;
            case AnimationTypeEnum.ANIMATION6:
                Animation6();
                break;
            case AnimationTypeEnum.ANIMATION7:
                Animation7();
                break;
            case AnimationTypeEnum.NONE:
				break;
		}
	}

	private void Animation1()
	{
		foreach (Letter letter in lettersText) 
		{
            //t.fontSize = ((int)Mathf.Lerp((float)fontsIndividualLetterConfig[(int)font].text.fontSize,(float)fontsIndividualLetterConfig[(int)font].text.fontSize * (1f + fontSizeNormalizedPercentDiff),lerp));
            letter.text.fontSize = ((int)Mathf.Lerp((float)text.fontSize,(float)text.fontSize * (1f + fontSizeNormalizedPercentDiff),lerp));
            letter.text.color = new Color(letter.text.color.r, letter.text.color.g,letter.text.color.b,Mathf.Lerp(1f,0f,lerp));
			// Debug.Log("Lerp " + lerp.ToString());
		}

		//Debug.Log ((float)text.fontSize * (1f + fontSizeNormalizedPercentDiff));
	}

	private void Animation2()
	{
		foreach (Letter letter in lettersText) 
		{
            letter.text.fontSize = ((int)Mathf.Lerp((float)text.fontSize, (float)text.fontSize * (1f - fontSizeNormalizedPercentDiff),lerp));
            letter.text.color = new Color(letter.text.color.r, letter.text.color.g,letter.text.color.b,Mathf.Lerp(1f,0f,lerp));
			// Debug.Log("Lerp " + lerp.ToString());
		}

	}

	private void Animation3()
	{
		foreach (Letter letter in lettersText)
		{
            letter.text.fontSize = ((int)Mathf.Lerp((float)text.fontSize,(float)text.fontSize * (1f + fontSizeNormalizedPercentDiff),lerp));
            letter.text.color = new Color(letter.text.color.r, letter.text.color.g, letter.text.color.b,Mathf.Lerp(0f,1f,lerp));
		}
	}

	private void Animation4()
	{
		foreach (Letter letter in lettersText)
		{
            letter.text.fontSize = ((int)Mathf.Lerp((float)text.fontSize,(float)text.fontSize * (1f - fontSizeNormalizedPercentDiff),lerp));
            letter.text.color = new Color(letter.text.color.r, letter.text.color.g, letter.text.color.b,Mathf.Lerp(0f,1f,lerp));
		}
	}

    private void Animation5()
    {
        // Shake

        // Shake it up down, left and right
        // Shake with random movements from center position
        // Uses min1 and max1

            if(Time.time >= oldTimeShake + interval1)
            {
                oldTimeShake = Time.time;
                foreach (Letter letter in lettersText)
                {
                    letter.rectTransform.localPosition = letter.RealPosition + new Vector3(GetValueByFontSize(fontsIndividualLetterConfig[(int)font].baseFontSize,text.fontSize, Random.Range(min1, max1)), GetValueByFontSize(fontsIndividualLetterConfig[(int)font].baseFontSize, text.fontSize, Random.Range(min1, max1)), 0f);
                    letter.text.color = new Color(letter.text.color.r, letter.text.color.g, letter.text.color.b, text.color.a);
                }
            }

    }
    
    private void Animation6()
    {
        // Heart Beat Animation

        // INPUT: StartSize, EndSize
        
        foreach  (Letter letter in lettersText)
        {
            if(isPlayingForward)
            {
                letter.text.fontSize = ((int)Mathf.Lerp((float)text.fontSize, (float)text.fontSize * (1f + fontSizeNormalizedPercentDiff), lerp));
                letter.text.color = new Color(letter.text.color.r, letter.text.color.g, letter.text.color.b, Mathf.Lerp(1f, 0f, lerp));
            }
            else
            {
                letter.text.fontSize = ((int)Mathf.Lerp((float)text.fontSize, (float)text.fontSize * (1f + fontSizeNormalizedPercentDiff), 1f - lerp));
                letter.text.color = new Color(letter.text.color.r, letter.text.color.g, letter.text.color.b, Mathf.Lerp(1f, 0f, lerp));
            }
        }
    }

    private void Animation7()
    {
        // Heart Beat Without Alpha Animation

        // INPUT: StartSize, EndSize

        foreach (Letter letter in lettersText)
        {
            if (isPlayingForward)
            {
                letter.text.fontSize = ((int)Mathf.Lerp((float)text.fontSize, (float)text.fontSize * (1f + fontSizeNormalizedPercentDiff), lerp));
                letter.text.color = text.color;
            }
            else
            {
                letter.text.fontSize = ((int)Mathf.Lerp((float)text.fontSize, (float)text.fontSize * (1f + fontSizeNormalizedPercentDiff), 1f - lerp));
                letter.text.color = text.color;
            }
        }
    }

    private float GetValueByPercentage(float value,float normalizedPercent)
    {
        return value * normalizedPercent;
    }

    private float GetValueByFontSize(float baseFontSize,float fontSize,float value)
    {
        return (GetValueByPercentage(value,GetNormalizedPercentage(baseFontSize, fontSize) * value));
    }
}
