using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using jcsilva.AISystem;

public class LastKnownLocation : AIBehaviour {

    // Behaviour Settings
    private bool isActive;
    private bool hasWaypoints;

    // Patrol Logic
    private NavMeshAgent agent;
    private Transform selfTransform;
    private Transform target;
    private List<Transform> waypoints;
    private int currentWayPoint = -1;

    // Vision Settings
    private float maxDistance;
    private float maxFieldOfView;

    public LastKnownLocation(MonoBehaviour self, AIStateMachine aIStateMachine) : base (self, aIStateMachine, "Last Known Location") {
        this.agent = stateMachine.GetNavMeshAgent();
        this.selfTransform = stateMachine.GetSelfPosition();
        this.target = stateMachine.GetTargetPosition();
        this.maxDistance = stateMachine.GetMaxDistanceToView();
        this.maxFieldOfView = stateMachine.GetMaxFieldOfView();
    }

    public override void OnBehaviourStart() {
        isActive = true;
        agent.SetDestination(target.position);
        stateMachine.EventAIEnableLastKnownLocation?.Invoke();
    }

    public override void OnBehaviourEnd() {
        isActive = false;
        agent.ResetPath();
        stateMachine.EventAIDisableLastKnownLocation?.Invoke();
    }


    public override void OnUpdate() {
        if (isActive) {

            // Check if the has Vision of the enemy
            if (AIUtils.HasVisionOfTarget(selfTransform, target, maxDistance, maxFieldOfView)) {
                stateMachine.HandleState(AIEvents.SeePlayer);
                return;
            } else {
                if(!agent.pathPending && agent.remainingDistance < 0.1f) {
                    stateMachine.HandleState(AIEvents.ReachedLastKnownPosition);
                    return;
                }
            }

        }
    }
}
