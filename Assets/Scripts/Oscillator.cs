using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour {

    [SerializeField] Vector3 mouvementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 2f;

    Vector3 startingPos;
    float mouvementFactor; // 0 for not moved, 1 for fully moved.


    // Use this for initialization
    void Start () {
        startingPos = transform.position;

    }
	
	// Update is called once per frame
	void Update () {
        if (period <= Mathf.Epsilon) { return; }

        float cycles = Time.time / period;
        const float tau = Mathf.PI * 2;
        float rawSinWave = Mathf.Sin(cycles * tau);

        mouvementFactor = rawSinWave / 2f + 0.5f;
        Vector3 offset = mouvementVector * mouvementFactor;
        transform.position = startingPos + offset;
	}
}
