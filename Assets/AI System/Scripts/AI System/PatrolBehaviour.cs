using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace jcsilva.AISystem {

    public class PatrolBehaviour : AIBehaviour {

        // Behaviour Settings
        private bool isActive;
        private bool hasWaypoints;
        private Animator animator;

        // Patrol Logic
        private NavMeshAgent agent;
        private Transform selfTransform;
        private Transform target;
        private List<Transform> waypoints;
        private int currentWayPoint = -1;

        // Vision Settings
        private float maxDistance;
        private float maxFieldOfView;


        public PatrolBehaviour(MonoBehaviour self, AIStateMachine aIStateMachine) : base(self, aIStateMachine, "Patrol") {
            this.agent = stateMachine.GetNavMeshAgent();
            this.selfTransform = stateMachine.GetSelfPosition();
            this.target = stateMachine.GetTargetPosition();
            this.waypoints = stateMachine.GetWaypoints();
            if (waypoints.Count > 0) {
                hasWaypoints = true;
            }
            this.maxDistance = stateMachine.GetMaxDistanceToView();
            this.maxFieldOfView = stateMachine.GetMaxFieldOfView();
            this.animator = stateMachine.GetAnimator();
        }

        public override void OnBehaviourEnd() {
            isActive = false;
            stateMachine.EventAIDisablePatrol?.Invoke();

            // All things that need to be set when this behaviour ends should be put bellow this comment
            // example: Animator.SetBool("AnimationWalk", false);
            animator.SetBool("walk", false);
        }

        public override void OnBehaviourStart() {
            if (hasWaypoints) {
                isActive = true;
                Vector3 nextWayPoint = GetNextWaypoint();
                agent.SetDestination(nextWayPoint);
                stateMachine.EventAIEnablePatrol?.Invoke();

                // All things that need to be set when this behaviour start should be put bellow this comment
                // example: Animator.SetBool("AnimationWalk", true);
                animator.SetBool("walk", true);
            } else {
                stateMachine.HandleState(AIEvents.ReachedDestination);
            }
        }

        public override void OnUpdate() {

            if (isActive) {
                if (AIUtils.HasVisionOfTarget(selfTransform, target, maxDistance, maxFieldOfView)) {
                    stateMachine.HandleState(AIEvents.SeePlayer);
                    Debug.Log("I see the player");
                    return;
                } else {
                    if (!agent.pathPending && agent.remainingDistance < 0.1f) {
                        stateMachine.HandleState(AIEvents.ReachedDestination);
                        return;
                    }
                }
            }

        }

        /// <summary>
        /// Method that returns the next waypoint the enemy should go.
        /// </summary>
        /// <returns>The waypoint position in the world</returns>
        private Vector3 GetNextWaypoint() {
            if (waypoints != null && waypoints.Count > 0) {
                currentWayPoint = (currentWayPoint + 1) % waypoints.Count;
                return waypoints[currentWayPoint].position;
            }
            return self.transform.position;
        }
    }
}