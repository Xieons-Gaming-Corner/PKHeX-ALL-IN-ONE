using static PKHeX.Core.PIDType;

namespace PKHeX.Core;

/// <summary>
/// PID + IV correlation.
/// </summary>
/// <remarks>This is just a catch-all enumeration to describe the different correlations.</remarks>
public enum PIDType : byte
{
    /// <summary> No relationship between the PID and IVs </summary>
    None,

    #region LCRNG

    /// <summary> Method 1 Variants (H1/J/K) </summary>
    /// <remarks><see cref="LCRNG"/></remarks>
    Method_1,

    /// <summary> Method H2 </summary>
    /// <remarks><see cref="LCRNG"/></remarks>
    Method_2,

    /// <summary> Method H3 </summary>
    /// <remarks><see cref="LCRNG"/></remarks>
    Method_3,

    /// <summary> Method H4 </summary>
    /// <remarks><see cref="LCRNG"/></remarks>
    Method_4,

    /// <summary> Method H1_Unown (FR/LG) </summary>
    /// <remarks><see cref="LCRNG"/></remarks>
    Method_1_Unown,

    /// <summary> Method H2_Unown (FR/LG) </summary>
    /// <remarks><see cref="LCRNG"/></remarks>
    Method_2_Unown,

    /// <summary> Method H3_Unown (FR/LG) </summary>
    /// <remarks><see cref="LCRNG"/></remarks>
    Method_3_Unown,

    /// <summary> Method H4_Unown (FR/LG) </summary>
    /// <remarks><see cref="LCRNG"/></remarks>
    Method_4_Unown,

    /// <summary>  Method 1 Roamer (Gen3) </summary>
    /// <remarks><see cref="LCRNG"/></remarks>
    Method_1_Roamer,

    #region BACD Event

    #region Internal Matching Tagging
    /// <summary> Event Reversed Order PID without Origin Seed restrictions </summary>
    BACD,
    /// <summary> Event Reversed Order PID without Origin Seed restrictions, anti-shiny (add). </summary>
    BACD_A,
    /// <summary> Event Reversed Order PID without Origin Seed restrictions, anti-shiny (xor) </summary>
    BACD_AX,
    /// <summary> Event Reversed Order PID without Origin Seed restrictions, shiny </summary>
    BACD_S,
    /// <summary> Event Reversed Order PID without Origin Seed restrictions, anti-shiny (add). </summary>
    #endregion

    #region Egg-Only Internal Matching Tagging
    BACD_EA,
    /// <summary> Event Reversed Order PID without Origin Seed restrictions, anti-shiny (xor) </summary>
    BACD_EAX,
    /// <summary> Event Reversed Order PID without Origin Seed restrictions, shiny </summary>
    BACD_ES,
    #endregion

    /// <summary> Event Reversed Order PID without Origin Seed restrictions </summary>
    BACD_U,
    /// <summary> Event Reversed Order PID without Origin Seed restrictions, anti-shiny (xor) </summary>
    BACD_U_AX,
    // BACD_U_A and BACD_U_S were never generated by any event.

    /// <summary> Event Reversed Order PID restricted to 16bit Origin Seed </summary>
    BACD_R,
    /// <summary> Event Reversed Order PID restricted to 16bit Origin Seed, anti-shiny. </summary>
    BACD_R_A,
    /// <summary> Event Reversed Order PID restricted to [0,213] Origin Seed, shiny </summary>
    BACD_RBCD,
    /// <summary> Event Reversed Order PID with Origin Seed restrictions, consuming 2 calls to the RNG before the PID is generated. </summary>
    BACD_T2,
    /// <summary> Event Reversed Order PID with Origin Seed restrictions, consuming 3 calls to the RNG before the PID is generated. </summary>
    BACD_T3,
    /// <summary> Event Reversed Order PID with Origin Seed restrictions, only using the Mystry Mew table. </summary>
    BACD_M,

    #endregion

    /// <summary>
    /// Generation 4 Cute Charm PID, which is forced to an 8 bit PID value based on the gender &amp; gender ratio value.
    /// </summary>
    /// <remarks><see cref="LCRNG"/></remarks>
    CuteCharm,

    /// <summary>
    /// Generation 4 Chained Shiny
    /// </summary>
    /// <remarks><see cref="LCRNG"/></remarks>
    ChainShiny,

    #endregion

    #region XDRNG

    /// <summary>
    /// Generation 3 <see cref="GameVersion.CXD"/> PID+IV correlation.
    /// </summary>
    /// <remarks><see cref="XDRNG"/></remarks>
    CXD,

    /// <summary>
    /// Generation 3 <see cref="GameVersion.CXD"/> PID+IV correlation that was rerolled because it was shiny.
    /// </summary>
    /// <remarks><see cref="XDRNG"/></remarks>
    CXDAnti,

    /// <summary>
    /// Generation 3 <see cref="GameVersion.CXD"/> PID+IV which is created immediately after the TID16 and SID16 RNG calls.
    /// </summary>
    /// <remarks><see cref="XDRNG"/>. The second starter is created after the first starter, with the same TID16 and SID16.</remarks>
    CXD_ColoStarter,

    /// <summary>
    /// Generation 3 Pokémon Channel Jirachi
    /// </summary>
    /// <remarks><see cref="XDRNG"/></remarks>
    Channel,

    /// <summary>
    /// Generation 3 <see cref="GameVersion.CXD"/> PokeSpot PID
    /// </summary>
    /// <remarks><see cref="XDRNG"/></remarks>
    PokeSpot,

    #endregion

    #region ARNG

    /// <summary>
    /// Generation 4 Mystery Gift Anti-Shiny
    /// </summary>
    /// <remarks><see cref="ARNG"/></remarks>
    G4MGAntiShiny,

    #endregion

    #region Formulaic

    /// <summary>
    /// Generation 5 Mystery Gift Shiny
    /// </summary>
    /// <remarks>Formulaic based on TID16, SID16, and Gender bytes.</remarks>
    /// <remarks>Unrelated to IVs</remarks>
    G5MGShiny,

    /// <summary>
    /// Generation 4 Pokewalker PID, never Shiny.
    /// </summary>
    /// <remarks>Formulaic based on TID16, SID16, and Gender bytes.</remarks>
    /// <remarks>Unrelated to IVs</remarks>
    Pokewalker,

    /// <summary>
    /// Generation 8 Xoroshiro correlation
    /// </summary>
    /// <remarks>Formulaic based on PID &amp; EC values from a 64bit-seed.</remarks>
    Xoroshiro,

    /// <summary>
    /// Generation 8 Overworld Spawn PID
    /// </summary>
    /// <remarks>Formulaic based on PID &amp; EC values from a 32bit-seed.</remarks>
    Overworld8,

    /// <summary>
    /// Generation 8b Roaming Pokémon PID
    /// </summary>
    /// <remarks>Formulaic based on EC value = 32bit-seed.</remarks>
    Roaming8b,

    /// <summary>
    /// Generation 9 Tera Raid PID
    /// </summary>
    /// <remarks>Formulaic based on PID &amp; EC values from a 32bit-seed.</remarks>
    Tera9,

    #endregion
}

public static class PIDTypeExtensions
{
    /// <summary>
    /// Checks if the provided PIDType is one of the in-game Method-X types for Gen3.
    /// </summary>
    public static bool IsClassicMethod(this PIDType type) => type
        is Method_1 or Method_2 or Method_3 or Method_4
        or Method_1_Unown or Method_2_Unown or Method_3_Unown or Method_4_Unown;

    public static bool IsRestricted(this PIDType type) => type is >= BACD_R and <= BACD_M;
}
