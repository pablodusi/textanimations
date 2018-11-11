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
    public bool soundsOn = true;
    public bool setInvisibleWhenStops;
    public bool inverseAlpha = false;
    public bool inverseAnimation = false;
    public float speed = 1f;
    public float speed2 = 1f;
    public float rotationSpeed = 1f;
    public float interval1;
    public float interval2;
    public float intervalBetweenAnimations = 1f;
    public float min1;
    public float max1;
    public Vector3 startPosition;
    public Vector3 endPosition;
    //public Color textColor;                             //
	public float fontSizeNormalizedPercentDiff;
    //public Transform startPosition;
    public List<TextIndividualLetterConfig> fontsIndividualLetterConfig;
    public List<AudioClip> fireworkWhistles;
    public List<AudioClip> fireworkShots;
    public GameObject letterPrefab;
	public GameObject canvasPrefab;
    public GameObject canvasWorldSpacePrefab;
    public GameObject particleSystemPrefab;
    public GameObject particleSystem2Prefab;

    private Text text;
    private RectTransform rectTransform;
	private List<Letter> lettersText;
	private Canvas canvas;
	private float lerp;
    private float lerpRotation;
    private int fontSizeOld;
    private string oldText;
    private Vector3 oldPosition;
    private float oldTimeAnimation;
    private Color oldColor;
    private Material oldMaterial;
    private bool isPlayingForward;          // False is reverse
    private float timeLastAnimation;        // The time when the last animation stopped
    private float timeLastFrame;
    private AnimationTypeEnum oldAnimation;
    private string cleanText;               // Text plane without /n return codes or nothing.
    private int counter1;
    private bool isInterpolation;
    private bool animationEnd;
    private LetterAnimationTypeEnum letterAnimationType;    
    private Vector3 startRotation1;
    private Vector3 endRotation1;
    private bool isMoving;
    private bool isRotating;

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
        oldColor = text.color;
        animationEnd = false;

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

                cleanText = text.text;
                cleanText.Replace("/n", string.Empty);

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
                        lettersText[currentLetter].realFontSize = text.fontSize;
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

        cleanText = text.text;
        cleanText.Replace("/n", string.Empty);

        while (current < text.text.Length)
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
                letterText.realFontSize = text.fontSize;
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
        /*
		foreach (Letter letter in lettersText) 
		{
			letter.rectTransform.sizeDelta = new Vector2(512f,512f);	
		}*/
	}


	public void Play()
	{
		isPlaying = true;
		lerp = 0f;
        counter1 = 0;
        isInterpolation = true;
        animationEnd = false;
        timeLastFrame = Time.time;
        lerpRotation = 0f;
        isMoving = true;
        isRotating = false;

        SetLettersInvisible();
        oldTimeAnimation = Time.time;
        ChangeToScreenSpaceOverlayCanvas();

        switch (animationType) 
		{
			case AnimationTypeEnum.ANIMATION1:
				ClearSizeRectTransform();
				break;
			case AnimationTypeEnum.ANIMATION2:
				ClearSizeRectTransform();
				break;
            case AnimationTypeEnum.ANIMATION5:
                ClearSizeRectTransform();
                break;
            case AnimationTypeEnum.ANIMATION6:
                ClearSizeRectTransform();
                break;
            case AnimationTypeEnum.ANIMATION7:
                ClearSizeRectTransform();
                break;
            case AnimationTypeEnum.ANIMATION8:
                ClearSizeRectTransform();

                SetLettersInvisible();

                isInterpolation = false;

                break;

            case AnimationTypeEnum.ANIMATION9:
                ClearSizeRectTransform();

                SetLettersInvisible();

                isInterpolation = false;
                break;
            case AnimationTypeEnum.ANIMATION10:
                ClearSizeRectTransform();
                isInterpolation = false;
                SetLettersInvisible();
                letterAnimationType = LetterAnimationTypeEnum.FireWorks;
                break;
            case AnimationTypeEnum.ANIMATION11:
                ClearSizeRectTransform();
                isInterpolation = false;
                SetLettersInvisible();
                letterAnimationType = LetterAnimationTypeEnum.FireWorks2;
                ChangeToWorldSpaceCanvas();
                break;
            case AnimationTypeEnum.ANIMATION12:
                ClearSizeRectTransform();
                isInterpolation = false;
                SetLettersInvisible();
                letterAnimationType = LetterAnimationTypeEnum.FireWorks3;
                break;
            case AnimationTypeEnum.ANIMATION13:
                ClearSizeRectTransform();
                isInterpolation = false;                
                letterAnimationType = LetterAnimationTypeEnum.FireWorks4;

                for (int counter1 = 0; counter1 < lettersText.Count; counter1++)
                {
                    Vector3 endPosition2 = Vector3.zero;

                    endPosition2 = Camera.main.ScreenToWorldPoint(lettersText[counter1].rectTransform.transform.position);
                    endPosition2 = new Vector3(endPosition2.x, -endPosition2.y, 0f);

                    lettersText[counter1].letterAnimations.Play(letterAnimationType,
                                                                startPosition,
                                                                endPosition2,
                                                                loop,
                                                                speed,
                                                                rotationSpeed,
                                                                fontSizeNormalizedPercentDiff,
                                                                particleSystemPrefab,
                                                                text.fontSize,
                                                                speed2,
                                                                interval2,
                                                                particleSystem2Prefab,
                                                                soundsOn,
                                                                fireworkWhistles,
                                                                fireworkShots
                                                                );

                    //Debug.Log(lettersText[counter1].text.text.ToString());
                    SetLettersInvisible();
                }
                break;
            case AnimationTypeEnum.ANIMATION14:
                ClearSizeRectTransform();
                isInterpolation = true;
                SetLettersVisible();
                break;
            case AnimationTypeEnum.ANIMATION15:
                isInterpolation = true;
                SetLettersVisible();
                break;
            case AnimationTypeEnum.ANIMATION16:
                // ATTENTION: IT REQUIRES BIG RECT TRANSFORM SIZE

                isInterpolation = true;
                SetLettersVisible();
                startRotation1 = new Vector3(0f, 0f, 0f);
                endRotation1 = new Vector3(0f, 0f, 359f);
                isMoving = true;
                isRotating = true;
                break;
            case AnimationTypeEnum.NONE:
				break;
		}
	}

    private void ChangeToWorldSpaceCanvas()
    {
        /*
        RectTransform rectTransformCanvas = canvas.GetComponent<RectTransform>();
        RectTransform rectTransformWorldSpace = canvasWorldSpacePrefab.GetComponent<RectTransform>();
        canvas.renderMode = RenderMode.WorldSpace;
        rectTransformCanvas = rectTransformWorldSpace;
        */
    }

    private void ChangeToScreenSpaceOverlayCanvas()
    {
        /*
        RectTransform rectTransformCanvas = canvas.GetComponent<RectTransform>();
        RectTransform rectTransformScreenSpaceOverlayCanvas = canvasPrefab.GetComponent<RectTransform>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        rectTransformCanvas = rectTransformScreenSpaceOverlayCanvas;
        */
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
        lerpRotation = 0f;
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
            if(isInterpolation)
            { 
                if (Time.time >= timeLastAnimation + intervalBetweenAnimations)
                { 
			        lerp += Time.deltaTime * speed;
                    lerpRotation += Time.deltaTime * rotationSpeed;

                    if (isMoving)
                    {
                        if(lerp < 1f)
                        { 
                            PlayFrame();
                        }
                        else
                        {
                            isMoving = false;
                        }
                    }
                    
                    if (isRotating)
                    {
                        if(lerpRotation < 1f)
                        {
                            PlayFrame();
                        }
                        else
                        {
                            isRotating = false;
                        }
                    }

                    if ( ! isMoving && ! isRotating)
                    {
                        isPlayingForward = !isPlayingForward;
                        timeLastAnimation = Time.time;
                        isMoving = false;

                        if (loop)
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
            else
            {
                    PlayFrameNotInterpolation();  
                    
                    if(animationEnd)
                    {
                        //isPlayingForward = !isPlayingForward;
                        timeLastAnimation = Time.time;

                        if (loop)
                        {
                            Play();
                        }
                        else
                        {
                            Stop();
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
        timeLastFrame = Time.time;

        switch (animationType) 
		{
			case AnimationTypeEnum.ANIMATION1:
				Animation1();	
			break;
			case AnimationTypeEnum.ANIMATION2:
				Animation2();
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
            case AnimationTypeEnum.ANIMATION8:
                Animation8();
                break;
            case AnimationTypeEnum.ANIMATION14:
                Animation14();
                break;
            case AnimationTypeEnum.ANIMATION15:
                Animation15();
                break;
            case AnimationTypeEnum.ANIMATION16:
                Animation16();
                break;
            case AnimationTypeEnum.NONE:
				break;
		}
	}

    private void PlayFrameNotInterpolation()
    {
        //SetLettersVisible();

        switch (animationType)
        {
            case AnimationTypeEnum.ANIMATION8:
                Animation8();
                break;
            case AnimationTypeEnum.ANIMATION9:
                Animation9();
                break;
            case AnimationTypeEnum.ANIMATION10:
                Animation10();
                break;
            case AnimationTypeEnum.ANIMATION11:
                Animation11();
                break;
            case AnimationTypeEnum.ANIMATION12:
                Animation12();
                break;
            case AnimationTypeEnum.ANIMATION13:
                Animation13();
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
            letter.text.color = new Color(letter.text.color.r, letter.text.color.g,letter.text.color.b,Mathf.Lerp(inverseAlpha == false? 1f : 0f, inverseAlpha == false ? 0f : 1f, lerp));
			// Debug.Log("Lerp " + lerp.ToString());
		}


		//Debug.Log ((float)text.fontSize * (1f + fontSizeNormalizedPercentDiff));
	}

    private void Animation2()
	{
		foreach (Letter letter in lettersText) 
		{
            letter.text.fontSize = ((int)Mathf.Lerp((float)text.fontSize, (float)text.fontSize * (1f - fontSizeNormalizedPercentDiff),lerp));
            letter.text.color = new Color(letter.text.color.r, letter.text.color.g,letter.text.color.b,Mathf.Lerp(inverseAlpha == false?1f : 0f,inverseAlpha == false? 0f : 1f,lerp));
			// Debug.Log("Lerp " + lerp.ToString());
		}
	}

    private void Animation5()
    {
        // Shake

        // Shake it up down, left and right
        // Shake with random movements from center position
        // Uses min1 and max1

            if(Time.time >= oldTimeAnimation + interval1)
            {
                oldTimeAnimation = Time.time;
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

    private void Animation8()
    {
        if (Time.time > timeLastFrame + interval1)
        {
            if (counter1 < lettersText.Count)
            {
                lettersText[counter1].text.color = new Color(lettersText[counter1].text.color.r, lettersText[counter1].text.color.g, lettersText[counter1].text.color.b, 1f);
                counter1++;
                timeLastFrame = Time.time;
            }
            else
            {
                animationEnd = true;
            }
        }

   }

    private void Animation9()
    {
        if (Time.time > timeLastFrame + interval1)
        {
            if (counter1 < lettersText.Count)
            {
                //lettersText[counter1].text.color = new Color(lettersText[counter1].text.color.r, lettersText[counter1].text.color.g, lettersText[counter1].text.color.b, 1f);
                counter1++;
                timeLastFrame = Time.time;
            }
        }

        if (Time.time >= timeLastAnimation + intervalBetweenAnimations)
        {
            lerp += Time.deltaTime * speed;

            //Debug.Log("lerp " + lerp.ToString());
            if (lerp < 1f)
            {
                for (int i = 0; i < counter1; i++)
                {
                    lettersText[i].text.fontSize = ((int)Mathf.Lerp((float)text.fontSize, (float)text.fontSize * (1f + fontSizeNormalizedPercentDiff), lerp));
                    lettersText[i].text.color = new Color(lettersText[i].text.color.r, lettersText[i].text.color.g, lettersText[i].text.color.b, Mathf.Lerp(1f, 0f, lerp));
                }
            }
            else
            {
                timeLastAnimation = Time.time;
                lerp = 0f;

                if(counter1 >= lettersText.Count)
                {
                    animationEnd = true;    
                }

                //Debug.Log("Animation1 Finished timeLastAnimation = " + timeLastAnimation.ToString());
            }
        }
    }

    private void Animation10()
    {
        // FireWorks 1

        if (Time.time > timeLastFrame + interval1)
        {
            if (counter1 < lettersText.Count)
            {
                counter1++;
                timeLastFrame = Time.time;
            }
        }

        if(counter1 < lettersText.Count)
        {
            lettersText[counter1].letterAnimations.Play(letterAnimationType,
                                                        startPosition,
                                                        lettersText[counter1].RealPosition,
                                                        loop,
                                                        speed,
                                                        rotationSpeed,
                                                        fontSizeNormalizedPercentDiff);
        }
        else
        {
            if(!lettersText[lettersText.Count-1].letterAnimations.isPlaying)
            { 
                animationEnd = true;
                lerp = 0f;
            }
        }
    }

    private void SetLettersInvisible(int indexFrom)
    {
        for(int i = 0; i < lettersText.Count; i++)
        {
            if(i >= indexFrom)
            {
                lettersText[i].text.color = new Color(lettersText[i].text.color.r, lettersText[i].text.color.g, lettersText[i].text.color.b,0f);
            }
        }
    }

    private void Animation11()
    {
        // FireWorks 2

        SetLettersInvisible(counter1);

        if (Time.time > timeLastFrame + interval1)
        {
            if (counter1 < lettersText.Count)
            {
                counter1++;
                timeLastFrame = Time.time;
                
                /*
                if (counter1 < lettersText.Count)
                {
                    Debug.Log(lettersText[counter1].text.text.ToString());
                }*/

                    while (counter1 < lettersText.Count && (lettersText[counter1].text.text.ToString() == " " || lettersText[counter1].text.text.ToString() == "/"))
                    {
                        if(lettersText[counter1].text.text.ToString() == "/")
                        {
                            counter1++;
                        }
                        else
                        {
                            //Debug.Log("The letter is empty");
                        }
                                        
                        counter1++;
                     }
            }
        }

        if (counter1 < lettersText.Count)
        {

            Vector3 worldPosition = Vector3.zero;

            //RectTransformUtility.ScreenPointToWorldPointInRectangle(lettersText[counter1].rectTransform, Camera.main.WorldToScreenPoint(lettersText[counter1].rectTransform.localPosition),null, out worldPosition);

            //worldPosition = lettersText[counter1].rectTransform.TransformPoint(lettersText[counter1].rectTransform.localPosition);

            //Vector3[] worldCornersRectTransform = new Vector3[4];
            //lettersText[counter1].rectTransform.GetWorldCorners(worldCornersRectTransform);


            //   worldPosition = RectTransformToScreenSpace(lettersText[counter1].rectTransform);

            worldPosition = Camera.main.ScreenToWorldPoint(lettersText[counter1].rectTransform.transform.position);
            worldPosition = new Vector3(worldPosition.x, -worldPosition.y, 0f);

            lettersText[counter1].letterAnimations.Play(letterAnimationType,
                                                        startPosition,
                                                        //lettersText[counter1].rectTransform.transform.position,
                                                        //worldPosition,
                                                        worldPosition,
                                                        loop,
                                                        speed,
                                                        rotationSpeed,
                                                        fontSizeNormalizedPercentDiff,
                                                        particleSystemPrefab,
                                                        text.fontSize,
                                                        speed2,
                                                        interval1,
                                                        particleSystem2Prefab,
                                                        soundsOn,
                                                        fireworkWhistles,
                                                        fireworkShots
                                                        );

            //Debug.DrawLine(Vector3.zero, worldPosition);
            //Debug.Log(worldPosition);
            //Debug.Log(lettersText[counter1].rectTransform.localPosition);
            //Debug.Log(lettersText[counter1].rectTransform.position);

            //Debug.Log(lettersText[counter1].rectTransform.localPosition);
            //Debug.DrawLine(Vector3.zero, worldCornersRectTransform[0]);
            //Debug.Log(worldCornersRectTransform[0]);
        }
        else
        {
            if (!lettersText[lettersText.Count - 1].letterAnimations.isPlaying)
            {
                animationEnd = true;
                lerp = 0f;
            }
        }
    }

    private void Animation12()
    {
        // FireWorks 3

        SetLettersInvisible(counter1);

        if (Time.time > timeLastFrame + interval1)
        {
            if (counter1 < lettersText.Count)
            {
                counter1++;
                timeLastFrame = Time.time;

                while (counter1 < lettersText.Count && (lettersText[counter1].text.text.ToString() == " " || lettersText[counter1].text.text.ToString() == "/"))
                {
                    if (lettersText[counter1].text.text.ToString() == "/")
                    {
                        counter1++;
                    }

                    counter1++;
                }
            }
        }

        if (counter1 < lettersText.Count)
        {
            Vector3 worldPosition = Vector3.zero;

            worldPosition = Camera.main.ScreenToWorldPoint(lettersText[counter1].rectTransform.transform.position);
            worldPosition = new Vector3(worldPosition.x, -worldPosition.y, 0f);

            lettersText[counter1].letterAnimations.Play(letterAnimationType,
                                                        startPosition,
                                                        worldPosition,
                                                        loop,
                                                        speed,
                                                        rotationSpeed,
                                                        fontSizeNormalizedPercentDiff,
                                                        particleSystemPrefab,
                                                        text.fontSize,
                                                        speed2,
                                                        interval2,
                                                        particleSystem2Prefab,
                                                        soundsOn,
                                                        fireworkWhistles,
                                                        fireworkShots
                                                        );

        }
        else
        {
            if (!lettersText[lettersText.Count - 1].letterAnimations.isPlaying)
            {
                animationEnd = true;
                lerp = 0f;
            }
        }
    }

    private void Animation13()
    {
        // It shots all trails at the same time

        // FireWorks 4

        if(Time.time > oldTimeAnimation + intervalBetweenAnimations)
        {
            if( AllLettersAreStopped() )
            {
                oldTimeAnimation = Time.time;
                animationEnd = true;
                lerp = 0f;
            }
        }
    }

    private void Animation14()
    {
        // "Swinging 1": It swings all letters from top to bottom and viceversa

            Vector3 startPosition2;
            Vector3 endPosition2;
            for (int i = 0; i < lettersText.Count; i++)
            {  
                if( ! isPlayingForward)
                {
                    startPosition2 = new Vector3(lettersText[i].RealPosition.x, lettersText[i].RealPosition.y + startPosition.y, lettersText[i].RealPosition.z);
                    endPosition2 = new Vector3(lettersText[i].RealPosition.x, lettersText[i].RealPosition.y - startPosition.y, lettersText[i].RealPosition.z);

                /*
                if ( i == 0)
                {
                    Debug.Log("Real Position " + lettersText[i].RealPosition + " startPosition " + startPosition2 + " endPosition " + endPosition2);
                }*/

                    lettersText[i].rectTransform.localPosition = Vector3.Lerp(startPosition2, endPosition2, lerp);
                }
                else
                {
                    startPosition2 = new Vector3(lettersText[i].RealPosition.x, lettersText[i].RealPosition.y - startPosition.y, lettersText[i].RealPosition.z);
                    endPosition2 = new Vector3(lettersText[i].RealPosition.x, lettersText[i].RealPosition.y + startPosition.y, lettersText[i].RealPosition.z);
                    lettersText[i].rectTransform.localPosition = Vector3.Lerp(startPosition2, endPosition2, lerp);
                }
            }
    }

    private void Animation15()
    {
        // New Animation: "Swinging 2": It swings all letters from left to right and viceversa

        Vector3 startPosition2;
        Vector3 endPosition2;
        for (int i = 0; i < lettersText.Count; i++)
        {
            if (!isPlayingForward)
            {
                startPosition2 = new Vector3(lettersText[i].RealPosition.x + startPosition.x, lettersText[i].RealPosition.y, lettersText[i].RealPosition.z);
                endPosition2 = new Vector3(lettersText[i].RealPosition.x - startPosition.x, lettersText[i].RealPosition.y, lettersText[i].RealPosition.z);
                lettersText[i].rectTransform.localPosition = Vector3.Lerp(startPosition2, endPosition2, lerp);
            }
            else
            {
                startPosition2 = new Vector3(lettersText[i].RealPosition.x - startPosition.x, lettersText[i].RealPosition.y, lettersText[i].RealPosition.z);
                endPosition2 = new Vector3(lettersText[i].RealPosition.x + startPosition.x, lettersText[i].RealPosition.y, lettersText[i].RealPosition.z);
                lettersText[i].rectTransform.localPosition = Vector3.Lerp(startPosition2, endPosition2, lerp);
            }
        }
    }

    private void Animation16()
    {
        // - New Animation: "Rotation 1": It rotates from one Z point to another Z point (360 circuit)

        if (Time.time > oldTimeAnimation + intervalBetweenAnimations)
        {
            for (int i = 0; i < lettersText.Count; i++)
            {
                if( ! inverseAnimation)
                { 
                    lettersText[i].rectTransform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Lerp(endRotation1.z, startRotation1.z, lerp)));
                }
                else
                {
                    lettersText[i].rectTransform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Lerp(startRotation1.z, endRotation1.z, lerp)));
                }
            }
        }
    }

    private bool AllLettersAreStopped()
    {
        bool allLettersAreStopped = true;
        int i = 0;

        while(i < lettersText.Count && allLettersAreStopped)
        {
            allLettersAreStopped = ! lettersText[i].letterAnimations.isPlaying;
            i++;
        }

        return allLettersAreStopped;
    }

    public static Rect RectTransformToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        return new Rect((Vector2)transform.position - (size * 0.5f), size);
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
