using System.Numerics;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Memory;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;


public static class RayTrace
{
    public static Vector Clone(this Vector vector)
    {
        var vec = new Vector
        {
            X = vector.X,
            Y = vector.Y,
            Z = vector.Z
        };
        return vec;
    }

    public static QAngle Clone(this QAngle angles)
    {
        var ang = new QAngle
        {
            X = angles.X,
            Y = angles.Y,
            Z = angles.Z
        };
        return ang;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate bool TraceShapeDelegate(
        nint GameTraceManager,
        nint vecStart,
        nint vecEnd,
        nint skip,
        ulong mask,
        byte a6,
        GameTrace* pGameTrace
    );

    public static TraceShapeDelegate? _traceShape;

    public static nint TraceFunc = NativeAPI.FindSignature(Addresses.ServerPath, "48 B8 ? ? ? ? ? ? ? ? 55 48 89 E5 41 57 41 56 49 89 D6 41 55");
    public static nint GameTraceManager = NativeAPI.FindSignature(Addresses.ServerPath, "48 8D 05 ? ? ? ? F3 0F 58 8D ? ? ? ? 31 FF");

    public static unsafe CounterStrikeSharp.API.Modules.Utils.Vector? TraceRay(

        CounterStrikeSharp.API.Modules.Utils.Vector origin,
        CounterStrikeSharp.API.Modules.Utils.QAngle viewangles,
        ulong mask,
        bool fromPlayer = false,
        bool crouching = false
    )
    {


        CounterStrikeSharp.API.Modules.Utils.Vector _origin = new(0, 0, 0);
        CounterStrikeSharp.API.Modules.Utils.QAngle _viewangles = new(0, 0, 0);

        var _gameTraceManagerAddress = Address.GetAbsoluteAddress(GameTraceManager, 3, 7);
        if (_gameTraceManagerAddress is 0) return null;

        _traceShape = Marshal.GetDelegateForFunctionPointer<TraceShapeDelegate>(TraceFunc);
        if (_traceShape is null) return null;

        var _forward = new CounterStrikeSharp.API.Modules.Utils.Vector();

        var d = 50;
        if (fromPlayer is true)
        {

            //copy the origin vector and the eyeangles if they are from the player
            _origin = origin.Clone();
            _viewangles = viewangles.Clone();

            _origin.X += _forward.X * 100;
            _origin.Y += _forward.Y * 100;

            if (crouching is true)
                _origin.Z += _forward.Z * d + 44;
            else
                _origin.Z += _forward.Z * d + 64;

        }
        else
        {
            _origin = origin;
            _viewangles = viewangles;
        }

        NativeAPI.AngleVectors(_viewangles.Handle, _forward.Handle, 0, 0);

        var _endOrigin = new CounterStrikeSharp.API.Modules.Utils.Vector(_origin.X + _forward.X * 8192, _origin.Y + _forward.Y * 8192, _origin.Z + _forward.Z * 8192);

        var _trace = stackalloc GameTrace[1];

        var result = _traceShape(*(nint*)_gameTraceManagerAddress, _origin.Handle, _endOrigin.Handle, 0, mask, 4, _trace);

        if (result is true)
            return new CounterStrikeSharp.API.Modules.Utils.Vector(_trace->EndPos.X, _trace->EndPos.Y, _trace->EndPos.Z);

        return null;
    }
}

internal static class Address
{
    static unsafe public nint GetAbsoluteAddress(nint addr, nint offset, int size)
    {
        int code = *(int*)(addr + offset);
        return addr + code + size;
    }

