using UnityEngine;
using UnityEngine.AI;

namespace jcsilva.AISystem {

    public class IdleBehaviour : AIBehaviour {

        // Behaviour Settings
        private bool isActive;
        private float idleDuration;
        private float elapsedIdleTimer;

        // References
        private Transform selfTransform;
        private Transform target;
        private NavMeshAgent agent;
        private Animator animator;

        // Vision Settings
        private float maxDistance;
        private float maxFieldOfView;
        private float minDistanceToAttack;

        public IdleBehaviour(MonoBehaviour self, AIStateMachine aIStateMachine) : base(self, aIStateMachine, "Idle") {
            this.selfTransform = stateMachine.GetSelfPosition();
            this.target = stateMachine.GetTargetPosition();
            this.idleDuration = stateMachine.GetIdleDuration();
            this.maxDistance = stateMachine.GetMaxDistanceToView();
            this.maxFieldOfView = stateMachine.GetMaxFieldOfView();
            this.animator = stateMachine.GetAnimator();
        }

        public override void OnBehaviourStart() {
            isActive = true;
            stateMachine.EventAIEnableIdle?.Invoke();

            // All things that need to be set when this behaviour start should be put bellow this comment
            // example: Animator.SetBool("AnimationIdle", true);
            animator.SetBool("idle", true);
        }

        public override void OnBehaviourEnd() {
            isActive = false;
            elapsedIdleTimer = 0f;
            stateMachine.EventAIDisableIdle?.Invoke();

            // All things that need to be set when this behaviour ends should be put bellow this comment
            // example: Animator.SetBool("AnimationIdle", false);
            animator.SetBool("idle", false);
        }

        public override void OnUpdate() {
            if (isActive) {

                // Check if the has Vision of the enemy
                if (AIUtils.HasVisionOfTarget(selfTransform, target, maxDistance, maxFieldOfView)) {
                    Debug.Log("I see the player");
                    stateMachine.HandleState(AIEvents.SeePlayer);
                    return;
                }
                if (elapsedIdleTimer >= idleDuration) {
                    stateMachine.HandleState(AIEvents.NoLongerIdle);
                } else {
                    elapsedIdleTimer += Time.deltaTime;
                }

            }
        }
    }

}