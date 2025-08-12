The architecture is mostly based around the "Clean Structure" architecture-with the Business Logic layer being independant, and all other layers(usually DB and UI) being dependant on it. Specifically, the DB layer's dependency on Business Logic is done by the Business Logic layer defining an interface to dictate the DB layer's required functionality, and the DB layer implementing it.

In reference to the MVVM structure-the Business Logic layer implements the Models(data objects/entities) and most of the ViewModels, the rest of the ViewModels implemented in the DB layer, and the main WPF project serves as the View component.

In terms of seperation of projects-most of the Business Logic is in the BusinessLogic project, with the implementation of the DB layer in the FileDbManager project and the view/UI in the Task Manager project. Since the LogManager is more of a utility class, it was seperated to another project so it can be used from every project.
The DB layer's ORM is implemented as a *.json file, and injected from the main window when initializing the main ViewModel.

UI-wise, the program only has 2 windows-a main one to display the list of tasks and manipulate them, and a sub-window for the editing/adding of tasks, one at a time. The sub-window is opened by clicking the "New/Edit Task" buttons in the main window, with the "Edit" button requiring the user to select a task before clicking it.

Running the project should be fairly intuitive-the data loads immediately at startup, adding/editing tasks is done through another window that opens by clicking the "Add/Edit Task" buttons, and same goes for deleting, saving, exporting the tasks, changing their priority/status or filtering which ones are shown.
There are 2 things I think need explaining:
1: serial addition-when adding new tasks, the user can click the "Next" button instead of done, saving the need to re-open the window to add the next task. After the last task is done the user can use the "Done" button to finally add them all to the list.
2: keyboard shortcuts-in order to make the working process more streamlined, I added an keyboard shortcuts for the main window:
Ctrl+N to add a new task
Ctrl+Enter to edit the current selected task
Delete to delete the current seelcted task
Ctrl+Space to toggle the current selected task's "Done" status
Ctrl+Up/Down to raise/lower the current selected task's priority
Ctrl+S to save
Escape to close the window

Similar shortcuts were added to the task editing window-Ctrl+Enter as a stand-in for the "Done" button, and Escape for the "Cancel" button

Sadly, the PDF export is the only part I'm unable to test at the moment. I tried multiple methods and left in place the one that seemed to be the most effective, but any and all methods I could find were only compatible with earlier frameworks, which seem to no longer be supported.
