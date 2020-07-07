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

        protected ButtonControl aButton;

        protected ButtonControl bButton;

        protected ButtonControl xButton;

        protected ButtonControl yButton;

        protected ButtonControl upButton;

        protected ButtonControl downButton;

        protected ButtonControl leftButton;

        protected ButtonControl rightButton;

        ///////////////////////////////////////////////////////////////////////////////////////////
        // update Methods

        // Start is called before the first frame update
        protected void Start() {
            this.UpdateAsObservable().Subscribe(_ => {
                mapGamepad();
            });
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // private methods

        private void mapGamepad() {
            // Identifies the OS.
            upButton = Gamepad.current.dpad.up;
            downButton = Gamepad.current.dpad.down;
            leftButton = Gamepad.current.dpad.left;
            rightButton = Gamepad.current.dpad.right;
            if (Application.platform == RuntimePlatform.Android) {
                // Android OS
                aButton = Gamepad.current.aButton;
                bButton = Gamepad.current.bButton;
                xButton = Gamepad.current.xButton;
                yButton = Gamepad.current.yButton;
            } else if (Application.platform == RuntimePlatform.WindowsPlayer) {
                // Windows OS
                aButton = Gamepad.current.bButton;
                bButton = Gamepad.current.aButton;
                xButton = Gamepad.current.yButton;
                yButton = Gamepad.current.xButton;
            } else {
                // FIXME: can't get it during development with Unity?
                aButton = Gamepad.current.bButton;
                bButton = Gamepad.current.aButton;
                xButton = Gamepad.current.yButton;
                yButton = Gamepad.current.xButton;
            }
        }
    }

}

