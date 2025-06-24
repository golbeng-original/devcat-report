namespace tests;

using Task1;

public class FrameStateRuleTest
{
    [Fact]
    public void BonusFrameStateRule1()
    {
        var stateRule = new BonusFrameStateRule();
        var isVerified = stateRule.IsVerifyPinFallCount([], 11);
        Assert.False(isVerified);
    }

    [Fact]
    public void BonusFrameStateRule2()
    {
        var stateRule = new BonusFrameStateRule();
        var isVerified = stateRule.IsVerifyPinFallCount([1], 10);
        Assert.True(isVerified);
    }

    [Fact]
    public void BonusFrameStateRule3()
    {
        var stateRule = new BonusFrameStateRule();
        var isVerified = stateRule.IsVerifyPinFallCount([10], 10);
        Assert.True(isVerified);
    }

    [Fact]
    public void BonusFrameStateRule4()
    {
        var stateRule = new BonusFrameStateRule();
        var isVerified = stateRule.IsVerifyPinFallCount([9], 1);
        Assert.True(isVerified);
    }

    [Fact]
    public void BonusFrameStateRule5()
    {
        var stateRule = new BonusFrameStateRule();
        var isVerified = stateRule.IsVerifyPinFallCount([10, 10], 10);
        Assert.True(isVerified);
    }

    [Fact]
    public void BonusFrameStateRule6()
    {
        var stateRule = new BonusFrameStateRule();
        var isVerified = stateRule.IsVerifyPinFallCount([9, 1], 6);
        Assert.True(isVerified);
    }

    [Fact]
    public void BonusFrameStateRule7()
    {
        var stateRule = new BonusFrameStateRule();
        var isVerified = stateRule.IsVerifyPinFallCount([1, 1], 1);
        Assert.False(isVerified);
    }

    [Fact]
    public void NomalFrameStateRule1()
    {
        var stateRule = new NormalFrameStateRule();
        var isVerified = stateRule.IsVerifyPinFallCount([], 11);
        Assert.False(isVerified);
    }

    [Fact]
    public void NomalFrameStateRule2()
    {
        var stateRule = new NormalFrameStateRule();
        var isVerified = stateRule.IsVerifyPinFallCount([1], 10);
        Assert.False(isVerified);
    }

    [Fact]
    public void NomalFrameStateRule3()
    {
        var stateRule = new NormalFrameStateRule();
        var isVerified = stateRule.IsVerifyPinFallCount([10], 10);
        Assert.False(isVerified);
    }

    [Fact]
    public void NomalFrameStateRule4()
    {
        var stateRule = new NormalFrameStateRule();
        var isVerified = stateRule.IsVerifyPinFallCount([9], 1);
        Assert.True(isVerified);
    }
}