using System.Collections;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gravigun
{
	public class Lightning : MonoBehaviour
	{
		[SerializeField] private LineRenderer _lineRenderer;
		[SerializeField] private AnimationCurve _widthCurve;
		[SerializeField] private float _width = 1f;
		[SerializeField] private int _pointsCount;
		[SerializeField] private float _duration;
		[SerializeField] private float _shift;
		[SerializeField] private bool _loop;

		[Header("EDITOR")]
		[SerializeField] private bool _enableGizmos = true;
		[SerializeField] private float _gizmosScale = 0.1f;
		[SerializeField] private Transform _origin;
		[SerializeField] private Transform _target;

		public void Play()
		{
			var originTransform = new LightningPositionTransform(_origin);
			var targetTransform = new LightningPositionTransform(_target);
			Play(originTransform, targetTransform);
		}

		public void Play(Vector3 origin, Vector3 target)
		{
			var originPosition = new LightningPositionVector(origin);
			var targetPosition = new LightningPositionVector(target);
			Play(originPosition, targetPosition);
		}
		
		public void Play(ILightningPosition origin, ILightningPosition target)
		{
			_lineRenderer.enabled = false;
			_lineRenderer.positionCount = _pointsCount;
			_lineRenderer.SetPositions(new Vector3[_pointsCount]);
			_lineRenderer.widthCurve = _widthCurve;
			_lineRenderer.widthMultiplier = _width;

			StartCoroutine(Draw(Time.time + _duration, origin, target));
		}
		
		private IEnumerator Draw(float duration, ILightningPosition origin, ILightningPosition target)
		{
			Vector3 direction = (target.position - origin.position);
			float interval = direction.magnitude / _lineRenderer.positionCount;
		
			_lineRenderer.enabled = true;

			while (Time.time < duration)
			{
				Vector3 originPosition = _lineRenderer.useWorldSpace ? origin.position : transform.InverseTransformPoint(origin.position);
				Vector3 targetPosition = _lineRenderer.useWorldSpace ? target.position : transform.InverseTransformPoint(target.position);
			
				_lineRenderer.SetPosition(0, originPosition);
				_lineRenderer.SetPosition(_lineRenderer.positionCount - 1, targetPosition);
				
				for (int i = 1; i < _lineRenderer.positionCount - 1; i++)
				{
					Vector3 pointDefaultPosition = origin.position + direction.normalized * interval * i;
					
					if (!_lineRenderer.useWorldSpace)
					{
						pointDefaultPosition = transform.InverseTransformPoint(pointDefaultPosition);
					}
					
					float shiftedX = GetShift(pointDefaultPosition.x, _shift);
					float shiftedY = GetShift(pointDefaultPosition.y, _shift);
					
					Vector3 pointShiftedPosition = new Vector3(shiftedX, shiftedY, pointDefaultPosition.z);

				
					
					_lineRenderer.SetPosition(i, pointShiftedPosition);	
				}
				
				yield return null;
			}

			if (_loop) StartCoroutine(Draw(Time.time + _duration, origin, target));
			else _lineRenderer.enabled = false;
		}

		public void Stop()
		{
			_lineRenderer.enabled = false;
			StopAllCoroutines();
		}

		private float GetShift(float value, float length)
		{
			float rand = Random.Range(value - length, value + length);
			return Mathf.Lerp(value, rand, Time.deltaTime * 10);
		}

		private void OnDrawGizmos()
		{
			if (!_enableGizmos) return;
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(_origin.position, _gizmosScale);
			Gizmos.DrawSphere(_target.position, _gizmosScale);
			
			Gizmos.color = Color.yellow;
			Vector3 direction = (_target.position - _origin.position);
			float interval = direction.magnitude / _lineRenderer.positionCount;

			for (int i = 0; i < _pointsCount; i++)
			{
				Vector3 pointDefaultPosition = _origin.position + direction.normalized * interval * i;
				Gizmos.DrawSphere(pointDefaultPosition, _gizmosScale);
			}
		}
	}

	[CanEditMultipleObjects]
	[CustomEditor(typeof(Lightning))]
	public class LightningEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var lightning = (target as Lightning);

			if (GUILayout.Button("Shoot")) lightning.Play();
			if (GUILayout.Button("Stop")) lightning.Stop();
		}
	}

	public interface ILightningPosition
	{
		Vector3 position { get; }
	}

	public struct LightningPositionVector : ILightningPosition
	{
		public Vector3 position { get; }
		public LightningPositionVector(Vector3 pos) => position = pos;
	}

	public struct LightningPositionTransform : ILightningPosition
	{
		public Vector3 position => _transform.position;
		private Transform _transform;
		public LightningPositionTransform(Transform transform) => _transform = transform;
	}
}