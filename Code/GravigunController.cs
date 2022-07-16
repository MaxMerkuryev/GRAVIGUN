using UnityEngine;

namespace Gravigun
{
	public class GravigunController : MonoBehaviour
	{
		[SerializeField] private float _range = 10f;
		[SerializeField] private CharacterController _playerController;
		[SerializeField] private GravigunEffectController _effectController;
		[SerializeField] private GravigunSoundController _soundController;

		private Transform _cameraTransform;
		
		public float Range => _range;
		public Ray CameraRay => new(_cameraTransform.position, _cameraTransform.forward);
		

		public Vector3 DragPoint => _cameraTransform.position + _cameraTransform.forward * 2.5f;

		public Vector3 CameraPosition => _cameraTransform.position;
		public Vector3 CameraDirection => _cameraTransform.forward;
		
		public GravigunEffectController EffectController => _effectController;
		public GravigunSoundController SoundController => _soundController;

		public Gravibody CurrentGravibody { get; set; }

		public GravigunState Idle { get; private set; }
		public GravigunState Ready { get; private set; }
		public GravigunState Drag { get; private set; }
		public GravigunState Throw { get; private set; }
		public GravigunState DryShot { get; private set; }
		
		private void Awake()
		{
			_cameraTransform = Camera.main.transform;
			_effectController.Init(_playerController);
			
			Idle = new GravigunStateIdle(this);
			Drag = new GravigunStateDrag(this);
			Throw = new GravigunStateThrow(this);
			Ready = new GravigunStateReady(this);
			DryShot = new GravigunStateDryShot(this);

			SetState(Idle);
		}
		
		private GravigunState _currentState;

		public void SetState(GravigunState state)
		{
			_currentState?.Exit();
			_currentState = state;
			_currentState.Enter();
		}

		private void Update() => _currentState?.Update();
		private void FixedUpdate() => _currentState?.FixedUpdate();
	}
}
