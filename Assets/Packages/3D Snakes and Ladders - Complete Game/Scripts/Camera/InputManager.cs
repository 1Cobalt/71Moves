using UnityEngine;

namespace SceneManagement
{
    public enum InputType
    {
        Automatic, Mouse, TouchPad
    }
    public class InputManager : MonoBehaviour
    {
        #region variable
        #region public
        public InputType inputType = InputType.Automatic;
        public float updateInterval = 0.00001F;
        #endregion
        #region private
        private float _x;
        private float _y;
        private float _deltaDistance;
        private bool _click;
        private double lastInterval;
        #endregion
        #endregion
        #region Functions
        private void Start()
        {
            if (inputType == InputType.Automatic)
            {
                if (SystemInfo.deviceType == DeviceType.Desktop)
                {
                    inputType = InputType.Mouse;
                }
                else if (SystemInfo.deviceType == DeviceType.Handheld)
                {
                    inputType = InputType.TouchPad;
                }
            }
        }
        private void Update()
        {
            switch (inputType)
            {
                case InputType.Mouse:
                    update_mouseClick();
                    update_mouseDeltaPosition();
                    break;
                case InputType.TouchPad:
                    update_touchPadDeltaPosition();
                    break;
            }
        }
        #endregion
        #region functions
        public Vector2 getDeltaPosition()
        {
            return new Vector2(_x, _y);
        }
        public float getDeltaDistance()
        {
            return _deltaDistance;
        }
        #region Mouse
        private void update_mouseClick()
        {
            if (Input.GetMouseButtonDown(0)) onClick();
        }
        private void update_mouseDeltaPosition()
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                _x = Input.GetAxis("Mouse X");
                _y = Input.GetAxis("Mouse Y");
            }
            else
            {
                _x = _y = 0;
            }
            _deltaDistance = Input.GetAxis("Mouse ScrollWheel");
        }
        #endregion
        #region TouchPad
        private void update_touchPadDeltaPosition()
        {
            if (Input.touchCount > 0)
            {

                Touch touch = Input.GetTouch(0);
                if (Input.touchCount == 1)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        onClick();
                    }
                }

                if (touch.phase == TouchPhase.Moved)//Input.GetAxis("Mouse X") Input.GetAxis("Mouse Y")
                {
                    Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
                    _x = touchDeltaPosition.x;
                    _y = touchDeltaPosition.y;
                }
                if (Input.touchCount == 2)
                {
                    touch = Input.GetTouch(1);

                    Touch tZero = Input.GetTouch(0);
                    Touch tOne = Input.GetTouch(1);
                    // get touch position from the previous frame
                    Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
                    Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;

                    float oldTouchDistance = Vector2.Distance(tZeroPrevious, tOnePrevious);
                    float currentTouchDistance = Vector2.Distance(tZero.position, tOne.position);

                    // get offset value
                    _deltaDistance = oldTouchDistance - currentTouchDistance;
                }
            }
            else
            {
                _x = _y = _deltaDistance = 0;
            }
        }
        #endregion
        #region share
        public bool IsClicked()
        {
            if (_click)
            {
                float timeNow = Time.realtimeSinceStartup;
                if (timeNow > lastInterval + updateInterval)
                {
                    _click = false;
                    lastInterval = timeNow;
                    return true;
                }
                _click = false;
            }
            return false;
        }
        private void onClick()
        {
            _click = true;
            lastInterval = Time.realtimeSinceStartup;
        }
        #endregion
        #region hit
        public bool getRaycastHit<T>(out RaycastHit hit)
        {
            Vector3 mouse = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mouse);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                return hit.collider.gameObject.GetComponent<T>() != null;
            }
            return false;
        }
        #endregion
        #endregion
    }
}