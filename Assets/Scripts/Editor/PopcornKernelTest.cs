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
		testInputManager.SetAttackKeyPressed (false);
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

		popcornKernel.FixedUpdate (Vector2.zero);
		Vector2 velocity = popcornKernel.CheckForJump (new Vector2 (0.0f, 0.0f));

		Assert.AreEqual (expectedJumpVelocity, velocity.y);
	}

	[Test]
	public void PopcornKernelTestCheckForJumpSetsTheVelocityAsMinJumpVelocityWhenPlayerReleasesJumpButton() {
		PopcornKernel popcornKernel = NewTestPopcornKernel ();
		float expectedJumpVelocity = GetMinJumpVelocity (8.0f, 0.4f, 1.0f);

		testInputManager.SetJumpKeyDown (false);
		testInputManager.SetJumpKeyUp (true);

		popcornKernel.FixedUpdate (Vector2.zero);
		Vector2 velocity = popcornKernel.CheckForJump (new Vector2 (0.0f, GetMaxJumpVelocity(8.0f, 1.0f)));

		Assert.AreEqual (expectedJumpVelocity, velocity.y);
	}

	[Test]
	public void PopcornKernelTestCheckForJumpSetsTheJumpVelocityAsCurrentYVelocityWhenItIsLessThanMinJumpVelocity() {
		PopcornKernel popcornKernel = NewTestPopcornKernel ();

		testInputManager.SetJumpKeyDown (false);
		testInputManager.SetJumpKeyUp (true);

		popcornKernel.FixedUpdate (Vector2.zero);
		Vector2 velocity = popcornKernel.CheckForJump (new Vector2 (0.0f, 0.001f));

		Assert.AreEqual (0.001f, velocity.y);
	}

	[Test]
	public void PopcornKernelTestCheckForJumpSetsGlidingAsTrueWhenJumpKeyIsPressedInTheAir() {
		PopcornKernel popcornKernel = NewTestPopcornKernel ();

		groundChecker.SetColliding (false);
		testInputManager.SetJumpKeyDown (true);
		popcornKernel.FixedUpdate (Vector2.zero);
		popcornKernel.CheckForJump (new Vector2 (0.0f, 0.0f));

		Assert.IsTrue (popcornKernel.isGliding());
	}

	[Test]
	public void PopcornKernelTestCheckForJumpSetsGlidingAsFalseWhenJumpKeyIsReleased() {
		PopcornKernel popcornKernel = NewTestPopcornKernel ();

		groundChecker.SetColliding (false);
		testInputManager.SetJumpKeyDown (true);
		popcornKernel.FixedUpdate (Vector2.zero);
		popcornKernel.CheckForJump (new Vector2 (0.0f, 0.0f));

		Assert.IsTrue (popcornKernel.isGliding());

		testInputManager.SetJumpKeyDown (false);
		testInputManager.SetJumpKeyUp (true);
		popcornKernel.CheckForJump (new Vector2 (0.0f, 0.0f));

		Assert.IsFalse (popcornKernel.isGliding());
	}

//	[Test]
//	public void PopcornKernelTestIsKickTriggeredReturnsTrueWhenAttackButtonIsPressedAndKernelIsOnTheGround() {
//		PopcornKernel popcornKernel = NewTestPopcornKernel ();
//		groundChecker.SetColliding (true);
//		testInputManager.SetAttackKeyPressed (true);
//		popcornKernel.FixedUpdate (Vector2.zero);
//		Assert.IsTrue(popcornKernel.IsKickTriggered ());
//	}
//
//	[Test]
//	public void PopcornKernelTestIsKickTriggeredReturnsFalseWhenAttackButtonIsPressedWhenTheKernelIsAlreadyKicking() {
//		PopcornKernel popcornKernel = NewTestPopcornKernel ();
//		groundChecker.SetColliding (true);
//		testInputManager.SetAttackKeyPressed (true);
//		popcornKernel.FixedUpdate (Vector2.zero);
//		Assert.IsTrue(popcornKernel.IsKickTriggered ());
//		Assert.IsFalse(popcornKernel.IsKickTriggered ());
//	}
//
//	[Test]
//	public void PopcornKernelTestIsKickTriggeredReturnsTrueWhenAttackButtonIsPressedWhenTheKernelHadFinishedKicking() {
//		PopcornKernel popcornKernel = NewTestPopcornKernel ();
//		groundChecker.SetColliding (true);
//		testInputManager.SetAttackKeyPressed (true);
//		popcornKernel.FixedUpdate (Vector2.zero);
//		Assert.IsTrue(popcornKernel.IsKickTriggered ());
//		popcornKernel.StopKicking ();
//		Assert.IsTrue(popcornKernel.IsKickTriggered ());
//	}
//
//	[Test]
//	public void PopcornKernelTestIsKickTriggeredReturnsFalseWhenAttackButtonIsPressedAndKernelIsNotOnTheGround() {
//		PopcornKernel popcornKernel = NewTestPopcornKernel ();
//		groundChecker.SetColliding (false);
//		testInputManager.SetAttackKeyPressed (true);
//		popcornKernel.FixedUpdate (Vector2.zero);
//		Assert.IsFalse(popcornKernel.IsKickTriggered ());
//	}

	[Test]
	public void PopcornKernelTestDieSetsTheTemperatureTo100() {
		PopcornKernel popcornKernel = NewTestPopcornKernel ();
		popcornKernel.Die ();
		Assert.AreEqual(100, popcornKernel.getTemperature ());
	}

	[Test]
	public void PopcornKernelTestResetTemperatureChangesTheTemperatureToZero() {
		PopcornKernel popcornKernel = NewTestPopcornKernel ();
		popcornKernel.increaseTemperature (100);
		Assert.AreEqual(100, popcornKernel.getTemperature ());
		popcornKernel.ResetTemperature ();
		Assert.AreEqual(0, popcornKernel.getTemperature ());
	}

	[Test]
	public void PopcornKernelTestIncreaseTemperatureChangesTemperatureWhenItIsInAValidRange() {
		PopcornKernel popcornKernel = NewTestPopcornKernel ();
		popcornKernel.increaseTemperature (50);
		Assert.AreEqual(50, popcornKernel.getTemperature ());
	}

	[Test]
	public void PopcornKernelTestIncreaseTemperatureOverMaxSetsTemperatureToMax() {
		PopcornKernel popcornKernel = NewTestPopcornKernel ();
		popcornKernel.increaseTemperature (500);
		Assert.AreEqual(100, popcornKernel.getTemperature ());
	}

	[Test]
	public void PopcornKernelTestIncreaseTemperatureUnderMinSetsTemperatureToMin() {
		PopcornKernel popcornKernel = NewTestPopcornKernel ();
		popcornKernel.increaseTemperature (-500);
		Assert.AreEqual(0, popcornKernel.getTemperature ());
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
		return new PopcornKernel (testInputManager, groundChecker, null, 0.4f, 8.0f, 1.0f);
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
		private bool attackKeyPressed = false;
		private float xAxisInput = 0.0f;

		public void SetXAxisInput(float xAxisInput) {
			this.xAxisInput = xAxisInput;
		}

		public void SetAttackKeyPressed(bool attackKeyPressed) {
			this.attackKeyPressed = attackKeyPressed;
		}

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

		public bool AttackKeyPressed() {
			return attackKeyPressed;
		}

		public float getXAxis() {
			return xAxisInput;
		}
	}
}
