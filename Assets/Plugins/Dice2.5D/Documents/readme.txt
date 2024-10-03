//------------------------------------
// English
//------------------------------------
[class] DiceRoll
  a class of Dice2.5 module(default).
  using sprite with animation sheet.

public void Roll();
  Start to roll this dice.
  
public void SetPip(int _pip)
  Set a pip of dice.
  _pip : 1 to 6(6-sided die) or 10(10-sided die).


//------------------------------------
[class] DiceRoll3DTex
  a class of Dice2.5 module(new).
  using 3Dtexture and dedicated Shader.
  This class can apply a blur effect to the dice.

public void Roll();
  Start to roll this dice.
  
public void SetPip(int _pip)
  Set a pip of dice.
  _pip : 1 to 6(6-sided die).

--- or ---
1. Change a parameter [m_status] to Roll to start roll.
2. Set a parameter [int m_pip] between 1 and 6.
3. Change a parameter [m_status] to Stop to stop and fix a pip.
//------------------------------------

//------------------------------------
// 日本語
//------------------------------------
[class] DiceRoll
  ダイス2.5のクラスです(default).
  アニメーションとテクスチャシートを使用しています.

public void Roll();
  ダイス回転を開始します.
  
public void SetPip(int _pip)
  指定した目でダイスを止めます.
  _pip : 1 から 6(6面ダイス) or 10(10面ダイス).


//------------------------------------
[class] DiceRoll3DTex
  ダイス2.5のクラスです(new).
  3Dtexture と専用シェーダを使用しています.
  ダイスにぶらーをかけることができます.

public void Roll();
  ダイス回転を開始します.
  
public void SetPip(int _pip)
  指定した目でダイスを止めます.
  _pip : 1 から 6(6面ダイス).

--- または ---
1. [m_status] を "Roll" にするとダイスが回転を始めます.
2. [int m_pip] に1 から 6の数字をセットします.
3. [m_status] を"Stop" にするとセットした目でダイスが止まります.
//------------------------------------
