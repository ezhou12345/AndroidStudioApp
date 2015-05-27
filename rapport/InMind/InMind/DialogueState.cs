/*
 *
 * Copyright (C) Carnegie Mellon University - All Rights Reserved.
 * Unauthorized copying of this file, via any medium is strictly prohibited.
 * This is proprietary and confidential.
 * Written by members of the ArticuLab, directed by Justine Cassell, 2014.
 * 
 * Author: Alexandros Papangelis, apapa@cs.cmu.edu
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMind
{
    public enum TASK_GOAL { NONE = 0, RETRIEVE_EMAIL, REPLY_TO_EMAIL, DELETE_EMAIL, RETRIEVE_NEWS, TG_ITEMS };
    public enum SOCIAL_GOAL { NONE = 0, ENHANCE_RAPPORT, MAINTAIN_RAPPORT, DESTROY_RAPPORT, SG_ITEMS };
    public enum RAPPORT_STATUS { NOINFO = 0, LOW, MED, HIGH, RS_ITEMS};

    public class DialogueState
    {
        private int _dialogueTurn, _otherAgentID;
        private string _currentUtterance, _previousUtterance;
        private double _currentASRConfidence, _previousASRConfidence, _currentEmotionConfidence, _previousEmotionConfidence, _currentToneConfidence, _previousToneConfidence;
        private TASK_GOAL _taskGoal, _userTaskGoal;    //TODO: Make lists of task and social goals
        private SOCIAL_GOAL _socialGoal, _userSocialGoal;
        private RAPPORT_STRATEGY_DEMO _userRapportStrategy, _previousSystemRapportStrategy;
        private RAPPORT_STATUS _traitRapport, _stateRapport;
        private bool _friends;

        public DialogueState() {
            _dialogueTurn = 0;
            _currentASRConfidence = _previousASRConfidence = _currentEmotionConfidence = _previousEmotionConfidence = _currentToneConfidence = _previousToneConfidence = 0;
            _currentUtterance = "";
            _previousUtterance = "";
            _otherAgentID = -1;
            _userRapportStrategy = RAPPORT_STRATEGY_DEMO.NONE;
            _previousSystemRapportStrategy = RAPPORT_STRATEGY_DEMO.NONE;
            _traitRapport = RAPPORT_STATUS.NOINFO;
            _stateRapport = RAPPORT_STATUS.NOINFO;
        }

        public DialogueState(DialogueState rhs) {
            _dialogueTurn = rhs._dialogueTurn;
            _currentUtterance = String.Copy(rhs._currentUtterance);
            _previousUtterance = String.Copy(rhs._previousUtterance);
            _currentASRConfidence = rhs.CurrentASRConfidence;
            _currentEmotionConfidence = rhs.CurrentEmotionConfidence;
            _currentToneConfidence = rhs.CurrentToneConfidence;
            _previousASRConfidence = rhs.PreviousASRConfidence;
            _previousEmotionConfidence = rhs.PreviousEmotionConfidence;
            _previousToneConfidence = rhs.PreviousToneConfidence;
            _taskGoal = rhs._taskGoal;
            _socialGoal = rhs._socialGoal;
            _otherAgentID = rhs.OtherAgentID;
            _userRapportStrategy = rhs._userRapportStrategy;
            _previousSystemRapportStrategy = rhs._previousSystemRapportStrategy;
            _traitRapport = rhs._traitRapport;
            _stateRapport = rhs._stateRapport;
            _friends = false;
        }

        ~DialogueState() { }

        public int DialogueTurn
        {
            get { return _dialogueTurn; }
            set { _dialogueTurn = value; }
        }

        public int OtherAgentID
        {
            get { return _otherAgentID; }
            set { _otherAgentID = value; }
        }

        public string PreviousUtterance
        {
            get { return _previousUtterance; }
            set { _previousUtterance = value; }
        }

        public string CurrentUtterance
        {
            get { return _currentUtterance; }
            set { _currentUtterance = value; }
        }

        public double PreviousToneConfidence
        {
            get { return _previousToneConfidence; }
            set { _previousToneConfidence = value; }
        }

        public double CurrentToneConfidence
        {
            get { return _currentToneConfidence; }
            set { _currentToneConfidence = value; }
        }

        public double PreviousEmotionConfidence
        {
            get { return _previousEmotionConfidence; }
            set { _previousEmotionConfidence = value; }
        }

        public double CurrentEmotionConfidence
        {
            get { return _currentEmotionConfidence; }
            set { _currentEmotionConfidence = value; }
        }

        public double PreviousASRConfidence
        {
            get { return _previousASRConfidence; }
            set { _previousASRConfidence = value; }
        }

        public double CurrentASRConfidence
        {
            get { return _currentASRConfidence; }
            set { _currentASRConfidence = value; }
        }

        public TASK_GOAL TaskGoal
        {
            get { return _taskGoal; }
            set { _taskGoal = value; }
        }

        public SOCIAL_GOAL SocialGoal
        {
            get { return _socialGoal; }
            set { _socialGoal = value; }
        }

        public RAPPORT_STRATEGY_DEMO UserRapportStrategy
        {
            get { return _userRapportStrategy; }
            set { _userRapportStrategy = value; }
        }

        public RAPPORT_STRATEGY_DEMO PreviousSystemRapportStrategy
        {
            get { return _previousSystemRapportStrategy; }
            set { _previousSystemRapportStrategy = value; }
        }

        public RAPPORT_STATUS TraitRapport
        {
            get { return _traitRapport; }
            set { _traitRapport = value; }
        }

        public RAPPORT_STATUS StateRapport
        {
            get { return _stateRapport; }
            set { _stateRapport = value; }
        }

        public bool Friends
        {
            get { return _friends; }
            set { _friends = value; }
        }

        public bool Equals(DialogueState rhs)
        {
            if (Object.ReferenceEquals(rhs, null))
            {
                return false;
            }
            if (Object.ReferenceEquals(rhs, this))
            {
                return true;
            }
            return this == rhs;
        }

        public override bool Equals(Object obj)
        {
            if (Object.ReferenceEquals(obj, null))
            {
                return false;
            }

            DialogueState dsObj = obj as DialogueState;
            if (Object.ReferenceEquals(dsObj, null))
            {
                return false;
            }
            return Equals(dsObj);
        }

        public static bool operator==(DialogueState lhs, DialogueState rhs){
            return Object.ReferenceEquals(rhs, null) ? false : (lhs._currentASRConfidence == rhs._currentASRConfidence) && (lhs._currentEmotionConfidence == rhs._currentEmotionConfidence) && (lhs._currentToneConfidence == rhs._currentToneConfidence) &&
                (lhs._currentUtterance.Equals(rhs._currentUtterance)) && (lhs._previousASRConfidence == rhs._previousASRConfidence) && (lhs._previousEmotionConfidence == rhs._previousEmotionConfidence) &&
                (lhs._previousToneConfidence == rhs._previousToneConfidence) && (lhs._previousUtterance.Equals(rhs._previousUtterance)) && (lhs._dialogueTurn == rhs._dialogueTurn) &&
                (lhs._socialGoal == rhs._socialGoal) && (lhs._taskGoal == rhs._taskGoal) && (lhs._friends == rhs._friends) && (lhs._otherAgentID == rhs._otherAgentID) &&
                (lhs._userRapportStrategy == rhs._userRapportStrategy) && (lhs._previousSystemRapportStrategy == rhs._previousSystemRapportStrategy) && (lhs.TraitRapport == rhs.TraitRapport) && (lhs._stateRapport == rhs._stateRapport);
        }

        public static bool operator !=(DialogueState lhs, DialogueState rhs){
            return !(lhs == rhs);
        }

        public void extractFeatures() { }
    }

    public class MacroDialogueState : DialogueState {
        protected List<DialogueState> _states;

        public MacroDialogueState() {
            _states = new List<DialogueState>();
        }

        public MacroDialogueState(List<DialogueState> states) {
            _states = new List<DialogueState>(states);
        }

        ~MacroDialogueState() {
            _states.Clear();
        }

        public List<DialogueState> States {
            get { return _states; }
        }
    }
}
