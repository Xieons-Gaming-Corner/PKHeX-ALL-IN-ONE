﻿using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public abstract class Learnset
    {
        protected int Count;
        protected int[] Moves;
        protected int[] Levels;

        /// <summary>
        /// Returns the moves a Pokémon can learn between the specified level range.
        /// </summary>
        /// <param name="maxLevel">Maximum level</param>
        /// <param name="minLevel">Minimum level</param>
        /// <returns>Array of Move IDs</returns>
        public int[] GetMoves(int maxLevel, int minLevel = 0)
        {
            if (minLevel <= 1 && maxLevel >= 100)
                return Moves;
            if (minLevel > maxLevel)
                return new int[0];
            int start = Array.FindIndex(Levels, z => z >= minLevel);
            if (start < 0)
                return new int[0];
            int end = Array.FindLastIndex(Levels, z => z <= maxLevel);
            if (end < 0)
                return new int[0];
            int[] result = new int[end - start + 1];
            Array.Copy(Moves, start, result, 0, result.Length);
            return result;
        }

        /// <summary>Returns the moves a Pokémon would have if it were encountered at the specified level.</summary>
        /// <remarks>In Generation 1, it is not possible to learn any moves lower than these encounter moves.</remarks>
        /// <param name="level">The level the Pokémon was encountered at.</param>
        /// <returns>Array of Move IDs</returns>
        public int[] GetEncounterMoves(int level)
        {
            const int count = 4;
            var moves = new int[count];
            return GetEncounterMoves(level, moves);
        }

        /// <summary>Returns the moves a Pokémon would have if it were encountered at the specified level.</summary>
        /// <remarks>In Generation 1, it is not possible to learn any moves lower than these encounter moves.</remarks>
        /// <param name="level">The level the Pokémon was encountered at.</param>
        /// <param name="moves">Move array to write to</param>
        /// <param name="ctr">Starting index to begin overwriting at</param>
        /// <returns>Array of Move IDs</returns>
        public int[] GetEncounterMoves(int level, int[] moves, int ctr = 0)
        {
            for (int i = 0; i < Moves.Length; i++)
            {
                if (Levels[i] > level)
                    break;
                int move = Moves[i];

                bool alreadyHasMove = false;
                foreach (int m in moves) if (m == move) { alreadyHasMove = true; break; }
                if (alreadyHasMove)
                    continue;

                moves[ctr++] = move;
                ctr &= 3;
            }
            return moves;
        }

        /// <summary>Returns the index of the lowest level move if the Pokémon were encountered at the specified level.</summary>
        /// <remarks>Helps determine the minimum level an encounter can be at.</remarks>
        /// <param name="level">The level the Pokémon was encountered at.</param>
        /// <returns>Array of Move IDs</returns>
        public int GetMinMoveLevel(int level)
        {
            if (Levels.Length == 0)
                return 1;

            int end = Array.FindLastIndex(Levels, z => z <= level);
            return Math.Max(end - 4, 1);
        }


        private Dictionary<int, int> Learn;
        private Dictionary<int, int> GetDictionary()
        {
            var dict = new Dictionary<int, int>();
            for (int i = 0; i < Moves.Length; i++)
                if (!dict.ContainsKey(Moves[i]))
                    dict.Add(Moves[i], Levels[i]);
            return dict;
        }

        /// <summary>Returns the level that a Pokémon can learn the specified move.</summary>
        /// <param name="move">Move ID</param>
        /// <returns>Level the move is learned at. If the result is below 0, it cannot be learned by levelup.</returns>
        public int GetLevelLearnMove(int move)
        {
            return (Learn ?? (Learn = GetDictionary())).TryGetValue(move, out var level) ? level : -1;
        }

        /// <summary>Returns the level that a Pokémon can learn the specified move.</summary>
        /// <param name="move">Move ID</param>
        /// <param name="min">Minimum level to start looking at.</param>
        /// <returns>Level the move is learned at. If the result is below 0, it cannot be learned by levelup.</returns>
        public int GetLevelLearnMove(int move, int min)
        {
            for (int i = 0; i < Moves.Length; i++)
            {
                if (move != Moves[i])
                    continue;

                var lv = Levels[i];
                if (lv >= min)
                    return lv;
            }
            return -1;
        }
    }
}
