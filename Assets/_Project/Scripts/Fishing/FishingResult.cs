public readonly struct FishingResult
{
    public bool Success { get; }
    public FishData Fish { get; }
    public string Message { get; }

    public FishingResult(bool success, FishData fish, string message)
    {
        Success = success;
        Fish = fish;
        Message = message;
    }
}
