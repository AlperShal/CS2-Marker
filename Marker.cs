using System.Drawing; // Colo
using System.Text.Json.Serialization; // JsonPropertyNamer
using CounterStrikeSharp.API.Core; // AddCommand, BasePlugin, CCSPlayerController, Localizer
using CounterStrikeSharp.API.Core.Attributes; // MinimumApiVersion
using CounterStrikeSharp.API.Core.Translations; // StringExtensions
using CounterStrikeSharp.API.Modules.Commands; // CommandInfo
using CounterStrikeSharp.API.Modules.Commands.Targeting; // TargetResult
using CounterStrikeSharp.API.Modules.Admin; // RequiresPermissions
using CounterStrikeSharp.API.Modules.Utils; // ChatColors, Vector

namespace Marker;

public class PlaceMarker
{
    [JsonPropertyName("Names")]
    public string[] Names { get; set; } = ["marker"];

    [JsonPropertyName("Flags")]
    public string[] Flags { get; set; } = ["@css/root"];

    [JsonPropertyName("Color")]
    public string Color { get; set; } = "RoyalBlue"; // Colors List: https://learn.microsoft.com/en-us/dotnet/api/system.drawing.color?view=net-8.0

    [JsonPropertyName("Radius")]
    public float Radius { get; set; } = 100.0f;

    [JsonPropertyName("Width")]
    public float Width { get; set; } = 3.0f;

    [JsonPropertyName("Roundness")]
    public int Roundness { get; set; } = 48; // Higher value means more roundness for the inner circle of the marker. This sets the amount of straight lines to be used to draw the circle.

    [JsonPropertyName("AllowMultiple")]
    public bool AllowMultiple { get; set; } = false;
}

public class GiveMarker
{
    [JsonPropertyName("Names")]
    public string[] Names { get; set; } = ["givemarker", "togglemarker"];

    [JsonPropertyName("Flags")]
    public string[] Flags { get; set; } = ["@css/root"];
}

public class ClearMarkers
{
    [JsonPropertyName("Names")]
    public string[] Names { get; set; } = ["clearmarkers", "clearmarker"];

    [JsonPropertyName("Flags")]
    public string[] Flags { get; set; } = ["@css/root"];
}

// Config /Commands/
public class Commands
{
    [JsonPropertyName("GiveMarker")]
    public GiveMarker GiveMarker { get; set; } = new();

    [JsonPropertyName("PlaceMarker")]
    public PlaceMarker PlaceMarker { get; set; } = new();

    [JsonPropertyName("ClearMarkers")]
    public ClearMarkers ClearMarkers { get; set; } = new();
}

// Config /
public class PluginConfig : BasePluginConfig
{
    [JsonPropertyName("ChatPrefix")]
    public string ChatPrefix { get; set; } = " {blue}[Marker]";

    [JsonPropertyName("Commands")]
    public Commands Commands { get; set; } = new();
}


// Plugin
[MinimumApiVersion(80)]
public class Plugin : BasePlugin, IPluginConfig<PluginConfig>
{
    // Plugin Information
    public override string ModuleName => "Marker";
    public override string ModuleVersion => "1.0.3";
    public override string ModuleAuthor => "AlperShal<alper@sal.web.tr>";
    public override string ModuleDescription => "A plugin to put a marker on the map.";


    // Create Config Object
    public PluginConfig Config { get; set; } = new();
    public void OnConfigParsed(PluginConfig config)
    {
        config.ChatPrefix = StringExtensions.ReplaceColorTags(config.ChatPrefix);

        Config = config;
    }


    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventRoundStart>((@event, info) =>
        {
            // Since game automatically clears all entities on EventRoundStart we are also supposed to clear the list of placedMarkers.
            placedMarkers = [];

            return HookResult.Continue;
        });

        // GiveMarker
        foreach (var name in Config.Commands.GiveMarker.Names)
        {
            AddCommand("css_" + name, Localizer["command.givemarker.description"], OnGiveMarkerCommand);
        }

        // PlaceMarker
        foreach (var name in Config.Commands.PlaceMarker.Names)
        {
            AddCommand(name, Localizer["command.placemarker.description"], OnPlaceMarkerCommand);
        }

