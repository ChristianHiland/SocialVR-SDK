using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SocialSDK {
    [System.Serializable]
    public class AssignmentData {
        public string toAssignVar;
        public string fromVar;
        public MonoBehaviour toScript;
        public MonoBehaviour fromScript;
    }
    
    public class DynamicAssigner : MonoBehaviour {
        public List<AssignmentData> assignmentData;

        void Awake() {
            foreach (AssignmentData data in assignmentData) {
                // Get the type of the script
                MonoBehaviour fromScript = null;
                if (data.fromScript == null) {
                    fromScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
                } else {
                    fromScript = data.fromScript;
                }
                System.Type toType = data.toScript.GetType();
                System.Type fromType = fromScript.GetType();

                // Find the field by the string name
                FieldInfo toField = toType.GetField(data.toAssignVar, BindingFlags.Public | BindingFlags.Instance);
                FieldInfo fromField = fromType.GetField(data.fromVar, BindingFlags.Public | BindingFlags.Instance);

                object valueToSet = null;

                // GET logic
                if (fromField != null) {
                    valueToSet = fromField.GetValue(fromScript);
                } else {
                    PropertyInfo fromProp = fromType.GetProperty(data.fromVar, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                    if (fromProp != null) valueToSet = fromProp.GetValue(fromScript);
                }

                // SET logic
                if (valueToSet != null) {
                    if (toField != null) {
                        toField.SetValue(data.toScript, valueToSet);
                    } else {
                        PropertyInfo toProp = toType.GetProperty(data.toAssignVar, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                        if (toProp != null) toProp.SetValue(data.toScript, valueToSet);
                    }
                } else {
                    Debug.LogError($"[DynamicAssigner] Failed to find or get value from: {data.fromVar}");
                }

            }
        }
    }
}