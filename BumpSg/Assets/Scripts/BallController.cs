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
    public Vector3 holePosition;
    public bool remoteRequestingAddForce;
    public Vector3 remoteAddForceDir;
    public Vector3 remoteAddForcePos;
    public Vector3 remoteAddForceVelocity;
    public Action<BallController> OnBallFallingInToHole;
    public float gavityAliveTime = 2f;
    public float gavityAliveTimeMax = 2f;
    void Awake()
    {
        if (ballRigidbody == null)
        {
            ballRigidbody = GetComponent<Rigidbody>();
        }
    }
    void OnEnable()
    {
        gavityAliveTime = gavityAliveTimeMax;
    }
    public void ResetBall()
    {
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        IsFalling = false;
        IsBallDead = false;
        holePosition = Vector3.zero;
        ballRigidbody.velocity = Vector3.zero;
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
            if (gavityAliveTime > 0)
            {
                gavityAliveTime -= Time.deltaTime;
                ballRigidbody.useGravity = true;
                if (gavityAliveTime <= 0)
                {
                    gavityAliveTime = 0;
                    ballRigidbody.useGravity = false;
                }
            }

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

        if (!point.IsHostHole)
        {
            SocketClientBase.GetInstance().C2A_GameResult(SocketClientBase.GetInstance().HostClientObjectID.Value);
        }
        else
        {
            if (SocketClientBase.GetInstance().GuestClientObjectID.HasValue)
            {
                SocketClientBase.GetInstance().C2A_GameResult(SocketClientBase.GetInstance().GuestClientObjectID.Value);
            }
            else
            {
                SocketClientBase.GetInstance().C2A_GameResult(1);
            }
        }
    }
}
