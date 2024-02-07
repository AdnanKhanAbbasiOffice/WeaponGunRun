using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwerveMovement : MonoBehaviour
{
    private SwerveInputSystem _swerveInputSystem;
    [SerializeField] private float swerveSpeed = 0.5f;
    [SerializeField] private float maxSwerveAmount = 1f;
    [SerializeField] float MinX = -3f;
    [SerializeField] float MaxX = 3.5f;

    private void Awake()
    {
        _swerveInputSystem = GetComponent<SwerveInputSystem>();
    }

    private void FixedUpdate()
    {
        float swerveAmount = Time.deltaTime * swerveSpeed * _swerveInputSystem.MoveFactorX;
        swerveAmount = Mathf.Clamp(swerveAmount, -maxSwerveAmount, maxSwerveAmount);
        transform.Translate(swerveAmount, 0, 0);

        var pos = transform.position;

        pos.x = Mathf.Clamp(transform.position.x, MinX, MaxX);
        transform.position = pos;
    }
}
