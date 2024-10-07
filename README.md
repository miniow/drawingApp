Drawing App - Graphic Primitives and Canvas

Welcome to Drawing App, a lightweight and creative tool designed to help you draw, modify, and interact with basic geometric shapes such as lines, rectangles, and circles directly on the canvas. This project is perfect for anyone looking to explore the fundamentals of graphics programming and UI interactions using WPF in C#. Dive in and start drawing!

Features

Our Drawing App offers a variety of features for interacting with graphic primitives:

Drawing Three Basic Primitives: Users can draw lines, rectangles, and circles effortlessly using two different methods:

Mouse Input: Define key points of your shapes by clicking on the canvas.

Text Input: Use the provided text fields to specify dimensions and parameters of each shape.

Intuitive Control of Shapes:

Modify Shapes with Mouse: Click and drag to move, resize, or modify your shapes directly on the canvas. Grab the edges or characteristic points of a shape to adjust its size or position.

Modify Shapes Using Text Fields: Select a shape from the list, and use text fields to adjust its parameters. Shape dimensions and properties update in real time.

Mouse Interaction:

Draw Shapes with Clicks: Define key points of shapes with mouse clicks to place and modify lines, rectangles, and circles easily.

Drag to Move Shapes: Click and drag shapes to reposition them anywhere on the canvas.

Serialization & Deserialization:

Save Your Work: Serialize all the drawn shapes into a JSON file so you can keep your work and open it again later.

Load Your Work: Load a previously saved drawing, and continue from where you left off. Serialization ensures that all shapes, sizes, colors, and positions are saved accurately.

How to Use

Select the Shape Type: Choose between Line, Rectangle, or Circle from the control panel on the left.

Pick a Color: Select the desired color for your shape.

Draw on Canvas:

Click and drag on the canvas to draw the selected shape.

Alternatively, input shape parameters in the provided text boxes and press the "Draw Shape" button.

Modify or Delete Shapes:

Click a shape to select it and make adjustments using either mouse interaction or text fields.

Click "Delete Shape" to remove the selected shape.

Save and Load Shapes:

Use the "File" menu to save all the shapes to a .json file or load shapes from a previously saved file.

Installation and Setup

Clone the Repository:

Open in Visual Studio: Open the solution file (.sln) in Visual Studio.

Build and Run: Build the solution and start the application to begin drawing.

Technologies Used

.NET Framework / .NET Core: To create a powerful and flexible WPF desktop application.

WPF (Windows Presentation Foundation): For creating the user interface and handling graphical elements.

C#: The primary programming language for implementing app logic.

JSON: Used for serialization and deserialization of drawn shapes.

Future Improvements

Additional Primitives: Add more complex shapes such as polygons, bezier curves, etc.

Undo/Redo Functionality: Add an undo/redo stack to allow users to reverse changes.

Layer Management: Allow users to manage shapes on different layers for more complex drawings.

Advanced Styling: Include options for line styles (dashed, dotted) and shape fills.

Contributing

Feel free to contribute to this project by submitting pull requests. Whether it's adding new features, fixing bugs, or improving the UI, all contributions are welcome!

License

This project is licensed under the MIT License. Feel free to use and modify it as you see fit.

Contact

If you have any questions or suggestions, please feel free to reach out!

Happy drawing! 🎨
