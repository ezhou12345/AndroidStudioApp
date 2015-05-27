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
    class MarkovDecisionProcess
    {
        private int _NStates { get; set; }
        private int _NActions { get; set; }

        public MarkovDecisionProcess() { }

        public MarkovDecisionProcess(int NStates, int NActions)
        {
            _NStates = NStates;
            _NActions = NActions;
        }

        ~MarkovDecisionProcess() { }
    }
    
    class FSM : MarkovDecisionProcess { }

    class MDP : MarkovDecisionProcess {
        private SparseMatrix<DialogueState, DialogueAction> _policy;

        public MDP() {
            _policy = new SparseMatrix<DialogueState, DialogueAction>();
        }

        ~MDP() { }
    }

    class SMDP : MarkovDecisionProcess {
        private SparseMatrix<MacroDialogueState, MDP> _macroPolicy;

        public SMDP() {
            _macroPolicy = new SparseMatrix<MacroDialogueState, MDP>();
        }

        ~SMDP() { }
    }

    class POMDP : MarkovDecisionProcess {
        private SparseMatrix<SparseMatrix<DialogueState, double>, DialogueAction> _policy;
    }

    class HPOMDP : POMDP {
        private HPOMDP _parent;

        public HPOMDP() { }
        ~HPOMDP() { }
    }

    class IPOMDP : POMDP { }
}


class BayesianNetwork { }

class DynamicBayesianNetwork : BayesianNetwork { }

class HiddenMarkovModel { }

class InfiniteHiddenMarkovModel { }

class GaussianMarkovModel { }

/**
 * POSMDP.h
 *
 * University of Texas at Arlington
 *             and
 * National Center for Scientific Research, "Demokritos"
 *
 *
 * Created on: Feb 25, 2013
 *
 * Authors:
 * Alexandros Papangelis
 *
 * 
 *
 */
