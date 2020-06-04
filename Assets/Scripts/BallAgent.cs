using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class BallAgent : Agent
{
    private Rigidbody ballRigidbody; //볼의 리지드바디
    public Transform pivotTrasform; // 위치의 기준점
    public Transform target;//목표

    public float moveForce = 10f;//이동 힘

    private bool targetEaten = false;//목표를 먹었는지
    private bool dead = false; //사망상태

    void Awake()
    {
        ballRigidbody = GetComponent<Rigidbody>(); //처음 시작됬을 경우 RigidBody에 대한 정보를 불러옴. 
    }

    void ResetTarget()
    {
        targetEaten = false; //Target을 먹거나, 떨어졌을경우 타겟 새로 재 설정
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));//위치 재설정
        target.position = randomPos + pivotTrasform.position; //상대좌표 + 랜덤좌표로 타겟 위치 변경
    }

    public override void AgentReset() //떨어질시. 
    {
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f)); //위와 같은 랜덤
        transform.position = randomPos + pivotTrasform.position; // 이것도 위와같이 랜덤

        dead = false;//죽은거 false로 만들어주고
        ballRigidbody.velocity = Vector3.zero; //움직이던 힘(Rigidbody) 초기화.

        ResetTarget();//Target도 초기화 해주기
    }

    public override void CollectObservations()//에이전트가 주변관측
    {
        Vector3 distanceToTarget = target.position - transform.position; //타겟과의 거리.

        AddVectorObs(Mathf.Clamp(distanceToTarget.x / 5,-1f,1f)); //AddVectorObs는 정보 보내기.
        AddVectorObs(Mathf.Clamp(distanceToTarget.z / 5,-1f,1f)); //타겟과의 x,z정보 보내기, y는 필요 없음

        Vector3 relativePos = transform.position - pivotTrasform.position;//상대좌표
        //세트가 6개 있을때, 절대좌표에서 모든 좌표계가 0,0,0에서 시작될 수는 없음. 그래서 상대좌표로 해야함

        AddVectorObs(Mathf.Clamp(relativePos.x / 5,-1f,1f));
        AddVectorObs(Mathf.Clamp(relativePos.z / 5,-1f,1f));

        AddVectorObs(Mathf.Clamp(ballRigidbody.velocity.x / 10,-1f,1f));
        AddVectorObs(Mathf.Clamp(ballRigidbody.velocity.z / 10,-1f,1f));
    }

    public override void AgentAction(float[] vectorAction, string textAction) //Agent의 액션하는것. 
    {
        AddReward(-0.001f);//Tick(프레임)마다 -0.001f씩 리워드를 줌. 시간끌면 안됨

        //vectorAction은 AI(Tensorflow)에서 명령을 내리는 방법의 모음들.
        float HorizontalInput = vectorAction[0];//가로로 임력을 줌 -1~1
        float verticalInput = vectorAction[1];//세로로 입력을 줌 -1~1

        ballRigidbody.AddForce(HorizontalInput * moveForce, 0f, verticalInput * moveForce);//AI의 input을 토대로 AddForce함. 

        if (targetEaten) //Target먹을경우
        {
            AddReward(1.0f);//보상 1.0주기
            ResetTarget();//타겟 위치 초기화
        }
        else if (dead)
        {
            AddReward(-1.0f);//죽으면 -1.0f 벌점주기
            Done();//결과랑 보상등을 텐서플로로 보내고 동작멈추기. 그리고 처음부터 리셋.
        }
        Monitor.Log(name, GetCumulativeReward(), transform);//Monitor에 화면 띄우기
    }
    private void OnTriggerEnter(Collider other) //트리거.
    {
        if (other.CompareTag("dead"))//Tag가 dead인 것에 닿으면 dead상태가 됨.
        {
            dead = true;
        }
        else if (other.CompareTag("goal"))//Tag가 goal(Target)에 닿으면 Target을 먹은 것처리.
        {
            targetEaten = true;
        }
    }
}
