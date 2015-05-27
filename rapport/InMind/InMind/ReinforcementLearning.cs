/*
 *
 * Copyright (C) Carnegie Mellon University - All Rights Reserved.
 * Unauthorized copying of this file, via any medium is strictly prohibited.
 * This is proprietary and confidential.
 * Written by members of the ArticuLab, directed by Justine Cassell, 2014.
 * 
 * Author: Alexandros Papangelis, apapa@cs.cmu.edu
 * 
 *
 * Description: An implementation of various Reinforcement Learning algorithms.
 *
 * Q-Learning algorithm:
 * REF: Watkins, C.J.C.H. (1989). Learning from Delayed Rewards. PhD thesis, Cambridge University, Cambridge, England.
 * 
 * Q-Lambda algorithm
 * REF:
 * 
 * SARSA-Lambda algorithm:
 * REF:
 * 
 * Actor-Critic algorithm:
 * 
 * Incremental Actor-Critic algorithm:
 * 
 * Natural Actor-Critic algorithm:
 * 
 * Inverse RL algorithm: ? :
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace InMind
{
    abstract class RLearner<S,A>
    {
        protected double _alpha, _beta, _g, _eg, _lambda, _initAlpha, _initBeta, _initG, _initEg, _initLambda;
        protected int _NStates, _NActions;
        protected double ALPHA_DECAY { get; set; }
        protected double EG_DECAY { get; set; }
        protected S _s, _startState;
        protected A _a;
        protected SparseMatrix<S, A> _dialoguePolicy, _el;
        public delegate double rewardDelegate(S state, A action);
        protected rewardDelegate _reward;
        protected Random _random;
        protected List<S> _stateList;
        protected List<A> _actionList;
        protected List<double> _episodeRewards;
        protected double _currentTotalReward;

        public RLearner() {
            ALPHA_DECAY = 0.995;
            EG_DECAY = 0.995;
            _random = new Random();
        }
        
        ~RLearner() { }

        //Equals operator for type A
        bool Compare<A>(A x, A y) where A : class
        {
            return x == y;
        }

        public void setStartState(S startState) { _startState = startState; }

        //The user *MUST* set a reward function:
        // INPUT: int stateId, int actionId
        // OUTPUT: double
        protected void setRewardFunction(rewardDelegate func)
        {
            _reward = func;
        }

        public S getCurrentState() { return _s; }

        public double getReward(S state, A action)
        {
            return _reward(state, action);
        }

        public List<double> getRewards() { return _episodeRewards; }

        public double getEpisodeReward(int episode) { return _episodeRewards[episode]; }

        public void newEpisode()
        {
            _episodeRewards.Add(_currentTotalReward);
            _currentTotalReward = 0;
            _s = _startState;

            //Decay learning rate
            _alpha *= ALPHA_DECAY;
            //Decay exploration
            _eg *= EG_DECAY;
        }

        public void newBatch() { }

        public void reset() {
            _currentTotalReward = 0;
            _s = _startState;
            _alpha = _initAlpha;
            _beta = _initBeta;
            _g = _initG;
            _eg = _initEg;
            _lambda = _initLambda;
        }

        public abstract void update(S newState, A newAction);

        public abstract A nextAction();
    }

    //Q-Learning
    class QLearner<S, A> : RLearner<S, A>
    {
        public QLearner()
        {
            //default values
            _initAlpha = _alpha = 0.95;
            _initG = _g = 0.7;
            _initEg = _eg = 0.05;
            _initLambda = _lambda = 0.4;
            _NStates = _NActions = -1;
            _currentTotalReward = 0;
            _episodeRewards = new List<double>();
            _stateList = new List<S>();
            _actionList = new List<A>();
        }

        public QLearner(double alpha, double g, double eg)
        {
            _initAlpha = _alpha = alpha;
            _initG = _g = g;
            _initEg = _eg = eg;
            _NStates = _NActions = -1;
            _currentTotalReward = 0;
            _episodeRewards = new List<double>();
            _stateList = new List<S>();
            _actionList = new List<A>();
        }

        public QLearner(double alpha, double g, double eg, SparseMatrix<S, A> dialoguePolicy, rewardDelegate rewardFunction)
        {
            _initAlpha = _alpha = alpha;
            _initG = _g = g;
            _initEg = _eg = eg;
            _dialoguePolicy = dialoguePolicy;
            _NStates = _dialoguePolicy.getNStates();
            _NActions = _dialoguePolicy.getNActions();
            _stateList = _dialoguePolicy.getStateList();
            _actionList = _dialoguePolicy.getActionList();
            _currentTotalReward = 0;
            _episodeRewards = new List<double>();

            _reward = rewardFunction;

            if (_stateList.Count > 0) { _s = _stateList[0]; }
            if (_actionList.Count > 0) { _a = _actionList[0]; }
        }

        ~QLearner() { }

        public override A nextAction() {
            //Get next action
            return _random.NextDouble() > _eg ? _actionList[Utilities.maxInd(_dialoguePolicy[_s], _dialoguePolicy.getStateMaximum(_s), _NActions)] : _actionList[_random.Next() % _NActions];
        }

        //Updates the dialogue policy and also returns next action
        public override void update(S s_new, A a_new)
        {
            double r = _reward(s_new, a_new);

            //Update current total reward
            _currentTotalReward += r;

            double delta = r + _g * _dialoguePolicy[new Tuple<S, A>(s_new, a_new)] - _dialoguePolicy[new Tuple<S, A>(_s, _a)];

            foreach (S state in _stateList)
            {
                foreach (A action in _actionList)
                {
                    _dialoguePolicy[new Tuple<S,A>(state,action)] += _alpha * delta;
                }
            }

            _s = s_new;
            _a = a_new;
        }
    }

    //Q-Lambda algorithm
    class QLambdaLearner<S, A> : RLearner<S, A>
    {
        public QLambdaLearner()
        {
            //default values
            _initAlpha = _alpha = 0.95;
            _initG = _g = 0.7;
            _initEg = _eg = 0.05;
            _initLambda = _lambda = 0.4;
            _NStates = _NActions = -1;
            _currentTotalReward = 0;
            _episodeRewards = new List<double>();
            _stateList = new List<S>();
            _actionList = new List<A>();
        }

        public QLambdaLearner(double alpha, double g, double eg, double lambda)
        {
            _initAlpha = _alpha = alpha;
            _initG = _g = g;
            _initEg = _eg = eg;
            _initLambda = _lambda = lambda;
            _NStates = _NActions = -1;
            _currentTotalReward = 0;
            _episodeRewards = new List<double>();
            _stateList = new List<S>();
            _actionList = new List<A>();
        }

        public QLambdaLearner(double alpha, double g, double eg, double lambda, SparseMatrix<S, A> dialoguePolicy, rewardDelegate rewardFunction)
        {
            _initAlpha = _alpha = alpha;
            _initG = _g = g;
            _initEg = _eg = eg;
            _initLambda = _lambda = lambda;
            _currentTotalReward = 0;
            _dialoguePolicy = dialoguePolicy;
            _NStates = _dialoguePolicy.getNStates();
            _NActions = _dialoguePolicy.getNActions();
            _stateList = _dialoguePolicy.getStateList();
            _actionList = _dialoguePolicy.getActionList();
            _episodeRewards = new List<double>();

            _reward = rewardFunction;

            if (_stateList.Count > 0) { _s = _stateList[0]; }
            if (_actionList.Count > 0) { _a = _actionList[0]; }

            //Initialize eligibility traces
            _el = new SparseMatrix<S, A>();
            foreach (S state in _stateList)
            {
                foreach (A action in _actionList)
                {
                    _el[new Tuple<S, A>(state, action)] = 0;
                }
            }
        }

        ~QLambdaLearner() { }

        public override A nextAction()
        {
            //Get next action
            return _random.NextDouble() > _eg ? _actionList[Utilities.maxInd(_dialoguePolicy[_s], _dialoguePolicy.getStateMaximum(_s), _NActions)] : _actionList[_random.Next() % _NActions];
        }

        //Updates the dialogue policy and also returns next action
        public override void update(S s_new, A a_new){
            double r = _reward(s_new, a_new);

            //Update current total reward
            _currentTotalReward += r;

            A a_star = _actionList[Utilities.maxInd(_dialoguePolicy[s_new], _NActions)];

            if (_dialoguePolicy[new Tuple<S,A>(s_new, a_star)] == _dialoguePolicy[new Tuple<S,A>(s_new, a_new)])
            {
				a_star = a_new;
			}
			
			//Replacing traces
            _el[new Tuple<S, A>(_s, _a)] = 1;

            double delta = r + _g * _dialoguePolicy[new Tuple<S, A>(s_new, a_star)] - _dialoguePolicy[new Tuple<S, A>(_s, _a)];

            foreach (S state in _stateList)
            {
                foreach (A action in _actionList)
                {
                    _dialoguePolicy[new Tuple<S, A>(state, action)] += _alpha * delta * _el[new Tuple<S, A>(state, action)];
                }
            }

			if(a_new.Equals(a_star)){                        //TODO: WARNING! THIS MAY BE SLOW!! - Change Action representation? Compare references?
                foreach (S state in _stateList)
                {
                    foreach (A action in _actionList)
                    {
                        _el[new Tuple<S, A>(state, action)] *= _g * _lambda;
                    }
                }
			}
			else{
                foreach (S state in _stateList)
                {
                    foreach (A action in _actionList)
                    {
                        _el[new Tuple<S, A>(state, action)] = 0;
                    }
                }
			}
			
			_s = s_new;
			_a = a_new;
		}
    }

    //SARSA-Lambda algorithm
    class SARSALearner<S, A> : RLearner<S, A>
    {
        public SARSALearner()
        {
            //default values
            _initAlpha = _alpha = 0.95;
            _initG = _g = 0.7;
            _initEg = _eg = 0.05;
            _initLambda = _lambda = 0.4;
            _NStates = _NActions = -1;
            _currentTotalReward = 0;
            _episodeRewards = new List<double>();
            _stateList = new List<S>();
            _actionList = new List<A>();
        }

        public SARSALearner(double alpha, double g, double eg, double lambda)
        {
            _initAlpha = _alpha = alpha;
            _initG = _g = g;
            _initEg = _eg = eg;
            _initLambda = _lambda = lambda;
            _NStates = _NActions = -1;
            _currentTotalReward = 0;
            _episodeRewards = new List<double>();
            _stateList = new List<S>();
            _actionList = new List<A>();
        }

        public SARSALearner(double alpha, double g, double eg, double lambda, SparseMatrix<S, A> dialoguePolicy, rewardDelegate rewardFunction)
        {
            _initAlpha = _alpha = alpha;
            _initG = _g = g;
            _initEg = _eg = eg;
            _initLambda = _lambda = lambda;
            _currentTotalReward = 0;
            _dialoguePolicy = dialoguePolicy;
            _NStates = _dialoguePolicy.getNStates();
            _NActions = _dialoguePolicy.getNActions();
            _stateList = _dialoguePolicy.getStateList();
            _actionList = _dialoguePolicy.getActionList();
            _episodeRewards = new List<double>();

            _reward = rewardFunction;

            if (_stateList.Count > 0) { _s = _stateList[0]; }
            if (_actionList.Count > 0) { _a = _actionList[0]; }

            //Initialize eligibility traces
            _el = new SparseMatrix<S, A>();
            foreach (S state in _stateList)
            {
                foreach (A action in _actionList)
                {
                    _el[new Tuple<S, A>(state, action)] = 0;
                }
            }
        }

        ~SARSALearner() { }

        public override A nextAction()
        {
            //Get next action
            return _random.NextDouble() > _eg ? _actionList[Utilities.maxInd(_dialoguePolicy[_s], _dialoguePolicy.getStateMaximum(_s), _NActions)] : _actionList[_random.Next() % _NActions];
        }

        //Updates the dialogue policy and also returns next action
        public override void update(S s_new, A a_new){
            double r = _reward(s_new, a_new);

            //Update current total reward
            _currentTotalReward += r;
			
			//Replacing traces
            _el[new Tuple<S, A>(_s, _a)] = 1;
			
            double delta = r + _g * _dialoguePolicy[new Tuple<S, A>(s_new, a_new)] - _dialoguePolicy[new Tuple<S, A>(_s, _a)];

            foreach (S state in _stateList)
            {
                foreach (A action in _actionList)
                {
                    _dialoguePolicy[new Tuple<S, A>(state, action)] += _alpha * delta * _el[new Tuple<S, A>(state, action)];
                }
            }

            foreach (S state in _stateList)
            {
                foreach (A action in _actionList)
                {
                    _el[new Tuple<S, A>(state, action)] *= _g * _lambda;
                }
            }
			
			_s = s_new;
			_a = a_new;
		}
    }
}
