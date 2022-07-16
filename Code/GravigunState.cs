using UnityEngine;
using UnityEngine.InputSystem;

namespace Gravigun
{
	public abstract class GravigunState
	{
		protected GravigunController _controller;
		protected GravigunState(GravigunController controller) => _controller = controller;

		public virtual void Enter(){}
		public virtual void Update(){}
		public virtual void FixedUpdate(){}
		public virtual void Exit(){}
	}

	public abstract class GravigunStateIdleBase : GravigunState
	{
		protected GravigunStateIdleBase(GravigunController controller) : base(controller){}

		protected bool TryGetBody(out Gravibody gravibody)
		{
			gravibody = null;

			if (Physics.Raycast(_controller.CameraRay, out RaycastHit hit, _controller.Range))
			{
				if (hit.collider.TryGetComponent(out gravibody))
				{
					return true;
				}
			}

			return false;
		}
	}
	
	public sealed class GravigunStateIdle : GravigunStateIdleBase
	{
		public GravigunStateIdle(GravigunController controller) : base(controller){}

		public override void Enter()
		{
			base.Enter();
			_controller.CurrentGravibody = null;
		}

		public override void Update()
		{
			base.Update();

			if (TryGetBody(out Gravibody body))
			{
				_controller.CurrentGravibody = body;
				_controller.SetState(_controller.Ready);
			}
			else
			{
				if (Mouse.current.rightButton.wasPressedThisFrame ||
				    Mouse.current.leftButton.wasPressedThisFrame)
				{
					_controller.SetState(_controller.DryShot);
				}
			}
		}
	}

	public sealed class GravigunStateReady : GravigunStateIdleBase
	{
		public GravigunStateReady(GravigunController controller) : base(controller){}

		public override void Enter()
		{
			base.Enter();
			
			_controller.EffectController.PlayReadyEffect();
			_controller.SoundController.PlayReadySound();
		}

		public override void Update()
		{
			base.Update();
			
			if (!TryGetBody(out Gravibody body)) _controller.SetState(_controller.Idle);
			else if (body != _controller.CurrentGravibody) _controller.CurrentGravibody = body;
			else
			{
				if(Mouse.current.rightButton.wasPressedThisFrame) _controller.SetState(_controller.Drag);
				else if(Mouse.current.leftButton.wasPressedThisFrame) _controller.SetState(_controller.Throw);
			}
		}

		public override void Exit()
		{
			base.Exit();
			
			_controller.EffectController.StopReadyEffect();
			_controller.SoundController.PlayUnreadySound();
		}
	}

	public sealed class GravigunStateDrag : GravigunState
	{
		public GravigunStateDrag(GravigunController controller) : base(controller){}

		public override void Enter()
		{
			base.Enter();
			
			_controller.CurrentGravibody.Locked = true;
			_controller.EffectController.PlayDragEffect();
			_controller.SoundController.PlayDragSound();
		}

		public override void Update()
		{
			base.Update();
			
			if (Mouse.current.rightButton.wasPressedThisFrame)
			{
				_controller.SoundController.PlayReleaseSound();
				_controller.SetState(_controller.Idle);
			}
			else if (Mouse.current.leftButton.wasPressedThisFrame)
			{
				_controller.SetState(_controller.Throw);
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			_controller.CurrentGravibody.Move(_controller.DragPoint);
		}

		public override void Exit()
		{
			base.Exit();
			
			_controller.CurrentGravibody.Locked = false;
			_controller.EffectController.StopDragEffect();
			_controller.SoundController.StopDragSound();
		}
	}

	public sealed class GravigunStateThrow : GravigunState
	{
		public GravigunStateThrow(GravigunController controller) : base(controller){}

		public override void Enter()
		{
			base.Enter();
			
			_controller.CurrentGravibody.AddForce(_controller.CameraDirection * 200f, ForceMode.Impulse);

			_controller.EffectController.PlayThrowEffect(_controller.CameraPosition + _controller.CameraDirection * 25f);
			_controller.SoundController.PlayThrowSound();

			_controller.SetState(_controller.Idle);
		}
	}
	
	public sealed class GravigunStateDryShot : GravigunState
	{
		public GravigunStateDryShot(GravigunController controller) : base(controller){}

		public override void Enter()
		{
			base.Enter();
			
			_controller.EffectController.PlayDryEffect();
			_controller.SoundController.PlayDrySound();
			
			_controller.SetState(_controller.Idle);
		}
	}
}