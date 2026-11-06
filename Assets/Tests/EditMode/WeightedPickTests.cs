using System.Collections.Generic;
using NUnit.Framework;

public class WeightedPickTests
{
    [Test]
    public void EmptyList_ReturnsMinusOne()
    {
        Assert.AreEqual(-1, WeightedPick.PickIndex(new int[0], max => 0));
    }

    [Test]
    public void NullArguments_ReturnMinusOne()
    {
        Assert.AreEqual(-1, WeightedPick.PickIndex(null, max => 0));
        Assert.AreEqual(-1, WeightedPick.PickIndex(new[] { 1 }, null));
    }

    [Test]
    public void AllZeroOrNegativeWeights_ReturnsMinusOne()
    {
        Assert.AreEqual(-1, WeightedPick.PickIndex(new[] { 0, 0, -3 }, max => 0));
    }

    [Test]
    public void ZeroAndNegativeWeights_AreNeverPicked()
    {
        int[] weights = { 0, 5, -2, 3 };
        int total = 8;

        for (int roll = 0; roll < total; roll++)
        {
            int picked = WeightedPick.PickIndex(weights, max => roll);
            Assert.That(picked, Is.EqualTo(1).Or.EqualTo(3), $"roll {roll}");
        }
    }

    [Test]
    public void RollMapsToExpectedBuckets()
    {
        int[] weights = { 2, 3, 5 };   // 0-1 → 0, 2-4 → 1, 5-9 → 2
        int[] expected = { 0, 0, 1, 1, 1, 2, 2, 2, 2, 2 };

        for (int roll = 0; roll < expected.Length; roll++)
        {
            Assert.AreEqual(expected[roll], WeightedPick.PickIndex(weights, max => roll), $"roll {roll}");
        }
    }

    [Test]
    public void NextInt_IsCalledWithTotalOfPositiveWeights()
    {
        int received = -1;
        WeightedPick.PickIndex(new[] { 2, 0, 3, -1 }, max => { received = max; return 0; });
        Assert.AreEqual(5, received);
    }

    [Test]
    public void Distribution_FollowsWeights()
    {
        int[] weights = { 1, 3, 6 };
        int[] counts = new int[3];
        var rand = new System.Random(12345);
        const int Trials = 20000;

        for (int i = 0; i < Trials; i++)
        {
            counts[WeightedPick.PickIndex(weights, max => rand.Next(0, max))]++;
        }

        Assert.AreEqual(0.1, counts[0] / (double)Trials, 0.02);
        Assert.AreEqual(0.3, counts[1] / (double)Trials, 0.02);
        Assert.AreEqual(0.6, counts[2] / (double)Trials, 0.02);
    }
}
