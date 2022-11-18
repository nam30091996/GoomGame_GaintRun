# Example list

## Example 1(Basic FSM)

Basic state machine usage example.
ArborFSM of MainMenu object performs menu transition using OnGUI.

### Contents

* Input
    * Any Key
	  Example of transition by any input
	  (AnyKeyDownTransition, AnyKeyTransition)

	* Key
	  Example of transition by keyboard input
	  (KeyDownTransition, KeyUpTransition)

	* Mouse Button
	  Example of transition by mouse button input
	  (MouseButtonDownTransition, MouseButtonUpTransition)

* Time
  Example of transition over time
  (TimeTransition)

* Collision
  Example of transition by OnTriggerEnter event, OnTriggerExit event
  (InstantiateGameObject, OnTriggerEnterTransition, OnTriggerExitTransition, OnTriggerEnterDestroy)

* Trigger
  Example of transition by sending and receiving trigger message between FSMs
  (SendTrigger, TriggerTransition)



## Example 2(OnMouse)

Example of a state machine that uses the OnMouse event.

### Contents

* MouseEnter/Exit
  Example of transition by OnMouseEnter event, OnMouseExit event
  (OnMouseEnterTransition, OnMouseExitTransition)

* MouseDown/Up
  Example of transition by OnMouseDown event, OnMouseUp event
  (OnMouseDownTransition, OnMouseUpTransition)



## Example 3(EventSystems)

An example state machine using EventSystems.

### Contents

* Left Cube (PointerDown/Up)
  Example of transition by OnPointerDown、OnPointerUp event
  (OnPointerDownTransition, OnPointerUpTransition)

* Center Cube (PointerEnter/Exit)
  Example of transition by OnPointerEnter、OnPointerExit event
  (OnPointerEnterTransition, OnPointerExitTransition)

* Right Cube (PointerClick)
  Example of transition by OnPointerClick event
  (OnPointerClickTransition)



## Example 4(UI)

Example of a state machine using UnityUI.

### Contents

* Button
  Make transition to button click and set text to Text.
  It also uses UITween to perform appearing animation and click animation.
  (UIButtonTransition, UISetText, UITweenSize, UITweenColor, UITweenPosition)

* Toggle
  Perform transition by switching Toggle.
  (UIToggleTransition, UISetText)

* Slider
  Make a transition according to the value of Slider.
  (UISliderTransition, UISetText)



## Example 5(ParameterContainer)

An example of transition conditioning by parameter using ParameterContainer.

### Contents

* ParameterContainer
  Owns an int type Counter parameter.

* ArborFSM
  The Counter parameter is increased from 0 by 1 to complete Once reached 10 or more.
  (CalcParameter, ParameterTransition, UISetTextFromParameter)



## Example 6(GlobalParameterContainer)

An example of handling parameters that are not discarded even if the scene is changed using GlobalParameterContainer.

### Contents

* GlobalParameterContainer
  Holds a reference to the shared ParameterContainer prefab.

* ArborFSM
  An example via GlobalParameterContainer in Container reference of CalcParameter.
  (CalcParameter, UISetTextFromParameter, TweenCancasGroupAlpha)



## Example 7(DataFlow)

Example of performing data input / output by data flow.

### Contents

* DataFlowExampleData.cs
  Script that defines the self-made structure DataFlowExampleData.
  InputSlot and OutputSlot are also defined together to handle self-made structures in data flow.

* DataFlowExampleBehaviour.cs
  StateBehaviour script that displays the value of DataFlowExampleData received from the input slot in Text.

* DataFlowExampleNewDataCalculator.cs
  Calculator script that outputs new DataFlowExampleData.
  Each field of data uses FlexibeString and FlexibleInt, and is an example of setting an input value from a constant or data flow.

* ArborFSM object
  An example of displaying data received from InputSlot and Calculator which creates a self-made structure.
  (DataFlowExampleBehaviour, DataFlowExampleNewDataCalculator, Random.RangeInt)



## Example 8(Variable)

Example of adding a custom data type to ParameterContainer.

### Contents

* VariableExampleDataVariable.cs
  Example of generation of Variable for ParameterContainer by Variable Generator.

* VariableExampleBehaviour.cs
  An example StateBehaviour script that displays a value using the generated FlexibleVariableExampleData type.

* VariableExampleSetNameCalculator.cs
  An example Calculator script that handles self-made Variables in a data flow using the generated InputSlotVariableExampleData and OutputSlotVariableExampleData types.

* ParameterContainer object
　An example of using generated VariableExampleData as a parameter.

