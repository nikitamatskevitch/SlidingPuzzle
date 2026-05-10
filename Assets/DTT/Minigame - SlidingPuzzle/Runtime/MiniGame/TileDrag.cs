using UnityEngine;
using UnityEngine.EventSystems;

namespace DTT.MiniGame.SlidingPuzzle
{
	///<summary>
	/// The component on the slide tile objects that lets them be dragged inside the board.
	///</summary>
	public class TileDrag : Drag, IPointerUpHandler
	{
        /// <summary>
        /// Reference to the slideTileController of this scene.
        /// </summary>
        private SlideTileController _tileController;

        /// <summary>
        /// Setter for _tileController reference.
        /// </summary>
        public SlideTileController TileController { set => _tileController = value; }

        /// <summary>
        /// Action performed when user is no longer pressing on this tile.
        /// </summary> 
        public event System.Action OnPointerUpAction;

        /// <summary>
        /// Has the tile been dragged and moved?
        /// </summary>
        private bool _beenDragged;

        /// <summary>
        /// Getter and setter for _beenDragged property.
        /// </summary>
        public bool BeenDragged
        {
            get => _beenDragged; 
            set => _beenDragged = value; 
        }

        /// <summary>
        /// Make sure only one tile at a time can be moved so that they dont disregard any of the checks.
        /// </summary>
        public bool HandleDragInput => Input.touches.Length <= 1; 

        /// <summary>
        /// Initial position in UI space.
        /// </summary>
        private Vector2 _initialAnchoredPosition;

        /// <summary>
        /// Getter and setter for _beenDragged property.
        /// </summary>
        public Vector2 InitialAnchoredPosition
        {
            get => _initialAnchoredPosition; 
            set => _initialAnchoredPosition = value; 
        }

        /// <summary>
        /// How much should the tile be offset from its initial position?
        /// </summary>
        private Vector3 _moveVec;

        /// <summary>
        /// The user is currently holding the tile will set this to true.
        /// </summary>
        private bool _isMovingTile = false;

        /// <summary>
        /// OnDragStart = true, OnDragEnd = false.
        /// </summary>
        private bool _isDragging = true;

        /// <summary>
        /// Sets the initial values for the dragging properties.
        /// </summary>
        private void OnEnable()
        {
            _isDragging = true;
            CanDrag = false;
        }

        /// <summary>
        /// When OnDrag is triggered.
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnDrag(PointerEventData eventData)
        {
            if (_tileController.IsScrambling || !CanDrag)
                return;

            CallDragAction();
            if (!HandleDragInput)
            {
                OnPointerUp(eventData);
                eventData.pointerDrag = null;
                return;
            }

            FollowInputScreenSpaceAxis();
        }

        /// <summary>
        /// Sets values for when the tile starts to get dragged.
        /// </summary>
        public override void OnStartDrag()
        {
            if (_tileController.IsScrambling)
                return;

            base.OnStartDrag();
            _isDragging = true;
        }

        /// <summary>
        /// Sets values for when OnPointerUp event is triggered.
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (_tileController.IsScrambling)
                return;

            if (!CanDrag || eventData.pointerDrag == null)
                return;

            OnPointerUpAction?.Invoke();
            _beenDragged = false;
        }

        /// <summary>
        /// Sets values for when OnEndDrag event is triggered.
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnEndDrag(PointerEventData eventData)
        {
            if (_tileController.IsScrambling)
                return;

            base.OnEndDrag(eventData);
            _isDragging = false;
            _isMovingTile = false;
        }

        /// <summary>
        /// Sets the initial position for the tile.
        /// </summary>
        /// <param name="eventData"></param>
        public override void SetInitialPosition()
        {
            if (_isMovingTile)
                return;

            InitialPosition = transform.position;
            _initialAnchoredPosition = ((RectTransform)transform).anchoredPosition;

        }

        /// <summary>
        /// Drag the object on a specific axis.
        /// </summary>
        private void FollowInputScreenSpaceAxis()
        {
            if (!_tileController.TileMoveDir.HasValue)
                return;

            if (!_isDragging)
                return;

            _isMovingTile = true;
            _beenDragged = true;

            Vector2Int moveDir = _tileController.TileMoveDir.Value;

            Vector3 screenPoint = Vector3.zero;

#if UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount <= 0)
                return;

            screenPoint = Input.GetTouch(0).position;
#else
            screenPoint = Input.mousePosition;
#endif
            screenPoint.z = 100f; // Distance of the plane from the camera.
            screenPoint = p_sceneCamera.ScreenToWorldPoint(screenPoint);
            _moveVec = screenPoint - InitialPosition;
            transform.position = InitialPosition + new Vector3(_moveVec.x * Mathf.Abs(moveDir.x), _moveVec.y * Mathf.Abs(moveDir.y));
            
            // Clamp tile position to within the available moving space.
            ((RectTransform)transform).anchoredPosition = new Vector2(
                SmartClamp(((RectTransform)transform).anchoredPosition.x, _initialAnchoredPosition.x,
                _initialAnchoredPosition.x + _tileController.TileMoveAmount * moveDir.x), // x component.
            
                SmartClamp(((RectTransform)transform).anchoredPosition.y, _initialAnchoredPosition.y, // y component.
                _initialAnchoredPosition.y + _tileController.TileMoveAmount * (moveDir.y * -1)));

        }

        /// <summary>
        /// Clamp the given value between two values (automatically clamps between smallest and largest rather than needing to specify by argument order).
        /// </summary>
        /// <param name="value">the value needed to be clamped</param>
        /// <param name="clamp1">the min or max</param>
        /// <param name="clamp2">the min or max</param>
        /// <returns>the clamped value</returns>
        private float SmartClamp(float value, float clamp1, float clamp2) => Mathf.Clamp(value, clamp1 < clamp2 ? clamp1 : clamp2, clamp1 > clamp2 ? clamp1 : clamp2);
    }
}