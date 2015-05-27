/*
 *
 * Copyright (C) Carnegie Mellon University - All Rights Reserved.
 * Unauthorized copying of this file, via any medium is strictly prohibited.
 * This is proprietary and confidential.
 * Written by members of the ArticuLab, directed by Justine Cassell, 2014.
 * 
 * Author: Alexandros Papangelis, apapa@cs.cmu.edu
 */

//TODO: Override GetHashCode where needed!

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMind
{
    //Reference to VARIABLE
    //REFERENCE: http://stackoverflow.com/questions/2980463/how-do-i-assign-by-reference-to-a-class-field-in-c/2982037#2982037
    /*sealed class Ref<T>
    {
        private readonly Func<T> getter;
        private readonly Action<T> setter;
        public Ref(Func<T> getter, Action<T> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }
        public T Value { get { return getter(); } set { setter(value); } }
    }*/

    //Reference to VALUE
    //REFERENCE: http://stackoverflow.com/questions/16110187/c-sharp-good-and-flexible-way-to-pass-value-types-by-reference
    class Ref<T>
    {
        public T Value;
        public Ref()
        {
        }
        public Ref(T value)
        {
            this.Value = value;
        }

        public bool Equals(Ref<T> rhs)
        {
            if (Object.ReferenceEquals(rhs, null))
            {
                return false;
            }
            if (Object.ReferenceEquals(rhs, this))
            {
                return true;
            }
            return this.Value.Equals(rhs.Value);
        }

        /*public override bool Equals(Object obj)
        {
            if (Object.ReferenceEquals(obj, null))
            {
                return false;
            }

            NonVerbalAct daObj = obj as NonVerbalAct;
            if (Object.ReferenceEquals(daObj, null))
            {
                return false;
            }
            return Equals(daObj);
        }*/

        public static bool operator ==(Ref<T> lhs, Ref<T> rhs)
        {
            return lhs.Value.Equals(rhs.Value);
        }

        public static bool operator !=(Ref<T> lhs, Ref<T> rhs)
        {
            return !(lhs == rhs);
        }
    }

    class customTuple<T1, T2> { 
        private Tuple<Ref<T1>, Ref<T2>> _internalTuple;

        public customTuple(T1 item1, T2 item2)
        { 
            _internalTuple = new Tuple<Ref<T1>, Ref<T2>>(new Ref<T1>(item1), new Ref<T2>(item2));
        }

        public T1 Item1 { get { return _internalTuple.Item1.Value; } }
        public T2 Item2 { get { return _internalTuple.Item2.Value; } }

        public static bool operator ==(customTuple<T1, T2> lhs, customTuple<T1, T2> rhs)
        {
            return lhs.Item1.Equals(rhs.Item1) && lhs.Item2.Equals(rhs.Item2);
        }

        public static bool operator !=(customTuple<T1, T2> lhs, customTuple<T1, T2> rhs)
        {
            return !(lhs == rhs);
        }

        /*public override int GetHashCode()
        {
            int hash = Id;
            if (Values != null)
            {
                hash = (hash * 17) + Values.Length;
                foreach (T t in Values)
                {
                    hash *= 17;
                    if (t != null) hash = hash + t.GetHashCode();
                }
            }
            return hash;
        }*/
    }

    class customList<T> : List<T>
    {
        private List<Ref<T>> _internalList = new List<Ref<T>>();
        
        public new int Count { get { return _internalList.Count; } }

        public new void Add(T item) { 
            _internalList.Add(new Ref<T>(item));
        }

        public void Add(Ref<T> item) {
            _internalList.Add(item);
        }

        public new bool Contains(T item)
        {
            return _internalList.Any<Ref<T>>(x => x.Value.Equals(item));
        }

        public bool Contains(Ref<T> item) { 
            return _internalList.Any<Ref<T>>(x => x.Equals(item));
        }
    }

    class TupleComparer<S, A> : IEqualityComparer<Tuple<S, A>>
    {
        public bool Equals(Tuple<S, A> pair1, Tuple<S, A> pair2)
        {
            return pair1.Item1.Equals(pair2.Item1) && pair1.Item2.Equals(pair2.Item2);
        }

        public int GetHashCode(Tuple<S, A> pair)
        {
            return pair.GetHashCode();
        }
    }

    /*class TupleRefComparer<S,A> : IEqualityComparer<Tuple<S, A>> { 
        public bool Equals(Tuple<S, A> pair1, Tuple<S, A> pair2){
            return pair1.Item1.Value.Equals(pair2.Item1.Value) && pair1.Item2.Value.Equals(pair2.Item2.Value);
        }

        public int GetHashCode(Tuple<S, A> pair) { 
            return pair.GetHashCode();
        }
    }*/

    class SparseMatrix<S,A>
    {
        private double DEFAULT_VALUE = -1000;
        //TODO: Modify and use Efficient Hash?
        private Dictionary<Tuple<S, A>, double> _sparseMatrix;    //SxA -> Re
        private TupleComparer<S,A> _comparer = new TupleComparer<S,A>();
        private Dictionary<S, double> _maxUtilities; //used to speed-up action selection, in case multiple actions have the same utility
        //private Efficient64bitHashTable<Tuple<S,A>> _sparseMatrix;    //SxA -> Re

        private List<S> _stateList;
        private List<A> _actionList;

        public SparseMatrix(){
            _sparseMatrix = new Dictionary<Tuple<S, A>, double>();
            _stateList = new List<S>();
            _actionList = new List<A>();
            _maxUtilities = new Dictionary<S,double>();
        }

        public SparseMatrix(List<S> States, List<A> Actions)
        {
            _sparseMatrix = new Dictionary<Tuple<S, A>, double>();
            _maxUtilities = new Dictionary<S, double>();
            _stateList = States;
            _actionList = Actions;

            foreach(S state in States){
                _maxUtilities.Add(state, DEFAULT_VALUE);
            }

            //Populate and initialize policy
            /*foreach(S state in States){
                foreach(A action in Actions){
                    _sparseMatrix[new Tuple<S, A>(state, action)] = DEFAULT_VALUE;
                }
            }*/
        }

        ~SparseMatrix() {
            _sparseMatrix.Clear();
            if (_stateList.Count > 0) { _stateList.Clear(); }
            if (_actionList.Count > 0) { _actionList.Clear(); }
        }

        //Get and Set π(S,A) = value
        public double this[Tuple<S, A> pair]
        {
            get
            {
                //Add pair-value if it does not exist
                if (!_sparseMatrix.ContainsKey(pair))
                {
                    _sparseMatrix.Add(pair, DEFAULT_VALUE);

                    //Update state and action lists as necessary
                    if (!_stateList.Contains(pair.Item1))
                    {
                        _stateList.Add(pair.Item1);
                        _maxUtilities.Add(pair.Item1, DEFAULT_VALUE);
                    }
                    if (!_actionList.Contains(pair.Item2))
                    {
                        _actionList.Add(pair.Item2);
                    }
                }

                return _sparseMatrix[pair];
            }
            set
            {
                //Add pair-value if it does not exist
                if (!_sparseMatrix.ContainsKey(pair))
                {
                    _sparseMatrix.Add(pair, value);
                    
                    //Update state and action lists as necessary
                    if(!_stateList.Contains(pair.Item1))
                    {
                        _stateList.Add(pair.Item1);
                        _maxUtilities.Add(pair.Item1, value);
                    }
                    if (!_actionList.Contains(pair.Item2))
                    {
                        _actionList.Add(pair.Item2);
                    }
                }
                //Else update the pair
                else { 
                    _sparseMatrix[pair] = value; 
                }

                //Update max utility
                if (_maxUtilities[pair.Item1] < value)
                {
                    _maxUtilities[pair.Item1] = value;
                }
            }
        }
        
        //CAUTION: REQUIRES double[] OF dimension _NActions
        //Get π(S,A) of 'state' row
        //Set π(S,A) of 'state' row
        public double[] this[S state]
        {
            get
            {
                double[] values = new double[_actionList.Count];
                int index = 0;

                //For each Action paired with that state
                foreach(A action in _actionList){
                    //if the pair exists
                    Tuple<S, A> pair = new Tuple<S, A>(state, action);

                    if (_sparseMatrix.ContainsKey(pair))
                    {
                        values[index++] = _sparseMatrix[pair];
                    }
                    else { values[index++] = DEFAULT_VALUE; }
                }

                return values;
            }
            set
            {
                int index = 0;

                //For each Action paired with that state
                foreach (A action in _actionList)
                {
                    //if the pair exists
                    Tuple<S, A> pair = new Tuple<S, A>(state, action);

                    if (_sparseMatrix.ContainsKey(pair))
                    {
                        _sparseMatrix[pair] = value[index];
                    }
                    index++;
                }
            }
        }

        //CAUTION: REQUIRES double[] OF dimension _NStates
        //Get π(S,A) of 'action' column
        //Set π(S,A) of 'action' column
        public double[] this[A action]
        {
            get
            {
                double[] values = new double[_stateList.Count];
                int index = 0;

                //For each Action paired with that state
                foreach (S state in _stateList)
                {
                    //if the pair exists
                    Tuple<S, A> pair = new Tuple<S, A>(state, action);

                    if (_sparseMatrix.ContainsKey(pair))
                    {
                        values[index++] = _sparseMatrix[pair];
                    }
                    else { values[index++] = -1; }
                }

                return values;
            }
            set
            {
                int index = 0;

                //For each Action paired with that state
                foreach (S state in _stateList)
                {
                    //if the pair exists
                    Tuple<S, A> pair = new Tuple<S, A>(state, action);

                    if (_sparseMatrix.ContainsKey(pair))
                    {
                        _sparseMatrix[pair] = value[index];
                    }
                    index++;
                }
            }
        }

        public Dictionary<Tuple<S, A>, double>.Enumerator GetEnumerator() { return _sparseMatrix.GetEnumerator(); }

        public int getNStates() { return _stateList.Count; }

        public int getNActions() { return _actionList.Count; }

        public List<S> getStateList() { return _stateList; }

        public List<A> getActionList() { return _actionList; }

        public double getStateMaximum(S state) {
            if (_maxUtilities.ContainsKey(state))
            {
                return _maxUtilities[state];
            }
            else { 
                return DEFAULT_VALUE;
            }
        }

        public Dictionary<Tuple<S, A>, double> getPolicy() { return _sparseMatrix; }

        public void addState(S state)
        {
            if (!_stateList.Contains(state))
            {
                _stateList.Add(state);
                _maxUtilities.Add(state, DEFAULT_VALUE);
            }
        }

        public void addAction(A action)
        {
            if (!_actionList.Contains(action))
            {
                _actionList.Add(action);
            }
        }
    }
}
