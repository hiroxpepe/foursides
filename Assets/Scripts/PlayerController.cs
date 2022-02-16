/*
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Studio.MeowToon {
    /// <summary>
    /// player controller.
    /// @author h.adachi
    /// </summary>
    public class PlayerController : GamepadMaper {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // References

        [SerializeField]
        float _jumpPower = 5.0f;

        [SerializeField]
        float _rotationalSpeed = 5.0f;

        [SerializeField]
        float _forwardSpeedLimit = 1.1f;

        [SerializeField]
        float _runSpeedLimit = 3.25f;

        [SerializeField]
        float _backwardSpeedLimit = 0.75f;

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        DoFixedUpdate _doFixedUpdate;

        float _speed;

        ///////////////////////////////////////////////////////////////////////////////////////////
        // update Methods

        // Awake is called when the script instance is being loaded.
        void Awake() {
            _doFixedUpdate = DoFixedUpdate.GetInstance();
        }

        // Start is called before the first frame update
        new void Start() {
            base.Start();

            var rb = transform.GetComponent<Rigidbody>(); // Rigidbody should be only used in FixedUpdate.

            this.FixedUpdateAsObservable().Subscribe(_ => {
                _speed = rb.velocity.magnitude; // get speed.
                Debug.Log("speed: " + _speed); // FIXME:
            });

            // walk.
            this.UpdateAsObservable().Where(_ => _upButton.isPressed).Subscribe(_ => {
                _doFixedUpdate.ApplyWalk();
            });

            this.FixedUpdateAsObservable().Where(_ => _doFixedUpdate.walk && _speed < _forwardSpeedLimit).Subscribe(_ => {
                rb.AddFor​​ce(transform.forward * 12.0f, ForceMode.Acceleration);
                _doFixedUpdate.CancelWalk();
            });

            // run.
            this.UpdateAsObservable().Where(_ => _upButton.isPressed && _yButton.isPressed).Subscribe(_ => {
                _doFixedUpdate.ApplyRun();
            });

            this.FixedUpdateAsObservable().Where(_ => _doFixedUpdate.run && _speed < _runSpeedLimit).Subscribe(_ => {
                rb.AddFor​​ce(transform.forward * 12.0f, ForceMode.Acceleration);
                _doFixedUpdate.CancelRun();
            });

            // backward.
            this.UpdateAsObservable().Where(_ => _downButton.isPressed).Subscribe(_ => {
                _doFixedUpdate.ApplyBackward();
            });

            this.FixedUpdateAsObservable().Where(_ => _doFixedUpdate.backward && _speed < _backwardSpeedLimit).Subscribe(_ => {
                rb.AddFor​​ce(-transform.forward * 12.0f, ForceMode.Acceleration);
                _doFixedUpdate.CancelBackward();
            });

            // jump.
            this.UpdateAsObservable().Where(_ => _bButton.wasPressedThisFrame).Subscribe(_ => {
                _doFixedUpdate.ApplyJump();
            });

            this.FixedUpdateAsObservable().Where(_ => _doFixedUpdate.jump).Subscribe(_ => {
                rb.useGravity = true;
                rb.AddRelativeFor​​ce(Vector3.up * _jumpPower * 40f, ForceMode.Acceleration);
                _doFixedUpdate.CancelJump();
            });

            // rotate.
            this.UpdateAsObservable().Subscribe(_ => {
                var axis = _rightButton.isPressed ? 1 : _leftButton.isPressed ? -1 : 0;
                transform.Rotate(0, axis * (_rotationalSpeed * Time.deltaTime) * 12.0f, 0);
            });
        }

        #region DoUpdate

        /// <summary>
        /// class for the Update() method.
        /// </summary>
        class DoUpdate {

            ///////////////////////////////////////////////////////////////////////////////////////
            // Fields

            bool _grounded;

            ///////////////////////////////////////////////////////////////////////////////////////
            // Properties

            public bool grounded {
                get => _grounded;
                set => _grounded = value;
            }

            ///////////////////////////////////////////////////////////////////////////////////////
            // Constructor

            /// <summary>
            /// returns an initialized instance.
            /// </summary>
            public static DoUpdate GetInstance() {
                return new DoUpdate();
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // public Methods [verb]

            public void ResetState() {
                _grounded = false;
            }
        }

        #endregion

        #region DoFixedUpdate

        /// <summary>
        /// class for the FixedUpdate() method.
        /// </summary>
        class DoFixedUpdate {

            ///////////////////////////////////////////////////////////////////////////////////////
            // Fields

            bool _idol;
            bool _run;
            bool _walk;
            bool _jump;
            bool _backward;

            ///////////////////////////////////////////////////////////////////////////////////////
            // Properties

            public bool idol { get => _idol; }
            public bool run { get => _run; }
            public bool walk { get => _walk; }
            public bool jump { get => _jump; }
            public bool backward { get => _backward; }

            ///////////////////////////////////////////////////////////////////////////////////////
            // Constructor

            /// <summary>
            /// returns an initialized instance.
            /// </summary>
            public static DoFixedUpdate GetInstance() {
                return new DoFixedUpdate();
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // public Methods

            public void ApplyRun() {
                _idol = _walk = _backward = false;
                _run = true;
            }

            public void CancelRun() {
                _run = false;
            }

            public void ApplyWalk() {
                _idol = _run = _backward = false;
                _walk = true;
            }

            public void CancelWalk() {
                _walk = false;
            }

            public void ApplyBackward() {
                _idol = _run = _walk = false;
                _backward = true;
            }

            public void CancelBackward() {
                _backward = false;
            }

            public void ApplyJump() {
                _jump = true;
            }

            public void CancelJump() {
                _jump = false;
            }
        }

        #endregion

    }

}
