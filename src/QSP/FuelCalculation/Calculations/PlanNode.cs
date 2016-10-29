﻿using System;
using System.Collections.Generic;
using QSP.FuelCalculation.Results;
using QSP.FuelCalculation.Results.Nodes;
using QSP.MathTools.Vectors;
using QSP.RouteFinding.Containers;
using QSP.RouteFinding.Data.Interfaces;
using QSP.RouteFinding.Routes;
using QSP.WindAloft;
using static QSP.AviationTools.SpeedConversion;
using static QSP.WindAloft.GroundSpeedCalculation;
using System.Linq;
using QSP.Common;

namespace QSP.FuelCalculation.Calculations
{
    public class PlanNode
    {
        // Remember to update Coordinate property getter if this is changed.
        // All allowed types of NodeValue must return the correct ICoordinate.
        public static readonly IReadOnlyList<Type> AllowedNodeTypes = new[]
        {
            typeof(RouteNode) ,
            typeof(IntermediateNode),
            typeof(TocNode),
            typeof(TodNode)
        };

        public object NodeValue { get; }

        // Variable units:
        // Altitude: ft
        // Time: min
        // Distance: nm
        // Speed: knots
        // Climb/Descent rate: ft/min
        // Weight: kg
        // Fuel amount: kg
        // Fuel flow: kg/min

        // Here 'previous' and 'next' refers to the order of nodes/waypoints
        // in route. Do not confuse with the order of calculation although some
        // classes like InitialPlanCreator computes the flight plan backwards.

        // These are passed in via ctor.
        public IWindTableCollection WindTable { get; }
        public Waypoint PrevWaypoint { get; }
        public LinkedListNode<RouteNode> NextRouteNode { get; }
        public ICoordinate NextPlanNodeCoordinate { get; }
        public double Alt { get; }
        public double GrossWt { get; }
        public double FuelOnBoard { get; }
        public double TimeRemaining { get; }
        public double Kias { get; }
        
        // These are computed in the class.
        public double Ktas { get; private set; }
        public double Gs { get; private set; }

        public ICoordinate Coordinate
        {
            get
            {
                var intermediateNode = NodeValue as IntermediateNode;
                if (intermediateNode != null) return intermediateNode.Coordinate;

                var routeNode = NodeValue as RouteNode;
                if (routeNode != null) return routeNode.Waypoint;

                var tocNode = NodeValue as TocNode;
                if (tocNode != null) return tocNode.Coordinate;

                var todNode = NodeValue as TodNode;
                if (todNode != null) return todNode.Coordinate;

                throw new UnexpectedExecutionStateException(
                    "Something is wrong in NodeValue validation.");
            }
        }

        public PlanNode(
            object NodeValue,
            IWindTableCollection WindTable,
            Waypoint PrevWaypoint,
            LinkedListNode<RouteNode> NextRouteNode,
            ICoordinate NextPlanNodeCoordinate,
            double Alt,
            double GrossWt,
            double FuelOnBoard,
            double TimeRemaining,
            double Kias)
        {
            if (!IsValidType(NodeValue))
            {
                throw new ArgumentException("Type not allowed.");
            }

            this.NodeValue = NodeValue;
            this.WindTable = WindTable;
            this.PrevWaypoint = PrevWaypoint;
            this.NextRouteNode = NextRouteNode;
            this.NextPlanNodeCoordinate = NextPlanNodeCoordinate;
            this.Alt = Alt;
            this.GrossWt = GrossWt;
            this.FuelOnBoard = FuelOnBoard;
            this.TimeRemaining = TimeRemaining;
            this.Kias = Kias;

            ComputeParameters();
        }

        private static bool IsValidType(object NodeValue)
        {
            var type = NodeValue.GetType();
            return AllowedNodeTypes.Any(t => t.IsAssignableFrom(type));
        }

        private void ComputeParameters()
        {
            Ktas = Ktas(Kias, Alt);
            Gs = GetGS(
                WindTable,
                Alt,
                Ktas,
                PrevWaypoint.ToVector3D(),
                NextRouteNode.Value.Waypoint.ToVector3D(),
                NextPlanNodeCoordinate.ToVector3D());
        }
    }
}