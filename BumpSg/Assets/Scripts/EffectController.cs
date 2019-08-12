using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    [SerializeField] GameObject effectPrefab;
    [SerializeField] float lifeTime;
    [SerializeField] bool isOneShot;
    [SerializeField] float timeLeft;
    void Awake()
    {
        timeLeft = lifeTime;
    }
    public void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.unscaledDeltaTime;
            return;
        }

        Destroy(this.gameObject);
    }
}
