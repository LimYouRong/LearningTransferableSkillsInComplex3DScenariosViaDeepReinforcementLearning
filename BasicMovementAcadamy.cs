using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
public class BasicMovementAcadamy : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody goal;
    public void Awake()
    {
        Academy.Instance.OnEnvironmentReset += EnvironmentReset;
    }

    void EnvironmentReset()
    {
        ResetGoal();
        // Reset the scene here
    }
    void ResetGoal(){
        // goal.transform.localPosition = new Vector3(Random.Range(-5f,5), 2.0f, Random.Range(-5,5));
    }
}
