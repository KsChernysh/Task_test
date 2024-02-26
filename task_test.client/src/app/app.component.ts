// app.component.ts

import { Component } from '@angular/core';
import { ApiService } from './load-service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  fileUploadModel = { userEmail: '', file: null };

  constructor(private apiService: ApiService) {}

  onFileSelected(event: any): void {
    this.fileUploadModel.file = event.target.files[0];
  }

  onSubmit(): void {
    if (this.fileUploadModel.file && this.fileUploadModel.userEmail) {
      this.apiService.uploadFile(this.fileUploadModel).subscribe(
        response => {
          console.log('File uploaded successfully:', response);
        },
        error => {
          console.error('Error uploading file:', error);
        }
      );
    } else {
      console.warn('Please select a file and enter an email.');
    }
  }
}
