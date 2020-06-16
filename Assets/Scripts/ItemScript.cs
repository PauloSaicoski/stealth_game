using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour {
    //[HideInInspector]
    public bool picked, thrown;
    public Material defaultMaterial, pickedMaterial;
    [HideInInspector]
    public GameObject format;
    public GameObject noiseSphere;

    void Start() {
        picked = false;
        format = transform.GetChild(0).gameObject;
        format.tag = "Item";
        thrown = false;
    }

    // Update is called once per frame
    void Update() {
        if (picked) {
            format.GetComponent<Collider>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
            format.GetComponent<Renderer>().material = pickedMaterial;
        } else {
            format.GetComponent<Collider>().enabled = true;
            GetComponent<Rigidbody>().isKinematic = false;
            format.GetComponent<Renderer>().material = defaultMaterial;
        }
    }
    private void OnCollisionEnter(Collision other) {
        if (!other.gameObject.CompareTag("Player")) {
            if (thrown) {
                thrown = false;
                Instantiate(noiseSphere, transform.position, Quaternion.identity);
            }
        }
    }
}