    static public nint GetCallAddress(nint a)
    {
        return GetAbsoluteAddress(a, 1, 5);
    }
}

[StructLayout(LayoutKind.Explicit, Size = 0x35)]
public unsafe struct Ray
{
    [FieldOffset(0)] public Vector3 Start;
    [FieldOffset(0xC)] public Vector3 End;
    [FieldOffset(0x18)] public Vector3 Mins;
    [FieldOffset(0x24)] public Vector3 Maxs;
    [FieldOffset(0x34)] public byte UnkType;
}

[StructLayout(LayoutKind.Explicit, Size = 0x44)]
public unsafe struct TraceHitboxData
{
    [FieldOffset(0x38)] public int HitGroup;
    [FieldOffset(0x40)] public int HitboxId;
}

[StructLayout(LayoutKind.Explicit, Size = 0xB8)]
public unsafe struct GameTrace
{
    [FieldOffset(0)] public void* Surface;
    [FieldOffset(0x8)] public void* HitEntity;
    [FieldOffset(0x10)] public TraceHitboxData* HitboxData;
    [FieldOffset(0x50)] public uint Contents;
    [FieldOffset(0x78)] public Vector3 StartPos;
    [FieldOffset(0x84)] public Vector3 EndPos;
    [FieldOffset(0x90)] public Vector3 Normal;
    [FieldOffset(0x9C)] public Vector3 Position;
    [FieldOffset(0xAC)] public float Fraction;
    [FieldOffset(0xB6)] public bool AllSolid;
}

[StructLayout(LayoutKind.Explicit, Size = 0x3a)]
public unsafe struct TraceFilter
{
    [FieldOffset(0)] public void* Vtable;
    [FieldOffset(0x8)] public ulong Mask;
    [FieldOffset(0x20)] public fixed uint SkipHandles[4];
    [FieldOffset(0x30)] public fixed ushort arrCollisions[2];
    [FieldOffset(0x34)] public uint Unk1;
    [FieldOffset(0x38)] public byte Unk2;
    [FieldOffset(0x39)] public byte Unk3;
}

public unsafe struct TraceFilterV2
{
    public ulong Mask;
    public fixed ulong V1[2];
    public fixed uint SkipHandles[4];
    public fixed ushort arrCollisions[2];
    public short V2;
    public byte V3;
    public byte V4;
    public byte V5;

}

public static class LayerIndices
{
    public const int LAYER_INDEX_CONTENTS_SOLID = 0;
    public const int LAYER_INDEX_CONTENTS_HITBOX = 1;
    public const int LAYER_INDEX_CONTENTS_TRIGGER = 2;
    public const int LAYER_INDEX_CONTENTS_SKY = 3;
    public const int LAYER_INDEX_CONTENTS_PLAYER_CLIP = 4;
    public const int LAYER_INDEX_CONTENTS_NPC_CLIP = 5;
    public const int LAYER_INDEX_CONTENTS_BLOCK_LOS = 6;
    public const int LAYER_INDEX_CONTENTS_BLOCK_LIGHT = 7;
    public const int LAYER_INDEX_CONTENTS_LADDER = 8;
    public const int LAYER_INDEX_CONTENTS_PICKUP = 9;
    public const int LAYER_INDEX_CONTENTS_BLOCK_SOUND = 10;
    public const int LAYER_INDEX_CONTENTS_NODRAW = 11;
    public const int LAYER_INDEX_CONTENTS_WINDOW = 12;
    public const int LAYER_INDEX_CONTENTS_PASS_BULLETS = 13;
    public const int LAYER_INDEX_CONTENTS_WORLD_GEOMETRY = 14;
    public const int LAYER_INDEX_CONTENTS_WATER = 15;
    public const int LAYER_INDEX_CONTENTS_SLIME = 16;
    public const int LAYER_INDEX_CONTENTS_TOUCH_ALL = 17;
    public const int LAYER_INDEX_CONTENTS_PLAYER = 18;
    public const int LAYER_INDEX_CONTENTS_NPC = 19;
    public const int LAYER_INDEX_CONTENTS_DEBRIS = 20;
    public const int LAYER_INDEX_CONTENTS_PHYSICS_PROP = 21;
    public const int LAYER_INDEX_CONTENTS_NAV_IGNORE = 22;
    public const int LAYER_INDEX_CONTENTS_NAV_LOCAL_IGNORE = 23;
    public const int LAYER_INDEX_CONTENTS_POST_PROCESSING_VOLUME = 24;
    public const int LAYER_INDEX_CONTENTS_UNUSED_LAYER3 = 25;
    public const int LAYER_INDEX_CONTENTS_CARRIED_OBJECT = 26;
    public const int LAYER_INDEX_CONTENTS_PUSHAWAY = 27;
    public const int LAYER_INDEX_CONTENTS_SERVER_ENTITY_ON_CLIENT = 28;
    public const int LAYER_INDEX_CONTENTS_CARRIED_WEAPON = 29;
    public const int LAYER_INDEX_CONTENTS_STATIC_LEVEL = 30;