* ArborFSM object
  An example of a state machine that uses a custom Variable.
  (VariableExampleBehaviour, VariableExampleSetNameCalculator, GetParameter)



## Example 9(DataLink)

An example of a new function that uses DataLink attributes to input from a data flow.

### Contents

* DataLinkExampleData.cs
  Script that defines the self-made structure DataLinkExampleData.
  We also define OutputSlot to handle self-made structures in data flow.

* DataLinkExampleBehaviour.cs
  StateBehaviour script that displays the value of DataLinkExampleData set using DataLink attribute in Text.

* DataLinkExampleNewDataCalculator.cs
  Calculator script that outputs new DataLinkExampleData.
  Each field of data uses DataLink attribute, and is an example of setting input value from constant or data flow.

* ArborFSM object
  An example of displaying a data received from a data flow with a calculator that creates a self-made structure.
  (DataLinkExampleBehaviour, DataLinkExampleNewDataCalculator, Random.RangeInt)



## Example 10(Animator)

An example of linking Animator's state machine with ArborFSM.

### Contents

* Animator Parameter Controller
  Simple AnimatorController that only transitions by Trigger parameter

* Animator object
  An example of linking Animator component and ArborFSM.
  (CalcAnimatorParamerter, AnimatorStateTransition)



## Example 11(Agent)

An example of moving a character using NavMeshAgent.

### Contents

* PatrolAgent object
  A character that rouses around the initial position.
  (AgentPatrol)

* FollowAgent object
  A character that follows the player.
  (AgentFollow)

* EscapeAgent object
  A character that escapes from the player.
  (AgentEscape)

* Patrol-FollowAgent object
  Follow the player within 5 meters with the player. Otherwise it roams around the initial position.
  (AgentPatrol, AgentFollow, DistanceTransition)

* Patrol-EscapeAgent object
  Escape from the player within 5 meters of the player. Otherwise it roams around the initial position.
  (AgentPatrol, AgentEscape, DistanceTransition)

* Waypoint-FollowAgent object
  Follow the player within 5 meters with the player. Otherwise, move on Waypoint.
  (AgentMoveOnWaypoint, AgentFollow, DistanceTransition)



## Example 12(RaycastFollow)

An example of using Raycast to determine the object that NavMeshAgent follows.

### Contents

* Patrol-EscapeAgent object
  Escape from RaycastFollowAgent within 3 meters with RaycastFollowAgent. Otherwise it roams around the initial position,
  (AgentPatrol, AgentEscape, DistanceTransition)

* RaycastFollowAgent object
  Raycast from your own position while moving on Waypoint, and follow if there is a hit.
  (AgentMoveOnWaypoint, RaycastTransition, RaycastHitTransformCalculator, AgentFollow, AgentMoveToPosition)



## Example 13(BT Agent)

An example of moving NavMeshAgent using BehaviourTree.

### Contents

* EnemyAnget object
  Follow the player if the distance to the player is less than 5 meters. Otherwise, move on Waypoint.
  (Selector, Sequencer, CalculatorCheck, AgentMoveToTransition, AgentMoveOnWaypoint)



## Example 14(Graph Hierarcy)

Example of graph hierarchy.

### Contents

* RootFSM object
  This ArborFSM has a child BehaviourTree and a child ArborFSM.
  (SubBehaviourTree, SubStateMachine, EndStateMachine)



## Example 15(External Graph)

An example of executing a prefabricated graph as a child graph.

### Contents

* RootFSM object
  This ArborFSM instantiates and executes an external BehaviourTree and ArborFSM.
  (SubBehaviourTreeReference, SubStateMachineReference)

* ExternalFSM prefab
  FSM object instantiated from RootFSM with SubStateMachineReference.

* ExternalBT prefab
  BT object instantiated from RootFSM with SubBehaviourTreeReference.
  From here, also execute External FSM with SubStateMachineReference.



## Example 16(RandomTransition)

Example of random transition with weighted StateLink.

### Contents

* ArborFSM object
  Transition randomly using RandomTransition.
  (RandomTransition)



## Example 17(Coin Pusher)

An example of creating a simple game (coin pusher) using ArborFSM.

### Contents

* Stage object
  Manage the progress of the game using Arbor FSM.
  Start by pressing the Start button, determine the hand-held score and display the result screen.

* ParameterContainer object
  In possession of Score parameters.

* Pusher object
　Perform move processing front and back.
  (TweenRigidbodyPosition)

* Spawner object
  If space key is input, coin is instantiated if there is hand score.

* Saucer object
  If a coin falls, add a score.