using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogSpeakingController : MonoBehaviour
{

    Animator anim;
    AudioSource speech;

    public AnimationCurve mouthcurve;

    public Transform lowerJaw;
    public float openValue, closedValue;

    public float updateStep = 0.1f;
    public int sampleDataLength = 1024;

    private float currentUpdateTime = 0f;

    private float clipLoudness;
    private float[] clipSampleData;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        speech = GetComponent<AudioSource>();
        clipSampleData = new float[sampleDataLength];
        //StartCoroutine(MoveMouthWithLoudness());
    }

    // Update is called once per frame
    void Update()
    {


        currentUpdateTime = speech.time;
        if (currentUpdateTime >= updateStep)
        {
            currentUpdateTime = 0f;
            speech.clip.GetData(clipSampleData, speech.timeSamples); //I read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
            clipLoudness = 0f;
            foreach (var sample in clipSampleData)
            {
                clipLoudness += Mathf.Abs(sample);
            }
            clipLoudness /= sampleDataLength; //clipLoudness is what you are looking for
        }
    }
    float minValue = 100;
    float maxValue = -10000;
    IEnumerator MoveMouthWithLoudness()
    {
        while(true)
        {
 
            //Debug.Log("MIN: " + minValue + ", MAX: " + maxValue);

            //lowerJaw.position = new Vector3(lowerJaw.position.x, jawPos, lowerJaw.position.z);
            if (clipLoudness != 0) anim.Play("DogSpeak", 0);
            if (clipLoudness == 0) anim.Play("MouthClosed", 0);

            anim.SetFloat("Mouth", clipLoudness/0.25f);

            //mouthcurve.Evaluate(clipLoudness)
            yield return null;
        }
    }
}

 
/*
 public class AudioSourceLoudnessTester : MonoBehaviour
{

    public AudioSource audioSource;
    public float updateStep = 0.1f;
    public int sampleDataLength = 1024;

    private float currentUpdateTime = 0f;

    private float clipLoudness;
    private float[] clipSampleData;

    // Use this for initialization
    void Awake()
    {

        if (!audioSource)
        {
            Debug.LogError(GetType() + ".Awake: there was no audioSource set.");
        }
        clipSampleData = new float[sampleDataLength];

    }

    // Update is called once per frame
    void Update()
    {

        currentUpdateTime += Time.deltaTime;
        if (currentUpdateTime >= updateStep)
        {
            currentUpdateTime = 0f;
            audioSource.clip.GetData(clipSampleData, audioSource.timeSamples); //I read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
            clipLoudness = 0f;
            foreach (var sample in clipSampleData)
            {
                clipLoudness += Mathf.Abs(sample);
            }
            clipLoudness /= sampleDataLength; //clipLoudness is what you are looking for
        }

    }

}
*/