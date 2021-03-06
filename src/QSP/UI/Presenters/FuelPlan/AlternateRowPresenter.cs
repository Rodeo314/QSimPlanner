﻿using QSP.Common.Options;
using QSP.LibraryExtension;
using QSP.RouteFinding.Airports;
using QSP.RouteFinding.AirwayStructure;
using QSP.RouteFinding.Containers.CountryCode;
using QSP.RouteFinding.Routes;
using QSP.RouteFinding.TerminalProcedures.Star;
using QSP.RouteFinding.Tracks;
using QSP.UI.Models.FuelPlan;
using QSP.UI.Models.FuelPlan.Routes;
using QSP.UI.Presenters.FuelPlan.Routes;
using QSP.UI.Views.FuelPlan;
using QSP.UI.Views.FuelPlan.Routes;
using QSP.WindAloft;
using System;
using System.Collections.Generic;

namespace QSP.UI.Presenters.FuelPlan
{
    public class AlternateRowPresenter : IRefreshForNavDataChange
    {
        public IAlternateRowView View { get; private set; }
        public ActionContextMenuPresenter ContextMenuPresenter { get; private set; }

        private Locator<AppOptions> appOptions;
        private AirwayNetwork airwayNetwork;
        private ISelectedProcedureProvider destController;

        public FindAltnPresenter FindAltnPresenter(IFindAltnView altnView) =>
            new FindAltnPresenter(altnView, airwayNetwork.AirportList);

        public string DestIcao => destController.Icao;
        public RouteGroup Route => ContextMenuPresenter.Route;

        /// <exception cref="Exception"></exception>
        public List<string> GetAllProcedures()
        {
            var wptList = airwayNetwork.WptList;
            var handler = StarHandlerFactory.GetHandler(
                View.Icao,
                appOptions.Instance.NavDataLocation,
                wptList,
                new WaypointListEditor(wptList),
                airwayNetwork.AirportList);

            return handler.StarCollection.GetStarList(View.Rwy);
        }

        public AlternateRowPresenter(
            IAlternateRowView view,
            Locator<AppOptions> appOptionsLocator,
            AirwayNetwork airwayNetwork,
            ISelectedProcedureProvider destController,
            Locator<CountryCodeCollection> checkedCodesLocator,
            Func<AvgWindCalculator> windCalcGetter)
        {
            this.View = view;

            ContextMenuPresenter = new ActionContextMenuPresenter(
                view,
                appOptionsLocator,
                airwayNetwork,
                destController,
                view,
                checkedCodesLocator,
                windCalcGetter);

            this.appOptions = appOptionsLocator;
            this.airwayNetwork = airwayNetwork;
            this.destController = destController;
        }

        public void FindRoute() => ContextMenuPresenter.FindRoute();
        public void ExportRouteFiles() => ContextMenuPresenter.ExportRouteFiles();
        public void AnalyzeRoute() => ContextMenuPresenter.AnalyzeRoute();
        public void ShowMap() => ContextMenuPresenter.ShowMap();
        public void ShowMapBrowser() => ContextMenuPresenter.ShowMapBrowser();

        public void UpdateRunways()
        {
            RunwaySelect.UpdateRunways(
                () => View.Icao,
                rwys => View.RunwayList = rwys,
                rwy => View.Rwy = rwy,
                airwayNetwork.AirportList);
        }

        public void OnNavDataChange()
        {
            var rwy = View.Rwy;
            UpdateRunways();
            View.Rwy = rwy;

            ContextMenuPresenter.OnNavDataChange();
        }
    }
}
