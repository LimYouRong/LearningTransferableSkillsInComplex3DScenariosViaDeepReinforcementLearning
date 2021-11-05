using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class JungleAgent : Agent
{
    public Rigidbody goal;
    public Rigidbody ball;
    public Rigidbody target;
    public TargetController targetController;
    Rigidbody player; //Can use this instead of declaring
    public float ballspeed = 1f;
    float previousY = 2.0f;
    float current = 1;
    public float level;
    public float agentJumpHeight=0.1f;
    public float pending_reward;//float GetCumulativeReward() default method
    public bool shot;
    public bool touched; 
    public override void Initialize()
    {
        player = this.GetComponent<Rigidbody>();
        targetController = target.GetComponent<TargetController>();
        targetController.agent = this;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        this.AddVecObs(sensor, ball.transform.localPosition);
        this.AddVecObs(sensor, ball.velocity);
        Quaternion rotation = ball.rotation;
        Vector3 normalizedBallRotation = rotation.eulerAngles / 360.0f;  // [0,1]
        this.AddVecObs(sensor, normalizedBallRotation);
        this.AddVecObs(sensor, ball.angularVelocity);
        //Possiblily normalize localPosition to be [0,1]
        //VectorSensor.AddObservation(transform.position.x / maxValue);
        this.AddVecObs(sensor, player.transform.localPosition);
        Vector3 playerbound = new Vector3(5f,100f,5f);
        this.AddVecObs(sensor, player.velocity);
        this.AddVecObs(sensor, player.rotation);
        rotation = player.rotation;
        Vector3 normalizedPlayerRotation = rotation.eulerAngles / 360.0f;  // [0,1]
        this.AddVecObs(sensor, normalizedPlayerRotation);
        this.AddVecObs(sensor, player.angularVelocity);
        
        // var xy_from_ball = Mathf.Pow(ball.transform.localPosition[0] - player.transform.localPosition[0],2) + Mathf.Pow(ball.transform.localPosition[2] - player.transform.localPosition[2],2);
        // sensor.AddObservation(xy_from_ball);

        this.AddVecObs(sensor,goal.transform.localPosition);
        this.AddVecObs(sensor,target.transform.localPosition);

    }
    //Normalize
    //normalizedValue = (currentValue - minValue)/(maxValue - minValue)
    private void AddVecObs(VectorSensor sensor, Vector3 vec)
    {
        for (int i = 0; i < 3; i++) sensor.AddObservation(vec[i]);
    }
    private void AddVecObs(VectorSensor sensor, Vector4 vec)
    {
        for (int i = 0; i < 4; i++) sensor.AddObservation(vec[i]);
    }

    private void AddVecObs(VectorSensor sensor, Quaternion vec)
    {
        for (int i = 0; i < 4; i++) sensor.AddObservation(vec[i]);
    }
    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;
        var dirToGoForwardAction = act[0];
        var dirToGoSideAction = act[1];
        // var rotateDirAction = act[2];
  
        var jumpAction = act[2];

       if (dirToGoForwardAction == 1)
            dirToGo =  1f * transform.forward;
        if (dirToGoForwardAction == 2)
            dirToGo =  -1f * transform.forward;
        // if (rotateDirAction == 1)
        //     rotateDir = transform.up * -1f;
        // else if (rotateDirAction == 2)
        //     rotateDir = transform.up * 1f;
        if (dirToGoSideAction == 1)
            dirToGo =  -1f * transform.right;
        if (dirToGoSideAction == 2)
            dirToGo =  1f * transform.right;
        if (jumpAction == 1 && (player.transform.localPosition.y<=0.51 && player.transform.localPosition.y>=0.50) && (player.transform.localPosition.x<=5 && player.transform.localPosition.x>=-5) && (player.transform.localPosition.z<=5 && player.transform.localPosition.z>=-5)){
            dirToGo = agentJumpHeight * transform.up;
        }   
        // transform.Rotate(rotateDir, Time.fixedDeltaTime * 300f);
        player.AddForce(dirToGo,ForceMode.VelocityChange);
    }   
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        MoveAgent(actionBuffers.DiscreteActions);
    }
    public void setupBallLevel0(){//FOR TESTING ONLY
        player.transform.localPosition = Vector3.up;
        ball.transform.localPosition = new Vector3(0, 6.0f, 0);
        goal.transform.localPosition = new Vector3(0, -1.0f,0);
        target.transform.localPosition = new Vector3(Random.Range(-5f,5), Random.Range(3f,10f),Random.Range(-5f,5));
    }
    public void setupBallLevel1(){//catching
        player.transform.localPosition = Vector3.up;
        ball.transform.localPosition = new Vector3(0, 6.0f, 0);
        ball.velocity = new Vector3(Random.Range(-1f,1),Random.Range(-1f,1),Random.Range(-1f,1));
        ball.angularVelocity = new Vector3(Random.Range(-1f,1),Random.Range(-1f,1),Random.Range(-1f,1));
        goal.transform.localPosition = new Vector3(0, -1.0f,0);
        target.transform.localPosition = new Vector3(0, -3.0f,0);
    }
    public void setupBallLevel2(){//knock into goal x,z area
        player.transform.localPosition = Vector3.up;
        ball.transform.localPosition = new Vector3(0, 6.0f, 0);
        ball.velocity = new Vector3(Random.Range(-1f,1),Random.Range(-1f,1),Random.Range(-1f,1));
        ball.angularVelocity = new Vector3(Random.Range(-1f,1),Random.Range(-1f,1),Random.Range(-1f,1));
        var goal_x = Random.Range(-5f,5);
        var goal_z = Random.Range(-5f,5);
        goal.transform.localPosition = new Vector3(goal_x, -1.0f,goal_z);
        target.transform.localPosition = new Vector3(Random.Range(-5f,5), Random.Range(3f,10f),Random.Range(-5f,5));


        // target.transform.localPosition = new Vector3(goal_x, 8.0f,goal_z);

        // goal.transform.localPosition = new Vector3(0, -1.0f,0);
    }
    public void setupBallLevel3(){//knock into goal x,z area + y
        player.transform.localPosition = Vector3.up;
        ball.transform.localPosition = new Vector3(0, 6.0f, 0);
        ball.velocity = new Vector3(Random.Range(-1f,1),Random.Range(-1f,1),Random.Range(-1f,1));
        ball.angularVelocity = new Vector3(Random.Range(-1f,1),Random.Range(-1f,1),Random.Range(-1f,1));
        var goal_x = Random.Range(-5f,5);
        var goal_z = Random.Range(-5f,5);
        goal.transform.localPosition = new Vector3(goal_x, -1.0f,goal_z);
        target.transform.localPosition = new Vector3(Random.Range(-5f,5), Random.Range(3f,10f),Random.Range(-5f,5));

        // target.transform.localPosition = new Vector3(goal_x, Random.Range(3f,10f),goal_z);
        // goal.transform.localPosition = new Vector3(Random.Range(-1f,1), -1.0f,Random.Range(-1f,1));
    }
    public void setupBallLevel4(){//knock into target while standing on target
        player.transform.localPosition = Vector3.up;
        ball.transform.localPosition = new Vector3(0, 6.0f, 0);
        ball.velocity = new Vector3(Random.Range(-1f,1),Random.Range(-1f,1),Random.Range(-1f,1));
        ball.angularVelocity = new Vector3(Random.Range(-1f,1),Random.Range(-1f,1),Random.Range(-1f,1));
        goal.transform.localPosition = new Vector3(Random.Range(-5f,5), -1.0f,Random.Range(-5f,5));
        target.transform.localPosition = new Vector3(Random.Range(-5f,5), Random.Range(3f,10f),Random.Range(-5f,5));

        // goal.transform.localPosition = new Vector3(Random.Range(-1f,1), -1.0f,Random.Range(-1f,1));
    }

    public override void OnEpisodeBegin(){
        // ball.transform.localPosition = new Vector3(Random.Range(-0.1f,0.1f), 4.0f, Random.Range(-0.1f,0.1f));
        level = Academy.Instance.EnvironmentParameters.GetWithDefault("Bounce", 1);
        //level would be used to setup the environment in more complex environment
        ball.transform.localPosition = new Vector3(0, 6.0f, 0);
        ball.velocity = Vector3.zero;
        ball.rotation = Quaternion.Euler(Vector3.zero);
        ball.angularVelocity = Vector3.zero;

        player.transform.localPosition = Vector3.up;
        player.velocity = Vector3.zero;
        player.rotation = Quaternion.Euler(Vector3.zero);
        player.angularVelocity = Vector3.zero;

        touched = false;
        shot = false;
        pending_reward = 0f;
        if (level==0){
            setupBallLevel0();
        }else if (level==1){
            setupBallLevel1();
        }else if(level==2){
            setupBallLevel2();
        }else if(level==3){
            setupBallLevel3();
        }else if(level==4){
            setupBallLevel4();
        }else if(level==5){
            setupBallLevel4();
        //     setupGoalLevel2();
        // }else if(level==6){//not ready, max level now is 5
        //     setupGoalLevel3();
        }
        current = 0;
    }
    //x normalized = (x – x minimum) / (x maximum – x minimum)
    public void OutOfBoundFixedUpdate(){
        if (ball.transform.localPosition[1]<= 0.6 || player.transform.localPosition[1]<0){
            AddReward(-1f);
            EndEpisode();
        }
        if (ball.transform.localPosition[1]> previousY){
            previousY = ball.transform.localPosition[1];
        }
    }
    public void Level1FixedUpdates(){
        float xz_from_player = Mathf.Pow(player.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(player.transform.localPosition[2] - ball.transform.localPosition[2],2);
        if (ball.transform.localPosition[1]<=0.6 || player.transform.localPosition[1]<0){
                if (xz_from_player>15){
                    SetReward(-1f);
                }else{
                    SetReward(-((-xz_from_player+15)/(0-15)));
                }
                EndEpisode();
        }
        if (ball.transform.localPosition[1]> previousY){
            previousY = ball.transform.localPosition[1];
        }
    }
    public void Level2FixedUpdates(){//Agent will bounce ball towards goal
        float xz_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
        // float y = (ball.transform.localPosition[1]-12f/-10f);
        if(xz_from_goal<15 && y<12f){
            score = -((-xz_from_goal+15)/(0-15));
            AddReward(score);
            EndEpisode();
        }
        OutOfBoundFixedUpdate();
    }
    public void Level3FixedUpdates(){//Agent will bounce ball as low as possible at goal
        float xy_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
        if(xy_from_goal<10){//Standing close to the goal
            score = -((-xy_from_goal+15)/(0-15));
            pending_reward = score*0.5f;
            AddReward(ScoredAGoal*0.5f);
            // float y = (ball.transform.localPosition[1]-12f/-10f);
            // float normalizedValue = ((xy_from_ball -25)/(-25)) * y;
            // reward +=normalizedValue;
            // AddReward(y);//normalize to max 1
            // EndEpisode();
        }
        if (ball.transform.localPosition[1]<= 0.6 || player.transform.localPosition[1]<0){
            if (pending_reward>0){//Distance from ball
                float xz_from_player = Mathf.Pow(player.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(player.transform.localPosition[2] - ball.transform.localPosition[2],2);
                if (xz_from_player<10){
                    AddReward(-((-xz_from_player+10)/(0-10)) * 0.1f);
                }
            }
            AddReward(-1f);
            EndEpisode();
        }
        if (ball.transform.localPosition[1]> previousY){
            previousY = ball.transform.localPosition[1];
        }
    }

    public void Level4FixedUpdates(){//Agent will bounce ball to goal, goal relocate, bounce again.
        // float xy_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
        // if(xy_from_goal<10){
        //     float y = (ball.transform.localPosition[1]-12f/-10f);
        //     // float normalizedValue = ((xy_from_ball -25)/(-25)) * y;
        //     // reward +=normalizedValue;
        //     AddReward(y/10);//normalize to max 1
        //     goal.transform.localPosition = new Vector3(Random.Range(-5f,5), -1.0f,Random.Range(-5f,5));
        // }
        OutOfBoundFixedUpdate();
    }
    public void Level5FixedUpdates(){//Bounce to multiple goal then make one shot only
        float xyz_from_target = Mathf.Pow(target.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(target.transform.localPosition[1] - ball.transform.localPosition[1],2)+ Mathf.Pow(target.transform.localPosition[2] - ball.transform.localPosition[2],2);        
        if(xyz_from_target<25){//score for closeby, bonus if it collided
            float score = -((-xyz_from_target+25)/(0-25));
            pending_reward = score*0.5f;//Only give reward if it manage to catch the ball on the other side
        }
        //     float y = (ball.transform.localPosition[1]-12f/-10f);
        //     // float normalizedValue = ((xy_from_ball -25)/(-25)) * y;
        //     // reward +=normalizedValue;
        //     AddReward(y/100);//normalize to max 1
        //     goal.transform.localPosition = new Vector3(Random.Range(-8f,8), -1.0f,Random.Range(-8f,8));
        // }

        OutOfBoundFixedUpdate();
    }
    public void ScoredAGoal(){
        if (level>=5){
            AddReward(0.5f);
            target.transform.localPosition = new Vector3(Random.Range(-5f,5), Random.Range(3f,10f),Random.Range(-5f,5));
        }
    }

    void FixedUpdate()
    {  
        if(level==1){
            Level1FixedUpdates();
        }else if(level==2){
            Level2FixedUpdates();
        }else if(level==3){
            Level3FixedUpdates();
        }else if(level==4){
            Level4FixedUpdates();
        }else if(level==5){//Single shot
            Level5FixedUpdates();
        }else if(level==6){//Score as much as it can, target will move if hit
            Level5FixedUpdates();
        }
        // if (level==1){
        //     OutOfBoundFixedUpdate();
        //     // Level0FixedUpdates();
        // }
        // else if (level==2){
        //     Level2FixedUpdates();
        // }
        // else{
        //     OutOfBoundFixedUpdate();
        // }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody == ball)
        {
            var dir = collision.contacts[0].point - transform.position;
            dir = dir.normalized;
            collision.gameObject.GetComponent<Rigidbody>().AddForce((dir * 0.1f));
            //^ might not be working
            current+=1;
            previousY =0;

            if (level== 1){
                AddReward(1);
                EndEpisode();
            }else if(level==3){
                if (pending_reward>0){
                    AddReward(pending_reward);
                    EndEpisode();
                }
            }else if(level==4 && touched==true){
                float xy_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
                if (xy_from_goal<=10){
                    float normalised_reward = -((-xy_from_goal+10)/(0-10));
                    AddReward(normalised_reward/5);//5 times
                    goal.transform.localPosition = new Vector3(Random.Range(-8f,8), -1.0f,Random.Range(-8f,8));
                } 
                // print("did it work?");
                // float xy_from_ball = Mathf.Pow(target.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(target.transform.localPosition[2] - ball.transform.localPosition[2],2);
                // AddReward(xy_from_ball);
                // print(xy_from_ball+"??");
                if (GetCumulativeReward() >=1){//1 means perfected the ability
                    SetReward(1);
                    EndEpisode();
                }
                // EndEpisode();
                // if(touched==true && xy_from_ball<25){
                    
                //     if (GetCumulativeReward() >=10){
                //         SetReward(10);
                //         EndEpisode();
                //     }
                // }
            }else if(level==5 && touched==true){
                float xy_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
                if (xy_from_goal<=10){
                    shot=true;
                }else{
                    shot=false;
                }
                if (pending_reward>= 0.1 || GetCumulativeReward() >=0.1){//if it make a shot, end epsoide
                    AddReward(pending_reward);
                    EndEpisode();
                }
            }else if(level==6 && touched==true){
                float xy_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
                if (xy_from_goal<=10){
                    shot=true;
                }else{
                    shot=false;
                }
            }
            touched = true;
            // else if(level==2 || level==3){//Bounce on player and goal
            //     float xy_from_ball = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
            //     if (xy_from_ball<25){
            //         float normalizedValue = ((xy_from_ball -25)/(-25));
            //         reward +=normalizedValue;
            //         AddReward(reward);
            //     }
            //     EndEpisode();
                
            // }else if(level==4){//bounce reaches as low as possible, while close to goal
            //     float xy_from_ball = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
            //     if (xy_from_ball<25){

            //         float y = (ball.transform.localPosition[1]-26.6f/-25f);
                    
            //         float normalizedValue = ((xy_from_ball -25)/(-25)) * y;
            //         reward +=normalizedValue;
            //         AddReward(normalizedValue);//normalize to max 1
            //     }
            //     EndEpisode();
            //}
            // else if(level==2){
            //     float xz_from_ball = Mathf.Pow(goal.transform.localPosition[0] - player.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - player.transform.localPosition[2],2);
            //     if (xz_from_ball<25){
            //         float normalizedValue = ((xz_from_ball -25)/(-25)/10);
            //         reward +=normalizedValue;
            //         AddReward(normalizedValue);
            //     }
            //     // float distanceToTarget = Vector3.Distance(goal.transform.localPosition, ball.transform.localPosition);
            //     // AddReward(xy_from_ball);
            // }
            //Try use -current if out of bound next time.
            // if (current >=5){
            //     AddReward(0.5f);
            //     print(reward);
            //     EndEpisode();
            // }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[1] = 2;
        }
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
        // if (Input.GetKey(KeyCode.Q))
        // {
        //     discreteActionsOut[2] = 1;
        // }
        // if (Input.GetKey(KeyCode.E))
        // {
        //     discreteActionsOut[2] = 2;
        // }
        //if (Input.GetKey(KeyCode.B)){
        //     discreteActionsOut[3] = 1;
        // }
        if(Input.GetKey(KeyCode.Space)){
            discreteActionsOut[2] = 1;
        }else{
             discreteActionsOut[2] = 0;
        }
        // = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }
}
