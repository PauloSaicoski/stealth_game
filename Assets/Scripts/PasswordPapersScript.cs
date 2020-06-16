using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasswordPapersScript : MonoBehaviour {
    private int passwordInt;
    private string passWord;
    private List<GameObject> papers;
    public GameObject safePad;
    // Start is called before the first frame update
    void Start() {
        passwordInt = Random.Range(0, 99999);
        passWord = passwordInt.ToString();
        papers = new List<GameObject>();
        while (passWord.Length < 5) {
            passWord = "0" + passWord;
        }
        foreach (Transform t in transform) {
            if (t != transform) {
                papers.Add(t.gameObject);
                t.GetChild(0).GetComponent<TMPro.TextMeshPro>().SetText(passWord);
                t.gameObject.SetActive(false);
            }
        }
        int paperIndice = Random.Range(0, papers.Count);
        papers[paperIndice].SetActive(true);
        safePad.GetComponent<PadScript>().password = passWord;
        Debug.Log(passWord);

    }

    // Update is called once per frame
    void Update() {

    }
}