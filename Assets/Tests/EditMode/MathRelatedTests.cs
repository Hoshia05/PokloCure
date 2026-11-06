using NUnit.Framework;

public class MathRelatedTests
{
    [Test]
    public void ExpRequirement_StrictlyIncreasesFromLevel1To100()
    {
        float previous = MathRelated.GetNextExpRequirement(1);

        for (int level = 2; level <= 100; level++)
        {
            float current = MathRelated.GetNextExpRequirement(level);
            Assert.Greater(current, previous, $"레벨 {level}의 요구 경험치가 이전 레벨보다 커야 한다");
            previous = current;
        }
    }

    [Test]
    public void ExpRequirement_IsPositiveAndFinite()
    {
        for (int level = 1; level <= 100; level++)   // 레벨은 1부터 시작 (level 0 은 공식상 0)
        {
            float value = MathRelated.GetNextExpRequirement(level);
            Assert.Greater(value, 0f, $"level {level}");
            Assert.IsFalse(float.IsNaN(value) || float.IsInfinity(value), $"level {level}");
        }
    }

    [Test]
    public void ExpRequirement_Level1_IsAboutSixteen()
    {
        // 1 → 2 레벨업 요구치 (이전 버그: 초기값이 79로 고정되어 있었음)
        Assert.AreEqual(16.215f, MathRelated.GetNextExpRequirement(1), 0.01f);
    }
}
