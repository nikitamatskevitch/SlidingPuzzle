using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace DTT.MiniGame.SlidingPuzzle
{
	/// <summary>
	/// Makes an object Draggable.
	/// </summary>
	[RequireComponent(typeof(Button))]
    [RequireComponent(typeof(CanvasGroup))]
    public class Drag : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
    {
        /// <summary>
        /// When the object is being dragged.
        /// </summary>
        public event Action OnDragAction;

        /// <summary>
        /// When the object has ended being dragged.
        /// </summary>
        public event Action OnEndDragAction;

        /// <summary>
        /// When the object has started being dragged.
        /// </summary>
        public event Action OnStartDragAction;

        /// <summary>
        /// When the object has been tapped.
        /// </summary>
        public event Action OnTapAction;

        /// <summary>
        /// Reference to the button so we can click on it to start dragging.
        /// </summary>
        private Button _thisButton;

        /// <summary>
        /// Reference to the canvas group so we can disable raycast.
        /// </summary>
        protected CanvasGroup p_thisCanvas;

        /// <summary>
        /// Whether <see cref="OnDrag(PointerEventData)"/> has been called after <see cref="OnStartDrag"/> and before <see cref="OnPointerUp(PointerEventData)"/>
        /// </summary>
        private bool _draggedAfterStartDrag = false;

        /// <summary>
        /// Set when correct answer, so doesn't get turned back on again.
        /// </summary>
        private bool _turnedOffForever;

        /// <summary>
        /// Camera for the scene.
        /// </summary>
        protected Camera p_sceneCamera;

        /// <summary>
        /// Setter for the camera.
        /// </summary>
        public Camera SceneCamera { set => p_sceneCamera = value; }

        /// <summary>
        /// An offset to the GameObject when dragging.
        /// </summary>
        protected Vector2 p_draggingOffset = Vector2.zero;

        /// <summary>
        /// Is this gameobject actually draggable?
        /// </summary>
        private bool _canDrag;

        /// <summary>
        /// Getter and Setter for _canDrag property.
        /// </summary>
        public bool CanDrag
        {
            get => _canDrag;
            set => _canDrag = value; 
        }

        /// <summary>
        /// Starting position of the Snappable.
        /// </summary>
        private Vector3 _initialPosition;

        /// <summary>
        /// Getter and setter for the _initialPosition property.
        /// </summary>
        public Vector3 InitialPosition
        {
            get => _initialPosition; 
            set => _initialPosition = value; 
        }

        /// <summary>
        /// The snappingPoint reference if it is currently on one.
        /// </summary>
        public SnappingPoint SnappingPoint { get; protected set; }

        /// <summary>
        /// Whether any item is being dragged.
        /// </summary>
        private static bool _isDragging;

        /// <summary>
        /// Getter and Setter for the isDragging property.
        /// </summary>
        public bool IsDragging => _isDragging;

        /// <summary>
        /// Whether we want to animate the draggable going to the dragging location.
        /// </summary>
        [SerializeField]
        [Tooltip("Whether we want to animate the draggable going to the dragging location")]
        private bool _animateStartDrag;

        /// <summary>
        /// Whether we want the draggable to follow the dragging location with a delay.
        /// </summary>
        [SerializeField]
        [Tooltip("Whether we want the draggable to follow the dragging location with a delay")]
        private bool _animateDrag;

        /// <summary>
        /// Gets the button, saves its initial position and allow it to drag.
        /// </summary>
        private void Awake()
        {
            _thisButton = this.gameObject.GetComponent<Button>();
            p_thisCanvas = this.gameObject.GetComponent<CanvasGroup>();
            SetInitialPosition();
            this._canDrag = true;
        }

        /// <summary>
        /// Follow the input (mouse/touch) position if canvas is screenSpaceCamera.
        /// </summary>
        private void FollowInputScreenSpaceCamera()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount <= 0)
                return;

            Touch touch = Input.GetTouch(0);
            Vector3 screenPointTouch = touch.position;

            Vector3 newPos = p_sceneCamera.ScreenToWorldPoint(screenPointTouch);
#else
            Vector3 screenPoint = Input.mousePosition;
            screenPoint.x += p_draggingOffset.x;
            screenPoint.y += p_draggingOffset.y;
            Vector3 newPos = p_sceneCamera.ScreenToWorldPoint(screenPoint);
#endif
            newPos.z = this.transform.position.z;
            if (!_animateDrag)
                this.transform.position = newPos;
        }

        /// <summary>
        /// Get a ray cast hit list at the current input position.
        /// </summary>
        /// <returns>List of all the items that are hit on the current input position (Only items that have raycast enabled)</returns>
        public T GetScriptFromRaycast<T>(GameObject excludeObject) where T : class
        {
            List<RaycastResult> hitResults = new List<RaycastResult>();
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;
            EventSystem.current.RaycastAll(pointer, hitResults);

            foreach (RaycastResult result in hitResults)
            {
                if (!result.gameObject.Equals(excludeObject) && result.gameObject.GetComponent<T>() != null)
                    return result.gameObject.GetComponent<T>(); 

                else if (!result.gameObject.transform.parent.gameObject.Equals(excludeObject) && result.gameObject.transform.parent.GetComponentInParent<T>() != null)
                    return result.gameObject.GetComponentInParent<T>();
            }

            return null;
        }

        /// <summary>
        /// This function gets called when you Stop Dragging.
        /// </summary>
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (!_canDrag)
                return;

            OnEndDragAction?.Invoke();
            _isDragging = false;
        }

        /// <summary>
        /// This function gets called when you are dragging.
        /// </summary>
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging || !_canDrag)
            {
                eventData.pointerDrag = null;
                return;
            }
            
            FollowInputScreenSpaceCamera();
            OnDragAction?.Invoke();
            SetRayCast(false);
        }

        /// <summary>
        /// Invokes the drag action, allowed by overriding classes.
        /// </summary>
        protected void CallDragAction() => OnDragAction?.Invoke();

        /// <summary>
        /// This function gets called when you start dragging.
        /// </summary>
        public virtual void OnStartDrag()
        {
            if (_animateStartDrag)
            {
                Vector3 from = transform.position;
                Vector3 to = Vector3.zero;

#if UNITY_ANDROID || UNITY_IOS
                if (Input.touchCount <= 0)
                    return;

                to = p_sceneCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
#else 
                to = p_sceneCamera.ScreenToWorldPoint(Input.mousePosition);
#endif
                to.z = transform.position.z;
            }
            OnStartDragAction?.Invoke();
        }

        /// <summary>
        /// On pointer down set the current drag element in the ItemDrag Handler.
        /// </summary>
        /// <param name="eventData">event payload associated with pointer (mouse/touch)</param>  -> Automatically created method with param
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (!_canDrag && _isDragging)
            {
                eventData.pointerDrag = null;
                return;
            }

            _isDragging = true;
            ((IPointerDownHandler)_thisButton).OnPointerDown(eventData);
            OnStartDrag();
            transform.SetAsLastSibling();
        }

        /// <summary>
        /// On pointer up, check if the element has been dragged or just been tapped.
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (!_draggedAfterStartDrag && _canDrag)
                OnTapAction?.Invoke();
            _isDragging = false;
            _draggedAfterStartDrag = false;
        }

        /// <summary>
        /// Sets the initial position of this snappable.
        /// </summary>
        public virtual void SetInitialPosition() => _initialPosition = this.transform.position;

        /// <summary>
        /// Turn the raycast on or off so that the item can't be dragged anymore.
        /// </summary>
        /// <param name="active">True enables raycast, false disables</param>
        /// <param name="finalTurnOff">If it now turned off forever</param>
        public void SetRayCast(bool active,bool finalTurnOff = false)
        {
            if (_turnedOffForever)
                return;

            p_thisCanvas.blocksRaycasts = active;
            _turnedOffForever = finalTurnOff;
        } 

        /// <summary>
        /// Set the snappingpoint and if we unsnap, reenable the raycast for our former snappingpoint.
        /// </summary>
        /// <param name="snappingPoint">The snappingpoint to snap to, null if we unsnap</param>
        public virtual void SetSnappingPoint(SnappingPoint snappingPoint)
        {
            if (SnappingPoint != null)
            {
                if (!SnappingPoint.Equals(snappingPoint))
                {
                    SnappingPoint.SnapArea.raycastTarget = true;
                    SnappingPoint = snappingPoint;
                }
            }
            else
            {
                SnappingPoint = snappingPoint;
            }
        }
    }
}
