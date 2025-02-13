using UnityEngine;
using System.Collections.Generic;

public class Basic_State_Machine : MonoBehaviour
{
    public GameObject playerMain;

    [SerializeField] private float distanceToPlayer;
    [SerializeField] private bool playerVisible;
    [SerializeField] private float followDistance;
    [SerializeField] private float danceDistance;
    [SerializeField] private float followHysterisis;
    [SerializeField] private float danceHysterisis;

    Animator basicMachine;
    [SerializeField] private Animator AIState_Patrol;
    [SerializeField] private Animator AIState_Follow;
    [SerializeField] private Animator AIState_Dance;

    private static Dictionary<int, string> animStateHashToName = new Dictionary<int, string>();
    private const string FSM_PlayerSpotted = "PlayerSpotted";
    private const string FSM_Tired = "AI_Tired";
    private const string FSM_DanceInRange = "PlayerInDanceRange";

    void Start()
    {
        // set initial player distance to infinity
        distanceToPlayer = Mathf.Infinity;
        basicMachine = GetComponent<Animator>();
    }

    void Update()
    {
        if (basicMachine.GetCurrentAnimatorStateInfo(0).IsName("Patrol"))
        {
            Debug.Log("Patrol State");
            AIState_Patrol.enabled = true;
            AIState_Follow.enabled = false;
            AIState_Dance.enabled = false;
        }
        else if (basicMachine.GetCurrentAnimatorStateInfo(0).IsName("Follow"))
        {
            Debug.Log("Chase State");
            AIState_Patrol.enabled = false;
            AIState_Follow.enabled = true;
            AIState_Dance.enabled = false;
        }
        else if (basicMachine.GetCurrentAnimatorStateInfo(0).IsName("Dance"))
        {
            Debug.Log("Attack State");
            AIState_Patrol.enabled = false;
            AIState_Follow.enabled = false;
            AIState_Dance.enabled = true;
        }
    }
    void FixedUpdate()
    {
        TopLevelFSMProcessing(); // process the top level FSM - every 1/60th of a second is fast enough
    }
    float CheckPlayerDistance()
    {
        // calculate the distance to the player
        distanceToPlayer = Vector3.Distance(playerMain.transform.position, transform.position);
        return distanceToPlayer;
    }
    void TopLevelFSMProcessing()
    {
        Debug.Log("Player Visible = " + playerVisible);
        if (playerVisible)
        {
            // note that hysterisis is used to prevent the FSM from flickering between states
            if ((CheckPlayerDistance() < followDistance - followHysterisis)) // if the player is within the chase distance, chase the player
            {
                basicMachine.SetBool(FSM_PlayerSpotted, true); // if the player is within the chase distance, chase the player 
                if (CheckPlayerDistance() < danceDistance - danceHysterisis) basicMachine.SetBool(FSM_DanceInRange, true); // if the player is within the attack distance, attack the player
                else if (CheckPlayerDistance() < danceDistance + danceHysterisis) basicMachine.SetBool(FSM_DanceInRange, false);
            }
            else if (CheckPlayerDistance() > followDistance + followHysterisis) // if the player is outside the chase distance, stop chasing the player
            {
                basicMachine.SetBool(FSM_PlayerSpotted, false);
                basicMachine.SetBool(FSM_DanceInRange, false);
            }
        }
        else
        {
            basicMachine.SetBool(FSM_PlayerSpotted, false);
            basicMachine.SetBool(FSM_DanceInRange, false);
        }
    }

    void OnDrawGizmos()
    {
        // draw the chase distance sphere
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, followDistance);
        // draw the attack distance sphere
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, danceDistance);
    }
}
