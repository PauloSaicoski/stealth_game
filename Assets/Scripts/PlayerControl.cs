using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {
    CharacterController characterController;
    public float standSpeed = 6.0f;
    public float crouchSpeed = 3.0f;
    private bool crouch;
    private float speed;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    private Transform playerCamera;
    private Vector3 lastMouse;
    public float camSens = 1.25f;
    public float itemMaxDistancePickUp = 1f;
    private Ray ray;
    private RaycastHit hit;
    public GameObject crosshairDefault, crosshairSelectable;
    public GameObject itemPos, itemThrowPos;
    private GameObject item;
    public int maxItemCount;
    private int itemCount;
    public float throwPower;
    private bool canThrow;
    public bool inputEnabled;

    private Vector3 moveDirection = Vector3.zero;
    // Start is called before the first frame update
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();
        crouch = false;
        playerCamera = transform.Find("Main Camera");
        lastMouse = Input.mousePosition;
        ray = new Ray();
        itemCount = 0;
        canThrow = true;
    }

    // Update is called once per frame
    void Update() {
        if (inputEnabled) {

            //lastMouse = Input.mousePosition - lastMouse ;
            //lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0 );
            //lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x , transform.eulerAngles.y + lastMouse.y, 0);
            transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y + Input.GetAxis("Mouse X") * camSens, 0.0f);
            playerCamera.eulerAngles = new Vector3(playerCamera.eulerAngles.x - Input.GetAxis("Mouse Y") * camSens, transform.eulerAngles.y, 0.0f);
            //lastMouse =  Input.mousePosition;
            if (Input.GetKey(KeyCode.LeftControl)) {
                crouch = true;
                speed = crouchSpeed;
            } else {
                crouch = false;
                speed = standSpeed;
            }
            if (characterController.isGrounded) {
                moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= speed;

                if (Input.GetButton("Jump")) {
                    moveDirection.y = jumpSpeed;
                }
            }
            //transform.Rotate(0.0f, Input.GetAxis("Mouse X"), 0.0f);
            moveDirection.y -= gravity * Time.deltaTime;

            characterController.Move(moveDirection * Time.deltaTime);
            ray.origin = playerCamera.position;
            ray.direction = playerCamera.TransformDirection(Vector3.forward);

            crosshairDefault.SetActive(true);
            crosshairSelectable.SetActive(false);

            if (Physics.Raycast(ray, out hit, itemMaxDistancePickUp)) {

                if (hit.collider.CompareTag("Item")) {
                    crosshairDefault.SetActive(false);
                    crosshairSelectable.SetActive(true);

                    if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.E)) && itemCount < maxItemCount) {
                        itemCount++;
                        item = hit.collider.transform.parent.gameObject;
                        item.transform.SetParent(transform, false);
                        item.transform.position = itemPos.transform.position;
                        item.GetComponent<ItemScript>().picked = true;
                    }
                }
                if (hit.collider.CompareTag("Interactable")) {
                    canThrow = false;
                    crosshairDefault.SetActive(false);
                    crosshairSelectable.SetActive(true);

                    if (Input.GetMouseButtonDown(0)) {
                        hit.collider.GetComponent<InteractableScript>().interacted();
                    }

                } else {
                    canThrow = true;
                }
            }
            if (Input.GetMouseButtonDown(0) && itemCount > 0 && canThrow) {
                item.GetComponent<ItemScript>().picked = false;
                item.transform.SetParent(null);
                item.transform.position = itemThrowPos.transform.position;
                item.GetComponent<Rigidbody>().isKinematic = false;
                if (!crouch) {
                    item.GetComponent<ItemScript>().thrown = true;
                    item.GetComponent<Rigidbody>().AddForce(playerCamera.forward * throwPower);
                }
                itemCount--;
            }
        }
    }
}