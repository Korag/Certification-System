# Certification-System

Certification-System is a powerful web application designed for vocational training centres. Certification-System provides more than 100 unique functionalities that allow for comprehensive management of all aspects of running a vocational training business. In addition to the managerial aspect, the main purpose of the application is to provide functionalities for the verification of certificates of competence and professional degrees issued by the vocational centre using the application. Certification-System is designed for all types of business service beneficiaries, both individuals and companies. The designed functionalities made available to the platform users include the following groups of issues related to:

+ user document generation
+ competence verification system
+ additional functionalities of the administrator
+ event logging system
+ basic user-system interactions
+ course management system
+ degree management system
+ certificate management system
+ examination system

The Certification-System is built using ASP.NET Core and Razor Pages. Application data is stored in a non-relational MongoDB database, and access to this data is done using the MongoDB Driver. The database is located in the MongoDB Atlas service. AutoMapper was used for entity mapping. User authentication and authorization is provided by ASP.NET Core Identity along with a provider that allows the MongoDB database to be used as a data store. The Bootstrap framework was used to style the UI. To generate QR codes of resources the QR Coder library was used, whereas the possibility of printing the content of individual panels of the application is provided by the printThis.js package.

The application provides the following functionalities: 

1. Additional administrator functionalities:
+ creation of user accounts
+ editing of user accounts
+ deleting of user accounts
+ displaying and filtering users
+ manual sending of automated emails
+ creating employer accounts
+ assigning employees to a company
+ editing of company data
+ display of companies with associated employees
+ deletion of companies

2. User document generation system:
+ generation of a document confirming that an employee holds a certificate
+ generation of a document confirming possession of a professional degree by an employee
+ generation of employee identifier

3. Competence verification system:
+ anonymous certificate verification
+ anonymous verification of employee competences
+ anonymous verification of the professional grade
+ API endpoint certificate verification
+ API endpoint employee competence verification
+ API endpoint verifications of professional steps
+ certificate verification by administrator
+ verification of employee competences by administrator
+ administrator's verification of professional rank

Either a QR code or a resource identifier is used for verification.

4. Event logging system:
+ logging of execution of any operation causing modification of database content and entity structure
+ logging of the execution of any operation whose results concern the user in any way
+ administrator notification panel
+ employer notification panel
+ user notification panel

5. Basic user-system interactions:
+ login to the application
+ editing personal data
+ setting up a password for a user account for the first time
+ displaying personal data of the logged user
+ changing the password
+ confirming the email address
+ registration of user account
+ resetting the password

6. Course management system:
+ course creation
+ course creation with meetings
+ display of course summary
+ adding a user to the course
+ adding a group of users to the course
+ enrolling an employee in the course
+ enrolling a group of employees in the course
+ editing the course
+ editing the course including meetings
+ deleting a user from the course
+ closing the course
+ listing courses
+ displaying course details
+ deleting a course
+ displaying user results
+ displaying course results
+ displaying results of employer's employees
+ displaying the results of individual course users
+ adding an appointment
+ editing an appointment
+ deleting an appointment
+ list of appointments
+ displaying meeting details
+ checking the presence of users in the meeting

7. Degree management system:
+ create professional degree
+ edit professional degree+ listing of professional degrees
+ display of details of the professional degree
+ deletion of a professional degree
+ listing of professional degrees
+ display of details of the professional degree
+ deletion of a professional degree.
+ awarding a professional degree to the user of the platform
+ list of granted professional degrees
+ deletion of user's professional degree
+ edit the professional degree of the user
+ displaying the details of the professional degree of the user

8. Certificate management system:
+ certificate creation
+ certificate editing
+ certificate listing
+ displaying details of certificate
+ certificate deletion
+ granting the certificate to the user of the platform
+ removing the certificate from the user
+ listing of granted certificates
+ editing of the granted certificate
+ displaying details of the granted certificate

9.Examination system:
+ withdrawal of a participant from an examination
+ enrolling an employee for an examination
+ enrolling a group of employees for an examination
+ evaluating the examination
+ deleting an examination
+ editing an examination
+ editing exam with all sessions
+ deleting a user from an examination
+ deleting a user group from an examination
+ adding exam
+ adding exam with all sessions
+ listing exams
+ displaying exam details
+ displaying total examination results
+ display exam results per user
+ display of examination results for employer's employees
+ display exam results for all exam participants
+ cancel exam session
+ adding exam sessions
+ editing exam sessions
+ marking exam sessions
+ deleting a user from an exam session
+ deleting a user group from an exam session
+ signing up a employee in an examination session
+ signing up a group of employees for an exam session
+ displaying exam session details
+ deleting an examination session
+ listing exam sessions
+ displaying exam session results for the user
+ displaying exam session results for the employer's employee
+ display exam session results for all session participants

