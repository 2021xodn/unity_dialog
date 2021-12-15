using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Reflection;
using TMPro;

public class Player_move : MonoBehaviour
{
    // Move
    public float moveSpeed = 20.0f; // ĳ���� �̵� �ӵ�
    public float jumpPower = 10.0f; // ������
    Rigidbody playerRigid;



    // Dialog

    private bool isDialog = false;   // ��ȭ ������
    private bool isDialogDelay = false;  // ��ȭ ���� ������ (�� �ѱ� �� ����)
    int whichDialog;    // 1 = NPC, 2 = Item

    private Queue<(string, int, string[])> dialogSentences;

    // NPC Information
    public Player_area area;    // �÷��̾� �ֺ� �ν�
    [HideInInspector]
    public AbstractNPC nearNPC;
    string nearNPCName;

    // Manager
    Quest_Manager questManager;
    Note_Manager noteManager;

    // Zoom
    Transform cameraPosition;


    // Canvas Mass
    public GameObject forDialogMass;
    RectTransform forDialogMassTransform;
    public GameObject forOptionMass;
    RectTransform forOptionMassTransform;
    public GameObject forInputMass;
    RectTransform forInputMassTransform;

    // Text
    public GameObject npcTextObj;
    TextMeshProUGUI npcTextMesh;
    public GameObject scoreTextObj;
    TextMeshProUGUI scoreTextMesh;
    int score = 0;
    public GameObject leashObj;

    // Text-Name
    public GameObject npcTextName;
    TextMeshProUGUI npcTextNameMesh;

    // Text-Select
    public GameObject[] npcOptionTextObj;
    TextMeshProUGUI[] npcOptionTextMesh;
    public GameObject imageNpcSelect;
    RectTransform npcSelectTransform;

    // Option
    int option; // �⺻ �� 0
    int optionType;

    

    // Input Field
    public GameObject inputFieldObj;
    TMP_InputField inputField;
    RectTransform inputFieldRectTransform;
    int InputMode;  // 0 : Nothing,, 1 : Choose,, 2 : Text
    public GameObject inputBtn;
    RectTransform inputBtnRectTransform;
    Btn_InputNPC btnData;

    // TTS
    public string[] textTTS = new string[6];    // ����1, 5 NPC �з��� �迭. ����Ʈ�� ���� ���� ����
    int countTextTTS = 0;
    public TextToSpeech tts;
    bool isPlayingTTS;


    // Keyboard , New Input System
    Keyboard keyboard;

    ECM2.Common.ThirdPersonCameraController newCamera;
    public Camera mainCamera;


    // Animator
    ECM2.Examples.Animation.UnityCharacterAnimatorExample.UnityCharacterAnimator animScript;

    // Score Manager
    public ScoreManager scoreManager;


    void Start() {
        playerRigid = GetComponent<Rigidbody>();

        dialogSentences = new Queue<(string, int, string[])>();

        questManager = new Quest_Manager();
        noteManager = new Note_Manager();

        // Canvas Mass
        forDialogMassTransform = forDialogMass.GetComponent<RectTransform>();
        forOptionMassTransform = forOptionMass.GetComponent<RectTransform>();
        forInputMassTransform = forInputMass.GetComponent<RectTransform>();

        // Text
        npcTextMesh = npcTextObj.GetComponent<TextMeshProUGUI>();
        scoreTextMesh = scoreTextObj.GetComponent<TextMeshProUGUI>();
        npcTextNameMesh = npcTextName.GetComponent<TextMeshProUGUI>();

        npcOptionTextMesh = new TextMeshProUGUI[] {
            npcOptionTextObj[0].GetComponent<TextMeshProUGUI>(),
            npcOptionTextObj[1].GetComponent<TextMeshProUGUI>()
        };

        npcSelectTransform = imageNpcSelect.GetComponent<RectTransform>();

        scoreTextMesh = scoreTextObj.GetComponent<TextMeshProUGUI>();
        scoreTextMesh.text = "0 / 5 ";

        // ��ǲ �ʵ� ����
        inputField = inputFieldObj.GetComponent<TMP_InputField>();
        inputFieldRectTransform = inputFieldObj.GetComponent<RectTransform>();
        inputBtnRectTransform = inputBtn.GetComponent<RectTransform>();
        btnData = inputBtn.GetComponent<Btn_InputNPC>();

        TTS_DATA tt = new TTS_DATA();
        string tmp = tt.getTTSDATA();
        Debug.Log(tmp);

        keyboard = Keyboard.current;
        newCamera = mainCamera.GetComponent<ECM2.Common.ThirdPersonCameraController>();

        animScript = GetComponent<ECM2.Examples.Animation.UnityCharacterAnimatorExample.UnityCharacterAnimator>();
    }

