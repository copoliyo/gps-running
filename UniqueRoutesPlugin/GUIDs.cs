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
        public static readonly Guid PluginMain = new Guid("5c630517-46c4-478d-89d6-a8a6ca6337db");
        public static readonly Guid Settings = new Guid("9f4625e0-1108-11e0-ac64-0800200c9a66");
        public static readonly Guid Activity = new Guid("0af379d0-5ebe-11df-a08a-0800200c9a66");

        //ST standard views
        public static readonly Guid DailyActivityView = new Guid("1dc82ca0-88aa-45a5-a6c6-c25f56ad1fc3");
        //public static readonly Guid EquipmentView = new Guid("92e1a9b4-de58-11db-9705-00e08161165f");
        //public static readonly Guid AthleteView = new Guid("709e607b-cb51-431c-ba3f-197ab4df3de0");
        //public static readonly Guid ReportView = new Guid("99498256-cf51-11db-9705-005056c00008");
        //public static readonly Guid SettingsView = new Guid("df106ae5-c497-11db-96fe-005056c00008");
        //public static readonly Guid RoutesView = new Guid("e9a99ef8-c497-11db-96fe-005056c00008");
        //public static readonly Guid OnlineView = new Guid("f8a828fc-c497-11db-96fe-005056c00008");
        //public static readonly Guid CategoriesView = new Guid("2cfdc5ac-d8d0-11db-9705-005056c00008");

    }
}
namespace TrailsPlugin
{
    class GUIDs
    {
#if ST_2_1
        public static readonly Guid MapControlLayer = new Guid("9f464cf0-1108-11e0-ac64-0800200c9a66");
#else
        public static readonly Guid TrailPointsControlLayerProvider = new Guid("9f464cf0-1108-11e0-ac64-0800200c9a66");
#endif
    }
}
