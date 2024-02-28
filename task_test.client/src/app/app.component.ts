import { Component } from '@angular/core';
import { ApiService } from './load-service';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  model: any = { userEmail: '' };
  errorMessageforFile: string = '';
  errorMessageforEmail: string = '';
  resultMessage: string = '';
  constructor(private http: HttpClient) { }

  onSubmit() {
    // Validate email
    const emailPattern = /^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$/;
    if (!emailPattern.test(this.model.userEmail)) {
      this.errorMessageforEmail = 'Invalid email address';
      this.triggerClearEmail(); // Call the function to trigger email input 

      return;
    } else {
      this.errorMessageforEmail = '';
    }

    // Remove the data URL prefix
    const base64Data = this.model.fileD.split(',')[1];

    // Construct the request body as a plain JavaScript object
    const requestBody = {
      userEmail: this.model.userEmail,
      file: base64Data
    };

    // Convert the object to a JSON string
    const jsonBody = JSON.stringify(requestBody);

    // Set the headers for JSON content
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    // Send the POST request with the JSON body
    this.http.post('filemanager/uploadbytes', jsonBody, { headers }).subscribe(
      (response) => {
        console.log('File uploaded successfully:', response);
        this.resultMessage = 'Uploader request sent successfully! Please, wait on email';
        
      },
      (error) => {
        console.error('Error uploading file:', error);
        this.errorMessageforFile = 'Error uploading file';
        // Handle error
        this.triggerFileInputClick(); // Call the function to trigger file input click
        
      }
    );
  }

  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    const reader = new FileReader();

    // Check file extension
    const fileExtension = file.name.split('.').pop();
    if (fileExtension !== 'docx') {
      this.errorMessageforFile = 'Invalid file format. Only .docx files are allowed';
      return;
    } else {
      this.errorMessageforFile = '';
    }

    reader.onloadend = () => {
      this.model.fileD = reader.result as string;
    };

    if (file) {
      reader.readAsDataURL(file);
    }

  }
  // Function to trigger file input click
  private triggerFileInputClick() {
    const fileInput = document.getElementById('fileD') as HTMLInputElement;
    fileInput.value = '';  // Clear the selected file to allow triggering change event again
    fileInput.click();

  }
  // Function to trigger file input click
  private triggerClearEmail() {
    const emailInput = document.getElementById('userEmail') as HTMLInputElement;
    emailInput.value = '';

  }
}
