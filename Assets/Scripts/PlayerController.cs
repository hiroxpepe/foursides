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

using System;
using UnityEngine;
using static UnityEngine.Vector3;
using UniRx;
using UniRx.Triggers;

namespace Studio.MeowToon {
    /// <summary>
    /// player controller.
    /// @author h.adachi
    /// </summary>
    public class PlayerController : GamepadMaper {
#nullable enable

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

        [SerializeField]
        SimpleAnimation _simpleAnime;

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        DoUpdate _doUpdate;

        DoFixedUpdate _doFixedUpdate;

        Acceleration _acceleration;

        ///////////////////////////////////////////////////////////////////////////////////////////
        // update Methods

        // Awake is called when the script instance is being loaded.
        void Awake() {
            _doUpdate = DoUpdate.GetInstance();
            _doFixedUpdate = DoFixedUpdate.GetInstance();
            _acceleration = Acceleration.GetInstance(_forwardSpeedLimit, _runSpeedLimit, _backwardSpeedLimit);
        }

        // Start is called before the first frame update
        new void Start() {
            base.Start();

            const float POWER = 12.0f;

            var rb = transform.GetComponent<Rigidbody>(); // Rigidbody should be only used in FixedUpdate.

            this.FixedUpdateAsObservable().Subscribe(_ => {
                _acceleration.previousSpeed = _acceleration.currentSpeed;// hold previous speed.
                _acceleration.currentSpeed = rb.velocity.magnitude; // get speed.
            });

            /// <summary>
            /// idol.
            /// </summary>
            this.UpdateAsObservable().Where(_ => _doUpdate.grounded && !_upButton.isPressed && !_downButton.isPressed)
                .Subscribe(_ => {
                    _simpleAnime.Play("Default");
                    _doFixedUpdate.ApplyIdol();
                });

            this.FixedUpdateAsObservable().Where(_ => _doFixedUpdate.idol)
                .Subscribe(_ => {
                    rb.useGravity = true;
                });

            /// <summary>
            /// walk.
            /// </summary>
            this.UpdateAsObservable().Where(_ => _upButton.isPressed && !_yButton.isPressed).Subscribe(_ => {
                if (_doUpdate.grounded) { _simpleAnime.Play("Walk"); }
                _doFixedUpdate.ApplyWalk();
            });

            this.FixedUpdateAsObservable().Where(_ => _doFixedUpdate.walk && _acceleration.walk).Subscribe(_ => {
                rb.AddFor​​ce(transform.forward * POWER * 7.5f, ForceMode.Acceleration);
                _doFixedUpdate.CancelWalk();
            });

            /// <summary>
            /// run.
            /// </summary>
            this.UpdateAsObservable().Where(_ => _upButton.isPressed && _yButton.isPressed).Subscribe(_ => {
                if (_doUpdate.grounded) { _simpleAnime.Play("Run"); }
                _doFixedUpdate.ApplyRun();
            });

            this.FixedUpdateAsObservable().Where(_ => _doFixedUpdate.run && _acceleration.run).Subscribe(_ => {
                rb.AddFor​​ce(transform.forward * POWER * 7.5f, ForceMode.Acceleration);
                _doFixedUpdate.CancelRun();
            });

            /// <summary>
            /// backward.
            /// </summary>
            this.UpdateAsObservable().Where(_ => _downButton.isPressed).Subscribe(_ => {
                if (_doUpdate.grounded) { _simpleAnime.Play("Walk"); }
                _doFixedUpdate.ApplyBackward();
            });

            this.FixedUpdateAsObservable().Where(_ => _doFixedUpdate.backward && _acceleration.backward).Subscribe(_ => {
                rb.AddFor​​ce(-transform.forward * POWER * 7.5f, ForceMode.Acceleration);
                _doFixedUpdate.CancelBackward();
            });

            /// <summary>
            /// jump.
            /// </summary>
            this.UpdateAsObservable().Where(_ => _bButton.wasPressedThisFrame && _doUpdate.grounded).Subscribe(_ => {
                _doUpdate.grounded = false;
                _simpleAnime.Play("Jump");
                _doFixedUpdate.ApplyJump();
            });

            this.FixedUpdateAsObservable().Where(_ => _doFixedUpdate.jump).Subscribe(_ => {
                rb.useGravity = true;
                rb.AddRelativeFor​​ce(up * _jumpPower * POWER * 2, ForceMode.Acceleration);
                _doFixedUpdate.CancelJump();
            });

            /// <summary>
            /// rotate.
            /// </summary>
            this.UpdateAsObservable().Subscribe(_ => {
                var axis = _rightButton.isPressed ? 1 : _leftButton.isPressed ? -1 : 0;
                transform.Rotate(0, axis * (_rotationalSpeed * Time.deltaTime) * POWER, 0);
            });

            /// <summary>
            /// freeze.
            /// </summary>
            this.OnCollisionStayAsObservable().Where(x => x.LikeBlock() && (_upButton.isPressed || _downButton.isPressed) && _acceleration.freeze).Subscribe(_ => {
                var reach = getReach();
                //Debug.Log("reach: " + Math.Round(transform.position.y, 2) % 1); // FIXME:
                if (_doUpdate.grounded && (reach < 0.5d || reach >= 0.99d)) {
                    moveLetfOrRight(getDirection(transform.forward));
                } else if (reach >= 0.5d && reach < 0.99d) {
                    rb.useGravity = false;
                    moveTop();
                }
            });

            /// <summary>
            /// when touching grounds.
            /// </summary>
            this.OnCollisionEnterAsObservable().Where(x => x.LikeGround()).Subscribe(x => {
                _doUpdate.grounded = true;
                rb.useGravity = true;
            });

            /// <summary>
            /// when touching blocks.
            /// </summary>
            this.OnCollisionEnterAsObservable().Where(x => x.LikeBlock()).Subscribe(x => {
                if (!isHitSide(x.gameObject)) {
                    _doUpdate.grounded = true;
                    rb.useGravity = true;
                }
            });

            /// <summary>
            /// when leaving blocks.
            /// </summary>
            this.OnCollisionExitAsObservable().Where(x => x.LikeBlock()).Subscribe(x => {
                rb.useGravity = true;
            });
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// the value until the top of the block.
        /// </summary>
        double getReach() {
            return Math.Round(transform.position.y, 2) % 1; // FIXME:
        }

        /// <summary>
        /// move top when the player hits a block.
        /// </summary>
        void moveTop() {
            const float SPEED = 6.0f;
            transform.position = new(
                transform.position.x,
                transform.position.y + SPEED * Time.deltaTime,
                transform.position.z
            );
        }

        /// <summary>
        /// move aside when the player hits a block.
        /// </summary>
        /// <param name="direction">the player's direction is provided.</param>
        void moveLetfOrRight(Direction direction) {
            const float SPEED = 0.3f;
            Vector3 movePosition = transform.position;
            // z-axis positive and negative.
            if (direction == Direction.PositiveZ || direction == Direction.NegativeZ) {
                if (transform.forward.x < 0f) {
                    movePosition = new(
                        transform.position.x - SPEED * Time.deltaTime,
                        transform.position.y,
                        transform.position.z
                    );
                } else if (transform.forward.x >= 0f) {
                    movePosition = new(
                        transform.position.x + SPEED * Time.deltaTime,
                        transform.position.y,
                        transform.position.z
                    );
                }
            }
            // x-axis positive and negative.
            if (direction == Direction.PositiveX || direction == Direction.NegativeX) {
                if (transform.forward.z < 0f) {
                    movePosition = new(
                        transform.position.x,
                        transform.position.y,
                        transform.position.z - SPEED * Time.deltaTime
                    );
                } else if (transform.forward.z >= 0f) {
                    movePosition = new(
                        transform.position.x,
                        transform.position.y,
                        transform.position.z + SPEED * Time.deltaTime
                    );
                }
            }
            // move to a new position.
            transform.position = movePosition;
        }

        /// <summary>
        /// returns an enum of the player's direction.
        /// </summary>
        Direction getDirection(Vector3 forwardVector) {
            var fX = (float) Math.Round(forwardVector.x);
            var fY = (float) Math.Round(forwardVector.y);
            var fZ = (float) Math.Round(forwardVector.z);
            // z-axis positive.
            if (fX == 0 && fZ == 1) { return Direction.PositiveZ; }
            // z-axis negative.
            if (fX == 0 && fZ == -1) { return Direction.NegativeZ; }
            // x-axis positive.
            if (fX == 1 && fZ == 0) { return Direction.PositiveX; }
            // x-axis negative.
            if (fX == -1 && fZ == 0) { return Direction.NegativeX; }
            // determine the difference between the two axes.
            float abX = Math.Abs(forwardVector.x);
            float abZ = Math.Abs(forwardVector.z);
            if (abX > abZ) {
                // x-axis positive.
                if (fX == 1) { return Direction.PositiveX; }
                // x-axis negative.
                if (fX == -1) { return Direction.NegativeX; }
            } else if (abX < abZ) {
                // z-axis positive.
                if (fZ == 1) { return Direction.PositiveZ; }
                // z-axis negative.
                if (fZ == -1) { return Direction.NegativeZ; }
            }
            return Direction.None; // unknown.
        }

        /// <summary>
        /// whether hits the side of the colliding object.
        /// </summary>
        bool isHitSide(GameObject target) {
            const float ADJUST = 0.1f;
            float targetHeight = target.GetRenderer().bounds.size.y;
            float targetY = target.transform.position.y;
            float targetTop = targetHeight + targetY;
            var y = transform.position.y;
            if (y < (targetTop - ADJUST)) {
                return true;
            } else {
                return false;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

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
                var instance = new DoUpdate();
                instance.ResetState();
                return instance;
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

            public void ApplyIdol() {
                _idol = true;
                _run = _walk = _backward = _jump = false;
            }

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

        #region Acceleration

        class Acceleration {

            ///////////////////////////////////////////////////////////////////////////////////////
            // Fields

            float _currentSpeed;
            float _previousSpeed;
            float _forwardSpeedLimit;
            float _runSpeedLimit;
            float _backwardSpeedLimit;

            ///////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjectives] 

            public float currentSpeed { get => _currentSpeed; set => _currentSpeed = value; }
            public float previousSpeed { get => _previousSpeed; set => _previousSpeed = value; }
            public bool walk { get => _currentSpeed < _forwardSpeedLimit; }
            public bool run { get => _currentSpeed < _runSpeedLimit; }
            public bool backward { get => _currentSpeed < _backwardSpeedLimit; }
            public bool freeze { get {
                if (Math.Round(_previousSpeed, 2) < 0.02 &&
                    Math.Round(_currentSpeed, 2) < 0.02 &&
                    Math.Round(_previousSpeed, 2) == Math.Round(_currentSpeed, 2)) {
                    return true;
                }
                return false;
            }}

            ///////////////////////////////////////////////////////////////////////////////////////
            // Constructor

            /// <summary>
            /// hide the constructor.
            /// </summary>
            Acceleration(float forwardSpeedLimit, float runSpeedLimit, float backwardSpeedLimit) {
                _forwardSpeedLimit = forwardSpeedLimit;
                _runSpeedLimit = runSpeedLimit;
                _backwardSpeedLimit = backwardSpeedLimit;
            }

            /// <summary>
            /// returns an initialized instance.
            /// </summary>
            public static Acceleration GetInstance(float forwardSpeedLimit, float runSpeedLimit, float backwardSpeedLimit) {
                return new Acceleration(forwardSpeedLimit, runSpeedLimit, backwardSpeedLimit);
            }
        }

        #endregion
    }
}
