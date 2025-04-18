<!DOCTYPE html>
<html lang="en">
<head>
</head>
<body>
    <header>
        <h1>Optimal Cell Tower Frequency Assignment</h1>
    </header>
    <main>
        <section>
            <p>This project was developed as part of a technical assessment for WIM Technologies to evaluate a candidate's potential in software development. The task involved creating an application to optimally assign frequencies to a range of cell towers, aiming to minimize interference between them.</p>
            <p>The following input was provided, along with a frequency range of one hundred and ten (N = 110) to one hundred and fifteen (N = 115):</p>
            <img src="Original_Information.png" alt="Original Information" style="max-width:100%; height:auto;">
        </section>
        <section>
            <h2>Features</h2>
            <ul>
                <li><strong>Dynamic Input:</strong> The application allows users to input any number of cell towers along with frequency ranges.</li>
                <li><strong>Graph Representation:</strong> The cell tower network is visualized as a graph, illustrating distances between towers.</li>
                <li><strong>Frequency Assignment Algorithm:</strong> An algorithm assigns frequencies while considering the closest <i>N</i> towers to minimize interference.</li>
                <li><strong>Distance Calculation:</strong> Utilizes the Haversine formula for accurate distance measurements between towers, accounting for the Earth's curvature.</li>
            </ul>
        </section>
        <section>
            <h2>How It Works</h2>
            <ol>
                <li><strong>Input Data:</strong> Users provide cell tower data in a standardized format (ID; Easting; Northing; Longitude; Latitude) and specify frequency ranges.</li>
                <li><strong>Graph Creation:</strong> A graph is constructed where towers are vertices, and distances between them are weighted edges.</li>
                <li><strong>Frequency Assignment:</strong> The algorithm determines the optimal frequency for each tower based on its proximity to other towers and the available frequencies. Afterwards, the frequency assignment for each tower is displayed.</li>
                <li><strong>Visuals:</strong> After the frequency assignments and distance calculations, the full Cell Tower Network graph is illustrated along with a matrix containing distance information for each Cell Tower.</li>
            </ol>
        </section>
        <section>
            <h2>Technology Stack</h2>
            <ul>
                <li><strong>Programming Language:</strong> C#</li>
                <li><strong>Libraries Used:</strong>
                    <ul>
                        <li><a href="https://www.nuget.org/packages/Microsoft.Msagl">Microsoft.Msagl</a>: For graph creation and visualization.</li>
                        <li><a href="https://www.nuget.org/packages/Ookii.Dialogs.WinForms">Ookii.Dialogs.WinForms</a>: For dynamic file selection.</li>
                    </ul>
                </li>
                <li><strong>Distance Calculation:</strong> Haversine formula.</li>
            </ul>
        </section>
        <section>
            <h2>Results</h2>
            <p>The project successfully addressed the task by providing a working solution for optimal frequency assignment.</p>
        </section>
        <section>
            <h2>Visuals</h2>
              <ul>
                  <li>Graph visualizations and frequency assignments are demonstrated in the application. Below is an example graph:</li>
                  <img src="Graph_Visual.JPG" alt="Cell Tower Network" style="max-width:100%; height:auto;">
                  <li>Detailed calculations and consistency checks are documented in the project's accompanying report. Below is an example of the distance matrix:</li>
                  <img src="Distance_Matrix.png" alt="Distance Matrix" style="max-width:100%; height:auto;">
                  <li>Illustration of the frequency assignments, considering the closest 4 and 5 towers before assignment:</li>
                  <img src="Frequency_Graphs.png" alt="Frequency Graph" style="max-width:100%; height:auto;">
              </ul>
        </section>
    </main>
</body>
