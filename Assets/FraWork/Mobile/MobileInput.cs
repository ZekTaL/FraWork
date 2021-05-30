using UnityEngine;

using InvalidOperationException = System.InvalidOperationException;
using NullReferenceException = System.NullReferenceException;

namespace FraWork.Mobile
{
    public class MobileInput : MonoBehaviour
    {
        // Has the mobile input system been initialised
        public static bool Initialised => instance != null;

        // Singleton reference instance
        private static MobileInput instance = null;

        /// <summary>
        /// If the system isn't already setup, this will instantiate the mobile input prefab and assign the static reference
        /// </summary>
        public static void Initialise()
        {
            // if the mobile input is already initialised, throw an exception to tell the user they dun goofed
            if (Initialised)
            {
                throw new InvalidOperationException("Mobile Input already initialised!");
            }

            // load the Mobile Input prefab and instantiate it, setting the instance
            MobileInput prefabInstance = Resources.Load<MobileInput>("MobileInputPrefab");
            instance = Instantiate(prefabInstance);

            // changed the instantiated objects name and mark it to not be destroyed
            instance.gameObject.name = "MobileInput";
            DontDestroyOnLoad(instance.gameObject);
        }

        /// <summary>
        /// returns the value of the joystick from the joystick module if it's valid
        /// </summary>
        /// <param name="_axis">the axis to get the input from, Horizontal = x; Vertical = y</param>
        public static float GetJoystickAxis(JoystickAxis _axis)
        {
            // if the mobile input isn't initialised, thrown an InvalidOperationException
            if (!Initialised)
            {
                throw new InvalidOperationException("Mobile Input not initialised.");
            }

            // if the joystick input module isn't set, throw a NullReferenceException
            if (instance.joystickInput == null)
            {
                throw new NullReferenceException("Joystick Input reference not set.");
            }

            // switch on the passed axis and return the appropriate value
            switch (_axis)
            {
                case JoystickAxis.Horizontal: return instance.joystickInput.Axis.x;
                case JoystickAxis.Vertical: return instance.joystickInput.Axis.y;
                default: return 0;
            }
        }

        /// <summary>
        /// Attempts to retrieve the relevant swipe information relating the the passed ID.
        /// </summary>
        /// <param name="_index">The fingerID we are attempting to get the swipe for.</param>
        /// <returns>The corresponding swipe if it exists, otherwise null.</returns>
        public static SwipeInput.Swipe GetSwipe(int _index)
        {
            // if the mobile input isn't initialised, thrown an InvalidOperationException
            if (!Initialised)
            {
                throw new InvalidOperationException("Mobile Input not initialised.");
            }

            // if the swipe input module isn't set, throw a NullReferenceException
            if (instance.swipeInput == null)
            {
                throw new NullReferenceException("Swipe Input reference not set.");
            }

            // Retrieve the swipe for this index from the swipe input manager
            return instance.swipeInput.GetSwipe(_index);
        }

        public static void GetFlickData(out float _flickPower, out Vector2 _flickDirection)
        {
            // if the mobile input isn't initialised, thrown an InvalidOperationException
            if (!Initialised)
            {
                throw new InvalidOperationException("Mobile Input not initialised.");
            }

            // if the swipe input module isn't set, throw a NullReferenceException
            if (instance.swipeInput == null)
            {
                throw new NullReferenceException("Swipe Input reference not set.");
            }

            // Set the out parameters to their corresponding values in the swipe input class
            _flickPower = instance.swipeInput.FlickPower;
            _flickDirection = instance.swipeInput.FlickDirection;
        }

        [SerializeField] private JoystickInput joystickInput;
        [SerializeField] private SwipeInput swipeInput;
    }
}
