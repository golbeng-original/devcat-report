using System.Text;

namespace Task1;

#region FrameState (Frame의 상태를 Enum값으로 관리)
public enum FrameState
{
    /// <summary>
    /// Frame 최종 상태가 결정 되지 않았다.(Pending 상태와 동일)
    /// </summary>
    None,
    OpenFrame,
    Strike,
    Spare,
    CompleteLastFrame
}

public static class FrameStateExtension
{
    /// <summary>
    /// FrameState에 따라서 현재 프레임 점수 계산에 필요한 연관 투구수가 다르다.(코드 응집도로 위한 확장 메서드 사용)
    /// </summary>
    public static int GetAssoicateRollCount(this FrameState frameState)
    {
        switch (frameState)
        {
            case FrameState.Strike:
                return 2;
            case FrameState.Spare:
                return 1;
            default:
                return 0;
        }
    }
}
#endregion

#region FrameStateRules(한 프레임의 OpenFrame, Strike, Spare 규칙 처리 담당)
public interface IFrameStateRule
{
    /// <summary>
    /// 쓰러뜨린 pin의 상태를 기반으로 frame이 완료 되었는지 판단
    /// </summary>
    public bool IsPending(IReadOnlyList<int> pins);

    /// <summary>
    /// 쓰러뜨린 pin의 상태를 기반으로 frame이 상태 결정
    /// </summary>
    public FrameState GetFrameState(IReadOnlyList<int> pins);

    /// <summary>
    /// 이전까지 pin의 상태와 새로운 쓰러뜨린 pin의 갯수를 기반으로 유효성 검증
    /// </summary>
    public bool IsVerifyPinFallCount(IReadOnlyList<int> pins, int addPinFallCount);
}

/// <summary>
/// 마지막 프레임에 대한 상태 결정 및 PinFall에 대한 유효성 검증
/// </summary>
public class BonusFrameStateRule : IFrameStateRule
{
    public bool IsPending(IReadOnlyList<int> pins)
    {
        return GetFrameState(pins) == FrameState.None;
    }

    public FrameState GetFrameState(IReadOnlyList<int> pins)
    {
        if (pins.Count == 2 && pins.Sum() < 10)
        {
            return FrameState.CompleteLastFrame;
        }
        else if (pins.Count == 3)
        {
            return FrameState.CompleteLastFrame;
        }

        return FrameState.None;
    }

    public bool IsVerifyPinFallCount(IReadOnlyList<int> pins, int addPinFallCount)
    {
        if (addPinFallCount > 10)
        {
            return false;
        }

        if (pins.Count == 3)
        {
            return false;
        }

        if (pins.Count == 2 && pins.Sum() < 10)
        {
            return false;
        }

        if (pins.Count == 1 && pins[0] != 10 && pins[0] + addPinFallCount > 10)
        {
            return false;
        }

        return true;
    }
}

/// <summary>
/// 일반 프레임에 대한 상태 결정 및 PinFall에 대한 유효성 검증
/// </summary>
public class NormalFrameStateRule : IFrameStateRule
{
    public bool IsPending(IReadOnlyList<int> pins)
    {
        return GetFrameState(pins) == FrameState.None;
    }

    public FrameState GetFrameState(IReadOnlyList<int> pins)
    {
        if (pins.Count == 1 && pins[0] == 10)
        {
            return FrameState.Strike;
        }
        else if (pins.Count == 2 && pins.Sum() == 10)
        {
            return FrameState.Spare;
        }
        else if (pins.Count == 2 && pins.Sum() < 10)
        {
            return FrameState.OpenFrame;
        }

        return FrameState.None;
    }

    public bool IsVerifyPinFallCount(IReadOnlyList<int> pins, int addPinFallCount)
    {
        if (addPinFallCount > 10)
        {
            return false;
        }

        if (pins.Count == 1 && pins[0] == 10)
        {
            return false;
        }

        if (pins.Count == 1 && pins[0] + addPinFallCount > 10)
        {
            return false;
        }

        return true;
    }
}
#endregion

#region FrameInfo, FrameScoreInfo (한 프레임에 대한 상태 업데이트 및 관리를 위한 객체들)
/// <summary>
/// 한 프레임에 대한 상태 및 Roll별 PinFall 갯수 정보
/// - FrameInfo으로부터 생성
/// </summary>
public struct FrameStateInfo
{
    public IReadOnlyList<int> PinFalls;

