﻿using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.OSS;
using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Parsers;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest
{
    public class QuestInfo : ModelBase
    {
        public int ID { get; }

        public int Total { get; }
        public int StartFrom { get; }
        public int DisplayTotal => Total - StartFrom;

        public bool IsDailyReset { get; }

        public int RuleVersion { get; }
        public ProgressRule[] ProgressRules { get; }

        internal QuestInfo(int rpID)
        {
            ID = rpID;
            Total = rpID != 214 ? -1 : 209316;

            if (rpID == 214)
            {
                QuestProgressService.Instance.Infos.Add(214, this);

                OSSQuestProgressRule rOSSRule;
                if (OSSQuestProgressRule.Maps.TryGetValue(214, out rOSSRule))
                    rOSSRule.Register(this);
            }
        }
        internal QuestInfo(JToken rpJson)
        {
            ID = (int)rpJson["id"];
            Total = (int?)rpJson["total"] ?? 1;
            StartFrom = (int?)rpJson["start_from"] ?? 0;

            IsDailyReset = (bool?)rpJson["daily_reset"] ?? false;

            RuleVersion = (int?)rpJson["version"] ?? 1;
            if (MatchingRuleParser.Version < RuleVersion)
                return;

            var rProgressRule = (string)rpJson["progress_rule"];
            if (rProgressRule != null)
            {
                ProgressRules = MatchingRuleParser.Instance.ParseProgressRule(rProgressRule);
                foreach (var rRule in ProgressRules)
                    rRule.Register(this);
            }

            OSSQuestProgressRule rOSSRule;
            if (OSSQuestProgressRule.Maps.TryGetValue(ID, out rOSSRule))
                rOSSRule.Register(this);
        }
    }
}
