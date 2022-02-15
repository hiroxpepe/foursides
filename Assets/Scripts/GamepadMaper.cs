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
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UniRx;
using UniRx.Triggers;

namespace Examproject {
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
