﻿using Routes = QSP.RouteFinding.Routes;

namespace QSP.UI.Views.Route.Actions
{
    /// <summary>
    /// This inteface should be implemented for any view that supports ActionContextMenu.
    /// </summary>
    public interface ISupportActionContextMenu
    {
        string DistanceInfo { set; }
        string Route { get; set; }

        void ShowMap(Routes.Route route);
        void ShowMapBrowser(Routes.Route route);
        void ShowInfo(string info);
        void ShowWarning(string warning);
    }
}