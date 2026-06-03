using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace MantaMinigames.Fishing.Tests
{
    public sealed class FishingMinigameControllerGameplayTests
    {
        private GameObject gameObject;
        private FishingMinigameController controller;

        [SetUp]
        public void SetUp()
        {
            gameObject = new GameObject("FishingMinigameControllerGameplayTests");
            gameObject.AddComponent<FishingInputHandler>();
            controller = gameObject.AddComponent<FishingMinigameController>();
            controller.ConfigureForTest(3, 0.4f, 0.6f, 0.5f);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void StartMinigame_ResetsAttemptsAndFishState()
        {
            controller.StartMinigame();

            Assert.AreEqual(3, controller.RemainingAttempts);
            Assert.IsFalse(controller.HasFish);
            Assert.IsTrue(controller.IsRunning);
        }

        [Test]
        public void TryCatchInSuccessZoneFinishesWithFish()
        {
            controller.StartMinigame();

            FishingResult result = controller.TryCatchAtCurrentPosition();

            Assert.AreEqual(FishingResultStatus.Success, result.Status);
            Assert.AreEqual("Conseguiste un pescado", result.Message);
            Assert.IsTrue(controller.HasFish);
            Assert.IsFalse(controller.IsRunning);
        }

        [Test]
        public void TryCatchOutsideSuccessZoneConsumesAttempt()
        {
            controller.ConfigureForTest(3, 0.4f, 0.6f, 0.2f);
            controller.StartMinigame();

            FishingResult result = controller.TryCatchAtCurrentPosition();

            Assert.AreEqual(FishingResultStatus.InProgress, result.Status);
            Assert.AreEqual("Intenta otra vez", result.Message);
            Assert.AreEqual(2, controller.RemainingAttempts);
            Assert.IsFalse(controller.HasFish);
            Assert.IsTrue(controller.IsRunning);
        }

        [Test]
        public void ThirdMissFinishesWithFailedResult()
        {
            controller.ConfigureForTest(3, 0.4f, 0.6f, 0.2f);
            controller.StartMinigame();

            controller.TryCatchAtCurrentPosition();
            controller.TryCatchAtCurrentPosition();
            FishingResult result = controller.TryCatchAtCurrentPosition();

            Assert.AreEqual(FishingResultStatus.Failed, result.Status);
            Assert.AreEqual("Intenta otra vez", result.Message);
            Assert.AreEqual(0, controller.RemainingAttempts);
            Assert.IsFalse(controller.HasFish);
            Assert.IsFalse(controller.IsRunning);
        }

        [Test]
        public void ConfigureUiSetsSuccessZoneWidthFromWindow()
        {
            GameObject bar = new GameObject("Bar", typeof(RectTransform), typeof(Image));
            GameObject success = new GameObject("SuccessZone", typeof(RectTransform), typeof(Image));
            GameObject indicator = new GameObject("Indicator", typeof(RectTransform), typeof(Image));

            success.transform.SetParent(bar.transform, false);
            indicator.transform.SetParent(bar.transform, false);

            ((RectTransform)bar.transform).sizeDelta = new Vector2(200f, 20f);
            controller.SetUiReferencesForTest(
                bar.GetComponent<Image>(),
                success.GetComponent<Image>(),
                indicator.GetComponent<Image>());

            controller.RefreshVisuals();

            Assert.AreEqual(40f, ((RectTransform)success.transform).sizeDelta.x);

            Object.DestroyImmediate(bar);
        }
    }
}
