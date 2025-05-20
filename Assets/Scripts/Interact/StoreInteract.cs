using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreInteract : InteractGo
{
    Store store;
    private void Awake()
    {
        store = GetComponent<Store>();
    }
    public override void CloseInteract()
    {
        PlayerState.Instance.ChangeState(PlayerState.State.Idle);
    }

    public override void InteractObject()
    {
        store.SetInventoryState(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
