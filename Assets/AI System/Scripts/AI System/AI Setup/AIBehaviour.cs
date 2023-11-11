using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace jcsilva.AISystem {

    public abstract class AIBehaviour {

        protected MonoBehaviour self;
        protected AIStateMachine stateMachine;
        private string behaviourName;

        public AIBehaviour(MonoBehaviour self, AIStateMachine stateMachine, string behaviourName = "DEFAULT NAME") {
            this.self = self;
            this.stateMachine = stateMachine;
            this.behaviourName = behaviourName;
        }

        /// <summary>
        /// Method that will update the behaviouor Status
        /// </summary>
        public abstract void OnUpdate();

        /// <summary>
        /// Called when the behaviour is Ongoing
        /// </summary>
        public abstract void OnBehaviourStart();

        /// <summary>
        /// Called when the behaviour needs to be stopped
        /// </summary>
        public abstract void OnBehaviourEnd();

    }

}