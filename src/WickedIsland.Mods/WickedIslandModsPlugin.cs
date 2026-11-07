using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;
using WickedIsland.Mods.Interaction;

namespace WickedIsland.Mods;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class WickedIslandModsPlugin : BaseUnityPlugin
{
    public const string PluginGuid = "com.gnasty97.wickedisland.mods";
    public const string PluginName = "Wicked Island Mods";
    public const string PluginVersion = "0.1.0";

    private readonly PoseSwitchSystem _poseSwitchSystem = new();
    private readonly UniversalEntityInteractionPolicy _interactionPolicy = new();
    private readonly DemoSceneInteractionContext _demoContext = new();

    private ConfigEntry<bool>? _enablePoseOverlay;
    private ConfigEntry<bool>? _enableMockScene;
    private ConfigEntry<string>? _blockedTags;

    internal static ManualLogSource Log { get; private set; } = null!;

    private void Awake()
    {
        Log = Logger;

        _enablePoseOverlay = Config.Bind("PoseSwitch", "EnableOverlay", true, "Show numbered pose overlay in active scene.");
        _enableMockScene = Config.Bind("PoseSwitch", "EnableMockSceneForTesting", true, "Enable local test scene context for keybind testing.");
        _blockedTags = Config.Bind("UniversalEntity", "GlobalBlockedTags", "NoInteraction,DebugOnly", "Comma-separated tags blocked by universal policy.");

        Logger.LogInfo($"{PluginName} {PluginVersion} loaded.");
    }

    private void Update()
    {
        if (_enableMockScene is { Value: false })
        {
            return;
        }

        for (var key = 0; key <= 9; key++)
        {
            if (!Input.GetKeyDown(ToKeyCode(key)))
            {
                continue;
            }

            if (_poseSwitchSystem.HandleNumberKeyPress(key, _demoContext))
            {
                Logger.LogInfo($"Switched pose to '{_demoContext.CurrentPoseId}'.");
            }
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            var state = new InteractionState(
                IsInCutscene: false,
                IsInRestrictedZone: false,
                GlobalBlockTags: ParseBlockedTags(_blockedTags?.Value));

            var sample = new EntityProfile(
                EntityId: "sample-npc",
                Kind: EntityKind.Npc,
                Tags: new[] { "Town" },
                IsAlive: true,
                IsQuestLocked: false);

            var eligible = _interactionPolicy.IsEligible(sample, state);
            Logger.LogInfo($"Universal entity policy sample check (F7): eligible={eligible}");
        }
    }

    private void OnGUI()
    {
        if (_enablePoseOverlay is not { Value: true })
        {
            return;
        }

        if (_enableMockScene is not { Value: true })
        {
            return;
        }

        var overlayText = _poseSwitchSystem.BuildOverlayText(_demoContext);
        GUI.Box(new Rect(20, 20, 320, 220), overlayText);
    }

    private static KeyCode ToKeyCode(int value)
    {
        return value switch
        {
            0 => KeyCode.Alpha0,
            1 => KeyCode.Alpha1,
            2 => KeyCode.Alpha2,
            3 => KeyCode.Alpha3,
            4 => KeyCode.Alpha4,
            5 => KeyCode.Alpha5,
            6 => KeyCode.Alpha6,
            7 => KeyCode.Alpha7,
            8 => KeyCode.Alpha8,
            9 => KeyCode.Alpha9,
            _ => KeyCode.None
        };
    }

    private static IReadOnlyCollection<string> ParseBlockedTags(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return new List<string>();
        }

        return raw
            .Split(',')
            .Select(tag => tag.Trim())
            .Where(tag => tag.Length > 0)
            .ToList();
    }

    private sealed class DemoSceneInteractionContext : ISceneInteractionContext
    {
        private readonly List<PoseOption> _poses =
        [
            new(1, "pose_01", "Pose 1", true),
            new(2, "pose_02", "Pose 2", false),
            new(3, "pose_03", "Pose 3", false),
            new(4, "pose_04", "Pose 4", false)
        ];

        public string CurrentPoseId => _poses.FirstOrDefault(p => p.IsCurrent)?.Id ?? string.Empty;

        public bool IsSceneActive() => true;

        public IReadOnlyList<PoseOption> GetAvailablePoses() => _poses;

        public bool TrySwitchPose(string poseId)
        {
            var selected = _poses.FirstOrDefault(p => p.Id == poseId);
            if (selected is null)
            {
                return false;
            }

            for (var i = 0; i < _poses.Count; i++)
            {
                _poses[i] = _poses[i] with { IsCurrent = _poses[i].Id == poseId };
            }

            return true;
        }
    }
}
