/*
 *
 * Copyright (C) Carnegie Mellon University - All Rights Reserved.
 * Unauthorized copying of this file, via any medium is strictly prohibited.
 * This is proprietary and confidential.
 * Written by members of the ArticuLab, directed by Justine Cassell, 2014.
 * 
 * Author: Alexandros Papangelis, apapa@cs.cmu.edu
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMind
{
    public enum INMIND_TASK_GOAL { NONE = 0, READ_EMAIL, REPLY_EMAIL, DELETE_EMAIL, READ_NEWS, TG_ITEMS };

    class Goal
    {
    }

    class SocialGoal : Goal{ }

    class TaskGoal : Goal { }

    class GoalTree {
        private bool _isRoot;
        private Goal _goal;
        private List<GoalTree> _children;

        public GoalTree() { _isRoot = false; }

        public GoalTree(bool isRoot) { _isRoot = isRoot; }

        public GoalTree(Goal goal) { _goal = goal; }

        public GoalTree(bool isRoot, Goal goal)
        {
            _isRoot = isRoot;
            _goal = goal;
        }

        //TODO: RECURSIVELY DELETE?
        ~GoalTree() { }

        public Goal getGoal() { return _goal; }

        public List<GoalTree> getChildren() { return _children; }

        public void setGoal(Goal goal) { _goal = goal; }

        public void setChildren(List<GoalTree> children) { _children = children; }

        public void addChild(GoalTree child)
        {
            _children.Add(child);
        }
    }
}
