using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LetterAnimations : MonoBehaviour
{
    public LetterAnimationTypeEnum _animation;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public bool loop;
    public bool isPlaying;
    public float speed;
    public float speed2;
    public float rotationSpeed;
    public float fontSizeNormalizedPercentDiff;
    public GameObject particleSystemPrefab;
    public GameObject particleSystem2Prefab;
    public float fontSize;
    public float interval1;

    private Letter letter;
    private float lerp;
    private float lerpRotation;
    private AnimationPhaseTypeEnum phase;
    private MoveCurve trail;
    private float time1;
    private ParticleSystem particleSystem1;
    private bool soundsOn;
    private List<AudioClip> fireworkWhistles;
    public List<AudioClip> fireworkShots;
    private AudioSource audioSource;

    private void Awake()
    {
        letter = GetComponent<Letter>();
        audioSource = GetComponent<AudioSource>();
    }

    public bool Play(LetterAnimationTypeEnum __animation,Vector3 _startPosition,Vector3 _endPosition, bool _loop, float _speed, float _rotationSpeed,float _fontSizeNormalizedPercentDiff)
    {
        if(! isPlaying)
        { 
            _animation = __animation;
            startPosition = _startPosition;
            endPosition = _endPosition;
            loop = _loop;
            speed = _speed;
            rotationSpeed = _rotationSpeed;
            fontSizeNormalizedPercentDiff = _fontSizeNormalizedPercentDiff;

            Play();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool Play(LetterAnimationTypeEnum __animation, Vector3 _startPosition, Vector3 _endPosition, bool _loop, float _speed, float _rotationSpeed, float _fontSizeNormalizedPercentDiff,GameObject _particleSystemPrefab, int _fontSize, float _speed2,float _interval1,GameObject _particleSystem2Prefab, bool _soundsOn,List<AudioClip> _fireworkWhistles, List<AudioClip> _fireworkShots)
    {
        if(!isPlaying)
        { 
            _animation = __animation;
            startPosition = _startPosition;
            endPosition = _endPosition;
            loop = _loop;
            speed = _speed;
            rotationSpeed = _rotationSpeed;
            fontSizeNormalizedPercentDiff = _fontSizeNormalizedPercentDiff;
            particleSystemPrefab = _particleSystemPrefab;
            fontSize = _fontSize;
            speed2 = _speed2;
            interval1 = _interval1;
            particleSystem2Prefab = _particleSystem2Prefab;
            fireworkWhistles = _fireworkWhistles;
            fireworkShots = _fireworkShots;
            soundsOn = _soundsOn;

            Play();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool Play()
    {
        if (!isPlaying)
        {
            lerp = 0f;
            lerpRotation = 0f;
            isPlaying = true;
            time1 = 0f;
            phase = AnimationPhaseTypeEnum.PHASE1;

            if(particleSystem1 != null)
            {
                Destroy(particleSystem1.gameObject);
            }

            switch (_animation)
            {
                case LetterAnimationTypeEnum.FireWorks:
                    StartFireWorksAnimation();
                    break;
                case LetterAnimationTypeEnum.FireWorks2:
                    StartFireWorks2Animation();
                    break;
                case LetterAnimationTypeEnum.NONE:
                    break;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    private void Stop()
    {
        if (trail != null)
        {
            Destroy(trail.gameObject);
            trail.OnAnimationStops -= HandleOnTrailAnimationStops;
            trail = null;
        }

        isPlaying = false;
    }


    public void StartFireWorksAnimation()
    {
        //letter.text.color = new Color(letter.text.color.r, letter.text.color.g, letter.text.color.b, 1f);
    }

    public void StartFireWorks2Animation()
    {
        if (trail != null)
        {
            Destroy(trail.gameObject);
            trail = null;
            trail.OnAnimationStops -= HandleOnTrailAnimationStops;
        }

        trail = GameObject.Instantiate(particleSystemPrefab, startPosition, Quaternion.identity).GetComponent<MoveCurve>();
        trail.OnAnimationStops += HandleOnTrailAnimationStops;

        letter.text.color = new Color(letter.text.color.r, letter.text.color.g, letter.text.color.b, 0f);
    }

    private void HandleOnTrailAnimationStops()
    {
        phase = AnimationPhaseTypeEnum.PHASE2;
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
            case LetterAnimationTypeEnum.FireWorks2:
                PlayFrameFireWors2Animation();
                break;
            case LetterAnimationTypeEnum.NONE:

                break;
        }
    }

    private void PlayFrameFireWors2Animation()
    {
        switch(phase)
        {
            case AnimationPhaseTypeEnum.PHASE1:
                if( ! trail.IsPlaying )
                {
                    trail.Play(speed, startPosition, endPosition, true);
                    PlayRandomSound(fireworkWhistles);
                }

                break;
            case AnimationPhaseTypeEnum.PHASE2:

                lerp += Time.deltaTime * speed2;
                time1 += Time.deltaTime;

                if(time1 < interval1)
                {
                    if(lerp < 1f)
                    { 
                        letter.text.fontSize = ((int)Mathf.Lerp((float)fontSize, (float)fontSize * (1f + fontSizeNormalizedPercentDiff), lerp));
                        letter.text.color = new Color(letter.text.color.r, letter.text.color.g, letter.text.color.b, Mathf.Lerp(1f, 0f, lerp));
                    }
                    else
                    {
                        letter.text.fontSize = ((int)Mathf.Lerp((float)fontSize, (float)fontSize * (1f + fontSizeNormalizedPercentDiff), 1f));
                        letter.text.color = new Color(letter.text.color.r, letter.text.color.g, letter.text.color.b, Mathf.Lerp(1f, 0f, 1f));
                        lerp = 0f;
                    }
                }
                else
                {
                    letter.text.color = new Color(letter.text.color.r, letter.text.color.g, letter.text.color.b, Mathf.Lerp(1f, 0f, 1f));
                    phase = AnimationPhaseTypeEnum.PHASE3;
                    time1 = 0f;
                }

                break;
            case AnimationPhaseTypeEnum.PHASE3:

                //Debug.Log("PHASE 3");

                if(particleSystem1 == null)
                {
                    //Debug.Log("ParticleSystem1");
                    particleSystem1 = Instantiate(particleSystem2Prefab, endPosition, Quaternion.identity).GetComponent<ParticleSystem>();
                    //particleSystem1.Play();

                    ParticleSystem.MainModule main = particleSystem1.main;
                    Color randomColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
                    main.startColor = new ParticleSystem.MinMaxGradient(randomColor);

                    PlayRandomSound(fireworkShots);
                }
                else
                {
                    time1 += Time.deltaTime;

                    //Debug.Log(particleSystem1.isPlaying);
                    
                    if(time1 >= 1f)
                    { 
                        if ( ! particleSystem1.isPlaying)
                        {
                            //Debug.Log("Destroy particleSystem1");
                            Destroy(particleSystem1.gameObject);
                            Stop();
                        }
                    }
                }

                break;
        }
    }

    private void PlayFrameFireWorksAnimation()
    {
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
            letter.rectTransform.rotation = Quaternion.Lerp(Quaternion.Euler(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 359f), lerpRotation);
            letter.text.fontSize = (int)Mathf.Lerp(letter.realFontSize * (1f - fontSizeNormalizedPercentDiff), letter.realFontSize,lerp);
            letter.rectTransform.eulerAngles = Vector3.Lerp(new Vector3(0f,0f,0f),new Vector3(0f,0f,359),lerp);
        }
        else
        {
            phase = AnimationPhaseTypeEnum.PHASE2;
            isPlaying = false;
        }
    }

    private void PlaySound(AudioClip audioClip)
    {
        if(soundsOn)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }

    private void PlayRandomSound(List<AudioClip> audioClips)
    {
        PlaySound(audioClips[Random.Range(0, audioClips.Count - 1)]);
    }
}
