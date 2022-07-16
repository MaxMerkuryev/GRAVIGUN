using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Gravigun
{
	public class GravigunSoundController : MonoBehaviour
	{
		[SerializeField] private AudioClip _ready;
		[SerializeField] private AudioClip _unready;
		[SerializeField] private AudioClip _dry;
		[SerializeField] private AudioClip _dragEnter;
		[SerializeField] private AudioClip _dragLoop;
		[SerializeField] private AudioClip _throw;
		[SerializeField] private AudioClip _release;

		public void PlayReadySound() => Play(_ready);
		public void PlayUnreadySound() => Play(_unready);
		public void PlayDrySound() => Play(_dry);
		public void PlayThrowSound() => Play(_throw);
		public void PlayReleaseSound() => Play(_release);
		
		public void PlayDragSound()
		{
			Play(_dragEnter);
			PlayLoop(_dragLoop);
		}

		public void StopDragSound() => Stop(_dragLoop);


		private List<AudioSource> _sources = new ();
		
		private void Play(AudioClip clip)
		{
			AudioSource source = transform.AddComponent<AudioSource>();
			source.PlayOneShot(clip);
			_sources.Add(source);
			StartCoroutine(WaitForPlaying(source));
		}

		private void PlayLoop(AudioClip clip)
		{			
			AudioSource source = transform.AddComponent<AudioSource>();
			_sources.Add(source);
			source.clip = clip;
			source.loop = true;
			source.Play();
		}
		
		private IEnumerator WaitForPlaying(AudioSource source)
		{
			yield return new WaitUntil(() => !source.isPlaying);
			Stop(source);
		}

		private void Stop(AudioSource source)
		{
			if (source == null) return;
			_sources.Remove(source);
			source.Stop();
			Destroy(source);
		}
		
		private void Stop(AudioClip clip)
		{
			foreach (var source in _sources)
			{
				if (source.clip == clip)
				{
					Stop(source);
					break;
				}
			} 
		}
	}
}