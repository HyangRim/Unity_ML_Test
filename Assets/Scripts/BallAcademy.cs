using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class BallAcademy : Academy
{

    public override void AcademyReset()
    {
        Monitor.SetActive(true); //모니터에 보이게하기.
    }
}
