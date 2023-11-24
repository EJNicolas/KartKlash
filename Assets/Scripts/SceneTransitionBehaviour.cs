using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionBehaviour : MonoBehaviour
{
    public GameObject transitionSphere;
    public LeanTweenType sphereFadeEase = LeanTweenType.easeInOutSine;
    public Material dissolveMaterial;

    public float minCutoff, maxCutoff, animDuration;
    bool sphereActive = false, transitioning = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (transitionSphere && !dissolveMaterial) dissolveMaterial = transitionSphere.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !transitioning)
        {
            if (sphereActive) TransitionOut();
            else TransitionIn();
        }
    }

    void TransitionIn()
    {
        transitioning = true;
        LeanTween.value(dissolveMaterial.GetFloat("_CutoffHeight"), maxCutoff, animDuration).setOnUpdate(UpdateMaterial).setOnComplete(ChangeScene);
    }

    void TransitionOut()
    {
        transitioning = true;
        LeanTween.value(dissolveMaterial.GetFloat("_CutoffHeight"), minCutoff, animDuration).setOnUpdate(UpdateMaterial).setOnComplete(TransitionComplete);
    }

    void UpdateMaterial(float cutoff)
    {
        dissolveMaterial.SetFloat("_CutoffHeight", cutoff);
    }

    void ChangeScene()
    {
        transitioning = false;
        sphereActive = true;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void TransitionComplete()
    {
        transitioning = false;
        sphereActive = false;
    }
}
