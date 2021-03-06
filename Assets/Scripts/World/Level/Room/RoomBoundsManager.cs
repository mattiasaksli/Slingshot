﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RoomBoundsSize))]
public class RoomBoundsManager : MonoBehaviour
{
    public BoxCollider2D RoomCollider;
    private Transform player;
    [HideInInspector]
    public BoxCollider2D collider;
    private Transform content;
    private RoomManager manager;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        collider = gameObject.GetComponentInChildren<BoxCollider2D>();
        content = transform.parent.Find("Content");
        manager = transform.parent.GetComponent<RoomManager>();
        SetRoomActive(false);
    }

    private void Awake()
    {
        LevelEvents.OnRoomChange += OnRoomChange;
    }

    private void OnDestroy()
    {
        LevelEvents.OnRoomChange -= OnRoomChange;
    }

    private void OnRoomChange(RoomBoundsManager newroom)
    {
        if(newroom != this)
        {
            if (RoomCollider.gameObject.activeSelf)
            {
                SetRoomActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Bounds bounds = collider.bounds;
        var padding = 1;
        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller.state != controller.states[3])
        {
            if (!RoomCollider.gameObject.activeSelf)
            {
                padding = 0;
            }
                if (player.position.x < bounds.min.x- padding || player.position.y < bounds.min.y- padding || player.position.x > bounds.max.x+ padding || player.position.y > bounds.max.y+ padding)
            {
                if (RoomCollider.gameObject.activeSelf)
                {
                    SetRoomActive(false);
                }
            }
            else
            {
                if (!RoomCollider.gameObject.activeSelf)
                {
                    SetRoomActive(true);
                    LevelEvents.ChangeRoom(this);
                }
            }
        }

    }

    public SpawnPoint GetClosestSpawnPoint()
    {
        SpawnPoint[] spawnPoints = RoomCollider.gameObject.GetComponentsInChildren<SpawnPoint>();
        SpawnPoint closest = null;
        if (spawnPoints.Length > 0)
        {
            closest = spawnPoints[0];
            float c = Vector3.Distance(closest.transform.position, player.position);
            foreach (SpawnPoint spawn in spawnPoints)
            {
                float s = Vector3.Distance(spawn.transform.position, player.position);
                if (s < c)
                {
                    closest = spawn;
                    c = s;
                }
            }
        }
        return closest;
    }

    private void SetRoomActive(bool active)
    {
        RoomCollider.gameObject.SetActive(active);
        if (active)
        {
            manager.StartRoom();
        } else
        {
            manager.EndRoom();
        }
        //content.gameObject.SetActive(active);
    } 

    private void OnDrawGizmos()
    {
        Bounds bounds = gameObject.GetComponent<BoxCollider2D>().bounds;
        Gizmos.DrawLine(new Vector3(bounds.min.x, bounds.min.y, 0), new Vector3(bounds.max.x, bounds.min.y, 0));
        Gizmos.DrawLine(new Vector3(bounds.max.x, bounds.min.y, 0), new Vector3(bounds.max.x, bounds.max.y, 0));
        Gizmos.DrawLine(new Vector3(bounds.max.x, bounds.max.y, 0), new Vector3(bounds.min.x, bounds.max.y, 0));
        Gizmos.DrawLine(new Vector3(bounds.min.x, bounds.max.y, 0), new Vector3(bounds.min.x, bounds.min.y, 0));
    }
}