    public const int LAYER_INDEX_CONTENTS_CSGO_TEAM1 = 31;
    public const int LAYER_INDEX_CONTENTS_CSGO_TEAM2 = 32;
    public const int LAYER_INDEX_CONTENTS_CSGO_GRENADE_CLIP = 33;
    public const int LAYER_INDEX_CONTENTS_CSGO_DRONE_CLIP = 34;
    public const int LAYER_INDEX_CONTENTS_CSGO_MOVEABLE = 35;
    public const int LAYER_INDEX_CONTENTS_CSGO_OPAQUE = 36;
    public const int LAYER_INDEX_CONTENTS_CSGO_MONSTER = 37;
    public const int LAYER_INDEX_CONTENTS_CSGO_UNUSED_LAYER = 38;
    public const int LAYER_INDEX_CONTENTS_CSGO_THROWN_GRENADE = 39;
}

public struct Masks
{
    public const ulong MASK_ALL = ~0UL;
    public static readonly ulong MASK_SOLID = (ulong)Contents.SOLID | (ulong)Contents.WINDOW | (ulong)Contents.PLAYER | (ulong)Contents.NPC | (ulong)Contents.PASS_BULLETS;
    public static readonly ulong MASK_PLAYERSOLID = (ulong)Contents.SOLID | (ulong)Contents.PLAYER_CLIP | (ulong)Contents.WINDOW | (ulong)Contents.PLAYER | (ulong)Contents.NPC | (ulong)Contents.PASS_BULLETS;
    public static readonly ulong MASK_NPCSOLID = (ulong)Contents.SOLID | (ulong)Contents.NPC_CLIP | (ulong)Contents.WINDOW | (ulong)Contents.PLAYER | (ulong)Contents.NPC | (ulong)Contents.PASS_BULLETS;
    public static readonly ulong MASK_NPCFLUID = (ulong)Contents.SOLID | (ulong)Contents.NPC_CLIP | (ulong)Contents.WINDOW | (ulong)Contents.PLAYER | (ulong)Contents.NPC;
    public static readonly ulong MASK_WATER = (ulong)Contents.WATER | (ulong)Contents.SLIME;
    public static readonly ulong MASK_SHOT = (ulong)Contents.SOLID | (ulong)Contents.PLAYER | (ulong)Contents.NPC | (ulong)Contents.WINDOW | (ulong)Contents.DEBRIS | (ulong)Contents.HITBOX;
    public static readonly ulong MASK_SHOT_BRUSHONLY = (ulong)Contents.SOLID | (ulong)Contents.WINDOW | (ulong)Contents.DEBRIS;
    public static readonly ulong MASK_SHOT_HULL = (ulong)Contents.SOLID | (ulong)Contents.PLAYER | (ulong)Contents.NPC | (ulong)Contents.WINDOW | (ulong)Contents.DEBRIS | (ulong)Contents.PASS_BULLETS;
    public static readonly ulong MASK_SHOT_PORTAL = (ulong)Contents.SOLID | (ulong)Contents.WINDOW | (ulong)Contents.PLAYER | (ulong)Contents.NPC;
    public static readonly ulong MASK_SOLID_BRUSHONLY = (ulong)Contents.SOLID | (ulong)Contents.WINDOW | (ulong)Contents.PASS_BULLETS;
    public static readonly ulong MASK_PLAYERSOLID_BRUSHONLY = (ulong)Contents.SOLID | (ulong)Contents.WINDOW | (ulong)Contents.PLAYER_CLIP | (ulong)Contents.PASS_BULLETS;
    public static readonly ulong MASK_NPCSOLID_BRUSHONLY = (ulong)Contents.SOLID | (ulong)Contents.WINDOW | (ulong)Contents.NPC_CLIP | (ulong)Contents.PASS_BULLETS;
    public static ulong laserMask = (ulong)Contents.SOLID //stop at solid surfaces
                | (ulong)Contents.WINDOW //stop at windows
                | (ulong)Contents.NPC_CLIP //clip through NPCs
                | (ulong)Contents.PLAYER_CLIP //clip through players
                | (ulong)Contents.PASS_BULLETS; //not sure; either go through passing bullets or not

