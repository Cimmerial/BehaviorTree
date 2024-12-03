using System.Collections.Generic;
using System.Linq;

namespace BlackWinter.BehaviorTrees
{
    //===================================================================================================================
    // BASE CLASS
    //===================================================================================================================
    /// <summary>
    /// Base node which store the name, priority, Status, and children of any given subclass for a behavior trees, outlines 
    /// GetStatus() and Reset()
    /// </summary>
    public class BT_Node
    {
        // VARS
        public string name;
        public int priority;
        public enum Status { SUCCESS, RUNNING, FAILURE }
        // CONSTRUCTOR
        public BT_Node(string name = "Unnamed Node", int priority = 0)
        {
            this.priority = priority;
            this.name = name;
        }
        // CHILDREN
        public List<BT_Node> children = new();
        protected int currentChild;
        public void AddChild(BT_Node child) => children.Add(child);
        // CLASS FUNCTIONS
        /// <summary>
        /// Returns the Status of this nodes children based on the currentChild index. Returns Task status if leaf node.
        /// </summary>
        public virtual Status GetStatus() => children[currentChild].GetStatus();
        /// <summary>
        /// Resets current child to 0 on this node, and recursively to any children of this node.
        /// </summary>
        public virtual void Reset()
        {
            currentChild = 0;
            foreach (BT_Node child in children) child.Reset();
        }
    }

    //===================================================================================================================
    // SELECTORS
    //===================================================================================================================
    /// <summary>
    /// Randomly selects child with equal chance for all.
    /// </summary>
    public class RandomSelector : PrioritySelector
    {
        // CONSTRUCTOR
        public RandomSelector(string name = "Unnamed Random Selector") : base(name) { }
        // CHILDREN
        protected override List<BT_Node> SortChildren() => children.ShuffleList().ToList();
    }
    /// <summary>
    /// Similar to the Selector node, but instead it iterated through its children based on their assigned priority and 
    /// returns the first child to have a Status of RUNNING or SUCCESS.
    /// </summary>
    public class PrioritySelector : Selector
    {
        // CONSTRUCTOR
        public PrioritySelector(string name = "Unnamed PrioritySelector", int priority = 0) : base(name, priority) { }
        // CHILDREN
        List<BT_Node> sortedChildren;
        public List<BT_Node> SortedChildren => sortedChildren ??= SortChildren();
        protected virtual List<BT_Node> SortChildren() => children.OrderByDescending(child => child.priority).ToList();
        // CLASS FUNCTIONS
        public override void Reset()
        {
            base.Reset();
            sortedChildren = null;
        }
        // RETURN FIRST CHILD STATUS TO NOT FAIL
        public override Status GetStatus()
        {
            foreach (var child in SortedChildren)
            {
                switch (child.GetStatus())
                {
                    case Status.RUNNING:
                        // Debug.Log($"{name} retured child: {child.name} Status.RUNNING with priority: {child.priority}");
                        return Status.RUNNING;
                    case Status.SUCCESS:
                        // Debug.Log($"{name} retured child: {child.name} Status.SUCCESS with priority: {child.priority}");
                        Reset();
                        return Status.SUCCESS;
                    default:
                        // Debug.Log($"{name} FAILURE continue: {child.name} with priority: {child.priority}");
                        continue;
                }
            }
            Reset();
            return Status.FAILURE;
        }
    }
    /// <summary>
    /// Only fails when all children fail, goes down the child list in order, runs GetStatus() and returns the given child 
    /// Status which is passed to the Selector parent if it is RUNNING or SUCCESS.
    /// </summary>
    public class Selector : BT_Node
    {
        // CONSTRUCTOR
        public Selector(string name = "Unnamed Selector", int priority = 0) : base(name, priority) { }
        // RETURN FIRST CHILD STATUS TO NOT FAIL
        public override Status GetStatus()
        {
            if (currentChild < children.Count)
            {
                switch (children[currentChild].GetStatus())
                {
                    case Status.RUNNING:
                        return Status.RUNNING;
                    case Status.SUCCESS:
                        Reset();
                        return Status.SUCCESS;
                    default:
                        currentChild++;
                        return Status.RUNNING;
                }
            }
            Reset();
            return Status.FAILURE;
        }
    }
    //===================================================================================================================
    // SEQUENCES
    //===================================================================================================================
    /// <summary>
    /// Only succeeds when all children succeed, goes down the child list in order, runs GetStatus() and returns the given 
    /// child Status which is passed to the Selector parent if it is RUNNING or FAILURE.
    /// </summary>
    public class Sequence : BT_Node
    {
        // CONSTRUCTOR
        public Sequence(string name = "Unnamed Sequence", int priority = 0) : base(name, priority) { }
        // CLASS FUNCTIONS
        public override Status GetStatus()
        {
            if (currentChild < children.Count)
            {
                switch (children[currentChild].GetStatus())
                {
                    case Status.RUNNING:
                        // Debug.Log($"{name} return Status.RUNNING");
                        return Status.RUNNING;
                    case Status.FAILURE:
                        // Debug.Log($"{name} return Status.FAILURE and call Reset()");
                        // Reset(); 
                        currentChild = 0;
                        return Status.FAILURE;
                    default:
                        currentChild++;
                        // Debug.Log($"{name} Inc child so currentChild = {currentChild} and children.Count = {children.Count}");
                        return currentChild == children.Count ? Status.SUCCESS : Status.RUNNING;
                }
            }
            // Debug.Log($"Reset and Return SUCCESS from {name}, currentChild = {currentChild} and children.Count = {children.Count}");
            Reset();
            return Status.SUCCESS;
        }
    }
    //===================================================================================================================
    // TREE AND LEAF
    //===================================================================================================================
    /// <summary>
    /// Basically acts as the root node, will recursively iterate through all children via GetStatus(), only moves to the 
    /// next child when the current child returns SUCCESS, resets the whole tree and starts the process over when all 
    /// children have Status SUCCESS
    /// </summary>
    public class BT_Tree : BT_Node
    {
        // CONSTRUCTOR
        public BT_Tree(string name = "Unnamed Tree", int priority = 0) : base(name, priority) { }
        // CLASS FUNCTIONS
        public override Status GetStatus()
        {
            // INFINITELY LOOP
            children[currentChild].GetStatus();
            currentChild = (currentChild + 1) % children.Count;
            return Status.RUNNING;
        }
    }
    /// <summary>
    /// Has no children, instead runs a task which returns a Status based on the completion status of said task. 
    /// </summary>
    public class BT_Leaf : BT_Node
    {
        // INTERFACE
        readonly ITask task;
        // CONSTRUCTOR
        public BT_Leaf(string name, ITask task, int priority = 0) : base(name, priority)
        {
            this.task = task;
        }
        // CLASS FUNCTIONS
        public override Status GetStatus() => task.GetStatus();
        public override void Reset() => task.Reset();
    }
}