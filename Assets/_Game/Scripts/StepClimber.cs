using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
//Also requires some sort of collider, made with an AABB in mind

/// The class that takes care of all the player related physics
/// Many configurable parameters with defaults set as the recommended values in BBR
public class StepClimber : MonoBehaviour
{
    [Header("Player Step Clinmb:")]
    [SerializeField] GameObject stepRayUpper;
    [SerializeField] GameObject stepRayLower;
    [SerializeField] float stepHeight = 0.3f;
    [SerializeField] float stepSmooth = 2f;
    private void Awake()
    {
        stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
    }
    void FixedUpdate()
    {
        stepClimb();
    }
    void stepClimb()
    {
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f, LayerMask.GetMask(Constant.LAYER_GROUND)))
        {

            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f, LayerMask.GetMask(Constant.LAYER_GROUND)))
            {
                GetComponent<Rigidbody>().position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
                Debug.DrawLine(transform.position, transform.position + Vector3.forward * 1.1f, Color.red);
                Debug.Log("hitUpper");
            }
        }

        RaycastHit hitLower45;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitLower45, 0.1f, LayerMask.GetMask(Constant.LAYER_GROUND)))
        {

            RaycastHit hitUpper45;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitUpper45, 0.2f, LayerMask.GetMask(Constant.LAYER_GROUND)))
            {
                GetComponent<Rigidbody>().position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
                Debug.DrawLine(transform.position, transform.position + new Vector3(1.5f, 0, 1) * 1.1f, Color.red);
                Debug.Log("hitUpper45");
            }
        }

        RaycastHit hitLowerMinus45;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitLowerMinus45, 0.1f))
        {

            RaycastHit hitUpperMinus45;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitUpperMinus45, 0.2f))
            {
                GetComponent<Rigidbody>().position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
                Debug.DrawLine(transform.position, transform.position + new Vector3(-1.5f, 0, 1) * 1.1f, Color.red);
                Debug.Log("hitUpperMinus45");
            }
        }
    }
   
}