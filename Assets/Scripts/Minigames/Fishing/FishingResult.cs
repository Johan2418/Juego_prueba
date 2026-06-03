using UnityEngine;

namespace MantaMinigames.Fishing
{
    // Resultado inmutable que el minijuego puede devolver a otros sistemas.
    public enum FishingResultStatus
    {
        InProgress = -1,
        Failed = 0,
        Success = 1
    }

    public readonly struct FishingResult
    {
        public FishingResult(FishingResultStatus status, FishingRewardData reward, string message, float normalizedProgress, int remainingAttempts = 0)
        {
            Status = status;
            Reward = status == FishingResultStatus.Success ? reward : null;
            Message = string.IsNullOrWhiteSpace(message) ? string.Empty : message;
            NormalizedProgress = Mathf.Clamp01(normalizedProgress);
            RemainingAttempts = Mathf.Max(0, remainingAttempts);
        }

        public FishingResultStatus Status { get; }
        public FishingRewardData Reward { get; }
        public string Message { get; }
        public float NormalizedProgress { get; }
        public int RemainingAttempts { get; }
        public bool IsSuccess => Status == FishingResultStatus.Success;
    }
}