/*

class MicroPolicy : public Policy<Action>{
	private:
		double **_policy;
		int _NStates, _NActions;
	
	public:
		MicroPolicy(){
			_NStates = _NActions = 0;
			_policy = NULL;
		}

		MicroPolicy(int NStates, int NActions){
			_NStates = NStates;
			_NActions = NActions;
			
			_policy = new double*[_NStates];
			for(int s = 0; s < _NStates; s++){
				_policy[s] = new double[_NActions];
				
				for(int a = 0; a < _NActions; a++){
					_policy[s][a] = 0;
				}
			}
		}

		~MicroPolicy(){}

		int getNStates() const {return _NStates;}
		int getNActions() const {return _NActions;}
		double** getPolicy() const {return _policy;}

		MicroPolicy& operator=(MicroPolicy& rhs){
			_NStates = rhs.getNStates();
			_NActions = rhs.getNActions();
			
			double** rhsPolicy = rhs.getPolicy();
			
			_policy = new double*[_NStates];
			for(int s = 0; s < _NStates; s++){
				_policy[s] = new double[_NActions];
				
				for(int a = 0; a < _NActions; a++){
					_policy[s][a] = rhsPolicy[s][a];
				}
			}
			
			return *this;
		}

		//Return the next action			//TODO: use Action instead of int?
		int nextAction(int s){
			//Return
			return maxInd(_policy[s], _NActions);
		}
		
		MicroPolicy& operator=(const MicroPolicy& rhs){
			_NStates = rhs.getNStates();
			_NActions = rhs.getNActions();
			
			_policy = new double*[_NStates];
			
			for(int i = 0; i < _NActions; i++){
				_policy[i] = new double[_NActions];
				
				for(int j = 0; j < _NActions; j++){
					_policy[i][j] = rhs.getPolicy()[i][j];
				}
			}
			
			return *this;
		}
};

class MacroState : public State {
	protected:
		//Linked list with void* objects, where each can be anything the user needs
		myList<State> _states;
		
	public:
		MacroState(){}		
		
		MacroState(myList<State> states){
			_states.append(&states);
		}
		
		MacroState(State s){
			
		}
		
		~MacroState(){}
		
		myList<State> getStates(){return _states;}
		
		void addState(State *s){
			_states.add(s);
		}
};

//A special case of macro state
//State S (s1)----(s2)-- ... -- (sN)	(si: values of S)
//			|		|			  |
//		 [b(s1)] [b(s2)]	   [b(sN)]
class BeliefState : public MacroState{
	private:
		double _belief;
		
	protected:
		void setBelief(int belief){
			_belief = (0 <= belief && belief <= 1) ? belief : _belief;
		}
	
	public:
		BeliefState(){_belief = 0;}
		BeliefState(int resolution){
			_belief = 0;
			for(int i = 0; i < resolution; i++){
				_states.add(new MacroState());	//The object of each list item is the value of the belief state b(s)
			}
		}
		~BeliefState(){}
};

class Option : public Action {
	private:
		MacroState I;
		double *_beta;
		int _size;
		MicroPolicy _pi;
		
	public:
		Option(){
			_size = 0;
		}
		
		Option(int size){
			if(size > 0){
				_size = size;			
				
				_beta = new double[_size];
				
				for(int i = 0; i < _size; i++){
					_beta[i] = (double) 1 / _size;
				}
			}
		}
		
		~Option(){
			//delete _beta;
			//delete[] _policy;
		}
	
		int getSize(){return _size;}
		MacroState getI(){return I;}
		double* getBetaDistribution(){return _beta;}
		MicroPolicy getMicroPolicy(){return _pi;}
		
		int getNextAction(int s){
			return _pi.nextAction(s);
		}
		
		void setMicroPolicy(MicroPolicy pi){
			_pi = pi;
		}
		
		double beta(int s){
			if(0 > s || s > _size){return -1;}
			
			return _beta[s];
		}
};

//Policy over macro actions, which are represented as integers
class MacroPolicy : public Policy<int>{
	private:
		double **_policy;
		int _NStates, _NActions;
		
	public:
		MacroPolicy(){
			_NStates = _NActions = 0;
		}
		
		MacroPolicy(int NStates, int NActions){
			_NStates = NStates;
			_NActions = NActions;
			_policy = new double*[_NStates];
			
			for(int s = 0; s < _NStates; s++){
				_policy[s] = new double[_NActions];
				
				for(int a = 0; a < _NActions; a++){
					_policy[s][a] = 0;
				}
			}
		}

		~MacroPolicy(){}

		double** getPolicy() const {
			return _policy;
		}
		
		int getNStates() const {
			return _NStates;
		}
		
		int getNActions() const {
			return _NActions;
		}

		void setPolicy(double** pol, int NStates, int NActions){
			_NStates = NStates;
			_NActions = NActions;
			
			for(int s = 0; s < _NStates; s++){
				for(int a = 0; a < _NActions; a++){
					_policy[s][a] = pol[s][a];
					std::cout<<"Policy["<<s<<"]["<<a<<"]: "<<_policy[s][a]<<" ";
				}std::cout<<"\n";
			}
		}

		//Return next option index			//TODO: use Option instead of int?
		int nextOption(int s){
			//Return Option
			return maxInd(_policy[s], _NActions);
		}
};

class POSMDP {
	protected:
		//std::list<Option> Options;
		//std::list<MacroState> MacroStates;
		std::map<int, State*> MacroStates;
		std::map<int, Action*> Options;
		double ***TransitionProbability, ***ObservationProbability;
		MacroPolicy mu;
		int _NStates, _NActions, _NObservations, _TStates, _prevAction, _prevObservation, _currentStateId;
		
		//Update matrices to account for changes in the _NStates
		void updateTrObProbs(){													//TODO: Must be dynamic wrt actions as well
			// |_TStates - _NStates| is in {0, 1}

			if(_TStates < _NStates && _NActions > 0){	//A new state has been added
				
				if(_NObservations > 0){ std::cout<<"In updateTrObProbs!\n";
					//Copy old matrix
					double ***tmpObs = new double**[_NObservations];
					
					for(int o = 0; o < _NObservations; o ++){
						tmpObs[o] = new double*[_NStates];
						for(int s = 0; s < _NStates; s++){
							tmpObs[o][s] = new double[_NActions];
						}
					}
					
					for(int s = 0; s < _NStates; s++){
						if(s < _TStates){
							for(int a = 0; a < _NActions; a++){
								for(int o = 0; o < _NObservations; o ++){
									tmpObs[o][s][a] = ObservationProbability[o][s][a];
								}
							}
						}
					}
					
					ObservationProbability = tmpObs;
					
					//Divide existing probabilities by _TStates/(_NStates * _NActions) and assign 1/(_NStates * _NActions) in the new dimensions
					for(int i = 0; i < _TStates; i++){
						for(int j = 0; j < _NActions; j++){
							for(int o = 0; o < _NObservations; o++){
								ObservationProbability[o][i][j] *= (double) _TStates / _NObservations;	//TODO: Maybe we need a different update? E.g. new entries are 0
							}
						}
					}
					
					for(int a = 0; a < _NActions; a++){	
						for(int o = 0; o < _NObservations; o++){
							ObservationProbability[o][_TStates][a] = (double) 1 / _NObservations;	//TODO: Maybe we need a different update? E.g. new entries are 0
						}
					}
				}
				
				//Copy old matrix
				double ***tmpTrans = new double**[_NStates];
				
				for(int s = 0; s < _NStates; s++){
					tmpTrans[s] = new double*[_NStates];
					for(int ss = 0; ss < _NStates; ss++){
						tmpTrans[s][ss] = new double[_NActions];
					}
				}				

				for(int s = 0; s < _NStates; s++){
					if(s < _TStates){
						for(int a = 0; a < _NActions; a++){
							for(int ss = 0; ss < _NStates; ss++){
								if(ss < _TStates){
									tmpTrans[s][ss][a] = TransitionProbability[s][ss][a];
								}
							}
						}
					}
				}
					
				TransitionProbability = tmpTrans;
				
				//Divide existing probabilities by _TStates/(_NStates * _NActions) and assign 1/(_NStates * _NActions) in the new dimensions
				for(int i = 0; i < _TStates; i++){
					for(int j = 0; j < _NActions; j++){
						for(int s = 0; s < _TStates; s++){
							TransitionProbability[s][i][j] *= (double) _TStates / _NStates;
						}
					}
				}
				
				for(int a = 0; a < _NActions; a++){
					for(int s = 0; s < _NStates; s++){
						TransitionProbability[s][_TStates][a] = (double) 1 / _NStates;
						TransitionProbability[_TStates][s][a] = (double) 1 / _NStates;
					}
				}
				
				_TStates++;
			}
			else if(_TStates > _NStates){	//A state has been deleted
				std::cout<<"State Deleted!\n";
			}
		}
		
	public:
		POSMDP(){
			_NStates = _NActions = _TStates = 0;
			TransitionProbability = ObservationProbability = NULL;
			srand(time(NULL));
		}
		
		POSMDP(int NStates, int NActions){
			_NStates = _TStates = NStates;
			_NActions = NActions;
			_NObservations = 10;	//TODO: Change this
			srand(time(NULL));
			
			//Add states and initialize Transition Probability matrix
			ObservationProbability = new double**[_NObservations], TransitionProbability = new double**[_NStates];
				
			for(int o = 0; o < _NObservations; o ++){
				ObservationProbability[o] = new double*[_NStates];
				
				for(int s = 0; s < _NStates; s++){
					ObservationProbability[o][s] = new double[_NActions];
					
					for(int a = 0; a < _NActions; a++){
						ObservationProbability[o][s][a] = (double) 1 / _NObservations;
					}
				}
			}
				
			for(int s = 0; s < _NStates; s++){
				TransitionProbability[s] = new double*[_NStates];
				
				for(int ss = 0; ss < _NStates; ss++){
					TransitionProbability[s][ss] = new double[_NActions];
					
					for(int a = 0; a < _NActions; a++){
						TransitionProbability[s][ss][a] = (double) 1 / _NStates;
					}
				}
			}	

			//Add states
			for(int i = 0; i < _NStates; i++){
				MacroState m;
				//MacroStates.push_front(m);
				MacroStates[i] = &m;
			}
			
			//Add options
			for(int i = 0; i < _NActions; i++){
				Option o;
				//Options.push_front(o);
				Options[i] = &o;
			}
			
			//Initialize Policy
			mu = MacroPolicy(NStates, NActions);
		}
		
		~POSMDP(){}
		
		//Reset pobability tables
		void reset(){
			if(TransitionProbability != NULL){
				for(int s = 0; s < _NStates; s++){
					for(int ss = 0; ss < _NStates; ss++){
						for(int a = 0; a < _NActions; a++){
							TransitionProbability[s][ss][a] = (double) 1 / _NStates;
						}
					}
				}
			}
			
			if(ObservationProbability != NULL){
				for(int o = 0; o < _NObservations; o++){
					for(int s = 0; s < _NStates; s++){
						for(int a = 0; a < _NActions; a++){
							TransitionProbability[o][s][a] = (double) 1 / _NObservations;
						}
					}
				}
			}
			
			//Reset starting state
			_currentStateId = 0; //MacroStates;
		}
		
		MacroPolicy* getMacroPolicy(){
			return &mu;
		}
		
		MacroState* getStateAt(int pos){
			//return MacroStates.front();	//TODO: Write the actual code
			return (MacroState*)MacroStates[pos];
		}
		
		MacroState* getCurrentState(){
			//return new MacroState;	//TODO: Write the actual code
			return (MacroState*)MacroStates[_currentStateId];
		}
		
		//Return state with highest belief?	//TODO: Do we need both?
		State* currentState(){
			return MacroStates[_currentStateId];
		}
		
		int getCurrentStateId(){
			return _currentStateId;
		}
		
		//std::list<MacroState> getMacroStates(){
		std::map<int, State*> getMacroStates(){
			return MacroStates;
		}
		
		void setNStates(int NStates){
			_NStates = NStates;	//TODO: add corresponding stats?
			updateTrObProbs();
		}
		
		void setNActions(int NActions){	//TODO: Must be dynamic!
			_NActions = NActions;
		}
		
		void setMacroPolicy(MacroPolicy m){
			mu = m;
		}
		
		void setTransitionProbability(double ***TransProb){
			TransitionProbability = TransProb;
		}
		
		void addStartState(State* ms){	//TODO: ObservationProbability also needs to be dynamic!
			//MacroStates.push_front(ms);
			//_NStates++;
			MacroStates[_NStates++] = ms;
			
			_currentStateId = _NStates-1;
			
			//Update Probability Matrices
			updateTrObProbs();
		}
		
		void addState(State* ms){	//TODO: ObservationProbability also needs to be dynamic!
			//MacroStates.push_front(ms);
			//_NStates++;
			MacroStates[_NStates++] = ms;
			
			//Update Probability Matrices
			updateTrObProbs();
		}	
		void addOption(Option* o){
			//Options.push_front(o);
			//_NActions++;
			Options[_NActions++] = o;
			//updateTrObProbs();
		}
		
		double Reward(){return 0;}
		
		//Transition from _currentState to another, by taking action ActionId, according to TransitionProbability
		void transition(int actionId){
			double r = (double) rand() / RAND_MAX, sum = 0;
			
			for(int s = 0; s < _NStates; s++){
				sum += TransitionProbability[_currentStateId][s][actionId];

				if(r <= sum){
					_currentStateId = s;
					return;
				}
			}
		}
		
		void transition(Action act){}
		
		int getNextOption(){
			return mu.nextOption(_currentStateId);
		}
		
		int getNStates(){
			return _NStates;
		}
		
		int getNActions(){
			return _NActions;
		}
		
		//Return the Value Function V, for the current policy mu
		double V(){
			return 0.0;
		}
		
		//Return the Value Function V, for some policy nu
		double V(MacroPolicy nu){
			return 0.0;
		}
		
		//Return the Expected Reward J, for the current policy mu
		double J(){
			return 0.0;
		}
		
		//Return the Expected Reward J, for some policy nu
		double J(MacroPolicy nu){
			return 0.0;
		}
		
		//TODO: Delete these functions
		void printTr(){
			if(TransitionProbability == NULL){
				std::cout<<"There is no Transition Probability Matrix.\n";
				return;
			}
			
			std::cout<<"TR Prob ("<<_NStates<<","<<_NStates<<","<<_NActions<<"):\n";
			for(int s = 0; s < _NStates; s++){
				std::cout<<"State: "<<s<<"\n";
				for(int i = 0; i < _NStates; i++){
					std::cout<<"[";
					for(int j = 0; j < _NActions; j++){
						std::cout<<TransitionProbability[s][i][j]<<" ";
					}
					std::cout<<"]\n";
				}
			}
		}
		
		void printOb(){
			if(ObservationProbability == NULL){
				std::cout<<"There is no Observation Probability Matrix.\n";
				return;
			}
			
			std::cout<<"OB Prob ("<<_NObservations<<","<<_NStates<<","<<_NActions<<"):\n";
			for(int o = 0; o < _NObservations; o++){
				std::cout<<"Observation: "<<o<<"\n";
				for(int i = 0; i < _NStates; i++){
					std::cout<<"[";
					for(int j = 0; j < _NActions; j++){
						std::cout<<ObservationProbability[o][i][j]<<" ";
					}
					std::cout<<"]\n";
				}
			}
		}
};


class POMDP : public POSMDP {
	private:
		double* _belief;
		
	public:
		POMDP(){
			_NStates = _NActions = _TStates = _prevAction = _prevObservation = 0;
			TransitionProbability = ObservationProbability = NULL;
			_belief = NULL;
			srand(time(NULL));
		}
		
		POMDP(int NStates, int resolution){
			_NStates = _TStates = NStates;
			_prevAction = _prevObservation = 0;
			TransitionProbability = ObservationProbability = NULL;
			_belief = NULL;
			srand(time(NULL));
				
			//Add states
			BeliefState *b;
			for(int i = 0; i < NStates; i++){
				b = new BeliefState(resolution);
				//MacroStates.push_front(*b);
				MacroStates[i] = b;
			}
		}
		
		POMDP(int NStates, int resolution, int NActions){
			_NStates = _TStates = NStates;
			_NActions = NActions;
			_NObservations = resolution;	//TODO: Change?
			_prevAction = _prevObservation = 0;
			srand(time(NULL));
			
			//Add states and initialize Transition Probability matrix
			_belief = new double[_NStates];
			ObservationProbability = new double**[_NObservations], TransitionProbability = new double**[_NStates];
			
			for(int o = 0; o < _NObservations; o ++){
				ObservationProbability[o] = new double*[_NStates];
				
				for(int s = 0; s < _NStates; s++){
					ObservationProbability[o][s] = new double[_NActions];
					
					for(int a = 0; a < _NActions; a++){
						ObservationProbability[o][s][a] = (double) 1 / _NObservations;
					}
				}
			}
			
			for(int s = 0; s < _NStates; s++){
				TransitionProbability[s] = new double*[_NStates];
				_belief[s] = (double) 1 / _NStates;
				
				for(int ss = 0; ss < _NStates; ss++){
					TransitionProbability[s][ss] = new double[_NActions];
					
					for(int a = 0; a < _NActions; a++){
						TransitionProbability[s][ss][a] = (double) 1 / _NStates;
					}
				}
			}	
			
			BeliefState *b;
			for(int i = 0; i < _NStates; i++){
				b = new BeliefState(resolution);
				//MacroStates.push_front(*b);
				MacroStates[i] = b;
			}
			
			//Fill with NActions options			
			for(int i = 0; i < _NActions; i++){
				Option o;
				//Options.push_front(o);
				Options[i] = &o;
			}
			
			mu = MacroPolicy(_NStates, _NActions);
		}
		
		~POMDP(){}
		
		//Override
		MacroState* currentState(){
			//Return state with maximum belief
			double max_belief = _belief[0];
			int pos = 0;
			
			for(int s = 1; s < _NStates; s++){
				if(_belief[s] > max_belief){
					max_belief = _belief[s];
					pos = s;
				}
			}
			
			return (MacroState*)MacroStates[pos];
		}
		
		void beliefUpdate(){
			double sum_T = 0, sum_O = 0;
			
			//For each state
			for(int s_prime = 0; s_prime < _NStates; s_prime++){
				sum_T = sum_O = 0;
				
				//Calculate Sum_s'{Obs(o|s',a)}				//TODO: Merge fors?
				for(int s = 0; s < _NStates; s++){ 
					sum_O += ObservationProbability[_prevObservation][s][_prevAction];
				}
				
				//Calculate Sum_s'{T(s'|s,a)b(s)}
				for(int s = 0; s < _NStates; s++){
					sum_T += TransitionProbability[s_prime][s][_prevAction] * _belief[s];
				}
								
				//Update the belief of state s
				_belief[s_prime] = ( ObservationProbability[_prevObservation][s_prime][_prevAction] * sum_T ) / (sum_O * sum_T);	//TODO: FIX SUM_T??
			}
		}
		
		//Return the Value Function V, for the current policy mu, for each belief state
		double* V(){
			double d = 0.0;
			return &d;
		}
		
		//Return the Value Function V, for some policy nu, for each belief state
		double* V(MacroPolicy nu){
			double d = 0.0;
			return &d;
		}
		
		//Return the Expected Reward J, for the current policy mu, for each belief state
		double* J(){
			double d = 0.0;
			return &d;
		}
		
		//Return the Expected Reward J, for some policy nu, for each belief state
		double* J(MacroPolicy nu){
			double d = 0.0;
			return &d;
		}
		
		double getBeliefState(int stateId){ 
			return _belief[stateId];
		}
		
		double getBeliefState(MacroState *state){return 0;}
};


class SMDP : public POSMDP {
	private:
		int _currOption;
		
	public:
		SMDP(){
			_NStates = _NActions = _TStates = _currOption = 0;
			TransitionProbability = ObservationProbability = NULL;
			srand(time(NULL));
		}
		
		SMDP(int NStates, int NActions){
			_NStates = NStates;
			_NActions = NActions;
			_currOption = 0;
			TransitionProbability = ObservationProbability = NULL;
			srand(time(NULL));
			
			//Add states
			for(int i = 0; i < _NStates; i++){
				MacroState m;
				//MacroStates.push_front(m);
				MacroStates[i] = &m;
			}
			
			//Add options
			for(int i = 0; i < _NActions; i++){
				Option o;
				//Options.push_front(o);
				Options[i] = &o;
			}
			
			mu = MacroPolicy(_NStates, _NActions);
		}
		
		~SMDP(){}
		
		int getNextAction(int s){
			return ((Option*)Options[_currOption])->getNextAction(s);
		}
};

class MDP : public POSMDP {
	private:
	
	public:
		MDP(){
			_NStates = _NActions = _TStates = 0;
			TransitionProbability = ObservationProbability = NULL;
			_prevAction = _prevObservation = _NObservations = 0;
			_currentStateId = 0;
			srand(time(NULL));
			
			//Add an empty MacroState
			MacroState m;
			//MacroStates.push_front(m);
			MacroStates[0] = &m;
			
			//Add an empty Option
			Option o;
			//Options.push_front(o);
			Options[0] = &o;
		}
		
		MDP(int NStates, int NActions){
			_NStates = NStates;
			_NActions = NActions;
			_NObservations = 0;
			ObservationProbability = NULL;
			_prevAction = _prevObservation = 0;
			_currentStateId = 0;
			srand(time(NULL));
			
			//Add states
			for(int i = 0; i < _NStates; i++){
				MacroState m;
				//MacroStates.push_front(m);
				MacroStates[i] = &m;
			}
			
			//Add actions
			for(int i = 0; i < _NActions; i++){
				Option o;
				//Options.push_front(o);
				Options[i] = &o;
			}
			
			//If neither _NStates nor _NActions is zero
			if(_NStates * _NActions != 0){
				mu = MacroPolicy(_NStates, _NActions);
			}
			
			for(int s = 0; s < _NStates; s++){
				TransitionProbability[s] = new double*[_NStates];
				
				for(int ss = 0; ss < _NStates; ss++){
					TransitionProbability[s][ss] = new double[_NActions];
					
					for(int a = 0; a < _NActions; a++){
						TransitionProbability[s][ss][a] = (double) 1 / _NStates;
					}
				}
			}	
		}
		
		//Return current state
		State* currentState(){
			
			return MacroStates[_currentStateId];
		}
		
		int getNextAction(){
			return getNextOption();
		}
		
		int getActuationId(Action* act){
			return 0; //Options[act];
		}
		
		//Returns stateId
		int addState(State* s){
			//MacroStates.front().addState(s);
			//_NStates++;
			MacroStates[_NStates++] = s;
			updateTrObProbs();
			
			//Return stateId
			return _NStates;
		}
		
		//Returns actionId
		int addActuation(Actuation* act){
			Options[_NActions++] = act;
			
			//Update Probability Matrices
			updateTrObProbs();
			
			//return actionId
			return _NActions;
		}
		
		//Adds a link between fromStateId and toStateId, by taking action actionId
		void addLink(int fromStateId, int toStateId, int actionId, double prob = 1){					//TODO: Need a good definition of stateId and actionId!
			//prob should belong to [0,1]
			if(0 > prob || prob > 1){
				prob = 0;
			}
			
			if(0 > fromStateId || fromStateId > _NStates || 0 > fromStateId || fromStateId > _NStates){
				std::cout<<"ERROR! Invalid fromStateId("<<fromStateId<<") or toStateId("<<toStateId<<") or actionId("<<actionId<<")\n";
				return;
			}
			
			//Check for TransitionProbability matrix
			if(TransitionProbability == NULL){
				std::cout<<"Cannot add link, there is no Transition Probability matrix. Did you forget to add actions?\n";
				return;
			}
			
			for(int s = 0; s < _NStates; s++){
				TransitionProbability[fromStateId][s][actionId] = (double)(1-prob)/_NStates;
			}
			
			TransitionProbability[fromStateId][toStateId][actionId] = prob;
		}
		
		//Delets a link between fromStateId and toStateId, by taking action actionId
		void deleteLink(int fromStateId, int toStateId, int actionId){
			//Check for TransitionProbability matrix
			if(TransitionProbability == NULL){
				std::cout<<"ERROR! Cannot delete link, there is no Transition Probability matrix.\n";
				return;
			}
			
			double prob = TransitionProbability[fromStateId][toStateId][actionId];
			
			TransitionProbability[fromStateId][toStateId][actionId] = 0;
			
			//Assign probability mass to the rest of the states
			for(int s = 0; s < _NStates; s++){
				TransitionProbability[fromStateId][s][actionId] += (double)(prob)/_NStates;
			}
		}
		
		~MDP(){}
};

class HMDP : public POSMDP {
	private:
	
	public:
		HMDP(){
			_NStates = _NActions = _TStates = 0;
			TransitionProbability = ObservationProbability = NULL;
			_prevAction = _prevObservation = _NObservations = 0;
			_currentStateId = 0;
			srand(time(NULL));
		}
		
		HMDP(int NStates, int NActions){
			_NStates = _TStates = NStates;
			_NActions = NActions;
			TransitionProbability = ObservationProbability = NULL;
			_prevAction = _prevObservation = _NObservations = 0;
			_currentStateId = 0;
			srand(time(NULL));
			
			//Add states
			for(int i = 0; i < _NStates; i++){
				MacroState m;
				//MacroStates.push_front(m);
				MacroStates[i] = &m;
			}
			
			//Add actions
			for(int i = 0; i < _NActions; i++){
				Option o;
				//Options.push_front(o);
				Options[i] = &o;
			}
			
			mu = MacroPolicy(_NStates, _NActions);
		}
		
		~HMDP(){}
		
		int getNextAction(){
			return getNextOption();
		}
		
		//Adds an MDP to the HMDP, by creating a new MacroState
		void addMDP(MDP m){
			//If the MDP has macro states
			if(!m.getMacroStates().empty()){
				std::cout<<"In Macro States\n";
				//Retrieve macro states from the MDP				
				MacroStates.insert(m.getMacroStates().begin(), m.getMacroStates().end());	//TODO: be careful of duplicate stateId!	-- have a unique id for each state?
				std::cout<<"DONE\n";

				updateTrObProbs();
			}
		}
		
		//Adds an HMDP to the HMDP, by creating a new MacroState
		void addHMDP(HMDP h){
			//If the MDP has macro states
			if(!h.getMacroStates().empty()){
				MacroStates.insert(h.getMacroStates().begin(), h.getMacroStates().end());	//TODO: be careful of duplicate stateId!	-- have a unique id for each state?
				
				updateTrObProbs();
			}
		}
};

class FSM : public POSMDP {
	private:
	
	public:
		FSM(){
			_NStates = _NActions = _TStates = _NObservations = 0;
			TransitionProbability = ObservationProbability = NULL;
			_prevAction = _prevObservation = 0;
			_currentStateId = 0;
			
			//Add an empty MacroState
			MacroState m;
			//MacroStates.push_front(m);
			MacroStates[0] = &m;
			
			//Add an empty Option
			Option o;
			//Options.push_front(o);
			Options[0] = &o;
		}
		
		FSM(int NStates, int NActions){
			_NStates = NStates;
			_NActions = NActions;
			TransitionProbability = ObservationProbability = NULL;
			_prevAction = _prevObservation = _NObservations = 0;
			_currentStateId = 0;
			
			//Add states
			for(int i = 0; i < _NStates; i++){
				MacroState m;
				//MacroStates.push_front(m);
				MacroStates[i] = &m;
			}
			
			//Add actions
			for(int i = 0; i < _NActions; i++){
				Option o;
				//Options.push_front(o);
				Options[i] = &o;
			}
			
			mu = MacroPolicy(_NStates, _NActions);
		}
		
		~FSM(){}
		
		//Return current state
		State* currentState(){

			return MacroStates[_currentStateId];
		}
		
		int getNextAction(){
			return getNextOption();
		}
		
		void addState(State *s){
			//MacroStates.front().addState(s);
			//_NStates++;
			MacroStates[_NStates++] = s;
			updateTrObProbs();
		}
		
		//Adds a link between fromStateId and toStateId, by taking action actionId
		void addLink(int fromStateId, int toStateId, int actionId){					//TODO: Need a good definition of stateId and actionId!
			if(0 > fromStateId || fromStateId > _NStates || 0 > fromStateId || fromStateId > _NStates){
				std::cout<<"ERROR! Invalid fromStateId("<<fromStateId<<") or toStateId("<<toStateId<<") or actionId("<<actionId<<")\n";
				return;
			}
			
			//Check for TransitionProbability matrix
			if(TransitionProbability == NULL){
				std::cout<<"ERROR! Cannot add link, there is no Transition Probability matrix. Did you forget to add actions?\n";
				return;
			}
			
			for(int s = 0; s < _NStates; s++){
				TransitionProbability[fromStateId][s][actionId] = 0;
			}
			
			TransitionProbability[fromStateId][toStateId][actionId] = 1;
		}
		
		//Delets a link between fromStateId and toStateId, by taking action actionId
		void deleteLink(int fromStateId, int toStateId, int actionId){
			//Check for TransitionProbability matrix
			if(TransitionProbability == NULL){
				std::cout<<"ERROR! Cannot delete link, there is no Transition Probability matrix.\n";
				return;
			}
			
			TransitionProbability[fromStateId][toStateId][actionId] = 0;
		}
		
		//Deterministically transition from _currentState to wherever actionId links
		void transition(int actionId){		
			//Find link
			for(int s = 0; s < _NStates; s++){
				if(TransitionProbability[_currentStateId][s][actionId] == 1){
					_currentStateId = s;
					return;
				}
			}
		}
		
		//Returns actionId
		int addActuation(Actuation* act){
			Options[_NActions++] = act;
			
			//return actionId
			return _NActions;
		}
};

*/