local state = {
    poses = {
        { id = "pose_01", label = "Pose 1" },
        { id = "pose_02", label = "Pose 2" },
        { id = "pose_03", label = "Pose 3" },
        { id = "pose_04", label = "Pose 4" }
    },
    currentPoseIndex = 1
}

local function log(message)
    print("[WickedIslandMods] " .. message)
end

local function getCurrentPose()
    return state.poses[state.currentPoseIndex]
end

local function switchPoseByNumber(number)
    if number < 1 or number > #state.poses then
        return
    end

    state.currentPoseIndex = number
    local pose = getCurrentPose()

    -- Replace this with real game hook/API call.
    log("Switched to pose: " .. pose.id .. " (" .. pose.label .. ")")
end

local function renderOverlay()
    local lines = {}
    table.insert(lines, "WickedIslandMods Pose Switch:")
    for index, pose in ipairs(state.poses) do
        local marker = (index == state.currentPoseIndex) and " <current>" or ""
        table.insert(lines, tostring(index) .. ": " .. pose.label .. marker)
    end

    -- Replace with your UI/HUD draw API call.
    -- For starter debug output we print once on load only.
    return table.concat(lines, "\n")
end

local function isEntityEligible(entity)
    if entity == nil then
        return false
    end

    if entity.isAlive == false then
        return false
    end

    if entity.isQuestLocked == true then
        return false
    end

    -- Default allow for all entity kinds unless blocked by runtime conditions.
    return true
end

local function bindInput()
    -- Hook these callbacks to your UE4SS key/input API.
    -- Example usage once hooked:
    -- switchPoseByNumber(1)
    -- switchPoseByNumber(2)
end

local function initialize()
    log("UE4SS mod loaded.")
    log(renderOverlay())
    bindInput()

    -- Example policy check.
    local sampleEntity = { kind = "Npc", isAlive = true, isQuestLocked = false }
    local eligible = isEntityEligible(sampleEntity)
    log("Sample entity eligible: " .. tostring(eligible))
end

initialize()