    public FrameState FrameState;
}

/// <summary>
/// 한 프레임에 대한 상태 업데이트 및 상태 조회
/// </summary>
public class FrameInfo
{
    private readonly IFrameStateRule stateRule;
    public int FrameNum { get; private set; } = 0;
    public bool IsLastFrame { get; private set; } = false;
    public bool IsPending { get; private set; } = true;

    public FrameStateInfo FrameStateInfo
    {
        get
        {
            return new FrameStateInfo()
            {
                PinFalls = this.pinFalls,
                FrameState = stateRule.GetFrameState(pinFalls)
            };
        }
    }

    // Roll 별로 쓰러뜨린 핀 기록
    private List<int> pinFalls = [];

    public FrameInfo(int frameNum, IFrameStateRule stateRule, bool isLastFrame)
    {
        this.FrameNum = frameNum;
        this.stateRule = stateRule;
        this.IsLastFrame = isLastFrame;
    }

    public void UpdatePinFall(int count)
    {
        if (IsPending == false)
        {
            throw new Exception("플레이가 완료 된 Frame 입니다.");
        }

        if (stateRule.IsVerifyPinFallCount(this.pinFalls, count) == false)
        {
            throw new Exception("올바르지 않은 pinFall 입니다.");
        }

        pinFalls.Add(count);

        IsPending = stateRule.IsPending(this.pinFalls);
    }

    public override string ToString()
    {
        var pinFallsString = string.Join(",", pinFalls);
        return $"[{nameof(FrameNum)}: {FrameNum}][FrameState: {this.FrameStateInfo.FrameState}][pinFalls: {pinFallsString}]";
    }
}

/// <summary>
/// 볼링 플레이 중에 생성된 FrameInfo들을 Round 순서로 관리
/// </summary>
public class FrameInfoCollection
{
    private SortedDictionary<int, FrameInfo> frames = new();

    public IEnumerable<FrameInfo> Frames => frames.Values;

    public int Count => frames.Count;

    public void Add(FrameInfo frameInfo)
    {
        frames[frameInfo.FrameNum] = frameInfo;
    }

    public FrameInfo? FindFrameInfoByFrameNum(int frameNum)
    {
        return frames.GetValueOrDefault(frameNum);
    }

    public FrameInfo? FindLatestFrame()
    {
        return frames.Values.LastOrDefault();
    }

    /// <summary>
    /// 특정 프레임부턴 투구별 PinFall 목록을 가져온다.(요청한 투구 횟수를 채우지 못하면 null 반환)
    /// </summary>
    public IList<int>? GetPinFalls(int beginFrameNum, int rollCount)
    {
        if (rollCount == 0)
        {
            return [];
        }

        var filteredFrames = frames.Values.SkipWhile(e => e.FrameNum < beginFrameNum).ToList();

        var pinFalls = new List<int>();
        foreach (var filteredFrame in filteredFrames)
        {
            var frameStateInfo = filteredFrame.FrameStateInfo;
            foreach (var pinPall in frameStateInfo.PinFalls)
            {
                pinFalls.Add(pinPall);
                if (pinFalls.Count == rollCount)
                {
                    return pinFalls;
                }
            }
        }

        return null;
    }

}

public static class FrameInfoCollectionExtension
{
    public static FrameScoreInfoCollection ToFrameScoreInfoCollection(this FrameInfoCollection collection)
    {
        var scoreInfoCollection = new FrameScoreInfoCollection();

        var frameNums = collection.Frames.Select(e => e.FrameNum);
        foreach (var frameNum in frameNums)
        {
            scoreInfoCollection.Add(frameNum, collection);
        }

        return scoreInfoCollection;
    }
}

/// <summary>
/// FrameInfo 상태를 기반으로 Frame 별로 점수 계산한 결과 정보 
/// </summary>
public struct FrameScoreInfo : IEquatable<FrameScoreInfo>
{
    public int FrameNum;
    public int FrameScore;
    public int AccScore;

    public override string ToString()
    {
        return $"[{nameof(FrameNum)} : {FrameNum}][{nameof(FrameScore)} : {FrameScore}][{nameof(AccScore)} : {AccScore}]";
    }

