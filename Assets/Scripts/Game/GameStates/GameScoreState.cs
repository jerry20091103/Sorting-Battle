using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SortGame
{
    public class GameScoreState
    {
        public int totalScore { get; private set; } = 0;
        public int combo { get; private set; } = 0;
        public int effectiveCombo => Mathf.Min(combo, maxEffectiveCombo);
        public int comboScoreBuffer { get; private set; } = 0;
        public event System.Action<int> onComboReset;
        public event System.Action<int> onScoreIncrease;
        public struct Config
        {
            public int minimumRemoveCount, baseRemoveScore, maxEffectiveCombo, removeLengthBonus;
            public float comboScoreStep;
        }
        private int minimumRemoveCount, baseRemoveScore, maxEffectiveCombo, removeLengthBonus;
        private float comboScoreStep;
        public GameScoreState(Config config)
        {
            this.minimumRemoveCount = config.minimumRemoveCount;
            this.baseRemoveScore = config.baseRemoveScore;
            this.maxEffectiveCombo = config.maxEffectiveCombo;
            this.removeLengthBonus = config.removeLengthBonus;
            this.comboScoreStep = config.comboScoreStep;
        }
        public void OnRemove(int removeCount)
        {
            if(removeCount < minimumRemoveCount)
            {
                // Failed remove. Maybe reset combo?
                ResetCombo();
            }
            else
            {
                // Successful remove.
                RegisterCombo(removeCount);
            }
        }
        private void RegisterCombo(int removeCount)
        {
            ++combo;
            int plusScore = Mathf.CeilToInt(effectiveCombo / comboScoreStep) * (baseRemoveScore + (removeCount - minimumRemoveCount) * removeLengthBonus);
            totalScore += plusScore;
            comboScoreBuffer += plusScore;
            onScoreIncrease?.Invoke(plusScore);
        }

        private void ResetCombo()
        {
            combo = 0;
            onComboReset?.Invoke(comboScoreBuffer);
            comboScoreBuffer = 0;
        }
    }
}
