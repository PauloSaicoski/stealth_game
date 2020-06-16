using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseSphereScript : MonoBehaviour {
    private List<GameObject> enemys;
    public float growSpeed = 20f;
    public float shrinkSpeed = 30f;
    public float maxScale = 30f;
    public float minScale = 2f;
    private bool growing;
    public Material m;
    private void Awake() {
        enemys = new List<GameObject>();
        growing = true;
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy")) {
            enemys.Add(other.gameObject);
        }
    }
    private void Update() {
        if (growing && transform.localScale.x < maxScale) {
            transform.localScale = new Vector3(transform.localScale.x + (growSpeed * Time.deltaTime), transform.localScale.y + (growSpeed * Time.deltaTime), transform.localScale.z + (growSpeed * Time.deltaTime));
        } else {
            growing = false;
            if (enemys.Count > 0) {
                enemys[0].transform.parent.GetComponent<EnemyGroupBehavior>().HeardNoise(transform.position, enemys);
            }
            transform.localScale = new Vector3(transform.localScale.x + (-shrinkSpeed * Time.deltaTime), transform.localScale.y + (-shrinkSpeed * Time.deltaTime), transform.localScale.z + (-shrinkSpeed * Time.deltaTime));
            if (transform.localScale.x <= minScale) {
                Destroy(gameObject);
            }

        }
    }

}