    public readonly bool Equals(FrameScoreInfo other)
    {
        if (this.FrameNum != other.FrameNum)
        {
            return false;
        }

        if (this.FrameScore != other.FrameScore)
        {
            return false;
        }

        if (this.AccScore != other.AccScore)
        {
            return false;
        }

        return true;
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is FrameScoreInfo && Equals((FrameScoreInfo)obj);
    }

    public override int GetHashCode()
    {
        return this.FrameNum.GetHashCode() ^ this.FrameScore.GetHashCode() ^ this.AccScore.GetHashCode();
    }

    public static bool operator ==(FrameScoreInfo lhs, FrameScoreInfo rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(FrameScoreInfo lhs, FrameScoreInfo rhs)
    {
        return !lhs.Equals(rhs);
    }

    public static FrameScoreInfo Default = new()
    {
        FrameNum = -1,
        FrameScore = -1,
        AccScore = -1
    };


}


/// <summary>
/// FrameScoreInfo 생성을 담당하며, 생성 된 FrameScoreInfo 목록 Round 별로 관리
/// </summary>
public class FrameScoreInfoCollection
{
    private SortedDictionary<int, FrameScoreInfo> frameScoreInfos = new();

    public IEnumerable<FrameScoreInfo> FrameScores => frameScoreInfos.Values;

    /// <summary>
    /// 특정 프레임에 대한 점수를 계산하기 위해선, 특정 프레임 +1, +2의 프레임의 정보까지 필요하다. 
    /// </summary>
    public void Add(int targetFrameNum, FrameInfoCollection frameInfoCollection)
    {
        var frameScoreInfo = GenerateFrameScoreInfo(targetFrameNum, frameInfoCollection);
        if (frameScoreInfo == FrameScoreInfo.Default)
        {
            return;
        }

        frameScoreInfos[frameScoreInfo.FrameNum] = frameScoreInfo;
    }

    private FrameScoreInfo GenerateFrameScoreInfo(int targetFrameNum, FrameInfoCollection frameInfoCollection)
    {
        // 프레임을 찾는다.
        var targetFrameInfo = frameInfoCollection.FindFrameInfoByFrameNum(targetFrameNum);
        if (targetFrameInfo == null || targetFrameInfo.IsPending == true)
        {
            return FrameScoreInfo.Default;
        }

        var targetFrameStateInfo = targetFrameInfo.FrameStateInfo;

        // 프레임 점수 계산을 위한 프레임 상태에 따른 필요한 추가 투구수를 가져온다.
        var associateRollCount = targetFrameStateInfo.FrameState.GetAssoicateRollCount();

        // 특정 프레임부터 필요한 투구별로 PinFall 목록를 가져온다.
        var associatepinFalls = frameInfoCollection.GetPinFalls(targetFrameNum + 1, associateRollCount);
        if (associatepinFalls == null)
        {
            return FrameScoreInfo.Default;
        }

        // 현재 프레임의 최종 점수 계산 및 누적 점수 계산
        var currFrameScore = targetFrameStateInfo.PinFalls.Sum() + associatepinFalls.Sum();
        var prevAccScore = 0;
        if (frameScoreInfos.ContainsKey(targetFrameNum - 1) == true)
        {
            prevAccScore = frameScoreInfos[targetFrameNum - 1].AccScore;
        }

        return new FrameScoreInfo()
        {
            FrameNum = targetFrameNum,
            FrameScore = currFrameScore,
            AccScore = currFrameScore + prevAccScore
        };
    }
}

/// <summary>
/// FrameInfo를 주입된 최대 프레임 갯수만큼만 생성하도록 하는 생성 책임 객체
/// </summary>
public class FrameInfoFactory
{
    private int currFrame = 0;
    private int maxFrame = 0;

    public FrameInfoFactory(int maxFrame = 10)
    {
        this.maxFrame = maxFrame;
    }