Access to particular functionalities is conditioned by the role of the user of the certification system. 
There are the following available roles:
+ worker
+ company
+ examiner
+ instructor
+ instructor-examiner
+ admin

The following diagram shows which groups of functionality specific user roles have access to:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_1.PNG "Overall use-case diagram")

The following design patterns has been implemented in the application:
+ Model View Controller
+ Singleton
+ Dependency Injection
+ Repository

Application main window:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_2.PNG "Application main window")

Email message template:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_3.PNG "Email message template")

Admin user menu:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_4.PNG "Admin user menu")

User data in anonymous resource verification:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_5.PNG "User data in anonymous resource verification")

User data in a resource verification action intended for the administrator:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_6.PNG "User data in a resource verification action intended for the administrator")

User competence verification manual and issued certificates, professional degrees:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_7.PNG "User competence verification manual and issued certificates, professional degrees")

Fragment of the user competence verification panel showing certificates and professional degrees:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_8.PNG "Fragment of the user competence verification panel showing certificates and professional degrees")

Fragment of the user competence verification panel showing some of the courses a person has attended:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_9.PNG "Fragment of the user competence verification panel showing some of the courses a person has attended")

Certificate verification panel - certificate data section:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_10.PNG "Certificate verification panel - certificate data section")

Fragment of the professional degree verification panel showing its details and the certificates and professional degrees required:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_11.PNG "Fragment of the professional degree verification panel showing its details and the certificates and professional degrees required")

API panel:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_12.PNG "API panel")

User identifier generated:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_13.PNG "User identifier generated")

Document certifying that the user possesses certificate of competence:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_14.PNG "Document certifying that the user possesses certificate of competence")

Document certifying that the user possesses professional degree:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_15.PNG "Document certifying that the user possesses professional degree")

Notification panel for the user of an application in the role of an employee:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_16.PNG "Notification panel for the user of an application in the role of an employee")

Notification panel for the application user in the administrator role:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_17.PNG "Notification panel for the application user in the administrator role")

Panel displaying all courses saved in the database:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_18.PNG "Panel displaying all courses saved in the database")

Fragment of the data entry form for a new coursewith associated meetings:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_19.PNG "Fragment of the data entry form for a new coursewith associated meetings")

Panel listing existing meetings:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_20.PNG "Panel listing existing meetings")

A panel to check the attendance of course participants at a meeting:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_21.PNG "A panel to check the attendance of course participants at a meeting")

Attendance register of course participants:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_22.PNG "Attendance register of course participants")

User course enrollment panel:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_23.PNG "User course enrollment panel")

Professional course offerings panel:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_24.PNG "Professional course offerings panel")

Details panel of the professional course offer:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_25.PNG "Details panel of the professional course offer")

Selection panel for adding a new exam:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_26.PNG "Selection panel for adding a new exam")

Exam entity editing form:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_27.PNG "Exam entity editing form")

Evaluation form for papers included in the selected exam:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_28.PNG "Evaluation form for papers included in the selected exam")

Form for manually assigning a user to an exam:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_29.PNG "Form for manually assigning a user to an exam")

Listing of the user's exams in the selected course:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_30.PNG "Listing of the user's exams in the selected course")

Panel for enrolling company employees for the exam:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_31.PNG "Panel for enrolling company employees for the exam")

Certificate data entry form:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_32.PNG "Certificate data entry form")

Panel listing certificates in the system:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_33.PNG "Panel listing certificates in the system")

The form for manually assigning a certificate to a platform user:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_34.PNG "The form for manually assigning a certificate to a platform user")

Awarding certificates through a course closing action:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_35.PNG "Awarding certificates through a course closing action")

A summary of the results achieved by course participants being a part of the closing panel of the course:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_36.PNG "A summary of the results achieved by course participants being a part of the closing panel of the course")

Form for adding a new professional degree:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_37.PNG "Form for adding a new professional degree")

Panel for granting a professional degree to a platform user:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_38.PNG "Panel for granting a professional degree to a platform user")

User login panel:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_39.PNG "User login panel")

Fragment of the user registration form:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_40.PNG "Fragment of the user registration form")

Fragment of the admin form for adding a new user:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_41.PNG "Fragment of the admin form for adding a new user")

Logged user account details panel:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_42.PNG "Logged user account details panel")

Form for setting up a new account password:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_43.PNG "Form for setting up a new account password")

List panel of application users:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_44.PNG "List panel of application users")

Panel listing all companies saved in the database:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_45.PNG "Panel listing all companies saved in the database")

Details panel of the selected company:

![alt text](https://github.com/Korag/DocumentationImages/blob/master/Certification-System/Certification-System_46.PNG "Details panel of the selected company")
