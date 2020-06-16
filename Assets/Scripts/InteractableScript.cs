using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableScript : MonoBehaviour {
    public int interactableType;
    [Range(0, 9)]
    public int buttonValue;
    public GameObject smoke;
    public GameObject particlesSystem;
    private smokeScript smokeScript;

    private void Start() {
        if (interactableType == 0) {
            smokeScript = smoke.GetComponent<smokeScript>();

            particlesSystem.GetComponent<ParticleSystem>().Stop();
        } else {
            smoke = null;
            smokeScript = null;
        }
    }

    public void interacted() {
        switch (interactableType) {
            case 0:
                activateSmoke();
                break;

            case 1:
                clickButton();
                break;

        }
    }

    private void activateSmoke() {
        smoke.SetActive(true);
        smokeScript.currentTimer = 0f;
        particlesSystem.GetComponent<ParticleSystem>().Play();
    }

    private void clickButton() {
        transform.parent.GetComponent<PadScript>().buttonClicked(buttonValue);
    }
}