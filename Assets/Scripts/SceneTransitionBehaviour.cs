using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionBehaviour : MonoBehaviour
{
    public GameObject transitionSphere;
    public BoxCollider carCollider;
    public LeanTweenType sphereFadeEase = LeanTweenType.easeInOutSine;
    public Material dissolveMaterial;

    public float minCutoff, maxCutoff, animDuration;
    bool sphereActive = true, transitioning = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (!carCollider) carCollider = GetComponent<BoxCollider>();
        if (transitionSphere && !dissolveMaterial) dissolveMaterial = transitionSphere.GetComponent<Renderer>().material;
        TransitionOut();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //transition condition
        if (Input.GetMouseButtonDown(0) && !transitioning)
        {
            if (sphereActive) TransitionOut();
            else TransitionIn();
        }
    }

    public void TransitionIn()
    {
        transitioning = true;
        LeanTween.value(dissolveMaterial.GetFloat("_CutoffHeight"), maxCutoff, animDuration).setOnUpdate(UpdateMaterial).setOnComplete(ChangeScene);
    }

    public void TransitionOut()
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
