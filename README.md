 # **Disclaimer** 
 This project is not intended for personal use as it was made for learning purposes.
 It was written more than two years ago when I started learning WPF. 
 The code is a mess.
 I came back to this project because it's a good candidate for refactoring and this is something I want to understand better.
 
 # Introduction
 
 Visual Programming Editor is a node based editor that tries to mimic the way you write code in Unreal Engine's 4 Blueprints editor.
 It features an infinite canvas where you can place nodes and connect their input and output pins to create logically connected sequences of actions.
 It pretty much functions like a procedural programming language, where you chain function calls starting from an entry point. Each function has its own entry point (input) and exit point (output) and can call other functions and declare and use variables to store temporary data.
 
 The UI code is written in WPF using the MVVM pattern. The theme and window and dialog controls comes from the MahApps library. 
 
 ![](https://i.imgur.com/UAcIlWr.png)
 
 ![](https://i.imgur.com/i4HrPjt.png)
 
 ![](https://i.imgur.com/ET3Prl3.png)
 
 # How to use
 
 ## The Start Page
 When the application is first launched, it will display the Start Page which has options for creating a new project or opening an existing one. It also displays the most recent used projects.
 
 ![](https://i.imgur.com/u07Z3xv.png)
 
 ## The Project Explorer
 Here you can add, remove or rename files or folders by right clicking on them.
 You can open files by double clicking on them or from the context menu. The file will open in a new tab using a custom editor for that file type.
 
 ## The Canvas
 Creating a new graph file will open in a custom editor which contains a canvas.
 The canvas can be navigated by holding the right mouse button and dragging.
 Canvas elements can be selected by holding the left mouse button and dragging or by clicking an item holding the CTRL button to add or remove it from the selection.
 Nodes can be created using the Actions Menu made visible by clicking the right mouse button. 
 Most of the elements show a context menu when you right click on them.
 
 # How to execute a graph
 https://github.com/miroiu/NodeBasedEditor/issues/1
 
 