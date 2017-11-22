﻿using CommonLibrary.AviationTools;
using CommonLibrary.LibraryExtension;
using QSP.AircraftProfiles.Configs;
using QSP.Common;
using QSP.Common.Options;
using QSP.FuelCalculation;
using QSP.FuelCalculation.Calculations;
using QSP.FuelCalculation.FuelData;
using QSP.FuelCalculation.Results;
using QSP.LibraryExtension;
using QSP.Metar;
using QSP.RouteFinding.Airports;
using QSP.RouteFinding.Containers.CountryCode;
using QSP.RouteFinding.Data.Interfaces;
using QSP.RouteFinding.Routes;
using QSP.RouteFinding.TerminalProcedures;
using QSP.RouteFinding.Tracks;
using QSP.UI.Controllers;
using QSP.UI.Controllers.Units;
using QSP.UI.Controllers.WeightControl;
using QSP.UI.Models;
using QSP.UI.Models.FuelPlan;
using QSP.UI.Models.MsgBox;
using QSP.UI.Presenters.FuelPlan;
using QSP.UI.Presenters.FuelPlan.Route;
using QSP.UI.UserControls;
using QSP.UI.UserControls.TakeoffLanding.Common;
using QSP.UI.Util;
using QSP.UI.Util.ScrollBar;
using QSP.UI.Views.Route;
using QSP.UI.Views.Route.Actions;
using QSP.Utilities.Units;
using QSP.WindAloft;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static QSP.AviationTools.Constants;
using static QSP.AviationTools.SpeedConversion;
using static QSP.MathTools.Numbers;
using static QSP.UI.Util.MsgBoxHelper;
using static QSP.UI.Views.Factories.FormFactory;
using static QSP.Utilities.LoggerInstance;
using static QSP.Utilities.Units.Conversions;

namespace QSP.UI.Views.FuelPlan
{
    // The implementation of ISupportActionContextMenu is used to support the actions 
    // for the route from origin to destination.

