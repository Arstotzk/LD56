using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    public AudioClip newMusicClip;
    public string lavelName;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            var player = collision.transform.GetComponent<PlayerInput>();
            if (player == null)
                return;
            //TODO Animation and startCorotine next level
            StartCoroutine(NextLevel());
        }

        IEnumerator NextLevel() 
        {
            if (lavelName == "Level6")
            {
                var music = GameObject.Find("Music").GetComponent<Music>();
                music.audioSource.clip = newMusicClip;
                music.audioSource.Play();
            }
            var level =  SceneManager.LoadSceneAsync(lavelName);
            yield return new WaitForSeconds(2f);
        }
    }
}
