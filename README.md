## Getting Started

1. Clone the repository
2. Open SQL Server and restore the provided `.bak` file from the `Database` folder
3. Name the restored database exactly `DVLD_02`
5. Open the solution in Visual Studio and run
6. On first launch the app will prompt you to set up credentials automatically
7. Use the default admin account to get started:
   - Username: `Admin_1`
   - Password: `123123123`


## Technical Highlights
First License Issuance — The Heart of the System
The most intricate part of DVLD is not the tests themselves, but the entire first license issuance workflow. 
Before proceeding, the system checks:
	1.	The applicant does not already hold an active license of the same category
	2.	The applicant has no existing active application in progress
	3.	The applicant meets the minimum age requirement for that license class


Only then does the applicant proceed through 3 sequential tests:
	1.	Vision Test
	2.	Written Test
	3.	Street Driving Test
Each test is tracked independently with its full pass/fail history, number of attempts, and retake costs. 
A license is only issued when all 3 are passed and every condition is met.
Security That Goes the Extra Mile
Most apps stop at password hashing. DVLD doesn’t:
	∙	Passwords are hashed before being stored in the database
	∙	The Windows Registry handles session and login state management
	∙	The registry-stored password is protected with asymmetric cryptography, keeping credentials secure even at the OS level


## Pure ADO.NET
No Entity Framework, no shortcuts:
	∙	Every database call is handwritten using ADO.NET
	∙	All operations go through stored procedures
	∙	Full explicit control over every query and transaction

