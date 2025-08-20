Design Approach
The solution is divided into four layers, each with it's own responsibility.
I have chosen this architecture to ensure a separation of concerns, making the codebase easier to maintain and extend,
following SOLID principles , it is also the infrasturcture I have been working recently (clean code architecture).

The app will set up 3 default courses on startup  :
"English For Beginners", 
"Mathematic Fundamentals"
"Afrikaans Fundamentals"

I have created a admin super user login which will alow the user to create users , edit courses ,delete courses and assign courses to a user
The login credentials are : 
            UserName = "admin@admin.com",
            Password = "Admin123!",

A user can also just register as a student from the login screen and they will be automatically logged in.
They can sign out or in , enroll for a course and deregister from a course

In terms of CRUD operations created:
- Create : User registration
- Read : View courses, view enrolled courses
- Update : Admin users can update existing courses or existing user profiles
- Delete : Deregister from a course

I have also added unit tests for testing of the user workflows and endpoints.

How to run the application :

Option 1
What Users Need:
Prerequisites: .NET 8.0 SDK installed
Action: Just double-click start-app.bat
Result: Both projects start automatically

Option 2 
open solution in visual studio
there is a arrow next to start click that and select configure start up projects
once in here click on multiple startup projects
than set StudentCourseEnrollmentApp.API to start
and than set StudentCourseEnrollmentApp.UI to start
Click apply and ok
Now click play and the app will start up automatically