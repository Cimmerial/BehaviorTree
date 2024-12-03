using BlackWinter.BehaviorTrees;

// VARS
protected BT_Tree TREE;

/*
 * Example of a behavior tree setup (not a complete implementation).
 *
 * "MAKE" functions are used instead of inlining the code to maintain a clear, hierarchical tree structure for readability.
*/

/// <summary>
/// A simplified behavior tree (BT) setup. 
/// The "MAKE", "CONDITION," and "ACTION" functions are placeholders in the example.
/// See "MAKE_IdleSequence" below for an example of a "MAKE" function.
/// </summary>
protected override void SetupBT()
{
    TREE = new BT_Tree;
    TREE = MAKE_NewTree(
    "Test Tree",
    MAKE_NewPrioritySelector(0,
        "TREE_PSEL",
        MAKE_IdleSequence(0),
        MAKE_PatrolSequence(1),
        MAKE_AStarPathfindSequence(2),
        MAKE_NewSequence(6,
            "SPECIALS_SEQ",
            MAKE_SpecialAbilitySequence()
        ),
        MAKE_NewSequence(3,
            "FOLLOW_SEQ",
            new BT_Leaf("FOLLOW_CONDITION_SEQ", new Condition(() => FOLLOW_CONDITION())),
            MAKE_NewPrioritySelector(0,
                "TEST_FOLLOW_PSEL",
                MAKE_TestFollowFastSequence(0)
                MAKE_TestFollowSlowSequence(1)
        )))
    );
}

/// <summary>
/// Example of "MAKE" function which combines given "CONDITION" and "IDLE" functions together into a Sequence (similar 
/// to a logical AND)
/// </summary>
protected Sequence MAKE_IdleSequence(int priority = 0)
{
    Sequence IDLE_SEQ = new("IDLE_SEQ", priority);
    IDLE_SEQ.AddChild(new BT_Leaf("IDLE_CONDITION_L", new Condition(() => IDLE_CONDITION())));
    IDLE_SEQ.AddChild(new BT_Leaf("IDLE_L", new ActionTask(() => IDLE())));
    return IDLE_SEQ;
}

/// <summary>
/// Example of an incomplete "CONDITION" function. Will check for X, Y, and Z to be true.
/// </summary>
protected bool IDLE_CONDITION()
{
    return true;
}

/// <summary>
/// Example of an incomplete "ACTION" function. 
/// </summary>
protected void IDLE()
{
    // noop
}
