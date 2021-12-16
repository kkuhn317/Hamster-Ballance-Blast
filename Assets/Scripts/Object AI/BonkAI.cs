using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BonkAI : MonoBehaviour
{

    bool started = false;
    bool finished = false;
    Vector3 originPosition;
    Vector3 newPosition;
    Animator animator;
    GameObject target;
    Quaternion newRotation;
    private float moveSpeed = 10;
    private float swingSpeed = 10;

    void Start() {
        originPosition = transform.position + new Vector3(0, 2, 0);
        animator = GetComponent<Animator>();
        newPosition = transform.parent.position;
        newRotation = transform.parent.rotation;
    }

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject [] fakePlayers = GameObject.FindGameObjectsWithTag("fakePlayer");
        List<GameObject> allTargets = fakePlayers.ToList();
        allTargets.Add(player);

        target = GetClosestEnemy(allTargets, transform).gameObject;

        
        if (!started) {
            
            if (Vector3.Distance(originPosition, target.transform.position) < 10) {
                started = true;
                InvokeRepeating("findPosition", 0, 1);
                InvokeRepeating("swingAtBall", .5f, 1);
            }
        } else if (Vector3.Distance(originPosition, target.transform.position) > 25 && !finished ) {
            finished = true;
            CancelInvoke();
        }

        transform.parent.position = Vector3.Lerp(transform.parent.position, newPosition, moveSpeed * Time.deltaTime);
		transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, newRotation, swingSpeed * Time.deltaTime);
    }

    Transform GetClosestEnemy(List<GameObject> enemies, Transform fromThis)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = fromThis.position;
        foreach (GameObject potentialTargetObject in enemies)
        {
            Transform potentialTarget = potentialTargetObject.transform;
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }
        return bestTarget;
    }

    void findPosition() {

        Vector3 playerPos = target.transform.position;
        Rigidbody playerRB = target.GetComponent<Rigidbody>();

        // Find the direction to look at the target
        

        // First find where the ball is going
        Vector3 playerGoingToPos = playerPos + (playerRB.velocity * .6f);
        

        Vector3 lookPosition = new Vector3(playerGoingToPos.x, transform.parent.position.y, playerGoingToPos.z);
        Vector3 relativePos = lookPosition - transform.parent.position;
        newRotation = Quaternion.LookRotation(relativePos);

        // Find position to move to so that it can hit the target
        // go back some units, and go up 2 units
        Vector3 directionAway = newRotation * Vector3.forward * -5 * (float)globalVariables.levelScale;
        directionAway.y = (1.5f * (float)globalVariables.levelScale) - .5f + (1 * (float)globalVariables.levelScale);
        // half the hammer width * level scale - ball radius gets us the distance to the floor
        // then we add the extra height to compensate for the hammer swing point

        newPosition = new Vector3(playerGoingToPos.x, playerPos.y, playerGoingToPos.z) + directionAway;

    }

    void swingAtBall() {
        animator.SetTrigger("SwingTrig");
    }

}