    public partial class FuelPlanningControl : UserControl, IFuelPlanningView,
        ISupportActionContextMenu, IRefreshForOptionChange
    {
        private AirwayNetwork airwayNetwork;
        private Locator<AppOptions> appOptionsLocator;
        private ProcedureFilter procFilter;
        private Locator<IWindTableCollection> windTableLocator;
        private Locator<CountryCodeManager> countryCodeLocator;
        private Locator<CountryCodeCollection> checkedCodesLocator;

        private RouteFinderSelection origController;
        private RouteFinderSelection destController;
        private DestinationSidSelection destSidProvider;
        private AdvancedRouteTool advancedRouteTool;
        private AcConfigManager aircrafts;
        private IEnumerable<FuelData> fuelData;
        private ActionContextMenu routeActionMenu;
        private RouteOptionContextMenu routeOptionMenu;
        private MetarCache metarCache;
        //   private ISupportActionContextMenu origMenu;
        // private ISupportActionContextMenu destMenu;

        public WeightController WeightControl { get; private set; }
        public WeightTextBoxController Extra { get; private set; }
        public AlternatePresenter AltnPresenter { get; private set; }

        // Do not set the values of these controllers directly. 
        // Use WeightControl to interact with the weights.
        private WeightTextBoxController oew;
        private WeightTextBoxController payload;
        public WeightTextBoxController Zfw { get; private set; }

        /// <summary>
        /// After the fuel calculation completes, the user can press 'request' button
        /// in takeoff or landing calculation page to automatically fill some paramters of the 
        /// selected aircraft. 
        /// 
        /// Returns null if not available.
        /// </summary>
        public AircraftRequest AircraftRequest { get; private set; }

        public event EventHandler AircraftRequestChanged;


        public WeightUnit WeightUnit
        {
            get => (WeightUnit)wtUnitComboBox.SelectedIndex;

            set => wtUnitComboBox.SelectedIndex = (int)value;
        }

        public IEnumerable<string> AircraftList { set => throw new NotImplementedException(); }
        public IEnumerable<string> RegistrationList { set => throw new NotImplementedException(); }
        public double OewKg { set => throw new NotImplementedException(); }
        public double MaxZfwKg { set => throw new NotImplementedException(); }

        public string OrigIcao => origController.Icao;
        public string DestIcao => destController.Icao;

        public IEnumerable<string> OrigRwyList { set => throw new NotImplementedException(); }
        public IEnumerable<string> DestRwyList { set => throw new NotImplementedException(); }
        public IEnumerable<string> SidList { set => throw new NotImplementedException(); }
        public IEnumerable<string> StarList { set => throw new NotImplementedException(); }
        public string Route { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private AirportManager AirportList => airwayNetwork.AirportList;
        private AppOptions AppOptions => appOptionsLocator.Instance;
        private RouteGroup RouteToDest => routeActionMenu.Route;

        public string DistanceInfo { set => throw new NotImplementedException(); }

        public FuelPlanningControl()
        {
            InitializeComponent();
        }

        public void Init(
            Locator<AppOptions> appOptionsLocator,
            AirwayNetwork airwayNetwork,
            ProcedureFilter procFilter,
            Locator<CountryCodeManager> countryCodeLocator,
            Locator<IWindTableCollection> windTableLocator,
            AcConfigManager aircrafts,
            IEnumerable<FuelData> fuelData,
            MetarCache metarCache)
        {
            this.appOptionsLocator = appOptionsLocator;
            this.airwayNetwork = airwayNetwork;
            this.procFilter = procFilter;
            this.countryCodeLocator = countryCodeLocator;
            this.windTableLocator = windTableLocator;
            this.aircrafts = aircrafts;
            this.fuelData = fuelData;
            this.metarCache = metarCache;
            checkedCodesLocator = new CountryCodeCollection().ToLocator();

            SetDefaultState();
            SetOrigDestControllers();

            AltnPresenter = new AlternatePresenter(
                alternateControl, appOptionsLocator, airwayNetwork, windTableLocator,
                destSidProvider, GetFuelData, GetZfwTon, () => OrigIcao, () => DestIcao);

            SetRouteOptionControl();
            SetRouteActionControl();
            SetWeightController();
            SetAircraftSelection();
            SetBtnColorStyles();
            AddMetarCacheEvents();

            wtUnitComboBox.SelectedIndex = 0;
            SubscribeEventHandlers();
            advancedRouteTool = new AdvancedRouteTool();
            advancedRouteTool.Init(
                appOptionsLocator,
                airwayNetwork,
                countryCodeLocator,
                checkedCodesLocator,
                procFilter,
                () => GetWindCalculator());

            if (acListComboBox.Items.Count > 0) acListComboBox.SelectedIndex = 0;

            LoadSavedState();
        }

        private void AddMetarCacheEvents()
        {
            Func<string, Task> updateCache = async (icao) =>
            {
                if (AirportList[icao] != null && !metarCache.Contains(icao))
                {
                    string metar = null;
                    if (await Task.Run(() => MetarDownloader.TryGetMetar(icao, out metar)))
                    {
                        metarCache.AddOrUpdateItem(icao, MetarCacheItem.Create(metar));
                    }
                }
            };

            new[] { origTxtBox, destTxtBox }.ForEach(i =>
            {
                i.TextChanged += async (s, e) => await updateCache(Icao.TrimIcao(i.Text));
            });

            AltnPresenter.AlternatesChanged += (s, e) =>
             {
                 AltnPresenter.Alternates.ForEach(async a => await updateCache(Icao.TrimIcao(a)));
             };
        }

        private void SetBtnColorStyles()
        {
            var style = ButtonColorStyle.Default;
            var filterSidStyle = new ControlDisableStyleController(filterSidBtn, style);
            var filterStarStyle = new ControlDisableStyleController(filterStarBtn, style);

            filterSidStyle.Activate();
            filterStarStyle.Activate();
        }

        private void SetRouteOptionControl()
        {
            routeOptionMenu = new RouteOptionContextMenu(checkedCodesLocator, countryCodeLocator);

            routeOptionMenu.Subscribe();
            routeOptionBtn.Click += (s, e) =>
                routeOptionMenu.Show(routeOptionBtn, new Point(0, routeOptionBtn.Height));
        }

        private void SetRouteActionControl()
        {
            routeActionMenu = new ActionContextMenu();

            var presenter = new ActionContextMenuPresenter(
                this,
                appOptionsLocator,
                airwayNetwork,
                origController,
                destController,
                checkedCodesLocator,
                () => GetWindCalculator()); // TODO: move this method

            showRouteActionsBtn.Click += (s, e) =>
               routeActionMenu.Show(showRouteActionsBtn, new Point(0, showRouteActionsBtn.Height));
        }

        public void OnWptListChanged()
        {
            advancedRouteTool.OnWptListChanged();
        }

        private void SubscribeEventHandlers()
        {
            wtUnitComboBox.SelectedIndexChanged += WtUnitChanged;
            acListComboBox.SelectedIndexChanged += RefreshRegistrations;
            registrationComboBox.SelectedIndexChanged += RegistrationChanged;
            calculateBtn.Click += Calculate;
            advancedToolLbl.Click += ShowAdvancedTool;
            mainRouteRichTxtBox.UpperCaseOnly();
        }

        private string[] AvailAircraftTypes()
        {
            var allProfileNames = fuelData.Select(t => t.ProfileName).ToHashSet();

            return aircrafts
                .Aircrafts
                .Where(c => allProfileNames.Contains(c.Config.FuelProfile))
                .Select(c => c.Config.AC)
                .Distinct()
                .OrderBy(i => i)
                .ToArray();
        }

        private void SetAircraftSelection()
        {
            acListComboBox.Items.Clear();
            acListComboBox.Items.AddRange(AvailAircraftTypes());
        }

        private void SetDefaultState()
        {
            routeDisLbl.Text = "";
            FinalReserveTxtBox.Text = "30";
            ContPercentComboBox.Text = "5";
            extraFuelTxtBox.Text = "0";
            ApuTimeTxtBox.Text = "30";
            TaxiTimeTxtBox.Text = "20";
            HoldTimeTxtBox.Text = "0";
        }

        private void LoadSavedState()
        {
            try
            {
                new FuelPageState(this).LoadFromFile();
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        public void SaveStateToFile()
        {
            try
            {
                new FuelPageState(this).SaveToFile();
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        private void SetOrigDestControllers()
        {
            origController = new RouteFinderSelection(
               origTxtBox,
               true,
               origRwyComboBox,
               sidComboBox,
               filterSidBtn,
               this,
               appOptionsLocator,
               () => airwayNetwork.AirportList,
               () => airwayNetwork.WptList,
               procFilter);

            destController = new RouteFinderSelection(
                destTxtBox,
                false,
                destRwyComboBox,
                starComboBox,
                filterStarBtn,
                this,
                appOptionsLocator,
                () => airwayNetwork.AirportList,
                () => airwayNetwork.WptList,
                procFilter);

            destSidProvider = new DestinationSidSelection(destController);

            origController.Subscribe();
            destController.Subscribe();
        }

        private void WtUnitChanged(object sender, EventArgs e)
        {
            var unit = WeightUnit;
            oew.Unit = unit;
            payload.Unit = unit;
            Zfw.Unit = unit;
            Extra.Unit = unit;
        }

        private void SetWeightController()
        {
            oew = new WeightTextBoxController(oewTxtBox, oewLbl);
            payload = new WeightTextBoxController(payloadTxtBox, payloadLbl);
            Zfw = new WeightTextBoxController(zfwTxtBox, zfwLbl);
            Extra = new WeightTextBoxController(extraFuelTxtBox, extraFuelLbl);

            WeightControl = new WeightController(oew, payload, Zfw, payloadTrackBar);
            WeightControl.Enable();
        }

        private bool FuelProfileExists(string profileName)
        {
            return fuelData.Any(c => c.ProfileName == profileName);
        }

        private void RefreshRegistrations(object sender, EventArgs e)
        {
            if (acListComboBox.SelectedIndex >= 0)
            {
                var ac = aircrafts.FindAircraft(acListComboBox.Text);
                var items = registrationComboBox.Items;
                items.Clear();

                items.AddRange(
                    ac.Where(c => FuelProfileExists(c.Config.FuelProfile))
                      .Select(c => c.Config.Registration)
                      .ToArray());

                if (items.Count > 0)
                {
                    registrationComboBox.SelectedIndex = 0;
                }
            }
        }

        private void RegistrationChanged(object sender, EventArgs e)
        {
            if (registrationComboBox.SelectedIndex < 0) return;

            var config = aircrafts.Find(registrationComboBox.Text).Config;
            WeightUnit = config.WtUnit;
            WeightControl.SetAircraftWeights(config.OewKg, config.MaxZfwKg);
            var maxPayloadKg = config.MaxZfwKg - config.OewKg;
            WeightControl.ZfwKg = config.OewKg + 0.5 * maxPayloadKg;
        }

        /// <summary>
        /// Gets the fuel data of currently selected aircraft.
        /// Returns null if no aircraft exists in ComboBox.
        /// </summary>
        public FuelDataItem GetFuelData()
        {
            var dataName = GetCurrentAircraft().Config.FuelProfile;
            return fuelData.First(d => d.ProfileName == dataName).Data;
        }

        /// <summary>
        /// Returns null if no aircraft exists in ComboBox.
        /// </summary>
        public AircraftConfig GetCurrentAircraft()
        {
            if (registrationComboBox.SelectedIndex < 0) return null;
            return aircrafts.Find(registrationComboBox.Text);
        }

        private void Calculate(object sender, EventArgs e)
        {
            fuelReportTxtBox.ForeColor = Color.Black;
            fuelReportTxtBox.Text = "";

            var validator = new FuelParameterValidator(this);
            FuelParameters para = null;

            try
            {
                para = validator.Validate();
            }
            catch (InvalidUserInputException ex)
            {
                ShowMessage(ex.Message, MessageLevel.Warning);
                return;
            }

            var altnRoutes = AltnPresenter.Routes;

            if (altnRoutes.Any(r => r == null))
            {
                ShowMessage("All alternate routes must be entered.", MessageLevel.Warning);
                return;
            }

            if (RouteToDest == null)
            {
                ShowMessage("Route to destination must be entered.", MessageLevel.Warning);
                return;
            }

            var windTables = windTableLocator.Instance;

            if (windTables is DefaultWindTableCollection)
            {
                var result = this.ShowDialog(
                    "The wind data has not been downloaded. " +
                    "Continue to calculate and ignore wind aloft?",
                    MsgBoxIcon.Info,
                    "",
                    DefaultButton.Button1,
                    "Yes", "No", "Cancel");

                if (result != MsgBoxResult.Button1) return;
            }

            FuelReport fuelReport = null;

            try
            {
                fuelReport = new FuelReportGenerator(
                    AirportList,
                    new BasicCrzAltProvider(),
                    windTables,
                    RouteToDest.Expanded,
                    AltnPresenter.Routes.Select(r => r.Expanded),
                    para).Generate();
            }
            catch (InvalidPlanAltitudeException)
            {
                ShowMessage("Cannot find a valid cruising altitude.", MessageLevel.Warning);
                return;
            }

            var ac = GetCurrentAircraft().Config;

            if (fuelReport.TotalFuel > ac.MaxFuelKg)
            {
                var msg = InsufficientFuelMsg(fuelReport.TotalFuel, ac.MaxFuelKg, WeightUnit);
                this.ShowInfo(msg, "Insufficient fuel");
                return;
            }

            string outputText = fuelReport.ToString(WeightUnit);
            fuelReportTxtBox.Text = "\n" + outputText.ShiftToRight(20);

            AircraftRequest = new AircraftRequest(
                acListComboBox.Text,
                registrationComboBox.Text,
                para.Zfw + fuelReport.TakeoffFuel,
                para.Zfw + fuelReport.PredictedLdgFuel,
                para.Zfw,
                WeightUnit);

            AircraftRequestChanged?.Invoke(this, EventArgs.Empty);
            SaveStateToFile();
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            var panel = this.FindPanel().FindPanel();
            var scroll = panel.VerticalScroll;
            var target = 1 + scroll.Maximum - scroll.LargeChange;
            ScrollAnimation.ScrollToPosition(scroll, target, 500.0);
        }

        private static string InsufficientFuelMsg(
            double fuelReqKg, double fuelCapacityKg, WeightUnit unit)
        {
            int fuelReqInt, fuelCapacityInt;
            string wtUnit = WeightUnitToString(unit);

            if (unit == WeightUnit.KG)
            {
                fuelReqInt = RoundToInt(fuelReqKg);
                fuelCapacityInt = RoundToInt(fuelCapacityKg);
            }
            else // WeightUnit.LB
            {
                fuelReqInt = RoundToInt(fuelReqKg * KgLbRatio);
                fuelCapacityInt = RoundToInt(fuelCapacityKg * KgLbRatio);
            }

            return
                $"Fuel required for this flight is {fuelReqInt} {wtUnit}. " +
                $"Maximum fuel tank capacity is {fuelCapacityInt} {wtUnit}.";
        }

        private void ShowAdvancedTool(object sender, EventArgs e)
        {
            var size = advancedRouteTool.Size;
            var newSize = new Size(size.Width + 25, size.Height + 40);

            using (var frm = GetForm(newSize))
            {
                frm.Owner = this.ParentForm;
                frm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                frm.Controls.Add(advancedRouteTool);

                // Remove control from form so that it is not disposed
                // when form closes.
                frm.FormClosing += (_s, _e) => frm.Controls.Remove(advancedRouteTool);

                frm.ShowDialog();
            }
        }

        /// <summary>
        /// Refresh the aircraft and registration comboBoxes,
        /// after the AcConfigManager is updated.
        /// </summary>
        public void RefreshAircrafts(object sender, EventArgs e)
        {
            // Set the selected aircraft/registration.
            string ac = acListComboBox.Text;
            string reg = registrationComboBox.Text;

            SetAircraftSelection();

            if (acListComboBox.Items.Count > 0)
            {
                acListComboBox.SelectedIndex = 0;
            }

            acListComboBox.Text = ac;
            registrationComboBox.Text = reg;
        }

        private AvgWindCalculator GetWindCalculator()
        {
            return GetWindCalculator(AppOptions, windTableLocator, AirportList, GetFuelData(),
                GetZfwTon(), origTxtBox.Text, destTxtBox.Text);
        }

        /// <exception cref="InvalidOperationException"></exception>
        private double GetZfwTon()
        {
            try
            {
                return Zfw.GetWeightKg() / 1000.0;
            }
            catch (InvalidOperationException)
            {
                throw new InvalidUserInputException("Please enter a valid ZFW.");
            }
        }

        /// <summary>
        /// Get AvgWindCalculator to approximate the wind.
        /// Returns null if user disabled wind optimization.
        /// </summary>
        /// <exception cref="InvalidUserInputException"></exception>
        public static AvgWindCalculator GetWindCalculator(
            AppOptions appSettings,
            Locator<IWindTableCollection> windTableLocator,
            AirportManager airportList,
            FuelDataItem fuelData,
            double zfwTon,
            string orig,
            string dest)
        {
            if (!appSettings.EnableWindOptimizedRoute) return null;

            if (windTableLocator.Instance is DefaultWindTableCollection)
            {
                throw new InvalidUserInputException(
                    "Wind data has not been downloaded or loaded from file.\n" +
                    "If you do not want to use wind-optimized route, it can be disabled " +
                    "from Options > Route.");
            }

            if (fuelData == null)
            {
                throw new InvalidUserInputException("No aircraft is selected.");
            }

            var origin = airportList[orig.Trim().ToUpper()];

            if (orig == null)
            {
                throw new InvalidUserInputException("Cannot find origin airport.");
            }

            var destination = airportList[dest.ToUpper()];

            if (dest == null)
            {
                throw new InvalidUserInputException("Cannot find destination airport.");
            }

            var dis = origin.Distance(destination);
            var alt = fuelData.EstimatedCrzAlt(dis, zfwTon);
            var tas = Ktas(fuelData.CruiseKias(zfwTon), alt);

            return new AvgWindCalculator(windTableLocator.Instance, tas, alt);
        }

        public void RefreshForAirportListChange()
        {
            origController.RefreshRwyComboBox();
            destController.RefreshRwyComboBox();
            AltnPresenter.RefreshForAirportListChange();
        }

        public void RefreshForNavDataLocationChange()
        {
            origController.RefreshProcedureComboBox();
            destController.RefreshProcedureComboBox();
            AltnPresenter.RefreshForNavDataLocationChange();
        }

        public void ShowMap(RouteFinding.Routes.Route route) =>
            ShowMapHelper.ShowMap(route, ParentForm.Size, ParentForm);

        public void ShowMapBrowser(RouteFinding.Routes.Route route) =>
            ShowMapHelper.ShowMap(route, ParentForm.Size, ParentForm, true, true);

        public void ShowMessage(string s, MessageLevel lvl) => ParentForm.ShowMessage(s, lvl);
    }
}
