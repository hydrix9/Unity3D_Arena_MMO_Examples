using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAlpha : MonoBehaviour
{
    [SerializeField] private float fadePerSecond = 2.5f;

    public Material mat;

    public bool fadeInOnAwake = true;

    public bool fadeEmission = true;

    private void Awake()
    {
        if (mat == null)
            mat = GetComponent<Renderer>().material; //try get automatically

        target = mat.GetColor("_Color"); //set initially, then fade to for fade in
        targetE = mat.GetColor("_EmissionColor");

        if (fadeInOnAwake)
            FadeIn();
    }

    public bool debugFadeOut = false;
    public bool debugFadeIn = false;
    private void Update()
    {
        if(debugFadeIn)
        {
            debugFadeIn = false;
            FadeIn();
        }

        if(debugFadeOut)
        {
            debugFadeOut = false;
            FadeOut(false);
        }

    }

    public void FadeIn()
    {
        StartCoroutine(FadeInIE());
        if (fadeEmission)
            StartCoroutine(FadeInEmissionIE());
    }

    public void FadeOut(bool thenDestroy)
    {

            StartCoroutine(FadeOutIE(thenDestroy));
            if (fadeEmission)
                StartCoroutine(FadeOutEmissionIE(thenDestroy));
        
    }

    Color target;
    IEnumerator FadeInIE()
    {

        mat.SetColor("_Color", new Color(target.r, target.g, target.b, 0)); //set a to 0

        while (mat.GetColor("_Color").a < target.a) //while less than initial set
        {
            mat.SetColor("_Color", new Color(target.r, target.g, target.b, mat.GetColor("_Color").a + (fadePerSecond * Time.deltaTime)));
            yield return new WaitForEndOfFrame();
        }

    }



    IEnumerator FadeOutIE(bool thenDestroy)
    {
        while (mat.GetColor("_Color").a > 0)
        {
            mat.SetColor("_Color", new Color(target.r, target.g, target.b, mat.GetColor("_Color").a - (fadePerSecond * Time.deltaTime)));
            yield return new WaitForEndOfFrame();
        }


        if (thenDestroy && !fadeEmission) //if we need to destroy and emission won't do it anyway... would rather let emission do it
            Destroy(gameObject);
    }

    Color targetE;
    //so far only supports white emission...
    IEnumerator FadeInEmissionIE()
    {

        mat.SetColor("_EmissionColor", new Color(0, 0, 0, 0)); //set a to 0

        currentEColor = mat.GetColor("_EmissionColor");

        while (currentEColor.r < targetE.r)
        {
            mat.SetColor("_EmissionColor", new Color(currentEColor.r + (fadePerSecond * Time.deltaTime), currentEColor.g + (fadePerSecond * Time.deltaTime), currentEColor.b + (fadePerSecond * Time.deltaTime), currentEColor.a + (fadePerSecond * Time.deltaTime)));

            yield return new WaitForEndOfFrame();
            currentEColor = mat.GetColor("_EmissionColor");
        }
    }

    public float emEndpoint = 0.1f;
    Color currentEColor;
    IEnumerator FadeOutEmissionIE(bool thenDestroy)
    {
        currentEColor = mat.GetColor("_EmissionColor");

        while (currentEColor.r > emEndpoint)
        {
            mat.SetColor("_EmissionColor", new Color(currentEColor.r - (fadePerSecond * Time.deltaTime), currentEColor.g - (fadePerSecond * Time.deltaTime), currentEColor.b - (fadePerSecond * Time.deltaTime), currentEColor.a - (fadePerSecond * Time.deltaTime)));

            yield return new WaitForEndOfFrame();
            currentEColor = mat.GetColor("_EmissionColor");
        }

        if (thenDestroy) //if we need to destroy and emission is done..
            Destroy(gameObject);
    }



} //end class
