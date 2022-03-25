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

using System.Linq;
using UnityEngine;
using static UnityEngine.Vector3;
using UniRx;
using UniRx.Triggers;

namespace Studio.MeowToon {
    /// <summary>
    /// camera controller.
    /// @author h.adachi
    /// </summary>
    public class CameraSystem : GamepadMaper {
#nullable enable

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // References

        [SerializeField]
        GameObject _horizontalAxis;

        [SerializeField]
        GameObject _verticalAxis;

        [SerializeField]
        GameObject _mainCamera;

        [SerializeField]
        GameObject _lookTarget;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Vector3 _defaultLocalPosition;

        Quaternion _defaultLocalRotation;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // update Methods

        // Start is called before the first frame update
        new void Start() {
            base.Start();

            /// <summary>
            /// hold the default position and rotation of the camera.
            /// </summary>
            _defaultLocalPosition = transform.localPosition;
            _defaultLocalRotation = transform.localRotation;

            /// <summary>
            /// rotate the camera view.
            /// </summary>
            this.UpdateAsObservable().Subscribe(_ => {
                rotateView();
            });

            /// <summary>
            /// reset the camera view.
            /// </summary>
            this.UpdateAsObservable().Where(_ => _rightStickButton.wasPressedThisFrame).Subscribe(_ => {
                resetRotateView();
            });

            /// <summary>
            /// when touching the back wall.
            /// </summary>
            this.OnTriggerEnterAsObservable().Where(x => x.LikeWall()).Subscribe(x => {
                var materialList = x.gameObject.GetMeshRenderer().materials.ToList();
                materialList.ForEach(material => {
                    material.ToOpaque();
                });
            });

            /// <summary>
            /// when leaving the back wall.
            /// </summary>
            this.OnTriggerExitAsObservable().Where(x => x.LikeWall()).Subscribe(x => {
                var materialList = x.gameObject.GetMeshRenderer().materials.ToList();
                materialList.ForEach(material => {
                    material.ToTransparent();
                });
            });

            /// <summary>
            /// when touching the block.
            /// </summary>
            this.OnTriggerEnterAsObservable().Where(x => x.LikeBlock()).Subscribe(x => {
                var materialList = x.gameObject.GetMeshRenderer().materials.ToList();
                materialList.ForEach(material => {
                    material.ToOpaque();
                });
            });

            /// <summary>
            /// when leaving the block.
            /// </summary>
            this.OnTriggerExitAsObservable().Where(x => x.LikeBlock()).Subscribe(x => {
                var materialList = x.gameObject.GetMeshRenderer().materials.ToList();
                materialList.ForEach(material => {
                    material.ToTransparent();
                });
            });
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// rotate the camera view.
        /// </summary>
        void rotateView() {
            const float ADJUST = 120.0f;
            var playerPosition = transform.parent.gameObject.transform.position;
            // up.
            if (_rightStickUpButton.isPressed) {
                transform.RotateAround(playerPosition, right, 1.0f * ADJUST * Time.deltaTime);
                transform.LookAt(_lookTarget.transform);
            }
            // down.
            else if (_rightStickDownButton.isPressed) {
                transform.RotateAround(playerPosition, right, -1.0f * ADJUST * Time.deltaTime);
                transform.LookAt(_lookTarget.transform);
            }
            // left.
            else if (_rightStickLeftButton.isPressed) {
                transform.RotateAround(playerPosition, up, 1.0f * ADJUST * Time.deltaTime);
            }
            // right
            else if (_rightStickRightButton.isPressed) {
                transform.RotateAround(playerPosition, up, -1.0f * ADJUST * Time.deltaTime);
            }
        }

        /// <summary>
        /// reset the camera view.
        /// </summary>
        void resetRotateView() {
            transform.localPosition = _defaultLocalPosition;
            transform.localRotation = _defaultLocalRotation; 
        }
    }
}
