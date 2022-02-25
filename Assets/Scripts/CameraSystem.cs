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
        // Fields

        [SerializeField]
        GameObject _mainCamera;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // update Methods

        // Start is called before the first frame update
        new void Start() {
            base.Start();

            // when touching the back wall.
            this.OnTriggerEnterAsObservable().Where(x => x.LikeWall()).Subscribe(x => {
                var materialList = x.gameObject.GetMeshRenderer().materials.ToList();
                materialList.ForEach(material => {
                    material.ToOpaque();
                });
            });

            // when leaving the back wall.
            this.OnTriggerExitAsObservable().Where(x => x.LikeWall()).Subscribe(x => {
                var materialList = x.gameObject.GetMeshRenderer().materials.ToList();
                materialList.ForEach(material => {
                    material.ToTransparent();
                });
            });
        }
    }
}
