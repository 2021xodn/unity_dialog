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
    public float moveSpeed = 20.0f; // 캐릭터 이동 속도
    public float jumpPower = 10.0f; // 점프력
    Rigidbody playerRigid;



    // Dialog

    private bool isDialog = false;   // 대화 중인지
    private bool isDialogDelay = false;  // 대화 도중 딜레이 (막 넘길 수 없게)
    int whichDialog;    // 1 = NPC, 2 = Item

    private Queue<(string, int, string[])> dialogSentences;

    // NPC Information
    public Player_area area;    // 플레이어 주변 인식
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
    int option; // 기본 값 0
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
    public string[] textTTS = new string[6];    // 최초1, 5 NPC 분량의 배열. 리스트로 추후 변경 가능
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

        // 인풋 필드 관련
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

        /*if (questManager.haveInputEvent(nearNPC)) { // 인풋필드 사용하는 퀘스트
            inputFieldRectTransform.anchoredPosition = new Vector2(0, -100);
            inputBtnRectTransform.anchoredPosition = new Vector2(325, -90);
            InputMode = 2;
            isDialog = true;
            Debug.Log("인풋필드 이동");
        }

        if (!isDialog) {    // 대화 시 이동 봉인  방향키 상하좌우 + 스페이스키로 대화 하게 설정함. 일단
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
                
        if (InputMode != 0) { // 무언가를 입력중 선택지, 텍스트
            if (InputMode == 1) {   // 선택지 입력
                if (keyboard.tKey.isPressed || keyboard.wKey.isPressed) {
                    option = 1;
                    npcSelectTransform.anchoredPosition = new Vector2(370, -170);
                }
                if (keyboard.gKey.isPressed || keyboard.sKey.isPressed) {
                    npcSelectTransform.anchoredPosition = new Vector2(370, -230);
                    option = 2;
                }

                if (keyboard.fKey.isPressed && !isDialogDelay) {  // 선택
                    Debug.Log("선택 : " + option);
                    npcOptionTextMesh[0].text = ""; // 텍스트 초기화
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
                            // proceedDialog();    // 대화 불러오고 선택지 처리
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
                            Debug.Log("인풋모드 2 진입");
                        }

                        else { 

                        }

                        // 대화 종료 처리
                        nearNPC.questProgress = false;
                        nearNPC.renderMark();
                        scoreManager.plusScore();
                        int nowScore = scoreManager.getScore();
                        scoreTextMesh.text = nowScore + " / 5 ";
                        Debug.Log("점수 : " + nowScore);


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

                    Debug.Log("NPC 정보 클 : " + nearNPC.questClear);
                    Debug.Log("NPC 정보 프 : " + nearNPC.questProgress);

                    Debug.Log("입력 성공");
                    Debug.Log(textTTS[countTextTTS - 1]);
                    Debug.Log("길이 : " + textTTS[countTextTTS - 1].Length);
                }

            }



        }

        else if (keyboard.fKey.isPressed && !isDialogDelay) {    // 대화

            if (!isDialog) {    // 최초 대화
                if (area.getRecognizedNPC()) { // NPC 최초 대화
                    whichDialog = 1;

                    nearNPC = area.getRecognizedNPC().GetComponent<AbstractNPC>();       // 해당 NPC 객체 저장
                    // 근처에 있는 해당 NPC의 대화 문자열을 가져옴
                    foreach ((string, int, string[]) sentence in nearNPC.getDialog(false)) {
                        dialogSentences.Enqueue(sentence);
                    }

                    npcTextNameMesh.text = nearNPC.NPC_NAME;
                    Debug.Log("대화 시작 NPC : " + nearNPC.name);

                }

                else if (area.getRecognizedQuest()) {   // 퀘스트 최초 대화
                    whichDialog = 2;

                    foreach ((string, int, string[]) sentence in questManager.getQuestDialog(area.getRecognizedQuest())) {
                        dialogSentences.Enqueue(sentence);
                    }

                    npcTextNameMesh.text = questManager.getQuestItemName(area.getRecognizedQuest().name);
                    npcTextNameMesh.text = "(퀘스트)";
                }

                else if (area.getRecognizedNote()) {    // 쪽지 최초 대화
                    whichDialog = 3;

                    dialogSentences.Enqueue((noteManager.getNoteDialog(area.getRecognizedNote()), 0, new string[] { }));
                    dialogSentences.Enqueue(("", 0, new string[] { }));

                    npcTextNameMesh.text = "(쪽지#" + area.getRecognizedNote().name.Replace("Note","") + ")";
                    Debug.Log("쪽지 대화 시작");
                }

                else if (area.getRecognizedTrigger() && !isPlayingTTS) {   // 기타 트리거 처리
                    isPlayingTTS = true;
                    tts.playTTS(textTTS);

                }

                if (whichDialog != 0) { // 대화가 시작되었을 때.
                    forDialogMassTransform.anchoredPosition = new Vector2(0, 0);
                    isDialogDelay = true;
                    isDialog = true;

                    // 카메라 && 이동 봉인
                    newCamera.enabled = false;
                    animScript.setMoveValue(true);
                    // 줌인 구현 필요 ★
                    // cameraPosition = camera.transform;

                    Debug.Log(whichDialog + "대화 시작");
                    proceedDialog();    // 대화 불러오고 선택지 처리
                    StartCoroutine(DialogDelay("Player_move DialogStartDelay | 딜레이 종료", 1));

                }

                
            }

            else if (!isDialogDelay) {  // 반복 대화
                isDialogDelay = true;

                proceedDialog();    // 대화 불러오고 선택지 처리
                                
                StartCoroutine(DialogDelay("Player_move DialogDelay | 대화 중", 1));

                if (InputMode == 0) {
                }

                if (dialogSentences.Count == 0) {   // 모든 대화를 읽어야 종료

                    // 카메라 && 이동 봉인 해제
                    newCamera.enabled = true;
                    animScript.setMoveValue(false);

                    isDialog = false;
                    Debug.Log("Player_move 대화 종료");
                    forDialogMassTransform.anchoredPosition = new Vector2(0, -1000);
                    
                    if (whichDialog == 1) {         // NPC 대화 종료
                        if (!nearNPC.questClear && !nearNPC.questProgress && option == 1) {    // 퀘스트 시작
                            nearNPC.setQuestOn(questManager.startQuest(nearNPC));   // questProgres = true;
                            nearNPC.renderMark();
                        }

                        if (nearNPC.questClear && nearNPC.questProgress) {   // 퀘스트 종료 (이후부턴 클리어 DIALOG 읽어옴)
                            nearNPC.questProgress = false;
                            nearNPC.renderMark();

                            scoreManager.plusScore();
                            int nowScore = scoreManager.getScore();
                            scoreTextMesh.text = nowScore + " / 5 ";
                            Debug.Log("점수 : " + nowScore);

                        }

                    }

                    else if (whichDialog == 2) {    // 아이템 대화 종료
                        questManager.proceedQuest(area.getRecognizedQuest());
                    }

                    else if (whichDialog == 3) {    // 쪽지 대화 종료
                        Destroy(area.getRecognizedNote());
                        
                    }


                    // Debug.Log("which : " + whichDialog);
                    whichDialog = 0;


                    // 줌 아웃 구현 필요 ★
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
            option = 1; // 기본 선택 1
            npcSelectTransform.anchoredPosition = new Vector2(370, -170);
            forOptionMassTransform.anchoredPosition = new Vector2(0, 0);
        }
        else {
            npcOptionTextMesh[0].text = "";
            npcOptionTextMesh[1].text = "";
        }
    }


    IEnumerator DialogDelay(string text, float delayTime) { // 대화 도중 딜레이
        yield return new WaitForSeconds(0.5f * delayTime); // 2초 대기
        isDialogDelay = false;
    }
}
