using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionSphere : MonoBehaviour {
    // Start is called before the first frame update
    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            transform.parent.GetComponent<EnemyMovement>().playerIsNear(other.gameObject);
        }
    }
}