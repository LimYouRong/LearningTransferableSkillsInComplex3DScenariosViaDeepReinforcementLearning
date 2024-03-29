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
    float previousY = 2.0f;//Not used
    float current = 0;
    public float level;
    public float agentJumpHeight=0.1f;
    public float pending_reward;//float GetCumulativeReward() default method
    public bool shot;
    public bool touched; 
    public int no_bounce = 0;
    public int score_no = 0;
    public bool touched_on_goal;
    public float minimum_height;
    public bool on_ground = false;
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
        this.AddVecObs(sensor, player.transform.localPosition);
        Vector3 playerbound = new Vector3(5f,100f,5f);
        this.AddVecObs(sensor, player.velocity);
        this.AddVecObs(sensor, player.rotation);
        rotation = player.rotation;
        Vector3 normalizedPlayerRotation = rotation.eulerAngles / 360.0f;  // [0,1]
        this.AddVecObs(sensor, normalizedPlayerRotation);
        this.AddVecObs(sensor, player.angularVelocity);
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
        if (jumpAction ==1 && on_ground==true){
            dirToGo = agentJumpHeight * transform.up;
            on_ground = false;
        // if (jumpAction == 1 && (player.transform.localPosition.y<=0.51 && player.transform.localPosition.y>=0.50) && (player.transform.localPosition.x<=5 && player.transform.localPosition.x>=-5) && (player.transform.localPosition.z<=5 && player.transform.localPosition.z>=-5)){
            // dirToGo = agentJumpHeight * transform.up;
        }   
        // transform.Rotate(rotateDir, Time.fixedDeltaTime * 300f);
        player.AddForce(dirToGo,ForceMode.VelocityChange);
    }   
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        MoveAgent(actionBuffers.DiscreteActions);
    }
    void spawnGoalTarget(){
        float goal_x = Random.Range(-2f,2);
        float goal_z = Random.Range(-2f,2);
        goal.transform.localPosition = new Vector3(goal_x ,0f,goal_z);
        target.transform.localPosition = new Vector3(Random.Range(-1f,1) + goal_x, Random.Range(2f,3f),Random.Range(-1f,1)+ goal_z);
    }
    void spawnGoalTargetReverse(){
        float goal_x = Random.Range(-2f,2);
        float goal_z = Random.Range(-2f,2);
        float player_x = Random.Range(-3f,3f);
        float player_z = Random.Range(-3f,3f);

        // float player_x = Random.Range(-1f,1) + goal_x;
        // float player_z = Random.Range(-1f,1) + goal_z;

        ball.transform.localPosition = new Vector3(player_x, 3.0f, player_z);
        ball.velocity = new Vector3(Random.Range(-1f,1),Random.Range(-1f,1),Random.Range(-1f,1));
        ball.angularVelocity = new Vector3(Random.Range(-1f,1),Random.Range(-1f,1),Random.Range(-1f,1));

        goal.transform.localPosition = new Vector3(goal_x ,0f,goal_z);
        target.transform.localPosition = new Vector3(Random.Range(-1f,1) + goal_x, Random.Range(1f,2f),Random.Range(-1f,1)+ goal_z);
        player.transform.localPosition = new Vector3(player_x, 0.1f, player_z);
    }
    public void setupBallLevel0(){//FOR TESTING ONLY
        player.transform.localPosition = Vector3.up;
        ball.transform.localPosition = new Vector3(0, 3.0f, 0);
        goal.transform.localPosition = new Vector3(0, -1.0f,0);
        spawnGoalTarget();
        // target.transform.localPosition = new Vector3(Random.Range(-5f,5), Random.Range(3f,10f),Random.Range(-5f,5));
    }
    // public void setupBallLevel1(){//catching
    //     player.transform.localPosition = new Vector3(Random.Range(-1f,1), 0.1f, Random.Range(-1f,1));
    //     ball.transform.localPosition = new Vector3(0, 4.0f, 0);
    //     ball.velocity = new Vector3(Random.Range(-1f,1),Random.Range(-1f,1),Random.Range(-1f,1));
    //     ball.angularVelocity = new Vector3(Random.Range(-1f,1),Random.Range(-1f,1),Random.Range(-1f,1));
    //     spawnGoalTarget();
    // }   
    public void setupBallLevel1(){
        spawnGoalTargetReverse();
    }
    public void setupBallLevel2(){//Sloping ground
        spawnGoalTargetReverse();
        float ground_x = Random.Range(-5f,5f);
        float ground_z = Random.Range(-5f,5f);

        goal.transform.localRotation =  Quaternion.Euler(ground_x ,0f,ground_z);
        this.transform.parent.localRotation = Quaternion.Euler(ground_x ,0f,ground_z);
    }
    public void setupBallCurr1(){//knock into goal x,z area
        spawnGoalTargetReverse();
        // target.sphereCollider.localradius = 3f;
        target.transform.localScale = new Vector3(2f,2f,2f);
    }

    public void setupBallCurr2(){//knock into goal x,z area
        spawnGoalTargetReverse();
        target.transform.localScale = new Vector3(1.5f,1.5f,1.5f);
    }

    public void setupBallCurr3(){//knock into goal x,z area
        spawnGoalTargetReverse();
        target.transform.localScale = new Vector3(1.3f,1.3f,1.3f);
    }

    public void setupBallCurr4(){//knock into goal x,z area
        spawnGoalTargetReverse();
        target.transform.localScale = new Vector3(1.2f,1.2f,1.2f);
    }

    public void setupBallCurr5(){//knock into goal x,z area
        spawnGoalTargetReverse();
        target.transform.localScale = new Vector3(1.1f,1.1f,1.1f);
    }

    public void setupBallLevel4(){//knock into goal x,z area + y
        player.transform.localPosition = Vector3.up;
        ball.transform.localPosition = new Vector3(0, 4.0f, 0);
        ball.velocity = new Vector3(Random.Range(-1f,1),Random.Range(-1f,1),Random.Range(-1f,1));
        ball.angularVelocity = new Vector3(Random.Range(-1f,1),Random.Range(-1f,1),Random.Range(-1f,1));
        goal.transform.localPosition = new Vector3(Random.Range(-8f,8), -1.0f,Random.Range(-8f,8));
        target.transform.localPosition = new Vector3(0, -3.0f,0);
        // target.transform.localPosition = new Vector3(Random.Range(-5f,5), Random.Range(3f,10f),Random.Range(-5f,5));

        // target.transform.localPosition = new Vector3(goal_x, Random.Range(3f,10f),goal_z);
        // goal.transform.localPosition = new Vector3(Random.Range(-1f,1), -1.0f,Random.Range(-1f,1));
    }
    public void setupBallLevel5(){//knock into target while standing on target
        player.transform.localPosition = Vector3.up;
        ball.transform.localPosition = new Vector3(0, 4.0f, 0);
        ball.velocity = new Vector3(Random.Range(-1f,1),Random.Range(-1f,1),Random.Range(-1f,1));
        ball.angularVelocity = new Vector3(Random.Range(-1f,1),Random.Range(-1f,1),Random.Range(-1f,1));
        goal.transform.localPosition = new Vector3(Random.Range(-8f,8), -1.0f,Random.Range(-8f,8));
        target.transform.localPosition = new Vector3(0, -3.0f,0);

        // target.transform.localPosition = new Vector3(2, 5.0f,2);
        // target.transform.localPosition = new Vector3(Random.Range(-5f,5), Random.Range(3f,10f),Random.Range(-5f,5));

        // goal.transform.localPosition = new Vector3(Random.Range(-1f,1), -1.0f,Random.Range(-1f,1));
    }
    public void setupBallLevel6(){//knock into target while standing on target
        player.transform.localPosition = Vector3.up;
        ball.transform.localPosition = new Vector3(0, 4.0f, 0);
        ball.velocity = new Vector3(Random.Range(-1f,1),Random.Range(-1f,1),Random.Range(-1f,1));
        ball.angularVelocity = new Vector3(Random.Range(-1f,1),Random.Range(-1f,1),Random.Range(-1f,1));
        goal.transform.localPosition = new Vector3(Random.Range(-8f,8), -1.0f,Random.Range(-8f,8));
        // target.transform.localPosition = new Vector3(0, -3.0f,0);
        target.transform.localPosition = new Vector3(Random.Range(-5f,5), Random.Range(3f,10f),Random.Range(-5f,5));

        // goal.transform.localPosition = new Vector3(Random.Range(-1f,1), -1.0f,Random.Range(-1f,1));
    }

    public override void OnEpisodeBegin(){
        // ball.transform.localPosition = new Vector3(Random.Range(-0.1f,0.1f), 4.0f, Random.Range(-0.1f,0.1f));
        level = Academy.Instance.EnvironmentParameters.GetWithDefault("Bounce", 11);
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
        touched_on_goal = false;
        pending_reward = 0f;
        score_no = 0;
        no_bounce = 0;
        on_ground = true;
        minimum_height = (2 * player.transform.lossyScale.y) * 0.8f;
        if (level==0){
            setupBallLevel0();
        }else if (level==1){
            setupBallLevel1();
        }else if (level==2){
            setupBallLevel2();
        }else if(level==11){
            setupBallCurr1();
        }else if(level==12){
            setupBallCurr2();
        }else if(level==13){
            setupBallCurr3();
        }else if(level==14){
            setupBallCurr4();
        }else if(level==15){
            setupBallCurr5();
        }else if(level==16){
            setupBallLevel1();
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
    public void getResults(){
        float xz_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
        float xz_from_target = Mathf.Pow(target.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(target.transform.localPosition[1] - ball.transform.localPosition[1],2) + Mathf.Pow(target.transform.localPosition[2] - ball.transform.localPosition[2],2);
        if (shot){
            float xz_from_player = Mathf.Pow(player.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(player.transform.localPosition[2] - ball.transform.localPosition[2],2);
            if (xz_from_player<=80){
                AddReward(-((-xz_from_player+80)/(0-80))*0.1f);
            }
        }else if(previousY<=40){
            AddReward(((40-previousY)/(40))*0.1f);
        }else{
            if(xz_from_goal<=80){
                AddReward(-((-xz_from_goal+80)/(0-80))*0.1f);
            }
        }
        // print(GetCumulativeReward());
        EndEpisode();
    
    }
    //Level1: Bounce ball towards goal
    public void RewardShapingFixedUpdates(){//Goal focused training
        float xz_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
        float xz_from_target = Mathf.Pow(target.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(target.transform.localPosition[1] - ball.transform.localPosition[1],2) + Mathf.Pow(target.transform.localPosition[2] - ball.transform.localPosition[2],2);
        
        if (ball.transform.localPosition[1]<=-2f || player.transform.localPosition[1]<-2f){
            getResults();
        }
        if (xz_from_target< previousY && touched_on_goal){
            previousY = xz_from_target;
        }
    }
    //Bounce ball towards goal then the other goal
    public void CurrLearningFixedUpdates(){
        if (ball.transform.localPosition[1]<=-2f || player.transform.localPosition[1]<-2f){
            EndEpisode();
        }
    }
    public void Level3FixedUpdates(){
        float xz_from_target = Mathf.Pow(target.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(target.transform.localPosition[1] - ball.transform.localPosition[1],2) + Mathf.Pow(target.transform.localPosition[2] - ball.transform.localPosition[2],2);
        float xz_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
        
        if (GetCumulativeReward()>=1){
            SetReward(1f);
            EndEpisode();
        }
        if (ball.transform.localPosition[1]<=0.6 || player.transform.localPosition[1]<0){
                if (previousY<=40){
                    AddReward(((40-previousY)/(40))*0.2f);
                }
                AddReward(-1f);
                // print(GetCumulativeReward());
                EndEpisode();
        }
        if (xz_from_target< previousY && touched_on_goal){
            previousY = xz_from_target;
        }
    }
    //Ball touch object on goal
        public void Level4FixedUpdates(){
        float xz_from_target = Mathf.Pow(target.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(target.transform.localPosition[1] - ball.transform.localPosition[1],2) + Mathf.Pow(target.transform.localPosition[2] - ball.transform.localPosition[2],2);
        float xz_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
        
        if (GetCumulativeReward()>=1){
            SetReward(1);
            EndEpisode();
        }
        if (ball.transform.localPosition[1]<=0.6 || player.transform.localPosition[1]<0){
                if (previousY<=40){
                    AddReward(((40-previousY)/(40))*0.2f);
                }
                AddReward(-1f);
                EndEpisode();
        }
        if (xz_from_target< previousY && touched_on_goal){
            previousY = xz_from_target;
        }
    }
        public void Level5FixedUpdates(){
        float xz_from_target = Mathf.Pow(target.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(target.transform.localPosition[1] - ball.transform.localPosition[1],2) + Mathf.Pow(target.transform.localPosition[2] - ball.transform.localPosition[2],2);
        float xz_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);

        if (GetCumulativeReward()>=1){
            SetReward(1);
            EndEpisode();
        }
        if (ball.transform.localPosition[1]<=0.6 || player.transform.localPosition[1]<0){
                if (previousY<=40){
                    AddReward(((40-previousY)/(40))*0.2f);
                }
                AddReward(-1f);
                // print(GetCumulativeReward());
                EndEpisode();
        }
        if (xz_from_target< previousY && touched_on_goal){
            previousY = xz_from_target;
        }
    }
    public void Level6FixedUpdates(){
        float xz_from_target = Mathf.Pow(target.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(target.transform.localPosition[1] - ball.transform.localPosition[1],2) + Mathf.Pow(target.transform.localPosition[2] - ball.transform.localPosition[2],2);
        float xz_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
        if (GetCumulativeReward()>=1){
            SetReward(1f);
            EndEpisode();
        }
        if (ball.transform.localPosition[1]<=0.6 || player.transform.localPosition[1]<0){
                if (previousY<=40){
                    AddReward(((40-previousY)/(40))*0.2f);
                }
                AddReward(-1f);
                // print(GetCumulativeReward());
                EndEpisode();
        }
        if (xz_from_target< previousY && touched_on_goal){
            previousY = xz_from_target;
        }
    }
    // public void Level7FixedUpdates(){
    //     float xz_from_target = Mathf.Pow(target.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(target.transform.localPosition[1] - ball.transform.localPosition[1],2) + Mathf.Pow(target.transform.localPosition[2] - ball.transform.localPosition[2],2);
    //     float xz_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
    //     // if (touched && xz_from_target<1){
    //     //     AddReward(1f);
    //     //     EndEpisode();
    //     // }
    //     // if (touched && ball.transform.localPosition[0]>=-5f && ball.transform.localPosition[0]<=5f && ball.transform.localPosition[1]>=0.6f && ball.transform.localPosition[2]>=-5f && ball.transform.localPosition[2]<=5f){
    //     //     AddReward(0.0000001f);
    //     // }
    //     // if (touched && xz_from_target<15){
    //     //     AddReward(0.5f);
    //     //     previousY = 0;
    //     //     spawnGoalTarget();
    //     // }
    //     if (GetCumulativeReward()>=1){
    //         AddReward(1f);
    //         EndEpisode();
    //     }
    //     if (GetCumulativeReward()>0){
    //         AddReward(0.0000001f);
    //         EndEpisode();
    //     }

    //     if (ball.transform.localPosition[1]<=0.6 || player.transform.localPosition[1]<0){
    //             // if (previousY<=40){
    //             //     AddReward(-((-xz_from_target+40)/(0-40))*0.2f);
    //             // }
    //             AddReward(-1f);
    //             // print(GetCumulativeReward());
    //             EndEpisode();
    //     }
    //     if (xz_from_target< previousY && xz_from_goal<15){
    //         previousY = xz_from_target;
    //     }
    // }
    // public void Level1FixedUpdates(){
    //     float xz_from_player = Mathf.Pow(player.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(player.transform.localPosition[2] - ball.transform.localPosition[2],2);
    //     float xyz_target_from_ball = Mathf.Pow(target.transform.localPosition[0] - ball.transform.localPosition[0],2)+ Mathf.Pow(target.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(target.transform.localPosition[2] - ball.transform.localPosition[2],2);
    //     // if (touched && ball.transform.localPosition[0]>=-5f && ball.transform.localPosition[0]<=5f && ball.transform.localPosition[1]>=0.6f && ball.transform.localPosition[2]>=-5f && ball.transform.localPosition[2]<=5f){
    //     //     AddReward(0.0000001f);
    //     // }
    //     if(xyz_target_from_ball<50){
    //         AddReward(-((-xyz_target_from_ball+50/(0-50)))* 0.0000001f);
    //     }
    //     if (ball.transform.localPosition[1]<=0.6 || player.transform.localPosition[1]<0){
    //             // if (xz_from_player>15){
    //             AddReward(-1f);
    //             // }else{
    //             //     AddReward(-((-xz_from_player+15)/(0-15))*0.2f);
    //             // }
    //             EndEpisode();
    //     }
    //     if (ball.transform.localPosition[1]> previousY){
    //         previousY = ball.transform.localPosition[1];
    //     }
    // }
    // public void Level2FixedUpdates(){//Agent will bounce ball towards fixed goal
    //     float xz_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
    //     float xz_from_player = Mathf.Pow(player.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(player.transform.localPosition[2] - ball.transform.localPosition[2],2);
    //     if(touched && xz_from_goal<40 && ball.transform.localPosition[1]>=2f && ball.transform.localPosition[1]<=5){
    //         AddReward(0.0000001f);
    //     //     // float y = (ball.transform.localPosition[1]-8f/2-8f);
    //     //     AddReward(1);
    //     //     EndEpisode();
    //     // }
    //     // if (ball.transform.localPosition[1]<= 0.6 || player.transform.localPosition[1]<0){
    //     //     if(xz_from_goal<30f && touched){
    //     //         float score = -((-xz_from_goal+30)/(0-30));
    //     //         AddReward(score*0.5f);
    //     //     }else{
    //     //         AddReward(-1f);
    //     //     }
    //     //     EndEpisode();
    //     // }
    //     // if (ball.transform.localPosition[1]> previousY){
    //     //     previousY = ball.transform.localPosition[1];
    //     // }
    //     // OutOfBoundFixedUpdate();
    //     // if (ball.transform.localPosition[0]>=-5f && ball.transform.localPosition[0]<=5f && ball.transform.localPosition[1]>=0.6f && ball.transform.localPosition[2]>=-5f && ball.transform.localPosition[2]<=5f){
    //     //     AddReward(0.0005f);
    //     // }
    //     // if (ball.transform.localPosition[1]<= 0.6 || player.transform.localPosition[1]<0){
    //     //     // if(xz_from_goal<30f && touched){
    //     //     //     float score = -((-xz_from_goal+30)/(0-30));
    //     //     //     AddReward(score*0.2f);
    //     //     // }else{
    //     //     print(GetCumulativeReward());
    //     //     AddReward(-1f);
    //     //     // }
    //     //     EndEpisode();
    //     }
    //     if (ball.transform.localPosition[1]<=0.6 || player.transform.localPosition[1]<0){
            
    //         if (xz_from_goal>80){
    //             AddReward(-1f);
    //         }else{
    //             AddReward(-((-xz_from_player+80)/(40-80))*0.2f);
    //             // print(GetCumulativeReward());

    //         }
    //         EndEpisode();
    //     }
    // }
    // public void Level3FixedUpdates(){//Agent will knock the ball towards random goal --working, 2.5mil~
    //     float xz_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
    //     if(xz_from_goal<40 && ball.transform.localPosition[1]>=2f && ball.transform.localPosition[1]<=5){
    //         AddReward(1);
    //         EndEpisode();
    //     }
    //     if (ball.transform.localPosition[1]<= 0.6 || player.transform.localPosition[1]<0){
    //         if(xz_from_goal<70f && touched){
    //             float score = -((-xz_from_goal+70)/(0-70));
    //             AddReward(score*0.5f);
    //         }else{
    //             AddReward(-1f);
    //         }
    //         EndEpisode();
    //     }
    //     if (ball.transform.localPosition[1]> previousY){
    //         previousY = ball.transform.localPosition[1];
    //     }
    //     // OutOfBoundFixedUpdate();
    // }
    // public void Level4FixedUpdates(){//Agent will be able to catch the ball after bouncing it from goal
    //     float xz_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
    //     // float xyz_from_target = Mathf.Pow(target.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(target.transform.localPosition[1] - ball.transform.localPosition[1],2)+ Mathf.Pow(target.transform.localPosition[2] - ball.transform.localPosition[2],2);        
    //     // if(xyz_from_target<27 &&pending_reward==5){//score for closeby, bonus if it collided
    //     //     // float score = -((-xyz_from_target+25)/(0-25));
    //     //     // pending_reward = (Mathf.Min(pending_reward, score*0.5f));//Only give reward if it manage to catch the ball on the other side
    //     //     // pending_reward +=0.5;
    //     //     SetReward(1f);
    //     // }
    //     if(xz_from_goal<40 && ball.transform.localPosition[1]<5f){//Standing close to the goal
    //         // score = -((-xy_from_goal+15)/(0-15));
    //         pending_reward = 0.2f;
    //         // AddReward(ScoredAGoal*0.5f);
    //         // float y = (ball.transform.localPosition[1]-12f/-10f);
    //         // float normalizedValue = ((xy_from_ball -25)/(-25)) * y;
    //         // reward +=normalizedValue;
    //         // AddReward(y);//normalize to max 1
    //         // EndEpisode();
    //     }
    //     if (ball.transform.localPosition[1]<= 0.6 || player.transform.localPosition[1]<0){//Bounce into random, fail 2nd bounce
    //         // if(pending_reward>0 && touched){
    //         //     if (xyz_from_target<27){
    //         //         AddReward(-((-xyz_from_target+27/(0-27))));
    //         //         EndEpisode();
    //         //     }
    //         // }
    //         // else 
    //         if (pending_reward>0 && touched){//Distance from ball
    //             float xz_from_player = Mathf.Pow(player.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(player.transform.localPosition[2] - ball.transform.localPosition[2],2);
    //             if (xz_from_player<40){
    //                 AddReward((pending_reward) * -((-xz_from_player+40)/(0-40)) * 0.2f);
    //             }
    //         }
    //         AddReward(-1f);
    //         EndEpisode();
    //     }
    //     if (ball.transform.localPosition[1]> previousY){
    //         previousY = ball.transform.localPosition[1];
    //     }
    // }
    // public void Level5FixedUpdates(){//duplicate of 4 but moves goal afterwards
    //     float xz_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
    //     // float xyz_from_target = Mathf.Pow(target.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(target.transform.localPosition[1] - ball.transform.localPosition[1],2)+ Mathf.Pow(target.transform.localPosition[2] - ball.transform.localPosition[2],2);        
    //     // if(xyz_from_target<27 &&pending_reward==5){//score for closeby, bonus if it collided
    //     //     // float score = -((-xyz_from_target+25)/(0-25));
    //     //     // pending_reward = (Mathf.Min(pending_reward, score*0.5f));//Only give reward if it manage to catch the ball on the other side
    //     //     // pending_reward +=0.5;
    //     //     SetReward(1f);
    //     // }
    //     if(xz_from_goal<40 && ball.transform.localPosition[1]<5f){//Standing close to the goal
    //         // score = -((-xy_from_goal+15)/(0-15));
    //         pending_reward = 0.2f;
    //         goal.transform.localPosition = new Vector3(Random.Range(-8f,8), -1.0f,Random.Range(-8f,8));
    //         // AddReward(ScoredAGoal*0.5f);
    //         // float y = (ball.transform.localPosition[1]-12f/-10f);
    //         // float normalizedValue = ((xy_from_ball -25)/(-25)) * y;
    //         // reward +=normalizedValue;
    //         // AddReward(y);//normalize to max 1
    //         // EndEpisode();
    //     }
    //     if (ball.transform.localPosition[1]<= 0.6 || player.transform.localPosition[1]<0){//Bounce into random, fail 2nd bounce
    //         // if(pending_reward>0 && touched){
    //         //     if (xyz_from_target<27){
    //         //         AddReward(-((-xyz_from_target+27/(0-27))));
    //         //         EndEpisode();
    //         //     }
    //         // }
    //         // else 
    //         if (pending_reward>0 && touched){//Distance from ball
    //             float xz_from_player = Mathf.Pow(player.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(player.transform.localPosition[2] - ball.transform.localPosition[2],2);
    //             if (xz_from_player<40){
    //                 AddReward((pending_reward) * -((-xz_from_player+40)/(0-40)) * 0.2f);
    //             }
    //         }
    //         AddReward(-1f);
    //         EndEpisode();
    //     }
    //     if (ball.transform.localPosition[1]> previousY){
    //         previousY = ball.transform.localPosition[1];
    //     }
    // }
    // public void Level5FixedUpdates(){//Agent will bounce ball to goal, goal relocate, bounce again.
    //     // float xy_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
    //     // if(xy_from_goal<10){
    //     //     float y = (ball.transform.localPosition[1]-12f/-10f);
    //     //     // float normalizedValue = ((xy_from_ball -25)/(-25)) * y;
    //     //     // reward +=normalizedValue;
    //     //     AddReward(y/10);//normalize to max 1
    //     //     goal.transform.localPosition = new Vector3(Random.Range(-5f,5), -1.0f,Random.Range(-5f,5));
    //     // }
    //     OutOfBoundFixedUpdate();
    // }
    // public void Level6FixedUpdates(){//Bounce to multiple goal then make one shot only
    //     float xyz_from_target = Mathf.Pow(target.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(target.transform.localPosition[1] - ball.transform.localPosition[1],2)+ Mathf.Pow(target.transform.localPosition[2] - ball.transform.localPosition[2],2);        
    //     if(xyz_from_target<25){//score for closeby, bonus if it collided
    //         float score = -((-xyz_from_target+25)/(0-25));
    //         pending_reward = (Mathf.Min(pending_reward, score*0.5f));//Only give reward if it manage to catch the ball on the other side
    //     }
    //     //     float y = (ball.transform.localPosition[1]-12f/-10f);
    //     //     // float normalizedValue = ((xy_from_ball -25)/(-25)) * y;
    //     //     // reward +=normalizedValue;
    //     //     AddReward(y/100);//normalize to max 1
    //     //     goal.transform.localPosition = new Vector3(Random.Range(-8f,8), -1.0f,Random.Range(-8f,8));
    //     // }

    //     OutOfBoundFixedUpdate();
    // }
    public void ScoredAGoal(){
        if (touched_on_goal){
            AddReward(0.3f);
            spawnGoalTarget();
            previousY = 41;
            shot = true;
            score_no+=1;
            if (score_no==2){
                // print("PERFECT RUN");
                // AddReward(no_bounce*0.05f);
                // SetReward(1f);
                // print(GetCumulativeReward());
                EndEpisode();
            }
            // target.transform.localPosition = new Vector3(Random.Range(-5f,5), Random.Range(3f,10f),Random.Range(-5f,5));
        }
    }

    void FixedUpdate(){  
        if(level<=10){//Touch ball
            RewardShapingFixedUpdates();
        }else if(level<=20){
            CurrLearningFixedUpdates();
        }
        // }else if(level==2){//Bounce ball towards goal
        //     Level2FixedUpdates();
        // }else if(level==3){//Catching ball after bouncing towards goal
        //     Level3FixedUpdates();
        // }else if(level==4){
        //     Level4FixedUpdates();
        // }else if(level==5){//Single shot
        //     Level5FixedUpdates();
        // }else{
        //     Level6FixedUpdates();
        // }
        // }else if(level==6){//Score as much as it can, target will move if hit
        //     Level6FixedUpdates();
        // }else if(level==7){
        //     Level7FixedUpdates();
        // }
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
    private void OnCollisionEnter(Collision collision){
        if (collision.rigidbody == ball){
            var dir = collision.contacts[0].point - transform.position;
            dir = dir.normalized;
            collision.gameObject.GetComponent<Rigidbody>().AddForce((dir * 0.1f));
            float xz_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
            bool cur_state = touched_on_goal;
            if (xz_from_goal<15){
                touched_on_goal = true;
            }else{
                touched_on_goal = false;
            }
            current+=1;
            if (shot){
                if(ball.transform.localPosition[1]>minimum_height){
                    if (touched_on_goal){
                        AddReward(0.1f);
                    }
                    AddReward(0.1f);
                }
            }else{
                if (cur_state==false && touched_on_goal==true && ball.transform.localPosition[1] >minimum_height){
                    AddReward(0.1f);
                }else if(cur_state==true && touched_on_goal==false){
                    AddReward(-0.1f);
                }
            }
            no_bounce+=1;
            shot = false;
            touched = true;
            previousY =41;
        }
        else if(collision.gameObject.name=="Ground"){//Agent touched ground
            on_ground = true;
        }
    }
            // if (level==2){
            //     if(shot){
            //         AddReward(0.7f);
            //         EndEpisode();
            //     }
            // }else if (level==3){
            //     // if(state==1){
            //     //     AddReward(0.1);
            //     // }
            // }

            // if (shot){
            //     AddReward(0.1f);
            //     state = 0;
            //     shot=false;
            // }
            // if (xz_from_goal<15){
            //     touched_on_goal = true;
            //     if (state==0){
            //         AddReward(0.1f);
            //         state = 1;
            //     }
            // }else{
            //     touched_on_goal = false;
            //     if (state==1){
            //         AddReward(0.1f);
            //         state = 0;
            //     }

            // }
            


            // if (level== 1){
            //     // AddReward(0.2f);
            //     // // ball.transform.localPosition = new Vector3(0, 6.0f, 0);
            //     // // ball.velocity = new Vector3(Random.Range(-1f,1),Random.Range(-1f,1),Random.Range(-1f,1));
            //     // // ball.angularVelocity = new Vector3(Random.Range(-1f,1),Random.Range(-1f,1),Random.Range(-1f,1));
            //     // if (GetCumulativeReward()>= 1){
            //     //     EndEpisode();
            //     // }
            // }else if(level==4){
            //     if (pending_reward>0){
            //         AddReward(pending_reward);
            //         // EndEpisode();
            //         pending_reward = 0;
            //     }
            //     if(GetCumulativeReward()>=1){
            //         EndEpisode();
            //     }

            // }else if(level==5 && touched==true){
            //     float xy_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
            //     if (xy_from_goal<=40){
            //         float normalised_reward = -((-xy_from_goal+40)/(0-40));
            //         AddReward(normalised_reward/5);//5 times
            //         goal.transform.localPosition = new Vector3(Random.Range(-8f,8), -1.0f,Random.Range(-8f,8));
            //     } 
            //     if (GetCumulativeReward() >=1){//1 means perfected the ability
            //         SetReward(1);
            //         EndEpisode();
            //     }
            // }else if(level==6 && touched==true){//Make a shot
            //     float xy_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
            //     if (xy_from_goal<=40){
            //         shot=true;
            //     }else{
            //         shot=false;
            //     }
            //     if (pending_reward>= 0.1 || GetCumulativeReward() >=0.1){//if it make a shot and catches the ball after, end epsoide
            //         AddReward(pending_reward);
            //         EndEpisode();
            //     }
            // }else if(level==7 && touched==true){
            //     float xy_from_goal = Mathf.Pow(goal.transform.localPosition[0] - ball.transform.localPosition[0],2) + Mathf.Pow(goal.transform.localPosition[2] - ball.transform.localPosition[2],2);
            //     if (xy_from_goal<=10){
            //         shot=true;
            //     }else{
            //         shot=false;
            //     }
            // }
            
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
