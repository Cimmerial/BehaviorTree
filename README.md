# BehaviorTree

**Coded on November 17, 2024**  
An implementation of behavior trees in C# for use with Unity. While designed for Unity, the variable names and nodes are flexible enough to adapt to other applications. Behavior trees allow for modular and reusable logic for AI decision-making, making it easier to piece together **states**, **substates**, and **conditions**. The system also supports prioritization of tasks, acting as an organized alternative to a series of `if-else` statements.  

---

## Motivation
- This behavior tree implementation was created to serve as the "brain" for the AI in my game, **BlackWinter**.
- Unlike Finite State Machines (FSMs) or Hierarchical State Machines (HSMs), behavior trees scale better with increasing complexity.  
- I chose behavior trees over Goal-Oriented Action Planning (GOAP) as they offer similar functionality while being more straightforward for the needs of my game.

---

## Features
1. **Modularity**: Nodes can be easily added, reused, or customized to create complex decision-making logic.
2. **Scalability**: Designed to handle increasing complexity, making it suitable for AI with multiple states and tasks.
3. **Priority-Based Execution**: Nodes are executed based on their assigned priority, allowing for efficient decision-making.
4. **Ease of Use**: Provides a clean structure for defining behaviors, conditions, and actions, making the AI logic more readable and maintainable.

---

## Details
- See the `Example.cs` file to understand how a behavior tree is set up.

---

## Developer Notes
- This was written for **BlackWinter**, a game that I am developing. You can see the project (to some extent) here:  
  [BlackWinter GitHub Repository](https://github.com/Cimmerial/BlackWinter-Public.git)
