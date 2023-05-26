using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairBrick : MonoBehaviour
{

    [SerializeField] private GameObject wall;
    [SerializeField] private int index;
    MeshRenderer mesh;
    Brick brick;
    float stepOffset = 0.33f;

    private void Start()
    {
        mesh = this.gameObject.GetComponent<MeshRenderer>();
        brick = this.gameObject.GetComponent<Brick>();
        index = Mathf.RoundToInt(transform.position.y / stepOffset);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<BotAI>(out var botAI))
        {
            if (!mesh.enabled)
            {
                if (botAI.BrickCount > 0)
                {
                    botAI.RemoveBrick();
                    brick.ChangeColor(botAI.ColorType);
                    mesh.enabled = true;
                    if (index == 12)
                    {
                        botAI.StairTP = botAI.EndTarget.transform.position;
                    }
                    else if (index == 26)
                    {
                        botAI.IsWin = true;
                        botAI.StairTP = botAI.EndTarget.transform.position;
                    }
                    else
                    {
                        botAI.StairTP = new Vector3(transform.position.x, transform.position.y + 0.33f, transform.position.z + 0.72f);
                    }

                }
            }
            else if (botAI.ColorType != brick.ColorType)
            {
                if (botAI.BrickCount > 0)
                {
                    botAI.RemoveBrick();
                    brick.ChangeColor(botAI.ColorType);
                    mesh.enabled = true;
                    if (index == 12)
                    {
                        botAI.StairTP = botAI.EndTarget.transform.position;
                    }
                    else if (index == 26)
                    {
                        botAI.IsWin = true;
                        botAI.StairTP = botAI.EndTarget.transform.position;
                    }
                    else
                    {
                        botAI.StairTP = new Vector3(transform.position.x, transform.position.y + 0.33f, transform.position.z + 0.72f);
                    }
                }
            }
            
        }
        if (other.gameObject.TryGetComponent<Player>(out var player))
        {
            if (!mesh.enabled)
            {
                if (player.BrickCount > 0)
                {
                    if (index == 26)
                    {
                        player.IsWin = true;
                    }
                    player.RemoveBrick();
                    brick.ChangeColor(player.ColorType);
                    mesh.enabled = true;
                    wall.SetActive(false);
                }
                else
                {
                    wall.SetActive(true);
                }
            }
            else if (player.ColorType != brick.ColorType)
            {
                if (player.BrickCount > 0)
                {
                    if (index == 26)
                    {
                        player.IsWin = true;
                    }
                    player.RemoveBrick();
                    brick.ChangeColor(player.ColorType);
                    mesh.enabled = true;
                    wall.SetActive(false);
                }
                else
                {
                    wall.SetActive(true);
                }
            }
            else
            {
                if (index == 26)
                {
                    player.IsWin = true;
                }
                wall.SetActive(false);
            }
        }
    }
}
