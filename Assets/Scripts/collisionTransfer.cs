using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionTransfer : MonoBehaviour
{
    public int numParentsUp = 3;
    void OnCollisionEnter(Collision collision)
    {
        Transform parentObj = transform;
        for(int i = 0; i < numParentsUp; i++) {}
            parentObj = parentObj.parent;
        
        if (parentObj.GetComponent<Trapdoor>())
            parentObj.GetComponent<Trapdoor>().CollisionDetected(collision);
    }

    private void OnCollisionStay(Collision collision) {
        transform.parent.parent.parent.GetComponent<Trapdoor>().CollisionStayDetected(collision);
    }
}
