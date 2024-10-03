namespace GameManager
{

    public enum ProgressStatus
    {
        DOUBLE_REQUEST,
        NORMAL_PROGRESS,
        WAIT_FOR_MOVE,
        JUST_STARTED,
    }

    public enum FinishStatus
    {
        Resign,
        DoubleReject,
        FinishGame
    }

    public enum Point
    {
        OUT = 0,
        Z11 = 1,
        Z12 = 2,
        Z13 = 3,
        Z14 = 4,
        Z15 = 5,
        Z16 = 6,
        Z21 = 7,
        Z22 = 8,
        Z23 = 9,
        Z24 = 10,
        Z25 = 11,
        Z26 = 12,
        Z31 = 13,
        Z32 = 14,
        Z33 = 15,
        Z34 = 16,
        Z35 = 17,
        Z36 = 18,
        Z41 = 19,
        Z42 = 20,
        Z43 = 21,
        Z44 = 22,
        Z45 = 23,
        Z46 = 24,
        HOME = 25
    }

    public enum Dice {
        ONE = 1,
        TWO = 2,
        THREE = 3,
        FOUR = 4,
        FIVE = 5,
        SIX = 6
    }

    public enum DoubleRequestStatus
    {
        NO_REQUEST,
        DOUBLE_REQUESTED,
    }

    public enum DoubleResponseStatus
    {
        NO_REQUEST,
        DOUBLE_ACCEPTED,
        DOUBLE_REJECTED
    }

    public enum Colour
    {
        White,
        Black
    }
}