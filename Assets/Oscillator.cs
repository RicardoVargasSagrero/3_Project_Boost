using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent] //This allow only one component to the Object
public class Oscillator : MonoBehaviour
{
    //Vectors variables
    [SerializeField] Vector3 movementVector = new Vector3(10f,10f,10f);
    [SerializeField] float period = 2f;
    // todo remove from inspector later 
    /*The things that are in brakes [] modify the item that it's below them */
    [Range(0,1)] [SerializeField] float movementFactor;
    Vector3 startingPos;
   
    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Protect against period zero
        if(period <= Mathf.Epsilon) { return; }
        float cycle = Time.time/period;
        const float tau = Mathf.PI * 2; //about 6.28 
        float rawSinWave = Mathf.Sin(tau * cycle);
        /*The first factor dives the rawSinWave in half, so the new waves goes from -0.5 to 0.5, then 
         we added 0.5f thus they go now to 0 to 1*/
        movementFactor = rawSinWave / 2f + 0.5f; 
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
