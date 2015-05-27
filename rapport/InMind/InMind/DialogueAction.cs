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
    public enum COMMUNICATIVE_ACT { NO_COMM = 0, ASK_TOPIC, SUGGEST_TOPIC, SUGGEST_NEWS_ARTICLE, PRESENT_NEWS, ASK_FEEDBACK, COMM_ITEMS };
    public enum COMPUTATIONAL_ACT { NO_COMP = (COMMUNICATIVE_ACT.COMM_ITEMS + 1), RETRIEVE_EMAIL, DELETE_EMAIL, RETRIEVE_NEWS, COMP_ITEMS };

    class CommunicativeAct {
        private COMMUNICATIVE_ACT _commActType;
        private RAPPORT_STRATEGY _rapportStrategyType;

        public CommunicativeAct() { 
            _commActType = COMMUNICATIVE_ACT.NO_COMM;
            _rapportStrategyType = RAPPORT_STRATEGY.NONE;
        }

        public CommunicativeAct(COMMUNICATIVE_ACT type) { _commActType = type; }

        public CommunicativeAct(RAPPORT_STRATEGY type) { _rapportStrategyType = type; }

        public CommunicativeAct(COMMUNICATIVE_ACT actType, RAPPORT_STRATEGY rapportType) { 
            _commActType = actType;
            _rapportStrategyType = rapportType;
        }

        public CommunicativeAct(CommunicativeAct rhs) {
            _commActType = rhs._commActType;
            _rapportStrategyType = rhs._rapportStrategyType;
        }

        ~CommunicativeAct() { }

        public bool Equals(CommunicativeAct rhs)
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

            CommunicativeAct daObj = obj as CommunicativeAct;
            if (Object.ReferenceEquals(daObj, null))
            {
                return false;
            }
            else
            {
                return Equals(daObj);
            }
        }

        public static bool operator ==(CommunicativeAct lhs, CommunicativeAct rhs)
        {
                                                       //True if both are null
            return Object.ReferenceEquals(rhs, null) ? Object.ReferenceEquals(lhs, null) : (lhs._commActType == rhs._commActType && lhs._rapportStrategyType == rhs._rapportStrategyType);
        }

        public static bool operator !=(CommunicativeAct lhs, CommunicativeAct rhs)
        {
            return !(lhs == rhs);
        }

        public COMMUNICATIVE_ACT getCommActType() { return _commActType; }

        public RAPPORT_STRATEGY getRapportActType() { return _rapportStrategyType; }

        public void setCommActType(COMMUNICATIVE_ACT type) { _commActType = type; }

        public void setRapportActType(RAPPORT_STRATEGY type) { _rapportStrategyType = type; }
    }

    class ComputationalAct {
        private COMPUTATIONAL_ACT _type;

        public ComputationalAct() { _type = COMPUTATIONAL_ACT.NO_COMP; }

        public ComputationalAct(COMPUTATIONAL_ACT type) { _type = type; }

        public ComputationalAct(ComputationalAct rhs)
        {
            _type = rhs.getActType();
        }

        ~ComputationalAct() { }

        public bool Equals(ComputationalAct rhs)
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

            ComputationalAct daObj = obj as ComputationalAct;
            if (Object.ReferenceEquals(daObj, null))
            {
                return false;
            }
            return Equals(daObj);
        }

        public static bool operator ==(ComputationalAct lhs, ComputationalAct rhs){
                                                       //True if both are null
            return Object.ReferenceEquals(rhs, null) ? Object.ReferenceEquals(lhs, null) : lhs._type == rhs._type;
        }

        public static bool operator !=(ComputationalAct lhs, ComputationalAct rhs){
            return !(lhs == rhs);
        }

        public COMPUTATIONAL_ACT getActType() { return _type; }

        public void setActType(COMPUTATIONAL_ACT type) { _type = type; }
    }

    class DialogueAction
    {
        private CommunicativeAct _communicativeAct;
        private ComputationalAct _computationalAct;
        private bool _systemHasFloor;

        public DialogueAction() {
            _systemHasFloor = false;
        }

        public DialogueAction(CommunicativeAct commAct) { _communicativeAct = commAct; }

        public DialogueAction(ComputationalAct compAct) { _computationalAct = compAct; }

        public DialogueAction(CommunicativeAct commAct, ComputationalAct compAct) {
            _communicativeAct = commAct;
            _computationalAct = compAct;
        }

        public DialogueAction(bool systemHasFloor) {
            _systemHasFloor = systemHasFloor;
        }

        public bool Equals(DialogueAction rhs)
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

            DialogueAction daObj = obj as DialogueAction;
            if (Object.ReferenceEquals(daObj, null))
            {
                return false;
            }
            else
            {
                return Equals(daObj);
            }
        }

        public static bool operator ==(DialogueAction lhs, DialogueAction rhs){
                                                        //True if both are null
            return Object.ReferenceEquals(rhs, null) ? Object.ReferenceEquals(lhs, null) : (lhs._communicativeAct == rhs._communicativeAct) && (lhs._computationalAct == rhs._computationalAct) && 
                (lhs._systemHasFloor == rhs._systemHasFloor);
        }
        
        public static bool operator !=(DialogueAction lhs, DialogueAction rhs)
        {
            return !(lhs == rhs);
        }

        public bool getSystemFloor() { return _systemHasFloor; }

        public CommunicativeAct getCommunicativeAct() { return _communicativeAct; }

        public ComputationalAct getComputationalAct() { return _computationalAct; }
        
        public void setSystemFloor(bool systemHasFloor) { _systemHasFloor = systemHasFloor; }

        public void setVerbalAct(CommunicativeAct verbalAct) { _communicativeAct = verbalAct; }

        public void setNonVerbalAct(ComputationalAct nonVerbalAct) { _computationalAct = nonVerbalAct; }

        public static string DialogueActionToString(DialogueAction da)
        {
            return da.getCommunicativeAct().getCommActType().ToString() + " " + da.getCommunicativeAct().getRapportActType().ToString() + " " + da.getComputationalAct().getActType().ToString();
        }

        public static DialogueAction StringToDialogueAction(String[] args)
        {
            CommunicativeAct commAct = new CommunicativeAct();
            ComputationalAct compAct = new ComputationalAct();

            //Ignore first two args: vrDMRLTrain agentID
            for (int i = 2; i < args.Length; i++)
            {
                switch (args[i])
                {
                    //Communicative acts
                    case "PRESENT_NEWS": { commAct.setCommActType(COMMUNICATIVE_ACT.PRESENT_NEWS); break; }
                    //Rapport strategies (part of communicative acts)
                    case "INIT_SELF_DISC": { commAct.setRapportActType(RAPPORT_STRATEGY.INIT_SELF_DISC); break; }
                    case "REFER_SHARED_EXP": { commAct.setRapportActType(RAPPORT_STRATEGY.REFER_SHARED_EXP); break; }
                    case "INTIMATE_PERSONAL_INFO": { commAct.setRapportActType(RAPPORT_STRATEGY.INTIMATE_PERSONAL_INFO); break; }
                    case "RECIPROCAL_APPRECIATION": { commAct.setRapportActType(RAPPORT_STRATEGY.RECIPROCAL_APPRECIATION); break; }
                    case "VIOLATE_NORMS": { commAct.setRapportActType(RAPPORT_STRATEGY.VIOLATE_NORMS); break; }
                    case "RECIPROCATE_PREV_ACT": { commAct.setRapportActType(RAPPORT_STRATEGY.RECIPROCATE_PREV_ACT); break; }
                    case "ACKNOWLEDGE": { commAct.setRapportActType(RAPPORT_STRATEGY.ACKNOWLEDGE); break; }
                    case "PRAISE": { commAct.setRapportActType(RAPPORT_STRATEGY.PRAISE); break; }
                    case "NEGATIVE_SELF_DISC": { commAct.setRapportActType(RAPPORT_STRATEGY.NEGATIVE_SELF_DISC); break; }
                    case "EMB_LAUGHTER": { commAct.setRapportActType(RAPPORT_STRATEGY.EMB_LAUGHTER); break; }
                    //Computational Acts
                    case "RETRIEVE_EMAIL": { compAct.setActType(COMPUTATIONAL_ACT.RETRIEVE_EMAIL); break; }
                    case "DELETE_EMAIL": { compAct.setActType(COMPUTATIONAL_ACT.DELETE_EMAIL); break; }
                    case "RETRIEVE_NEWS": { compAct.setActType(COMPUTATIONAL_ACT.RETRIEVE_NEWS); break; }
                    default: { break; }
                }
            }

            return new DialogueAction(commAct, compAct);
        }
    }
}
