using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.ImageEffects;

namespace ExamineSystem
    {
    public class DisplayInventory : MonoBehaviour
    {
        public MouseItem mouseItem = new MouseItem();
        public GameObject boolTestRemove;
        //public GameObject tudien;
        //public PlayerData playerData;
        public SaveSystem saveSystem;
        public TriggerQuaiVat triggerQuaiVat;
        public PlayerStats playerStats;
        public GameObject tudienCollider;
        public GameObject chaiBiaCollider;
        public GameObject violinStandCollider;
        public GameObject boNhangCollider;
        public GameObject diary;
        public GameObject inventoryPrefab;
        public GameObject volumeNoiChuyen;
        public GameObject denTvBat;
        public AudioSource cuocNoiChuyen;
        public GameObject danViolin;
        public GameObject video;
        public GameObject violinBieuDien;
        public GameObject boNhang;
        public GameObject laBua;
        public ZoomInTriggerRaycast zoom;
        //public GameObject imageObject;
        public GameObject caiInventory;
        public InventoryObject inventory;
        public int X_START;
        public int Y_START;
        public int X_SPACE_BETWEEN_ITEMS;
        public int NUMBER_OF_COLUMNS;
        public int Y_SPACE_BETWEEN_ITEMS;
        public InventoryDisappear sth;
        public RectTransform rectTransform;

        public GameObject chaiBiaCamera;
        public GameObject player;
        public ExamineRaycast examineRaycast;
        [SerializeField] Sprite crayonSprite;
        [SerializeField] Sprite tuaVitSprite;
        [SerializeField] Sprite doKhuiSprite;
        [SerializeField] Sprite violinSprite;
        [SerializeField] Sprite boNhangSprite;
        [SerializeField] Sprite batLuaSprite;
        [SerializeField] Sprite laBuaSprite;
        Dictionary<GameObject, InventorySlot> itemsDisplayed = new Dictionary<GameObject, InventorySlot>();
        private void Awake()
        {
            if (denTvBat)
                denTvBat.SetActive(false);
            if (violinBieuDien)
                violinBieuDien.SetActive(false);
        }


        // Start is called before the first frame update
        void Start()
        {
            CreateSlots();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateSlots();
            /*if (boolTestRemove)
                if (boolTestRemove.activeInHierarchy == true)
                {

                }*/
        }
        public void UpdateAfterThapNhang(GameObject obj)
        {
            var position = rectTransform.position;
            foreach (KeyValuePair<GameObject, InventorySlot> _slot in itemsDisplayed) ;
            if (examineRaycast.daLayBatLua && examineRaycast.daLayBoNhang == true)
            {
                if (inventory.database.GetItem[itemsDisplayed[obj].ID].uiDisplay == boNhangSprite || inventory.database.GetItem[itemsDisplayed[obj].ID].uiDisplay == batLuaSprite)
                {
                    inventory.RemoveItem(itemsDisplayed[obj].item);
                }

            }

        }
        public void UpdateSlots()
        {
            foreach (KeyValuePair<GameObject, InventorySlot> _slot in itemsDisplayed)
            {
                if (_slot.Value.ID >= 0)
                {
                    _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[_slot.Value.item.Id].uiDisplay;
                    _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                    _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = _slot.Value.amount == 1 ? "" : _slot.Value.amount.ToString("n0");
                }
                else
                {
                    _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                    _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                    _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";
                }
            }
        }
        public void CreateSlots()
        {
            itemsDisplayed = new Dictionary<GameObject, InventorySlot>();
            for (int i = 0; i < inventory.Container.Items.Length; i++)
            {
                var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

                AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
                AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
                AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
                AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
                AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });
                AddEvent(obj, EventTriggerType.PointerClick, delegate { OnPointerClick(obj); });

                itemsDisplayed.Add(obj, inventory.Container.Items[i]);
            }
        }
        private void AddEvent1(GameObject obj, KeyCode keyCode, UnityAction<BaseEventData> action)
        {
            KeyCode trigger = obj.GetComponent<KeyCode>();
        }
        private void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = obj.GetComponent<EventTrigger>();
            var eventTrigger = new EventTrigger.Entry();
            eventTrigger.eventID = type;
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
        }

        public void OnEnter(GameObject obj)
        {
            mouseItem.hoverObj = obj;
            if (itemsDisplayed.ContainsKey(obj))
            {
                mouseItem.hoverItem = itemsDisplayed[obj];
            }
        }
        public void OnExit(GameObject obj)
        {
            mouseItem.hoverObj = null;
            mouseItem.hoverItem = null;

        }
        public void OnDragStart(GameObject obj)
        {
            var mouseObject = new GameObject();
            var rt = mouseObject.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(50, 50);
            mouseObject.transform.SetParent(transform.parent);
            if (itemsDisplayed[obj].ID >= 0)
            {
                var img = mouseObject.AddComponent<Image>();
                img.sprite = inventory.database.GetItem[itemsDisplayed[obj].ID].uiDisplay;
                img.raycastTarget = false;
            }
            mouseItem.obj = mouseObject;
            mouseItem.item = itemsDisplayed[obj];
        }
        public void OnDragEnd(GameObject obj)
        {
            if (mouseItem.hoverObj)
            {
                inventory.MoveItem(itemsDisplayed[obj], itemsDisplayed[mouseItem.hoverObj]);
            }
            else
            {
                //inventory.RemoveItem(itemsDisplayed[obj].item);
            }
            Destroy(mouseItem.obj);
            mouseItem.item = null;
        }
        public void OnDrag(GameObject obj)
        {
            if (mouseItem.obj != null)
            {
                mouseItem.obj.GetComponent<RectTransform>().position = Input.mousePosition;
            }
        }
        public void OnPointerClick(GameObject obj)
        {
            var position = rectTransform.position;
            foreach (KeyValuePair<GameObject, InventorySlot> _slot in itemsDisplayed)
            {
                if (inventory.database.GetItem[itemsDisplayed[obj].ID].uiDisplay == crayonSprite)
                {
                    if (boolTestRemove.activeInHierarchy == true)
                    {
                        inventory.RemoveItem(itemsDisplayed[obj].item);
                        Debug.Log("hoho");
                        position.x = 6666;
                        rectTransform.position = position;
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                        sth.blur.enabled = false;
                        sth.bgi.SetActive(false);
                        sth.crosshair.enabled = true;
                        sth.player.enabled = true;
                        Time.timeScale = 1f;
                        sth.blurOut.SetActive(false);
                        (sth.mainCam.GetComponent(sth.examineRay) as MonoBehaviour).enabled = true;
                        sth.isInventoryAlreadyOn = false;
                        //sudungrasaoghivaoday(nhuanimation,...)                        
                    }
                }
                if (inventory.database.GetItem[itemsDisplayed[obj].ID].uiDisplay == tuaVitSprite)
                {
                    if (PlayerData.moTuDien == true)
                    {
                        inventory.RemoveItem(itemsDisplayed[obj].item);
                        Debug.Log("hoho");
                        position.x = 6666;
                        rectTransform.position = position;
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                        sth.blur.enabled = false;
                        sth.bgi.SetActive(false);
                        sth.crosshair.enabled = true;
                        sth.player.enabled = true;
                        Time.timeScale = 1f;
                        sth.blurOut.SetActive(false);
                        (sth.mainCam.GetComponent(sth.examineRay) as MonoBehaviour).enabled = true;
                        sth.isInventoryAlreadyOn = false;
                        if (tudienCollider)
                            Destroy(tudienCollider);
                        PlayerData.daSua = true;
                        //sudungrasaoghivaoday(nhuanimation,...)                        
                    }
                }
                if (inventory.database.GetItem[itemsDisplayed[obj].ID].uiDisplay == doKhuiSprite)
                {
                    if (PlayerData.nhinChaiBia == true)
                    {
                        inventory.RemoveItem(itemsDisplayed[obj].item);
                        Debug.Log("hoho");
                        position.x = 6666;
                        rectTransform.position = position;
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                        sth.blur.enabled = false;
                        sth.bgi.SetActive(false);
                        sth.crosshair.enabled = true;
                        sth.player.enabled = true;
                        Time.timeScale = 1f;
                        sth.blurOut.SetActive(false);
                        (sth.mainCam.GetComponent(sth.examineRay) as MonoBehaviour).enabled = true;
                        sth.isInventoryAlreadyOn = false;
                        if (chaiBiaCollider)
                            Destroy(chaiBiaCollider);
                        PlayerData.daKhuiChaiBia = true;
                        chaiBiaCamera.SetActive(false);
                        player.SetActive(true);
                        zoom.isInteracting = false;
                        //nhinChaiBia = false;
                        cuocNoiChuyen.Play();
                        volumeNoiChuyen.SetActive(true);
                        denTvBat.SetActive(true);
                        //saveSystem.Save();

                        PlayerData.firstSaved = true;
                        StartCoroutine(SauCuocNoiChuyen());


                        IEnumerator SauCuocNoiChuyen()
                        {
                            yield return new WaitForSeconds(35f);
                            danViolin.SetActive(true);
                            volumeNoiChuyen.SetActive(false);
                        }
                        //sudungrasaoghivaoday(nhuanimation,...)                        
                    }
                }
                if (inventory.database.GetItem[itemsDisplayed[obj].ID].uiDisplay == violinSprite)
                {
                    if (PlayerData.nhinViolinStand == true)
                    {
                        inventory.RemoveItem(itemsDisplayed[obj].item);
                        Debug.Log("hoho");
                        position.x = 6666;
                        rectTransform.position = position;
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                        sth.blur.enabled = false;
                        sth.bgi.SetActive(false);
                        sth.crosshair.enabled = true;
                        sth.player.enabled = true;
                        Time.timeScale = 1f;
                        sth.blurOut.SetActive(false);
                        (sth.mainCam.GetComponent(sth.examineRay) as MonoBehaviour).enabled = true;
                        sth.isInventoryAlreadyOn = false;
                        if (violinStandCollider)
                            Destroy(violinStandCollider);
                        //dadatviolin= true
                        // them nhung mon do tren tuong,...
                        //them mot cai volume chay qua
                        player.SetActive(true);
                        PlayerData.nhinViolinStand = false;
                        //playerStats.SavePlayer();
                        PlayerData.secondSaved = true;
                        //sudungrasaoghivaoday(nhuanimation,...)                        
                    }
                }

                if(inventory.database.GetItem[itemsDisplayed[obj].ID].uiDisplay == batLuaSprite)
                {

                    if (PlayerData.nhinBoNhang == true)
                    {
                        inventory.RemoveItem(itemsDisplayed[obj].item);
                        Debug.Log("hoho");
                        position.x = 6666;
                        rectTransform.position = position;
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                        sth.blur.enabled = false;
                        sth.bgi.SetActive(false);
                        sth.crosshair.enabled = true;
                        sth.player.enabled = true;
                        Time.timeScale = 1f;
                        sth.blurOut.SetActive(false);
                        (sth.mainCam.GetComponent(sth.examineRay) as MonoBehaviour).enabled = true;
                        sth.isInventoryAlreadyOn = false;
                        if (boNhangCollider)
                            Destroy(boNhangCollider);
                        //dadatviolin= true
                        // them nhung mon do tren tuong,...
                        //them mot cai volume chay qua
                        player.SetActive(true);
                        PlayerData.nhinBoNhang = false;
                        boNhang.SetActive(false);

                        StartCoroutine(waiter1());

                        IEnumerator waiter1()
                        {
                            yield return new WaitForSeconds(3f);
                            //playerTransform1.enabled = false;
                            //playerTransform.transform.position = new Vector3(-12f, 1f, -1f);
                            //playerTransform1.enabled = true;
                            
                            laBua.SetActive(true);
                        }
                        //laBua.SetActive(true);
                        //sudungrasaoghivaoday(nhuanimation,...)                  
                    }
                }

                if (inventory.database.GetItem[itemsDisplayed[obj].ID].uiDisplay == crayonSprite)
                {
                    if (PlayerData.nhinDiary == true)
                    {
                        inventory.RemoveItem(itemsDisplayed[obj].item);
                        Debug.Log("hoho");
                        position.x = 6666;
                        rectTransform.position = position;
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                        sth.blur.enabled = false;
                        sth.bgi.SetActive(false);
                        sth.crosshair.enabled = true;
                        sth.player.enabled = true;
                        Time.timeScale = 1f;
                        sth.blurOut.SetActive(false);
                        (sth.mainCam.GetComponent(sth.examineRay) as MonoBehaviour).enabled = true;
                        sth.isInventoryAlreadyOn = false;
                        if (diary)
                            Destroy(diary);
                        video.SetActive(true);
                        StartCoroutine(waiter());
                        PlayerData.isVideoPlayed = true;
                        
                        IEnumerator waiter()
                        {
                            yield return new WaitForSeconds(56f);
                            video.SetActive(false);
                            player.SetActive(true);
                            //ban.SetActive(true);
                            //nhungcaighe.SetActive(true);
                            //thaydoiquakhuctieptheo
                            //denchieuvao
                            //tv dc di chuyen qua cho khac
                            if (violinBieuDien)
                                violinBieuDien.SetActive(true);
                            PlayerData.isVideoPlayed = false;
                        }
                    }
                }

                if(inventory.database.GetItem[itemsDisplayed[obj].ID].uiDisplay == laBuaSprite)
                {
                    if (triggerQuaiVat.isQuaiVatAwake)
                    {
                        inventory.RemoveItem(itemsDisplayed[obj].item);
                        Debug.Log("hoho");
                        position.x = 6666;
                        rectTransform.position = position;
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                        sth.blur.enabled = false;
                        sth.bgi.SetActive(false);
                        sth.crosshair.enabled = true;
                        sth.player.enabled = true;
                        Time.timeScale = 1f;
                        sth.blurOut.SetActive(false);
                        (sth.mainCam.GetComponent(sth.examineRay) as MonoBehaviour).enabled = true;
                        sth.isInventoryAlreadyOn = false;
                        //if (boNhangCollider)
                            //Destroy(boNhangCollider);
                        //dadatviolin= true
                        // them nhung mon do tren tuong,...
                        //them mot cai volume chay qua
                        player.SetActive(true);
                        //nhinBoNhang = false;
                        //boNhang.SetActive(false);
                        triggerQuaiVat.isQuaiVatAwake = false;
                        PlayerData.usedAmulet = true;
                    }
                }


            }


            Debug.Log("hoho");

            position.x = 6666;
            rectTransform.position = position;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            sth.blur.enabled = false;
            sth.bgi.SetActive(false);
            sth.crosshair.enabled = true;
            sth.player.enabled = true;
            Time.timeScale = 1f;
            sth.blurOut.SetActive(false);
            (sth.mainCam.GetComponent(sth.examineRay) as MonoBehaviour).enabled = true;
            sth.isInventoryAlreadyOn = false;
        }

        public Vector3 GetPosition(int i)
        {
            return new Vector3(X_START + X_SPACE_BETWEEN_ITEMS * (i % NUMBER_OF_COLUMNS), Y_START + (-Y_SPACE_BETWEEN_ITEMS * (i / NUMBER_OF_COLUMNS)), 0f);
        }
    }
    public class MouseItem
    {
        public GameObject obj;
        public InventorySlot item;
        public InventorySlot hoverItem;
        public GameObject hoverObj;
    }
}
