using System;
using System.Collections.Generic;

namespace WickedIsland.Mods.Interaction;

/// <summary>
/// Broad entity categories used by default interaction logic.
/// </summary>
public enum EntityKind
{
    Unknown = 0,
    Player = 1,
    Npc = 2,
    Creature = 3,
    Boss = 4,
    Summon = 5,
    Ambient = 6
}

/// <summary>
/// Normalized entity payload passed into interaction policy.
/// </summary>
public sealed record EntityProfile(
    string EntityId,
    EntityKind Kind,
    IReadOnlyCollection<string> Tags,
    bool IsAlive,
    bool IsQuestLocked);

/// <summary>
/// Runtime state used by policy checks.
/// </summary>
public sealed record InteractionState(
    bool IsInCutscene,
    bool IsInRestrictedZone,
    IReadOnlyCollection<string> GlobalBlockTags);

/// <summary>
/// Policy that allows interactions with all entities unless explicitly blocked.
/// </summary>
public sealed class UniversalEntityInteractionPolicy
{
    private readonly Dictionary<EntityKind, Func<EntityProfile, InteractionState, bool>> _entityKindRules = new();

    public UniversalEntityInteractionPolicy()
    {
        // Default: all known kinds are allowed unless blocked by shared gates.
        _entityKindRules[EntityKind.Unknown] = static (_, _) => true;
        _entityKindRules[EntityKind.Player] = static (_, _) => true;
        _entityKindRules[EntityKind.Npc] = static (_, _) => true;
        _entityKindRules[EntityKind.Creature] = static (_, _) => true;
        _entityKindRules[EntityKind.Boss] = static (_, _) => true;
        _entityKindRules[EntityKind.Summon] = static (_, _) => true;
        _entityKindRules[EntityKind.Ambient] = static (_, _) => true;
    }

    /// <summary>
    /// Register or replace a resolver for one entity kind.
    /// </summary>
    public void SetRule(EntityKind kind, Func<EntityProfile, InteractionState, bool> rule)
    {
        _entityKindRules[kind] = rule ?? throw new ArgumentNullException(nameof(rule));
    }

    /// <summary>
    /// Checks if the current entity is eligible for interaction.
    /// </summary>
    public bool IsEligible(EntityProfile entity, InteractionState state)
    {
        if (entity is null)
        {
            return false;
        }

        if (state is null)
        {
            return false;
        }

        if (state.IsInCutscene || state.IsInRestrictedZone)
        {
            return false;
        }

        if (!entity.IsAlive || entity.IsQuestLocked)
        {
            return false;
        }

        foreach (var tag in entity.Tags)
        {
            if (state.GlobalBlockTags.Contains(tag))
            {
                return false;
            }
        }

        if (_entityKindRules.TryGetValue(entity.Kind, out var rule))
        {
            return rule(entity, state);
        }

        return true;
    }
}
