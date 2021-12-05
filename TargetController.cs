using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    public JungleAgent agent;
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other){
        // if col.gameObject.CompareTag()
        // print("ball collied");
        // print(col.gameObject);
        if (other.gameObject.CompareTag("Ball")){
            agent.ScoredAGoal();
        }
    }
}
