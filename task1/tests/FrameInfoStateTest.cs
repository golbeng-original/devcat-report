namespace tests;

using Task1;

public class FrameInfoStateTest
{
    [Fact]
    public void TestAllStrike()
    {
        var frameInfoFactory = new FrameInfoFactory();

        var frameCollection = new FrameInfoCollection();
        for (var i = 0; i < 9; i++)
        {
            var frameInfo = frameInfoFactory.GenerateFrameInfo();
            frameInfo.UpdatePinFall(10);
            frameCollection.Add(frameInfo);
        }

        foreach (var frame in frameCollection.Frames)
        {
            Assert.Equal(frame.FrameStateInfo.FrameState, FrameState.Strike);
        }

        var lastFrameInfo = frameInfoFactory.GenerateFrameInfo();
        lastFrameInfo.UpdatePinFall(10);
        lastFrameInfo.UpdatePinFall(10);
        lastFrameInfo.UpdatePinFall(10);
        frameCollection.Add(lastFrameInfo);

        var lastFrame = frameCollection.FindLatestFrame();
        Assert.Equal(lastFrame.FrameStateInfo.FrameState, FrameState.CompleteLastFrame);
    }

    [Fact]
    public void TestFrameState1()
    {
        var frameInfo = new FrameInfo(10, new BonusFrameStateRule(), true);
        frameInfo.UpdatePinFall(10);
        frameInfo.UpdatePinFall(10);
        frameInfo.UpdatePinFall(10);

        var frameStateInfo = frameInfo.FrameStateInfo;
        Assert.Equal(FrameState.CompleteLastFrame, frameStateInfo.FrameState);
    }

    [Fact]
    public void TestFrameState2()
    {
        var frameInfo = new FrameInfo(10, new BonusFrameStateRule(), true);
        frameInfo.UpdatePinFall(1);
        frameInfo.UpdatePinFall(1);

        var frameStateInfo = frameInfo.FrameStateInfo;
        Assert.Equal(FrameState.CompleteLastFrame, frameStateInfo.FrameState);
    }

    [Fact]
    public void TestFrameState3()
    {
        var frameInfo = new FrameInfo(1, new NormalFrameStateRule(), false);
        frameInfo.UpdatePinFall(10);

        var frameStateInfo = frameInfo.FrameStateInfo;
        Assert.Equal(FrameState.Strike, frameStateInfo.FrameState);
    }

    [Fact]
    public void TestFrameState4()
    {
        var frameInfo = new FrameInfo(1, new NormalFrameStateRule(), false);
        frameInfo.UpdatePinFall(1);
        frameInfo.UpdatePinFall(1);

        var frameStateInfo = frameInfo.FrameStateInfo;
        Assert.Equal(FrameState.OpenFrame, frameStateInfo.FrameState);
    }

    [Fact]
    public void TestFrameState5()
    {
        var frameInfo = new FrameInfo(1, new NormalFrameStateRule(), false);
        frameInfo.UpdatePinFall(1);
        frameInfo.UpdatePinFall(9);

        var frameStateInfo = frameInfo.FrameStateInfo;
        Assert.Equal(FrameState.Spare, frameStateInfo.FrameState);
    }

    [Fact]
    public void TestFrameState6()
    {
        var frameInfo = new FrameInfo(1, new NormalFrameStateRule(), false);
        frameInfo.UpdatePinFall(9);
        frameInfo.UpdatePinFall(1);

        var frameStateInfo = frameInfo.FrameStateInfo;
        Assert.Equal(FrameState.Spare, frameStateInfo.FrameState);
    }
}