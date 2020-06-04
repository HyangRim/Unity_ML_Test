using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class TestProducer : Agent
{
    private Rigidbody ProducerRigidbody; //볼의 리지드바디
    public Transform pivotTransform; //위치의 기준점
    public Transform Prodator;//Prodator위치

    public float moveForce = 6f;

    private bool Eating = false;
    private float Energy = 0f;

    void Awake()
    {
       ProducerRigidbody = GetComponent<Rigidbody>();
    }

    public override void AgentReset()//새로 시작할시
    {
        Vector3 randompos = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
        transform.position = randompos + pivotTransform.position;
        Eating = false;
        //Energy = 0f;
        ProducerRigidbody.velocity = Vector3.zero;

        //Prodactor도 위치 재 설정 해줘야함. 
    }

    public override void CollectObservations()
    {
        Vector3 distanceToTarget = Prodator.position - transform.position; //타겟과의 거리.

        AddVectorObs(Mathf.Clamp(distanceToTarget.x / 5f, -1f, 1f)); //AddVectorObs는 정보 보내기.
        AddVectorObs(Mathf.Clamp(distanceToTarget.z / 5f, -1f, 1f)); //타겟과의 x,z정보 보내기, y는 필요 없음

        Vector3 relativePos = transform.position - pivotTransform.position;//상대좌표
        //세트가 6개 있을때, 절대좌표에서 모든 좌표계가 0,0,0에서 시작될 수는 없음. 그래서 상대좌표로 해야함

        AddVectorObs(Mathf.Clamp(relativePos.x / 5f, -1f, 1f));
        AddVectorObs(Mathf.Clamp(relativePos.z / 5f, -1f, 1f));

        AddVectorObs(Mathf.Clamp(ProducerRigidbody.velocity.x / 5, -1f, 1f));
        AddVectorObs(Mathf.Clamp(ProducerRigidbody.velocity.z / 5, -1f, 1f));
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(0.0005f);
        Energy += 0.0005f;
        float Horizontal = vectorAction[0];
        float Vertical = vectorAction[1];
        if (ProducerRigidbody.velocity.x < 9f || ProducerRigidbody.velocity.z < 9f)
        {
            ProducerRigidbody.AddForce(Horizontal * moveForce, 0f, Vertical * moveForce);
        }

        if (Eating)
        {
            //Energy -= 0.01f;
            AddReward(-1.0f);
            Done();
        }
        else if(Energy > 1f)
        { 
            AddReward(1.0f);
            Done();
        }/*
        else if(Energy == 1f)
        {
            AddReward(1.0f);
            Done();
        }*/
        Monitor.Log(name, GetCumulativeReward(), transform);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("dead"))
        {
            Eating = true;
            //Debug.Log("Eating");
        }
    }
}
