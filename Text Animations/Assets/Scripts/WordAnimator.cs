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

	public FontsEnum font;
	public AnimationTypeEnum animationType;
	public int fontSize;
    public Vector3 position;
	public bool startAtBegining;
	public bool isPlaying;
    public bool loop = true;
    public bool setInvisibleWhenStops;
    public float speed = 1f;
    public float interval1;
    public float intervalBetweenAnimations = 1f;
    public float min1;
    public float max1;
    public Color textColor;
	public float fontSizeNormalizedPercentDiff;
    public List<TextIndividualLetterConfig> fontsIndividualLetterConfig;
    public GameObject letterPrefab;
	public GameObject canvasPrefab;

 
	private List<Letter> lettersText;
	private Canvas canvas;
	private float lerp;
    private int fontSizeOld;
    private string oldWord;
    private Vector3 oldPosition;
    private float oldTimeShake;
    private bool isPlayingForward;          // False is reverse
    private float timeLastAnimation;        // The time when the last animation stopped

    public delegate void FontSizeChange(int newValue);
    public event FontSizeChange OnFontSizeChange;

    public delegate void FontChange(FontsEnum newFont);
    public event FontChange OnFontChange;

    public delegate void ChangeAnimation(AnimationTypeEnum newAnimation);
    public event ChangeAnimation OnChangeAnimation;

    public delegate void ChangeWord(string newWord);
    public event ChangeWord OnChangeWord;

    public delegate void ChangePosition(Vector3 newPosition);
    public event ChangePosition OnChangePosition;

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
        oldWord = word;
        oldPosition = position;
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
        OnChangeWord += HandleOnChangeWord;
        OnChangePosition += HandleChangePosition;
    }

    void OnDestroy()
    {
        OnFontSizeChange -= HandleOnFontSizeChange;
        OnFontChange -= HandleOnFontChange;
        OnChangeAnimation -= HandleOnChangeAnimation;
        OnChangeWord -= HandleOnChangeWord;
        OnChangePosition -= HandleChangePosition;
    }

    private void HandleChangePosition(Vector3 newPosition)
    {
        UpdateLetters();
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

                while (current < word.Length)
                {
                    if (word[current] != '/')
                    {
                        string lineBreaks = string.Empty;

                        for (int i = 0; i < newLineCounter; i++)
                        {
                            lineBreaks += "\n";
                        }

                        lettersText[currentLetter].text.text = lineBreaks + word[current].ToString();
                        lettersText[currentLetter].rectTransform.position = position;
                        lettersText[currentLetter].rectTransform.localPosition = lettersText[currentLetter].rectTransform.position + adddeltaPosition;
                        lettersText[currentLetter].RealPosition = lettersText[currentLetter].rectTransform.localPosition;
                        adddeltaPosition += GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize, fontSize) * fontsIndividualLetterConfig[(int)font].distanceBetweenLetters;
                        lettersText[currentLetter].rectTransform.sizeDelta = new Vector2(GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize, fontSize) * fontsIndividualLetterConfig[(int)font].sizeRectTransformForThisFontSize.x, GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize, fontSize) * fontsIndividualLetterConfig[(int)font].sizeRectTransformForThisFontSize.y);
                        //lettersText[currentLetter].rectTransform.sizeDelta = new Vector2(GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize, fontSize) * fontsIndividualLetterConfig[(int)font].sizeRectTransformForThisFontSize.x, GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize, fontSize) * fontsIndividualLetterConfig[(int)font].sizeRectTransformForThisFontSize.y);

                        lettersText[currentLetter].text.fontSize = fontSize;

                        currentLetter++;
                    }
                    else
                    {
                            int next = current + 1;

                            if (next < word.Length)
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

                Letter letterText = (Letter)GameObject.Instantiate(letterPrefab,Vector3.zero,Quaternion.identity).GetComponent<Letter>();
			    letterText.transform.name = "Letter";
			    letterText.transform.SetParent(canvas.gameObject.transform);

                letterText.text.font = fontsIndividualLetterConfig[(int)font].text.font;
                letterText.text.fontStyle = fontsIndividualLetterConfig[(int)font].text.fontStyle;
			    letterText.text.fontSize = fontSize;
			    letterText.rectTransform.sizeDelta = new Vector2(GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize,fontSize) * fontsIndividualLetterConfig[(int)font].sizeRectTransformForThisFontSize.x,GetNormalizedPercentage(fontsIndividualLetterConfig[(int)font].baseFontSize,fontSize) * fontsIndividualLetterConfig[(int)font].sizeRectTransformForThisFontSize.y);

			    letterText.text.lineSpacing = fontsIndividualLetterConfig[(int)font].text.lineSpacing;
			    letterText.text.alignment = fontsIndividualLetterConfig[(int)font].text.alignment;
			    letterText.text.alignByGeometry = fontsIndividualLetterConfig[(int)font].text.alignByGeometry;
			    letterText.text.horizontalOverflow = fontsIndividualLetterConfig[(int)font].text.horizontalOverflow;
			    letterText.text.verticalOverflow = fontsIndividualLetterConfig[(int)font].text.verticalOverflow;
			    letterText.text.color = fontsIndividualLetterConfig[(int)font].text.color;
			    letterText.text.material = fontsIndividualLetterConfig[(int)font].text.material;
			    letterText.text.raycastTarget = fontsIndividualLetterConfig[(int)font].text.raycastTarget;
			    letterText.text.text = lineBreaks + word[current].ToString();
			    //Debug.Log(letter.ToString());
			    letterText.rectTransform.position = position;
			    //Debug.Log(text.rectTransform.position);
			    letterText.rectTransform.rotation = fontsIndividualLetterConfig[(int)font].text.rectTransform.rotation;
			    letterText.rectTransform.localScale = fontsIndividualLetterConfig[(int)font].text.rectTransform.localScale;
			    letterText.rectTransform.localPosition = letterText.rectTransform.position + adddeltaPosition;
                letterText.RealPosition = letterText.rectTransform.localPosition;
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

        if(oldWord != word)
        {
            oldWord = word;

            if(OnChangeWord != null)
            {
                OnChangeWord(word);
            }
        }

        if(oldPosition != position)
        {
            oldPosition = position;

            if(OnChangePosition != null)
            {
                OnChangePosition(position);
            }
        }

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

    private void HandleOnChangeWord(string newWord)
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
            letter.text.fontSize = ((int)Mathf.Lerp((float)fontSize,(float)fontSize * (1f + fontSizeNormalizedPercentDiff),lerp));
            letter.text.color = new Color(letter.text.color.r, letter.text.color.g,fontsIndividualLetterConfig[(int)font].text.color.b,Mathf.Lerp(1f,0f,lerp));
			// Debug.Log("Lerp " + lerp.ToString());
		}

		//Debug.Log ((float)text.fontSize * (1f + fontSizeNormalizedPercentDiff));
	}

	private void Animation2()
	{
		foreach (Letter letter in lettersText) 
		{
            letter.text.fontSize = ((int)Mathf.Lerp((float)fontSize, (float)fontSize * (1f - fontSizeNormalizedPercentDiff),lerp));
            letter.text.color = new Color(letter.text.color.r, letter.text.color.g,fontsIndividualLetterConfig[(int)font].text.color.b,Mathf.Lerp(1f,0f,lerp));
			// Debug.Log("Lerp " + lerp.ToString());
		}

	}

	private void Animation3()
	{
		foreach (Letter letter in lettersText)
		{
            letter.text.fontSize = ((int)Mathf.Lerp((float)fontSize,(float)fontSize * (1f + fontSizeNormalizedPercentDiff),lerp));
            letter.text.color = new Color(letter.text.color.r, letter.text.color.g, letter.text.color.b,Mathf.Lerp(0f,1f,lerp));
		}
	}

	private void Animation4()
	{
		foreach (Letter letter in lettersText)
		{
            letter.text.fontSize = ((int)Mathf.Lerp((float)fontSize,(float)fontSize * (1f - fontSizeNormalizedPercentDiff),lerp));
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
                    letter.rectTransform.localPosition = letter.RealPosition + new Vector3(GetValueByFontSize(fontsIndividualLetterConfig[(int)font].baseFontSize,fontSize, Random.Range(min1, max1)), GetValueByFontSize(fontsIndividualLetterConfig[(int)font].baseFontSize, fontSize, Random.Range(min1, max1)), 0f);
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
                letter.text.fontSize = ((int)Mathf.Lerp((float)fontSize, (float)fontSize * (1f + fontSizeNormalizedPercentDiff), lerp));
                letter.text.color = new Color(letter.text.color.r, letter.text.color.g, fontsIndividualLetterConfig[(int)font].text.color.b, Mathf.Lerp(1f, 0f, lerp));
            }
            else
            {
                letter.text.fontSize = ((int)Mathf.Lerp((float)fontSize, (float)fontSize * (1f + fontSizeNormalizedPercentDiff), 1f - lerp));
                letter.text.color = new Color(letter.text.color.r, letter.text.color.g, fontsIndividualLetterConfig[(int)font].text.color.b, Mathf.Lerp(1f, 0f, lerp));
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
                letter.text.fontSize = ((int)Mathf.Lerp((float)fontSize, (float)fontSize * (1f + fontSizeNormalizedPercentDiff), lerp));
                letter.text.color = textColor;
            }
            else
            {
                letter.text.fontSize = ((int)Mathf.Lerp((float)fontSize, (float)fontSize * (1f + fontSizeNormalizedPercentDiff), 1f - lerp));
                letter.text.color = textColor;
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
