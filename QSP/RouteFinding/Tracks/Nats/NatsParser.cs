﻿using System;
using System.Collections.Generic;
using QSP.RouteFinding.Tracks.Common;
using QSP.RouteFinding.Tracks.Interaction;
using QSP.RouteFinding.Airports;

namespace QSP.RouteFinding.Tracks.Nats
{
    public class NatsParser : TrackParser<NorthAtlanticTrack>
    {
        private StatusRecorder statusRecorder;
        private AirportManager airportList;
        private NatsMessage message;

        public NatsParser(NatsMessage message, StatusRecorder statusRecorder, AirportManager airportList)
        {
            this.message = message;
            this.statusRecorder = statusRecorder;
            this.airportList = airportList;
        }

        public override List<NorthAtlanticTrack> Parse()
        {
            var NatTrackCollection = tryAddMessage(message.WestMessage);
            NatTrackCollection.AddRange(tryAddMessage(message.EastMessage));
            return NatTrackCollection;
        }

        private List<NorthAtlanticTrack> tryAddMessage(IndividualNatsMessage msg)
        {
            try
            {
                return ConvertToTracks(msg);
            }
            catch
            {
                statusRecorder.AddEntry(StatusRecorder.Severity.Caution,
                                        string.Format("Unable to interpret {0} tracks.",
                                        (msg.Direction == NatsDirection.East) ? "eastbound" : "westbound"),
                                        TrackType.Nats);

                return new List<NorthAtlanticTrack>();
            }
        }

        private static List<NorthAtlanticTrack> ConvertToTracks(IndividualNatsMessage msg)
        {
            char trkStartChar = (msg.Direction == NatsDirection.West ? 'A' : 'N');
            var Message = msg.Message;
            var tracks = new List<NorthAtlanticTrack>();

            for (int i = trkStartChar; i <= trkStartChar + 12; i++)
            {
                int j = Message.IndexOf("\n" + (char)i + " ");

                if (j == -1)
                {
                    continue;
                }

                int k = Message.IndexOf('\n', j + 3);

                string str = Message.Substring(j + 3, k - j - 3);
                string[] wp = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                tryConvertNatsLatLon(wp);

                tracks.Add(new NorthAtlanticTrack(msg.Direction,
                                                  ((char)i).ToString(),
                                                  "",
                                                  "",
                                                  "",
                                                  new List<string>(wp).AsReadOnly(),
                                                  Constants.CENTER_ATL));
            }
            return tracks;
        }

        /// <summary>
        /// Sample input : "54/20". Sample output : "5420N"
        /// </summary>
        private static string NatsLatLonToIdent(string s)
        {
            return new string(new char[] { s[0], s[1], s[3], s[4], 'N' });
        }

        private static bool IsNatsLatLonFormat(string s)
        {
            return (s.Length == 5 &&
                    s[2] == '/' &&
                    char.IsDigit(s[0]) &&
                    char.IsDigit(s[1]) &&
                    char.IsDigit(s[3]) &&
                    char.IsDigit(s[4]));
        }

        private static void tryConvertNatsLatLon(string[] wpts)
        {
            for (int i = 0; i < wpts.Length; i++)
            {
                if (IsNatsLatLonFormat(wpts[i]))
                {
                    wpts[i] = NatsLatLonToIdent(wpts[i]);
                }
            }
        }
    }
}