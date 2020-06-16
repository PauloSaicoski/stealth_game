using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {
    public int activeBehaviour;
    public GameObject[] wayPointsBundle;
    private List<Transform> wayPoints;
    private Vector3 destination;
    [HideInInspector]
    public int wayPointIndice;
    public float wayPointDistance;
    public float lookDistance;
    private NavMeshPath path;
    private NavMeshAgent nav;
    public float timer = 5f;
    public float wanderTimer = 2f;
    private float currentTimer;
    private int cornerIndice;
    public float wanderDist, wanderRadius;
    public float fieldOfViewDist = 6f;
    public float fieldOfViewInSmoke = 3f;
    public float fieldOfViewAngle = 60f;
    private int firstWayPoint;
    public GameObject animCamera;
    public bool foundPlayer;
    // Start is called before the first frame update
    void Start() {
        foundPlayer = false;
        wayPointIndice = 0;
        activeBehaviour = 0;
        path = new NavMeshPath();
        currentTimer = 0;
        nav = gameObject.GetComponent<NavMeshAgent>();
        nav.ResetPath();
        firstWayPoint = Random.Range(0, wayPointsBundle.Length);
        if (nav.enabled) {
            nav.CalculatePath(wayPointsBundle[firstWayPoint].transform.position, path);
            nav.SetPath(path);
        }

    }

    // Update is called once per frame
    void Update() {
        if (!foundPlayer) {
            switch (activeBehaviour) {
                case 0:
                    Patrol();
                    break;

                case 1:
                    checkLocation();
                    break;

                case 2:
                    lookAtDestination();
                    break;

                case 3:
                    wander();
                    break;

            }
        } else {
            nav.isStopped = true;
        }
    }

    private void Patrol() {
        nav.isStopped = false;
        if (nav.enabled && !nav.hasPath) {
            wayPointIndice = findClosestWayPoint();
            nav.CalculatePath(wayPointsBundle[wayPointIndice].transform.position, path);
            nav.SetPath(path);
        }
        if (Vector3.Distance(transform.position, wayPointsBundle[wayPointIndice].transform.position) < wayPointDistance) {
            wayPointIndice = (wayPointIndice + 1) % wayPointsBundle.Length;

            if (nav.enabled) {
                nav.CalculatePath(wayPointsBundle[wayPointIndice].transform.position, path);
                nav.SetPath(path);
                //nav.destination = wayPointsBundle[wayPointIndice].transform.position;
            }
        }
    }
    private void checkLocation() {

        if (Vector3.Distance(transform.position, destination) > lookDistance) {
            nav.destination = destination;
        } else {
            nav.isStopped = true;
            if (currentTimer < timer) {
                currentTimer += Time.deltaTime;
            } else {
                activeBehaviour = 0;
                currentTimer = 0;
                nav.ResetPath();
                nav.isStopped = false;
            }

        }
    }
    public void goCheckNoise(Vector3 noisePos) {
        activeBehaviour = 1;
        destination = noisePos;
    }

    public void coverAnotherEnemy(Vector3 noisePos) {
        activeBehaviour = 2;
        destination = noisePos;
        nav.CalculatePath(destination, path);
        cornerIndice = 0;
    }

    private int findClosestWayPoint() {
        int closestPointIndex = 0;
        float closestPointDist = float.MaxValue;
        nav.isStopped = true;
        for (int i = 0; i < wayPointsBundle.Length; i++) {
            nav.CalculatePath(wayPointsBundle[i].transform.position, path);
            nav.SetPath(path);
            if (nav.remainingDistance < closestPointDist) {
                closestPointDist = nav.remainingDistance;
                closestPointIndex = i;
            }
        }

        return closestPointIndex;
    }
    private void lookAtDestination() {
        Ray ray = new Ray(transform.position, (destination - transform.position).normalized);
        nav.isStopped = true;

        int enemyLayerMask = 1 << 12;
        int itemLayerMask = 1 << 10;
        int Mask = itemLayerMask + enemyLayerMask;
        Mask = ~Mask;
        if (Physics.Raycast(ray, Vector3.Distance(transform.position, destination), Mask)) {
            Debug.DrawRay(transform.position, (destination - transform.position), Color.white, 10);
            nav.isStopped = false;
            //nav.destination = path.corners[cornerIndice];
            nav.SetPath(path);
            //if (Vector3.Distance(transform.position, path.corners[cornerIndice]) < wayPointDistance && cornerIndice < path.corners.Length) {
            //    cornerIndice++;
            //}

        } else {
            Debug.DrawRay(transform.position, (destination - transform.position), Color.blue, 10);
            cornerIndice = 0;
            nav.ResetPath();
            Quaternion targetRotation = Quaternion.LookRotation(destination - transform.position, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);
            if (currentTimer < timer) {
                currentTimer += Time.deltaTime;
            } else {
                nav.isStopped = false;
                activeBehaviour = 0;
                currentTimer = 0;
                nav.ResetPath();
                Debug.Log(activeBehaviour);
            }
        }
    }

    private Vector3 calculateWander() {
        Vector3 wanderCenter = transform.position + (transform.forward * wanderDist);
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * wanderRadius;
        randomDirection += wanderCenter;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas);
        return hit.position;
    }

    private void wander() {
        currentTimer += Time.deltaTime;
        if (currentTimer >= wanderTimer) {
            nav.isStopped = false;
            nav.destination = calculateWander();
            currentTimer = 0;
        }
    }

    public void enteredSmoke() {
        activeBehaviour = 3;
        currentTimer = 0;
    }
    public void stillInSmoke() {
        activeBehaviour = 3;
    }

    public void exitedSmoke() {
        activeBehaviour = 0;
        currentTimer = 0;
    }

    public void playerIsNear(GameObject player) {
        Ray ray = new Ray(transform.position, (player.transform.position - transform.position).normalized);
        RaycastHit hit;
        if (Vector3.Angle(player.transform.position - transform.position, transform.forward) < fieldOfViewAngle * 0.5f) {
            float viewDist = (activeBehaviour == 3) ? fieldOfViewInSmoke : fieldOfViewDist;
            int layerMask = 1 << 2;
            layerMask = ~layerMask;
            if (Physics.Raycast(ray, out hit, viewDist, layerMask)) {
                if (hit.transform.CompareTag("Player") && !foundPlayer) {
                    foundPlayer = true;
                    nav.isStopped = true;
                    hit.transform.GetComponent<PlayerControl>().inputEnabled = false;
                    animCamera.GetComponent<CameraAnimScript>().moveFromTo(hit.transform.Find("Main Camera"), transform.Find("Spot Light"), 3);
                    transform.parent.GetComponent<EnemyGroupBehavior>().PlayerWasFound();

                } else {
                    Debug.DrawRay(transform.position, hit.transform.position - transform.position, Color.green, 10);
                }

            } else {
                Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.blue, 10);
            }
        } else {
            Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.white, 10);
        }
    }

}