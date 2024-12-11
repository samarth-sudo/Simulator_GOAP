# Simulator for GOAP(Goal Oriented Action Planning)

This repository hosts a Unity-based simulation project that demonstrates **Goal-Oriented Action Planning (GOAP)** in action. The simulation allows users to experiment with various algorithms for decision-making and behavior planning within a dynamic environment.

Tutorial video  : https://drive.google.com/drive/folders/1ouXIhtLaKA3-s3y-6_myBAOuxiwWKBZG?usp=sharing
## Getting Started

### Requirements

1. **Unity Hub**: Install Unity Hub to manage and build Unity projects. [Download here](https://unity.com/download).
2. **Unity Editor**: Use Unity Editor (version 2021.3.x or above recommended) for compatibility with the provided assets.
3. **Hardware Compatibility**:
   - **Apple Silicon**: Pre-built simulator version for M1/M2 Macs.
   - **Intel**: Pre-built simulator version for x86 architecture.

### Setup Instructions

1. Clone this repository to your local machine:
   ```bash
   git clone https://github.com/samarth-sudo/Simulator_GOAP.git
   ```
2. Extract the `sim.zip` file located in the root directory of the repository.
3. Place the extracted files into the desired directory.
4. Open Unity Hub:
   - Click "Open" and select the folder containing this repository's files.
   - Unity will load the project as assets into the editor.
5. Select the appropriate build configuration:
   - Navigate to **Build Settings** in Unity.
   - Choose the correct target platform (Mac or Windows) based on your system.
   - Hit "Play" to run the simulation within Unity Editor.


---

## Features

### Core Framework
- **Agent Interface (`IGOAPAgent`)**:
  Defines essential methods for agents to interact with the GOAP system, including state retrieval (`GetCurrentState`) and operational checks (`IsAlive`).

- **Pathfinding (`GOAPPathing`)**:
  Centralized system for generating optimal paths using GOAP principles.

- **World States (`GOAPWorldState`)**:
  Represents states in the pathfinding process, extended for vehicles and aircraft with specific parameters like position, velocity, and altitude constraints.

- **Goals (`GOAPGoal`, `IGOAPGoal`)**:
  Goals define the direction for agents, providing evaluation metrics (`GetValue`) and conditions for completion (`GetIsGoal`).

---

### Specialized Components
#### World States
- **Vehicle World State (`GOAPWorldState_Vehicle`)**:
  Includes vehicle-specific data like position and velocity.

- **Aircraft World State (`GOAPWorldState_SimpleAircraft`)**:
  Adds logic for altitude limits, climb, and descent rates.

#### Path Requests
- **Vehicle Path Request (`GOAPPathRequest_Vehicle`)**:
  Includes vehicle-specific constraints like speed and collision radius.

- **Aircraft Path Request (`GOAPPathRequest_SimpleAircraft`)**:
  Adds acceleration and altitude-related parameters for aircraft.

#### Goals
- **Movement Goals (`GOAPGoal_MoveTo`)**:
  Direct agents to move toward target positions.

- **Specialized Goals**:
  - `GOAPGoal_FollowTerrain`: Ensures safe altitudes above terrain.
  - `GOAPGoal_MoveTo_Orbit`: Guides agents to orbit a target.
  - `GOAPGoal_MoveTo_Position`: Encourages smooth navigation to a position.

---

### Agent Controllers
- **Base Driver (`GOAPDriver`)**:
  Manages agent navigation and panic recovery modes.

- **Aircraft-Specific Drivers**:
  - `GOAPDriver_DemoFixedWing`: Handles fixed-wing navigation and stall recovery.
  - `GOAPDriver_DemoHelicopter`: Manages helicopter hovering, stopping, and recovery.
  - `GOAPDriver_DemoVtol`: Handles VTOL-specific transitions between hover and forward flight modes.

---

### Action System
- **Modular Actions**:
  Encapsulate agent behaviors such as flying to waypoints, taking off, and landing.

- **Examples**:
  - Fixed-Wing: `DemoAction_FixedWing_FlyToWaypoint`
  - VTOL: `DemoAction_Vtol_TakeOff`, `DemoAction_Vtol_LandAtWaypoint`, and engine controls.

---

### Pilots
- **`DemoFixedWingPilot`**:
  Simulates fixed-wing dynamics with realistic controls for pitch, roll, and throttle.

- **`DemoHelicopterPilot`**:
  Controls helicopter behaviors like hovering and orbiting.

- **`DemoVtolPilot`**:
  Combines fixed-wing and helicopter behaviors for VTOL aircraft, transitioning between hover and forward flight.

---

### Visualization Tools
- **Debug Tools**:
  - `DebugGOAPVisualiser`: Visualizes paths, start points, and goals.
  - `PathRequestVisualiser`: Displays pathfinding requests.
  - `PlanningVisualiser`: Shows explored nodes during the planning process.

---

## Workflow
1. **Initialization**:
   - Agents request paths via their respective drivers.

2. **Pathfinding**:
   - `GOAPPathing` computes paths based on goals and world states.

3. **Execution**:
   - Drivers convert paths into commands for pilots.

4. **Monitoring**:
   - Actions and visualizers track progress and provide feedback.

---

## Building and Running the Simulator

1. Open Unity Hub and load the project.
2. Go to **File > Build Settings**.
3. Select the target platform (Mac or Windows).
4. Click "Build", specify a location for the build output, and wait for Unity to compile the project.
5. Run the compiled application to use the standalone simulator.

## Experimentation Workflow

To maximize your exploration of the simulator:

1. **Setup**: Install Unity Hub, load the project, and configure the simulator for your platform.
2. **Experiment**: Try various algorithms, tune their parameters, and customize the environment.
3. **Analyze**: Observe the impact of each change on agent behavior and decision-making.
4. **Iterate**: Use insights to refine scripts, algorithms, and scenarios further.
5. **Build**: Compile the application to share or run the standalone version of the simulator.

## License

This project is licensed under the [MIT License](LICENSE). Feel free to use, modify, and distribute the software with proper attribution.

---
For Further Querries contact email at  samarths@bu.edu
Enjoy exploring the possibilities of Goal-Oriented Action Planning with **Simulator_GOAP**!

