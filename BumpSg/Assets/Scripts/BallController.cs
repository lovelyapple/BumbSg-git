using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] Rigidbody ballRigidbody;
    [SerializeField] float powerUpHeadTime;
    [SerializeField] float powerUpForcePower;

    [SerializeField] float endPointForce = 1.0f;
    Coroutine PowerUpCoroutine;
    public bool IsFalling;
    public bool IsBallDead;
    public float HoleFallInTime = 2f;
    public float HoleFallInTimeLeft;
    Vector3 holePosition;
    public bool remoteRequestingAddForce;
    public Vector3 remoteAddForceDir;
    public Vector3 remoteAddForcePos;
    public Vector3 remoteAddForceVelocity;
    public Action<BallController> OnBallFallingInToHole;
    void Awake()
    {
        if (ballRigidbody == null)
        {
            ballRigidbody = GetComponent<Rigidbody>();
        }
    }
    public void Update()
    {
        if (IsBallDead)
        {
            transform.position = holePosition;
            return;
        }

        if (IsFalling)
        {
            var diff = (holePosition - transform.position).normalized;
            diff.z = 0;
            ballRigidbody.AddForce(diff * endPointForce, ForceMode.Impulse);

            if (HoleFallInTimeLeft > 0)
            {
                HoleFallInTimeLeft -= Time.deltaTime;

                if (HoleFallInTimeLeft <= 0)
                {
                    IsBallDead = true;
                    HoleFallInTimeLeft = 0;

                    if (OnBallFallingInToHole != null)
                    {
                        OnBallFallingInToHole.Invoke(this);
                    }
                }
            }
        }
        else
        {
            if (remoteRequestingAddForce)
            {
                transform.position = remoteAddForcePos;
                ballRigidbody.velocity = remoteAddForceVelocity;
                ballRigidbody.AddForce(remoteAddForceDir, ForceMode.Impulse);
                remoteRequestingAddForce = false;
            }
        }
    }
    public void PowerUp(float powerUpRate)
    {
        powerUpForcePower = powerUpRate;
        if (PowerUpCoroutine != null)
        {
            StopCoroutine(PowerUpCoroutine);
            PowerUpCoroutine = null;
        }

        PowerUpCoroutine = StartCoroutine(PowerUpEnumerator());
    }
    IEnumerator PowerUpEnumerator()
    {
        var headTime = powerUpHeadTime;
        var beforePos = gameObject.transform.position;

        while (headTime > 0)
        {
            headTime -= Time.unscaledDeltaTime;
            yield return null;
        }

        var currentPos = gameObject.transform.position;
        var dir = currentPos - beforePos;
        dir.z = 0;
        dir *= powerUpForcePower;

        SocketClientBase.GetInstance().C2A_AddForceToBall(SocketClientBase.GetInstance().SelfClientObjectID.Value, dir, currentPos, ballRigidbody.velocity);
        ballRigidbody.AddForce(dir, ForceMode.Impulse);
    }
    public void SetupRemoteAddForece(Vector3 dir, Vector3 pos, Vector3 velocity)
    {
        remoteAddForceDir = dir;
        remoteRequestingAddForce = true;
        remoteAddForcePos = pos;
        remoteAddForceVelocity = velocity;
    }
    public void FallIntoHole(HolePoint point)
    {
        if (IsFalling || IsBallDead)
        {
            return;
        }

        holePosition = point.gameObject.transform.position;
        IsFalling = true;
        HoleFallInTimeLeft = HoleFallInTime;
    }
}
