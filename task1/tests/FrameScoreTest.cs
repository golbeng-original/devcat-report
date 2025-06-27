namespace tests;

using Task1;

public class FrameScoreTest
{
    [Fact]
    public void TestAllStrikeScore()
    {
        var frameInfoFactory = new FrameInfoFactory();

        var frameCollection = new FrameInfoCollection();
        for (var i = 0; i < 9; i++)
        {
            var frameInfo = frameInfoFactory.GenerateFrameInfo();
            frameInfo.UpdatePinFall(10);
            frameCollection.Add(frameInfo);
        }

        var lastFrameInfo = frameInfoFactory.GenerateFrameInfo();
        lastFrameInfo.UpdatePinFall(10);
        lastFrameInfo.UpdatePinFall(10);
        lastFrameInfo.UpdatePinFall(10);
        frameCollection.Add(lastFrameInfo);

        var scoreInfoCollection = frameCollection.ToFrameScoreInfoCollection();
        var lastFrameScore = scoreInfoCollection.FrameScores.Last();
        Assert.Equal(300, lastFrameScore.AccScore);
    }
}