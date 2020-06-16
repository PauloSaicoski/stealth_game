using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGroupBehavior : MonoBehaviour {
    public void HeardNoise(Vector3 noisePos, List<GameObject> enemysWhoHeard) {
        if (enemysWhoHeard.Count == 1) {
            enemysWhoHeard[0].GetComponent<EnemyMovement>().goCheckNoise(noisePos);
        } else if (enemysWhoHeard.Count > 1) {
            GameObject closest = findClosestToPos(enemysWhoHeard, noisePos);
            foreach (GameObject enemy in enemysWhoHeard) {
                enemy.GetComponent<EnemyMovement>().coverAnotherEnemy(noisePos);
            }
            closest.GetComponent<EnemyMovement>().goCheckNoise(noisePos);
        }
    }

    public void PlayerWasFound() {
        foreach (Transform child in transform) {
            if (child != transform) {
                child.GetComponent<EnemyMovement>().foundPlayer = true;
            }
        }
    }

    private GameObject findClosestToPos(List<GameObject> enemys, Vector3 position) {
        GameObject closest = enemys[0];
        float closestDist = float.MaxValue;
        NavMeshAgent nav;
        NavMeshPath path = new NavMeshPath();
        for (int i = 0; i < enemys.Count; i++) {
            nav = enemys[i].GetComponent<NavMeshAgent>();
            nav.CalculatePath(position, path);
            nav.SetPath(path);
            if (nav.remainingDistance < closestDist) {
                closest = enemys[i];
                closestDist = nav.remainingDistance;
            }
        }
        return closest;
    }
}