    public static ulong CreateCustomMask(params Contents[] contents)
    {
        ulong mask = 0;
        foreach (var content in contents)
        {
            mask |= (ulong)content;
        }
        return mask;
    }
}

[Flags]
public enum Contents : ulong
{
    EMPTY = 0x0,

    SOLID = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_SOLID,
    HITBOX = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_HITBOX,
    TRIGGER = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_TRIGGER,
    SKY = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_SKY,

    PLAYER_CLIP = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_PLAYER_CLIP,
    NPC_CLIP = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_NPC_CLIP,
    BLOCK_LOS = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_BLOCK_LOS,
    BLOCK_LIGHT = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_BLOCK_LIGHT,
    LADDER = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_LADDER,
    PICKUP = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_PICKUP,
    BLOCK_SOUND = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_BLOCK_SOUND,
    NODRAW = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_NODRAW,
    WINDOW = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_WINDOW,
    PASS_BULLETS = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_PASS_BULLETS,
    WORLD_GEOMETRY = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_WORLD_GEOMETRY,
    WATER = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_WATER,
    SLIME = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_SLIME,
    TOUCH_ALL = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_TOUCH_ALL,
    PLAYER = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_PLAYER,
    NPC = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_NPC,
    DEBRIS = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_DEBRIS,
    PHYSICS_PROP = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_PHYSICS_PROP,
    NAV_IGNORE = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_NAV_IGNORE,
    NAV_LOCAL_IGNORE = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_NAV_LOCAL_IGNORE,
    POST_PROCESSING_VOLUME = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_POST_PROCESSING_VOLUME,
    UNUSED_LAYER3 = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_UNUSED_LAYER3,
    CARRIED_OBJECT = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_CARRIED_OBJECT,
    PUSHAWAY = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_PUSHAWAY,
    SERVER_ENTITY_ON_CLIENT = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_SERVER_ENTITY_ON_CLIENT,
    CARRIED_WEAPON = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_CARRIED_WEAPON,
    STATIC_LEVEL = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_STATIC_LEVEL,

    CSGO_TEAM1 = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_CSGO_TEAM1,
    CSGO_TEAM2 = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_CSGO_TEAM2,
    CSGO_GRENADE_CLIP = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_CSGO_GRENADE_CLIP,
    CSGO_DRONE_CLIP = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_CSGO_DRONE_CLIP,
    CSGO_MOVEABLE = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_CSGO_MOVEABLE,
    CSGO_OPAQUE = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_CSGO_OPAQUE,
    CSGO_MONSTER = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_CSGO_MONSTER,
    CSGO_UNUSED_LAYER = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_CSGO_UNUSED_LAYER,
    CSGO_THROWN_GRENADE = 1UL << LayerIndices.LAYER_INDEX_CONTENTS_CSGO_THROWN_GRENADE
}


