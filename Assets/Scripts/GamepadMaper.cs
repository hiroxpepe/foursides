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
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UniRx;
using UniRx.Triggers;

namespace Studio.MeowToon {
    /// <summary>
    /// to map physical Gamepad.
    /// @author h.adachi
    /// </summary>
    public class GamepadMaper : MonoBehaviour {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        protected ButtonControl _aButton;

        protected ButtonControl _bButton;

        protected ButtonControl _xButton;

        protected ButtonControl _yButton;

        protected ButtonControl _upButton;

        protected ButtonControl _downButton;

        protected ButtonControl _leftButton;

        protected ButtonControl _rightButton;

        ///////////////////////////////////////////////////////////////////////////////////////////
        // update Methods

        // Start is called before the first frame update
        protected void Start() {
            this.UpdateAsObservable().Subscribe(_ => {
                mapGamepad();
            });
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // private Methods

        private void mapGamepad() {
            // Identifies the OS.
            _upButton = Gamepad.current.dpad.up;
            _downButton = Gamepad.current.dpad.down;
            _leftButton = Gamepad.current.dpad.left;
            _rightButton = Gamepad.current.dpad.right;
            if (Application.platform == RuntimePlatform.Android) {
                // Android OS
                _aButton = Gamepad.current.aButton;
                _bButton = Gamepad.current.bButton;
                _xButton = Gamepad.current.xButton;
                _yButton = Gamepad.current.yButton;
            } else if (Application.platform == RuntimePlatform.WindowsPlayer) {
                // Windows OS
                _aButton = Gamepad.current.bButton;
                _bButton = Gamepad.current.aButton;
                _xButton = Gamepad.current.yButton;
                _yButton = Gamepad.current.xButton;
            } else {
                // FIXME: can't get it during development with Unity?
                _aButton = Gamepad.current.bButton;
                _bButton = Gamepad.current.aButton;
                _xButton = Gamepad.current.yButton;
                _yButton = Gamepad.current.xButton;
            }
        }
    }
}
