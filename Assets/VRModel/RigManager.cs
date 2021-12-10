using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace KhanhAK.VR
{
    public class RigManager : MonoBehaviour
    {
        public RigBuilder Builder = null;


        [Header("Movement Sources")]
        public Transform Head = null;
        public Transform HeadAim = null;
        public Transform LeftHand = null, RightHand = null;
        public float headHeight = 0f;


        [Header("In-Model parts")]
        public Transform BodyPart = null;
        public Transform HeadPart = null;
        public GameObject HeadXYRig = null;
        public GameObject HeadZRig = null;
        public GameObject LeftHandRig = null, RightHandRig = null; 
        

        Vector3 bodyHeadOffset = Vector3.zero;


        void Start()
        {
            RigSetup();
        }

        void Update()
        {
            if(Head && BodyPart)
            {
                BodyPart.transform.position = Head.transform.position + bodyHeadOffset + Head.transform.up * headHeight;
            }
        }

        void RigSetup()
        {
            if(HeadAim && HeadXYRig)
            {
                UpdateLinkConstraint(HeadXYRig, HeadAim, 1f);
            }

            if(HeadAim && HeadZRig)
            {
                UpdateLinkConstraint(HeadZRig, Head, 1f);
            }

            if(LeftHand && LeftHandRig)
            {
                UpdateLinkConstraint(LeftHandRig, LeftHand, 1f);
            } 

            if(RightHand && RightHandRig)
            {
                UpdateLinkConstraint(RightHandRig, RightHand, 1f);
            } 

            Builder.Build();

            if(HeadPart && BodyPart)     
                bodyHeadOffset = BodyPart.transform.position - HeadPart.transform.position;
        }

        void UpdateLinkConstraint(GameObject constraint, Transform trans, float weight)
        {
            var pos = constraint.GetComponent<MultiPositionConstraint>();
            if(pos)
            {
                pos.data.sourceObjects = ApplySourceObjects(pos.data.sourceObjects, 0, trans, weight);
                AlignPos(constraint.transform, trans);
            }

            var rot = constraint.GetComponent<MultiRotationConstraint>();
            if(rot)
            {
                rot.data.sourceObjects = ApplySourceObjects(rot.data.sourceObjects, 0, trans, weight);
                AlignRot(constraint.transform, trans);
            }

            var aim = constraint.GetComponent<MultiAimConstraint>();
            if(aim)
            {
                aim.data.sourceObjects = ApplySourceObjects(aim.data.sourceObjects, 0, trans, weight);
                AlignPos(constraint.transform, trans);
            }
        }

        WeightedTransformArray ApplySourceObjects(WeightedTransformArray wArray, int index, Transform trans, float weight)
        {
            wArray.SetTransform(index, trans);
            wArray.SetWeight(index, weight);
            return wArray;
        }

        void AlignPos(Transform target, Transform source) => target.transform.position = source.transform.position;
        void AlignRot(Transform target, Transform source) => target.transform.rotation = source.transform.rotation;
    }
}
