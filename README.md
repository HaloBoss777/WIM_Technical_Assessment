# Optimal Cell Tower Frequency Assignment

This project was developed as part of a technical assessment for WIM Technologies to evaluate a candidate's potential in software development. The task involved creating an application to optimally assign frequencies to a range of cell towers, aiming to minimize interference between them.

## Features

- **Dynamic Input**: The application allows users to input any number of cell towers along with frequency ranges.
- **Graph Representation**: The cell tower network is visualized as a graph, illustrating distances between towers.
- **Frequency Assignment Algorithm**: An algorithm assigns frequencies while considering the closest \(N\) towers to minimize interference.
- **Distance Calculation**: Utilizes the Haversine formula for accurate distance measurements between towers, accounting for the Earth's curvature.

## How It Works

1. **Input Data**: Users provide cell tower data in a standardized format (ID, coordinates) and specify frequency ranges.
2. **Graph Creation**: A graph is constructed where towers are vertices, and distances between them are weighted edges.
3. **Frequency Assignment**: The algorithm determines the optimal frequency for each tower based on proximity and available frequencies.

## Technology Stack

- **Programming Language**: C#
- **Libraries Used**:
  - [Microsoft.Msagl](https://www.nuget.org/packages/Microsoft.Msagl): For graph creation and visualization.
  - [Ookii.Dialogs.WinForms](https://www.nuget.org/packages/Ookii.Dialogs.WinForms): For dynamic file selection.
- **Distance Calculation**: Haversine formula.

## Results

The project successfully addressed the task by providing a working solution for optimal frequency assignment.

## Visuals

- Graph visualizations and frequency assignments are demonstrated in the application.
- Detailed calculations and consistency checks are documented in the project's accompanying report.
