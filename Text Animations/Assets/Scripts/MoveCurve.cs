using UnityEngine;


public class MoveCurve : MonoBehaviour
{
    public float speed;
    public Vector3 startPoint;
    public Vector3 endPoint;
    public bool setInvisibleWhenStops;
    private bool isPlaying;
    private float lerp;
    private ParticleSystem particleSystem2;

    public delegate void AnimationStops();
    public event AnimationStops OnAnimationStops;

    public bool IsPlaying
    {
        get
        {
            return isPlaying;
        }
    }

    private void Awake()
    {
        particleSystem2 = GetComponent<ParticleSystem>();
    }
    private void Start()
    {
        isPlaying = false;
        lerp = 0f;
        particleSystem2.Stop();
    }

    public void Play(float _speed,Vector3 _startPoint, Vector3 _endPoint,bool _setInvisibleWhenStops)
    {
        particleSystem2.Play();
        speed = _speed;
        isPlaying = true;
        startPoint = _startPoint;
        endPoint = _endPoint;
        setInvisibleWhenStops = _setInvisibleWhenStops;
        ParticleSystem.MainModule main = particleSystem2.main;
        //ParticleSystem.MinMaxGradient startColor = main.startColor;
        //ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams(); 
        Color randomColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        //main.startColor.color = randomColor;
        //startColor.color = randomColor;
        //main.startColor = startColor;
        main.startColor = new ParticleSystem.MinMaxGradient(randomColor);
        //Debug.Log(Color.green);
            
            //new Color(Random.Range(1f, 255f), Random.Range(1f, 255f), Random.Range(1f, 255f), 1f);
        
        
        //Debug.Log(randomColor);
        //Debug.Log(main.startColor.color);
    }

    public void Stop()
    {
        isPlaying = false;    

        if(setInvisibleWhenStops)
        {
            // IMPORTANT: PUT CODE HERE
        }

        particleSystem2.Stop();

        if(OnAnimationStops != null)
        {
            OnAnimationStops();
        }
    }


    private void Update()
    {
        if(isPlaying)
        {
            lerp += Time.deltaTime * speed;

            if(lerp < 1f)
            {
                transform.position = GetBezierPosition(startPoint, endPoint, lerp);
            }
            else
            {
                transform.position = endPoint;
                Stop(); 
            }
        }
    }

    // parameter t ranges from 0f to 1f
    // this code might not compile!
    Vector3 GetBezierPosition(Vector3 startPosition,Vector3 endPosition, float t)
    {       
        Vector3 p0 = startPosition;
        Vector3 p1 = p0 + startPosition + Vector3.forward;
        Vector3 p3 = endPosition;
        Vector3 p2 = p3 - (endPosition + Vector3.back);
      

        // here is where the magic happens!
        return Mathf.Pow(1f - t, 3f) * p0 + 3f * Mathf.Pow(1f - t, 2f) * t * p1 + 3f * (1f - t) * Mathf.Pow(t, 2f) * p2 + Mathf.Pow(t, 3f) * p3;
    }

    /*
    TRY THIS IF OTHER FUNCTION DOESN'T FUNCTION
    RETURNS POINTS MOVING FROM ONE POINT TO ANOTHER IN CURVE WAY
    Vector3 GetBezierPosition(Transform transformBegin,Transform transformEnd, float t)
    {       
        Vector3 p0 = transformBegin.position;
        Vector3 p1 = p0 + transformBegin.forward;
        Vector3 p3 = transformEnd.position;
        Vector3 p2 = p3 - -transformEnd.forward;

        // here is where the magic happens!
        return Mathf.Pow(1f - t, 3f) * p0 + 3f * Mathf.Pow(1f - t, 2f) * t * p1 + 3f * (1f - t) * Mathf.Pow(t, 2f) * p2 + Mathf.Pow(t, 3f) * p3;
    }
     */

}
