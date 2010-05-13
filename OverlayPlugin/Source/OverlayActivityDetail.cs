/*
Copyright (C) 2007, 2008 Kristian Bisgaard Lassen
Copyright (C) 2010 Kristian Helkjaer Lassen

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
using System.Collections.Generic;
using System.Text;
using ZoneFiveSoftware.Common.Visuals.Fitness;
#if !ST_2_1
using ZoneFiveSoftware.Common.Visuals;
#endif

namespace SportTracksOverlayPlugin.Source
{
    class OverlayActivityDetail : IExtendActivityDetailPages
    {
        #region IExtendActivityDetailPages Members

#if ST_2_1
        public IList<IActivityDetailPage> ActivityDetailPages
        {
            get
            {
                return new IActivityDetailPage[] { new OverlayActivityDetailPage() };
            }
        }
#else
        public IList<IDetailPage> GetDetailPages(IDailyActivityView view, ExtendViewDetailPages.Location location)
        {
            return new IDetailPage[] { new OverlayActivityDetailPage(view) };
        }
#endif

        #endregion
    }
}
