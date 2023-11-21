using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    [SerializeField] private SpriteData[] spriteDatas;

    public void PlaySprite(int id)
    { 
        if (id >= spriteDatas.Length) return;
        PlaySprite(spriteDatas[id]);
    }
    
    public void PlaySprite(string type)
    {
        if (!spriteDatas.Any(data => data.spriteName == type)) return;
        PlaySprite(spriteDatas.First(data => data.spriteName == type));
    }

    private void PlaySprite(SpriteData spriteData)
    {
        if (spriteData.name.Length < 1) return;
        StartCoroutine(SpawnAfterTime(spriteData));
    }

    private IEnumerator SpawnAfterTime(SpriteData sprite)
    {
        var spawnPosition = transform.position + sprite.offset;
        yield return new WaitForSeconds(sprite.cooldownDuration);
        
        GameObject obj = Instantiate(sprite.prefab, spawnPosition , Quaternion.identity);

        if (sprite.flipHorizontal)
        {
            obj.transform.Rotate(new Vector3(0, 180, 0));
        }

        sprite.animator = obj.GetComponent<Animator>();
        sprite.animator.SetTrigger(sprite.name);
        float time = UpdateAnimClipTimes(sprite);
        Destroy(obj, time);
    }

    private float UpdateAnimClipTimes(SpriteData sprite)
    {
        AnimationClip[] clips = sprite.animator.runtimeAnimatorController.animationClips;
        float clipTime = 0;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Equals(sprite.name))
            {
                clipTime += clip.length;
                break;
            }
        }
        return clipTime;
    }
}
 
