using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimScript : MonoBehaviour {
    public Transform startPosition;
    public Transform endPosition;
    public GameObject playerCam;
    private bool inPreview;
    public bool thereIsPlayerCam;
    public float previewTime = 5f;
    public float toPlayerTime = 3f;
    private float currentTimer;

    public GameObject crosshairDefault, crosshairSelectable;

    void Start() {
        transform.SetPositionAndRotation(startPosition.position, startPosition.rotation);
        inPreview = true;
        currentTimer = 0;
        if (thereIsPlayerCam) {
            playerCam.SetActive(false);
            playerCam.transform.parent.GetComponent<PlayerControl>().inputEnabled = false;
            crosshairDefault.SetActive(false);
            crosshairSelectable.SetActive(false);

        }
    }

    public void moveFromTo(Transform from, Transform to, float time) {
        gameObject.SetActive(true);
        crosshairDefault.SetActive(false);
        crosshairSelectable.SetActive(false);
        playerCam.SetActive(false);
        playerCam.transform.parent.GetComponent<PlayerControl>().inputEnabled = false;
        transform.SetPositionAndRotation(from.position, from.rotation);
        startPosition = from;
        endPosition = to;
        inPreview = true;
        currentTimer = 0;
        previewTime = time;
        thereIsPlayerCam = false;
    }

    // Update is called once per frame
    void Update() {
        if (inPreview) {
            currentTimer += Time.deltaTime / previewTime;
            transform.position = Vector3.Lerp(startPosition.position, endPosition.position, currentTimer);
            transform.rotation = Quaternion.Lerp(startPosition.rotation, endPosition.rotation, currentTimer);
            if (Vector3.Distance(transform.position, endPosition.position) < 0.05) {
                inPreview = false;
                currentTimer = 0;
            }
        } else if (thereIsPlayerCam) {
            currentTimer += Time.deltaTime / toPlayerTime;
            transform.position = Vector3.Lerp(endPosition.position, playerCam.transform.position, currentTimer);
            transform.rotation = Quaternion.Lerp(endPosition.rotation, playerCam.transform.rotation, currentTimer);
            if (Vector3.Distance(transform.position, playerCam.transform.position) < 0.005) {
                currentTimer = 0;
                playerCam.SetActive(true);
                playerCam.transform.parent.GetComponent<PlayerControl>().inputEnabled = true;
                crosshairDefault.SetActive(true);
                crosshairSelectable.SetActive(false);
                gameObject.SetActive(false);
            }
        }
    }
}