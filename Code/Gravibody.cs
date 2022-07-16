using UnityEngine;

namespace Gravigun
{
	[RequireComponent(typeof(Rigidbody))]
	public class Gravibody : MonoBehaviour
	{
		private Rigidbody _rigidbody;

		private void Awake() => _rigidbody = GetComponent<Rigidbody>();
	
		public Vector3 Position => transform.position;
		public Vector3 Velocity => _rigidbody.velocity;
		public void AddForce(Vector3 force, ForceMode forceMode) => _rigidbody.AddForce(force * _rigidbody.mass, forceMode);
		
		private bool _locked;

		public bool Locked
		{
			get => _locked; 
			set => _rigidbody.useGravity = !(_locked = value);
		}
		
		public void Move(Vector3 targetPosition)
		{
			Vector3 direction = targetPosition - _rigidbody.position;
			_rigidbody.velocity = direction.normalized * direction.sqrMagnitude * 10f;
			
			if (_rigidbody.velocity.magnitude > 100f)
			{
				_rigidbody.velocity = _rigidbody.velocity.normalized * 100f;
			}
		}
	}
}