using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item_book
{
    public string logout;
    public string deleteAccount;
    public string terms;
    public string avatarTitle;
    public string signUpError;
    public string optionsTitle;
    public string profileTitle;
    public string nicknameTitle;
    public string nicknameHolder;
    public string regionTitle;
    public string birthdateTitle;
    public string boardTitle;
    public string checkerTitle;
    public string nicknameEmptyError;
    public string nicknameCharError;
    public string nicknameEnglishError;
    public string premiumPrice;
    public string premiumInfo1;
    public string premiumInfo2;
    public string premiumTitle;
    public string premiumReward1;
    public string premiumReward2;
    public string premiumReward3;
    public string socketAddress;
    public string turnTimeC;
    public string turnTimeB;
    public string turnTimeA;
    public string doubleTime;
    public string doubleRequest;
    public string doubleResponse;
    public string resignText;
    public string classC;
    public string classB;
    public string classA;
    public string startTournament;
    public string tournamentPremiumInfo;
    public string tournamentInviteFriendsInfo;
    public string tournamentCompleteDone;
    public string shareCode;
    public string regionButton;
    public string stateButton;

    public Item_book(Item_book d)
    {
        logout = d.logout;
        deleteAccount = d.deleteAccount;
        terms = d.terms;
        avatarTitle = d.avatarTitle;
        signUpError = d.signUpError;
        optionsTitle = d.optionsTitle;
        profileTitle = d.profileTitle;
        nicknameTitle = d.nicknameTitle;
        nicknameHolder = d.nicknameHolder;
        regionTitle = d.regionTitle;
        birthdateTitle = d.birthdateTitle;
        boardTitle = d.boardTitle;
        checkerTitle = d.checkerTitle;
        nicknameEmptyError = d.nicknameEmptyError;
        nicknameCharError = d.nicknameCharError;
        nicknameEnglishError = d.nicknameEnglishError;
        premiumPrice = d.premiumPrice;
        premiumInfo1 = d.premiumInfo1;
        premiumInfo2 = d.premiumInfo2;
        premiumTitle = d.premiumTitle;
        premiumReward1 = d.premiumReward1;
        premiumReward2 = d.premiumReward2;
        premiumReward3 = d.premiumReward3;
        socketAddress = d.socketAddress;
        turnTimeC = d.turnTimeC;
        turnTimeB = d.turnTimeB;
        turnTimeA = d.turnTimeA;
        doubleTime = d.doubleTime;
        doubleRequest = d.doubleRequest;
        doubleResponse = d.doubleResponse;
        resignText = d.resignText;
        classC = d.classC;
        classB = d.classB;
        classA = d.classA;
        startTournament = d.startTournament;
        tournamentPremiumInfo = d.tournamentPremiumInfo;
        tournamentInviteFriendsInfo = d.tournamentInviteFriendsInfo;
        tournamentCompleteDone = d.tournamentCompleteDone;
        shareCode = d.shareCode;
        regionButton = d.regionButton;
        stateButton = d.stateButton;
    }
}


[System.Serializable]
public class Item_regions
{
    public string r1,r2,r3,r4,r5,r6,r7,r8,r9,r10,r11,r12,r13,r14,r15,r16,r17,r18,r19,r20,r21,r22,r23,r24,r25,r26,r27,r28,r29,r30,r31,r32,r33,r34,r35
        ,r36,r37,r38,r39,r40,r41,r42,r43,r44,r45,r46,r47,r48,r49,r50,r51,r52,r53,r54,r55,r56,r57,r58,r59,r60,r61,r62,r63,r64,r65,r66,r67,r68,r69,r70
        ,r71,r72,r73,r74,r75,r76,r77,r78,r79,r80,r81,r82,r83,r84,r85,r86,r87,r88,r89,r90,r91,r92,r93,r94,r95,r96,r97,r98,r99,r100,r101,r102,r103,r104
        ,r105,r106,r107,r108,r109,r110,r111,r112,r113,r114,r115,r116,r117,r118,r119,r120,r121,r122,r123,r124,r125,r126,r127,r128,r129,r130,r131,r132,r133,r134
        ,r135,r136,r137,r138,r139,r140,r141,r142,r143,r144,r145,r146,r147,r148,r149,r150,r151,r152,r153,r154,r155,r156,r157,r158,r159,r160,r161,r162
        ,r163,r164,r165,r166,r167,r168,r169,r170,r171,r172,r173,r174,r175,r176,r177,r178,r179,r180,r181,r182,r183,r184,r185,r186;

    public Item_regions(Item_regions d)
    {
        var fields = this.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (var field in fields)
        {
            field.SetValue(this, field.GetValue(d));
        }
    }
}
