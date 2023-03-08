namespace Common
{
    using System;


    public record ClaimSubmitted
    {
        public Guid ClaimId { get; init; }
        public string Content { get; init; }
    }
}
