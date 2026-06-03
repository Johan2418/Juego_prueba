using NUnit.Framework;
using UnityEngine;
using MantaMinigames.Fishing;

namespace MantaMinigames.Fishing.Tests
{
    public sealed class FishingMinigameControllerTests
    {
        private GameObject gameObject;
        private FishingMinigameController controller;

        [SetUp]
        public void SetUp()
        {
            gameObject = new GameObject("FishingMinigameControllerTests");
            controller = gameObject.AddComponent<FishingMinigameController>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void IsProgressInSuccessWindow_ReturnsTrueInsideDefaultWindow()
        {
            Assert.IsTrue(controller.IsProgressInSuccessWindow(0.5f));
        }

        [Test]
        public void IsProgressInSuccessWindow_ReturnsFalseOutsideDefaultWindow()
        {
            Assert.IsFalse(controller.IsProgressInSuccessWindow(0.2f));
        }

        [Test]
        public void CreateResult_ReturnsSuccessWithConfiguredReward()
        {
            FishingRewardData reward = ScriptableObject.CreateInstance<FishingRewardData>();
            controller.SetRewardData(reward);

            FishingResult result = controller.CreateResult(FishingResultStatus.Success);

            Assert.AreEqual(FishingResultStatus.Success, result.Status);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreSame(reward, result.Reward);

            Object.DestroyImmediate(reward);
        }

        [Test]
        public void CreateResult_ReturnsFailedWithoutReward()
        {
            FishingResult result = controller.CreateResult(FishingResultStatus.Failed);

            Assert.AreEqual(FishingResultStatus.Failed, result.Status);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(result.Reward);
        }
    }
}
