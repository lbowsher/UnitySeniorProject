﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);

    [SerializeField] float period = 2f;

    Vector3 startingPosition;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon) { return; }
        float cycles = Time.time / period;
        float rawSinOutput = Mathf.Sin(Mathf.PI * 2 * cycles);
        float movementFactor = rawSinOutput / 2f + 0.5f;
        Vector3 offset = movementVector * movementFactor;
        transform.position = offset + startingPosition;
    }
}
