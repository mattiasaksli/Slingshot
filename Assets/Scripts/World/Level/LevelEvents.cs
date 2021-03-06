﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System;

public static class LevelEvents
{
    public static event Action<RoomBoundsManager> OnRoomChange;
    public static event Action OnPlayerRespawn;
    public static void ChangeRoom(RoomBoundsManager roomCollider) => OnRoomChange?.Invoke(roomCollider);
    public static void RespawnPlayer() => OnPlayerRespawn?.Invoke();
}
