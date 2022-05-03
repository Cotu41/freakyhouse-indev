using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void LightningEvent();

public class LightningDirector : MonoBehaviour
{
    public static LightningDirector director;

    public static event LightningEvent OnLightningStrike, OnThunder;

    

    [Tooltip("When checked, lightning will randomly strike based on cooldown values")]
    public bool storming = false;
    

    public float minLightningCooldown, maxLightningCooldown;
    
    public AudioClip[] thunderFX;
    AudioSource thunder;
    // Start is called before the first frame update
    static float lightningLevel = 0.0f;
    static readonly float STRIKESPEED = 6.0f;



    public static float LightningLevel { 
        get
        {
            return lightningLevel;
        }

    }

    Coroutine storm;

    void Start()
    {
        thunder = GetComponent<AudioSource>();
        if (director != null) Debug.LogError("MULTIPLE LIGHTNING DIRECTORS!!! BAD BAD BAD!");
        director = this;
        storm = StartCoroutine(RandomStrikes());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) StartCoroutine(Strike());
    }

    public void LightningStrike()
    {
        StartCoroutine(Strike());
    }

    public void ChangeStormState(bool storming)
    {
        if (!storming && storm != null) StopCoroutine(storm);
        if (storming && storm == null) storm = StartCoroutine(RandomStrikes());
    }


    IEnumerator RandomStrikes()
    {
        while(true)
        {
            StartCoroutine(Strike());


            yield return new WaitForSeconds(Random.Range(minLightningCooldown, maxLightningCooldown));


            
        }
        
    }

    IEnumerator Strike()
    {

        OnLightningStrike();
        while (true)
        {
            lightningLevel += Time.deltaTime * STRIKESPEED;


            lightningLevel = Mathf.Clamp(lightningLevel, 0, 1);
            if (lightningLevel == 1)
            {
                lightningLevel = 0;
                break;
            }
            yield return null;
            
        }

        yield return new WaitForSeconds(Random.Range(1, 5));
        int randIndex = Random.Range(0, thunderFX.Length);
        OnThunder?.Invoke();
        thunder.PlayOneShot(thunderFX[randIndex]);

    }
}
