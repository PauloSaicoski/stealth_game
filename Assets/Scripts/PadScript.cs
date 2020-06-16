using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadScript : MonoBehaviour {
    private TMPro.TextMeshPro displayText;
    public float cursorTimer = 0.5f;
    public float ereaseTimer = 2f;
    private float currentTimer;
    private bool cursorEnabled;
    private string typed;
    public string password;
    private Transform hinges;
    private Quaternion hingesTargetRotation;
    private Color defaultTextColor;
    // Start is called before the first frame update
    void Start() {
        displayText = transform.Find("Display").GetComponent<TMPro.TextMeshPro>();
        displayText.SetText("");
        typed = "";
        currentTimer = 0;
        hinges = transform.parent.parent;
        hingesTargetRotation = Quaternion.LookRotation(hinges.right);
        defaultTextColor = displayText.color;
    }

    // Update is called once per frame
    void Update() {
        currentTimer += Time.deltaTime;
        if (currentTimer >= cursorTimer && typed.Length < 5) {
            cursorEnabled = !cursorEnabled;
            currentTimer = 0;
        }
        if (cursorEnabled && typed.Length < 5) {
            displayText.SetText(typed + "-");
        } else {
            displayText.SetText(typed);
        }
        if (typed.Length == 5) {
            if (typed == password) {
                hinges.rotation = Quaternion.Slerp(hinges.rotation, hingesTargetRotation, Time.deltaTime);
            } else {
                displayText.color = Color.red;
                if (currentTimer >= ereaseTimer) {
                    typed = "";
                    displayText.SetText("");
                    cursorEnabled = false;
                    displayText.color = defaultTextColor;
                }
            }
        }
    }

    public void buttonClicked(int buttonValue) {
        if (typed.ToString().Length < 5) {
            typed += buttonValue;
        }
    }

}