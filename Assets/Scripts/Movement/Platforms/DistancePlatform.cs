﻿using UnityEngine;
using System.Collections;


public class DistancePlatform : PlatformController
{
    public float MaxSpeed;
    public float Speed;
    public float Acceleration;
    public float Rest;
    public float MaxDistance;

    private Vector3 waypointDist;
    private float lastDirection = 0;
    private float RestTimer;

    private Animator gemAnimator;

    public override void Start()
    {
        base.Start();
        waypointDist = (globalWaypoints[1] - globalWaypoints[0]);
        gemAnimator = GetComponentInChildren<Animator>();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        float minMovementAudioThreshold = 0.01f;
        if (Movement.magnitude > minMovementAudioThreshold && !audioSource.loop)
        {
            audioSource.loop = true;
            audioSource.volume = 0.5f * Volume;
            audioSource.clip = AudioMove.AudioClips[Random.Range(0, AudioMove.AudioClips.Count)];
            audioSource.Play();
        }
        if (Movement.magnitude < minMovementAudioThreshold && audioSource.loop)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.volume = 0.5f * Volume * StopVolume;
            audioSource.clip = AudioStop.AudioClips[Random.Range(0, AudioStop.AudioClips.Count)];
            audioSource.Play();
        }
        if (roomActive)
        {
            Movement = CalculatePlatformMovement();

            Move(Movement);
        }
    }

    Vector3 CalculatePlatformMovement()
    {
        float targetPercent = DistanceBetweenOrbAndPlayer();
        float currentPercent = (transform.position - globalWaypoints[0]).magnitude / waypointDist.magnitude;
        float direction = targetPercent - currentPercent;
        if(Mathf.Abs(direction) == 0)
        {
            Speed = 0;
        } else
        if(Mathf.Sign(direction) != Mathf.Sign(lastDirection))
        {
            Speed = 0;
            RestTimer = Time.time + Rest;
        } else
        {
            if(Time.time > RestTimer)
            {
                var speedDist = MaxSpeed - Speed;
                Speed += Mathf.Min(speedDist, Acceleration * Time.fixedDeltaTime);
            }
        }
        Vector3 target = Vector3.Lerp(globalWaypoints[0], globalWaypoints[1], targetPercent)-transform.position;
        Vector3 add = Mathf.Min(Speed * Time.fixedDeltaTime, target.magnitude) * target.normalized;

        lastDirection = direction;

        return add;
    }

    float DistanceBetweenOrbAndPlayer()
    {
        PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();
        if(player.orb != null)
        {
            gemAnimator.SetBool("IsActive", true);
            return Mathf.Clamp((player.transform.position-player.orb.transform.position).magnitude/MaxDistance,0,1);
        }
        gemAnimator.SetBool("IsActive", false);
        return 0;
    }

    public override void OnPlayerRespawn()
    {
        base.OnPlayerRespawn();
        RestTimer = Time.time;
    }

    public override Vector3 GetStoredMovement()
    {
        return Movement.normalized * Speed;
    }
}
