using System;

namespace PKHeX.Core;

/// <summary>
/// Details about moves in <see cref="EntityContext.Gen4"/>
/// </summary>
internal static class MoveInfo4
{
    public static ReadOnlySpan<byte> MovePP_DP =>
    [
        00, 35, 25, 10, 15, 20, 20, 15, 15, 15, 35, 30, 05, 10, 30, 30, 35, 35, 20, 15,
        20, 20, 15, 20, 30, 05, 25, 15, 15, 15, 25, 20, 05, 35, 15, 20, 20, 20, 15, 30,
        35, 20, 20, 30, 25, 40, 20, 15, 20, 20, 20, 30, 25, 15, 30, 25, 05, 15, 10, 05,
        20, 20, 20, 05, 35, 20, 25, 20, 20, 20, 15, 25, 15, 10, 40, 25, 10, 35, 30, 15,
        20, 40, 10, 15, 30, 15, 20, 10, 15, 10, 05, 10, 10, 25, 10, 20, 40, 30, 30, 20,
        20, 15, 10, 40, 15, 10, 30, 20, 20, 10, 40, 40, 30, 30, 30, 20, 30, 10, 10, 20,
        05, 10, 30, 20, 20, 20, 05, 15, 10, 20, 15, 15, 35, 20, 15, 10, 20, 30, 15, 40,
        20, 15, 10, 05, 10, 30, 10, 15, 20, 15, 40, 40, 10, 05, 15, 10, 10, 10, 15, 30,
        30, 10, 10, 20, 10, 01, 01, 10, 10, 10, 05, 15, 25, 15, 10, 15, 30, 05, 40, 15,
        10, 25, 10, 30, 10, 20, 10, 10, 10, 10, 10, 20, 05, 40, 05, 05, 15, 05, 10, 05,
        15, 10, 10, 10, 20, 20, 40, 15, 10, 20, 20, 25, 05, 15, 10, 05, 20, 15, 20, 25,
        20, 05, 30, 05, 10, 20, 40, 05, 20, 40, 20, 15, 35, 10, 05, 05, 05, 15, 05, 20,
        05, 05, 15, 20, 10, 05, 05, 15, 15, 15, 15, 10, 10, 10, 20, 10, 10, 10, 10, 15,
        15, 15, 10, 20, 20, 10, 20, 20, 20, 20, 20, 10, 10, 10, 20, 20, 05, 15, 10, 10,
        15, 10, 20, 05, 05, 10, 10, 20, 05, 10, 20, 10, 20, 20, 20, 05, 05, 15, 20, 10,
        15, 20, 15, 10, 10, 15, 10, 05, 05, 10, 15, 10, 05, 20, 25, 05, 40, 10, 05, 40,
        15, 20, 20, 05, 15, 20, 30, 15, 15, 05, 10, 30, 20, 30, 15, 05, 40, 15, 05, 20,
        05, 15, 25, 40, 15, 20, 15, 20, 15, 20, 10, 20, 20, 05, 05, 10, 05, 40, 10, 10,
        05, 10, 10, 15, 10, 20, 30, 30, 10, 20, 05, 10, 10, 15, 10, 10, 05, 15, 05, 10,
        10, 30, 20, 20, 10, 10, 05, 05, 10, 05, 20, 10, 20, 10, 15, 10, 20, 20, 20, 15,
        15, 10, 15, 20, 15, 10, 10, 10, 20, 05, 30, 05, 10, 15, 10, 10, 05, 20, 30, 10,
        30, 15, 15, 15, 15, 30, 10, 20, 15, 10, 10, 20, 15, 05, 05, 15, 15, 05, 10, 05,
        20, 05, 15, 20, 05, 20, 20, 20, 20, 10, 20, 10, 15, 20, 15, 10, 10, 05, 10, 05,
        05, 10, 05, 05, 10, 05, 05, 05,
    ];
   
