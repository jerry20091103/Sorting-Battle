using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SortGame.GameFunctions;
using ChocoUtil.Algorithms;
namespace SortGame
{
    public class GameBoardState
    {
        public struct Config
        {
            public int seed;
            public int rowCount, columnCount;
            public int minimumSortedLength;
            public int baseRemoveScore;
            public int removeLengthBonus;
            public int maxEffectiveCombo;
            public float comboScoreStep;
        }
        public readonly GameGridState gameGridState;
        public Random.State randomState;
        public readonly GameControllerState gameControllerState;
        public readonly GameScoreState gameScoreState;
        public GameBoardState(Config config)
        {
            Random.InitState(config.seed);
            randomState = Random.state;
            gameGridState = new(config.rowCount, config.columnCount);
            gameControllerState = new(gameGridState, config.minimumSortedLength);
            // Forward parameters to GameScoreState.
            gameScoreState = new(
                new(){
                    minimumRemoveCount = config.minimumSortedLength,
                    baseRemoveScore = config.baseRemoveScore,
                    removeLengthBonus = config.removeLengthBonus,
                    maxEffectiveCombo = config.maxEffectiveCombo,
                    comboScoreStep = config.comboScoreStep,
                }
            );
            // Tell GameScoreState whenever a remove event happens.
            gameControllerState.onRemove += gameScoreState.OnRemove;
        }
        // ! A GameBoardState can have multiple events that must be triggered manually.
        // ! The environment is supposed to invoke them appropriately according to the specification.

        public (List<Vector2Int> newBlocks, bool overflow) 
        PushNewRow(int numberOfColumns)
        {
            var columns = RandLib.RandomIntegerSequence(0, gameGridState.columnCount);
            bool anyOverflow = false;
            List<Vector2Int> newBlocks = new();
            foreach(var column in columns[0..Mathf.Min(numberOfColumns, columns.Length)])
            {
                newBlocks.Add(new(gameGridState.rowCount - 1, column));
                bool overflow = gameGridState.PushUp(column, Random.Range(0, 100));
                anyOverflow |= overflow;
            }
            return (newBlocks, anyOverflow);
        }
        
    }
}
