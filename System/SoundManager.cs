using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public Dictionary<string, Queue<GameObject>> soundPool;
    public List<GameObject> soundList;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        soundPool = new Dictionary<string, Queue<GameObject>>();
        soundList = new List<GameObject>();
    }

    public void Play(string path, bool loop = false, float volume = 1f)
    {
        GameObject soundObj;
        AudioSource sound;
        Queue<GameObject> queue;
        if (!soundPool.TryGetValue(path, out queue))
        {
            soundObj = ObjectPoolManager.instance.Instantiate("Prefab/SoundPrefab");
            sound = soundObj.GetComponent<AudioSource>();
            AudioClip audioClip = Resources.Load<AudioClip>(path);
            sound.clip = audioClip;
            queue = new Queue<GameObject>();
            queue.Enqueue(soundObj);
            soundPool.Add(path, queue);
            soundList.Add(soundObj);
        }
        else
        {
            if (queue.Peek().activeSelf)
            {
                soundObj = ObjectPoolManager.instance.Instantiate("Prefab/SoundPrefab");
                sound = soundObj.GetComponent<AudioSource>();
                AudioClip audioClip = Resources.Load<AudioClip>(path);
                sound.clip = audioClip;
                queue.Enqueue(soundObj);
            }
            else
            {
                soundObj = queue.Dequeue();
                sound = soundObj.GetComponent<AudioSource>();
                soundObj.SetActive(true);
                queue.Enqueue(soundObj);
            }
        }
        soundObj.transform.parent = transform;
        sound.clip.name = path;
        sound.loop = loop;
        sound.volume = volume;
        sound.Play();
    }

    public void Stop(string path)
    {
        foreach (GameObject obj in soundList)
        {
            if (obj.GetComponent<AudioSource>().clip.name == path)
            {
                GameObject target = null;
                foreach (GameObject temp in soundPool[path])
                {
                    if (temp.activeSelf)
                    {
                        target = temp;
                        break;
                    }
                }
                target.SetActive(false);
            }
        }
    }
}