    public static ReadOnlySpan<byte> MovePower_DP => new byte[]
 {
        00,40,50,15,18,80,40,75,75,75,40,55,01,80,00,50,40,60,00,90,15,80,35,65,30,120,85,60,00,70,65,15,01,35,85,15,90,90,120,00,15,25,14,00,60,00,00,00,00,01,00,40,40,95,00,40,120,95,95,120,65,65,65,150,35,80,80,01,01,01,80,20,40,00,00,55,120,00,00,00,90,00,01,15,40,95,00,120,50,100,01,80,00,50,90,00,00,00,40,20,00,01,00,00,00,00,00,00,00,00,00,00,00,00,00,00,00,01,00,00,200,100,20,20,65,65,120,80,35,60,100,20,10,00,00,00,100,00,100,00,15,20,00,140,00,20,70,00,00,01,00,00,90,250,18,50,00,75,80,00,00,80,01,70,00,50,00,10,40,00,00,00,60,40,00,01,00,100,00,01,00,40,00,40,00,60,00,00,90,20,65,00,120,00,00,00,55,00,25,00,120,00,60,00,00,30,40,00,00,65,10,70,00,00,00,00,01,01,01,00,00,100,01,100,120,60,00,00,40,20,00,100,50,70,00,00,00,01,100,40,00,00,80,01,00,80,60,80,80,40,15,10,40,50,00,01,00,100,00,00,00,00,00,70,150,60,00,00,00,00,00,00,00,00,00,00,120,00,00,60,75,00,20,01,150,00,00,00,00,00,70,80,15,00,00,70,70,00,00,85,00,30,60,00,90,50,75,150,150,100,30,50,00,00,55,140,00,50,60,00,00,00,00,150,75,60,80,85,15,01,95,10,60,10,00,00,00,80,150,00,85,55,50,40,120,60,00,00,90,00,25,60,60,120,140,00,00,00,60,100,01,00,65,01,50,60,00,00,01,70,120,50,50,00,01,00,01,00,01,00,00,00,00,00,00,00,01,130,00,80,00,00,00,00,120,60,90,00,80,80,70,90,80,75,80,90,90,100,70,60,40,120,80,120,90,00,150,00,40,60,40,70,65,65,65,40,65,70,80,65,80,90,00,00,140,80,80,140,120,150,70,120,80,60,100,00,00,01,60,100,60,50,120,40,90,00,00,150,35,150,100,00,01,120,00,120,60,120,100,100,100
 };
    public static ReadOnlySpan<byte> MoveAccuracy_DP => new byte[] { 00, 100, 100, 85, 85, 85, 100, 100, 100, 100, 100, 100, 30, 100, 00, 95, 100, 100, 100, 95, 75, 75, 100, 100, 100, 75, 95, 85, 100, 100, 100, 85, 30, 95, 100, 85, 85, 100, 100, 100, 100, 100, 85, 100, 100, 100, 100, 55, 55, 90, 80, 100, 100, 100, 00, 100, 80, 100, 100, 70, 100, 100, 100, 90, 100, 100, 80, 100, 100, 100, 100, 100, 100, 90, 00, 95, 100, 75, 75, 75, 100, 95, 100, 70, 100, 100, 100, 70, 90, 100, 30, 100, 85, 100, 100, 70, 00, 00, 100, 100, 00, 100, 00, 85, 00, 00, 00, 00, 100, 100, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 100, 75, 100, 70, 100, 85, 85, 100, 75, 00, 100, 100, 100, 00, 80, 00, 90, 75, 100, 55, 85, 100, 75, 90, 00, 100, 100, 100, 100, 80, 00, 00, 85, 100, 80, 90, 00, 90, 90, 00, 00, 100, 90, 100, 00, 00, 00, 90, 100, 00, 00, 100, 100, 100, 00, 100, 00, 95, 85, 100, 100, 100, 00, 100, 90, 00, 75, 00, 100, 100, 85, 00, 50, 00, 00, 00, 95, 00, 80, 00, 100, 00, 100, 00, 100, 90, 100, 90, 00, 100, 95, 90, 00, 100, 00, 00, 100, 90, 100, 00, 00, 95, 100, 50, 85, 100, 00, 100, 100, 100, 100, 75, 95, 00, 00, 00, 00, 100, 80, 100, 00, 00, 100, 100, 00, 100, 100, 100, 90, 100, 70, 100, 100, 100, 00, 100, 00, 90, 00, 100, 100, 75, 100, 100, 100, 100, 00, 00, 00, 100, 00, 100, 00, 00, 00, 00, 100, 00, 00, 100, 100, 00, 100, 100, 100, 00, 00, 00, 00, 00, 100, 100, 100, 00, 00, 100, 100, 100, 100, 90, 00, 90, 100, 00, 100, 100, 95, 90, 90, 85, 100, 100, 00, 100, 95, 90, 00, 80, 100, 85, 55, 100, 00, 100, 100, 00, 100, 90, 70, 30, 85, 100, 00, 100, 00, 00, 00, 100, 90, 00, 85, 95, 100, 100, 100, 00, 00, 00, 100, 00, 80, 00, 100, 85, 90, 00, 00, 00, 100, 90, 100, 00, 100, 100, 100, 100, 00, 00, 100, 100, 100, 100, 100, 100, 100, 90, 00, 100, 100, 00, 100, 00, 00, 00, 00, 00, 100, 100, 100, 100, 00, 00, 00, 00, 100, 100, 00, 00, 100, 100, 100, 90, 100, 95, 100, 100, 100, 75, 100, 100, 100, 70, 100, 100, 100, 100, 90, 00, 100, 100, 100, 100, 95, 95, 95, 100, 85, 100, 90, 85, 100, 85, 00, 00, 90, 100, 100, 90, 85, 90, 100, 70, 100, 00, 80, 100, 00, 100, 100, 100, 100, 90, 100, 100, 100, 00, 00, 80, 90, 90, 95, 00, 100, 70, 80, 85, 100, 100, 100, 100, 100 };
    public static ReadOnlySpan<byte> MoveCategory_DP => new byte[] { 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 02, 00, 01, 02, 01, 00, 01, 01, 01, 01, 01, 01, 01, 01, 01, 00, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 00, 01, 01, 01, 00, 01, 00, 00, 00, 00, 02, 00, 02, 02, 02, 00, 02, 02, 02, 02, 02, 02, 02, 02, 02, 01, 01, 01, 01, 01, 01, 01, 02, 02, 00, 00, 01, 02, 00, 00, 00, 02, 00, 02, 02, 02, 02, 00, 02, 01, 01, 01, 01, 00, 02, 02, 00, 00, 00, 01, 01, 00, 02, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 01, 00, 00, 01, 01, 01, 02, 02, 01, 02, 01, 01, 02, 01, 01, 01, 00, 00, 00, 01, 00, 02, 00, 01, 01, 00, 01, 00, 02, 01, 00, 00, 02, 00, 00, 01, 01, 01, 01, 00, 01, 01, 00, 00, 02, 01, 01, 00, 01, 00, 01, 01, 00, 00, 00, 01, 02, 00, 01, 00, 02, 00, 01, 00, 02, 00, 01, 00, 01, 00, 00, 02, 02, 02, 00, 02, 00, 00, 00, 02, 00, 01, 00, 01, 00, 02, 00, 00, 01, 01, 00, 00, 01, 01, 01, 00, 00, 00, 00, 01, 01, 01, 00, 00, 01, 01, 01, 01, 02, 00, 00, 01, 01, 00, 01, 01, 01, 00, 00, 00, 02, 01, 02, 00, 00, 01, 02, 00, 01, 02, 02, 02, 01, 02, 01, 01, 02, 00, 02, 00, 02, 00, 00, 00, 00, 00, 01, 01, 01, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 01, 00, 00, 01, 01, 00, 01, 01, 02, 00, 00, 00, 00, 00, 01, 01, 01, 00, 00, 02, 02, 00, 00, 01, 00, 01, 01, 00, 02, 01, 01, 02, 02, 01, 01, 02, 00, 00, 02, 02, 00, 01, 02, 00, 00, 00, 00, 02, 02, 01, 02, 01, 01, 02, 02, 01, 01, 01, 00, 00, 00, 01, 02, 00, 01, 02, 01, 01, 01, 02, 00, 00, 01, 00, 01, 02, 02, 02, 02, 00, 00, 00, 01, 01, 01, 00, 02, 01, 01, 01, 00, 00, 01, 01, 01, 01, 01, 00, 01, 00, 02, 00, 02, 00, 00, 00, 00, 00, 00, 00, 01, 01, 00, 01, 00, 00, 00, 00, 01, 01, 02, 00, 01, 02, 01, 01, 01, 02, 01, 02, 02, 01, 02, 01, 02, 02, 02, 01, 02, 00, 01, 00, 01, 01, 01, 01, 01, 01, 01, 01, 02, 01, 01, 02, 02, 01, 00, 00, 02, 02, 02, 02, 01, 01, 01, 01, 01, 01, 01, 00, 00, 02, 02, 02, 01, 02, 01, 01, 01, 00, 00, 01, 01, 02, 02, 00, 01, 02, 00, 02, 02, 01, 02, 02, 02 };

}
