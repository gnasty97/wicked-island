using System;
using System.Collections.Generic;
using System.Linq;

namespace WickedIsland.Mods.Interaction;

/// <summary>
/// Represents one selectable pose/action entry displayed in the active scene overlay.
/// </summary>
public sealed record PoseOption(int Index, string Id, string Label, bool IsCurrent);

/// <summary>
/// Adapter for the game runtime. Implement this in your mod host with actual game hooks.
/// </summary>
public interface ISceneInteractionContext
{
    bool IsSceneActive();
    IReadOnlyList<PoseOption> GetAvailablePoses();
    bool TrySwitchPose(string poseId);
}

/// <summary>
/// Number-key driven pose switching helper for active scenes.
/// </summary>
public sealed class PoseSwitchSystem
{
    private readonly Dictionary<int, int> _keyToPoseIndex;

    public PoseSwitchSystem(int maxShortcuts = 10)
    {
        if (maxShortcuts <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxShortcuts), "Value must be greater than zero.");
        }

        _keyToPoseIndex = BuildNumberMapping(maxShortcuts);
    }

    /// <summary>
    /// Handles one numeric key press and attempts pose switch if a scene is active.
    /// </summary>
    public bool HandleNumberKeyPress(int numberKey, ISceneInteractionContext context)
    {
        if (context is null || !context.IsSceneActive())
        {
            return false;
        }

        if (!_keyToPoseIndex.TryGetValue(numberKey, out var poseIndex))
        {
            return false;
        }

        var poses = context.GetAvailablePoses();
        var selected = poses.FirstOrDefault(p => p.Index == poseIndex);
        if (selected is null)
        {
            return false;
        }

        return context.TrySwitchPose(selected.Id);
    }

    /// <summary>
    /// Builds a compact on-screen list of numeric shortcuts for the current scene.
    /// </summary>
    public string BuildOverlayText(ISceneInteractionContext context)
    {
        if (context is null || !context.IsSceneActive())
        {
            return "No active scene.";
        }

        var poses = context
            .GetAvailablePoses()
            .OrderBy(p => p.Index)
            .ToList();

        if (poses.Count == 0)
        {
            return "No available poses.";
        }

        var lines = new List<string> { "Pose Switch (press number):" };

        foreach (var pose in poses)
        {
            var mappedKey = -1;
            foreach (var pair in _keyToPoseIndex)
            {
                if (pair.Value == pose.Index)
                {
                    mappedKey = pair.Key;
                    break;
                }
            }

            if (mappedKey < 0)
            {
                continue;
            }

            var currentMarker = pose.IsCurrent ? " <current>" : string.Empty;
            lines.Add($"{mappedKey}: {pose.Label}{currentMarker}");
        }

        return string.Join(Environment.NewLine, lines);
    }

    private static Dictionary<int, int> BuildNumberMapping(int maxShortcuts)
    {
        // Maps keyboard keys to pose indices:
        // 1 -> index 1, ..., 9 -> index 9, 0 -> index 10
        var mapping = new Dictionary<int, int>();
        for (var i = 1; i <= Math.Min(maxShortcuts, 9); i++)
        {
            mapping[i] = i;
        }

        if (maxShortcuts >= 10)
        {
            mapping[0] = 10;
        }

        return mapping;
    }
}
