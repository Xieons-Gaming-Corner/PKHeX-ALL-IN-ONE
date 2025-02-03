using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find possible encounters for <see cref="GameVersion.Gen5"/> encounters.
/// </summary>
public record struct EncounterPossible5(EvoCriteria[] Chain, EncounterTypeGroup Flags, GameVersion Version) : IEnumerator<IEncounterable>
{
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
    public IEncounterable? Current { get; private set; }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

    private int Index;
  
    private YieldState State;
#pragma warning disable CS8603 // Possible null reference return.
    readonly object IEnumerator.Current => Current;
#pragma warning restore CS8603 // Possible null reference return.

    public void Reset() => throw new NotSupportedException();

    public void Dispose() { }

    public IEnumerator<IEncounterable> GetEnumerator() => this;

    private enum YieldState : byte
    {
        Start,

        EventStart,
        Event,
        EventLocal,
        Bred,
        BredSplit,

        TradeStart,
        TradeW,
        TradeB,
        TradeBW,
        TradeW2,
        TradeB2,
        TradeB2W2,

        StaticStart,
        StaticW,
        StaticB,
        StaticSharedBW,
        StaticEntreeBW,
        StaticW2,
        StaticB2,
        StaticN,
        StaticSharedB2W2,
        StaticEntreeB2W2,
        StaticRadar,
        StaticEntreeShared,

        SlotStart,
        SlotW,
        SlotB,
        SlotW2,
        SlotB2,
        SlotEnd,
    }

    public bool MoveNext()
    {
        switch (State)
        {
            case YieldState.Start:
                if (Chain.Length == 0) return false;
                if (Flags.HasFlag(EncounterTypeGroup.Egg)) return TrySetState(YieldState.Bred);
                return TrySetState(YieldState.EventStart);

            case YieldState.Bred:
#pragma warning disable CS8974 // Converting method group to non-delegate type
                return TrySetStateIfEgg(EncounterGenerator5.TryGetEgg);
#pragma warning restore CS8974 // Converting method group to non-delegate type

            case YieldState.BredSplit:
#pragma warning disable CS8974 // Converting method group to non-delegate type
                return TrySetStateIfSplit(EncounterGenerator5.TryGetSplit);
#pragma warning restore CS8974 // Converting method group to non-delegate type

            case YieldState.EventStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Mystery)) return TrySetState(YieldState.TradeStart);
                return TrySetState(YieldState.Event);

            case YieldState.Event:
                if (TryGetNextEvent(EncounterEvent.MGDB_G5)) return true;
                Index = 0; State = YieldState.EventLocal; goto case YieldState.EventLocal;

            case YieldState.EventLocal:
                if (TryGetNextEvent(EncounterEvent.EGDB_G5)) return true;
                Index = 0; return TrySetState(YieldState.TradeStart);

            case YieldState.TradeStart:
                return TrySetStateForTrade();

            case YieldState.StaticStart:
                return TrySetStateForStatic();

            case YieldState.SlotStart:
                return TrySetStateForSlot();

            case YieldState.SlotEnd:
                break;
        }
        return false;
    }

    private bool TrySetStateIfSplit(object tryGetSplit)
    {
        throw new NotImplementedException();
    }

    private bool TrySetStateIfEgg(object tryGetEgg)
    {
        throw new NotImplementedException();
    }

    private bool TrySetState(YieldState newState)
    {
        State = newState;
        return false;
    }

    private bool TrySetStateIfEgg(Func<EvoCriteria[], GameVersion, IEncounterable> tryGetEgg)
    {
        var egg = tryGetEgg(Chain, Version);
        if (egg != null)
        {
            State = YieldState.BredSplit;
            return SetCurrent(egg);
        }
        return TrySetState(YieldState.EventStart);
    }

private bool TrySetStateIfSplit(Func<EncounterEgg, EvoCriteria[], IEncounterable> tryGetSplit)
{
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.
        var egg = tryGetSplit((EncounterEgg)Current, Chain);
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        if (egg != null)
    {
        State = YieldState.EventStart;
        return SetCurrent(egg);
    }
    return TrySetState(YieldState.EventStart);
}


    private bool TrySetStateForTrade()
    {
        if (!Flags.HasFlag(EncounterTypeGroup.Trade)) return TrySetState(YieldState.StaticStart);

        return Version switch
        {
            GameVersion.W => TrySetState(YieldState.TradeW),
            GameVersion.B => TrySetState(YieldState.TradeB),
            GameVersion.W2 => TrySetState(YieldState.TradeW2),
            GameVersion.B2 => TrySetState(YieldState.TradeB2),
            _ => throw new ArgumentOutOfRangeException(nameof(Version)),
        };
    }

    private bool TrySetStateForStatic()
    {
        if (!Flags.HasFlag(EncounterTypeGroup.Static)) return TrySetState(YieldState.SlotStart);

        return Version switch
        {
            GameVersion.W => TrySetState(YieldState.StaticW),
            GameVersion.B => TrySetState(YieldState.StaticB),
            GameVersion.W2 => TrySetState(YieldState.StaticW2),
            GameVersion.B2 => TrySetState(YieldState.StaticB2),
            _ => throw new ArgumentOutOfRangeException(nameof(Version)),
        };
    }

    private bool TrySetStateForSlot()
    {
        if (!Flags.HasFlag(EncounterTypeGroup.Slot)) return TrySetState(YieldState.SlotEnd);

        return Version switch
        {
            GameVersion.W => TrySetState(YieldState.SlotW),
            GameVersion.B => TrySetState(YieldState.SlotB),
            GameVersion.W2 => TrySetState(YieldState.SlotW2),
            GameVersion.B2 => TrySetState(YieldState.SlotB2),
            _ => throw new ArgumentOutOfRangeException(nameof(Version)),
        };
    }

    private bool TryGetNext<T>(T[] db) where T : class, IEncounterable, IEncounterMatch
    {
        for (; Index < db.Length;)
        {
            var enc = db[Index++];
            foreach (var evo in Chain)
            {
                if (evo.Species == enc.Species) return SetCurrent(enc);
            }
        }
        return false;
    }

    private bool TryGetNextEvent<T>(T[] db) where T : class, IEncounterable, IRestrictVersion
    {
        for (; Index < db.Length;)
        {
            var enc = db[Index++];
            if (!enc.CanBeReceivedByVersion(Version)) continue;
            foreach (var evo in Chain)
            {
                if (evo.Species == enc.Species) return SetCurrent(enc);
            }
        }
        return false;
    }

    private bool SetCurrent(IEncounterable match)
    {
        Current = match;
        return true;
    }
}