    void move_dummmy() {
        /*if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.UpArrow)) {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }*/


        /*if (Input.GetKey(KeyCode.D)) {
            playerRigid.velocity = new Vector3(moveSpeed, 0.0f, 0.0f);
        }
        if (Input.GetKey(KeyCode.A)) {
            playerRigid.velocity = new Vector3(-moveSpeed, 0.0f, 0.0f);
        }
        if (Input.GetKey(KeyCode.W)) {
            playerRigid.velocity = new Vector3(0.0f, 0.0f, moveSpeed);
        }
        if (Input.GetKey(KeyCode.S)) {
            playerRigid.velocity = new Vector3(0.0f, 0.0f, -moveSpeed);
        }*/

        /*if (questManager.haveInputEvent(nearNPC)) { // ��ǲ�ʵ� ����ϴ� ����Ʈ
            inputFieldRectTransform.anchoredPosition = new Vector2(0, -100);
            inputBtnRectTransform.anchoredPosition = new Vector2(325, -90);
            InputMode = 2;
            isDialog = true;
            Debug.Log("��ǲ�ʵ� �̵�");
        }

        if (!isDialog) {    // ��ȭ �� �̵� ����  ����Ű �����¿� + �����̽�Ű�� ��ȭ �ϰ� ������. �ϴ�
            if (Input.GetKey(KeyCode.Z)) {
                playerRigid.velocity = new Vector3(0.0f, jumpPower, 0.0f);
            }
            float _moveDirX = Input.GetAxisRaw("Horizontal");
            float _moveDirZ = Input.GetAxisRaw("Vertical");
            Vector3 _moveHorizontal = transform.right * _moveDirX;
            Vector3 _moveVertical = transform.forward * _moveDirZ;

            Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * moveSpeed;

            playerRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
        }*/
    }

