﻿using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class QuestManager : ModelBase
    {
        public IDTable<Quest> Table { get; } = new IDTable<Quest>();

        int r_TotalCount;
        public int TotalCount
        {
            get { return r_TotalCount; }
            internal set
            {
                if (r_TotalCount != value)
                {
                    r_TotalCount = value;
                    OnPropertyChanged(nameof(TotalCount));
                }
            }
        }

        public int ActiveQuestCount { get; internal set; }

        bool r_IsLoaded;
        public bool IsLoaded
        {
            get { return r_IsLoaded; }
            internal set
            {
                if (r_IsLoaded != value)
                {
                    r_IsLoaded = value;
                    OnPropertyChanged(nameof(IsLoaded));
                }
            }
        }

        public bool IsLoadCompleted => IsLoaded && TotalCount == Table.Count;

        public Quest this[int rpID] => Table[rpID];

        public IList<Quest> Active { get; private set; }

        internal QuestManager()
        {
            SessionService.Instance.Subscribe("api_get_member/questlist", _ =>
            {
                UpdateQuestList();

                IsLoaded = true;
                OnPropertyChanged(nameof(IsLoaded));
            });
            SessionService.Instance.Subscribe("api_req_quest/stop", r =>
            {
                var rQuestID = int.Parse(r.Parameters["api_quest_id"]);
                Table[rQuestID].RawData.State = QuestState.None;
            });
            SessionService.Instance.Subscribe("api_req_quest/clearitemget", r =>
            {
                var rQuestID = int.Parse(r.Parameters["api_quest_id"]);
                Table.Remove(rQuestID);
                TotalCount--;
            });
        }

        internal void UpdateQuestList()
        {
            var rActive = Table.Values.OrderBy(r => r.ID).Where(r => r.State != QuestState.None).ToList();
            if (rActive.Count < ActiveQuestCount)
                rActive.AddRange(Enumerable.Repeat(Quest.Dummy, ActiveQuestCount - rActive.Count));

            Active = rActive;
            OnPropertyChanged(nameof(Active));
        }
    }
}
