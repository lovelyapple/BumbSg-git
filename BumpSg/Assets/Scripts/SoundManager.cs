using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static SoundManager _instance;
    public static SoundManager GetInstance()
    {
        if (_instance == null)
        {
            Debug.LogError("FieldManager is null");
            SetupInstace();

            return _instance;
        }

        return _instance;
    }
    public static void SetupInstace()
    {
        var fieldRoot = GameObject.Find("SoundRoot");

        if (fieldRoot == null)
        {
            return;
        }

        _instance = fieldRoot.GetComponent<SoundManager>();
    }

    [SerializeField] GameObject titleBGMRoot;
    [SerializeField] GameObject gameBGMRoot;
    [SerializeField] GameObject endBGMRoot;
    public void OnTitle()
    {
        titleBGMRoot.SetActive(true);
        gameBGMRoot.SetActive(false);
        endBGMRoot.SetActive(false);
    }
    public void OnGame()
    {
        titleBGMRoot.SetActive(false);
        gameBGMRoot.SetActive(true);
        endBGMRoot.SetActive(false);
    }
    public void OnEnd()
    {
        titleBGMRoot.SetActive(false);
        gameBGMRoot.SetActive(false);
        endBGMRoot.SetActive(true);
    }

    [SerializeField] AudioSource ball_line_hit;
    [SerializeField] AudioClip ball_line_hit_clip;
    public void PlayerOneShotBallLineHit()
    {
        ball_line_hit.PlayOneShot(ball_line_hit_clip, 1);
    }
}