    public FrameInfo GenerateFrameInfo()
    {
        if (currFrame + 1 > this.maxFrame)
        {
            throw new Exception("최대 프레임 생성까지 생성이 끝났습니다.");
        }

        var frame = ++currFrame;
        var isBonusFrame = frame == maxFrame;
        return new FrameInfo(
            frame,
            isBonusFrame ? new BonusFrameStateRule() : new NormalFrameStateRule(),
            isBonusFrame
        );
    }
}
#endregion

#region BowlingRoundOutput(BowlingRound 출력 책임)
public abstract class IBowlingRoundOutput
{
    // 출력 순서는 정해져 있어서 Output의 흐름은 고정적이다.
    // 일반 프레임, 보너스 프레임 출력 영역은 구현체에 의임
    // 일반 프레임 점수 출력, 보너스 프레임 점수 출력 영역은 구현체에 의임
    public void Output(int maxFrame, BowlingRound bowlingRound)
    {
        var frames = bowlingRound.frameCollection.Frames;

        for (int i = 1; i <= maxFrame; i++)
        {
            var frame = frames.FirstOrDefault(e => e.FrameNum == i);
            this.PresentFrame(i, maxFrame, frame);
        }

        var frameScores = bowlingRound.frameScoreInfoCollection.FrameScores;
        for (int i = 1; i <= maxFrame; i++)
        {
            var frameScore = frameScores.FirstOrDefault(e => e.FrameNum == i, FrameScoreInfo.Default);
            this.PresentFrameScore(i, maxFrame, frameScore);
        }
    }

    /// <summary>
    /// 프레임의 PinFall 상태를 출력
    /// </summary>
    protected abstract void PresentFrame(int frameNum, int maxFrame, FrameInfo? frameInfo);

    /// <summary>
    /// 프레임의 누적 점수 출력
    /// </summary>
    protected abstract void PresentFrameScore(int frameNum, int maxFrame, FrameScoreInfo frameScoreInfo);
}

/// <summary>
/// Console 창에 출력을 위한 IBowlingRoundOutput 구현체
/// </summary>
public class ConsoleBowlingRoundOutput : IBowlingRoundOutput
{
    public struct NormalFrameOutput
    {
        private FrameInfo? frameInfo;
        public NormalFrameOutput(FrameInfo? frameInfo)
        {
            this.frameInfo = frameInfo;
        }

        public override string ToString()
        {
            if (frameInfo == null)
            {
                return $"[{' ',3}]";
            }

            var frameStateInfo = frameInfo.FrameStateInfo;
            if (frameStateInfo.FrameState == FrameState.Strike)
            {
                return $"[{' ',1}X{' ',1}]";
            }

            if (frameStateInfo.FrameState == FrameState.Spare)
            {
                return $"[{frameStateInfo.PinFalls[0]},{'/',1}]";
            }

            var openFrameScores = string.Join(",", frameStateInfo.PinFalls);
            if (frameInfo.IsPending == true)
            {
                openFrameScores += ",";
            }

            return $"[{openFrameScores,-3}]";
        }
    }

    public struct BonusFrameOutput
    {
        private FrameInfo? frameInfo;
        public BonusFrameOutput(FrameInfo? frameInfo)
        {
            this.frameInfo = frameInfo;
        }

        public override string ToString()
        {
            if (frameInfo == null)
            {
                return $"[{' ',5}]";
            }

            var frameStateInfo = frameInfo.FrameStateInfo;

            var pinFallPresents = new List<string>();
            var prevPinFall = 0;
            foreach (var pinFall in frameStateInfo.PinFalls)
            {
                if (pinFall == 10)
                {
                    pinFallPresents.Add("X");
                    prevPinFall = 0;
                }
                else if (prevPinFall > 0 && prevPinFall + pinFall == 10)
                {
                    pinFallPresents.Add("/");
                    prevPinFall = 0;
                }
                else
                {
                    pinFallPresents.Add(pinFall.ToString());
                    prevPinFall = pinFall;
                }
            }

            var pinFallPresentString = string.Join(",", pinFallPresents);
            if (frameInfo.IsPending == true)
            {
                pinFallPresentString += ",";
            }

            return $"[{pinFallPresentString,-5}]";
        }
    }

    public struct NormalFrameScoreOutput
    {
        private FrameScoreInfo frameScoreInfo;
        public NormalFrameScoreOutput(FrameScoreInfo frameScoreInfo)
        {
            this.frameScoreInfo = frameScoreInfo;
        }

