﻿using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {

	public GUISkin skin;
	public Texture2D healthyTexture, damagedTexture, criticalTexture;
	public Texture2D upArrow, downArrow, leftArrow, rightArrow;
	public Texture2D move, moveActive;
	public Texture2D attack, attackActive;
	public Texture2D defend, defendActive;
	public Texture2D cancel;

	private SoundManager soundManager;
	private Player activePlayer;
	private bool started = false;
	private GUIStyle selectedStyle = new GUIStyle();
	private GUIStyle targetStyle = new GUIStyle();
	private GUIStyle movementStyle = new GUIStyle();
	private GUIStyle buttonStyle = new GUIStyle();

	private bool moveButtonActive = false, attackButtonActive = false, defendButtonActive = false;
	
	void Start () {
		soundManager = FindObjectOfType(typeof(SoundManager)) as SoundManager;
		selectedStyle.alignment = TextAnchor.MiddleLeft;
		selectedStyle.fontSize = 30;
		selectedStyle.normal.textColor = Color.white;
		targetStyle.alignment = TextAnchor.MiddleRight;
		targetStyle.fontSize = 30;
		targetStyle.normal.textColor = Color.white;
	}

	void OnGUI() {
		if(!activePlayer || !started) return;
		GUI.skin = skin;
		GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
		Soldier currentSelection = activePlayer.GetSelectedSoldier();
		Soldier currentTarget = activePlayer.GetSelectedEnemy();
		if(currentSelection) {
			selectedStyle.normal.background = null;
			selectedStyle.padding.top = 0;
			GUI.Label(new Rect(10, 30, Screen.width / 2 - 10, 40), currentSelection.GetDisplayName(), selectedStyle);
		}
		if(currentTarget) {
			targetStyle.normal.background = null;
			targetStyle.padding.top = 0;
			GUI.Label(new Rect(Screen.width / 2, 30, Screen.width / 2 - 10, 40), currentTarget.GetDisplayName(), targetStyle);
		}
		float healthPercentage = 0.0f;
		if(currentSelection) {
			healthPercentage = currentSelection.GetHealthPercentage();
			if(healthPercentage > Resources.highSplit) selectedStyle.normal.background = healthyTexture;
			else if(healthPercentage > Resources.lowSplit) selectedStyle.normal.background = damagedTexture;
			else selectedStyle.normal.background = criticalTexture;
			selectedStyle.padding.top = -20;
			GUI.Label(new Rect(10, 10, 300 * healthPercentage, 20), "", selectedStyle);
		}
		if(currentTarget) {
			healthPercentage = currentTarget.GetHealthPercentage();
			if(healthPercentage > Resources.highSplit) targetStyle.normal.background = healthyTexture;
			else if(healthPercentage > Resources.lowSplit) targetStyle.normal.background = damagedTexture;
			else targetStyle.normal.background = criticalTexture;
			targetStyle.padding.top = -20;
			float targetHealthWidth = 300 * healthPercentage;
			GUI.Label(new Rect(Screen.width - targetHealthWidth - 10, 10, targetHealthWidth, 20), "", targetStyle);
		}
		if(currentSelection) {
			int padding = 20;
			int buttonWidth = 50;
			int topPos = 80;
			int leftPos = padding;
			buttonStyle.normal.background = moveButtonActive ? moveActive : move;
			if(GUI.Button(new Rect(leftPos, topPos, buttonWidth, buttonWidth), "", buttonStyle)) {
				if(soundManager) soundManager.PlaySound("ActionClick");
				if(moveButtonActive) {
					moveButtonActive = false;
					if(activePlayer) activePlayer.SetState(Player.State.None);
				} else {
					moveButtonActive = true;
					if(attackButtonActive) attackButtonActive = false;
					if(defendButtonActive) defendButtonActive = false;
					if(activePlayer) activePlayer.SetState(Player.State.Move);
				}
			}
			topPos += padding + buttonWidth;
			buttonStyle.normal.background = attackButtonActive ? attackActive : attack;
			if(GUI.Button(new Rect(leftPos, topPos, buttonWidth, buttonWidth), "", buttonStyle)) {
				if(soundManager) soundManager.PlaySound("ActionClick");
				if(attackButtonActive) {
					attackButtonActive = false;
					if(activePlayer) activePlayer.SetState(Player.State.None);
				} else {
					attackButtonActive = true;
					if(moveButtonActive) moveButtonActive = false;
					if(defendButtonActive) defendButtonActive = false;
					if(activePlayer) activePlayer.SetState(Player.State.Attack);
				}
			}
			topPos += padding + buttonWidth;
			buttonStyle.normal.background = defendButtonActive ? defendActive : defend;
			if(GUI.Button(new Rect(leftPos, topPos, buttonWidth, buttonWidth), "", buttonStyle)) {
				if(soundManager) soundManager.PlaySound("ActionClick");
				if(defendButtonActive) {
					defendButtonActive = false;
					if(activePlayer) activePlayer.SetState(Player.State.None);
				} else {
					defendButtonActive = true;
					if(moveButtonActive) moveButtonActive = false;
					if(attackButtonActive) attackButtonActive = false;
					if(activePlayer) activePlayer.SetState(Player.State.Defend);
				}
			}
			topPos += padding + buttonWidth;
			buttonStyle.normal.background = cancel;
			if(GUI.Button(new Rect(leftPos, topPos, buttonWidth, buttonWidth), "", buttonStyle)) {
				if(soundManager) soundManager.PlaySound("CancelClick");
				if(activePlayer) {
					activePlayer.DeselectSoldier();
					activePlayer.SetState(Player.State.None);
				}
			}
		} else {
			moveButtonActive = false;
			attackButtonActive = false;
			defendButtonActive = false;
			if(activePlayer) activePlayer.SetState(Player.State.None);
		}
		movementStyle.normal.background = upArrow;
		GUI.Box(Resources.topZone, "", movementStyle);
		movementStyle.normal.background = downArrow;
		GUI.Box(Resources.bottomZone, "", movementStyle);
		movementStyle.normal.background = leftArrow;
		GUI.Box(Resources.leftZone, "", movementStyle);
		movementStyle.normal.background = rightArrow;
		GUI.Box(Resources.rightZone, "", movementStyle);
		GUI.EndGroup();
	}

	public void SetActivePlayer(Player player) {
		activePlayer = player;
	}

	public void Begin() {
		started = true;
	}

	public void Pause() {
		started = false;
	}

	public void Finish() {
		started = false;
	}
}
