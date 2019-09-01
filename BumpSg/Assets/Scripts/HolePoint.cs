using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolePoint : MonoBehaviour
{
    public float endPointInRange = 1.0f;

    [SerializeField] BallController ball;
    void Update()
    {
        if (ball == null)
        {
            ball = FieldManager.GetInstance().Ball;
            return;
        }

        var diff = ball.gameObject.transform.position - transform.position;

        if (diff.sqrMagnitude <= endPointInRange * endPointInRange)
        {
            ball.FallIntoHole(this);
        }
    }
}
