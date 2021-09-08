using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolePoint : MonoBehaviour
{
    public float endPointInRange = 1.0f;

    [SerializeField] BallController ball;
    [SerializeField] Collider collider;
    public bool IsHostHole;
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
    void OnTriggerEnter(Collider col)
    {
        Debug.Log("collision " + col.transform.name);
        if (ball != null && ball.transform == col.transform)
        {
            ball.FallIntoHole(this);
        }
    }
}
