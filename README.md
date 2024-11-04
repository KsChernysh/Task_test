File Upload and Processing Web Application
This project enables users to upload files to a web application, which then securely stores the files in Azure Blob Storage. The main purpose of the application is to provide a seamless experience for users who need to upload documents and receive immediate feedback.

Key Features
File Upload: Users can easily upload files through a user-friendly interface.
Azure Blob Storage Integration: Once a file is uploaded, it is stored in Azure Blob Storage, ensuring scalability and security.
Azure Function Trigger: An Azure Function with a blob trigger automatically processes the uploaded file as soon as it is stored in the blob. This function handles the logic necessary to manage file processing.
User Notification: After the file is successfully uploaded, the user receives a notification confirming the upload, along with a link to download the file.
Use Case
This application is ideal for scenarios where users need to upload files, such as documents or images, with the assurance that their files are securely stored and easily accessible for later retrieval.
