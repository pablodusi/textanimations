using UnityEngine;
using UnityEngine.UI;

public class LetterAnimations : MonoBehaviour
{
    public LetterAnimationTypeEnum _animation;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public bool loop;
    public bool isPlaying;
    public float speed;
    public float rotationSpeed;
    public float fontSizeNormalizedPercentDiff;

    private Letter letter;
    private float lerp;
    private float lerpRotation;
    private AnimationPhaseTypeEnum phase;

    private void Awake()
    {
        letter = GetComponent<Letter>();
    }

    public void Play(LetterAnimationTypeEnum __animation,Vector3 _startPosition,Vector3 _endPosition, bool _loop, float _speed, float _rotationSpeed,float _fontSizeNormalizedPercentDiff)
    {
        _animation = __animation;
        startPosition = _startPosition;
        endPosition = _endPosition;
        loop = _loop;
        speed = _speed;
        rotationSpeed = _rotationSpeed;
        fontSizeNormalizedPercentDiff = _fontSizeNormalizedPercentDiff;

        Play();
    }

    public void Play()
    {
        lerp = 0f;
        lerpRotation = 0f;
        isPlaying = true;
        phase = AnimationPhaseTypeEnum.PHASE1;

        switch (_animation)
        {
            case LetterAnimationTypeEnum.FireWorks:
                StartFireWorksAnimation();
                break;
            case LetterAnimationTypeEnum.NONE:
                break;
        }
    }
    public void StartFireWorksAnimation()
    {
        //letter.text.color = new Color(letter.text.color.r, letter.text.color.g, letter.text.color.b, 1f);
    }

    private void Update()
    {
        if(isPlaying)
        {
            PlayFrame();
        }
    }

    private void PlayFrame()
    {
        switch(_animation)
        {
            case LetterAnimationTypeEnum.FireWorks:
                PlayFrameFireWorksAnimation();
                break;
            case LetterAnimationTypeEnum.NONE:
                break;
        }
    }

    private void PlayFrameFireWorksAnimation()
    {
        switch(phase)
        {
            case AnimationPhaseTypeEnum.PHASE1:
                lerp += Time.deltaTime * speed;
                lerpRotation += Time.deltaTime * rotationSpeed;

                if(lerpRotation > 1f)
                {
                    lerpRotation = 0f;
                }

                if(lerp < 1f)
                {
                    letter.rectTransform.localPosition = Vector3.Lerp(startPosition, endPosition, lerp);
                    letter.text.color = new Color(letter.text.color.r, letter.text.color.g, letter.text.color.b, Mathf.Lerp(0f,1f,lerp));
                    //letter.rectTransform.rotation = Quaternion.Lerp(Quaternion.Euler(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 359f), lerpRotation);
                    letter.text.fontSize = (int)Mathf.Lerp(letter.realFontSize * (1f - fontSizeNormalizedPercentDiff), letter.realFontSize,lerp);
                    letter.rectTransform.eulerAngles = Vector3.Lerp(new Vector3(0f,0f,0f),new Vector3(0f,0f,359),lerp);
                }
                else
                {
                    phase = AnimationPhaseTypeEnum.PHASE2;    
                }

                break;
            case AnimationPhaseTypeEnum.PHASE2:
                isPlaying = false;          // ERASE
                break;
            case AnimationPhaseTypeEnum.PHASE3:
                break;
        }
    }
}
