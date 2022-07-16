using System;
using UnityEngine;

namespace Gravigun
{
	public class GravigunEffectController : MonoBehaviour
	{
		[SerializeField] private Transform _muzzle;
		[SerializeField] private Animator _gunAnimator;

		[SerializeField] private Lightning _lightningThrow;
		[SerializeField] private Lightning[] _lightningsDrag;
		
		[SerializeField] private Animator[] _tentacleAnimators;
		
		private static readonly int Drag = Animator.StringToHash("Drag");
		private static readonly int Throw = Animator.StringToHash("Throw");
		private static readonly int Ready = Animator.StringToHash("Ready");
		private static readonly int Dry = Animator.StringToHash("Dry");
		private static readonly int PlayerVelocity = Animator.StringToHash("PlayerVelocity");

		private CharacterController _playerController;

		public void Init(CharacterController playerController) => _playerController = playerController;
		
		public void PlayDragEffect()
		{
			foreach (var animator in _tentacleAnimators) animator.SetBool(Drag, true);
			foreach (var lightning in _lightningsDrag) lightning.Play();
		}		
		
		public void StopDragEffect()
		{
			foreach (var animator in _tentacleAnimators) animator.SetBool(Drag, false);
			foreach (var lightning in _lightningsDrag) lightning.Stop();
		}

		public void PlayThrowEffect(Vector3 targetPosition)
		{
			foreach (var animator in _tentacleAnimators) animator.SetTrigger(Throw);
			_gunAnimator.SetTrigger(Throw);
			_lightningThrow.Play(_muzzle.position, targetPosition);
		}

		public void PlayReadyEffect()
		{
			foreach (var animator in _tentacleAnimators) animator.SetBool(Ready, true);
		}

		public void StopReadyEffect()
		{
			foreach (var animator in _tentacleAnimators) animator.SetBool(Ready, false);
		}

		public void PlayDryEffect()
		{
			_gunAnimator.SetTrigger(Dry);
		}
		
		private void Update()
		{
			Vector2 playerHorizontalVelocity = new Vector2(_playerController.velocity.x, _playerController.velocity.z);
			_gunAnimator.SetFloat(PlayerVelocity, playerHorizontalVelocity.magnitude);
		}
	}
}