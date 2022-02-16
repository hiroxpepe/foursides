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

namespace Studio.MeowToon {
    /// <summary>
    /// generic extension method.
    /// @author h.adachi
    /// </summary>
    public static class Extensions {

        #region type of object.

        /// <summary>
        /// whether the GameObject's name contains "Ground".
        /// </summary>
        public static bool LikeGround(this GameObject self) {
            return self.name.Contains("Ground");
        }

        /// <summary>
        /// whether the Transform's name contains "Ground".
        /// </summary>
        public static bool LikeGround(this Transform self) {
            return self.name.Contains("Ground");
        }

        /// <summary>
        /// whether the Collision's name contains "Ground".
        /// </summary>
        public static bool LikeGround(this Collision self) {
            return self.gameObject.name.Contains("Ground");
        }

        #endregion
    }
}
