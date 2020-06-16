using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smokeScript : MonoBehaviour {

    private List<GameObject> enemys;
    public float smokeTimer = 5f;
    [HideInInspector]
    public float currentTimer;

    private void Awake() {
        enemys = new List<GameObject>();
        currentTimer = 0;
    }

    private void Update() {
        currentTimer += Time.deltaTime;
        if (currentTimer >= smokeTimer) {
            foreach (GameObject enemy in enemys) {
                enemy.GetComponent<EnemyMovement>().exitedSmoke();
            }
            enemys.Clear();
            transform.parent.GetComponent<InteractableScript>().particlesSystem.GetComponent<ParticleSystem>().Stop();
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy")) {
            other.GetComponent<EnemyMovement>().enteredSmoke();
            enemys.Add(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Enemy")) {
            other.GetComponent<EnemyMovement>().stillInSmoke();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Enemy")) {
            other.GetComponent<EnemyMovement>().exitedSmoke();
            enemys.Remove(other.gameObject);
        }
    }
}