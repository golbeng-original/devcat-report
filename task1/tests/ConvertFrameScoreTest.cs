namespace tests;

using Task1;

public class ConvertFrameScoreTest
{
    [Fact]
    public void TestConvertFrameScore1()
    {
        var frameInfoFactory = new FrameInfoFactory();

        var frameCollection = new FrameInfoCollection();

        var frameInfo_1 = frameInfoFactory.GenerateFrameInfo();
        frameInfo_1.UpdatePinFall(2);
        frameInfo_1.UpdatePinFall(3);
        frameCollection.Add(frameInfo_1);

        var frameInfo_2 = frameInfoFactory.GenerateFrameInfo();
        frameInfo_2.UpdatePinFall(10);
        frameCollection.Add(frameInfo_2);

        var frameScoreCollection = frameCollection.ToFrameScoreInfoCollection();

        Assert.Equal(1, frameScoreCollection.FrameScores.Count());
    }

    [Fact]
    public void TestConvertFrameScore2()
    {
        var frameInfoFactory = new FrameInfoFactory();

        var frameCollection = new FrameInfoCollection();

        var frameInfo_1 = frameInfoFactory.GenerateFrameInfo();
        frameInfo_1.UpdatePinFall(10);
        frameCollection.Add(frameInfo_1);

        var frameInfo_2 = frameInfoFactory.GenerateFrameInfo();
        frameInfo_2.UpdatePinFall(10);
        frameCollection.Add(frameInfo_2);

        var frameScoreCollection = frameCollection.ToFrameScoreInfoCollection();

        Assert.Equal(0, frameScoreCollection.FrameScores.Count());
    }

    [Fact]
    public void TestConvertFrameScore3()
    {
        var frameInfoFactory = new FrameInfoFactory();

        var frameCollection = new FrameInfoCollection();

        var frameInfo_1 = frameInfoFactory.GenerateFrameInfo();
        frameInfo_1.UpdatePinFall(10);
        frameCollection.Add(frameInfo_1);

        var frameInfo_2 = frameInfoFactory.GenerateFrameInfo();
        frameInfo_2.UpdatePinFall(2);
        frameInfo_2.UpdatePinFall(4);
        frameCollection.Add(frameInfo_2);

        var frameInfo_3 = frameInfoFactory.GenerateFrameInfo();
        frameInfo_3.UpdatePinFall(2);
        frameInfo_3.UpdatePinFall(4);
        frameCollection.Add(frameInfo_3);

        var frameScoreCollection = frameCollection.ToFrameScoreInfoCollection();

        Assert.Equal(3, frameScoreCollection.FrameScores.Count());
    }
}