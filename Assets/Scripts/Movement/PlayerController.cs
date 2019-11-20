﻿using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class PlayerController : MonoBehaviour
{
    public State state;
    public float MovementSpeed = 4;
    public float JumpPower = 10;
    public float AccelerationGround = 40;
    public float AccelerationAir = 20;
    public float GravityPower = 40;
    [Space(10)]
    public float WalljumpHorizontalPower = 8;
    public float WalljumpVerticalPower = 5;
    public float WalljumpHoldTime = 0.5f;
    public float WalljumpHoldCounter = 0;
    public float WalljumpMinHoldTime = 0.01f;
    public float WalljumpUnHugTime = 0.1f;
    [Space(10)]
    public float ThrowPower = 20;
    public KinematicBody OrbPrefab;
    [Space(10)]
    public float SlingShotStartSpeed = 10;
    public float SlingShotAcceleration = 60;
    public float SlingShotMaxSpeed = 50;
    [Space(10)]
    public bool IsGrounded = false;
    public bool IsJumping = false;
    public bool IsSlingshotting = false;
    public bool IsHuggingRight = false;
    public bool IsWallJumping = false;
    public bool IsFacingRight = true;
    [Space(10)]
    public List<State> states;
    public KinematicBody body { get; private set; }
    public float DeathTime;

    public bool IsOrbAvailable = true;
    public KinematicBody orb = null;
    public SpriteRenderer Sprite;

    private bool createOrb = false;
    private bool isInputLocked = false;

    // Start is called before the first frame update
    void Start()
    {
        states = new List<State>() { new StatePlayerMove(), new StatePlayerSlingshot(), new StatePlayerWallHug(), new StatePlayerDead() };
        state = states[0];
        body = gameObject.GetComponent<KinematicBody>();
        Sprite = GetComponentInChildren<SpriteRenderer>();
    }


    // Update is called once per frame
    void Update()
    {
        if (!isInputLocked)
        {
            state.Update(this);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Defeat();
        }
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            IsFacingRight = body.TargetMovement.x > 0;
        }
        Sprite.transform.localScale = new Vector3(IsFacingRight ? 1 : -1, 1, 1);
    }

    private void FixedUpdate()
    {
        if (!isInputLocked)
        {
            state.FixedUpdate(this);
        }
    }

    public void CreateOrb()
    {
        if (createOrb)
        {
            if (orb != null)
            {
                GameObject.Destroy(orb.gameObject);
            }
            orb = GameObject.Instantiate<KinematicBody>(OrbPrefab);
            orb.transform.position = transform.position;
            orb.Movement = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized * ThrowPower;
            createOrb = false;
        }
    }
    public void RecallOrb()
    {
        if (orb)
        {
            GameObject.Destroy(orb.gameObject);
            IsOrbAvailable = false;
            orb = null;
        }
    }

    public void Slingshot()
    {
        state = states[1];
        body.detection.collisions.Reset();
        body.Movement = SlingShotStartSpeed * new Vector2(orb.transform.position.x - transform.position.x, orb.transform.position.y - transform.position.y).normalized;
    }

    public void OrbBehaviour()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (orb != null)
            {
                RecallOrb();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (orb == null)
            {
                if (IsOrbAvailable) { createOrb = true; }
            }
            else
            {
                Slingshot();
            }
            if (IsSlingshotting)
            {
                var collisions = body.detection.collisions;
                if (collisions.above || collisions.below || collisions.right || collisions.left)
                {
                    IsSlingshotting = false;
                }
                body.Movement = new Vector2(orb.transform.position.x - transform.position.x, orb.transform.position.y - transform.position.y).normalized * 15;
                body.TargetMovement = body.Movement;
                if ((orb.transform.position - transform.position).magnitude < 0.4)
                {
                    IsSlingshotting = false;
                    RecallOrb();
                }
            }
        }
    }

    public void disablePlayer()
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
        isInputLocked = true;
    }

    public void enablePlayer()
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
        isInputLocked = false;
    }

    public void Defeat()
    {
        if (state != states[3])
        {
            state = states[3];
            DeathTime = Time.time + 0.7f;
            body.Movement.y = 10;
            body.TargetMovement.y = body.Movement.y;
        }
    }

    public bool IsInputLocked { get => isInputLocked; set => isInputLocked = value; }
}
