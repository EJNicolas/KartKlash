using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionBehaviour : MonoBehaviour
{
    public GameObject transitionSphere;
    Material dissolveMaterial;
    public LeanTweenType sphereFadeEase = LeanTweenType.easeInOutSine;

    public float minCutoff, maxCutoff, enterDuration, exitDuration;
    bool sphereActive = true, transitioning = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (transitionSphere && !dissolveMaterial) dissolveMaterial = transitionSphere.GetComponent<Renderer>().material;
        TransitionOut();
    }

    private void OnEnable() {
        RaceManager.SwitchingToNewScene += TransitionIn;
    }

    private void OnDisable() {
        RaceManager.SwitchingToNewScene -= TransitionIn;
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
        LeanTween.value(dissolveMaterial.GetFloat("_CutoffHeight"), maxCutoff, exitDuration).setOnUpdate(UpdateMaterial).setOnComplete(ChangeScene);
    }

    public void TransitionOut()
    {
        transitioning = true;
        LeanTween.value(dissolveMaterial.GetFloat("_CutoffHeight"), minCutoff, enterDuration).setOnUpdate(UpdateMaterial).setOnComplete(TransitionComplete);
    }

    void UpdateMaterial(float cutoff)
    {
        dissolveMaterial.SetFloat("_CutoffHeight", cutoff);
    }

    void ChangeScene()
    {
        transitioning = false;
        sphereActive = true;
        RaceManager.instance.LoadNextScene();
    }

    void TransitionComplete()
    {
        transitioning = false;
        sphereActive = false;
    }
}
