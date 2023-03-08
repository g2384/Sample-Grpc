namespace Contracts
{
    using System;


    public record SubmitClaim
    {
        public string Content { get; init; }
        public int Index { get; init; }
        public int Count { get; init; }
    }
}