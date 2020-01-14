

namespace Harris.Automation.ADC.Services.ListService.Source.Features.BreakAway
{
    public struct CommentIds
    {
        public const string BreakAway = "BREAKAWAY";
        public const string Return = "RETURN";
    }

    public struct CommentTypes
    {
        public const string Immediate = "IMMEDIATE";
        public const string Jip = "JIP";
        public const string FromBreakAway = "FROM BREAK AWAY";
    }

    public enum ReturnType
    {
        Unspecified = 0,
        Slide = 1,
        Jip = 2,
    }
}
