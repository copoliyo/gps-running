/*
Copyright (C) 2009 Brendan Doherty

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library. If not, see <http://www.gnu.org/licenses/>.
 */

using System;

namespace GpsRunningPlugin
{
	class GUIDs {
        public static readonly Guid PluginMain = new Guid("489FD22A-DB13-49DB-A77C-57E45CB1D049");
        public static readonly Guid Settings = new Guid("03f2bb00-1214-11e0-ac64-0800200c9a66");
        public static readonly Guid Activity = new Guid("75af74a0-5ec7-11df-a08a-0800200c9a66");
        public static readonly Guid OpenView = new Guid("1dc82ca0-88aa-45a5-a6c6-c25f56ad1fc3");
    }
}
namespace TrailsPlugin
{
    class GUIDs
    {
#if ST_2_1
        public static readonly Guid MapControlLayer = new Guid("03f2bb01-1214-11e0-ac64-0800200c9a66");
#else
        public static readonly Guid TrailPointsControlLayerProvider = new Guid("03f2bb01-1214-11e0-ac64-0800200c9a66");
#endif
    }
}
