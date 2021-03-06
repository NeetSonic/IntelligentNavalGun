﻿using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.KanColle.Amatsukaze.Views.Game;
using Sakuno.KanColle.Amatsukaze.Views.Overviews;
using Sakuno.UserInterface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    [ViewInfo(typeof(Overview))]
    public class OverviewViewModel : TabItemViewModel
    {
        public override string Name
        {
            get { return StringResources.Instance.Main.Tab_Overview; }
            protected set { throw new NotImplementedException(); }
        }

        public AdmiralViewModel Admiral { get; } = new AdmiralViewModel();
        public MaterialsViewModel Materials { get; } = new MaterialsViewModel();

        int r_ShipCount;
        public int ShipCount
        {
            get { return r_ShipCount; }
            private set
            {
                if (r_ShipCount != value)
                {
                    r_ShipCount = value;
                    OnPropertyChanged(nameof(ShipCount));
                }
            }
        }
        int r_EquipmentCount;
        public int EquipmentCount
        {
            get { return r_EquipmentCount; }
            private set
            {
                if (r_EquipmentCount != value)
                {
                    r_EquipmentCount = value;
                    OnPropertyChanged(nameof(EquipmentCount));
                }
            }
        }

        IReadOnlyList<FleetViewModel> r_Fleets;
        public IReadOnlyList<FleetViewModel> Fleets
        {
            get { return r_Fleets; }
            internal set
            {
                if (r_Fleets != value)
                {
                    r_Fleets = value;
                    OnPropertyChanged(nameof(Fleets));
                }
            }
        }

        public AirBaseViewModel AirBase { get; }

        public IList<ModelBase> RightTabs { get; private set; }

        ModelBase r_SelectedTab;
        public ModelBase SelectedTab
        {
            get { return r_SelectedTab; }
            set
            {
                if (r_SelectedTab != value)
                {
                    r_SelectedTab = value;
                    OnPropertyChanged(nameof(SelectedTab));
                }
            }
        }

        IReadOnlyCollection<RepairDockViewModel> r_RepairDocks;
        public IReadOnlyCollection<RepairDockViewModel> RepairDocks
        {
            get { return r_RepairDocks; }
            private set
            {
                if (r_RepairDocks != value)
                {
                    r_RepairDocks = value;
                    OnPropertyChanged(nameof(RepairDocks));
                }
            }
        }
        IReadOnlyCollection<ConstructionDockViewModel> r_ConstructionDocks;
        public IReadOnlyCollection<ConstructionDockViewModel> ConstructionDocks
        {
            get { return r_ConstructionDocks; }
            private set
            {
                if (r_ConstructionDocks != value)
                {
                    r_ConstructionDocks = value;
                    OnPropertyChanged(nameof(ConstructionDocks));
                }
            }
        }

        IList<QuestViewModel> r_ActiveQuests;
        public IList<QuestViewModel> ActiveQuests
        {
            get { return r_ActiveQuests; }
            internal set
            {
                if (r_ActiveQuests != value)
                {
                    r_ActiveQuests = value;
                    OnPropertyChanged(nameof(ActiveQuests));
                }
            }
        }

        public ICommand ShowShipOverviewWindowCommand { get; }
        public ICommand ShowEquipmentOverviewWindowCommand { get; }

        internal OverviewViewModel()
        {
            var rPort = KanColleGame.Current.Port;

            var rPortPCEL = PropertyChangedEventListener.FromSource(rPort);
            rPortPCEL.Add(nameof(rPort.Ships), (s, e) => ShipCount = rPort.Ships.Count);
            rPortPCEL.Add(nameof(rPort.Equipment), (s, e) => EquipmentCount = rPort.Equipment.Count);
            rPortPCEL.Add(nameof(rPort.RepairDocks), (s, e) => RepairDocks = rPort.RepairDocks.Values.Select(r => new RepairDockViewModel(r)).ToList());
            rPortPCEL.Add(nameof(rPort.ConstructionDocks), (s, e) => ConstructionDocks = rPort.ConstructionDocks.Values.Select(r => new ConstructionDockViewModel(r)).ToList());

            AirBase = new AirBaseViewModel();

            SessionService.Instance.SubscribeOnce("api_get_member/base_air_corps", delegate
            {
                DispatcherUtil.UIDispatcher.BeginInvoke(new Action(() =>
                {
                    if (RightTabs == null)
                    {
                        RightTabs = new ObservableCollection<ModelBase>();
                        OnPropertyChanged(nameof(RightTabs));
                    }

                    RightTabs.Add(AirBase);
                }));
            });

            SessionService.Instance.Subscribe("api_req_map/next", delegate
            {
                var rSortie = SortieInfo.Current;
                if (rSortie != null)
                    ShipCount = rPort.Ships.Count + rSortie.PendingShipCount;
            });

            ShowShipOverviewWindowCommand = new DelegatedCommand(() => WindowService.Instance.Show<ShipOverviewWindow>());
            ShowEquipmentOverviewWindowCommand = new DelegatedCommand(() => WindowService.Instance.Show<EquipmentOverviewWindow>());
        }
    }
}
