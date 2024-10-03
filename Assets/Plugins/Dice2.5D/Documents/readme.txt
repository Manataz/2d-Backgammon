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
// ���{��
//------------------------------------
[class] DiceRoll
  �_�C�X2.5�̃N���X�ł�(default).
  �A�j���[�V�����ƃe�N�X�`���V�[�g���g�p���Ă��܂�.

public void Roll();
  �_�C�X��]���J�n���܂�.
  
public void SetPip(int _pip)
  �w�肵���ڂŃ_�C�X���~�߂܂�.
  _pip : 1 ���� 6(6�ʃ_�C�X) or 10(10�ʃ_�C�X).


//------------------------------------
[class] DiceRoll3DTex
  �_�C�X2.5�̃N���X�ł�(new).
  3Dtexture �Ɛ�p�V�F�[�_���g�p���Ă��܂�.
  �_�C�X�ɂԂ�[�������邱�Ƃ��ł��܂�.

public void Roll();
  �_�C�X��]���J�n���܂�.
  
public void SetPip(int _pip)
  �w�肵���ڂŃ_�C�X���~�߂܂�.
  _pip : 1 ���� 6(6�ʃ_�C�X).

--- �܂��� ---
1. [m_status] �� "Roll" �ɂ���ƃ_�C�X����]���n�߂܂�.
2. [int m_pip] ��1 ���� 6�̐������Z�b�g���܂�.
3. [m_status] ��"Stop" �ɂ���ƃZ�b�g�����ڂŃ_�C�X���~�܂�܂�.
//------------------------------------
