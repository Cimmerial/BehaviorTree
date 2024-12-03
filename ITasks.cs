
namespace BlackWinter.BehaviorTrees
{
    //===================================================================================================================
    // BASE INTERFACE
    //===================================================================================================================
    public interface ITask
    {
        BT_Node.Status GetStatus();
        void Reset() { }
    }

    //===================================================================================================================
    // SUBCLASSES
    //===================================================================================================================
    /// <summary>
    /// Attaches to leaf node, executes action and returns success when GetStatus() called.
    /// </summary>
    public class ActionTask : ITask
    {
        // ACTION
        readonly Action action;
        public ActionTask(Action action)
        {
            this.action = action;
        }
        // CLASS FUNCTION
        public BT_Node.Status GetStatus()
        {
            action();
            return BT_Node.Status.SUCCESS;
        }
    }

    /// <summary>
    /// Runs primary action one time, then runs secondary action in the background until the duration of said task has 
    /// been completed.
    /// </summary>
    public class OneUseActionTask : ITask
    {
        // VARS
        readonly Action oneUseAction;
        readonly Action backgroundAction;
        readonly float timeUntilActionCompleted;
        public float timeElapsed;
        public bool oneUseActionBeenUsed;
        public OneUseActionTask(Action oneUseAction, Action backgroundAction, float timeUntilActionCompleted)
        {
            this.oneUseAction = oneUseAction;
            this.backgroundAction = backgroundAction;
            this.timeUntilActionCompleted = timeUntilActionCompleted;
            oneUseActionBeenUsed = false;
            timeElapsed = 0;
        }
        // CLASS FUNCTION
        public BT_Node.Status GetStatus()
        {
            timeElapsed += Time.deltaTime;
            // RESET AND RETURN
            if (timeElapsed > timeUntilActionCompleted)
            {
                timeElapsed = 0;
                oneUseActionBeenUsed = false;
                return BT_Node.Status.SUCCESS;
            }
            // RUN ONE USE ACTION
            if (!oneUseActionBeenUsed)
            {
                oneUseActionBeenUsed = true;
                oneUseAction();
            }
            backgroundAction();

            return BT_Node.Status.RUNNING;
        }
    }

    /// <summary>
    /// Has a function run in the background, then has fail and start functions which run and return when their 
    /// respective conditions pass. The "startAction" runs once when the Task begins.
    /// </summary>
    public class ActionTaskSuccessFailBackground : ITask
    {
        // VARS
        readonly Func<bool> successConditon;
        readonly Func<bool> failConditon;
        readonly Action successAction;
        readonly Action failAction;
        readonly Action startAction;
        readonly Action backgroundAction;
        public bool startActionBeenRan;
        public ActionTaskSuccessFailBackground(Func<bool> successConditon, Func<bool> failConditon, Action successAction, Action failAction, Action startAction, Action backgroundAction)
        {
            this.successConditon = successConditon;
            this.failConditon = failConditon;
            this.successAction = successAction;
            this.failAction = failAction;
            this.startAction = startAction;
            this.backgroundAction = backgroundAction;
            startActionBeenRan = false;
        }
        // CLASS FUNCTION
        public BT_Node.Status GetStatus()
        {
            // RUN START ACTION ONCE
            if (!startActionBeenRan)
            {
                startActionBeenRan = true;
                startAction();
            }
            // CHECK FOR SUCCESS AND FAIL
            if (successConditon())
            {
                successAction();
                startActionBeenRan = false;
                return BT_Node.Status.SUCCESS;
            }
            else if (failConditon())
            {
                failAction();
                startActionBeenRan = false;
                return BT_Node.Status.SUCCESS;
            }
            // RUN BACKGROUND ACTION
            backgroundAction();
            return BT_Node.Status.RUNNING;
        }
    }

    /// <summary>
    /// Is for requiring a given ActionTask to meet this Condition's condition before being able to start. Its GetStatus is 
    /// a simple boolean check.
    /// </summary>
    public class Condition : ITask
    {
        // CONDITION
        readonly Func<bool> condition;
        public Condition(Func<bool> condition) => this.condition = condition;
        // CLASS FUNCTION
        public BT_Node.Status GetStatus() => condition() ? BT_Node.Status.SUCCESS : BT_Node.Status.FAILURE;
    }
}