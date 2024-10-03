using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadExcel : MonoBehaviour
{
    public bool updated;
    public bool bookUpdated;
    public bool regionUpdated;
    
    public Item_book blankItem_book;
    public Item_regions blankItem_regions;
    
    public List<Item_book> itemDatabase_book = new List<Item_book>();
    public List<Item_regions> itemDatabase_regions = new List<Item_regions>();

    private LoadingUpdate _loadingUpdate;
    private Controller _controller;
    private SignController _signController;
    private OnlineGameServer _onlineGameServer;
    private PlayerInfo _playerInfo;

    private void Start()
    {
        _loadingUpdate = FindObjectOfType<LoadingUpdate>();
        _controller = FindObjectOfType<Controller>();
        _signController = FindObjectOfType<SignController>();
        _onlineGameServer = FindObjectOfType<OnlineGameServer>();
        _playerInfo = FindObjectOfType<PlayerInfo>();
    }

    private void Update()
    {
        if (bookUpdated && regionUpdated && !updated)
        {
            _playerInfo.SetTimeFromData();
            _playerInfo.SetInGameTextFromData();
            _playerInfo.UpdateClass();
            
            _controller.SetLoadDataUI("book");
            _controller.SetupRegionList();
            _controller.UpdateUI();
            
            if (!_onlineGameServer.serverUrlUpdated)
                _onlineGameServer.SetServerUrl(itemDatabase_book[0].socketAddress);
            
            _signController.loading = false;
            updated = true;
        }
    }

    public void LoadItemData(string type)
    {
        if (type == "book")
        {
            string filePath_book = Path.Combine(Application.persistentDataPath, "Resources", "book.csv");
            string fileContent_book = File.ReadAllText(filePath_book);
            List<Dictionary<string, object>> data_book = CSVReader.ReadFromString(fileContent_book);
            for (var i = 0; i < data_book.Count; i++)
            {
                string logout = data_book[i]["logout"].ToString();
                string deleteAccount = data_book[i]["deleteAccount"].ToString();
                string terms = data_book[i]["terms"].ToString();
                string avatarTitle = data_book[i]["avatarTitle"].ToString();
                string signUpError = data_book[i]["signUpError"].ToString();
                string optionsTitle = data_book[i]["optionsTitle"].ToString();
                string profileTitle = data_book[i]["profileTitle"].ToString();
                string nicknameTitle = data_book[i]["nicknameTitle"].ToString();
                string nicknameHolder = data_book[i]["nicknameHolder"].ToString();
                string regionTitle = data_book[i]["regionTitle"].ToString();
                string birthdateTitle = data_book[i]["birthdateTitle"].ToString();
                string boardTitle = data_book[i]["boardTitle"].ToString();
                string checkerTitle = data_book[i]["checkerTitle"].ToString();
                string nicknameEmptyError = data_book[i]["nicknameEmptyError"].ToString();
                string nicknameCharError = data_book[i]["nicknameCharError"].ToString();
                string nicknameEnglishError = data_book[i]["nicknameEnglishError"].ToString();
                string premiumPrice = data_book[i]["premiumPrice"].ToString();
                string premiumInfo1 = data_book[i]["premiumInfo1"].ToString();
                string premiumInfo2 = data_book[i]["premiumInfo2"].ToString();
                string premiumTitle = data_book[i]["premiumTitle"].ToString();
                string premiumReward1 = data_book[i]["premiumReward1"].ToString();
                string premiumReward2 = data_book[i]["premiumReward2"].ToString();
                string premiumReward3 = data_book[i]["premiumReward3"].ToString();
                string socketAddress = data_book[i]["socketAddress"].ToString();
                string turnTimeC = data_book[i]["turnTimeC"].ToString();
                string turnTimeB = data_book[i]["turnTimeB"].ToString();
                string turnTimeA = data_book[i]["turnTimeA"].ToString();
                string doubleTime = data_book[i]["doubleTime"].ToString();
                string doubleRequest = data_book[i]["doubleRequest"].ToString();
                string doubleResponse = data_book[i]["doubleResponse"].ToString();
                string resignText = data_book[i]["resignText"].ToString();
                string classC = data_book[i]["classC"].ToString();
                string classB = data_book[i]["classB"].ToString();
                string classA = data_book[i]["classA"].ToString();
                string startTournament = data_book[i]["startTournament"].ToString();
                string tournamentPremiumInfo = data_book[i]["tournamentPremiumInfo"].ToString();
                string tournamentInviteFriendsInfo = data_book[i]["tournamentInviteFriendsInfo"].ToString();
                string tournamentCompleteDone = data_book[i]["tournamentCompleteDone"].ToString();
                string shareCode = data_book[i]["shareCode"].ToString();
                string regionButton = data_book[i]["regionButton"].ToString();
                string stateButton = data_book[i]["stateButton"].ToString();

                AddItem_book(logout, deleteAccount, terms, avatarTitle, signUpError, optionsTitle,
                    profileTitle,
                    nicknameTitle, nicknameHolder, regionTitle, birthdateTitle, boardTitle, checkerTitle,
                    nicknameEmptyError, nicknameCharError, nicknameEnglishError, premiumPrice, premiumInfo1, premiumInfo2, premiumTitle, premiumReward1, premiumReward2, premiumReward3, socketAddress,
                    turnTimeC, turnTimeB, turnTimeA, doubleTime, doubleRequest, doubleResponse, resignText, classC, classB, classA, startTournament, tournamentPremiumInfo, tournamentInviteFriendsInfo,
                    tournamentCompleteDone, shareCode, regionButton, stateButton);
            }

            bookUpdated = true;
        }
        else if (type == "regions")
        {
            string filePath_regions = Path.Combine(Application.persistentDataPath, "Resources", "regions.csv");
            string fileContent_regions = File.ReadAllText(filePath_regions);
            List<Dictionary<string, object>> data_regions = CSVReader.ReadFromString(fileContent_regions);
            for (var i = 0; i < data_regions.Count; i++)
            {
                string r1 = data_regions[i]["r1"].ToString();
                string r2 = data_regions[i]["r2"].ToString();
                string r3 = data_regions[i]["r3"].ToString();
                string r4 = data_regions[i]["r4"].ToString();
                string r5 = data_regions[i]["r5"].ToString();
                string r6 = data_regions[i]["r6"].ToString();
                string r7 = data_regions[i]["r7"].ToString();
                string r8 = data_regions[i]["r8"].ToString();
                string r9 = data_regions[i]["r9"].ToString();
                string r10 = data_regions[i]["r10"].ToString();
                string r11 = data_regions[i]["r11"].ToString();
                string r12 = data_regions[i]["r12"].ToString();
                string r13 = data_regions[i]["r13"].ToString();
                string r14 = data_regions[i]["r14"].ToString();
                string r15 = data_regions[i]["r15"].ToString();
                string r16 = data_regions[i]["r16"].ToString();
                string r17 = data_regions[i]["r17"].ToString();
                string r18 = data_regions[i]["r18"].ToString();
                string r19 = data_regions[i]["r19"].ToString();
                string r20 = data_regions[i]["r20"].ToString();
                string r21 = data_regions[i]["r21"].ToString();
                string r22 = data_regions[i]["r22"].ToString();
                string r23 = data_regions[i]["r23"].ToString();
                string r24 = data_regions[i]["r24"].ToString();
                string r25 = data_regions[i]["r25"].ToString();
                string r26 = data_regions[i]["r26"].ToString();
                string r27 = data_regions[i]["r27"].ToString();
                string r28 = data_regions[i]["r28"].ToString();
                string r29 = data_regions[i]["r29"].ToString();
                string r30 = data_regions[i]["r30"].ToString();
                string r31 = data_regions[i]["r31"].ToString();
                string r32 = data_regions[i]["r32"].ToString();
                string r33 = data_regions[i]["r33"].ToString();
                string r34 = data_regions[i]["r34"].ToString();
                string r35 = data_regions[i]["r35"].ToString();
                string r36 = data_regions[i]["r36"].ToString();
                string r37 = data_regions[i]["r37"].ToString();
                string r38 = data_regions[i]["r38"].ToString();
                string r39 = data_regions[i]["r39"].ToString();
                string r40 = data_regions[i]["r40"].ToString();
                string r41 = data_regions[i]["r41"].ToString();
                string r42 = data_regions[i]["r42"].ToString();
                string r43 = data_regions[i]["r43"].ToString();
                string r44 = data_regions[i]["r44"].ToString();
                string r45 = data_regions[i]["r45"].ToString();
                string r46 = data_regions[i]["r46"].ToString();
                string r47 = data_regions[i]["r47"].ToString();
                string r48 = data_regions[i]["r48"].ToString();
                string r49 = data_regions[i]["r49"].ToString();
                string r50 = data_regions[i]["r50"].ToString();
                string r51 = data_regions[i]["r51"].ToString();
                string r52 = data_regions[i]["r52"].ToString();
                string r53 = data_regions[i]["r53"].ToString();
                string r54 = data_regions[i]["r54"].ToString();
                string r55 = data_regions[i]["r55"].ToString();
                string r56 = data_regions[i]["r56"].ToString();
                string r57 = data_regions[i]["r57"].ToString();
                string r58 = data_regions[i]["r58"].ToString();
                string r59 = data_regions[i]["r59"].ToString();
                string r60 = data_regions[i]["r60"].ToString();
                string r61 = data_regions[i]["r61"].ToString();
                string r62 = data_regions[i]["r62"].ToString();
                string r63 = data_regions[i]["r63"].ToString();
                string r64 = data_regions[i]["r64"].ToString();
                string r65 = data_regions[i]["r65"].ToString();
                string r66 = data_regions[i]["r66"].ToString();
                string r67 = data_regions[i]["r67"].ToString();
                string r68 = data_regions[i]["r68"].ToString();
                string r69 = data_regions[i]["r69"].ToString();
                string r70 = data_regions[i]["r70"].ToString();
                string r71 = data_regions[i]["r71"].ToString();
                string r72 = data_regions[i]["r72"].ToString();
                string r73 = data_regions[i]["r73"].ToString();
                string r74 = data_regions[i]["r74"].ToString();
                string r75 = data_regions[i]["r75"].ToString();
                string r76 = data_regions[i]["r76"].ToString();
                string r77 = data_regions[i]["r77"].ToString();
                string r78 = data_regions[i]["r78"].ToString();
                string r79 = data_regions[i]["r79"].ToString();
                string r80 = data_regions[i]["r80"].ToString();
                string r81 = data_regions[i]["r81"].ToString();
                string r82 = data_regions[i]["r82"].ToString();
                string r83 = data_regions[i]["r83"].ToString();
                string r84 = data_regions[i]["r84"].ToString();
                string r85 = data_regions[i]["r85"].ToString();
                string r86 = data_regions[i]["r86"].ToString();
                string r87 = data_regions[i]["r87"].ToString();
                string r88 = data_regions[i]["r88"].ToString();
                string r89 = data_regions[i]["r89"].ToString();
                string r90 = data_regions[i]["r90"].ToString();
                string r91 = data_regions[i]["r91"].ToString();
                string r92 = data_regions[i]["r92"].ToString();
                string r93 = data_regions[i]["r93"].ToString();
                string r94 = data_regions[i]["r94"].ToString();
                string r95 = data_regions[i]["r95"].ToString();
                string r96 = data_regions[i]["r96"].ToString();
                string r97 = data_regions[i]["r97"].ToString();
                string r98 = data_regions[i]["r98"].ToString();
                string r99 = data_regions[i]["r99"].ToString();
                string r100 = data_regions[i]["r100"].ToString();
                string r101 = data_regions[i]["r101"].ToString();
                string r102 = data_regions[i]["r102"].ToString();
                string r103 = data_regions[i]["r103"].ToString();
                string r104 = data_regions[i]["r104"].ToString();
                string r105 = data_regions[i]["r105"].ToString();
                string r106 = data_regions[i]["r106"].ToString();
                string r107 = data_regions[i]["r107"].ToString();
                string r108 = data_regions[i]["r108"].ToString();
                string r109 = data_regions[i]["r109"].ToString();
                string r110 = data_regions[i]["r110"].ToString();
                string r111 = data_regions[i]["r111"].ToString();
                string r112 = data_regions[i]["r112"].ToString();
                string r113 = data_regions[i]["r113"].ToString();
                string r114 = data_regions[i]["r114"].ToString();
                string r115 = data_regions[i]["r115"].ToString();
                string r116 = data_regions[i]["r116"].ToString();
                string r117 = data_regions[i]["r117"].ToString();
                string r118 = data_regions[i]["r118"].ToString();
                string r119 = data_regions[i]["r119"].ToString();
                string r120 = data_regions[i]["r120"].ToString();
                string r121 = data_regions[i]["r121"].ToString();
                string r122 = data_regions[i]["r122"].ToString();
                string r123 = data_regions[i]["r123"].ToString();
                string r124 = data_regions[i]["r124"].ToString();
                string r125 = data_regions[i]["r125"].ToString();
                string r126 = data_regions[i]["r126"].ToString();
                string r127 = data_regions[i]["r127"].ToString();
                string r128 = data_regions[i]["r128"].ToString();
                string r129 = data_regions[i]["r129"].ToString();
                string r130 = data_regions[i]["r130"].ToString();
                string r131 = data_regions[i]["r131"].ToString();
                string r132 = data_regions[i]["r132"].ToString();
                string r133 = data_regions[i]["r133"].ToString();
                string r134 = data_regions[i]["r134"].ToString();
                string r135 = data_regions[i]["r135"].ToString();
                string r136 = data_regions[i]["r136"].ToString();
                string r137 = data_regions[i]["r137"].ToString();
                string r138 = data_regions[i]["r138"].ToString();
                string r139 = data_regions[i]["r139"].ToString();
                string r140 = data_regions[i]["r140"].ToString();
                string r141 = data_regions[i]["r141"].ToString();
                string r142 = data_regions[i]["r142"].ToString();
                string r143 = data_regions[i]["r143"].ToString();
                string r144 = data_regions[i]["r144"].ToString();
                string r145 = data_regions[i]["r145"].ToString();
                string r146 = data_regions[i]["r146"].ToString();
                string r147 = data_regions[i]["r147"].ToString();
                string r148 = data_regions[i]["r148"].ToString();
                string r149 = data_regions[i]["r149"].ToString();
                string r150 = data_regions[i]["r150"].ToString();
                string r151 = data_regions[i]["r151"].ToString();
                string r152 = data_regions[i]["r152"].ToString();
                string r153 = data_regions[i]["r153"].ToString();
                string r154 = data_regions[i]["r154"].ToString();
                string r155 = data_regions[i]["r155"].ToString();
                string r156 = data_regions[i]["r156"].ToString();
                string r157 = data_regions[i]["r157"].ToString();
                string r158 = data_regions[i]["r158"].ToString();
                string r159 = data_regions[i]["r159"].ToString();
                string r160 = data_regions[i]["r160"].ToString();
                string r161 = data_regions[i]["r161"].ToString();
                string r162 = data_regions[i]["r162"].ToString();
                string r163 = data_regions[i]["r163"].ToString();
                string r164 = data_regions[i]["r164"].ToString();
                string r165 = data_regions[i]["r165"].ToString();
                string r166 = data_regions[i]["r166"].ToString();
                string r167 = data_regions[i]["r167"].ToString();
                string r168 = data_regions[i]["r168"].ToString();
                string r169 = data_regions[i]["r169"].ToString();
                string r170 = data_regions[i]["r170"].ToString();
                string r171 = data_regions[i]["r171"].ToString();
                string r172 = data_regions[i]["r172"].ToString();
                string r173 = data_regions[i]["r173"].ToString();
                string r174 = data_regions[i]["r174"].ToString();
                string r175 = data_regions[i]["r175"].ToString();
                string r176 = data_regions[i]["r176"].ToString();
                string r177 = data_regions[i]["r177"].ToString();
                string r178 = data_regions[i]["r178"].ToString();
                string r179 = data_regions[i]["r179"].ToString();
                string r180 = data_regions[i]["r180"].ToString();
                string r181 = data_regions[i]["r181"].ToString();
                string r182 = data_regions[i]["r182"].ToString();
                string r183 = data_regions[i]["r183"].ToString();
                string r184 = data_regions[i]["r184"].ToString();
                string r185 = data_regions[i]["r185"].ToString();
                string r186 = data_regions[i]["r186"].ToString();

                AddItem_regions(r1, r2, r3, r4, r5, r6, r7, r8, r9, r10, 
                    r11, r12, r13, r14, r15, r16, r17, r18, r19, r20, 
                    r21, r22, r23, r24, r25, r26, r27, r28, r29, r30, 
                    r31, r32, r33, r34, r35, r36, r37, r38, r39, r40, 
                    r41, r42, r43, r44, r45, r46, r47, r48, r49, r50, 
                    r51, r52, r53, r54, r55, r56, r57, r58, r59, r60, 
                    r61, r62, r63, r64, r65, r66, r67, r68, r69, r70, 
                    r71, r72, r73, r74, r75, r76, r77, r78, r79, r80, 
                    r81, r82, r83, r84, r85, r86, r87, r88, r89, r90, 
                    r91, r92, r93, r94, r95, r96, r97, r98, r99, r100, 
                    r101, r102, r103, r104, r105, r106, r107, r108, r109, r110, 
                    r111, r112, r113, r114, r115, r116, r117, r118, r119, r120, 
                    r121, r122, r123, r124, r125, r126, r127, r128, r129, r130, 
                    r131, r132, r133, r134, r135, r136, r137, r138, r139, r140, 
                    r141, r142, r143, r144, r145, r146, r147, r148, r149, r150, 
                    r151, r152, r153, r154, r155, r156, r157, r158, r159, r160, 
                    r161, r162, r163, r164, r165, r166, r167, r168, r169, r170, 
                    r171, r172, r173, r174, r175, r176, r177, r178, r179, r180, 
                    r181, r182, r183, r184, r185, r186);
            }

            regionUpdated = true;
        }
    }

    void AddItem_book(string logout, string deleteAccount, string terms, string avatarTitle,
        string signUpError, string optionsTitle, string profileTitle, string nicknameTitle, string nicknameHolder,
        string regionTitle, string birthdateTitle, string boardTitle, string checkerTitle, string nicknameEmptyError,
        string nicknameCharError, string nicknameEnglishError, string premiumPrice, string premiumInfo1, string premiumInfo2, string premiumTitle, string premiumReward1, string premiumReward2, string premiumReward3, string socketAddress,
        string turnTimeC, string turnTimeB, string turnTimeA, string doubleTime, string doubleRequest, string doubleResponse, string resignText, string classC, string classB, string classA, string startTournament, string tournamentPremiumInfo,
        string tournamentInviteFriendsInfo, string tournamentCompleteDone, string shareCode, string regionButton, string stateButton)
    {
        Item_book tempItem = new Item_book(blankItem_book);

        tempItem.logout = logout;
        tempItem.deleteAccount = deleteAccount;
        tempItem.terms = terms;
        tempItem.avatarTitle = avatarTitle;
        tempItem.signUpError = signUpError;
        tempItem.optionsTitle = optionsTitle;
        tempItem.profileTitle = profileTitle;
        tempItem.nicknameTitle = nicknameTitle;
        tempItem.nicknameHolder = nicknameHolder;
        tempItem.regionTitle = regionTitle;
        tempItem.birthdateTitle = birthdateTitle;
        tempItem.boardTitle = boardTitle;
        tempItem.checkerTitle = checkerTitle;
        tempItem.nicknameEmptyError = nicknameEmptyError;
        tempItem.nicknameCharError = nicknameCharError;
        tempItem.nicknameEnglishError = nicknameEnglishError;
        tempItem.premiumPrice = premiumPrice;
        tempItem.premiumInfo1 = premiumInfo1;
        tempItem.premiumInfo2 = premiumInfo2;
        tempItem.premiumTitle = premiumTitle;
        tempItem.premiumReward1 = premiumReward1;
        tempItem.premiumReward2 = premiumReward2;
        tempItem.premiumReward3 = premiumReward3;
        tempItem.socketAddress = socketAddress;
        tempItem.turnTimeC = turnTimeC;
        tempItem.turnTimeB = turnTimeB;
        tempItem.turnTimeA = turnTimeA;
        tempItem.doubleTime = doubleTime;
        tempItem.doubleRequest = doubleRequest;
        tempItem.doubleResponse = doubleResponse;
        tempItem.resignText = resignText;
        tempItem.classC = classC;
        tempItem.classB = classB;
        tempItem.classA = classA;
        tempItem.startTournament = startTournament;
        tempItem.tournamentPremiumInfo = tournamentPremiumInfo;
        tempItem.tournamentInviteFriendsInfo = tournamentInviteFriendsInfo;
        tempItem.tournamentCompleteDone = tournamentCompleteDone;
        tempItem.shareCode = shareCode;
        tempItem.regionButton = regionButton;
        tempItem.stateButton = stateButton;

        itemDatabase_book.Add(tempItem);
    }

    void AddItem_regions(string r1,  string r2,  string r3,  string r4,  string r5,  string r6,  string r7,  string r8,  string r9,  string r10, 
    string r11,  string r12,  string r13,  string r14,  string r15,  string r16,  string r17,  string r18,  string
        r19,  string r20, 
    string r21,  string r22,  string r23,  string r24,  string r25,  string r26,  string r27,  string r28,  string
        r29,  string r30, 
    string r31,  string r32,  string r33,  string r34,  string r35,  string r36,  string r37,  string r38,  string
        r39,  string r40, 
    string r41,  string r42,  string r43,  string r44,  string r45,  string r46,  string r47,  string r48,  string
        r49,  string r50, 
    string r51,  string r52,  string r53,  string r54,  string r55,  string r56,  string r57,  string r58,  string
        r59,  string r60, 
    string r61,  string r62,  string r63,  string r64,  string r65,  string r66,  string r67,  string r68,  string
        r69,  string r70, 
    string r71,  string r72,  string r73,  string r74,  string r75,  string r76,  string r77,  string r78,  string
        r79,  string r80, 
    string r81,  string r82,  string r83,  string r84,  string r85,  string r86,  string r87,  string r88,  string
        r89,  string r90, 
    string r91,  string r92,  string r93,  string r94,  string r95,  string r96,  string r97,  string r98,  string
        r99,  string r100, 
    string r101,  string r102,  string r103,  string r104,  string r105,  string r106,  string r107,  string
        r108,  string r109,  string r110, 
    string r111,  string r112,  string r113,  string r114,  string r115,string r116, string r117,  string r118,  string r119,  string
        r120,  string r121,  string r122, 
    string r123,  string r124,  string r125,  string r126,  string r127,  string r128,  string r129,  string
        r130,  string r131,  string r132, 
    string r133,  string r134,  string r135,  string r136,  string r137,  string r138,  string r139,  string
        r140,  string r141,  string r142, 
    string r143,  string r144,  string r145,  string r146,  string r147,  string r148,  string r149,  string
        r150,  string r151,  string r152, 
    string r153,  string r154,  string r155,  string r156,  string r157,  string r158,  string r159,  string
        r160,  string r161,  string r162, 
    string r163,  string r164,  string r165,  string r166,  string r167,  string r168,  string r169,  string
        r170,  string r171,  string r172, 
    string r173,  string r174,  string r175,  string r176,  string r177,  string r178,  string r179,  string
        r180,  string r181,  string r182, 
    string r183,  string r184,  string r185,  string r186)
    {
            Item_regions tempItem = new Item_regions(blankItem_regions);

            tempItem.r1 = r1;
            tempItem.r2 = r2;
            tempItem.r3 = r3;
            tempItem.r4 = r4;
            tempItem.r5 = r5;
            tempItem.r6 = r6;
            tempItem.r7 = r7;
            tempItem.r8 = r8;
            tempItem.r9 = r9;
            tempItem.r10 = r10;
            tempItem.r11 = r11;
            tempItem.r12 = r12;
            tempItem.r13 = r13;
            tempItem.r14 = r14;
            tempItem.r15 = r15;
            tempItem.r16 = r16;
            tempItem.r17 = r17;
            tempItem.r18 = r18;
            tempItem.r19 = r19;
            tempItem.r20 = r20;
            tempItem.r21 = r21;
            tempItem.r22 = r22;
            tempItem.r23 = r23;
            tempItem.r24 = r24;
            tempItem.r25 = r25;
            tempItem.r26 = r26;
            tempItem.r27 = r27;
            tempItem.r28 = r28;
            tempItem.r29 = r29;
            tempItem.r30 = r30;
            tempItem.r31 = r31;
            tempItem.r32 = r32;
            tempItem.r33 = r33;
            tempItem.r34 = r34;
            tempItem.r35 = r35;
            tempItem.r36 = r36;
            tempItem.r37 = r37;
            tempItem.r38 = r38;
            tempItem.r39 = r39;
            tempItem.r40 = r40;
            tempItem.r41 = r41;
            tempItem.r42 = r42;
            tempItem.r43 = r43;
            tempItem.r44 = r44;
            tempItem.r45 = r45;
            tempItem.r46 = r46;
            tempItem.r47 = r47;
            tempItem.r48 = r48;
            tempItem.r49 = r49;
            tempItem.r50 = r50;
            tempItem.r51 = r51;
            tempItem.r52 = r52;
            tempItem.r53 = r53;
            tempItem.r54 = r54;
            tempItem.r55 = r55;
            tempItem.r56 = r56;
            tempItem.r57 = r57;
            tempItem.r58 = r58;
            tempItem.r59 = r59;
            tempItem.r60 = r60;
            tempItem.r61 = r61;
            tempItem.r62 = r62;
            tempItem.r63 = r63;
            tempItem.r64 = r64;
            tempItem.r65 = r65;
            tempItem.r66 = r66;
            tempItem.r67 = r67;
            tempItem.r68 = r68;
            tempItem.r69 = r69;
            tempItem.r70 = r70;
            tempItem.r71 = r71;
            tempItem.r72 = r72;
            tempItem.r73 = r73;
            tempItem.r74 = r74;
            tempItem.r75 = r75;
            tempItem.r76 = r76;
            tempItem.r77 = r77;
            tempItem.r78 = r78;
            tempItem.r79 = r79;
            tempItem.r80 = r80;
            tempItem.r81 = r81;
            tempItem.r82 = r82;
            tempItem.r83 = r83;
            tempItem.r84 = r84;
            tempItem.r85 = r85;
            tempItem.r86 = r86;
            tempItem.r87 = r87;
            tempItem.r88 = r88;
            tempItem.r89 = r89;
            tempItem.r90 = r90;
            tempItem.r91 = r91;
            tempItem.r92 = r92;
            tempItem.r93 = r93;
            tempItem.r94 = r94;
            tempItem.r95 = r95;
            tempItem.r96 = r96;
            tempItem.r97 = r97;
            tempItem.r98 = r98;
            tempItem.r99 = r99;
            tempItem.r100 = r100;
            tempItem.r101 = r101;
            tempItem.r102 = r102;
            tempItem.r103 = r103;
            tempItem.r104 = r104;
            tempItem.r105 = r105;
            tempItem.r106 = r106;
            tempItem.r107 = r107;
            tempItem.r108 = r108;
            tempItem.r109 = r109;
            tempItem.r110 = r110;
            tempItem.r111 = r111;
            tempItem.r112 = r112;
            tempItem.r113 = r113;
            tempItem.r114 = r114;
            tempItem.r115 = r115;
            tempItem.r116 = r116;
            tempItem.r117 = r117;
            tempItem.r118 = r118;
            tempItem.r119 = r119;
            tempItem.r120 = r120;
            tempItem.r121 = r121;
            tempItem.r122 = r122;
            tempItem.r123 = r123;
            tempItem.r124 = r124;
            tempItem.r125 = r125;
            tempItem.r126 = r126;
            tempItem.r127 = r127;
            tempItem.r128 = r128;
            tempItem.r130 = r130;
            tempItem.r131 = r131;
            tempItem.r132 = r132;
            tempItem.r133 = r133;
            tempItem.r134 = r134;
            tempItem.r135 = r135;
            tempItem.r136 = r136;
            tempItem.r137 = r137;
            tempItem.r138 = r138;
            tempItem.r139 = r139;
            tempItem.r140 = r140;
            tempItem.r141 = r141;
            tempItem.r142 = r142;
            tempItem.r143 = r143;
            tempItem.r144 = r144;
            tempItem.r145 = r145;
            tempItem.r146 = r146;
            tempItem.r147 = r147;
            tempItem.r148 = r148;
            tempItem.r149 = r149;
            tempItem.r150 = r150;
            tempItem.r151 = r151;
            tempItem.r152 = r152;
            tempItem.r153 = r153;
            tempItem.r154 = r154;
            tempItem.r155 = r155;
            tempItem.r156 = r156;
            tempItem.r157 = r157;
            tempItem.r158 = r158;
            tempItem.r159 = r159;
            tempItem.r160 = r160;
            tempItem.r161 = r161;
            tempItem.r162 = r162;
            tempItem.r163 = r163;
            tempItem.r164 = r164;
            tempItem.r165 = r165;
            tempItem.r166 = r166;
            tempItem.r167 = r167;
            tempItem.r168 = r168;
            tempItem.r169 = r169;
            tempItem.r170 = r170;
            tempItem.r171 = r171;
            tempItem.r172 = r172;
            tempItem.r173 = r173;
            tempItem.r174 = r174;
            tempItem.r175 = r175;
            tempItem.r176 = r176;
            tempItem.r177 = r177;
            tempItem.r178 = r178;
            tempItem.r179 = r179;
            tempItem.r180 = r180;
            tempItem.r181 = r181;
            tempItem.r182 = r182;
            tempItem.r183 = r183;
            tempItem.r184 = r184;
            tempItem.r185 = r185;
            tempItem.r186 = r186;

            itemDatabase_regions.Add(tempItem);
    }
} 
