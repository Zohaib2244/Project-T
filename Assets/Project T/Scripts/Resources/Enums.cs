namespace Scripts.Resources
{
    public enum Panels
    {
        LoginPanel,
        TournamentSelectionPanel,
        TournamentInfoPanel,
        TournamentHomePanel,
        CRUDSpeakerPanel,
        CRUDAdjudicatorPanel,
        CRUDTeamPanel,
        CRUInstitutionPanel,
        TicketRequestPanel,
        RoundsPanel,
        RankingsPanel,
        BreaksPanel,
        EmailPanel,
    }
    public enum TournamentType
    {
        British,
        Asian,
    }
    public enum SpeakerTypes
    {
        Open,
        Novice,
    }
    public enum AdjudicatorTypes
    {
        CAP,
        Normie,
    }
    public enum BreakTypes
    {
        QF,
        SF,
        F,
    }
    public enum AdminCategories
    {
        Super,
        Assistant,
    }
    public enum UserCategories
    {
        Speaker,
        Adjudicator,
    }
    public enum TeamPositionsBritish
    {
        OG,
        OO,
        CG,
        CO,
    }
    public enum TeamPositionsAsian
    {
        Gov,
        Opp,
    }
    public enum RoundTypes
    {
        PreLim,
        QF,
        SF,
        F,
    }
    public enum RoundCategory
    {
        PreLim,
        NoviceBreak,
        OpenBreak,
    }
    public enum RoundStates
    {
        NotStarted, //0
        InProgress, //1
        Completed //2
    }
    public enum RoundPanelTypes
    {
        MotionsPanel,
        AttendancePanel,
        DrawsPanel,
        BallotsPanel,
        None,
    }
    public enum DrawPanelTypes
    {
        DrawOptionsPanel,
        DrawDisplayPanel,
        DrawEditPanel,
    }
    public enum BreakParameters
    {
        TeamScore,
        TeamPoints
    }
    public enum EmailTypes
    {
        Speaker,
        Adjudicator,
        Feedback,
    }
}