        // ClearMarkers
        foreach (var name in Config.Commands.ClearMarkers.Names)
        {
            AddCommand("css_" + name, Localizer["command.clearmarkers.description"], OnClearMarkersCommand);
        }
    }


    CCSPlayerController? markerController;
    public void OnGiveMarkerCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (AdminManager.PlayerHasPermissions(player, Config.Commands.GiveMarker.Flags))
        {
            TargetResult targetResults = commandInfo.GetArgTargetResult(1);
            CCSPlayerController targetResult = targetResults.First();
            if (targetResults.Count() == 0)
            {
                commandInfo.ReplyToCommand(Config.ChatPrefix + ChatColors.Default + Localizer["general.target.noMatch"]);
                return;
            }
            if (targetResults.Count() > 1)
            {
                commandInfo.ReplyToCommand(Config.ChatPrefix + ChatColors.Default + Localizer["general.target.multipleMatches"]);
                return;
            }

            if (markerController == targetResult)
            {
                markerController = null;
                commandInfo.ReplyToCommand(Config.ChatPrefix + ChatColors.Default + Localizer["command.givemarker.admin.noLonger", targetResult.PlayerName]);
                targetResult.PrintToChat(Config.ChatPrefix + ChatColors.Default + Localizer["command.givemarker.controller.noLonger"]);
            }
            else
            {
                if (markerController != null)
                {
                    commandInfo.ReplyToCommand(Config.ChatPrefix + ChatColors.Default + Localizer["command.givemarker.admin.noLonger", markerController.PlayerName]);
                    markerController.PrintToChat(Config.ChatPrefix + ChatColors.Default + Localizer["command.givemarker.controller.noLonger"]);
                }
                markerController = targetResult;
                commandInfo.ReplyToCommand(Config.ChatPrefix + ChatColors.Default + Localizer["command.givemarker.admin.fromNowOn", markerController.PlayerName]);
                markerController.PrintToChat(Config.ChatPrefix + ChatColors.Default + Localizer["command.givemarker.controller.fromNowOn"]);
                markerController.PrintToChat(Config.ChatPrefix + ChatColors.Default + Localizer["command.givemarker.controller.howToUse1"]);
                markerController.PrintToChat(Config.ChatPrefix + ChatColors.Default + Localizer["command.givemarker.controller.howToUse2"]);
            }
        }
    }


    List<List<CBeam>> placedMarkers = [];
    public void OnPlaceMarkerCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null)
        {
            commandInfo.ReplyToCommand("Console can not place markers!");
            return;
        }

        // Check if player is valid
        if ((markerController != null) && (markerController == player))
        {
            if (!Config.Commands.PlaceMarker.AllowMultiple)
            {
                ClearMarkers();
            }

            if ((player.Pawn.Value != null) && (player.Pawn.Value.AbsOrigin != null) && (player.PlayerPawn.Value != null))
            {
                Vector? markerPos = RayTrace.TraceRay(player.Pawn.Value.AbsOrigin, player.PlayerPawn.Value.EyeAngles, Masks.laserMask, true);

                if (markerPos != null)
                {
                    placedMarkers.Add(Draw.Marker(markerPos, Config.Commands.PlaceMarker.Radius, Config.Commands.PlaceMarker.Width, Config.Commands.PlaceMarker.Roundness, Color.FromName(Config.Commands.PlaceMarker.Color)));
                }
            }
        }
    }


    public void OnClearMarkersCommand(CCSPlayerController? player, CommandInfo? commandInfo)
    {
        if (AdminManager.PlayerHasPermissions(player, Config.Commands.ClearMarkers.Flags) || markerController == player)
        {
            ClearMarkers();
        }
    }


    public void ClearMarkers()
    {
        if (placedMarkers.Count != 0)
        {
            foreach (var marker in placedMarkers)
            {
                foreach (var beam in marker)
                {
                    beam.Remove();
                }
            }
            placedMarkers = [];
        }
    }


    public override void Unload(bool hotReload)
    {
        ClearMarkers();
    }
}
