﻿using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class SortieInfo : ModelBase
    {
        static SortieInfo r_Current;

        public long ID { get; } = (long)DateTimeOffset.Now.ToUnixTime();

        public Fleet Fleet { get; }
        public Fleet EscortFleet { get; }
        public MapInfo Map { get; }

        public SortieCellInfo Cell { get; private set; }

        int r_PendingShipCount;
        public int PendingShipCount
        {
            get { return r_PendingShipCount; }
            private set
            {
                if (r_PendingShipCount != value)
                {
                    r_PendingShipCount = value;
                    OnPropertyChanged(nameof(PendingShipCount));
                }
            }
        }

        static SortieInfo()
        {
            SessionService.Instance.Subscribe("api_port/port", _ => r_Current = null);

            SessionService.Instance.Subscribe(new[] { "api_req_map/start", "api_req_map/next" }, r => r_Current?.Explore(r.Requests, (RawMapExploration)r.Data));

            SessionService.Instance.Subscribe(new[] { "api_req_sortie/battleresult", "api_req_combined_battle/battleresult" }, r =>
            {
                var rData = (RawBattleResult)r.Data;

                if (rData.DroppedShip != null)
                     r_Current.PendingShipCount++;
            });
        }
        internal SortieInfo() { }
        internal SortieInfo(Fleet rpFleet, int rpMapID)
        {
            r_Current = this;

            Fleet = rpFleet;
            if (KanColleGame.Current.Port.Fleets.CombinedFleetType != 0 && rpFleet.ID == 1)
                EscortFleet = KanColleGame.Current.Port.Fleets[2];

            Map = KanColleGame.Current.Maps[rpMapID];
        }

        void Explore(IReadOnlyDictionary<string, string> rpRequests, RawMapExploration rpData)
        {
            Cell = new SortieCellInfo(rpData);

            var rDifficulty = Map.Difficulty;
            if (!rDifficulty.HasValue)
                Cell.InternalID = Cell.ID;
            else
            {
                var rDifficultyCount = Enum.GetNames(typeof(EventMapDifficulty)).Length - 1;
                Cell.InternalID = Cell.ID * rDifficultyCount + (int)rDifficulty.Value - 3;
            }

            OnPropertyChanged(nameof(Cell));
        }
    }
}
