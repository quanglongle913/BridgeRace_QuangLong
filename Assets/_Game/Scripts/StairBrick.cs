using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairBrick : MonoBehaviour
{
    float stepOffet = 0.5f;
    [SerializeField] private GameObject wall;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            Character _character = other.gameObject.GetComponent<Character>();
            Brick BrickObject = this.gameObject.GetComponent<Brick>();
            //UNDONE
            if (_character.BrickCount > 0 && _character.ColorType != BrickObject.ColorType)
            {
                if (_character.ColorType != BrickObject.ColorType)
                {
                    _character.RemoveBrick();
                    BrickObject.ChangeColor(_character.ColorType);
                    MeshRenderer mesh = this.gameObject.GetComponent<MeshRenderer>();
                    mesh.enabled = true;
                }
                Vector3 TargetPoint = new Vector3(this.transform.position.x, this.transform.position.y + stepOffet, this.transform.position.z-0.1f);
                Player _player = other.gameObject.GetComponent<Player>();
                if (_player != null)
                {
                    _player.moveTarget(TargetPoint.y);
                }
            }
            else if (_character.ColorType == BrickObject.ColorType)
            {
                Vector3 TargetPoint = new Vector3(this.transform.position.x, this.transform.position.y + stepOffet, this.transform.position.z - 0.1f);
                Player _player = other.gameObject.GetComponent<Player>();
                if (_player != null)
                {
                    _player.moveTarget(TargetPoint.y);
                }
            }
        }
        if (other.gameObject.GetComponent<BotAI>())
        {
            Character _character = other.gameObject.GetComponent<Character>();
            Brick BrickObject = this.gameObject.GetComponent<Brick>();
            //UNDONE
            if (_character.BrickCount > 0 && _character.ColorType != BrickObject.ColorType)
            {
                //wall.gameObject.SetActive(false);
                _character.RemoveBrick();
                BrickObject.ChangeColor(_character.ColorType);
                MeshRenderer mesh = this.gameObject.GetComponent<MeshRenderer>();
                mesh.enabled = true;

            }
        }
    }
}
