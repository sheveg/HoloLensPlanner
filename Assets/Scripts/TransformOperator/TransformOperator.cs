using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.Receivers;
using HoloToolkit.Unity.InputModule;

namespace HoloLensPlanner
{
    public enum TransformOperation
    {
        X,
        Y,
        Z,
        X_Neg,
        Y_Neg,
        Z_Neg
    }

    public enum TransformPosition
    {
        Above,
        Centered
    }

    /// <summary>
    /// TransformOperator is used to move the corners of the floor plane or the floor plane itself
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class TransformOperator : InteractionReceiver
    {
        public float MoveValue = 0.01f;

        [SerializeField]
        private TransformButton Button_X;

        [SerializeField]
        private TransformButton Button_Y;

        [SerializeField]
        private TransformButton Button_Z;

        [SerializeField]
        private TransformButton Button_X_Neg;

        [SerializeField]
        private TransformButton Button_Y_Neg;

        [SerializeField]
        private TransformButton Button_Z_Neg;

        private void Awake()
        {
            GetComponent<BoxCollider>().enabled = false;
        }

        public void AttachToTarget(Transform target, bool xEnabled, bool yEnabled, bool zEnabled, TransformPosition position)
        {
            changeButtonState(Button_X, xEnabled, target);
            changeButtonState(Button_X_Neg, xEnabled, target);

            changeButtonState(Button_Y, yEnabled, target);
            changeButtonState(Button_Y_Neg, yEnabled, target);

            changeButtonState(Button_Z, zEnabled, target);
            changeButtonState(Button_Z_Neg, zEnabled, target);

            transform.parent = target;
            positionToTarget(target, position);
        }

        protected override void InputDown(GameObject obj, InputEventData eventData)
        {
            TransformButton button;
            if ((button = obj.GetComponent<TransformButton>()) != null)
            {
                handleOperation(button.Operation, button.Target);
            }
        }

        private void changeButtonState(TransformButton button, bool state, Transform target)
        {
            button.gameObject.SetActive(state);
            if (!state)
                return;

            button.Target = target;
            Registerinteractable(button.gameObject);
        }

        private void positionToTarget(Transform target, TransformPosition position)
        {
            switch (position)
            {
                case TransformPosition.Above:
                    positionAboveTarget(target);
                    break;
                case TransformPosition.Centered:
                    positionCenteredToTarget(target);
                    break;
            }
            
        }

        private void positionAboveTarget(Transform target)
        {
            transform.position = target.transform.position + Vector3.up * GetComponent<BoxCollider>().size.y * transform.localScale.y;
        }

        private void positionCenteredToTarget(Transform target)
        {
            transform.position = target.transform.position + Vector3.up * 0.01f;
        }

        private void handleOperation(TransformOperation op, Transform target)
        {
            switch (op)
            {
                case TransformOperation.X:
                    moveX(target, 1);
                    break;
                case TransformOperation.Y:
                    moveY(target, 1);
                    break;
                case TransformOperation.Z:
                    moveZ(target, 1);
                    break;
                case TransformOperation.X_Neg:
                    moveX(target, -1);
                    break;
                case TransformOperation.Y_Neg:
                    moveY(target, -1);
                    break;
                case TransformOperation.Z_Neg:
                    moveZ(target, -1);
                    break;
            }
        }

        private void moveX(Transform target, int sign)
        {
            target.transform.position += Vector3.right * MoveValue * sign;
            updatePoint(target);
        }

        private void moveY(Transform target, int sign)
        {
            target.transform.position += Vector3.up * MoveValue * sign;
            updatePoint(target);
        }

        private void moveZ(Transform target, int sign)
        {
            target.transform.position += Vector3.forward * MoveValue * sign;
            updatePoint(target);
        }

        private void updatePoint(Transform target)
        {
            // update the points
            PolygonPoint point;
            if ((point = target.GetComponent<PolygonPoint>()) != null)
            {
                point.IngoingEdge.SetPoints(point.IngoingEdge.From, point);
                point.OutgoingEdge.SetPoints(point, point.OutgoingEdge.To);
                RoomManager.Instance.Floor.Setup(RoomManager.Instance.Floor.MeshPolygon, PlaneType.Floor);
            }
        }
    }
}
