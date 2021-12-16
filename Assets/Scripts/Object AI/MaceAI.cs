using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaceAI : MonoBehaviour
{

    public List<GameObject> objectsInsideTrigger;    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerStay(Collider other) {
        objectsInsideTrigger.Add(other.gameObject);
        
    }



    private void OnCollisionEnter(Collision other) {
        // on the first frame the level is loaded in the editor, it can break 8 balls. this is a workaround
        if (FindObjectOfType<EditorLogic>().editorMode) {
            return;
        }
        
        // if the player is also in the trigger area, then the player is hit
        if ((other.gameObject.tag == "Player" || other.gameObject.tag == "8Ball") && objectsInsideTrigger.Contains(other.gameObject)) {
            if (other.gameObject.GetComponent<ballanceRoll>())
                other.gameObject.GetComponent<ballanceRoll>().breakBall();
            else 
                other.gameObject.GetComponent<marbleRoll>().breakBall();
        }
    }

    // Update is called once per frame
    void Update()
    {
        objectsInsideTrigger.Clear();
    }
}