    void Update() {


        /* 
         * Item2 Option
         * 0 : option X
         * 1 : option O Yes or Yes
         * 2 : option O Yes or No
         * 3 : option O Yes or No, Yes -> InputMode = 2
         */

        /*
         * InputMode
         * 0 : Nothing
         * 1 : Select Option
         * 2 : Type Text
         */
                
        if (InputMode != 0) { // ���𰡸� �Է��� ������, �ؽ�Ʈ
            if (InputMode == 1) {   // ������ �Է�
                if (keyboard.tKey.isPressed || keyboard.wKey.isPressed) {
                    option = 1;
                    npcSelectTransform.anchoredPosition = new Vector2(370, -170);
                }
                if (keyboard.gKey.isPressed || keyboard.sKey.isPressed) {
                    npcSelectTransform.anchoredPosition = new Vector2(370, -230);
                    option = 2;
                }

                if (keyboard.fKey.isPressed && !isDialogDelay) {  // ����
                    Debug.Log("���� : " + option);
                    npcOptionTextMesh[0].text = ""; // �ؽ�Ʈ �ʱ�ȭ
                    npcOptionTextMesh[1].text = "";

                    InputMode = 0;
                    forOptionMassTransform.anchoredPosition = new Vector2(0, -1000);
                    if (optionType == 1) {      // 
                        option = 1;
                    }

                    else if (optionType == 2) { // End Dialog
                        if (option == 2) {
                            while (dialogSentences.Count > 1) {
                                dialogSentences.Dequeue();
                            }
                            forDialogMassTransform.anchoredPosition = new Vector2(0, -1000);
                        }
                        else{
                            // proceedDialog();    // ��ȭ �ҷ����� ������ ó��
                        }
                    }

                    else if (optionType == 3) { // Type Text
                        if (option == 1) {
                            npcTextMesh.text = "";

                            InputMode = 2;
                            forInputMassTransform.anchoredPosition = new Vector2(0, 0);
                            forDialogMassTransform.anchoredPosition = new Vector2(0, -1000);

                            newCamera.enabled = false;
                            Cursor.visible = true;
                            Cursor.lockState = CursorLockMode.None;
                            animScript.setMoveValue(true);
                            Debug.Log("��ǲ��� 2 ����");
                        }

                        else { 

                        }

                        // ��ȭ ���� ó��
                        nearNPC.questProgress = false;
                        nearNPC.renderMark();
                        scoreManager.plusScore();
                        int nowScore = scoreManager.getScore();
                        scoreTextMesh.text = nowScore + " / 5 ";
                        Debug.Log("���� : " + nowScore);


                    }
                }
            }

            if (InputMode == 2) {  // Enter Text
                if (btnData.pointUp) {  // btn click
                    btnData.pointUp = false;
                    newCamera.enabled = true;

                    textTTS[countTextTTS++] = inputField.text;
                    inputField.text = "";

                    forInputMassTransform.anchoredPosition = new Vector2(3000, 0);
                    
                    InputMode = 0;
                    isDialog = false;

                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    newCamera.enabled = true;
                    animScript.setMoveValue(false);

                    GameObject obj = new GameObject();
                    obj.name = nearNPC.name;
                    // questManager.proceedQuest(obj);

                    // nearNPC.renderMark();

                    Debug.Log("NPC ���� Ŭ : " + nearNPC.questClear);
                    Debug.Log("NPC ���� �� : " + nearNPC.questProgress);

                    Debug.Log("�Է� ����");
                    Debug.Log(textTTS[countTextTTS - 1]);
                    Debug.Log("���� : " + textTTS[countTextTTS - 1].Length);
                }

            }



        }

        else if (keyboard.fKey.isPressed && !isDialogDelay) {    // ��ȭ

            if (!isDialog) {    // ���� ��ȭ
                if (area.getRecognizedNPC()) { // NPC ���� ��ȭ
                    whichDialog = 1;

                    nearNPC = area.getRecognizedNPC().GetComponent<AbstractNPC>();       // �ش� NPC ��ü ����
                    // ��ó�� �ִ� �ش� NPC�� ��ȭ ���ڿ��� ������
                    foreach ((string, int, string[]) sentence in nearNPC.getDialog(false)) {
                        dialogSentences.Enqueue(sentence);
                    }

                    npcTextNameMesh.text = nearNPC.NPC_NAME;
                    Debug.Log("��ȭ ���� NPC : " + nearNPC.name);

                }

                else if (area.getRecognizedQuest()) {   // ����Ʈ ���� ��ȭ
                    whichDialog = 2;

                    foreach ((string, int, string[]) sentence in questManager.getQuestDialog(area.getRecognizedQuest())) {
                        dialogSentences.Enqueue(sentence);
                    }

                    npcTextNameMesh.text = questManager.getQuestItemName(area.getRecognizedQuest().name);
                    npcTextNameMesh.text = "(����Ʈ)";
                }

                else if (area.getRecognizedNote()) {    // ���� ���� ��ȭ
                    whichDialog = 3;

                    dialogSentences.Enqueue((noteManager.getNoteDialog(area.getRecognizedNote()), 0, new string[] { }));
                    dialogSentences.Enqueue(("", 0, new string[] { }));

                    npcTextNameMesh.text = "(����#" + area.getRecognizedNote().name.Replace("Note","") + ")";
                    Debug.Log("���� ��ȭ ����");
                }

                else if (area.getRecognizedTrigger() && !isPlayingTTS) {   // ��Ÿ Ʈ���� ó��
                    isPlayingTTS = true;
                    tts.playTTS(textTTS);

                }

                if (whichDialog != 0) { // ��ȭ�� ���۵Ǿ��� ��.
                    forDialogMassTransform.anchoredPosition = new Vector2(0, 0);
                    isDialogDelay = true;
                    isDialog = true;

                    // ī�޶� && �̵� ����
                    newCamera.enabled = false;
                    animScript.setMoveValue(true);
                    // ���� ���� �ʿ� ��
                    // cameraPosition = camera.transform;

                    Debug.Log(whichDialog + "��ȭ ����");
                    proceedDialog();    // ��ȭ �ҷ����� ������ ó��
                    StartCoroutine(DialogDelay("Player_move DialogStartDelay | ������ ����", 1));

                }

                
            }

            else if (!isDialogDelay) {  // �ݺ� ��ȭ
                isDialogDelay = true;

                proceedDialog();    // ��ȭ �ҷ����� ������ ó��
                                
                StartCoroutine(DialogDelay("Player_move DialogDelay | ��ȭ ��", 1));

                if (InputMode == 0) {
                }

                if (dialogSentences.Count == 0) {   // ��� ��ȭ�� �о�� ����

                    // ī�޶� && �̵� ���� ����
                    newCamera.enabled = true;
                    animScript.setMoveValue(false);

                    isDialog = false;
                    Debug.Log("Player_move ��ȭ ����");
                    forDialogMassTransform.anchoredPosition = new Vector2(0, -1000);
                    
                    if (whichDialog == 1) {         // NPC ��ȭ ����
                        if (!nearNPC.questClear && !nearNPC.questProgress && option == 1) {    // ����Ʈ ����
                            nearNPC.setQuestOn(questManager.startQuest(nearNPC));   // questProgres = true;
                            nearNPC.renderMark();
                        }

                        if (nearNPC.questClear && nearNPC.questProgress) {   // ����Ʈ ���� (���ĺ��� Ŭ���� DIALOG �о��)
                            nearNPC.questProgress = false;
                            nearNPC.renderMark();

                            scoreManager.plusScore();
                            int nowScore = scoreManager.getScore();
                            scoreTextMesh.text = nowScore + " / 5 ";
                            Debug.Log("���� : " + nowScore);

                        }

                    }

                    else if (whichDialog == 2) {    // ������ ��ȭ ����
                        questManager.proceedQuest(area.getRecognizedQuest());
                    }

                    else if (whichDialog == 3) {    // ���� ��ȭ ����
                        Destroy(area.getRecognizedNote());
                        
                    }


                    // Debug.Log("which : " + whichDialog);
                    whichDialog = 0;


                    // �� �ƿ� ���� �ʿ� ��
                }
            }
        }
    }

    void proceedDialog() {
        (string, int, string[]) sentences = dialogSentences.Dequeue();
        npcTextMesh.text = sentences.Item1;

        if (sentences.Item2 > 0) {
            npcOptionTextMesh[0].text = sentences.Item3[0];
            npcOptionTextMesh[1].text = sentences.Item3[1];
            InputMode = 1;
            optionType = sentences.Item2;
            option = 1; // �⺻ ���� 1
            npcSelectTransform.anchoredPosition = new Vector2(370, -170);
            forOptionMassTransform.anchoredPosition = new Vector2(0, 0);
        }
        else {
            npcOptionTextMesh[0].text = "";
            npcOptionTextMesh[1].text = "";
        }
    }


    IEnumerator DialogDelay(string text, float delayTime) { // ��ȭ ���� ������
        yield return new WaitForSeconds(0.5f * delayTime); // 2�� ���
        isDialogDelay = false;
    }
}
