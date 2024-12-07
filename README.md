# Simulator_GOAP

This repository hosts a Unity-based simulation project that demonstrates **Goal-Oriented Action Planning (GOAP)** in action. The simulation allows users to experiment with various algorithms for decision-making and behavior planning within a dynamic environment.

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

## Experimenting with Algorithms

### Approach 1: Algorithm Selection

1. Locate the `Scripts` folder in the Unity Project Explorer.
2. Inside, you'll find scripts implementing different algorithms for behavior and planning:
   - `Algorithm1.cs`: Implements a heuristic-based planning approach.
   - `Algorithm2.cs`: Uses a probabilistic decision-making strategy.
   - `Algorithm3.cs`: Integrates reinforcement learning concepts for planning.
3. To try a different algorithm:
   - Open the `AgentController` script in Unity.
   - Swap the active algorithm by referencing the desired script in the appropriate function or variable.
   - Save the changes and rerun the simulation to see the effects of the new algorithm.

### Approach 2: Parameter Tuning

1. Open the script for the chosen algorithm in Unity (e.g., `Algorithm1.cs`).
2. Modify key parameters such as:
   - Planning heuristics
   - Search depth
   - Decision thresholds
3. Save your changes and play the simulation to observe the effects on agent behavior.

### Approach 3: Scenario Customization

1. Modify the Unity scene to create different scenarios for the agents to interact with:
   - Add or remove objects in the scene.
   - Change agent goals and priorities.
   - Introduce obstacles to test adaptability.
2. Save your scene and play the simulation to observe agent responses.

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

Enjoy exploring the possibilities of Goal-Oriented Action Planning with **Simulator_GOAP**!
```
