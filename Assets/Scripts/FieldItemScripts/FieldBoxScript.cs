using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FieldBoxScript : FieldItemBase
{

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            PlayerScript script = player.GetComponent<PlayerScript>();

            //½Ã°£ ¸ØÃß±â
            //½ÀµæUI »ý¼º
            StageManager.Instance.FieldBoxEvent();

            Destroy(gameObject);
        }
    }

}
