using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using KhanhAK.VR;
using UnityEngine.Animations.Rigging;

namespace KhanhAK.VREditor
{
    [CustomEditor(typeof(RigManager))]
    public class RigManagerEditor : UnityEditor.Editor
    {
        string targetName = "Target";
        string hintName = "Hint";

        // Head
        string headRigGroupName = "HeadRigGroup";
        string headXYName = "HeadXY";
        string headZName = "HeadZ";

        // Hands
        string handRigGroupName = "HandRigGroup";
        string ArmRightName = "ArmRight";
        string ArmLeftName = "ArmLeft";


        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            RigManager rigManager = target as RigManager;
            if(rigManager.Builder == null) return;

            if(GUILayout.Button("Init structure"))
            {
                InitRigStructure(rigManager.Builder);
            }
        }

        void InitRigStructure(RigBuilder builder)
        {
            // Head
            var headRigGroup = GetOrCreate(headRigGroupName, builder.transform);

            var headXYTrans = GetOrCreate(headXYName, headRigGroup);
            var headXYTarget = GetOrCreate(headXYName + targetName, headXYTrans);

            var headZTrans = GetOrCreate(headZName, headRigGroup);
            var headZTarget = GetOrCreate(headZName + targetName, headZTrans);

            // Hands
            var handRigGroup = GetOrCreate(handRigGroupName, builder.transform);

            var handRight = GetOrCreate(ArmRightName, handRigGroup);
            var handRightTarget = GetOrCreate(ArmRightName + targetName, handRight);
            var handRightHint = GetOrCreate(ArmRightName + hintName, handRight);

            var handLeft = GetOrCreate(ArmLeftName, handRigGroup);
            var handLeftTarget = GetOrCreate(ArmLeftName + targetName, handLeft);
            var handLeftHint = GetOrCreate(ArmLeftName + hintName, handLeft);


            builder.layers.Clear();

            GetOrAddComponent<Rig>(headRigGroup.gameObject);

            GetOrAddComponent<MultiAimConstraint>(headXYTrans.gameObject);
            GetOrAddComponent<MultiPositionConstraint>(headXYTarget.gameObject);

            GetOrAddComponent<TwistCorrection>(headZTrans.gameObject);
            GetOrAddComponent<MultiRotationConstraint>(headZTarget.gameObject);   


            GetOrAddComponent<Rig>(handRigGroup.gameObject);

            GetOrAddComponent<TwoBoneIKConstraint>(handRight.gameObject);
            GetOrAddComponent<MultiPositionConstraint>(handRightTarget.gameObject);
            GetOrAddComponent<MultiRotationConstraint>(handRightTarget.gameObject);

            GetOrAddComponent<TwoBoneIKConstraint>(handLeft.gameObject);
            GetOrAddComponent<MultiPositionConstraint>(handLeftTarget.gameObject);
            GetOrAddComponent<MultiRotationConstraint>(handLeftTarget.gameObject);

            // Assign to component
            RigManager rigManager = target as RigManager;
            rigManager.HeadXYRig = headXYTarget.gameObject;
            rigManager.HeadZRig = headZTarget.gameObject;
            rigManager.LeftHandRig = handLeftTarget.gameObject;
            rigManager.RightHandRig = handRightTarget.gameObject;
        }

        void TargetMultiPosition(string name, Transform parent)
        {
            var trans = GetOrCreate(name, parent);
            var pos = GetOrAddComponent<MultiPositionConstraint>(trans.gameObject);
            pos.data.constrainedObject = trans;
        }

        Transform GetOrCreate(string name, Transform parent)
        {
            var obj = parent.Find(name);
            if(obj == null)
            {
                obj = new GameObject(name).transform;
                obj.SetParent(parent);
                ResetTransform(obj);
            }    
            return obj;
        }

        T GetOrAddComponent<T>(GameObject obj) where T : Component
        {
            T com = obj.GetComponent<T>();
            if(com == null) obj.AddComponent<T>();
            return com;
        }

        WeightedTransformArray ApplySourceObjects(WeightedTransformArray wArray, int index, Transform trans, float weight)
        {
            wArray.SetTransform(index, trans);
            wArray.SetWeight(index, weight);
            return wArray;
        }

        void ResetTransform(Transform t)
        {
            t.position = Vector3.zero;
            t.rotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }
    }
}
