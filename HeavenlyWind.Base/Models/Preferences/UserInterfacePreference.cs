﻿using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class UserInterfacePreference
    {
        public Property<string> Font { get; } = new UIFontProperty();

        public Property<ConfirmationMode> CloseConfirmationMode { get; } = new Property<ConfirmationMode>("ui.close_confirmation", ConfirmationMode.DuringSortie);

        public Property<Dock> LandscapeDock { get; } = new Property<Dock>("ui.layout.lanscape", Dock.Left);
        public Property<Dock> PortraitDock { get; } = new Property<Dock>("ui.layout.portrait", Dock.Top);

        public Property<double> Zoom { get; } = new Property<double>("ui.zoom", 1.0);

        public HeavyDamageLinePreference HeavyDamageLine { get; } = new HeavyDamageLinePreference();

        public Property<bool> UseGameMaterialIcons { get; } = new Property<bool>("ui.use_game_material_icons");
    }
}
