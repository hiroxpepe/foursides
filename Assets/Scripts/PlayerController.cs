/*
 * Copyright 2002-2020 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Examproject {
    /// <summary>
    /// player controller.
    /// @author h.adachi
    /// </summary>
    public class PlayerController : GamepadMaper {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // References

        [SerializeField]
        private float jumpPower = 5.0f;

        [SerializeField]
        private float rotationalSpeed = 5.0f;

        [SerializeField]
        private float forwardSpeedLimit = 1.1f;

        [SerializeField]
        private float runSpeedLimit = 3.25f;

        [SerializeField]
        private float backwardSpeedLimit = 0.75f;

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        private DoFixedUpdate doFixedUpdate;

        private float speed;

        ///////////////////////////////////////////////////////////////////////////////////////////
        // update Methods

        // Awake is called when the script instance is being loaded.
        void Awake() {
            doFixedUpdate = DoFixedUpdate.GetInstance();
        }

        // Start is called before the first frame update
        new void Start() {
            base.Start();

            var _rb = transform.GetComponent<Rigidbody>(); // Rigidbody should be only used in FixedUpdate.

            this.FixedUpdateAsObservable().Subscribe(_ => {
                speed = _rb.velocity.magnitude; // get speed.
                Debug.Log("speed: " + speed); // FIXME:
            });

            // walk.
            this.UpdateAsObservable().Where(_ => upButton.isPressed).Subscribe(_ => {
                doFixedUpdate.ApplyWalk();
            });

            this.FixedUpdateAsObservable().Where(_ => doFixedUpdate.walk && speed < forwardSpeedLimit).Subscribe(_ => {
                _rb.AddFor​​ce(transform.forward * 12.0f, ForceMode.Acceleration);
                doFixedUpdate.CancelWalk();
            });

            // run.
            this.UpdateAsObservable().Where(_ => upButton.isPressed && yButton.isPressed).Subscribe(_ => {
                doFixedUpdate.ApplyRun();
            });

            this.FixedUpdateAsObservable().Where(_ => doFixedUpdate.run && speed < runSpeedLimit).Subscribe(_ => {
                _rb.AddFor​​ce(transform.forward * 12.0f, ForceMode.Acceleration);
                doFixedUpdate.CancelRun();
            });

            // backward.
            this.UpdateAsObservable().Where(_ => downButton.isPressed).Subscribe(_ => {
                doFixedUpdate.ApplyBackward();
            });

            this.FixedUpdateAsObservable().Where(_ => doFixedUpdate.backward && speed < backwardSpeedLimit).Subscribe(_ => {
                _rb.AddFor​​ce(-transform.forward * 12.0f, ForceMode.Acceleration);
                doFixedUpdate.CancelBackward();
            });

            // jump.
            this.UpdateAsObservable().Where(_ => bButton.wasPressedThisFrame).Subscribe(_ => {
                doFixedUpdate.ApplyJump();
            });

            this.FixedUpdateAsObservable().Where(_ => doFixedUpdate.jump).Subscribe(_ => {
                _rb.useGravity = true;
                _rb.AddRelativeFor​​ce(Vector3.up * jumpPower * 40f, ForceMode.Acceleration);
                doFixedUpdate.CancelJump();
            });

            // rotate.
            this.UpdateAsObservable().Subscribe(_ => {
                var _axis = rightButton.isPressed ? 1 : leftButton.isPressed ? -1 : 0;
                transform.Rotate(0, _axis * (rotationalSpeed * Time.deltaTime) * 12.0f, 0);
            });
        }

        #region DoFixedUpdate

        /// <summary>
        /// structure for the FixedUpdate() method.
        /// </summary>
        protected struct DoFixedUpdate {

            ///////////////////////////////////////////////////////////////////////////////////////
            // Fields

            private bool _idol;
            private bool _run;
            private bool _walk;
            private bool _jump;
            private bool _backward;

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
                var _instance = new DoFixedUpdate();
                return _instance;
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