        public override string ToString()
        {
            if (this.frameScoreInfo == FrameScoreInfo.Default)
            {
                return $"[{' ',3}]";
            }

            return $"[{this.frameScoreInfo.AccScore,3}]";
        }
    }

    public struct BonusFrameScoreOutput
    {
        private FrameScoreInfo frameScoreInfo;
        public BonusFrameScoreOutput(FrameScoreInfo frameScoreInfo)
        {
            this.frameScoreInfo = frameScoreInfo;
        }

        public override string ToString()
        {
            if (this.frameScoreInfo == FrameScoreInfo.Default)
            {
                return $"[{' ',5}]";
            }

            return $"[{this.frameScoreInfo.AccScore,5}]";
        }
    }

    protected override void PresentFrame(int frameNum, int maxFrame, FrameInfo? frameInfo)
    {
        Console.Write($"{frameNum,3}");

        if (frameNum == maxFrame)
        {
            Console.Write(new BonusFrameOutput(frameInfo));
        }
        else
        {
            Console.Write(new NormalFrameOutput(frameInfo));
        }

        if (frameNum == maxFrame)
        {
            Console.WriteLine();
        }
    }
    protected override void PresentFrameScore(int frameNum, int maxFrame, FrameScoreInfo frameScoreInfo)
    {
        if (frameNum == 0)
        {
            Console.WriteLine();
        }

        Console.Write($"{' ',3}");

        if (frameNum == maxFrame)
        {
            Console.Write(new BonusFrameScoreOutput(frameScoreInfo));
        }
        else
        {
            Console.Write(new NormalFrameScoreOutput(frameScoreInfo));
        }

        if (frameNum == maxFrame)
        {
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
#endregion


/// <summary>
/// 볼링 전체 한 라운드 상태를 관리
/// </summary>
public class BowlingRound
{
    private readonly FrameInfoFactory frameInfoFractory;

    public readonly FrameInfoCollection frameCollection = new();

    public FrameScoreInfoCollection frameScoreInfoCollection
    {
        get => frameCollection.ToFrameScoreInfoCollection();
    }

    public bool IsEnd
    {
        get
        {
            var lastFrame = frameCollection.FindLatestFrame();
            if (lastFrame == null)
            {
                return false;
            }

            return lastFrame.IsLastFrame == true && lastFrame.IsPending == false;
        }
    }

    public BowlingRound(FrameInfoFactory frameInfoFactory)
    {
        this.frameInfoFractory = frameInfoFactory;
    }

    public void KnockedDownPins(int downPinCount)
    {
        // FrameInfo 생성 조건
        // 1. 최근 FrameInfo가 존재 하지 않는다.
        // 2. 최근 FrameInfo의 플레이가 끝났다.
        var latestFrame = frameCollection.FindLatestFrame();
        if (latestFrame == null || latestFrame.IsPending == false)
        {
            latestFrame = frameInfoFractory.GenerateFrameInfo();
            frameCollection.Add(latestFrame);
        }

        // 유효성 검증 통과 되었으므로, downPin 처리
        latestFrame.UpdatePinFall(downPinCount);
    }
}

public class Game
{
    private readonly int maxFrame;
    private readonly BowlingRound bowlingRound;
    private readonly IBowlingRoundOutput output;

    public Game(
        IBowlingRoundOutput output,
        int maxFrame = 10
    )
    {
        this.bowlingRound = new(
            new FrameInfoFactory(maxFrame)
        );

        this.output = output;
        this.maxFrame = maxFrame;
    }

    public void KnockedDownPins(int downPinCount)
    {
        try
        {
            if (bowlingRound.IsEnd == true)
            {
                throw new Exception("모든 볼링 플레이를 완료 했습니다.");
            }

            bowlingRound.KnockedDownPins(downPinCount);
            output.Output(maxFrame, bowlingRound);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[error] {e.Message}");
        }
    }
}

public static class Program
{
    public static void Main()
    {
        var game = new Game(
            new ConsoleBowlingRoundOutput()
        );
        game.KnockedDownPins(4);
        game.KnockedDownPins(6);
        game.KnockedDownPins(5);
        game.KnockedDownPins(5);
        game.KnockedDownPins(10);
        game.KnockedDownPins(6);
    }
}