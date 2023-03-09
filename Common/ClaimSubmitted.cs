namespace Common
{
    using System;

    public record ClaimSubmitted
    {
        public Guid ClaimId { get; init; }
        public int Index { get; init; }
        public int Count { get; init; }
        public string Content { get; init; }
    }
}
