using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class PopcornKernelTest {
	TestInputManager testInputManager;
	TestCollisionChecker groundChecker;

	public PopcornKernelTest() {
		testInputManager = new TestInputManager ();
		groundChecker = new TestCollisionChecker ();
	}

	[SetUp]
	public void setUp() {
		testInputManager.SetJumpKeyUp (false);
		testInputManager.SetJumpKeyDown (false);
		groundChecker.SetColliding (false);
	}

	[Test]
	public void PopcornKernelTestConstructorSetsTemperatureAsZero() {
		PopcornKernel popcornKernel = NewTestPopcornKernel ();
		Assert.Zero (popcornKernel.getTemperature ());
	}

	[Test]
	public void PopcornKernelTestIncreaseTemperatureSetsTheTemperatureWithinTheClampBounds() {
		PopcornKernel popcornKernel = NewTestPopcornKernel ();
		popcornKernel.increaseTemperature (10);
		Assert.AreEqual (10, popcornKernel.getTemperature ());
	}

	[Test]
	public void PopcornKernelTestIncreaseTemperatureOverTheMaxSetsTheTemperatureToTheMax() {
		PopcornKernel popcornKernel = NewTestPopcornKernel ();
		popcornKernel.increaseTemperature (110);
		Assert.AreEqual (100, popcornKernel.getTemperature ());
	}

	[Test]
	public void PopcornKernelTestIncreaseTemperatureUnderTheMinSetsTheTemperatureToTheMin() {
		PopcornKernel popcornKernel = NewTestPopcornKernel ();
		popcornKernel.increaseTemperature (-110);
		Assert.AreEqual (0, popcornKernel.getTemperature ());
	}

	[Test]
	public void PopcornKernelTestCheckForJumpDoesNotHaveAnyEffectWhenKernelIsAtMaxTemperature() {
		PopcornKernel popcornKernel = NewTestPopcornKernel ();
		popcornKernel.increaseTemperature (100);
		Vector2 jumpVelocity = popcornKernel.CheckForJump (new Vector2 (0.0f, 0.0f));
		Assert.AreEqual (0.0f, jumpVelocity.y);
		Assert.AreEqual (0.0f, jumpVelocity.x);
	}

	[Test]
	public void PopcornKernelTestCheckForJumpSetsTheVelocityAsMaxJumpVelocityWhenPlayerPressesJumpButton() {
		PopcornKernel popcornKernel = NewTestPopcornKernel ();
		float expectedJumpVelocity = GetMaxJumpVelocity (8.0f, 1.0f);
		groundChecker.SetColliding (true);
		testInputManager.SetJumpKeyDown (true);

		popcornKernel.Update ();
		Vector2 velocity = popcornKernel.CheckForJump (new Vector2 (0.0f, 0.0f));

		Assert.AreEqual (expectedJumpVelocity, velocity.y);
	}

	[Test]
	public void PopcornKernelTestCheckForJumpSetsTheVelocityAsMinJumpVelocityWhenPlayerReleasesJumpButton() {
		PopcornKernel popcornKernel = NewTestPopcornKernel ();
		float expectedJumpVelocity = GetMinJumpVelocity (8.0f, 0.4f, 1.0f);

		testInputManager.SetJumpKeyDown (false);
		testInputManager.SetJumpKeyUp (true);

		popcornKernel.Update ();
		Vector2 velocity = popcornKernel.CheckForJump (new Vector2 (0.0f, GetMaxJumpVelocity(8.0f, 1.0f)));

		Assert.AreEqual (expectedJumpVelocity, velocity.y);
	}

	[Test]
	public void PopcornKernelTestCheckForJumpSetsTheJumpVelocityAsCurrentYVelocityWhenItIsLessThanMinJumpVelocity() {
		PopcornKernel popcornKernel = NewTestPopcornKernel ();

		testInputManager.SetJumpKeyDown (false);
		testInputManager.SetJumpKeyUp (true);

		popcornKernel.Update ();
		Vector2 velocity = popcornKernel.CheckForJump (new Vector2 (0.0f, 0.001f));

		Assert.AreEqual (0.001f, velocity.y);
	}

	[Test]
	public void PopcornKernelTestCheckForJumpSetsGlidingAsTrueWhenJumpKeyIsPressedInTheAir() {
		PopcornKernel popcornKernel = NewTestPopcornKernel ();

		groundChecker.SetColliding (false);
		testInputManager.SetJumpKeyDown (true);
		popcornKernel.Update ();
		popcornKernel.CheckForJump (new Vector2 (0.0f, 0.0f));

		Assert.IsTrue (popcornKernel.isGliding());
	}

	[Test]
	public void PopcornKernelTestCheckForJumpSetsGlidingAsFalseWhenJumpKeyIsReleased() {
		PopcornKernel popcornKernel = NewTestPopcornKernel ();

		groundChecker.SetColliding (false);
		testInputManager.SetJumpKeyDown (true);
		popcornKernel.Update ();
		popcornKernel.CheckForJump (new Vector2 (0.0f, 0.0f));

		Assert.IsTrue (popcornKernel.isGliding());

		testInputManager.SetJumpKeyDown (false);
		testInputManager.SetJumpKeyUp (true);
		popcornKernel.CheckForJump (new Vector2 (0.0f, 0.0f));

		Assert.IsFalse (popcornKernel.isGliding());
	}

	float GetGravity(float maxJumpHeight, float timeToJumpApex) {
		return -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
	}

	float GetMaxJumpVelocity(float maxJumpHeight, float timeToJumpApex) {
		return Mathf.Abs(GetGravity(maxJumpHeight, timeToJumpApex) * timeToJumpApex);
	}

	float GetMinJumpVelocity(float maxJumpHeight, float minJumpHeight, float timeToJumpApex) {
		return Mathf.Sqrt (2 * Mathf.Abs(GetGravity(maxJumpHeight, timeToJumpApex) * minJumpHeight));
	}

	PopcornKernel NewTestPopcornKernel() {
		return new PopcornKernel (testInputManager, groundChecker, 0.4f, 8.0f, 1.0f);
	}

	class TestCollisionChecker: CollisionChecker {
		private bool colliding = false;

		public void SetColliding(bool colliding) {
			this.colliding = colliding;
		}

		public bool isColliding() {
			return colliding;
		}
	}

	class TestInputManager: InputManager {
		private bool keyDown = false;
		private bool keyUp = false;

		public void SetJumpKeyDown(bool down) {
			this.keyDown = down;
		}

		public void SetJumpKeyUp(bool up) {
			this.keyUp = up;
		}

		public bool JumpKeyDown () {
			return this.keyDown;
		}

		public bool JumpKeyUp () {
			return this.keyUp;
		}
	}
}
