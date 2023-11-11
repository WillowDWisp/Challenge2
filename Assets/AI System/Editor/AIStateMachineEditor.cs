using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using jcsilva.AISystem;

namespace jcsilva.AISystem {

    [CustomEditor(typeof(AIStateMachine))]
    public class AIStateMachineEditor : Editor {

        protected AIStateMachine aiStateMachine;
        
        private GameObject _mainHolder;         // Holds the main GameObject for Waypoints
        private GameObject _waypointHolder;     // Reference to the Holder that will store waypoints of this gameObject

        private GameObject _selfData;           // Reference of the gameObject currently selected

        protected void Awake() {
            aiStateMachine = (AIStateMachine)target;
            _selfData = aiStateMachine.gameObject;
            aiStateMachine.ClearEmptyWaypoints();
            GetWaypointsGameObject();
        }

        

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("AI Patrol Settings", EditorStyles.boldLabel);
            if (GUILayout.Button("Create New Waypoint")) {
                CreateNewWayPoint();
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("waypoints"));

            EditorGUILayout.LabelField("Developer Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enableDebug"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentState"));

            if (GUI.changed)
                EditorUtility.SetDirty(aiStateMachine);
        }

        private void GetWaypointsGameObject() {
            // Check if a Waypoint GameObject Was Generated
            if (!GameObject.Find("--- Waypoints ---")) {
                _mainHolder = Creator.CreateWaypointsHolder();
            } else {
                _mainHolder = GameObject.Find("--- Waypoints ---");
            }
        }

        private void CreateNewWayPoint() {

            if (_mainHolder == null) GetWaypointsGameObject();

            string waypointHolderName = Creator.SetInternalName(_selfData.transform.name);

            if (_waypointHolder == null) {
                bool exists = false;
                foreach(Transform child in _mainHolder.transform) {
                    if(child.name.Equals(waypointHolderName)) {
                        _waypointHolder = child.gameObject;
                        exists = true;
                        break;
                    }
                }
                if(!exists) {
                    _waypointHolder = Creator.CreateGameObject(_mainHolder, waypointHolderName);
                }
            }

            int waypointsCounter = _waypointHolder.transform.childCount;

            Transform newWaypoint = Creator.CreateGameObject(_waypointHolder, "Waypoint_" + waypointsCounter).transform;

            aiStateMachine.waypoints.Add(newWaypoint);

        }
    }
}