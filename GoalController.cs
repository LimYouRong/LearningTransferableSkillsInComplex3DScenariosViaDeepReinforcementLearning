using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    void OnCollisionEnter(Collision col){
        // if col.gameObject.CompareTag()
        // print("goal collied");
        if (col.gameObject.CompareTag("Finish")){
            
        }
    }

    private void OnTriggerEnter(Collider other){
        // if (other.gameObject.CompareTag("Ball")){
        //     print("hi");
        // }
    }
}
